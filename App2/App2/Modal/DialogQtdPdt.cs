using System;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;

//-----------------------------------------------------------
namespace App2.Modal
{
    //-----------------------------------------------------------
    public class OnSetQtdEventArgs : EventArgs
    {
        public string QtdSeted { get; set; }

        public OnSetQtdEventArgs(string qtdSeted)
        {
            QtdSeted = qtdSeted;
        }
    }

    //-----------------------------------------------------------
    internal class DialogQtdPdt : DialogFragment
    {
        //----------------------------------------------------------- 
        public EventHandler<OnSetQtdEventArgs> OnChangedComplete;

        //-----------------------------------------------------------
        private ImageButton _mImgBtnLess;
        private ImageButton _mImgBtnPlus;
        private Button _mBtnOk;
        private EditText _mtxtQtd;

        //-----------------------------------------------------------
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);

            var view = inflater.Inflate(Resource.Layout.chooseQtd, container, false);
            _mImgBtnLess = view.FindViewById<ImageButton>(Resource.Id.imgBtnChooseQtdLess);
            _mImgBtnPlus = view.FindViewById<ImageButton>(Resource.Id.imgBtnChooseQtdPlus);
            _mBtnOk = view.FindViewById<Button>(Resource.Id.btnChooseOk);
            _mtxtQtd = view.FindViewById<EditText>(Resource.Id.txtChooseQtd);

            var qtd = Convert.ToInt32(_mtxtQtd.Text);
            _mImgBtnLess.Click += (sender, args) =>
            {
                if (qtd > 0)
                    _mtxtQtd.Text = (--qtd).ToString();
            };

            _mImgBtnPlus.Click += (sender, args) =>
            {
                _mtxtQtd.Text = (++qtd).ToString();
            };

            _mBtnOk.Click += BtnOk_Click;

            return view;
        }

        //-----------------------------------------------------------
        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            Dialog.Window.RequestFeature(WindowFeatures.NoTitle);
            base.OnActivityCreated(savedInstanceState);
        }

        //-----------------------------------------------------------
        private void BtnOk_Click(object sender, EventArgs e)
        {
            OnChangedComplete.Invoke(this, new OnSetQtdEventArgs(_mtxtQtd.Text));
            Dismiss();
        }
    }
}