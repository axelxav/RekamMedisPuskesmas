using Npgsql;
using System;
using System.Configuration;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Text.RegularExpressions;
using System.Windows.Input;

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
                    //string columnName = e.Column.Header.ToString();
                    // Mengambil header kolom
                    string columnHeader = e.Column.Header.ToString();

                    // Menghapus spasi, karakter khusus, dan mengubah menjadi huruf kecil
                    string columnName = Regex.Replace(columnHeader, @"[^a-zA-Z0-9]", "").ToLower();

                    string newValue = (e.EditingElement as TextBox).Text;
                    string noRm = rowView["no_rm"].ToString(); // Asumsi no_rm adalah primary key

                    // Menangani tipe data integer
                    if (columnName == "RT" || columnName == "RW")
                    {
                        if (int.TryParse(newValue, out int intValue))
                        {
                            newValue = intValue.ToString();
                        }
                        else
                        {
                            MessageBox.Show("Invalid number format.");
                            return;
                        }
                    }
                    else if (columnName == "Tanggal Lahir")
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
                    else
                    {
                        newValue = newValue.ToUpper();
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
                    string query = $"UPDATE data_pasien SET \"{columnName}\" = @value WHERE no_rm = @noRm";

                    using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                    {
                        // Menambahkan parameter dengan tipe yang sesuai
                        if (columnName == "RT" || columnName == "RW")
                        {
                            command.Parameters.AddWithValue("@value", int.Parse(newValue));
                        }
                        else
                        {
                            command.Parameters.AddWithValue("@value", newValue);
                        }

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

        private void Cb_Wilayah_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selectedWilayah = Cb_Wilayah.SelectedItem as string;

            if (selectedWilayah != null)
            {
                if (selectedWilayah == "LUAR WILAYAH")
                {
                    // Tampilkan data di luar wilayah UJUNGGAGAK, UJUNGALANG, KLACES, PANIKEL
                    LoadFilteredPatientData("WHERE wilayah NOT IN ('UJUNGGAGAK', 'UJUNGALANG', 'PANIKEL', 'KLACES')");
                }
                else
                {
                    // Tampilkan data sesuai wilayah yang dipilih
                    LoadFilteredPatientData($"WHERE wilayah = '{selectedWilayah}'");
                }
            }
        }

        private void LoadFilteredPatientData(string filter)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = $"SELECT no_rm,rmepus,nama,tanggallahir,nik,nobpjs,wilayah,rt,rw,namakk,tglinput FROM data_pasien {filter}";

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
                            MessageBox.Show("Tidak terdapat data yang dicari!");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading data: {ex.Message}");
                }
            }
        }

        private void Tbx_noUrut_TextChanged(object sender, TextChangedEventArgs e)
        {
            string input = Tbx_noUrut.Text.Trim();
            string selectedWilayah = Cb_Wilayah.SelectedItem as string;

            if (string.IsNullOrEmpty(input))
            {
                if (!string.IsNullOrEmpty(selectedWilayah))
                {
                    // Tampilkan data sesuai wilayah yang dipilih
                    Cb_Wilayah_SelectionChanged(null, null);
                }
                else
                {
                    LoadPatientData();
                }
            }
            else
            {
                string paddedInput = input.PadLeft(6, '0');

                if (paddedInput.Length == 6)
                {
                    using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                    {
                        try
                        {
                            connection.Open();
                            string query = "SELECT no_rm, rmepus, nama, tanggallahir, nik, nobpjs, wilayah, rt, rw, namakk, tglinput FROM data_pasien WHERE no_rm = @noUrut";

                            if (!string.IsNullOrEmpty(selectedWilayah))
                            {
                                if (selectedWilayah == "LUAR WILAYAH")
                                {
                                    query += " AND wilayah NOT IN ('UJUNGGAGAK', 'UJUNGALANG', 'PANIKEL', 'KLACES')";
                                }
                                else
                                {
                                    query += " AND wilayah = @selectedWilayah";
                                }
                            }

                            using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                            {
                                command.Parameters.AddWithValue("@noUrut", paddedInput);
                                if (!string.IsNullOrEmpty(selectedWilayah) && selectedWilayah != "LUAR WILAYAH")
                                {
                                    command.Parameters.AddWithValue("@selectedWilayah", selectedWilayah);
                                }

                                using (NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(command))
                                {
                                    DataTable dataTable = new DataTable();
                                    adapter.Fill(dataTable);
                                    PatientDataGrid.ItemsSource = dataTable.DefaultView;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Error loading data: {ex.Message}");
                        }
                    }
                }
            }
        }


        private void Tbx_nama_TextChanged(object sender, TextChangedEventArgs e)
        {
            string userInput = Tbx_nama.Text.ToUpper();
            string selectedWilayah = Cb_Wilayah.SelectedItem as string;

            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT no_rm, rmepus, nama, tanggallahir, nik, nobpjs, wilayah, rt, rw, namakk, tglinput " +
                                   "FROM data_pasien " +
                                   "WHERE UPPER(nama) LIKE @nama";

                    if (!string.IsNullOrEmpty(selectedWilayah))
                    {
                        if (selectedWilayah == "LUAR WILAYAH")
                        {
                            query += " AND wilayah NOT IN ('UJUNGGAGAK', 'UJUNGALANG', 'PANIKEL', 'KLACES')";
                        }
                        else
                        {
                            query += " AND wilayah = @selectedWilayah";
                        }
                    }

                    using (NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(query, connection))
                    {
                        adapter.SelectCommand.Parameters.AddWithValue("@nama", $"%{userInput}%");
                        if (!string.IsNullOrEmpty(selectedWilayah) && selectedWilayah != "LUAR WILAYAH")
                        {
                            adapter.SelectCommand.Parameters.AddWithValue("@selectedWilayah", selectedWilayah);
                        }

                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);
                        PatientDataGrid.ItemsSource = dataTable.DefaultView;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading data: {ex.Message}");
                }
            }
        }


        private void Btn_search_Click(object sender, RoutedEventArgs e)
        {
            DateTime? startDate = Dp_tanggalAwal.SelectedDate;
            DateTime? endDate = Dp_tanggalAkhir.SelectedDate;
            string selectedWilayah = Cb_Wilayah.SelectedItem as string;

            if (startDate.HasValue && endDate.HasValue)
            {
                FilterDataByDate(startDate.Value, endDate.Value, selectedWilayah);
            }
            else
            {
                MessageBox.Show("Please select both start and end dates.");
            }
        }


        private void Btn_clear_Click(object sender, RoutedEventArgs e)
        {
            // Clear date pickers and reset the DataGrid
            Dp_tanggalAwal.SelectedDate = null;
            Dp_tanggalAkhir.SelectedDate = null;
            Cb_Wilayah.SelectedItem = null;
            Tbx_nama.Text = null;
            Tbx_noUrut.Text = null;

            LoadPatientData(); // Reload all data
        }

        private void FilterDataByDate(DateTime startDate, DateTime endDate, string wilayah)
        {
            // Menambahkan satu hari pada endDate untuk menyertakan seluruh hari tersebut
            DateTime adjustedEndDate = endDate.AddDays(1);

            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    // Kondisi filter untuk wilayah
                    string wilayahFilter = string.IsNullOrEmpty(wilayah) ? "" : $" AND wilayah = @wilayah";

                    string query = "SELECT no_rm, rmepus, nama, tanggallahir, nik, nobpjs, wilayah, rt, rw, namakk, tglinput " +
                                   "FROM data_pasien " +
                                   "WHERE tglinput >= @startDate AND tglinput < @endDate" + wilayahFilter;

                    using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@startDate", startDate);
                        command.Parameters.AddWithValue("@endDate", adjustedEndDate); // Menggunakan tanggal akhir yang telah disesuaikan

                        // Menambahkan parameter wilayah jika diperlukan
                        if (!string.IsNullOrEmpty(wilayah))
                        {
                            command.Parameters.AddWithValue("@wilayah", wilayah);
                        }

                        using (NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(command))
                        {
                            DataTable dataTable = new DataTable();
                            adapter.Fill(dataTable);
                            PatientDataGrid.ItemsSource = dataTable.DefaultView;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading data: {ex.Message}");
                }
            }
        }


        private void Tbx_noBPJS_TextChanged(object sender, TextChangedEventArgs e)
        {
            string userInput = Tbx_noBPJS.Text;
            string selectedWilayah = Cb_Wilayah.SelectedItem as string;

            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT no_rm, rmepus, nama, tanggallahir, nik, nobpjs, wilayah, rt, rw, namakk, tglinput " +
                                   "FROM data_pasien " +
                                   "WHERE nobpjs LIKE @nobpjs";

                    if (!string.IsNullOrEmpty(selectedWilayah))
                    {
                        if (selectedWilayah == "LUAR WILAYAH")
                        {
                            query += " AND wilayah NOT IN ('UJUNGGAGAK', 'UJUNGALANG', 'PANIKEL', 'KLACES')";
                        }
                        else
                        {
                            query += " AND wilayah = @selectedWilayah";
                        }
                    }

                    using (NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(query, connection))
                    {
                        adapter.SelectCommand.Parameters.AddWithValue("@nobpjs", $"%{userInput}%");
                        if (!string.IsNullOrEmpty(selectedWilayah) && selectedWilayah != "LUAR WILAYAH")
                        {
                            adapter.SelectCommand.Parameters.AddWithValue("@selectedWilayah", selectedWilayah);
                        }

                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);
                        PatientDataGrid.ItemsSource = dataTable.DefaultView;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading data: {ex.Message}");
                }
            }
        }
    }
}