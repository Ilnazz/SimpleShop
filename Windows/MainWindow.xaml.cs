using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using SessionProject.Pages;

namespace SessionProject.Windows
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            TBCurrentUserFullName.Text = App.CurrentUser.FullName;

            switch (App.CurrentUser.ID)
            {
                case 1: // admin
                    break;
                case 2: // manager
                    TIOrderList.Visibility = Visibility.Collapsed;
                    break;
                case 3: // storekeeper
                    TIProdList.Visibility = Visibility.Collapsed;
                    break;
                case 4: // customer - not realized
                    break;
            }

        }

        private void ButtonLogOut_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Выйти из системы? Несохранённые изменения будут потеряны", "Предупреждение",
                MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.No)
                return;
            
            App.CurrentUser = null;

            var authorizationWindow = new AuthorizationWindow();
            authorizationWindow.Show();
            authorizationWindow.Activate();

            Close();
        }
    }
}
