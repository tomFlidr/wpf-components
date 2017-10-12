using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MainProject {
    
    // speciálnější typ druhého argumentu handleru:
    public class DynamicTreeItemEventArgs: EventArgs {
        public IDynamicTreeClass ModelClassInstance;
    }

    public partial class DynamicTree: UserControl {

        // obyč událost:
        //public event EventHandler TreeItemDoubleClick;
        // speciálnější událost:
        public event TreeItemEventHandler TreeItemDoubleClick;

        // speciálnější typ handleru události:
        public delegate void TreeItemEventHandler (TreeViewItem o, DynamicTreeItemEventArgs e);


        private string _modelClass;

        public string ModelClass {
            get { return _modelClass; }
            set {
                this._modelClass = value;

                // potřebuji vyrobit instanci C: a nacist prvni uroven
                Type modelType = Type.GetType(value.Trim());
                Type interfaceType = typeof(IDynamicTreeClass);
                if (interfaceType.IsAssignableFrom(modelType) == false) {
                    throw new Exception(
                        $"Třída {value} nesplňuje požadavky IDynamicTreeClass."
                    );
                }
                IDynamicTreeClass instance = (IDynamicTreeClass)Activator.CreateInstance(modelType);
                this.LoadSubItems(instance, this.tree.Items);
            }
        }
        public void LoadSubItems (IDynamicTreeClass instance, ItemCollection items) {
            // načíst podelementy:
            List<IDynamicTreeClass> childs = instance.GetChilds(instance.Uid);
            // vyčistit případné nulové děti v uzlu - pro minulé zobrazování trojůhelníčku pro rozevírání:
            items.Clear();
            // projít cšechny načtené podděti:
            foreach (IDynamicTreeClass child in childs) {
                // vytvořit novou vizuelní instanci pro stromeček:
                TreeViewItem node = new TreeViewItem () {
                    Header = child.Header,
                    DataContext = child,
                    Tag = false
                };
                // zkusit také načíst její podděti:
                child.GetChilds(child.Uid);
                // pokud je má - zařídit zobrazení rozevíracího trojůhelníčku:
                if (child.HasChilds) { 
                    node.Items.Add(null); // přidá jeden nulový potomek -> zobrazí se rozevírací trojúhelník
                }
                // přidání události na kliknutí:
                node.MouseDoubleClick += this._nodeDoubleClickHandler;
                node.Expanded += this._nodeExpandedHandler;
                // přidání nové vizuelní instance do cílové kolekce items:
                items.Add(node);
            }
        }
        private void _nodeExpandedHandler (object sender, RoutedEventArgs e) {
            TreeViewItem node = (TreeViewItem)e.OriginalSource;
            //node.DataContext - instance modelové třídy
            //node.Tag - ano/ne - o tom, zda jsem už někdy načítal a vytvářel poduzly
            if (!(bool)node.Tag) {
                node.Tag = true;
                IDynamicTreeClass child = (IDynamicTreeClass)node.DataContext;
                this.LoadSubItems(child, node.Items);
            }
        }
        private void _nodeDoubleClickHandler (object sender, MouseButtonEventArgs e) {
            //TreeViewItem node = (TreeViewItem)e.OriginalSource;
            TreeViewItem node = (TreeViewItem)sender;
            IDynamicTreeClass child = (IDynamicTreeClass)node.DataContext;
            // spustit (odfirovat) vlastní definovanou událost:

            if (this.TreeItemDoubleClick == null) return;

            this.TreeItemDoubleClick.Invoke(
                node, 
                new DynamicTreeItemEventArgs() {
                    ModelClassInstance = child
                }
            );
            e.Handled = true; // TODO
        }

        public DynamicTree () {
            InitializeComponent();

        }

    }
}
