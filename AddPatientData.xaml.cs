using Npgsql;
using System;
using System.Configuration;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace RekamMedisPuskesmas
{
    public partial class AddPatientData : Window
    {
        private IndeksPasien _indeksPasien;
        string connectionString = ConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;

        public AddPatientData(IndeksPasien indeksPasien)
        {
            InitializeComponent();
            _indeksPasien = indeksPasien;

            // Menambahkan item ke dalam ComboBox
            Cb_Wilayah.Items.Add("UJUNGGAGAK");
            Cb_Wilayah.Items.Add("UJUNGALANG");
            Cb_Wilayah.Items.Add("PANIKEL");
            Cb_Wilayah.Items.Add("KLACES");
            Cb_Wilayah.Items.Add("LUAR WILAYAH");

            // Menyambungkan event handler untuk SelectionChanged
            Cb_Wilayah.SelectionChanged += Cb_Wilayah_SelectionChanged;
        }

        private void Btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private async void Btn_Add_Click(object sender, RoutedEventArgs e)
        {
            // Pastikan SetNoUrutAsync dipanggil untuk mengatur nilai Tbx_NoUrut.Text
            if (Cb_Wilayah.SelectedItem == null)
            {
                MessageBox.Show("Silakan pilih wilayah terlebih dahulu.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string selectedWilayah = Cb_Wilayah.SelectedItem.ToString().ToUpper();
            string tableName = GetTableNameForWilayah(selectedWilayah);

            if (string.IsNullOrEmpty(tableName))
            {
                MessageBox.Show("Wilayah yang dipilih tidak valid.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(Tbx_NoUrut.Text) ||
                string.IsNullOrWhiteSpace(Tbx_RMEpus.Text) ||
                string.IsNullOrWhiteSpace(Tbx_Nama.Text) ||
                !Dp_TglLahir.SelectedDate.HasValue ||
                string.IsNullOrWhiteSpace(Tbx_NIK.Text) ||
                string.IsNullOrWhiteSpace(Tbx_NoBpjs.Text) ||
                string.IsNullOrWhiteSpace((selectedWilayah == "LUAR WILAYAH") ? Tbx_LuarWilayah.Text : selectedWilayah) ||
                string.IsNullOrWhiteSpace(Tbx_Rt.Text) ||
                string.IsNullOrWhiteSpace(Tbx_Rw.Text) ||
                string.IsNullOrWhiteSpace(Tbx_NamaKK.Text))
            {
                // Show an error message or disable the button
                MessageBox.Show("Please fill in all required fields.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            await SetNoUrutAsync(tableName);

            string nourut = Tbx_NoUrut.Text;
            string rmepus = Tbx_RMEpus.Text;
            string nama = Tbx_Nama.Text.ToUpper();
            DateTime? tanggallahir = Dp_TglLahir.SelectedDate?.Date;
            string nik = Tbx_NIK.Text;
            string nobpjs = Tbx_NoBpjs.Text;
            string wilayah = (selectedWilayah == "LUAR WILAYAH") ? Tbx_LuarWilayah.Text.ToUpper() : selectedWilayah;

            int rt = int.Parse(Tbx_Rt.Text);
            int rw = int.Parse(Tbx_Rw.Text);
            string namakk = Tbx_NamaKK.Text.ToUpper();

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    // Insert query
                    string insertQuery = $"INSERT INTO {tableName}(no_rm, rmepus, nama, tanggallahir, nik, nobpjs, wilayah, rt, rw, namakk) " +
                                         "VALUES (@nourut, @rmepus, @nama, @tanggallahir, @nik, @nobpjs, @wilayah, @rt, @rw, @namakk)";

                    using (NpgsqlCommand insertCmd = new NpgsqlCommand(insertQuery, connection))
                    {
                        insertCmd.Parameters.AddWithValue("@nourut", nourut);
                        insertCmd.Parameters.AddWithValue("@rmepus", rmepus);
                        insertCmd.Parameters.AddWithValue("@nama", nama);
                        insertCmd.Parameters.AddWithValue("@tanggallahir", tanggallahir);
                        insertCmd.Parameters.AddWithValue("@nik", nik);
                        insertCmd.Parameters.AddWithValue("@nobpjs", nobpjs);
                        insertCmd.Parameters.AddWithValue("@wilayah", wilayah);
                        insertCmd.Parameters.AddWithValue("@rt", rt);
                        insertCmd.Parameters.AddWithValue("@rw", rw);
                        insertCmd.Parameters.AddWithValue("@namakk", namakk);

                        insertCmd.ExecuteNonQuery();
                    }

                    // Update query to apply LPAD
                    string updateQuery = $"UPDATE {tableName} " +
                                         "SET no_rm = LPAD(no_rm, 6, '0'), " +
                                             "rmepus = LPAD(rmepus, 6, '0') " +
                                         "WHERE no_rm = @nourut AND rmepus = @rmepus";

                    using (NpgsqlCommand updateCmd = new NpgsqlCommand(updateQuery, connection))
                    {
                        updateCmd.Parameters.AddWithValue("@nourut", nourut);
                        updateCmd.Parameters.AddWithValue("@rmepus", rmepus);

                        updateCmd.ExecuteNonQuery();
                    }
                }
                // Panggil metode refresh data pada IndeksPasien setelah data berhasil ditambahkan
                await RefreshDataGridAsync(selectedWilayah);

                this.Close();
            }
            catch (Exception ex)
            {
                // Tampilkan pesan kesalahan jika terjadi exception
                MessageBox.Show("Terjadi kesalahan saat menambahkan data: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task RefreshDataGridAsync(string selectedWilayah)
        {
            switch (selectedWilayah)
            {
                case "UJUNGGAGAK":
                    await _indeksPasien.LoadDataUjunggagakAsync();
                    break;
                case "UJUNGALANG":
                    await _indeksPasien.LoadDataUjungalangAsync();
                    break;
                case "PANIKEL":
                    await _indeksPasien.LoadDataPanikelAsync();
                    break;
                case "KLACES":
                    await _indeksPasien.LoadDataKlacesAsync();
                    break;
                case "LUAR WILAYAH":
                    await _indeksPasien.LoadDataLuarwilayahAsync();
                    break;
                default:
                    MessageBox.Show("Wilayah yang dipilih tidak valid.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    break;
            }
        }

        private async void Cb_Wilayah_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Aktifkan TextBox jika "LUAR WILAYAH" dipilih
            if (Cb_Wilayah.SelectedItem != null && Cb_Wilayah.SelectedItem.ToString().ToUpper() == "LUAR WILAYAH")
            {
                Tbx_LuarWilayah.IsEnabled = true;
            }
            else
            {
                Tbx_LuarWilayah.IsEnabled = false;
            }

            if (Cb_Wilayah.SelectedItem != null)
            {
                string selectedWilayah = Cb_Wilayah.SelectedItem.ToString().ToUpper();
                string tableName = GetTableNameForWilayah(selectedWilayah);

                if (!string.IsNullOrEmpty(tableName))
                {
                    await SetNoUrutAsync(tableName);
                }
            }
        }

        private async Task SetNoUrutAsync(string tableName)
        {
            string query = $"SELECT COALESCE(MAX(no_rm), '000000') FROM {tableName}";

            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                try
                {
                    await connection.OpenAsync();

                    using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                    {
                        object result = await command.ExecuteScalarAsync();

                        if (result != DBNull.Value)
                        {
                            string maxNoRm = result.ToString();
                            int maxNoRmInt;

                            // Try to parse the result as an integer and increment
                            if (int.TryParse(maxNoRm, out maxNoRmInt))
                            {
                                maxNoRmInt++;
                            }
                            else
                            {
                                maxNoRmInt = 1; // Handle case where parsing fails, default to 1
                            }

                            // Format the incremented value with leading zeros
                            Tbx_NoUrut.Text = maxNoRmInt.ToString("D6"); // Adjust length if needed
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error retrieving data: {ex.Message}");
                }
            }
        }

        private string GetTableNameForWilayah(string wilayah)
        {
            switch (wilayah)
            {
                case "UJUNGGAGAK":
                    return "data_ujunggagak";
                case "UJUNGALANG":
                    return "data_ujungalang";
                case "PANIKEL":
                    return "data_panikel";
                case "KLACES":
                    return "data_klaces";
                case "LUAR WILAYAH":
                    return "data_luarwilayah";
                default:
                    return "";
            }
        }
    }
}
