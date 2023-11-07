﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
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

using SessionProject.Components;
using SessionProject.Windows;

namespace SessionProject.Pages
{
    /// <summary>
    /// Логика взаимодействия для OrderListPage.xaml
    /// </summary>
    public partial class OrderListPage : Page
    {
        public ObservableCollection<Order> Orders
        {
            get { return (ObservableCollection<Order>)GetValue(OrdersProperty); }
            set { SetValue(OrdersProperty, value); }
        }
        public static readonly DependencyProperty OrdersProperty =
            DependencyProperty.Register("Orders", typeof(ObservableCollection<Order>), typeof(OrderListPage), new PropertyMetadata(null));

        public IEnumerable<OrderStatus> OrderStatuses => App.DB.OrderStatus1.ToList();

        public OrderListPage()
        {
            InitializeComponent();

            App.DB.Orders.Load();
            Orders = App.DB.Orders.Local;

            if (App.CurrentUser.RoleID == 3)
                BtnMakeOrder.Visibility = Visibility.Collapsed;
        }

        private void BtnMakeOrder_Click(object sender, RoutedEventArgs e)
        {
            var makeOrderWindow = new MakeOrderWindow();
            makeOrderWindow.ShowDialog();

            Orders = App.DB.Orders.Local;
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Orders = App.DB.Orders.Local;
        }
    }
}
