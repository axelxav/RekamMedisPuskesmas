using Npgsql;
using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace RekamMedisPuskesmas
{
    public partial class IndeksPasien : Page
    {
        private MainWindow _mainWindow;
        private string connectionString = "Host=localhost;Port=5432;Username=postgres;Password=1234;Database=puskesmas";

        public IndeksPasien()
        {
            InitializeComponent();
            LoadPatientData();
        }

        public IndeksPasien(MainWindow mainWindow) : this()
        {
            _mainWindow = mainWindow;
        }

        private void Btn_add_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            AddPatientData addpatientdata = new AddPatientData();
            addpatientdata.ShowDialog();

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
                    string query = "SELECT * FROM data_pasien";

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

    }
}
