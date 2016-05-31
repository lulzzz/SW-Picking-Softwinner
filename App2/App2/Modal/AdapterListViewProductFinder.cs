using System;
using System.Collections.Generic;
using Android.App;
using Android.Views;
using Android.Widget;
using ZSProduct;


namespace App2.Modal
{
    internal class AdapterListViewProductFinder : BaseAdapter<Product>
    {
        private readonly List<AddProducttoListView> _products;
        private readonly Activity _context;

        public AdapterListViewProductFinder(Activity context, List<AddProducttoListView> products)
        {
            _context = context;
            _products = products;
        }

        public override long GetItemId(int position) => position;

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var teste = new Pdt();
            Console.WriteLine("AdapterListView.cs" + teste.Qtd);
            var item = _products[position];
            var view = convertView ?? _context.LayoutInflater.Inflate(Resource.Layout.ListViewRow_ProductFinder, null);

            //Editar campos a mostrar
            view.FindViewById<TextView>(Resource.Id.txtAdapterStore).Text = item.ToString(); //Nome da Loja
            view.FindViewById<TextView>(Resource.Id.txtAdapterStock).Text = item.ToString(); //Stock
            return view;
        }

        public override int Count => _products.Count;

        public override Product this[int pos] => _products[pos];
    }
}