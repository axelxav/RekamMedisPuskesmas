using System.Windows.Controls;

namespace RekamMedisPuskesmas
{
    public partial class IndeksPasien : Page
    {
        private MainWindow _mainWindow;

        public IndeksPasien()
        {
            InitializeComponent();
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
    }
}
