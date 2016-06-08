using System.Collections.Generic;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;

namespace ZSProduct.Modal
{
    class DialogStock : DialogFragment
    {
        private readonly Dictionary<string, int> _data;
        private List<AddStockStore> _listStocks;

        public DialogStock(Dictionary<string, int> data)
        {
            _data = data;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            _listStocks = new List<AddStockStore>();
            foreach (var item in _data)
            {
                _listStocks.Add(new AddStockStore(item.Value.ToString(), item.Key));
            }
            base.OnCreateView(inflater, container, savedInstanceState);
            var view = inflater.Inflate(Resource.Layout.StockOnStores, container, false);
            view.FindViewById<ListView>(Resource.Id.lstStocks).Adapter = new AdapterListViewStock(Activity, _listStocks);
            return view;
        }

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            Dialog.Window.RequestFeature(WindowFeatures.NoTitle);
            base.OnActivityCreated(savedInstanceState);
        }
    }
}