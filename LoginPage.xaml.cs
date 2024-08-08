using System.Windows;
using System.Windows.Controls;

namespace RekamMedisPuskesmas
{
    public partial class LoginPage : Page
    {
        private MainWindow _mainWindow;
        public LoginPage(MainWindow mainWindow)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
        }

        private void Btn_login_Clicked(object sender, System.Windows.RoutedEventArgs e)
        {
            string username = tbx_username.Text;
            string password = pwdbox_password.Password;

            if (IsValidLogin(username, password))
            {
                MessageBox.Show("login Berhasil");
                _mainWindow.NavigateToPage(new IndeksPasien(_mainWindow));
            }
            else
            {
                MessageBox.Show("Login Gagal: Pastikan Username dan Password yang dimasukkan sudah benar!");
            }
        }

        private bool IsValidLogin(string username, string password)
        {
            return username == "admin" && password == "1234";
        }
    }
}
