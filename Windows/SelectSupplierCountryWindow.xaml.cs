using SessionProject.Components;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Shapes;

namespace SessionProject.Windows
{
    /// <summary>
    /// Логика взаимодействия для SelectSupplierCountryWindow.xaml
    /// </summary>
    public partial class SelectSupplierCountryWindow : Window
    {
        public SupplierCountry SelectedSupplierCountry;

        public SelectSupplierCountryWindow(IEnumerable<SupplierCountry> supplierCountries)
        {
            InitializeComponent();

            CBSupplierCountry.ItemsSource = supplierCountries;
        }

        private void CBSupplierCountry_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CBSupplierCountry.SelectedItem is SupplierCountry selectedSC == false)
                return;

            SelectedSupplierCountry = selectedSC;
            Close();
        }
    }
}
