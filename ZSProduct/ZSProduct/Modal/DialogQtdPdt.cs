using System;
using System.Globalization;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using Android.Content;

//-----------------------------------------------------------
namespace ZSProduct.Modal
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
        private readonly double _initialQtd;
        //----------------------------------------------------------- 
        public EventHandler<OnSetQtdEventArgs> OnChangedComplete;

        //-----------------------------------------------------------
        private ImageButton _mImgBtnLess;
        private ImageButton _mImgBtnPlus;
        private Button _mBtnOk;
        private EditText _mtxtQtd;

        public DialogQtdPdt(double qtd)
        {
            _initialQtd = qtd;
        }

        //-----------------------------------------------------------
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);

            var view = inflater.Inflate(Resource.Layout.chooseQtd, container, false);
            _mImgBtnLess = view.FindViewById<ImageButton>(Resource.Id.imgBtnChooseQtdLess);
            _mImgBtnPlus = view.FindViewById<ImageButton>(Resource.Id.imgBtnChooseQtdPlus);
            _mBtnOk = view.FindViewById<Button>(Resource.Id.btnChooseOk);
            _mtxtQtd = view.FindViewById<EditText>(Resource.Id.txtChooseQtd);
            _mtxtQtd.Text = _initialQtd.ToString(CultureInfo.InvariantCulture);
            var qtd = Convert.ToDouble(_mtxtQtd.Text);
            _mImgBtnLess.Click += (sender, args) =>
            {
                if (qtd > 0)
                    _mtxtQtd.Text = (--qtd).ToString(CultureInfo.InvariantCulture);
            };

            _mImgBtnPlus.Click += (sender, args) =>
            {
                _mtxtQtd.Text = (++qtd).ToString(CultureInfo.InvariantCulture);
            };

            _mBtnOk.Click += BtnOk_Click;
            return view;
        }

        //-----------------------------------------------------------
        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            Dialog.Window.RequestFeature(WindowFeatures.NoTitle);
            base.OnActivityCreated(savedInstanceState);
            _mtxtQtd.RequestFocusFromTouch();
            var inm = (InputMethodManager)Activity.GetSystemService(Context.InputMethodService);
            inm.ToggleSoftInput(ShowFlags.Forced, HideSoftInputFlags.None);
        }

        //-----------------------------------------------------------
        private void BtnOk_Click(object sender, EventArgs e)
        {
            Console.WriteLine(_mtxtQtd.Text);
            OnChangedComplete.Invoke(this, new OnSetQtdEventArgs(_mtxtQtd.Text));
            var imm = (InputMethodManager)Activity.GetSystemService(Context.InputMethodService);
            imm.HideSoftInputFromWindow(View.ApplicationWindowToken, 0);
            Dismiss();
        }
    }
}