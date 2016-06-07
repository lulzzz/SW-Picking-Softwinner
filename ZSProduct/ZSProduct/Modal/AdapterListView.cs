using System;
using System.Collections.Generic;
using Android.App;
using Android.Views;
using Android.Widget;

//-----------------------------------------------------------
namespace ZSProduct
{
    //-----------------------------------------------------------
    public class AdapterListView : BaseAdapter<Product>
    {
        //-----------------------------------------------------------
        public class OnDeleteClickEventArgs : EventArgs
        {
            public int Position { get; set; }
            public OnDeleteClickEventArgs(int pos) { Position = pos; }
        }

        //-----------------------------------------------------------
        public class OnEditClickEventArgs : EventArgs
        {
            public int Position { get; set; }
            public OnEditClickEventArgs(int pos) { Position = pos; }
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
        public EventHandler<OnEditClickEventArgs> OnEditClick;

        //-----------------------------------------------------------
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var item = _products[position];
            var view = convertView ?? _context.LayoutInflater.Inflate(Resource.Layout.ListViewRow, null);
            view.FindViewById<TextView>(Resource.Id.txtAdapterQtdProd).Text = item.qtd.ToString();
            view.FindViewById<TextView>(Resource.Id.txtAdapterProdDesc).Text = item.description;
            view.FindViewById<TextView>(Resource.Id.txtAdapterBarCode).Text = item.barCode;

            view.FindViewById<ImageButton>(Resource.Id.imgBtnChooseDelete).Click += (sender, args) => { OnDeleteClick.Invoke(this, new OnDeleteClickEventArgs(position)); };

            view.FindViewById<ImageButton>(Resource.Id.imgBtnChooseEdit).Click += (sender, args) => { OnEditClick.Invoke(this, new OnEditClickEventArgs(position)); };
            return view;
        }

        //-----------------------------------------------------------
        public override int Count => _products.Count;

        //-----------------------------------------------------------
        public override Product this[int pos] => _products[pos];
    }
}