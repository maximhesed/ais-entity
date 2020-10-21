using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Ais.src
{
    public partial class winInfoTable : Window
    {
        public winInfoTable(Dictionary<string, object> data, string title) {
            Grid grid = new Grid();
            int offset = 0;

            InitializeComponent();

            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.Margin = new Thickness(5);

            foreach (KeyValuePair<string, object> entry in data) {
                Label key = new Label {
                    Content = entry.Key + " ",
                    FontSize = 15,
                    VerticalAlignment = VerticalAlignment.Center
                };
                TextBox value = new TextBox {
                    Text = entry.Value + "",
                    FontSize = 15,
                    IsReadOnly = true,
                    Width = 400,
                    Margin = new Thickness(10, 0, 0, 0),
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalContentAlignment = HorizontalAlignment.Center
                };

                grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(30, GridUnitType.Pixel) });

                Grid.SetRow(key, offset);
                Grid.SetColumn(key, 0);
                grid.Children.Add(key);

                Grid.SetRow(value, offset);
                Grid.SetColumn(value, 1);
                grid.Children.Add(value);

                offset++;
            }

            this.Content = grid;
            this.Title = title + " info";
            this.SizeToContent = SizeToContent.WidthAndHeight;
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            this.UseLayoutRounding = true;
        }
    }
}
