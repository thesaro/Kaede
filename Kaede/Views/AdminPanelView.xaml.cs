using Kaede.ViewModels;
using System;
using System.Collections.Generic;
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
        }

        private void ToggleButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleButton tb && (tb.IsChecked ?? false))
            {
                RecruitBarberGrid.Visibility = Visibility.Visible;
            }
            else
            {
                RecruitBarberGrid.Visibility = Visibility.Collapsed;
            }
        }
    }
}
