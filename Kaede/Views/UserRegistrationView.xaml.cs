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

namespace Kaede.Views
{
    /// <summary>
    /// Interaction logic for UserRegistrationView.xaml
    /// </summary>
    public partial class UserRegistrationView : UserControl
    {
        public UserRegistrationView()
        {
            InitializeComponent();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                e.Handled = true;
                if (sender == UsernameTextBox)
                    PasswordTextBox.Focus();
                else if (sender == PasswordTextBox)
                    ConfirmPasswordTextBox.Focus();
                else if (sender == ConfirmPasswordTextBox)
                    RegisterButton.Focus();
            }
        }
    }
}
