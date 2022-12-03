using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SessionProject.Components
{
    public partial class Product
    {
        public bool IsOutOfStock { get => Quantity == 0; }
    }
}
