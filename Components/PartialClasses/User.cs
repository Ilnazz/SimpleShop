using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SessionProject.Components
{
    public partial class User
    {
        public string FullName { get => $"{LastName} {FirstName[0]}. {Patronymic[0]}."; }

        public IEnumerable<Order> Order { get => App.DB.Orders.Local.Where(o => o.UserCustomerID == ID); }
    }
}
