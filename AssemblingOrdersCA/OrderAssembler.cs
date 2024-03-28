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

                var ordersWithData = GetOrdersWithData(orderNumbers);
                var ordersWithoutData = orderNumbers.Except(ordersWithData).ToList();

                DisplayMainShelvesWithData(ordersWithData, line);
                DisplayOrdersWithoutData(ordersWithoutData, line);

                Console.WriteLine(line);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при формировании сборки заказов: {ex.Message}");
            }
        }

        private List<int> GetOrdersWithData(int[] orderNumbers)
        {
            return dbContext.ProductOrder
                .Where(po => orderNumbers.Contains(po.OrderID))
                .Select(po => po.OrderID)
                .Distinct()
                .ToList();
        }

        private void DisplayMainShelvesWithData(List<int> ordersWithData, string line)
        {
            var mainShelvesWithProducts = dbContext.ProductShelf
                .Include(ps => ps.Product)
                .Include(ps => ps.Shelf)
                .Where(ps => ordersWithData.Contains(ps.Product.ProductOrder.FirstOrDefault().OrderID) && ps.IsMainShelf)
                .GroupBy(ps => ps.Shelf.ShelfName)
                .ToList();

            foreach (var shelfGroup in mainShelvesWithProducts)
            {
                Console.WriteLine(line);
                Console.WriteLine($"Стеллаж {shelfGroup.Key}\n");

                foreach (var productInShelf in shelfGroup)
                {
                    Console.WriteLine($"{productInShelf.Product.ProductName} (id {productInShelf.ProductID}):");

                    var ordersForProduct = dbContext.ProductOrder
                        .Where(po => po.ProductID == productInShelf.ProductID && ordersWithData.Contains(po.OrderID))
                        .ToList();

                    foreach (var order in ordersForProduct)
                    {
                        Console.WriteLine($"заказ {order.OrderID}, количество {order.ProductOrderQuantity} шт");
                    }

                    var additionalShelves = dbContext.ProductShelf
                        .Include(ps => ps.Shelf)
                        .Where(ps => ps.ProductID == productInShelf.ProductID && !ps.IsMainShelf && ordersWithData.Contains(ps.Product.ProductOrder.FirstOrDefault().OrderID))
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
