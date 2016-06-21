using System;
using System.Collections.Generic;

namespace ZSProduct.Modal
{
    public class Product
    {
        public string ProductCode;
        public string ProductRef;
        public Dictionary<string, int> Stock;
        public string Supplier;
        public readonly string BarCode;
        public readonly string Description;
        public double Pvp1;
        public double Pvp2;
        public string Reference;
        public string Pcu;
        public string Pcm;

        public Product(string description, string productCode, Dictionary<string, int> stock, string barCode, double price, double pvp2, string reference, string supplier, string pcu, string pcm = "-")
        {
            ProductCode = productCode;
            Description = description;
            Stock = stock;
            BarCode = barCode;
            Pvp1 = price;
            Pvp2 = pvp2;
            Reference = reference;
            Pcu = pcu;
            Pcm = pcm;
            Supplier = supplier;
        }

        public void PrintProductDetails()
        {
            Console.WriteLine("ProductName:" + Description + "\n" +
                                "Stock:" + Stock + "\n" +
                                "BarCode:" + BarCode + "\n" +
                                "PriceOfSale:" + Pvp1 + "\n" +
                                "pvp2:" + Pvp2 + "\n" +
                                "Reference:" + Reference + "\n" +
                                "UnitPrice:" + Pcu + "\n"
            );
        }

        public static void PrintProductDetails(Product singleProduct)
        {
            Console.WriteLine("ProductName:" +
            singleProduct.Description + "\n" +
            "Stock:" + singleProduct.Stock + "\n" +
            "BarCode:" + singleProduct.BarCode + "\n" +
            "PriceOfSale:" + singleProduct.Pvp1 + "\n" +
            "pvp2:" + singleProduct.Pvp2 + "\n" +
            "Reference:" + singleProduct.Reference + "\n" +
            "UnitPrice:" + singleProduct.Pcu + "\n"
            );
        }
    }
}