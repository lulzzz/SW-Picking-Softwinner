using System;
using System.Collections.Generic;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;

//-----------------------------------------------------------
namespace App2.Modal
{
    //-----------------------------------------------------------
    internal class DialogShowStocks : DialogFragment
    {
        private Dictionary<string, int> _stocks;
        public DialogShowStocks(Dictionary<string, int> stocks)
        {
            _stocks = stocks;
        }
        //-----------------------------------------------------------
        private AdapterListViewProductFinder _view;

        //-----------------------------------------------------------
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            _view = new AdapterListViewProductFinder(Activity, _stocks);
            base.OnCreateView(inflater, container, savedInstanceState);
           
            var view = inflater.Inflate(Resource.Layout.StockOnStoresDetails, container, false);
            return view;
        }

        //-----------------------------------------------------------
        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            Dialog.Window.RequestFeature(WindowFeatures.NoTitle);
            base.OnActivityCreated(savedInstanceState);
        }
    }
}