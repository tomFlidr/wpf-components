using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace App.Controls {
	public class TabControl:TabItem {
		public MainWindow MainWindow = null;
		public object InitData = null;
		public object Layout = null;
		public string Title {
			get {
				return (this.Header as CloseableHeader).TitleLbl.Content.ToString();
			}
			set {
				(this.Header as CloseableHeader).TitleLbl.Content = value;
			}
		}
		public TabControl () {
			// create closable header
			CloseableHeader header = new CloseableHeader();
			header.CloseBtn.MouseEnter += new MouseEventHandler(this.OnTabTitleMouseEnter);
			header.CloseBtn.MouseLeave += new MouseEventHandler(this.OnTabTitleMouseLeave);
			header.TitleLbl.SizeChanged += new SizeChangedEventHandler(this.OnTabTitleSizeChanged);
			header.CloseBtn.Click += new RoutedEventHandler(this.OnTabClose);
			this.Header = header;
			// initialize layout items into this.Content
		}
		public void InitLayout () {
			// add completed layout into tab content to run render process
			this.Content = this.Layout;
		}
		// Overrides
		// Override OnSelected - Show the Close Button
		protected override void OnSelected (RoutedEventArgs e) {
			base.OnSelected(e);
			(this.Header as CloseableHeader).CloseBtn.Visibility = Visibility.Visible;
		}
		// Override OnUnSelected - Hide the Close Button
		protected override void OnUnselected (RoutedEventArgs e) {
			base.OnUnselected(e);
			(this.Header as CloseableHeader).CloseBtn.Visibility = Visibility.Hidden;
		}
		// Override OnMouseEnter - Show the Close Button
		protected override void OnMouseEnter (MouseEventArgs e) {
			base.OnMouseEnter(e);
			(this.Header as CloseableHeader).CloseBtn.Visibility = Visibility.Visible;
		}
		// Override OnMouseLeave - Hide the Close Button (If it is NOT selected)
		protected override void OnMouseLeave (MouseEventArgs e) {
			base.OnMouseLeave(e);
			if (!this.IsSelected) {
				(this.Header as CloseableHeader).CloseBtn.Visibility = Visibility.Hidden;
			}
		}
		// Custom handlers
		// Button MouseEnter - When the mouse is over the button - change color to Red
		public void OnTabTitleMouseEnter (object sender, MouseEventArgs e) {
			(this.Header as CloseableHeader).CloseBtn.Foreground = Brushes.Red;
		}
		// Button MouseLeave - When mouse is no longer over button - change color back to black
		public void OnTabTitleMouseLeave (object sender, MouseEventArgs e) {
			(this.Header as CloseableHeader).CloseBtn.Foreground = Brushes.Black;
		}
		// Label SizeChanged - When the Size of the Label changes (due to setting the Title) set position of button properly
		public void OnTabTitleSizeChanged (object sender, SizeChangedEventArgs e) {
			(this.Header as CloseableHeader).CloseBtn.Margin = new Thickness((this.Header as CloseableHeader).TitleLbl.ActualWidth + 5, 3, 4, 0);
		}
		// Button Close Click - Remove the Tab - (or raise an event indicating a "CloseTab" event has occurred)
		public void OnTabClose (object sender, RoutedEventArgs e) {
			this.MainWindow.MainTabs.Items.Remove(this);
		}
	}
}
