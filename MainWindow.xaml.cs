using System;
using System.Windows;
using System.Windows.Controls;

namespace RekamMedisPuskesmas
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Main.Navigate(new LoginPage(this));
        }

        public void NavigateToPage(Page page)
        {
            Main.Navigate(page);
        }
    }
}
