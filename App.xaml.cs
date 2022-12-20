using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

using SessionProject.Components;

namespace SessionProject
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static SessionDBEntities DB = new SessionDBEntities();

        public static User CurrentUser = null;

        static App()
        {
            DB.Users.Load();
            DB.MeasureUnits.Load();
            DB.SupplierCountries.Load();
            DB.Products.Load();
            DB.Orders.Load();
            DB.Order_Product.Load();
            DB.ProductsReceipts.Load();
            DB.Roles.Load();
            DB.Users.Load();
        }
    }
}
