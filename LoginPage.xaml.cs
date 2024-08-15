using System;
using System.Configuration;
using System.Data;
using System.Net.NetworkInformation;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Npgsql;

namespace RekamMedisPuskesmas
{
    public partial class LoginPage : Page
    {
        private MainWindow _mainWindow;
        string connectionString = ConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;
        public LoginPage(MainWindow mainWindow)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
        }

        private void Btn_login_Clicked(object sender, RoutedEventArgs e)
        {
            string username = tbx_username.Text;
            SessionManager.Username = username;
            string password = pwdbox_password.Password;
            Mouse.OverrideCursor = Cursors.Wait;

            if (IsValidLogin(username, password))
            {
                MessageBox.Show("Login Berhasil");
                Mouse.OverrideCursor = null;
                _mainWindow.NavigateToPage(new IndeksPasien(_mainWindow));
            }
            else
            {
                MessageBox.Show("Login Gagal: Pastikan Username dan Password yang dimasukkan sudah benar!");
                Mouse.OverrideCursor = null;
            }
        }

        private bool IsValidLogin(string username, string password)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT id FROM data_login WHERE username = @username AND password = @password";

                    using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@username", username);
                        command.Parameters.AddWithValue("@password", password);

                        var result = command.ExecuteScalar();
                        if (result != null)
                        {
                            // Simpan ID pengguna ke SessionManager
                            SessionManager.UserID = Convert.ToInt32(result);
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error connecting to database: {ex.Message}");
                    return false;
                }
            }
        }


        public static class SessionManager
        {
            public static int UserID { get; set; }
            public static string Username { get; set; }
        }

    }
}