using System.Collections.Generic;
using Android.App;
using Android.Views;
using Android.Widget;
using ZSProduct.Modal;

//-----------------------------------------------------------
namespace ZSProduct
{
    //-----------------------------------------------------------
    public class AdapterListViewStock : BaseAdapter<AddStockStore>
    {
        //-----------------------------------------------------------
        public AdapterListViewStock(Activity context, List<AddStockStore> data)
        {
            _context = context;
            _data = data;
        }

        //-----------------------------------------------------------
        private readonly Activity _context;
        public override long GetItemId(int position) => position;
        public List<AddStockStore> _data;
        //-----------------------------------------------------------
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var item = _data[position];
            var view = convertView ?? _context.LayoutInflater.Inflate(Resource.Layout.ListViewRowStock, null);
            view.FindViewById<TextView>(Resource.Id.txtAdapterStockStore).Text = item.Stock;
            view.FindViewById<TextView>(Resource.Id.txtAdapterStockStockProd).Text = item.Loja;
            return view;
        }

        //-----------------------------------------------------------
        public override int Count => _data.Count;

        //-----------------------------------------------------------
        public override AddStockStore this[int pos] => _data[pos];
    }
}