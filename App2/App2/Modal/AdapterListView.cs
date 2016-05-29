using System.Collections.Generic;
using Android.App;
using Android.Views;
using Android.Widget;
using ZSProduct;

namespace App2.Modal
{
    internal class AdapterListView : BaseAdapter<Product>
    {
        private readonly List<Product> _products;
        private readonly Activity _context;

        public AdapterListView(Activity context, List<Product> products)
        {
            _context = context;
            _products = products;
        }

        public override long GetItemId(int position) => position;

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var item = _products[position];
            var view = convertView ?? _context.LayoutInflater.Inflate(Resource.Layout.ListViewRow, null);
            view.FindViewById<EditText>(Resource.Id.txtAdapterQtdProd).Text = "1";
            view.FindViewById<TextView>(Resource.Id.txtAdapterProdDesc).Text = item.Description;
            view.FindViewById<TextView>(Resource.Id.txtAdapterBarCode).Text = item.BarCode;
            view.FindViewById<ImageButton>(Resource.Id.imageButton1).Id = position;
            return view;
        }

        public override int Count => _products.Count;

        public override Product this[int pos] => _products[pos];
    }
}