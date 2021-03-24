using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using DutchTreat.Data.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace DutchTreat.Data
{
    public class DutchSeeder
    {
        private readonly DutchContext ctx;
        private readonly IWebHostEnvironment env;
        private readonly ILogger<DutchSeeder> logger;

        public DutchSeeder(DutchContext ctx, IWebHostEnvironment env, ILogger<DutchSeeder> logger)
        {
            this.ctx = ctx;
            this.env = env;
            this.logger = logger;
        }

        public void Seed()
        {
            ctx.Database.EnsureCreated();

            if (!ctx.Products.Any())
            {
                // create sample data
                var filePath = Path.Combine("Data/art.json");
                var json = File.ReadAllText(filePath);

                var products = JsonSerializer.Deserialize<IEnumerable<Product>>(json);
                logger.LogInformation($"Product size {products.Count()}");
                ctx.Products.AddRange(products);

                var order = new Order()
                {
                    OrderDate = DateTime.Today,
                    OrderNumber = "10000",
                    Items = new List<OrderItem>()
                    {
                        new OrderItem()
                        {
                            Product = products.First(),
                            Quantity = 5,
                            UnitPrice = products.First().Price
                        }
                    }
                };

                ctx.Orders.Add(order);

                ctx.SaveChanges();
            }

        }
    }
}
