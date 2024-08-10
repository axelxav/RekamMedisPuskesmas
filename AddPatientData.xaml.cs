using Npgsql;
using System;
using System.Configuration;
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

        private void Btn_Add_Click(object sender, RoutedEventArgs e)
        {
            string nourut = Tbx_NoUrut.Text;
            string rmepus = Tbx_RMEpus.Text;
            string nama = Tbx_Nama.Text.ToUpper();
            DateTime? tanggallahir = Dp_TglLahir.SelectedDate;
            string nik = Tbx_NIK.Text;
            string nobpjs = Tbx_NoBpjs.Text;
            string wilayah;

            // Cek apakah "LUAR WILAYAH" yang dipilih, jika ya, ambil nilai dari Tbx_LuarWilayah
            if (Cb_Wilayah.SelectedItem != null && Cb_Wilayah.SelectedItem.ToString() == "LUAR WILAYAH")
            {
                wilayah = Tbx_LuarWilayah.Text.ToUpper();
            }
            else
            {
                wilayah = Cb_Wilayah.Text.ToUpper();
            }

            int rt = int.Parse(Tbx_Rt.Text);
            int rw = int.Parse(Tbx_Rw.Text);
            string namakk = Tbx_NamaKK.Text.ToUpper();

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    // Insert query
                    string insertQuery = "INSERT INTO data_pasien(no_rm, rmepus, nama, tanggallahir, nik, nobpjs, wilayah, rt, rw, namakk) " +
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
                    string updateQuery = "UPDATE data_pasien " +
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

                // Tutup jendela dan refresh DataGrid
                this.Close();
                _indeksPasien.RefreshData();
            }
            catch (Exception ex)
            {
                // Tampilkan pesan kesalahan jika terjadi exception
                MessageBox.Show("Terjadi kesalahan saat menambahkan data: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void Cb_Wilayah_SelectionChanged(object sender, SelectionChangedEventArgs e)
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
        }
    }
}
