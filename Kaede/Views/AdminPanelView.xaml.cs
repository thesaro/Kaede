using Kaede.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Kaede.Views
{
    /// <summary>
    /// Interaction logic for AdminPanelView.xaml
    /// </summary>
    public partial class AdminPanelView : UserControl
    {
        public AdminPanelView()
        {
            InitializeComponent();

            RecruitBarberGrid.DataContext = App.RunningInstance()
                .FetchProviderService<BarberRegistrationViewModel>();
            BarberListGrid.DataContext = App.RunningInstance()
                .FetchProviderService<BarberListingView>();
        }

        private void ToggleButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleButton tb)
            {
                var vis = (tb.IsChecked ?? false) ? Visibility.Visible : Visibility.Collapsed;
                switch (tb.Name)
                {
                    case "BarberListTB":
                        BarberListGrid.Visibility = vis;
                        break;
                    case "RecruitBarberTB":
                        RecruitBarberGrid.Visibility = vis;
                        break;
                    default:
                        break;
                }
            }
        }

        private void StackPanel_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }
    }

    public class PwdHashTruncationConverter : IValueConverter
    {
        public int MaxLength { get; set; } = 10;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return string.Empty;

            string? text = value.ToString();
            return string.Concat(text.AsSpan(0, MaxLength), "...");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
