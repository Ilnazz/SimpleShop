﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SessionProject.Components
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class SessionDBEntities : DbContext
    {
        public SessionDBEntities()
            : base("name=SessionDBEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Gender> Genders { get; set; }
        public virtual DbSet<MeasureUnit> MeasureUnits { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<Order_Product> Order_Product { get; set; }
        public virtual DbSet<OrderStatus> OrderStatus1 { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<ProductsReceipt> ProductsReceipts { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<SupplierCountry> SupplierCountries { get; set; }
        public virtual DbSet<User> Users { get; set; }
    }
}
