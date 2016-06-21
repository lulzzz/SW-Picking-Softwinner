using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;

namespace ZSProduct.Modal
{
    public class DialogLoading : DialogFragment
    {
        //-----------------------------------------------------------
        private ProgressBar _prgDialogLoading;

        //-----------------------------------------------------------
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);

            var view = inflater.Inflate(Resource.Layout.DialogLoading, container, false);
            _prgDialogLoading = view.FindViewById<ProgressBar>(Resource.Id.prgDialogLoading);
            _prgDialogLoading.Animate();
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