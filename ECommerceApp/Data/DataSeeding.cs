using ECommerceApp.Entities;
using ECommerceApp.Enums;
using Microsoft.EntityFrameworkCore;

namespace ECommerceApp.Data
{
    public static class DataSeeding
    {
        public static void SeedData(this ModelBuilder modelBuilder)
        {
            const string seedPasswordHash = "$2b$10$oSRZH.RmBuTu5GhguSMwrumb/jiWlbG/9yhUckKlOhVIarrFMogqi";
            const int homeKitchenCategoryId = 101;
            const int clothingCategoryId = 102;
            const int beautyCategoryId = 103;
            const int sportsCategoryId = 104;
            const int toysCategoryId = 105;

            const int headphonesProductId = 101;
            const int keyboardProductId = 102;
            const int dockingStationProductId = 103;
            const int cookwareProductId = 104;
            const int mugSetProductId = 105;
            const int tshirtPackProductId = 106;
            const int runningShoesProductId = 107;
            const int yogaMatProductId = 108;
            const int skincareProductId = 109;
            const int boardGameProductId = 110;
            const int mysteryBooksProductId = 111;
            const int smartwatchProductId = 112;
            const int discontinuedTrackerProductId = 113;

            const int adminCustomerId = 100;
            const int minhCustomerId = 101;
            const int anCustomerId = 102;
            const int lanCustomerId = 103;
            const int khoaCustomerId = 104;
            const int saraCustomerId = 105;

            const int minhHomeAddressId = 101;
            const int minhOfficeAddressId = 102;
            const int anHomeAddressId = 103;
            const int anOfficeAddressId = 104;
            const int lanHomeAddressId = 105;
            const int khoaHomeAddressId = 106;
            const int saraHomeAddressId = 107;
            const int lanOfficeAddressId = 108;

            const int deliveredPhoneOrderId = 101;
            const int laptopProcessingOrderId = 102;
            const int codShippedOrderId = 103;
            const int completedRefundOrderId = 104;
            const int rejectedCancellationOrderId = 105;
            const int pendingRefundOrderId = 106;
            const int failedPaymentOrderId = 107;
            const int deliveredBooksOrderId = 108;

            const int deliveredPhonePaymentId = 101;
            const int laptopProcessingPaymentId = 102;
            const int codShippedPaymentId = 103;
            const int completedRefundPaymentId = 104;
            const int rejectedCancellationPaymentId = 105;
            const int pendingRefundPaymentId = 106;
            const int failedPaymentId = 107;
            const int deliveredBooksPaymentId = 108;

            const int addressChangeCancellationId = 101;
            const int packedOrderCancellationId = 102;
            const int wrongColorCancellationId = 103;
            const int addressChangeRefundId = 101;
            const int wrongColorRefundId = 102;

            const int minhActiveCartId = 101;
            const int anCheckedOutCartId = 102;
            const int lanActiveCartId = 103;

            modelBuilder.Entity<Status>().HasData(
                new Status { Id = 1, Name = "Pending" },
                new Status { Id = 2, Name = "Processing" },
                new Status { Id = 3, Name = "Shipped" },
                new Status { Id = 4, Name = "Delivered" },
                new Status { Id = 5, Name = "Canceled" },

                new Status { Id = 6, Name = "Completed" },
                new Status { Id = 7, Name = "Failed" },

                new Status { Id = 8, Name = "Approved" },
                new Status { Id = 9, Name = "Rejected" },

                new Status { Id = 10, Name = "Refunded" }
            );

            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Electronics", Description = "Electronic devices and accessories", IsActive = true },
                new Category { Id = 2, Name = "Books", Description = "Books and magazines", IsActive = true },
                new Category { Id = homeKitchenCategoryId, Name = "Home & Kitchen", Description = "Cookware, drinkware, and everyday home essentials", IsActive = true },
                new Category { Id = clothingCategoryId, Name = "Clothing", Description = "Everyday apparel and wardrobe basics", IsActive = true },
                new Category { Id = beautyCategoryId, Name = "Beauty & Personal Care", Description = "Skincare, grooming, and personal care products", IsActive = true },
                new Category { Id = sportsCategoryId, Name = "Sports & Outdoors", Description = "Fitness gear and outdoor activity products", IsActive = true },
                new Category { Id = toysCategoryId, Name = "Toys & Games", Description = "Board games, puzzles, and family entertainment", IsActive = true }
            );

            modelBuilder.Entity<Product>().HasData(
                new Product
                {
                    Id = 1,
                    Name = "Smartphone",
                    Description = "Latest model smartphone with advanced features.",
                    Price = 699.99m,
                    StockQuantity = 50,
                    ImageUrl = "https://example.com/images/smartphone.jpg",
                    DiscountPercentage = 10,
                    CategoryId = 1,
                    IsAvailable = true
                },
                new Product
                {
                    Id = 2,
                    Name = "Laptop",
                    Description = "High-performance laptop suitable for all your needs.",
                    Price = 999.99m,
                    StockQuantity = 30,
                    ImageUrl = "https://example.com/images/laptop.jpg",
                    DiscountPercentage = 15,
                    CategoryId = 1,
                    IsAvailable = true
                },
                new Product
                {
                    Id = 3,
                    Name = "Science Fiction Novel",
                    Description = "A thrilling science fiction novel set in the future.",
                    Price = 19.99m,
                    StockQuantity = 100,
                    ImageUrl = "https://example.com/images/scifi-novel.jpg",
                    DiscountPercentage = 5,
                    CategoryId = 2,
                    IsAvailable = true
                },
                new Product
                {
                    Id = headphonesProductId,
                    Name = "Wireless Noise-Canceling Headphones",
                    Description = "Over-ear wireless headphones with active noise cancellation and long battery life.",
                    Price = 149.99m,
                    StockQuantity = 80,
                    ImageUrl = "https://example.com/images/wireless-headphones.jpg",
                    DiscountPercentage = 12,
                    CategoryId = 1,
                    IsAvailable = true
                },
                new Product
                {
                    Id = keyboardProductId,
                    Name = "Mechanical Keyboard",
                    Description = "Compact mechanical keyboard with tactile switches and white backlighting.",
                    Price = 89.99m,
                    StockQuantity = 65,
                    ImageUrl = "https://example.com/images/mechanical-keyboard.jpg",
                    DiscountPercentage = 8,
                    CategoryId = 1,
                    IsAvailable = true
                },
                new Product
                {
                    Id = dockingStationProductId,
                    Name = "USB-C Docking Station",
                    Description = "Multi-port USB-C docking station with HDMI, Ethernet, and fast charging support.",
                    Price = 129.99m,
                    StockQuantity = 40,
                    ImageUrl = "https://example.com/images/usb-c-docking-station.jpg",
                    DiscountPercentage = 10,
                    CategoryId = 1,
                    IsAvailable = true
                },
                new Product
                {
                    Id = cookwareProductId,
                    Name = "Stainless Steel Cookware Set",
                    Description = "Durable ten-piece stainless steel cookware set for daily home cooking.",
                    Price = 119.50m,
                    StockQuantity = 45,
                    ImageUrl = "https://example.com/images/cookware-set.jpg",
                    DiscountPercentage = 20,
                    CategoryId = homeKitchenCategoryId,
                    IsAvailable = true
                },
                new Product
                {
                    Id = mugSetProductId,
                    Name = "Ceramic Coffee Mug Set",
                    Description = "Set of four dishwasher-safe ceramic mugs for coffee, tea, and hot chocolate.",
                    Price = 24.99m,
                    StockQuantity = 150,
                    ImageUrl = "https://example.com/images/ceramic-mug-set.jpg",
                    DiscountPercentage = 0,
                    CategoryId = homeKitchenCategoryId,
                    IsAvailable = true
                },
                new Product
                {
                    Id = tshirtPackProductId,
                    Name = "Cotton T-Shirt Pack",
                    Description = "Three-pack of soft cotton crew neck t-shirts for everyday wear.",
                    Price = 34.99m,
                    StockQuantity = 120,
                    ImageUrl = "https://example.com/images/cotton-t-shirt-pack.jpg",
                    DiscountPercentage = 10,
                    CategoryId = clothingCategoryId,
                    IsAvailable = true
                },
                new Product
                {
                    Id = runningShoesProductId,
                    Name = "Lightweight Running Shoes",
                    Description = "Breathable running shoes with cushioned soles for road training.",
                    Price = 79.99m,
                    StockQuantity = 60,
                    ImageUrl = "https://example.com/images/running-shoes.jpg",
                    DiscountPercentage = 15,
                    CategoryId = sportsCategoryId,
                    IsAvailable = true
                },
                new Product
                {
                    Id = yogaMatProductId,
                    Name = "Non-Slip Yoga Mat",
                    Description = "Lightweight non-slip yoga mat with carrying strap for studio or home workouts.",
                    Price = 29.99m,
                    StockQuantity = 90,
                    ImageUrl = "https://example.com/images/yoga-mat.jpg",
                    DiscountPercentage = 5,
                    CategoryId = sportsCategoryId,
                    IsAvailable = true
                },
                new Product
                {
                    Id = skincareProductId,
                    Name = "Skincare Starter Kit",
                    Description = "Gentle cleanser, moisturizer, and sunscreen starter kit for daily skincare.",
                    Price = 59.99m,
                    StockQuantity = 70,
                    ImageUrl = "https://example.com/images/skincare-starter-kit.jpg",
                    DiscountPercentage = 10,
                    CategoryId = beautyCategoryId,
                    IsAvailable = true
                },
                new Product
                {
                    Id = boardGameProductId,
                    Name = "Family Strategy Board Game",
                    Description = "Easy-to-learn strategy board game designed for family game nights.",
                    Price = 39.99m,
                    StockQuantity = 85,
                    ImageUrl = "https://example.com/images/family-board-game.jpg",
                    DiscountPercentage = 5,
                    CategoryId = toysCategoryId,
                    IsAvailable = true
                },
                new Product
                {
                    Id = mysteryBooksProductId,
                    Name = "Mystery Paperback Collection",
                    Description = "Three bestselling mystery paperbacks bundled for weekend reading.",
                    Price = 29.99m,
                    StockQuantity = 110,
                    ImageUrl = "https://example.com/images/mystery-paperback-collection.jpg",
                    DiscountPercentage = 0,
                    CategoryId = 2,
                    IsAvailable = true
                },
                new Product
                {
                    Id = smartwatchProductId,
                    Name = "Smartwatch",
                    Description = "Water-resistant smartwatch with heart-rate tracking and phone notifications.",
                    Price = 249.99m,
                    StockQuantity = 35,
                    ImageUrl = "https://example.com/images/smartwatch.jpg",
                    DiscountPercentage = 18,
                    CategoryId = 1,
                    IsAvailable = true
                },
                new Product
                {
                    Id = discontinuedTrackerProductId,
                    Name = "Discontinued Fitness Tracker",
                    Description = "Previous-generation activity tracker kept unavailable because it is no longer stocked.",
                    Price = 89.99m,
                    StockQuantity = 0,
                    ImageUrl = "https://example.com/images/discontinued-fitness-tracker.jpg",
                    DiscountPercentage = 30,
                    CategoryId = 1,
                    IsAvailable = false
                }
            );

            modelBuilder.Entity<Customer>().HasData(
                new Customer
                {
                    Id = adminCustomerId,
                    FirstName = "System",
                    LastName = "Admin",
                    Email = "admin@example.com",
                    PhoneNumber = "+84900000000",
                    DateOfBirth = new DateTime(1990, 1, 1),
                    Password = seedPasswordHash,
                    IsActive = true
                },
                new Customer
                {
                    Id = minhCustomerId,
                    FirstName = "Minh",
                    LastName = "Nguyen",
                    Email = "minh.nguyen@example.com",
                    PhoneNumber = "+84901234567",
                    DateOfBirth = new DateTime(1995, 3, 18),
                    Password = seedPasswordHash,
                    IsActive = true
                },
                new Customer
                {
                    Id = anCustomerId,
                    FirstName = "An",
                    LastName = "Tran",
                    Email = "an.tran@example.com",
                    PhoneNumber = "+84909876543",
                    DateOfBirth = new DateTime(1992, 7, 24),
                    Password = seedPasswordHash,
                    IsActive = true
                },
                new Customer
                {
                    Id = lanCustomerId,
                    FirstName = "Lan",
                    LastName = "Pham",
                    Email = "lan.pham@example.com",
                    PhoneNumber = "+84911222333",
                    DateOfBirth = new DateTime(1988, 11, 5),
                    Password = seedPasswordHash,
                    IsActive = true
                },
                new Customer
                {
                    Id = khoaCustomerId,
                    FirstName = "Khoa",
                    LastName = "Le",
                    Email = "khoa.le@example.com",
                    PhoneNumber = "+84933444555",
                    DateOfBirth = new DateTime(2000, 1, 12),
                    Password = seedPasswordHash,
                    IsActive = true
                },
                new Customer
                {
                    Id = saraCustomerId,
                    FirstName = "Sara",
                    LastName = "Johnson",
                    Email = "sara.johnson@example.com",
                    PhoneNumber = "+12025550178",
                    DateOfBirth = new DateTime(1997, 5, 30),
                    Password = seedPasswordHash,
                    IsActive = true
                }
            );

            modelBuilder.Entity<Address>().HasData(
                new Address
                {
                    Id = minhHomeAddressId,
                    CustomerId = minhCustomerId,
                    AddressLine1 = "12 Nguyen Hue Street",
                    AddressLine2 = "Apartment 8A",
                    City = "Ho Chi Minh City",
                    State = "Ho Chi Minh",
                    PostalCode = "700000",
                    Country = "Vietnam"
                },
                new Address
                {
                    Id = minhOfficeAddressId,
                    CustomerId = minhCustomerId,
                    AddressLine1 = "88 Pasteur Street",
                    AddressLine2 = "Office Reception",
                    City = "Ho Chi Minh City",
                    State = "Ho Chi Minh",
                    PostalCode = "700000",
                    Country = "Vietnam"
                },
                new Address
                {
                    Id = anHomeAddressId,
                    CustomerId = anCustomerId,
                    AddressLine1 = "45 Tran Duy Hung Street",
                    AddressLine2 = "Floor 12",
                    City = "Hanoi",
                    State = "Hanoi",
                    PostalCode = "100000",
                    Country = "Vietnam"
                },
                new Address
                {
                    Id = anOfficeAddressId,
                    CustomerId = anCustomerId,
                    AddressLine1 = "19 Ly Thuong Kiet Street",
                    AddressLine2 = "Suite 402",
                    City = "Hanoi",
                    State = "Hanoi",
                    PostalCode = "100000",
                    Country = "Vietnam"
                },
                new Address
                {
                    Id = lanHomeAddressId,
                    CustomerId = lanCustomerId,
                    AddressLine1 = "7 Bach Dang Street",
                    AddressLine2 = "Unit 3B",
                    City = "Da Nang",
                    State = "Da Nang",
                    PostalCode = "550000",
                    Country = "Vietnam"
                },
                new Address
                {
                    Id = khoaHomeAddressId,
                    CustomerId = khoaCustomerId,
                    AddressLine1 = "23 Nguyen Trai Street",
                    AddressLine2 = "Townhouse",
                    City = "Can Tho",
                    State = "Can Tho",
                    PostalCode = "900000",
                    Country = "Vietnam"
                },
                new Address
                {
                    Id = saraHomeAddressId,
                    CustomerId = saraCustomerId,
                    AddressLine1 = "102 Pine Avenue",
                    AddressLine2 = "Apartment 14C",
                    City = "Seattle",
                    State = "Washington",
                    PostalCode = "98101",
                    Country = "United States"
                },
                new Address
                {
                    Id = lanOfficeAddressId,
                    CustomerId = lanCustomerId,
                    AddressLine1 = "101 Nguyen Van Linh Street",
                    AddressLine2 = "Office Mailroom",
                    City = "Da Nang",
                    State = "Da Nang",
                    PostalCode = "550000",
                    Country = "Vietnam"
                }
            );

            modelBuilder.Entity<Order>().HasData(
                new Order
                {
                    Id = deliveredPhoneOrderId,
                    OrderNumber = "ORD-2026-0001",
                    OrderDate = new DateTime(2026, 5, 10, 9, 15, 0),
                    CustomerId = minhCustomerId,
                    BillingAddressId = minhHomeAddressId,
                    ShippingAddressId = minhOfficeAddressId,
                    TotalBaseAmount = 749.97m,
                    TotalDiscountAmount = 70.00m,
                    ShippingCost = 5.00m,
                    TotalAmount = 684.97m,
                    OrderStatus = OrderStatus.Delivered
                },
                new Order
                {
                    Id = laptopProcessingOrderId,
                    OrderNumber = "ORD-2026-0002",
                    OrderDate = new DateTime(2026, 5, 12, 14, 30, 0),
                    CustomerId = anCustomerId,
                    BillingAddressId = anHomeAddressId,
                    ShippingAddressId = anHomeAddressId,
                    TotalBaseAmount = 1089.98m,
                    TotalDiscountAmount = 157.20m,
                    ShippingCost = 0.00m,
                    TotalAmount = 932.78m,
                    OrderStatus = OrderStatus.Processing
                },
                new Order
                {
                    Id = codShippedOrderId,
                    OrderNumber = "ORD-2026-0003",
                    OrderDate = new DateTime(2026, 5, 14, 18, 5, 0),
                    CustomerId = lanCustomerId,
                    BillingAddressId = lanHomeAddressId,
                    ShippingAddressId = lanOfficeAddressId,
                    TotalBaseAmount = 239.48m,
                    TotalDiscountAmount = 35.90m,
                    ShippingCost = 7.50m,
                    TotalAmount = 211.08m,
                    OrderStatus = OrderStatus.Shipped
                },
                new Order
                {
                    Id = completedRefundOrderId,
                    OrderNumber = "ORD-2026-0004",
                    OrderDate = new DateTime(2026, 5, 15, 10, 45, 0),
                    CustomerId = khoaCustomerId,
                    BillingAddressId = khoaHomeAddressId,
                    ShippingAddressId = khoaHomeAddressId,
                    TotalBaseAmount = 329.98m,
                    TotalDiscountAmount = 57.00m,
                    ShippingCost = 5.00m,
                    TotalAmount = 277.98m,
                    OrderStatus = OrderStatus.Canceled
                },
                new Order
                {
                    Id = rejectedCancellationOrderId,
                    OrderNumber = "ORD-2026-0005",
                    OrderDate = new DateTime(2026, 5, 16, 8, 20, 0),
                    CustomerId = anCustomerId,
                    BillingAddressId = anHomeAddressId,
                    ShippingAddressId = anOfficeAddressId,
                    TotalBaseAmount = 279.98m,
                    TotalDiscountAmount = 31.00m,
                    ShippingCost = 0.00m,
                    TotalAmount = 248.98m,
                    OrderStatus = OrderStatus.Processing
                },
                new Order
                {
                    Id = pendingRefundOrderId,
                    OrderNumber = "ORD-2026-0006",
                    OrderDate = new DateTime(2026, 5, 17, 12, 10, 0),
                    CustomerId = minhCustomerId,
                    BillingAddressId = minhHomeAddressId,
                    ShippingAddressId = minhHomeAddressId,
                    TotalBaseAmount = 149.99m,
                    TotalDiscountAmount = 18.00m,
                    ShippingCost = 5.00m,
                    TotalAmount = 136.99m,
                    OrderStatus = OrderStatus.Canceled
                },
                new Order
                {
                    Id = failedPaymentOrderId,
                    OrderNumber = "ORD-2026-0007",
                    OrderDate = new DateTime(2026, 5, 18, 20, 40, 0),
                    CustomerId = saraCustomerId,
                    BillingAddressId = saraHomeAddressId,
                    ShippingAddressId = saraHomeAddressId,
                    TotalBaseAmount = 69.98m,
                    TotalDiscountAmount = 2.00m,
                    ShippingCost = 5.00m,
                    TotalAmount = 72.98m,
                    OrderStatus = OrderStatus.Pending
                },
                new Order
                {
                    Id = deliveredBooksOrderId,
                    OrderNumber = "ORD-2026-0008",
                    OrderDate = new DateTime(2026, 5, 5, 16, 30, 0),
                    CustomerId = lanCustomerId,
                    BillingAddressId = lanHomeAddressId,
                    ShippingAddressId = lanHomeAddressId,
                    TotalBaseAmount = 69.97m,
                    TotalDiscountAmount = 3.50m,
                    ShippingCost = 5.00m,
                    TotalAmount = 71.47m,
                    OrderStatus = OrderStatus.Delivered
                }
            );

            modelBuilder.Entity<OrderItem>().HasData(
                new OrderItem { Id = 101, OrderId = deliveredPhoneOrderId, ProductId = 1, Quantity = 1, UnitPrice = 699.99m, Discount = 70.00m, TotalPrice = 629.99m },
                new OrderItem { Id = 102, OrderId = deliveredPhoneOrderId, ProductId = mugSetProductId, Quantity = 2, UnitPrice = 24.99m, Discount = 0.00m, TotalPrice = 49.98m },
                new OrderItem { Id = 103, OrderId = laptopProcessingOrderId, ProductId = 2, Quantity = 1, UnitPrice = 999.99m, Discount = 150.00m, TotalPrice = 849.99m },
                new OrderItem { Id = 104, OrderId = laptopProcessingOrderId, ProductId = keyboardProductId, Quantity = 1, UnitPrice = 89.99m, Discount = 7.20m, TotalPrice = 82.79m },
                new OrderItem { Id = 105, OrderId = codShippedOrderId, ProductId = cookwareProductId, Quantity = 1, UnitPrice = 119.50m, Discount = 23.90m, TotalPrice = 95.60m },
                new OrderItem { Id = 106, OrderId = codShippedOrderId, ProductId = skincareProductId, Quantity = 2, UnitPrice = 59.99m, Discount = 12.00m, TotalPrice = 107.98m },
                new OrderItem { Id = 107, OrderId = completedRefundOrderId, ProductId = smartwatchProductId, Quantity = 1, UnitPrice = 249.99m, Discount = 45.00m, TotalPrice = 204.99m },
                new OrderItem { Id = 108, OrderId = completedRefundOrderId, ProductId = runningShoesProductId, Quantity = 1, UnitPrice = 79.99m, Discount = 12.00m, TotalPrice = 67.99m },
                new OrderItem { Id = 109, OrderId = rejectedCancellationOrderId, ProductId = dockingStationProductId, Quantity = 1, UnitPrice = 129.99m, Discount = 13.00m, TotalPrice = 116.99m },
                new OrderItem { Id = 110, OrderId = rejectedCancellationOrderId, ProductId = headphonesProductId, Quantity = 1, UnitPrice = 149.99m, Discount = 18.00m, TotalPrice = 131.99m },
                new OrderItem { Id = 111, OrderId = pendingRefundOrderId, ProductId = headphonesProductId, Quantity = 1, UnitPrice = 149.99m, Discount = 18.00m, TotalPrice = 131.99m },
                new OrderItem { Id = 112, OrderId = failedPaymentOrderId, ProductId = boardGameProductId, Quantity = 1, UnitPrice = 39.99m, Discount = 2.00m, TotalPrice = 37.99m },
                new OrderItem { Id = 113, OrderId = failedPaymentOrderId, ProductId = mysteryBooksProductId, Quantity = 1, UnitPrice = 29.99m, Discount = 0.00m, TotalPrice = 29.99m },
                new OrderItem { Id = 114, OrderId = deliveredBooksOrderId, ProductId = 3, Quantity = 2, UnitPrice = 19.99m, Discount = 2.00m, TotalPrice = 37.98m },
                new OrderItem { Id = 115, OrderId = deliveredBooksOrderId, ProductId = yogaMatProductId, Quantity = 1, UnitPrice = 29.99m, Discount = 1.50m, TotalPrice = 28.49m }
            );

            modelBuilder.Entity<Payment>().HasData(
                new Payment
                {
                    Id = deliveredPhonePaymentId,
                    OrderId = deliveredPhoneOrderId,
                    PaymentMethod = "CreditCard",
                    TransactionId = "TXN-CARD-0001",
                    Amount = 684.97m,
                    PaymentDate = new DateTime(2026, 5, 10, 9, 20, 0),
                    Status = PaymentStatus.Completed
                },
                new Payment
                {
                    Id = laptopProcessingPaymentId,
                    OrderId = laptopProcessingOrderId,
                    PaymentMethod = "VNPAY",
                    TransactionId = "TXN-VNPAY-0002",
                    Amount = 932.78m,
                    PaymentDate = new DateTime(2026, 5, 12, 14, 33, 0),
                    Status = PaymentStatus.Completed
                },
                new Payment
                {
                    Id = codShippedPaymentId,
                    OrderId = codShippedOrderId,
                    PaymentMethod = "COD",
                    TransactionId = null,
                    Amount = 211.08m,
                    PaymentDate = new DateTime(2026, 5, 14, 18, 6, 0),
                    Status = PaymentStatus.Pending
                },
                new Payment
                {
                    Id = completedRefundPaymentId,
                    OrderId = completedRefundOrderId,
                    PaymentMethod = "CreditCard",
                    TransactionId = "TXN-CARD-0004",
                    Amount = 277.98m,
                    PaymentDate = new DateTime(2026, 5, 15, 10, 47, 0),
                    Status = PaymentStatus.Refunded
                },
                new Payment
                {
                    Id = rejectedCancellationPaymentId,
                    OrderId = rejectedCancellationOrderId,
                    PaymentMethod = "PayPal",
                    TransactionId = "TXN-PAYPAL-0005",
                    Amount = 248.98m,
                    PaymentDate = new DateTime(2026, 5, 16, 8, 22, 0),
                    Status = PaymentStatus.Completed
                },
                new Payment
                {
                    Id = pendingRefundPaymentId,
                    OrderId = pendingRefundOrderId,
                    PaymentMethod = "Stripe",
                    TransactionId = "TXN-STRIPE-0006",
                    Amount = 136.99m,
                    PaymentDate = new DateTime(2026, 5, 17, 12, 12, 0),
                    Status = PaymentStatus.Completed
                },
                new Payment
                {
                    Id = failedPaymentId,
                    OrderId = failedPaymentOrderId,
                    PaymentMethod = "CreditCard",
                    TransactionId = "TXN-FAILED-0007",
                    Amount = 72.98m,
                    PaymentDate = new DateTime(2026, 5, 18, 20, 41, 0),
                    Status = PaymentStatus.Failed
                },
                new Payment
                {
                    Id = deliveredBooksPaymentId,
                    OrderId = deliveredBooksOrderId,
                    PaymentMethod = "COD",
                    TransactionId = "COD-RECEIPT-0008",
                    Amount = 71.47m,
                    PaymentDate = new DateTime(2026, 5, 8, 11, 15, 0),
                    Status = PaymentStatus.Completed
                }
            );

            modelBuilder.Entity<Cancellation>().HasData(
                new Cancellation
                {
                    Id = addressChangeCancellationId,
                    OrderId = completedRefundOrderId,
                    Reason = "Customer requested cancellation before shipment because the delivery address changed.",
                    Status = CancellationStatus.Approved,
                    RequestedAt = new DateTime(2026, 5, 15, 11, 5, 0),
                    ProcessedAt = new DateTime(2026, 5, 15, 11, 30, 0),
                    ProcessedBy = 1,
                    OrderAmount = 277.98m,
                    CancellationCharges = 0.00m,
                    Remarks = "Approved before fulfillment; full refund issued."
                },
                new Cancellation
                {
                    Id = packedOrderCancellationId,
                    OrderId = rejectedCancellationOrderId,
                    Reason = "Customer tried to cancel after warehouse packing had started.",
                    Status = CancellationStatus.Rejected,
                    RequestedAt = new DateTime(2026, 5, 16, 9, 0, 0),
                    ProcessedAt = new DateTime(2026, 5, 16, 9, 25, 0),
                    ProcessedBy = 1,
                    OrderAmount = 248.98m,
                    CancellationCharges = 0.00m,
                    Remarks = "Rejected because the order had already entered packing."
                },
                new Cancellation
                {
                    Id = wrongColorCancellationId,
                    OrderId = pendingRefundOrderId,
                    Reason = "The customer selected the wrong headphone color.",
                    Status = CancellationStatus.Approved,
                    RequestedAt = new DateTime(2026, 5, 17, 12, 30, 0),
                    ProcessedAt = new DateTime(2026, 5, 17, 13, 0, 0),
                    ProcessedBy = 1,
                    OrderAmount = 136.99m,
                    CancellationCharges = 0.00m,
                    Remarks = "Approved; refund is waiting for gateway confirmation."
                }
            );

            modelBuilder.Entity<Refund>().HasData(
                new Refund
                {
                    Id = addressChangeRefundId,
                    CancellationId = addressChangeCancellationId,
                    PaymentId = completedRefundPaymentId,
                    Amount = 277.98m,
                    Status = RefundStatus.Completed,
                    RefundMethod = RefundMethod.Original.ToString(),
                    RefundReason = "Full refund for approved cancellation before shipment.",
                    TransactionId = "RFND-0001",
                    InitiatedAt = new DateTime(2026, 5, 15, 11, 35, 0),
                    CompletedAt = new DateTime(2026, 5, 15, 12, 0, 0),
                    ProcessedBy = 1
                },
                new Refund
                {
                    Id = wrongColorRefundId,
                    CancellationId = wrongColorCancellationId,
                    PaymentId = pendingRefundPaymentId,
                    Amount = 136.99m,
                    Status = RefundStatus.Pending,
                    RefundMethod = RefundMethod.Stripe.ToString(),
                    RefundReason = "Pending refund for approved item color cancellation.",
                    TransactionId = null,
                    InitiatedAt = new DateTime(2026, 5, 17, 13, 5, 0),
                    CompletedAt = null,
                    ProcessedBy = null
                }
            );

            modelBuilder.Entity<Cart>().HasData(
                new Cart
                {
                    Id = minhActiveCartId,
                    CustomerId = minhCustomerId,
                    IsCheckedOut = false,
                    CreatedAt = new DateTime(2026, 5, 20, 10, 0, 0),
                    UpdatedAt = new DateTime(2026, 5, 20, 10, 18, 0)
                },
                new Cart
                {
                    Id = anCheckedOutCartId,
                    CustomerId = anCustomerId,
                    IsCheckedOut = true,
                    CreatedAt = new DateTime(2026, 5, 16, 8, 10, 0),
                    UpdatedAt = new DateTime(2026, 5, 16, 8, 22, 0)
                },
                new Cart
                {
                    Id = lanActiveCartId,
                    CustomerId = lanCustomerId,
                    IsCheckedOut = false,
                    CreatedAt = new DateTime(2026, 5, 21, 19, 30, 0),
                    UpdatedAt = new DateTime(2026, 5, 21, 19, 45, 0)
                }
            );

            modelBuilder.Entity<CartItem>().HasData(
                new CartItem
                {
                    Id = 101,
                    CartId = minhActiveCartId,
                    ProductId = tshirtPackProductId,
                    Quantity = 2,
                    UnitPrice = 34.99m,
                    Discount = 7.00m,
                    TotalPrice = 62.98m,
                    CreatedAt = new DateTime(2026, 5, 20, 10, 1, 0),
                    UpdatedAt = new DateTime(2026, 5, 20, 10, 15, 0)
                },
                new CartItem
                {
                    Id = 102,
                    CartId = minhActiveCartId,
                    ProductId = yogaMatProductId,
                    Quantity = 1,
                    UnitPrice = 29.99m,
                    Discount = 1.50m,
                    TotalPrice = 28.49m,
                    CreatedAt = new DateTime(2026, 5, 20, 10, 18, 0),
                    UpdatedAt = new DateTime(2026, 5, 20, 10, 18, 0)
                },
                new CartItem
                {
                    Id = 103,
                    CartId = anCheckedOutCartId,
                    ProductId = dockingStationProductId,
                    Quantity = 1,
                    UnitPrice = 129.99m,
                    Discount = 13.00m,
                    TotalPrice = 116.99m,
                    CreatedAt = new DateTime(2026, 5, 16, 8, 11, 0),
                    UpdatedAt = new DateTime(2026, 5, 16, 8, 20, 0)
                },
                new CartItem
                {
                    Id = 104,
                    CartId = anCheckedOutCartId,
                    ProductId = headphonesProductId,
                    Quantity = 1,
                    UnitPrice = 149.99m,
                    Discount = 18.00m,
                    TotalPrice = 131.99m,
                    CreatedAt = new DateTime(2026, 5, 16, 8, 12, 0),
                    UpdatedAt = new DateTime(2026, 5, 16, 8, 20, 0)
                },
                new CartItem
                {
                    Id = 105,
                    CartId = lanActiveCartId,
                    ProductId = 3,
                    Quantity = 3,
                    UnitPrice = 19.99m,
                    Discount = 3.00m,
                    TotalPrice = 56.97m,
                    CreatedAt = new DateTime(2026, 5, 21, 19, 31, 0),
                    UpdatedAt = new DateTime(2026, 5, 21, 19, 45, 0)
                },
                new CartItem
                {
                    Id = 106,
                    CartId = lanActiveCartId,
                    ProductId = skincareProductId,
                    Quantity = 1,
                    UnitPrice = 59.99m,
                    Discount = 6.00m,
                    TotalPrice = 53.99m,
                    CreatedAt = new DateTime(2026, 5, 21, 19, 33, 0),
                    UpdatedAt = new DateTime(2026, 5, 21, 19, 45, 0)
                }
            );

            modelBuilder.Entity<Feedback>().HasData(
                new Feedback
                {
                    Id = 101,
                    CustomerId = minhCustomerId,
                    ProductId = 1,
                    Rating = 5,
                    Comment = "Fast delivery and the phone battery easily lasts through a busy day.",
                    CreatedAt = new DateTime(2026, 5, 13, 8, 45, 0),
                    UpdatedAt = new DateTime(2026, 5, 13, 8, 45, 0)
                },
                new Feedback
                {
                    Id = 102,
                    CustomerId = minhCustomerId,
                    ProductId = mugSetProductId,
                    Rating = 4,
                    Comment = "The mugs arrived well packed and match the product photos.",
                    CreatedAt = new DateTime(2026, 5, 13, 8, 50, 0),
                    UpdatedAt = new DateTime(2026, 5, 13, 8, 50, 0)
                },
                new Feedback
                {
                    Id = 103,
                    CustomerId = lanCustomerId,
                    ProductId = 3,
                    Rating = 5,
                    Comment = "A fun read with a good pace; the bundle was worth adding to my order.",
                    CreatedAt = new DateTime(2026, 5, 9, 17, 10, 0),
                    UpdatedAt = new DateTime(2026, 5, 9, 17, 10, 0)
                },
                new Feedback
                {
                    Id = 104,
                    CustomerId = lanCustomerId,
                    ProductId = yogaMatProductId,
                    Rating = 4,
                    Comment = "Good grip for home workouts and easy to carry to class.",
                    CreatedAt = new DateTime(2026, 5, 9, 17, 16, 0),
                    UpdatedAt = new DateTime(2026, 5, 9, 17, 16, 0)
                }
            );
        }
    }
}
