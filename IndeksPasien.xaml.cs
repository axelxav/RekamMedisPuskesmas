using Npgsql;
using System;
using System.Configuration;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace RekamMedisPuskesmas
{
    public partial class IndeksPasien : Page
    {
        private MainWindow _mainWindow;
        private string connectionString = ConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;

        public IndeksPasien()
        {
            InitializeComponent();
            LoadPatientData();
            Cb_Wilayah.Items.Add("UJUNGGAGAK");
            Cb_Wilayah.Items.Add("UJUNGALANG");
            Cb_Wilayah.Items.Add("PANIKEL");
            Cb_Wilayah.Items.Add("KLACES");
            Cb_Wilayah.Items.Add("LUAR WILAYAH");

            PatientDataGrid.CellEditEnding += PatientDataGrid_CellEditEnding;
        }

        public IndeksPasien(MainWindow mainWindow) : this()
        {
            _mainWindow = mainWindow;
        }

        public void RefreshData()
        {
            LoadPatientData();
        }

        private void Btn_add_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            AddPatientData addPatientData = new AddPatientData(this);
            addPatientData.ShowDialog();
        }

        private void LogOut_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            _mainWindow.NavigateToPage(new LoginPage(_mainWindow));
        }

        private void LoadPatientData()
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT no_rm,rmepus,nama,tanggallahir,nik,nobpjs,wilayah,rt,rw,namakk,tglinput FROM data_pasien";

                    using (NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(query, connection))
                    {
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);

                        if (dataTable.Rows.Count > 0)
                        {
                            PatientDataGrid.ItemsSource = dataTable.DefaultView;
                        }
                        else
                        {
                            MessageBox.Show("No data found in the table.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading data: {ex.Message}");
                }
            }
        }

        private void PatientDataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit)
            {
                DataRowView rowView = e.Row.Item as DataRowView;
                if (rowView != null)
                {
                    string columnName = e.Column.Header.ToString();
                    string newValue = (e.EditingElement as TextBox).Text;
                    string noRm = rowView["no_rm"].ToString(); // Asumsi no_rm adalah primary key

                    // Menyesuaikan tipe data untuk kolom tertentu jika diperlukan
                    if (columnName == "Tanggal Lahir")
                    {
                        if (DateTime.TryParse(newValue, out DateTime parsedDate))
                        {
                            newValue = parsedDate.ToString("yyyy-MM-dd");
                        }
                        else
                        {
                            MessageBox.Show("Invalid date format.");
                            return;
                        }
                    }
                    else if (columnName == "RT" || columnName == "RW")
                    {
                        if (!int.TryParse(newValue, out _))
                        {
                            MessageBox.Show("Invalid number format.");
                            return;
                        }
                    }

                    UpdateDatabase(columnName, newValue, noRm);
                }
            }
        }

        private void UpdateDatabase(string columnName, string newValue, string noRm)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    // Ubah string ke huruf kapital
                    string capitalizedValue = newValue.ToUpper();

                    // Menyesuaikan query dan tipe data jika perlu
                    string query = $"UPDATE data_pasien SET {columnName} = @capitalizedValue WHERE no_rm = @noRm";

                    using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@capitalizedValue", capitalizedValue);
                        command.Parameters.AddWithValue("@noRm", noRm);
                        command.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error updating data: {ex.Message}");
                }
            }
        }


        private void Btn_Delete_Click(object sender, RoutedEventArgs e)
        {
            DataRowView selectedRow = PatientDataGrid.SelectedItem as DataRowView;
            if (selectedRow != null)
            {
                MessageBoxResult messageBoxResult = MessageBox.Show(
                    "Are you sure you want to delete this record?",
                    "Delete Confirmation",
                    MessageBoxButton.YesNo);

                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    try
                    {
                        using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                        {
                            connection.Open();
                            string no_rm = selectedRow["no_rm"].ToString();

                            string query = "DELETE FROM data_pasien WHERE no_rm = @no_rm";
                            using (NpgsqlCommand cmd = new NpgsqlCommand(query, connection))
                            {
                                cmd.Parameters.AddWithValue("no_rm", no_rm);
                                cmd.ExecuteNonQuery();
                            }

                            MessageBox.Show("Record deleted successfully.");

                            // Refresh the DataGrid after deletion
                            LoadPatientData();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error deleting data: {ex.Message}");
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a row to delete.");
            }
        }


    }
}