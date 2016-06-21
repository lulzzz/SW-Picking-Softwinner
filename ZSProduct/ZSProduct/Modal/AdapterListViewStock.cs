using System;
using System.Collections.Generic;
using Android.App;
using Android.Renderscripts;
using Android.Views;
using Android.Widget;

//-----------------------------------------------------------
namespace ZSProduct.Modal
{
    //-----------------------------------------------------------
    public class AdapterListViewStock : BaseAdapter<AddStockStore>
    {
        //-----------------------------------------------------------
        public AdapterListViewStock(Activity context, List<AddStockStore> data)
        {
            _context = context;
            Data = data;
        }

        //-----------------------------------------------------------
        private readonly Activity _context;
        public override long GetItemId(int position) => position;
        public List<AddStockStore> Data;
        private readonly ZsManager _manager = new ZsManager();
        private ZsClient _zsClient;
        private string _nomeLoja;

        //-----------------------------------------------------------
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            _zsClient = new ZsClient(_manager.GetItem("username"), _manager.GetItem("password"), 0, _manager.GetItem("nif"));
            _zsClient.Login();
            var item = Data[position];
            var view = convertView ?? _context.LayoutInflater.Inflate(Resource.Layout.ListViewRowStock, null);
            if (_manager.GetItem("loginType") == "zonesoft")
                _nomeLoja = _zsClient.GetStoreDescription(Convert.ToInt32(item.Loja)).ToUpper();
            else
                _nomeLoja = item.Loja.ToUpper();
            //view.FindViewById<TextView>(Resource.Id.txtAdapterStockStore).Text = _nomeLoja.Length > 11 ? _nomeLoja.Substring(0, 13) : _nomeLoja;
            view.FindViewById<TextView>(Resource.Id.txtAdapterStockStore).Text = _nomeLoja;
            view.FindViewById<TextView>(Resource.Id.txtAdapterStockStockProd).Text = item.Stock + " un.";
            return view;
        }

        //-----------------------------------------------------------
        public override int Count => Data.Count;

        //-----------------------------------------------------------
        public override AddStockStore this[int pos] => Data[pos];
    }
}