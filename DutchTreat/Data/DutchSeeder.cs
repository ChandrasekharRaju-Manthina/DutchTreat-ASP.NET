using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using DutchTreat.Data.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace DutchTreat.Data
{
    public class DutchSeeder
    {
        private readonly DutchContext ctx;
        private readonly IWebHostEnvironment env;
        private readonly ILogger<DutchSeeder> logger;
        private readonly UserManager<StoreUser> userManager;

        public DutchSeeder(DutchContext ctx, IWebHostEnvironment env, ILogger<DutchSeeder> logger,
            UserManager<StoreUser> userManager)
        {
            this.ctx = ctx;
            this.env = env;
            this.logger = logger;
            this.userManager = userManager;
        }

        public async Task SeedAsync()
        {
            ctx.Database.EnsureCreated();

            StoreUser user = await userManager.FindByEmailAsync("shawn@dutchtreat.com");
            if (user == null)
            {
                user = new StoreUser()
                {
                    FirstName = "Shawn",
                    LastName = "Wildermuth",
                    Email = "shawn@dutchtreat.com",
                    UserName = "shawn@dutchtreat.com"
                };
                var result = await userManager.CreateAsync(user, "P@ssw0rd!");
                if (result != IdentityResult.Success)
                {
                    throw new InvalidOperationException("Could not creat enew user in seeder");
                }
            }

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
                    },
                    User = user
                };

                ctx.Orders.Add(order);

                ctx.SaveChanges();
            }

        }
    }
}
