using codeding.WPF.XamlQuery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace App {
	class Accordion:Grid {
		private double _expanderHeights;
		private Boolean _isRenderedForFirstTime = false;
		protected override void OnRender (DrawingContext dc) {
			base.OnRender(dc);
			if (!this._isRenderedForFirstTime) {
				this._isRenderedForFirstTime = true;
				this._firstRender();
			}
		}
		private void _firstRender () {

			this.ColumnDefinitions.Add(new ColumnDefinition() {
				Width = new GridLength(100, GridUnitType.Star)
			});

			Expander child;
			double h = 0;
			for (int i = 0; i < this.Children.Count; i++) {
				child = (Expander)this.Children[i];

				ControlSet gridBody = XamlQuery.Search(child, "Grid"); // [Name=expanderBody]
				h = ((Grid)gridBody[0]).ActualHeight + 10;

				this.RowDefinitions.Add(new RowDefinition() {
					Height = new GridLength(h, GridUnitType.Pixel)
				});

				Grid.SetRow(child, i);

				// přidani stylu expanderu
				try { 
					child.Style = this.FindResource("accordionExpander") as System.Windows.Style;
				} catch (Exception e) {
				}

				child.Expanded += this._handlerProvider(i, false);
				child.Collapsed += this._handlerProvider(i, true);
			}
			this._expanderHeights = h;

			this._expand(0);
		}

		private RoutedEventHandler _handlerProvider (int i, bool collapsed) {
			return (object o, RoutedEventArgs e) => {
				this._collapseAll();

				if (collapsed && this.Children.Count > 1) {
					int next = i + 1;
					if (next == this.Children.Count) {
						next = i - 1;
					}
					this._expand(next);

				} else {

					this._expand(i);
				}
				this.UpdateLayout(); // kvůli stretch alignu to radej prekresluji
			};
		}
		private void _collapseAll () {
			foreach (var item in this.RowDefinitions) {
				item.Height = new GridLength(this._expanderHeights);
			}
		}
		private void _expand (int i) {
			this.RowDefinitions[i].Height = new GridLength(1, GridUnitType.Star);
			((Expander)this.Children[i]).IsExpanded = true; // automaticky rozbalí expander - okamžitě
		}
	}
}
