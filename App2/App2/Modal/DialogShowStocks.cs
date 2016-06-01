using System;
using System.Linq;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using ZSProduct;

//-----------------------------------------------------------
namespace App2.Modal
{
    //-----------------------------------------------------------
    /*public class OnSetQtdEventArgs : EventArgs
    {
        public string QtdSeted { get; set; }

        public OnSetQtdEventArgs(string qtdSeted)
        {
            QtdSeted = qtdSeted;
        }
    }*/

    //-----------------------------------------------------------
    internal class DialogShowStocks : DialogFragment
    {
        //----------------------------------------------------------- 
        //public EventHandler<OnSetQtdEventArgs> OnChangedComplete;

        //-----------------------------------------------------------
 //-----------------------------------------------------------
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);

            var view = inflater.Inflate(Resource.Layout.StockOnStoresDetails, container, false);
            //view.FindViewById<ListView>(Resource.Id.lstStoresStock)

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