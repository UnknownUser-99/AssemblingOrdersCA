using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace AssemblingOrdersCA
{
    public class OrderAssembler
    {
        private readonly CADbContext dbContext;

        public OrderAssembler(CADbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public void DisplayShelvesForOrders(params int[] orderNumbers)
        {
            try
            {
                int lineWidth = Console.WindowWidth;
                string line = new string('-', lineWidth);

                Console.WriteLine(line);
                Console.WriteLine($"Сборка заказов {string.Join(", ", orderNumbers)}\n");

                var ordersWithData = dbContext.ProductOrder
                    .Include(po => po.Order)
                    .Include(po => po.Product)
                    .ThenInclude(p => p.ProductShelf)
                    .ThenInclude(ps => ps.Shelf)
                    .Where(po => orderNumbers.Contains(po.OrderID))
                    .ToList();

                var ordersWithoutData = orderNumbers.Except(ordersWithData.Select(po => po.OrderID)).ToList();

                DisplayMainShelvesWithData(ordersWithData, line);
                DisplayOrdersWithoutData(ordersWithoutData, line);

                Console.WriteLine(line);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при формировании сборки заказов: {ex.Message}");
            }
        }

        private void DisplayMainShelvesWithData(List<ProductOrder> ordersWithData, string line)
        {
            var mainShelvesWithProducts = ordersWithData
                .Where(po => po.Product.ProductShelf.Any(ps => ps.IsMainShelf))
                .GroupBy(po => po.Product.ProductShelf.First(ps => ps.IsMainShelf).Shelf.ShelfName)
                .ToList();

            foreach (var shelfGroup in mainShelvesWithProducts)
            {
                Console.WriteLine(line);
                Console.WriteLine($"Стеллаж {shelfGroup.Key}\n");

                foreach (var productOrder in shelfGroup)
                {
                    var product = productOrder.Product;
                    var shelf = product.ProductShelf.First(ps => ps.IsMainShelf).Shelf;

                    Console.WriteLine($"{product.ProductName} (id {product.ProductID}):");
                    Console.WriteLine($"заказ {productOrder.OrderID}, количество {productOrder.ProductOrderQuantity} шт");

                    var additionalShelves = product.ProductShelf
                        .Where(ps => !ps.IsMainShelf)
                        .Select(ps => ps.Shelf.ShelfName)
                        .Distinct()
                        .ToList();

                    if (additionalShelves.Any())
                    {
                        Console.WriteLine($"Дополнительные стеллажи: {string.Join(", ", additionalShelves)}");
                    }

                    Console.WriteLine();
                }
            }
        }

        private void DisplayOrdersWithoutData(List<int> ordersWithoutData, string line)
        {
            foreach (var orderNumber in ordersWithoutData)
            {
                Console.WriteLine(line);
                Console.WriteLine($"Заказ {orderNumber} не найден\n");
            }
        }
    }
}
