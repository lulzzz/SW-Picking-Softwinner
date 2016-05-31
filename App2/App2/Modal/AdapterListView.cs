using System;
using System.Collections.Generic;
using Android.App;
using Android.Views;
using Android.Widget;
using ZSProduct;

//-----------------------------------------------------------
namespace App2.Modal
{
    //-----------------------------------------------------------
    public class AdapterListView : BaseAdapter<Product>
    {
        //-----------------------------------------------------------
        public class OnDeleteClickEventArgs : EventArgs
        {
            public int Position { get; set; }

            public OnDeleteClickEventArgs(int pos)
            {
                Position = pos;
            }
        }

        //-----------------------------------------------------------
        public AdapterListView(Activity context, List<AddProducttoListView> products)
        {
            _context = context;
            _products = products;
        }

        //-----------------------------------------------------------
        private readonly List<AddProducttoListView> _products;
        private readonly Activity _context;
        public override long GetItemId(int position) => position;
        public EventHandler<OnDeleteClickEventArgs> OnDeleteClick;

        //-----------------------------------------------------------
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var teste = new Pdt();
            Console.WriteLine("AdapterListView.cs" + teste.Qtd);
            var item = _products[position];
            var view = convertView ?? _context.LayoutInflater.Inflate(Resource.Layout.ListViewRow, null);
            view.FindViewById<TextView>(Resource.Id.txtAdapterQtdProd).Text = item.qtd.ToString();
            view.FindViewById<TextView>(Resource.Id.txtAdapterProdDesc).Text = item.description;
            view.FindViewById<TextView>(Resource.Id.txtAdapterBarCode).Text = item.barCode;

            view.FindViewById<ImageButton>(Resource.Id.imgBtnChooseDelete).Click += (sender, args) =>
            {
                //   _products.RemoveAt(position);
                OnDeleteClick.Invoke(this, new OnDeleteClickEventArgs(position));
            };
            return view;
        }

        //-----------------------------------------------------------
        public override int Count => _products.Count;

        //-----------------------------------------------------------
        public override Product this[int pos] => _products[pos];
    }
}