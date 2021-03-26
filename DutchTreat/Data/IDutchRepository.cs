﻿using System.Collections.Generic;
using DutchTreat.Data.Entities;

namespace DutchTreat.Data
{
    public interface IDutchRepository
    {
        IEnumerable<Product> GetAllProducts();
        IEnumerable<Product> GetProductsByCategory(string cateogry);
        bool SaveAll();
        IEnumerable<Order> GetAllOrders(bool includeItems);
        Order GetOrdeById(int id);
        void AddEntity(object model);
    }
}