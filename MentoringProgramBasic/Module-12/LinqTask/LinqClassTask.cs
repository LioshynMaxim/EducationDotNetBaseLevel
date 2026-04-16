using LinqTask.DoNotChange;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LinqTask
{
    public static class LinqClassTask
    {
        public static IEnumerable<Customer> Linq1(IEnumerable<Customer> customers, decimal limit)
        {
            return customers == null
                ? throw new ArgumentNullException()
                : customers.Where(c => c.Orders.Sum(s => s.Total) > limit);
        }

        public static IEnumerable<(Customer customer, IEnumerable<Supplier> suppliers)> Linq2(
            IEnumerable<Customer> customers,
            IEnumerable<Supplier> suppliers
        )
        {
            return customers.Select(
                c => 
                (
                    c,
                    suppliers.Where(s => s.SupplierName == c.CompanyName)
                )
            );
        }

        public static IEnumerable<(Customer customer, IEnumerable<Supplier> suppliers)> Linq2UsingGroup(
            IEnumerable<Customer> customers,
            IEnumerable<Supplier> suppliers
        )
        {
            return customers
                .GroupBy(c => c)
                .Select(c => 
                (
                    c.Key,
                    suppliers.Where(s => s.SupplierName == c.Key.CompanyName)
                ));
        }

        public static IEnumerable<Customer> Linq3(IEnumerable<Customer> customers, decimal limit)
        {
            return customers
                .Where(c => c.Orders.Any(o => o.Total > limit))
                .OrderBy(c => c.Orders.Min(o => o.OrderDate).Year)
                .ThenBy(c => c.Orders.Min(o => o.OrderDate).Month)
                .ThenByDescending(c => c.Orders.Sum(o => o.Total))
                .ThenBy(c => c.CompanyName);
        }

        public static IEnumerable<(Customer customer, DateTime dateOfEntry)> Linq4(
            IEnumerable<Customer> customers
        )
        {
            if (customers == null)
            {
                throw new ArgumentNullException();
            }

            return customers
                .Where(c => c.Orders?.Length > 0)
                .Where(c =>
                    (c.PostalCode != null && !c.PostalCode.All(char.IsDigit)) 
                    || string.IsNullOrWhiteSpace(c.Region) 
                    || (c.Region != null && c.Region.Length > 3)
                )
                .Select(c => (c, c.Orders.Min(o => o.OrderDate)));
        }

        public static IEnumerable<(Customer customer, DateTime dateOfEntry)> Linq5(
            IEnumerable<Customer> customers
        )
        {
            if (customers == null)
            {
                throw new ArgumentNullException();
            }

            return customers
                .Where(c => c.Orders.Any())
                .OrderBy(c => c.Orders.Min(d => d.OrderDate))
                .Select(c => (c, c.Orders.Min(d => d.OrderDate)));
        }

        public static IEnumerable<Customer> Linq6(IEnumerable<Customer> customers)
        {
            if (customers == null)
            {
                throw new ArgumentNullException();
            }

            return customers
                .Where((c, index) => index != 0 && index != 1 && index != 5);
        }

        public static IEnumerable<Linq7CategoryGroup> Linq7(IEnumerable<Product> products)
        {
            if (products == null)
            {
                throw new ArgumentNullException();
            }

            return products
                .GroupBy(p => p.Category)
                .Select(categoryGroup => new Linq7CategoryGroup
                {
                    Category = categoryGroup.Key,
                    UnitsInStockGroup = categoryGroup
                        .GroupBy(p => p.UnitsInStock)
                        .Select(unitsGroup => new Linq7UnitsInStockGroup
                        {
                            UnitsInStock = unitsGroup.Key,
                            Prices = unitsGroup.Select(p => p.UnitPrice)
                        })
                });
        }

        public static IEnumerable<(decimal category, IEnumerable<Product> products)> Linq8(
            IEnumerable<Product> products,
            decimal cheap,
            decimal middle,
            decimal expensive
        )
        {
            if (products == null)
            {
                throw new ArgumentNullException();
            }

            return products.GroupBy(p =>
            {
                if (p.UnitPrice <= cheap)
                {
                    return cheap;
                }
                else if (p.UnitPrice > cheap && p.UnitPrice <= middle)
                {
                    return middle;
                }
                else
                {
                    return expensive;
                }
            })
            .Select(g => (category: g.Key, products: g.AsEnumerable()));
        }

        public static IEnumerable<(string city, int averageIncome, int averageIntensity)> Linq9(
            IEnumerable<Customer> customers
        )
        {
            if (customers == null)
            {
                throw new ArgumentNullException();
            }

            return customers.GroupBy(c => c.City)
                .Select(g => 
                (
                    city: g.Key,
                    averageIncome: (int)Math.Round(g.Average(c => c.Orders.Length == 0 ? 0 : c.Orders.Sum(o => o.Total))),
                    averageIntensity: (int)Math.Round(g.Average(c => c.Orders.Length))
                ));
        }

        public static string Linq10(IEnumerable<Supplier> suppliers)
        {
            if (suppliers == null)
            {
                throw new ArgumentNullException();
            }

            var countries = suppliers
                .Where(s => !string.IsNullOrWhiteSpace(s.Country))
                .Select(s => s.Country)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(c => c.Length)
                .ThenBy(c => c);
            
            return string.Join("", countries);
        }
    }
}