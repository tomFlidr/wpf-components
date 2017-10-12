using System;
using System.Collections.Generic;
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

namespace App.Controls {
	/// <summary>
	/// Interaction logic for CloseableHeader.xaml
	/// </summary>
	public partial class CloseableHeader:UserControl {
		public Button CloseBtn {
			get {
				return this.closeBtn;
			}
			set {
				this.closeBtn = value;
			}
		}
		public Label TitleLbl {
			get {
				return this.titleLbl;
			}
			set {
				this.titleLbl = value;
			}
		}
		public CloseableHeader () {
			InitializeComponent();
		}
	}
}
