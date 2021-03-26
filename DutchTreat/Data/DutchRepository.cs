﻿using System;
using System.Collections.Generic;
using System.Linq;
using DutchTreat.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DutchTreat.Data
{
    public class DutchRepository : IDutchRepository
    {
        private readonly DutchContext context;
        private readonly ILogger<DutchRepository> logger;

        public DutchRepository(DutchContext context, ILogger<DutchRepository> logger)
        {
            this.context = context;
            this.logger = logger;
        }

        public void AddEntity(object model)
        {
            context.Add(model);
        }

        public IEnumerable<Order> GetAllOrders(bool includeItems)
        {
            if (includeItems)
            {
                return context.Orders
                    .Include(o => o.Items)
                    .ThenInclude(i => i.Product)
                    .ToList();

            } else
            {
                return context.Orders
                    .ToList();
            }
        }

        public IEnumerable<Product> GetAllProducts()
        {
            try
            {
                logger.LogInformation("GetAll Products");
                return context.Products
                    .OrderBy(p => p.Title)
                    .ToList();
            } catch(Exception ex)
            {
                logger.LogError($"Failed to get all products: {ex}");
                return null;
            }
        }

        public Order GetOrdeById(int id)
        {
            return context.Orders
                .Include(o => o.Items)
                .ThenInclude(i => i.Product)
                .Where(o => o.Id == id)
                .FirstOrDefault();
        }

        public IEnumerable<Product> GetProductsByCategory(string cateogry)
        {
            return context.Products
                .Where(p => p.Category == cateogry)
                .ToList();
        }


        public bool SaveAll()
        {
            return context.SaveChanges() > 0;
        }

    }
}
