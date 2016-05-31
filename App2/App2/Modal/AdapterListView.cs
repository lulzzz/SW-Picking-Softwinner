using System;
using System.Collections.Generic;
using Android.App;
using Android.Views;
using Android.Widget;
using ZSProduct;

namespace App2.Modal
{
    internal class AdapterListView : BaseAdapter<Product>
    {
        private readonly List<AddProducttoListView> _products;
        private readonly Activity _context;

        public AdapterListView(Activity context, List<AddProducttoListView> products)
        {
            _context = context;
            _products = products;
        }

        public override long GetItemId(int position) => position;

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var teste = new Pdt();
            Console.WriteLine("AdapterListView.cs" + teste.qtd);
            var item = _products[position];
            var view = convertView ?? _context.LayoutInflater.Inflate(Resource.Layout.ListViewRow, null);
            //view.FindViewById<EditText>(Resource.Id.txtAdapterQtdProd).Text = teste.qtd.ToString();
            view.FindViewById<EditText>(Resource.Id.txtAdapterQtdProd).Text = item.qtd.ToString();
            view.FindViewById<TextView>(Resource.Id.txtAdapterProdDesc).Text = item.description;
            view.FindViewById<TextView>(Resource.Id.txtAdapterBarCode).Text = item.barCode;
            return view;
        }

        public override int Count => _products.Count;

        public override Product this[int pos] => _products[pos];
    }
}