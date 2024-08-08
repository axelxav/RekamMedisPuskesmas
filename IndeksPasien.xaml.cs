using System.Windows.Controls;

namespace RekamMedisPuskesmas
{
    public partial class IndeksPasien : Page
    {
        public IndeksPasien()
        {
            InitializeComponent();
        }

        private void Btn_add_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            AddPatientData addpatientdata = new AddPatientData();
            addpatientdata.ShowDialog();

        }
    }
}
