Imports System.Reflection
Imports System.Threading
Imports System.Windows.Controls
Imports System.Windows.Forms
Imports System.Windows.Input
Imports Libs

Public Class AutoComplete


    Const MODEL_CLASS_GETTER_INTERFACE = "Models:Models.IAutoComplete"

    Public Event Done As EventHandler

    Private Shared _treadIndex As Integer = 100
    Private _focusIndex As Integer = -1

    Private _searchField As String
    Public Property SearchField() As String
        Get
            Return _searchField
        End Get
        Set(ByVal value As String)
            _searchField = value
        End Set
    End Property

    Private _modelClass As Type
    ''' <summary>
    ''' Full.Class.Name or AssemblyName:Full.Class.Name
    ''' </summary>
    Public Property ModelClass() As String
        Get
            Return Me._modelClass.FullName
        End Get
        Set(ByVal value As String)
            Me._modelClass = Tools.GetTypeGlobaly(value)
            Dim interfaceType = Tools.GetTypeGlobaly(AutoComplete.MODEL_CLASS_GETTER_INTERFACE)
            If Not (interfaceType.IsAssignableFrom(Me._modelClass)) Then
                Throw New Exception(
                    String.Format("ModelClass '{0}' doesn't implements {1}.", value, AutoComplete.MODEL_CLASS_GETTER_INTERFACE)
                )
            End If

        End Set
    End Property

    Private _dbConnectionIndex As Integer
    Private _shown As Boolean = False

    Public Property DbConnectionIndex() As String
        Get
            Return Me._dbConnectionIndex.ToString()
        End Get
        Set(ByVal value As String)
            Me._dbConnectionIndex = CInt(value)
        End Set
    End Property

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

        Me.optionsList.Items.Clear()
        Me.optionsList.Visibility = System.Windows.Visibility.Hidden

        AddHandler Me.textField.KeyUp, AddressOf Me.textCHangedHandler

    End Sub

    Private Sub textCHangedHandler(sender As Object, e As System.Windows.Input.KeyEventArgs)

        If (e.Key = Key.Up Or e.Key = Key.Down) And Me.optionsList.Items.Count > 0 Then
            If e.Key = Key.Down Then
                Me._focusIndex += 1
            ElseIf e.Key = Key.Up Then
                Me._focusIndex -= 1
            End If

            If Me._focusIndex < 0 Then
                Me._focusIndex = Me.optionsList.Items.Count - 1
            ElseIf Me._focusIndex > Me.optionsList.Items.Count - 1 Then
                Me._focusIndex = 0
            End If

            Me.optionsList.SelectedIndex = Me._focusIndex
        ElseIf e.Key = Key.Enter Or e.Key = Key.Right Then
            'Tools.DispatchEvent()

            Dim currentItem As ListBoxItem = TryCast(Me.optionsList.SelectedItem, ListBoxItem)

            If (currentItem IsNot Nothing) Then
                Me.textField.Text = currentItem.Content.ToString()

                RaiseEvent Done(currentItem.Content.ToString(), New EventArgs())
            Else
                RaiseEvent Done(Me.textField.Text, New EventArgs())
            End If

        Else

            Dim actualText As String = Me.textField.Text
            AutoComplete._treadIndex += 1
            Dim index As Integer = AutoComplete._treadIndex

            Dim thread As Thread = New Thread(New ThreadStart(Sub()
                                                                  Me._processRequest(actualText, index)
                                                              End Sub))
            thread.Start()

            ' originalni vlakno pokračuje zde...

        End If

    End Sub

    Private Sub _processRequest(actualText As String, index As Integer)

        If Me._modelClass Is Nothing Then Return

        Dim getterInstance As Object = Activator.CreateInstance(Me._modelClass)
        Dim methodInfo As MethodInfo = Me._modelClass.GetMethod("GetAutocomplete")

        Dim dbResult As Object = methodInfo.Invoke(
            getterInstance,
            New Object(2) {actualText, Me._dbConnectionIndex, index}
        )

        Dim subItems As List(Of Model) = TryCast(
            dbResult, List(Of Model)
        )

        If (Dispatcher.CheckAccess()) Then
            Me._completeItems(dbResult, New EventArgs())
        Else
            Dim eh As EventHandler = New EventHandler(AddressOf Me._completeItems)
            Dispatcher.Invoke(eh, dbResult, New EventArgs())
        End If
        ' Me.optionsList.Items

    End Sub

    Private Sub _completeItems(dbResult As Object, eventArgs As EventArgs)
        Dim subItems As List(Of Model) = TryCast(
            dbResult, List(Of Model)
        )

        Me.optionsList.Items.Clear()
        Dim value As Object
        For Each item As Model In subItems

            Dim propInfo As PropertyInfo = Nothing
            Dim propInfos As IEnumerable(Of PropertyInfo) = item.GetType().GetRuntimeProperties()
            For Each pi As PropertyInfo In propInfos
                If pi.Name = Me._searchField Then
                    propInfo = pi
                    Exit For
                End If
            Next
            'If propInfo IsNot Nothing Then
            value = propInfo.GetValue(item)
            'Else
            '    Dim fieldInfo As FieldInfo = Nothing
            '    Dim fieldInfos As IEnumerable(Of FieldInfo) = item.GetType().GetRuntimeFields()
            '    For Each fi As FieldInfo In fieldInfos
            '        If fi.Name = Me._searchField Then
            '            fieldInfo = fi
            '            Exit For
            '        End If
            '    Next
            '    value = fieldInfo.GetValue(item)
            'End If

            Me.optionsList.Items.Add(New ListBoxItem() With {
                .Content = value.ToString()
            })
        Next

        If Me.optionsList.Items.Count > 0 Then
            Me.optionsList.Visibility = System.Windows.Visibility.Visible
        Else
            Me.optionsList.Visibility = System.Windows.Visibility.Hidden
        End If

        Me.optionsList.Height = Me.optionsList.Items.Count * 20


    End Sub
End Class
