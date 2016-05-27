using System;
using Android.Hardware.Usb;

namespace ZSProduct
{
	public class Product
	{
		public uint productCode;
		public uint productRef;
		public uint store;
		public uint stock;
		public uint supplierId;
		public readonly string barCode;
		public readonly string description;
		public readonly double priceOfSale;
		public double pvp2;
		public string reference;
		public string pcu;

		public Product (string description, uint productCode, uint store, uint stock, string barCode, double price, double pvp2, string reference, string pcu)
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

		public void printProductDetails ()
		{
			Console.WriteLine ("ProductName:" +
			this.description + "\n" +
			"Store:" + this.store + "\n" +
			"Stock:" + this.stock + "\n" +
			"BarCode:" + this.barCode + "\n" +
			"PriceOfSale:" + this.priceOfSale + "\n" +
			"pvp2:" + this.pvp2 + "\n" +
			"Reference:" + this.reference + "\n" +
			"UnitPrice:" + this.pcu + "\n"
			);
		}

		public static void printProductDetails (Product singleProduct)
		{
			Console.WriteLine ("ProductName:" +
			singleProduct.description + "\n" +
			"Store:" + singleProduct.store + "\n" +
			"Stock:" + singleProduct.stock + "\n" +
			"BarCode:" + singleProduct.barCode + "\n" +
			"PriceOfSale:" + singleProduct.priceOfSale + "\n" +
			"pvp2:" + singleProduct.pvp2 + "\n" +
			"Reference:" + singleProduct.reference + "\n" +
			"UnitPrice:" + singleProduct.pcu + "\n"
			);
		}
		//        public string GetBarCode() => barCode;
		//
		//        public uint GetProductCode() => productCode;
		//
		//        public string GetDescription() => description;
		//
		//        public double GetPriceOfSale() => priceOfSale;
		//
		//        public uint GetSupplier() => supplierId;
		//
		//        public double GetPvp2() => pvp2;
		//
		//        public uint GetStock() => stock;
		//
		//        public string GetReference() => reference;
		//
		//        public string GetPcu() => pcu;
	}
}