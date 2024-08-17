using Npgsql;
using System;
using System.Configuration;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Text.RegularExpressions;
using System.Windows.Input;
using System.Threading.Tasks;

namespace RekamMedisPuskesmas
{
    public partial class IndeksPasien : Page
    {
        private MainWindow _mainWindow;
        private string connectionString = ConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;

        public IndeksPasien()
        {
            InitializeComponent();
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

        private async void Btn_add_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            AddPatientData addPatientData = new AddPatientData(this);
            addPatientData.ShowDialog();
        }

        private void LogOut_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            _mainWindow.NavigateToPage(new LoginPage(_mainWindow));
        }

        public async Task LoadDataUjunggagakAsync()
        {
            LoadingProgressBar.Visibility = Visibility.Visible;

            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                try
                {
                    await connection.OpenAsync();
                    string query = "SELECT * FROM data_ujunggagak";

                    using (NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(query, connection))
                    {
                        DataTable dataTable = new DataTable();
                        await Task.Run(() => adapter.Fill(dataTable));

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
                finally
                {
                    LoadingProgressBar.Visibility = Visibility.Collapsed;
                }
            }
        }

        public async Task LoadDataUjungalangAsync()
        {
            LoadingProgressBar.Visibility = Visibility.Visible;

            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                try
                {
                    await connection.OpenAsync();
                    string query = "SELECT * FROM data_ujungalang";

                    using (NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(query, connection))
                    {
                        DataTable dataTable = new DataTable();
                        await Task.Run(() => adapter.Fill(dataTable));

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
                finally
                {
                    LoadingProgressBar.Visibility = Visibility.Collapsed;
                }
            }
        }

        public async Task LoadDataPanikelAsync()
        {
            LoadingProgressBar.Visibility = Visibility.Visible;

            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                try
                {
                    await connection.OpenAsync();
                    string query = "SELECT * FROM data_panikel";

                    using (NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(query, connection))
                    {
                        DataTable dataTable = new DataTable();
                        await Task.Run(() => adapter.Fill(dataTable));

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

                finally
                {
                    LoadingProgressBar.Visibility = Visibility.Collapsed;
                }
            }
        }

        public async Task LoadDataKlacesAsync()
        {
            LoadingProgressBar.Visibility = Visibility.Visible;

            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                try
                {
                    await connection.OpenAsync();
                    string query = "SELECT * FROM data_klaces";

                    using (NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(query, connection))
                    {
                        DataTable dataTable = new DataTable();
                        await Task.Run(() => adapter.Fill(dataTable));

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
                finally
                {
                    LoadingProgressBar.Visibility = Visibility.Collapsed;
                }
            }
        }

        public async Task LoadDataLuarwilayahAsync()
        {
            LoadingProgressBar.Visibility = Visibility.Visible;

            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                try
                {
                    await connection.OpenAsync();
                    string query = "SELECT * FROM data_luarwilayah";

                    using (NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(query, connection))
                    {
                        DataTable dataTable = new DataTable();
                        await Task.Run(() => adapter.Fill(dataTable));

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
                finally
                {
                    LoadingProgressBar.Visibility = Visibility.Collapsed;
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
                    // Mengambil header kolom
                    string columnHeader = e.Column.Header.ToString();

                    if (columnHeader.Equals("No RM", StringComparison.OrdinalIgnoreCase))
                    {
                        MessageBox.Show("Pengeditan pada kolom 'No RM' tidak diperbolehkan!", "Peringatan", MessageBoxButton.OK, MessageBoxImage.Warning);
                        e.Cancel = true; // Batalkan pengeditan
                        return;
                    }

                    // Menghapus spasi, karakter khusus, dan mengubah menjadi huruf kecil
                    string columnName = Regex.Replace(columnHeader, @"[^a-zA-Z0-9]", "").ToLower();

                    string newValue = (e.EditingElement as TextBox).Text;
                    string noRm = rowView["no_rm"].ToString(); // Asumsi no_rm adalah primary key

                    // Menangani tipe data integer
                    if (columnName == "rt" || columnName == "rw")
                    {
                        if (int.TryParse(newValue, out int intValue))
                        {
                            newValue = intValue.ToString(); // Simpan sebagai integer
                        }
                        else
                        {
                            MessageBox.Show("Format angka salah");
                            return;
                        }
                    }
                    else if (columnName == "tanggallahir")
                    {
                        if (DateTime.TryParse(newValue, out DateTime parsedDate))
                        {
                            newValue = parsedDate.ToString("yyyy-MM-dd");
                        }
                        else
                        {
                            MessageBox.Show("Format tanggal salah");
                            return;
                        }
                    }
                    else
                    {
                        newValue = newValue.ToUpper();
                    }

                    string wilayah = Cb_Wilayah.SelectedItem as string;
                    UpdateDatabase(columnName, newValue, noRm, wilayah);
                }
            }
        }

        private void UpdateDatabase(string columnName, string newValue, string noRm, string wilayah)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = $"UPDATE ";

                    if (!string.IsNullOrEmpty(wilayah))
                    {
                        if (wilayah == "UJUNGGAGAK")
                        {
                            query += $"data_ujunggagak SET \"{columnName}\" = @value WHERE no_rm = @noRm";
                        }
                        else if (wilayah == "UJUNGALANG")
                        {
                            query += $"data_ujungalang SET \"{columnName}\" = @value WHERE no_rm = @noRm";
                        }
                        else if (wilayah == "PANIKEL")
                        {
                            query += $"data_panikel SET \"{columnName}\" = @value WHERE no_rm = @noRm";
                        }
                        else if (wilayah == "KLACES")
                        {
                            query += $"data_klaces SET \"{columnName}\" = @value WHERE no_rm = @noRm";
                        }
                        else
                        {
                            query += $"data_luarwilayah SET \"{columnName}\" = @value WHERE no_rm = @noRm";
                        }
                    }

                    using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                    {
                        // Menambahkan parameter dengan tipe yang sesuai
                        if (columnName == "rt" || columnName == "rw")
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
                    MessageBox.Show($"Error dalam melakukan pembaharuan data: {ex.Message}");
                }
            }
        }


        public async void Btn_Delete_Click(object sender, RoutedEventArgs e)
        {
            DataRowView selectedRow = PatientDataGrid.SelectedItem as DataRowView;
            string selectedWilayah = Cb_Wilayah.SelectedItem as string;
            if (selectedRow != null)
            {
                MessageBoxResult messageBoxResult = MessageBox.Show(
                    "Apakah Anda yakin untuk menghapus data ini?",
                    "Konfirmasi penghapusan",
                    MessageBoxButton.YesNo);

                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    try
                    {
                        using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                        {
                            connection.Open();
                            string no_rm = selectedRow["no_rm"].ToString();

                            string query = "DELETE FROM ";

                            if (!string.IsNullOrEmpty(selectedWilayah))
                            {
                                if (selectedWilayah == "UJUNGGAGAK")
                                {
                                    query += " data_ujunggagak WHERE no_rm = @no_rm";
                                }
                                else if (selectedWilayah == "UJUNGALANG")
                                {
                                    query += " data_ujungalang WHERE no_rm = @no_rm";
                                }
                                else if (selectedWilayah == "PANIKEL")
                                {
                                    query += " data_panikel WHERE no_rm = @no_rm";
                                }
                                else if (selectedWilayah == "KLACES")
                                {
                                    query += " data_klaces WHERE no_rm = @no_rm";
                                }
                                else
                                {
                                    query += " data_luarwilayah WHERE no_rm = @no_rm";
                                }
                            }
                            using (NpgsqlCommand cmd = new NpgsqlCommand(query, connection))
                            {
                                cmd.Parameters.AddWithValue("no_rm", no_rm);
                                cmd.ExecuteNonQuery();
                            }

                            MessageBox.Show("Data telah dihapus dari database");

                            // Refresh the DataGrid after deletion
                            if (!string.IsNullOrEmpty(selectedWilayah))
                            {
                                if (selectedWilayah == "UJUNGGAGAK")
                                {
                                    await LoadDataUjunggagakAsync();
                                }
                                else if (selectedWilayah == "UJUNGALANG")
                                {
                                    await LoadDataUjungalangAsync();
                                }
                                else if (selectedWilayah == "PANIKEL")
                                {
                                    await LoadDataPanikelAsync();
                                }
                                else if (selectedWilayah == "KLACES")
                                {
                                    await LoadDataKlacesAsync();
                                }
                                else
                                {
                                    await LoadDataLuarwilayahAsync();
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Galat dalam menghapus data: {ex.Message}");
                    }
                }
            }
            else
            {
                MessageBox.Show("Klik terlebih dahulu pada baris yang hendak dihapus");
            }
        }

        private async void Cb_Wilayah_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selectedWilayah = Cb_Wilayah.SelectedItem as string;

            if (selectedWilayah != null)
            {
                if (selectedWilayah == "UJUNGGAGAK")
                {
                    await LoadDataUjunggagakAsync();
                }
                else if (selectedWilayah == "UJUNGALANG")
                {
                    await LoadDataUjungalangAsync();
                }
                else if (selectedWilayah == "PANIKEL")
                {
                    await LoadDataPanikelAsync();
                }
                else if (selectedWilayah == "KLACES")
                {
                    await LoadDataKlacesAsync();
                }
                else
                {
                    await LoadDataLuarwilayahAsync();
                }
            }
        }
        
        private void Tbx_nama_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                string userInput = Tbx_nama.Text.ToUpper();
                string selectedWilayah = Cb_Wilayah.SelectedItem as string;

                if (string.IsNullOrEmpty(selectedWilayah))
                {
                    MessageBox.Show("Pilih wilayah terlebih dahulu!");
                    return;
                }

                string query = $"SELECT * FROM ";
                if (!string.IsNullOrEmpty(selectedWilayah))
                {
                    if (selectedWilayah == "UJUNGGAGAK")
                    {
                        query += " data_ujunggagak WHERE UPPER(nama) LIKE @nama";
                    }
                    else if (selectedWilayah == "UJUNGALANG")
                    {
                        query += " data_ujungalang WHERE UPPER(nama) LIKE @nama";
                    }
                    else if (selectedWilayah == "PANIKEL")
                    {
                        query += " data_panikel WHERE UPPER(nama) LIKE @nama";
                    }
                    else if (selectedWilayah == "KLACES")
                    {
                        query += " data_klaces WHERE UPPER(nama) LIKE @nama";
                    }
                    else
                    {
                        query += " data_luarwilayah WHERE UPPER(nama) LIKE @nama";
                    }
                }

                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    try
                    {
                        connection.Open();
                        using (NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(query, connection))
                        {
                            adapter.SelectCommand.Parameters.AddWithValue("@nama", $"%{userInput}%");

                            DataTable dataTable = new DataTable();
                            adapter.Fill(dataTable);
                            PatientDataGrid.ItemsSource = dataTable.DefaultView;
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Galat dalam memuat data: {ex.Message}");
                    }
                }
            }
        }

        private void Tbx_noBPJS_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                string userInput = Tbx_noBPJS.Text;
                string selectedWilayah = Cb_Wilayah.SelectedItem as string;

                if (string.IsNullOrEmpty(selectedWilayah))
                {
                    MessageBox.Show("Pilih wilayah terlebih dahulu");
                    return;
                }

                string query = $"SELECT * FROM ";
                if (!string.IsNullOrEmpty(selectedWilayah))
                {
                    if (selectedWilayah == "UJUNGGAGAK")
                    {
                        query += " data_ujunggagak WHERE nobpjs LIKE @nobpjs";
                    }
                    else if (selectedWilayah == "UJUNGALANG")
                    {
                        query += " data_ujungalang WHERE nobpjs LIKE @nobpjs";
                    }
                    else if (selectedWilayah == "PANIKEL")
                    {
                        query += " data_panikel WHERE nobpjs LIKE @nobpjs";
                    }
                    else if (selectedWilayah == "KLACES")
                    {
                        query += " data_klaces WHERE nobpjs LIKE @nobpjs";
                    }
                    else
                    {
                        query += " data_luarwilayah WHERE nobpjs LIKE @nobpjs";
                    }
                }

                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    try
                    {
                        connection.Open();
                        using (NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(query, connection))
                        {
                            adapter.SelectCommand.Parameters.AddWithValue("@nobpjs", $"%{userInput}%");

                            DataTable dataTable = new DataTable();
                            adapter.Fill(dataTable);
                            PatientDataGrid.ItemsSource = dataTable.DefaultView;
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Galat dalam memuat data: {ex.Message}");
                    }
                }
            }
        }

        private void Tbx_noUrut_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                string userInput = Tbx_noUrut.Text;
                string selectedWilayah = Cb_Wilayah.SelectedItem as string;

                if (string.IsNullOrEmpty(selectedWilayah))
                {
                    MessageBox.Show("Pilihlah salah satu wilayah");
                    return;
                }

                string query = $"SELECT * FROM ";
                if (!string.IsNullOrEmpty(selectedWilayah))
                {
                    if (selectedWilayah == "UJUNGGAGAK")
                    {
                        query += " data_ujunggagak WHERE no_rm LIKE @no_rm";
                    }
                    else if (selectedWilayah == "UJUNGALANG")
                    {
                        query += " data_ujungalang WHERE no_rm LIKE @no_rm";
                    }
                    else if (selectedWilayah == "PANIKEL")
                    {
                        query += " data_panikel WHERE no_rm LIKE @no_rm";
                    }
                    else if (selectedWilayah == "KLACES")
                    {
                        query += " data_klaces WHERE no_rm LIKE @no_rm";
                    }
                    else
                    {
                        query += " data_luarwilayah WHERE no_rm LIKE @no_rm";
                    }
                }

                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    try
                    {
                        connection.Open();
                        using (NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(query, connection))
                        {
                            adapter.SelectCommand.Parameters.AddWithValue("@no_rm", $"%{userInput}%");

                            DataTable dataTable = new DataTable();
                            adapter.Fill(dataTable);
                            PatientDataGrid.ItemsSource = dataTable.DefaultView;
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Galat dalam memuat data: {ex.Message}");
                    }
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
                MessageBox.Show("Pilih tanggal mulai dan tanggal akhir");
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
            PatientDataGrid.ItemsSource = null;

            //LoadPatientData(); // Reload all data
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

                    string query = "SELECT * FROM ";

                    if (!string.IsNullOrEmpty(wilayah))
                    {
                        if (wilayah == "UJUNGGAGAK")
                        {
                            query += " data_ujunggagak WHERE tglinput >= @startDate AND tglinput < @endDate";
                        }
                        else if (wilayah == "UJUNGALANG")
                        {
                            query += " data_ujungalang WHERE tglinput >= @startDate AND tglinput < @endDate";
                        }
                        else if (wilayah == "PANIKEL")
                        {
                            query += " data_panikel WHERE tglinput >= @startDate AND tglinput < @endDate";
                        }
                        else if (wilayah == "KLACES")
                        {
                            query += " data_klaces WHERE tglinput >= @startDate AND tglinput < @endDate";
                        }
                        else
                        {
                            query += " data_luarwilayah WHERE tglinput >= @startDate AND tglinput < @endDate";
                        }
                    }

                    using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@startDate", startDate);
                        command.Parameters.AddWithValue("@endDate", adjustedEndDate); // Menggunakan tanggal akhir yang telah disesuaikan

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
                    MessageBox.Show($"Galat dalam memuat data: {ex.Message}");
                }
            }
        }

        private void Change_Click(object sender, RoutedEventArgs e)
        {
            ChangePassword changepassword = new ChangePassword();
            changepassword.ShowDialog();
        }
    }
}