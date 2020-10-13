using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Ais.src
{
    public partial class winInfoTable : Window
    {
        public winInfoTable(Dictionary<string, string> data, string title) {
            StackPanel stackPanel = new StackPanel {
                Margin = new Thickness(30, 15, 30, 15),
                Orientation = Orientation.Vertical
            };

            InitializeComponent();

            foreach (KeyValuePair<string, string> entry in data) {
                stackPanel.Children.Add(new Label {
                    Content = string.Format($"{entry.Key}: {entry.Value}"),
                    FontSize = 15
                });
            }

            this.Content = stackPanel;
            this.Title = title + " info";
            this.SizeToContent = SizeToContent.WidthAndHeight;
        }
    }
}
