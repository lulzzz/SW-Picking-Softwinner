using System;
using Android.Hardware.Usb;

namespace ZSProduct
{
    public class Product
    {
        private uint productCode;
        private uint productRef;
        private uint store;
        private uint stock;
        private uint supplierId;
        private readonly string barCode;
        private readonly string description;
        private readonly double priceOfSale;
        private double pvp2;
        private string reference;
        private string pcu;

        public Product(string description, uint productCode, uint store, uint stock, string barCode, double price, double pvp2, string reference, string pcu)
        {
            this.productCode = productCode;
            this.description = description;
            this.store = store;
            this.stock = stock;
            this.barCode = barCode;
            this.priceOfSale = price;
            this.pvp2 = pvp2;
            this.reference = reference;
            this.pcu = pcu;

        }

        public string GetBarCode() => barCode;

        public uint GetProductCode() => productCode;

        public string GetDescription() => description;

        public double GetPriceOfSale() => priceOfSale;

        public uint GetSupplier() => supplierId;

        public double GetPvp2() => pvp2;

        public uint GetStock() => stock;

        public string GetReference() => reference;

        public string GetPcu() => pcu;
    }
}