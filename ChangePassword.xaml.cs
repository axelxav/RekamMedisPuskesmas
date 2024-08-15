using Npgsql;
using System;
using System.Configuration;
using System.Windows;
using System.Windows.Controls;
using static RekamMedisPuskesmas.LoginPage;

namespace RekamMedisPuskesmas
{
    public partial class ChangePassword : Window
    {
        string connectionString = ConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;
        int userID = SessionManager.UserID;
        string Username = SessionManager.Username;
        string pass;
        string Newpass;

        public ChangePassword()
        {
            InitializeComponent();
        }

        private void Btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Btn_Change_Click(object sender, RoutedEventArgs e)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "UPDATE data_login SET password = @newpassword WHERE id = @id";
                    using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@newpassword", Newpass);
                        command.Parameters.AddWithValue("@id", userID);
                        command.ExecuteNonQuery();
                    }
                    MessageBox.Show($"Password berhasil berubah!");
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message );
                }
            }
        }

        private void Tbx_OldPassword_TextChanged(object sender, TextChangedEventArgs e)
        {
            string oldpass = Tbx_OldPassword.Text;
            using(NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT password FROM data_login WHERE username = @username";
                    using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@username", Username);
                        var result = command.ExecuteScalar();
                        pass = result.ToString();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error connecting to database: {ex.Message}");
                }
            }

            if (oldpass == pass)
            {
                Tbx_NewPassword.IsEnabled = true;
            }
            else
            {
                Tbx_NewPassword.IsEnabled=false;
            }
        }

        private void Tbx_NewPassword_TextChanged(object sender, TextChangedEventArgs e)
        {
            Newpass = Tbx_NewPassword.Text;
            if (Tbx_NewPassword.Text != null && Tbx_NewPassword.Text.Length >= 6)
            {
                Tbx_Retype.IsEnabled = true;
            }
            else
            {
                Tbx_Retype.IsEnabled = false;
            }
        }

        private void Tbx_Retype_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Tbx_Retype.Text != null && Tbx_Retype.Text == Newpass) 
            {
                Btn_Change.IsEnabled = true;
            }
            else
            {
                Btn_Change.IsEnabled = false;
            }
        }
    }
}