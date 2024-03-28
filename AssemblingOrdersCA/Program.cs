using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace AssemblingOrdersCA
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                List<int> orderNumbers = Input();

                if (orderNumbers.Count > 0)
                {
                    ProcessOrders(orderNumbers);
                }

                Console.ReadKey();
                Console.Clear();
            }
        }

        static List<int> Input()
        {
            Console.WriteLine("Введите номера заказов:");
            string input = Console.ReadLine();

            string[] orderNumbersStr = input.Split(' ');

            List<int> orderNumbers = new List<int>();

            foreach (var numberStr in orderNumbersStr)
            {
                if (int.TryParse(numberStr, out int orderNumber))
                {
                    orderNumbers.Add(orderNumber);
                }
                else
                {
                    Console.WriteLine($"Неверный формат числа '{numberStr}'.");
                }
            }

            return orderNumbers;
        }

        static void ProcessOrders(List<int> orderNumbers)
        {
            var optionsBuilder = new DbContextOptionsBuilder<CADbContext>();
            optionsBuilder.UseSqlServer(ConfigurationManager.ConnectionStrings["DBShop"].ConnectionString);

            using (var dbContext = new CADbContext(optionsBuilder.Options))
            {
                var orderAssembler = new OrderAssembler(dbContext);

                orderAssembler.DisplayShelvesForOrders(orderNumbers.ToArray());
            }
        }
    }
}
