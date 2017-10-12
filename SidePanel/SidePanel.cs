using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace App {

	public enum SidePanelValues {
		Left,
		Right
	}

	public class SidePanel:DockPanel {

		public SidePanelValues ContentAlign { get; set; }

		private Button _btn;

		private double _originalWidth = 0;
		private bool _initiazed = false;
		private Point _startPos;
		private bool _mouseDown;
		private double _startWidth;

		public SidePanel () : base() {
			this.LastChildFill = true;

			this._btn = new Button();

			//this._btn.Width = 50; // .. pro ucely vyvoje

			// lokalne
			System.Windows.Style style = this.FindResource("sidePanelBtn") as System.Windows.Style;
			//Style style = Application.Current.FindResource("sidePanelBtn") as Style; // globalne
			this._btn.Style = style;

			//this._btn.IsFocused

			this.Children.Add(this._btn);

			this._btn.PreviewMouseDoubleClick += (Object o, MouseButtonEventArgs e) => {
				this._doubleClickHandler();
				e.Handled = true;
			};
			this._btn.PreviewKeyDown += (Object o, KeyEventArgs e) => {
				if (e.Key == Key.Enter) this._doubleClickHandler();
			};

			// událost na click a na najetí myši
			this._btn.PreviewMouseDown += this._downHandler;
			this._btn.PreviewMouseUp += this._upHandler;
			this._btn.PreviewMouseMove += this._moveHandler;

			this._btn.PreviewTouchDown += this._downHandler;
			this._btn.PreviewTouchUp += this._upHandler;
			this._btn.PreviewTouchMove += this._moveHandler;
		}

		// protected override void OnRe -> TAB TAB
		protected override void OnRender (DrawingContext dc) {
			base.OnRender(dc);
			if (!this._initiazed) {
				this._initiazed = true;
				DockPanel.SetDock(
					this._btn,
					this.ContentAlign == SidePanelValues.Left ? Dock.Right : Dock.Left
				);
			}
		}

		private void _doubleClickHandler () {
			// pokud je sirka celeho side panelu vetsi nez button vlevo:
			if (this.ActualWidth > this._btn.Width) {
				// ulož aktualni sirku do promenne napriste
				this._originalWidth = this.ActualWidth;
				// zkolapsuj panel - setnutím do this.Width;
				this.Width = this._btn.Width;
			} else {
				// side panel je zkolapsovvany, nafoukni ho do sirky z predtim
				this.Width = this._originalWidth;
			}
		}

		private void _downHandler (object sender, EventArgs e) {
			this._startWidth = this.ActualWidth;
			this._startPos = Mouse.GetPosition(Application.Current.MainWindow);
			this._mouseDown = true;
		}

		private void _moveHandler (object sender, EventArgs e) {
			if (this._mouseDown) {
				var actualPos = Mouse.GetPosition(Application.Current.MainWindow);
				double difference;
				if (this.ContentAlign == SidePanelValues.Left) {
					difference = Math.Round(this._startPos.X - actualPos.X);
				} else {
					difference = Math.Round(actualPos.X - this._startPos.X);
				}
				var targetWidth = this._startWidth - difference;
				this.Width = targetWidth > this._btn.ActualWidth ? targetWidth : this._btn.ActualWidth;
			}
		}

		private void _upHandler (object sender, EventArgs e) {
			this._mouseDown = false;
		}
	}
}
