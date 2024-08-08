using System.Windows;

namespace RekamMedisPuskesmas
{
    public partial class AddPatientData : Window
    {
        public AddPatientData()
        {
            InitializeComponent();
        }

        private void Btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
