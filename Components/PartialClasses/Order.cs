using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SessionProject.Components
{
    public partial class Order
    {
        public int TotalProductsNumber { get => Order_Product.Sum(op => op.Quantity); }

        public decimal TotalCost { get => Order_Product.Sum(op => op.Quantity * op.PurchasePrice); }

        public User UserCustomer { get => App.DB.Users.FirstOrDefault(u => u.ID == UserCustomerID); }

        public User UserExecutor { get => App.DB.Users.FirstOrDefault(u => u.ID == UserExecutorID); }
    }
}
