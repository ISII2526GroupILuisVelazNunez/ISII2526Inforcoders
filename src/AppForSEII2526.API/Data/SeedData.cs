using AppForSEII2526.API.Models;

namespace AppForSEII2526.API.Data
{
    public static class SeedData
    {

        public static void Initialize(ApplicationDbContext dbContext, IServiceProvider serviceProvider, ILogger logger)
        {
            List<string> rolesNames = new List<string> { "Administrator", "Employee", "Customer" };

            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            try
            {
                SeedRoles(roleManager, rolesNames);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred seeding the roles in the Database.");
            }

            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            try
            {
                SeedUsers(userManager, rolesNames);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred seeding the Users in the Database.");
            }
            try
            {
                SeedItems(dbContext);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred seeding the Items in the Database.");
            }
        }

        public static void SeedRoles(RoleManager<IdentityRole> roleManager, List<string> roles)
        {

            foreach (string roleName in roles)
            {
                //it checks such role does not exist in the database 
                if (!roleManager.RoleExistsAsync(roleName).Result)
                {
                    IdentityRole role = new IdentityRole();
                    role.Name = roleName;
                    role.NormalizedName = roleName;
                    IdentityResult roleResult = roleManager.CreateAsync(role).Result;
                }
            }

        }

        public static void SeedUsers(UserManager<ApplicationUser> userManager, List<string> roles)
        {
            // Administrator
            if (userManager.FindByNameAsync("elena@uclm.es").Result == null)
            {
                var user = new ApplicationUser(
                    "Elena",
                    "Navarro Martínez"
                )
                {
                    UserName = "elena@uclm.es",
                    Email = "elena@uclm.es",
                    EmailConfirmed = true
                };

                // Create PaymentMethods
                var cc = new CreditCard
                {
                    CreditCardNumber = 4111222233334444,
                    ExpirationDate = new DateTime(2028, 12, 31),
                    User = user
                };

                var paypal = new PayPal
                {
                    Email = "elena.paypal@uclm.es",
                    User = user
                };

                user.PaymentMethods.Add(cc);
                user.PaymentMethods.Add(paypal);

                var result = userManager.CreateAsync(user, "Password1234%").Result;
                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(user, roles[0]).Wait();
                }
            }

            // Employee
            if (userManager.FindByNameAsync("gregorio@uclm.es").Result == null)
            {
                var user = new ApplicationUser(
                    "Gregorio",
                    "Diaz Descalzo"
                )
                {
                    UserName = "gregorio@uclm.es",
                    Email = "gregorio@uclm.es",
                    EmailConfirmed = true
                };

                var bizum = new Bizum
                {
                    TelephoneNumber = 612345678,
                    User = user
                };

                user.PaymentMethods.Add(bizum);

                var result = userManager.CreateAsync(user, "APassword1234%").Result;
                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(user, roles[1]).Wait();
                }
            }

            // Customer
            if (userManager.FindByNameAsync("peter@uclm.es").Result == null)
            {
                var user = new ApplicationUser(
                    "Peter",
                    "Jackson"
                )
                {
                    UserName = "peter@uclm.es",
                    Email = "peter@uclm.es",
                    EmailConfirmed = true
                };

                var cc = new CreditCard
                {
                    CreditCardNumber = 5555666677778888,
                    ExpirationDate = new DateTime(2029, 6, 30),
                    User = user
                };

                user.PaymentMethods.Add(cc);

                var result = userManager.CreateAsync(user, "OtherPass12$").Result;
                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(user, roles[2]).Wait();
                }
            }
        }

        public static void SeedItems(ApplicationDbContext context)
        {
            if (context.Items.Any()) return;

            // Brands
            var brandNike = new Brand { Name = "Nike" };
            var brandAdidas = new Brand { Name = "Adidas" };
            var brandReebok = new Brand { Name = "Reebok" };

            // Types
            var typeEquipment = new TypeItem { Name = "Equipment" };
            var typeAccessory = new TypeItem { Name = "Accessory" };
            var typeApparel = new TypeItem { Name = "Apparel" };

            // Items
            var items = new List<Item>
            {
                new Item
                {
                    Name = "Yoga Mat",
                    Description = "High-density, non-slip yoga mat",
                    PurchasePrice = 25.99m,
                    QuantityAvailableForPurchase = 100,
                    QuantityForRestock = 20,
                    TypeItem = typeAccessory,
                    Brand = brandNike,
                    RestockPrice = 20.00m,
                    PurchaseItems = new List<PurchaseItem>()
                },
                new Item
                {
                    Name = "Dumbbell Set 10-50kg",
                    Description = "Adjustable dumbbells for strength training",
                    PurchasePrice = 199.99m,
                    QuantityAvailableForPurchase = 15,
                    QuantityForRestock = 5,
                    TypeItem = typeEquipment,
                    Brand = brandReebok,
                    RestockPrice = 150.00m,
                    PurchaseItems = new List<PurchaseItem>()
                },
                new Item
                {
                    Name = "Running Shoes",
                    Description = "Lightweight running shoes for gym and outdoor use",
                    PurchasePrice = 120.00m,
                    QuantityAvailableForPurchase = 50,
                    QuantityForRestock = 10,
                    TypeItem = typeApparel,
                    Brand = brandAdidas,
                    RestockPrice = 90.00m,
                    PurchaseItems = new List<PurchaseItem>()
                },
                new Item
                {
                    Name = "Protein Shaker Bottle",
                    Description = "BPA-free shaker bottle for protein shakes",
                    PurchasePrice = 12.50m,
                    QuantityAvailableForPurchase = 200,
                    QuantityForRestock = 50,
                    TypeItem = typeAccessory,
                    Brand = brandNike,
                    RestockPrice = 10.00m,
                    PurchaseItems = new List<PurchaseItem>()
                },
                new Item
                {
                    Name = "Pull-Up Bar",
                    Description = "Doorway pull-up bar for home workouts",
                    PurchasePrice = 45.99m,
                    QuantityAvailableForPurchase = 30,
                    QuantityForRestock = 10,
                    TypeItem = typeEquipment,
                    Brand = brandReebok,
                    RestockPrice = 35.00m,
                    PurchaseItems = new List<PurchaseItem>()
                }
            };

            context.Brands.AddRange(brandNike, brandAdidas, brandReebok);
            context.TypeItems.AddRange(typeEquipment, typeAccessory, typeApparel);
            context.Items.AddRange(items);
            context.SaveChanges();
        }

        }
}