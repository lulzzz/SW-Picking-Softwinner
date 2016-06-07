using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using ZSProduct.Modal;

namespace ZSProduct
{
    [Activity(Label = "StockOnStores")]
    public class StockOnStores : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            Context _context = Application.Context;
            List<AddStockStore> _stocks = new List<AddStockStore>();
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.StockOnStores);
            /*
             _productList.Add(new AddProducttoListView(product.Description, product.ProductCode,
                            product.Store, product.Stock, product.BarCode, product.Pvp1, product.Pvp2,
                            product.Reference, product.Pcu, Qtd));
                        FindViewById<ListView>(Resource.Id.lstPdtProducts).Adapter = _view;
                        txtPdtCodBarras.Text = "";
             */
            _stocks.Add(new AddStockStore("0", "0"));
            _stocks.Add(new AddStockStore("5", "2"));
            _stocks.Add(new AddStockStore("0", "0"));
            _stocks.Add(new AddStockStore("545", "3"));
            FindViewById<ListView>(Resource.Id.lstStocks).Adapter = new AdapterListViewStock(this, _stocks);
            // Create your application here
        }
    }
}