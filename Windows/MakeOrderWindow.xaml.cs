using SessionProject.Components;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Runtime.CompilerServices;
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
    /// Логика взаимодействия для MakeOrderWindow.xaml
    /// </summary>
    public partial class MakeOrderWindow : Window, INotifyPropertyChanged
    {
        public MakeOrderWindow()
        {
            InitializeComponent();

            CurrentOrder = new Order();
            CurrentOrder.DateTime = DateTime.Now;
            CurrentOrder.OrderStatusID = 1;
            CurrentOrder.UserCustomerID = App.CurrentUser.ID;

            App.DB.Products.Load();
            AvailableProducts = new ObservableCollection<Product>(App.DB.Products.Local);

            SelectedProducts = new ObservableCollection<Order_Product>();

            AvailableProductList.ItemsSource = AvailableProducts;
            SelectedProductList.ItemsSource = SelectedProducts;
        }

        public Order CurrentOrder { get; set; }

        public ObservableCollection<Order_Product> SelectedProducts { get; set; }

        public ObservableCollection<Product> AvailableProducts { get; set; }

        private int _totalCount = 0;
        public int TotalCount
        {
            get => _totalCount;
            set
            {
                _totalCount = value;
                NotifyPropertyChanged(nameof(TotalCount));
            }
        }

        private decimal _totalCost = 0;
        public decimal TotalCost
        {
            get => _totalCost;
            set
            {
                _totalCost = value;
                NotifyPropertyChanged(nameof(TotalCost));
            }
        }

        private void UpdateTotalCount()
            => TotalCount = SelectedProducts.Sum(p => p.Quantity);

        private void UpdateTotalCost()
            => TotalCost = SelectedProducts.Sum(p => p.PurchasePrice * p.Quantity);

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged([CallerMemberName] string paramName = "")
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(paramName));

        private void BtnFromAvailable_Click(object sender, RoutedEventArgs e)
        {
            if (AvailableProductList.SelectedItem is Product product == false)
                return;
            var orderProd = new Order_Product();
            orderProd.Product = product;
            orderProd.PurchasePrice = 0;
            orderProd.Order = CurrentOrder;
            AvailableProducts.Remove(product);
            SelectedProducts.Add(orderProd);

            UpdateTotalCount();
            UpdateTotalCost();
        }

        private void BtnFromSelected_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedProductList.SelectedItem is Order_Product orderProd == false)
                return;
            SelectedProducts.Remove(orderProd);
            AvailableProducts.Add(orderProd.Product);

            UpdateTotalCount();
            UpdateTotalCost();
        }

        private void BtnMakeOrder_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedProducts.Count == 0)
            {
                MessageBox.Show("Выберите хотя бы один продукт для оформления заказа");
                return;
            }
            if (SelectedProducts.Any(orderProd => (orderProd.Quantity == 0 || orderProd.PurchasePrice == 0)))
            {
                MessageBox.Show("Заполните все данные");
                return;
            }
            SelectedProducts.ToList().ForEach(orderProd => CurrentOrder.Order_Product.Add(orderProd));
            App.DB.Orders.Local.Add(CurrentOrder);
            App.DB.SaveChanges();
            MessageBox.Show("Заказ оформлен");
            Close();
        }
    }
}
