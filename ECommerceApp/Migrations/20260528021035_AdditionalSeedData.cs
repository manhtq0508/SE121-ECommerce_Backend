using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ECommerceApp.Migrations
{
    /// <inheritdoc />
    public partial class AdditionalSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Description", "IsActive", "Name" },
                values: new object[,]
                {
                    { 101, "Cookware, drinkware, and everyday home essentials", true, "Home & Kitchen" },
                    { 102, "Everyday apparel and wardrobe basics", true, "Clothing" },
                    { 103, "Skincare, grooming, and personal care products", true, "Beauty & Personal Care" },
                    { 104, "Fitness gear and outdoor activity products", true, "Sports & Outdoors" },
                    { 105, "Board games, puzzles, and family entertainment", true, "Toys & Games" }
                });

            migrationBuilder.InsertData(
                table: "Customers",
                columns: new[] { "Id", "DateOfBirth", "Email", "FirstName", "IsActive", "LastName", "Password", "PhoneNumber" },
                values: new object[,]
                {
                    { 100, new DateTime(1990, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "admin@example.com", "System", true, "Admin", "$2b$10$oSRZH.RmBuTu5GhguSMwrumb/jiWlbG/9yhUckKlOhVIarrFMogqi", "+84900000000" },
                    { 101, new DateTime(1995, 3, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "minh.nguyen@example.com", "Minh", true, "Nguyen", "$2b$10$oSRZH.RmBuTu5GhguSMwrumb/jiWlbG/9yhUckKlOhVIarrFMogqi", "+84901234567" },
                    { 102, new DateTime(1992, 7, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), "an.tran@example.com", "An", true, "Tran", "$2b$10$oSRZH.RmBuTu5GhguSMwrumb/jiWlbG/9yhUckKlOhVIarrFMogqi", "+84909876543" },
                    { 103, new DateTime(1988, 11, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "lan.pham@example.com", "Lan", true, "Pham", "$2b$10$oSRZH.RmBuTu5GhguSMwrumb/jiWlbG/9yhUckKlOhVIarrFMogqi", "+84911222333" },
                    { 104, new DateTime(2000, 1, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), "khoa.le@example.com", "Khoa", true, "Le", "$2b$10$oSRZH.RmBuTu5GhguSMwrumb/jiWlbG/9yhUckKlOhVIarrFMogqi", "+84933444555" },
                    { 105, new DateTime(1997, 5, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), "sara.johnson@example.com", "Sara", true, "Johnson", "$2b$10$oSRZH.RmBuTu5GhguSMwrumb/jiWlbG/9yhUckKlOhVIarrFMogqi", "+12025550178" }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "CategoryId", "Description", "DiscountPercentage", "ImageUrl", "IsAvailable", "Name", "Price", "StockQuantity" },
                values: new object[,]
                {
                    { 101, 1, "Over-ear wireless headphones with active noise cancellation and long battery life.", 12, "https://example.com/images/wireless-headphones.jpg", true, "Wireless Noise-Canceling Headphones", 149.99m, 80 },
                    { 102, 1, "Compact mechanical keyboard with tactile switches and white backlighting.", 8, "https://example.com/images/mechanical-keyboard.jpg", true, "Mechanical Keyboard", 89.99m, 65 },
                    { 103, 1, "Multi-port USB-C docking station with HDMI, Ethernet, and fast charging support.", 10, "https://example.com/images/usb-c-docking-station.jpg", true, "USB-C Docking Station", 129.99m, 40 },
                    { 111, 2, "Three bestselling mystery paperbacks bundled for weekend reading.", 0, "https://example.com/images/mystery-paperback-collection.jpg", true, "Mystery Paperback Collection", 29.99m, 110 },
                    { 112, 1, "Water-resistant smartwatch with heart-rate tracking and phone notifications.", 18, "https://example.com/images/smartwatch.jpg", true, "Smartwatch", 249.99m, 35 },
                    { 113, 1, "Previous-generation activity tracker kept unavailable because it is no longer stocked.", 30, "https://example.com/images/discontinued-fitness-tracker.jpg", false, "Discontinued Fitness Tracker", 89.99m, 0 }
                });

            migrationBuilder.InsertData(
                table: "Addresses",
                columns: new[] { "Id", "AddressLine1", "AddressLine2", "City", "Country", "CustomerId", "PostalCode", "State" },
                values: new object[,]
                {
                    { 101, "12 Nguyen Hue Street", "Apartment 8A", "Ho Chi Minh City", "Vietnam", 101, "700000", "Ho Chi Minh" },
                    { 102, "88 Pasteur Street", "Office Reception", "Ho Chi Minh City", "Vietnam", 101, "700000", "Ho Chi Minh" },
                    { 103, "45 Tran Duy Hung Street", "Floor 12", "Hanoi", "Vietnam", 102, "100000", "Hanoi" },
                    { 104, "19 Ly Thuong Kiet Street", "Suite 402", "Hanoi", "Vietnam", 102, "100000", "Hanoi" },
                    { 105, "7 Bach Dang Street", "Unit 3B", "Da Nang", "Vietnam", 103, "550000", "Da Nang" },
                    { 106, "23 Nguyen Trai Street", "Townhouse", "Can Tho", "Vietnam", 104, "900000", "Can Tho" },
                    { 107, "102 Pine Avenue", "Apartment 14C", "Seattle", "United States", 105, "98101", "Washington" },
                    { 108, "101 Nguyen Van Linh Street", "Office Mailroom", "Da Nang", "Vietnam", 103, "550000", "Da Nang" }
                });

            migrationBuilder.InsertData(
                table: "Carts",
                columns: new[] { "Id", "CreatedAt", "CustomerId", "IsCheckedOut", "UpdatedAt" },
                values: new object[,]
                {
                    { 101, new DateTime(2026, 5, 20, 10, 0, 0, 0, DateTimeKind.Unspecified), 101, false, new DateTime(2026, 5, 20, 10, 18, 0, 0, DateTimeKind.Unspecified) },
                    { 102, new DateTime(2026, 5, 16, 8, 10, 0, 0, DateTimeKind.Unspecified), 102, true, new DateTime(2026, 5, 16, 8, 22, 0, 0, DateTimeKind.Unspecified) },
                    { 103, new DateTime(2026, 5, 21, 19, 30, 0, 0, DateTimeKind.Unspecified), 103, false, new DateTime(2026, 5, 21, 19, 45, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.InsertData(
                table: "Feedbacks",
                columns: new[] { "Id", "Comment", "CreatedAt", "CustomerId", "ProductId", "Rating", "UpdatedAt" },
                values: new object[,]
                {
                    { 101, "Fast delivery and the phone battery easily lasts through a busy day.", new DateTime(2026, 5, 13, 8, 45, 0, 0, DateTimeKind.Unspecified), 101, 1, 5, new DateTime(2026, 5, 13, 8, 45, 0, 0, DateTimeKind.Unspecified) },
                    { 103, "A fun read with a good pace; the bundle was worth adding to my order.", new DateTime(2026, 5, 9, 17, 10, 0, 0, DateTimeKind.Unspecified), 103, 3, 5, new DateTime(2026, 5, 9, 17, 10, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "CategoryId", "Description", "DiscountPercentage", "ImageUrl", "IsAvailable", "Name", "Price", "StockQuantity" },
                values: new object[,]
                {
                    { 104, 101, "Durable ten-piece stainless steel cookware set for daily home cooking.", 20, "https://example.com/images/cookware-set.jpg", true, "Stainless Steel Cookware Set", 119.50m, 45 },
                    { 105, 101, "Set of four dishwasher-safe ceramic mugs for coffee, tea, and hot chocolate.", 0, "https://example.com/images/ceramic-mug-set.jpg", true, "Ceramic Coffee Mug Set", 24.99m, 150 },
                    { 106, 102, "Three-pack of soft cotton crew neck t-shirts for everyday wear.", 10, "https://example.com/images/cotton-t-shirt-pack.jpg", true, "Cotton T-Shirt Pack", 34.99m, 120 },
                    { 107, 104, "Breathable running shoes with cushioned soles for road training.", 15, "https://example.com/images/running-shoes.jpg", true, "Lightweight Running Shoes", 79.99m, 60 },
                    { 108, 104, "Lightweight non-slip yoga mat with carrying strap for studio or home workouts.", 5, "https://example.com/images/yoga-mat.jpg", true, "Non-Slip Yoga Mat", 29.99m, 90 },
                    { 109, 103, "Gentle cleanser, moisturizer, and sunscreen starter kit for daily skincare.", 10, "https://example.com/images/skincare-starter-kit.jpg", true, "Skincare Starter Kit", 59.99m, 70 },
                    { 110, 105, "Easy-to-learn strategy board game designed for family game nights.", 5, "https://example.com/images/family-board-game.jpg", true, "Family Strategy Board Game", 39.99m, 85 }
                });

            migrationBuilder.InsertData(
                table: "CartItems",
                columns: new[] { "Id", "CartId", "CreatedAt", "Discount", "ProductId", "Quantity", "TotalPrice", "UnitPrice", "UpdatedAt" },
                values: new object[,]
                {
                    { 101, 101, new DateTime(2026, 5, 20, 10, 1, 0, 0, DateTimeKind.Unspecified), 7.00m, 106, 2, 62.98m, 34.99m, new DateTime(2026, 5, 20, 10, 15, 0, 0, DateTimeKind.Unspecified) },
                    { 102, 101, new DateTime(2026, 5, 20, 10, 18, 0, 0, DateTimeKind.Unspecified), 1.50m, 108, 1, 28.49m, 29.99m, new DateTime(2026, 5, 20, 10, 18, 0, 0, DateTimeKind.Unspecified) },
                    { 103, 102, new DateTime(2026, 5, 16, 8, 11, 0, 0, DateTimeKind.Unspecified), 13.00m, 103, 1, 116.99m, 129.99m, new DateTime(2026, 5, 16, 8, 20, 0, 0, DateTimeKind.Unspecified) },
                    { 104, 102, new DateTime(2026, 5, 16, 8, 12, 0, 0, DateTimeKind.Unspecified), 18.00m, 101, 1, 131.99m, 149.99m, new DateTime(2026, 5, 16, 8, 20, 0, 0, DateTimeKind.Unspecified) },
                    { 105, 103, new DateTime(2026, 5, 21, 19, 31, 0, 0, DateTimeKind.Unspecified), 3.00m, 3, 3, 56.97m, 19.99m, new DateTime(2026, 5, 21, 19, 45, 0, 0, DateTimeKind.Unspecified) },
                    { 106, 103, new DateTime(2026, 5, 21, 19, 33, 0, 0, DateTimeKind.Unspecified), 6.00m, 109, 1, 53.99m, 59.99m, new DateTime(2026, 5, 21, 19, 45, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.InsertData(
                table: "Feedbacks",
                columns: new[] { "Id", "Comment", "CreatedAt", "CustomerId", "ProductId", "Rating", "UpdatedAt" },
                values: new object[,]
                {
                    { 102, "The mugs arrived well packed and match the product photos.", new DateTime(2026, 5, 13, 8, 50, 0, 0, DateTimeKind.Unspecified), 101, 105, 4, new DateTime(2026, 5, 13, 8, 50, 0, 0, DateTimeKind.Unspecified) },
                    { 104, "Good grip for home workouts and easy to carry to class.", new DateTime(2026, 5, 9, 17, 16, 0, 0, DateTimeKind.Unspecified), 103, 108, 4, new DateTime(2026, 5, 9, 17, 16, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.InsertData(
                table: "Orders",
                columns: new[] { "Id", "BillingAddressId", "CustomerId", "OrderDate", "OrderNumber", "OrderStatus", "ShippingAddressId", "ShippingCost", "TotalAmount", "TotalBaseAmount", "TotalDiscountAmount" },
                values: new object[,]
                {
                    { 101, 101, 101, new DateTime(2026, 5, 10, 9, 15, 0, 0, DateTimeKind.Unspecified), "ORD-2026-0001", 4, 102, 5.00m, 684.97m, 749.97m, 70.00m },
                    { 102, 103, 102, new DateTime(2026, 5, 12, 14, 30, 0, 0, DateTimeKind.Unspecified), "ORD-2026-0002", 2, 103, 0.00m, 932.78m, 1089.98m, 157.20m },
                    { 103, 105, 103, new DateTime(2026, 5, 14, 18, 5, 0, 0, DateTimeKind.Unspecified), "ORD-2026-0003", 3, 108, 7.50m, 211.08m, 239.48m, 35.90m },
                    { 104, 106, 104, new DateTime(2026, 5, 15, 10, 45, 0, 0, DateTimeKind.Unspecified), "ORD-2026-0004", 5, 106, 5.00m, 277.98m, 329.98m, 57.00m },
                    { 105, 103, 102, new DateTime(2026, 5, 16, 8, 20, 0, 0, DateTimeKind.Unspecified), "ORD-2026-0005", 2, 104, 0.00m, 248.98m, 279.98m, 31.00m },
                    { 106, 101, 101, new DateTime(2026, 5, 17, 12, 10, 0, 0, DateTimeKind.Unspecified), "ORD-2026-0006", 5, 101, 5.00m, 136.99m, 149.99m, 18.00m },
                    { 107, 107, 105, new DateTime(2026, 5, 18, 20, 40, 0, 0, DateTimeKind.Unspecified), "ORD-2026-0007", 1, 107, 5.00m, 72.98m, 69.98m, 2.00m },
                    { 108, 105, 103, new DateTime(2026, 5, 5, 16, 30, 0, 0, DateTimeKind.Unspecified), "ORD-2026-0008", 4, 105, 5.00m, 71.47m, 69.97m, 3.50m }
                });

            migrationBuilder.InsertData(
                table: "Cancellations",
                columns: new[] { "Id", "CancellationCharges", "OrderAmount", "OrderId", "ProcessedAt", "ProcessedBy", "Reason", "Remarks", "RequestedAt", "Status" },
                values: new object[,]
                {
                    { 101, 0.00m, 277.98m, 104, new DateTime(2026, 5, 15, 11, 30, 0, 0, DateTimeKind.Unspecified), 1, "Customer requested cancellation before shipment because the delivery address changed.", "Approved before fulfillment; full refund issued.", new DateTime(2026, 5, 15, 11, 5, 0, 0, DateTimeKind.Unspecified), 8 },
                    { 102, 0.00m, 248.98m, 105, new DateTime(2026, 5, 16, 9, 25, 0, 0, DateTimeKind.Unspecified), 1, "Customer tried to cancel after warehouse packing had started.", "Rejected because the order had already entered packing.", new DateTime(2026, 5, 16, 9, 0, 0, 0, DateTimeKind.Unspecified), 9 },
                    { 103, 0.00m, 136.99m, 106, new DateTime(2026, 5, 17, 13, 0, 0, 0, DateTimeKind.Unspecified), 1, "The customer selected the wrong headphone color.", "Approved; refund is waiting for gateway confirmation.", new DateTime(2026, 5, 17, 12, 30, 0, 0, DateTimeKind.Unspecified), 8 }
                });

            migrationBuilder.InsertData(
                table: "OrderItems",
                columns: new[] { "Id", "Discount", "OrderId", "ProductId", "Quantity", "TotalPrice", "UnitPrice" },
                values: new object[,]
                {
                    { 101, 70.00m, 101, 1, 1, 629.99m, 699.99m },
                    { 102, 0.00m, 101, 105, 2, 49.98m, 24.99m },
                    { 103, 150.00m, 102, 2, 1, 849.99m, 999.99m },
                    { 104, 7.20m, 102, 102, 1, 82.79m, 89.99m },
                    { 105, 23.90m, 103, 104, 1, 95.60m, 119.50m },
                    { 106, 12.00m, 103, 109, 2, 107.98m, 59.99m },
                    { 107, 45.00m, 104, 112, 1, 204.99m, 249.99m },
                    { 108, 12.00m, 104, 107, 1, 67.99m, 79.99m },
                    { 109, 13.00m, 105, 103, 1, 116.99m, 129.99m },
                    { 110, 18.00m, 105, 101, 1, 131.99m, 149.99m },
                    { 111, 18.00m, 106, 101, 1, 131.99m, 149.99m },
                    { 112, 2.00m, 107, 110, 1, 37.99m, 39.99m },
                    { 113, 0.00m, 107, 111, 1, 29.99m, 29.99m },
                    { 114, 2.00m, 108, 3, 2, 37.98m, 19.99m },
                    { 115, 1.50m, 108, 108, 1, 28.49m, 29.99m }
                });

            migrationBuilder.InsertData(
                table: "Payments",
                columns: new[] { "Id", "Amount", "OrderId", "PaymentDate", "PaymentMethod", "Status", "TransactionId" },
                values: new object[,]
                {
                    { 101, 684.97m, 101, new DateTime(2026, 5, 10, 9, 20, 0, 0, DateTimeKind.Unspecified), "CreditCard", 6, "TXN-CARD-0001" },
                    { 102, 932.78m, 102, new DateTime(2026, 5, 12, 14, 33, 0, 0, DateTimeKind.Unspecified), "VNPAY", 6, "TXN-VNPAY-0002" },
                    { 103, 211.08m, 103, new DateTime(2026, 5, 14, 18, 6, 0, 0, DateTimeKind.Unspecified), "COD", 1, null },
                    { 104, 277.98m, 104, new DateTime(2026, 5, 15, 10, 47, 0, 0, DateTimeKind.Unspecified), "CreditCard", 10, "TXN-CARD-0004" },
                    { 105, 248.98m, 105, new DateTime(2026, 5, 16, 8, 22, 0, 0, DateTimeKind.Unspecified), "PayPal", 6, "TXN-PAYPAL-0005" },
                    { 106, 136.99m, 106, new DateTime(2026, 5, 17, 12, 12, 0, 0, DateTimeKind.Unspecified), "Stripe", 6, "TXN-STRIPE-0006" },
                    { 107, 72.98m, 107, new DateTime(2026, 5, 18, 20, 41, 0, 0, DateTimeKind.Unspecified), "CreditCard", 7, "TXN-FAILED-0007" },
                    { 108, 71.47m, 108, new DateTime(2026, 5, 8, 11, 15, 0, 0, DateTimeKind.Unspecified), "COD", 6, "COD-RECEIPT-0008" }
                });

            migrationBuilder.InsertData(
                table: "Refunds",
                columns: new[] { "Id", "Amount", "CancellationId", "CompletedAt", "InitiatedAt", "PaymentId", "ProcessedBy", "RefundMethod", "RefundReason", "Status", "TransactionId" },
                values: new object[,]
                {
                    { 101, 277.98m, 101, new DateTime(2026, 5, 15, 12, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 5, 15, 11, 35, 0, 0, DateTimeKind.Unspecified), 104, 1, "Original", "Full refund for approved cancellation before shipment.", 6, "RFND-0001" },
                    { 102, 136.99m, 103, null, new DateTime(2026, 5, 17, 13, 5, 0, 0, DateTimeKind.Unspecified), 106, null, "Stripe", "Pending refund for approved item color cancellation.", 1, null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Cancellations",
                keyColumn: "Id",
                keyValue: 102);

            migrationBuilder.DeleteData(
                table: "CartItems",
                keyColumn: "Id",
                keyValue: 101);

            migrationBuilder.DeleteData(
                table: "CartItems",
                keyColumn: "Id",
                keyValue: 102);

            migrationBuilder.DeleteData(
                table: "CartItems",
                keyColumn: "Id",
                keyValue: 103);

            migrationBuilder.DeleteData(
                table: "CartItems",
                keyColumn: "Id",
                keyValue: 104);

            migrationBuilder.DeleteData(
                table: "CartItems",
                keyColumn: "Id",
                keyValue: 105);

            migrationBuilder.DeleteData(
                table: "CartItems",
                keyColumn: "Id",
                keyValue: 106);

            migrationBuilder.DeleteData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: 100);

            migrationBuilder.DeleteData(
                table: "Feedbacks",
                keyColumn: "Id",
                keyValue: 101);

            migrationBuilder.DeleteData(
                table: "Feedbacks",
                keyColumn: "Id",
                keyValue: 102);

            migrationBuilder.DeleteData(
                table: "Feedbacks",
                keyColumn: "Id",
                keyValue: 103);

            migrationBuilder.DeleteData(
                table: "Feedbacks",
                keyColumn: "Id",
                keyValue: 104);

            migrationBuilder.DeleteData(
                table: "OrderItems",
                keyColumn: "Id",
                keyValue: 101);

            migrationBuilder.DeleteData(
                table: "OrderItems",
                keyColumn: "Id",
                keyValue: 102);

            migrationBuilder.DeleteData(
                table: "OrderItems",
                keyColumn: "Id",
                keyValue: 103);

            migrationBuilder.DeleteData(
                table: "OrderItems",
                keyColumn: "Id",
                keyValue: 104);

            migrationBuilder.DeleteData(
                table: "OrderItems",
                keyColumn: "Id",
                keyValue: 105);

            migrationBuilder.DeleteData(
                table: "OrderItems",
                keyColumn: "Id",
                keyValue: 106);

            migrationBuilder.DeleteData(
                table: "OrderItems",
                keyColumn: "Id",
                keyValue: 107);

            migrationBuilder.DeleteData(
                table: "OrderItems",
                keyColumn: "Id",
                keyValue: 108);

            migrationBuilder.DeleteData(
                table: "OrderItems",
                keyColumn: "Id",
                keyValue: 109);

            migrationBuilder.DeleteData(
                table: "OrderItems",
                keyColumn: "Id",
                keyValue: 110);

            migrationBuilder.DeleteData(
                table: "OrderItems",
                keyColumn: "Id",
                keyValue: 111);

            migrationBuilder.DeleteData(
                table: "OrderItems",
                keyColumn: "Id",
                keyValue: 112);

            migrationBuilder.DeleteData(
                table: "OrderItems",
                keyColumn: "Id",
                keyValue: 113);

            migrationBuilder.DeleteData(
                table: "OrderItems",
                keyColumn: "Id",
                keyValue: 114);

            migrationBuilder.DeleteData(
                table: "OrderItems",
                keyColumn: "Id",
                keyValue: 115);

            migrationBuilder.DeleteData(
                table: "Payments",
                keyColumn: "Id",
                keyValue: 101);

            migrationBuilder.DeleteData(
                table: "Payments",
                keyColumn: "Id",
                keyValue: 102);

            migrationBuilder.DeleteData(
                table: "Payments",
                keyColumn: "Id",
                keyValue: 103);

            migrationBuilder.DeleteData(
                table: "Payments",
                keyColumn: "Id",
                keyValue: 105);

            migrationBuilder.DeleteData(
                table: "Payments",
                keyColumn: "Id",
                keyValue: 107);

            migrationBuilder.DeleteData(
                table: "Payments",
                keyColumn: "Id",
                keyValue: 108);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 113);

            migrationBuilder.DeleteData(
                table: "Refunds",
                keyColumn: "Id",
                keyValue: 101);

            migrationBuilder.DeleteData(
                table: "Refunds",
                keyColumn: "Id",
                keyValue: 102);

            migrationBuilder.DeleteData(
                table: "Cancellations",
                keyColumn: "Id",
                keyValue: 101);

            migrationBuilder.DeleteData(
                table: "Cancellations",
                keyColumn: "Id",
                keyValue: 103);

            migrationBuilder.DeleteData(
                table: "Carts",
                keyColumn: "Id",
                keyValue: 101);

            migrationBuilder.DeleteData(
                table: "Carts",
                keyColumn: "Id",
                keyValue: 102);

            migrationBuilder.DeleteData(
                table: "Carts",
                keyColumn: "Id",
                keyValue: 103);

            migrationBuilder.DeleteData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 101);

            migrationBuilder.DeleteData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 102);

            migrationBuilder.DeleteData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 103);

            migrationBuilder.DeleteData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 105);

            migrationBuilder.DeleteData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 107);

            migrationBuilder.DeleteData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 108);

            migrationBuilder.DeleteData(
                table: "Payments",
                keyColumn: "Id",
                keyValue: 104);

            migrationBuilder.DeleteData(
                table: "Payments",
                keyColumn: "Id",
                keyValue: 106);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 101);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 102);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 103);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 104);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 105);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 106);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 107);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 108);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 109);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 110);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 111);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 112);

            migrationBuilder.DeleteData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: 102);

            migrationBuilder.DeleteData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: 103);

            migrationBuilder.DeleteData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: 104);

            migrationBuilder.DeleteData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: 105);

            migrationBuilder.DeleteData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: 107);

            migrationBuilder.DeleteData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: 108);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 101);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 102);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 103);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 104);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 105);

            migrationBuilder.DeleteData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 104);

            migrationBuilder.DeleteData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 106);

            migrationBuilder.DeleteData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: 101);

            migrationBuilder.DeleteData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: 106);

            migrationBuilder.DeleteData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: 102);

            migrationBuilder.DeleteData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: 103);

            migrationBuilder.DeleteData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: 105);

            migrationBuilder.DeleteData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: 101);

            migrationBuilder.DeleteData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: 104);
        }
    }
}
