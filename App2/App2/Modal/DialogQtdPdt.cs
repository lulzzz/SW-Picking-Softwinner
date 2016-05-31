using System;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;

namespace App2.Modal
{
    public class OnSetQtdEventArgs : EventArgs
    {
        public string QtdSeted { get; set; }

        public OnSetQtdEventArgs(string qtdSeted)
        {
            QtdSeted = qtdSeted;
        }
    }

    class DialogQtdPdt : DialogFragment
    {
        private ImageButton mImgBtnLess;
        private ImageButton mImgBtnPlus;
        private Button mBtnOk;
        private EditText mtxtQtd;

        public EventHandler<OnSetQtdEventArgs> OnChangedComplete;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);

            var view = inflater.Inflate(Resource.Layout.chooseQtd, container, false);
            mImgBtnLess = view.FindViewById<ImageButton>(Resource.Id.imgBtnChooseQtdLess);
            mImgBtnPlus = view.FindViewById<ImageButton>(Resource.Id.imgBtnChooseQtdPlus);
            mBtnOk = view.FindViewById<Button>(Resource.Id.btnChooseOk);
            mtxtQtd = view.FindViewById<EditText>(Resource.Id.txtChooseQtd);

            var qtd = Convert.ToInt32(mtxtQtd.Text);
            mImgBtnLess.Click += (sender, args) =>
            {
                if (qtd > 0)
                    mtxtQtd.Text = qtd--.ToString();
            };

            mImgBtnPlus.Click += (sender, args) =>
            {
                    mtxtQtd.Text = qtd++.ToString();
            };

            mBtnOk.Click += BtnOk_Click;

            return view;
        }

        private void BtnOk_Click(object sender, EventArgs e)
        {
            OnChangedComplete.Invoke(this, new OnSetQtdEventArgs(mtxtQtd.Text));
            Dismiss();
        }

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            Dialog.Window.RequestFeature(WindowFeatures.NoTitle);
            base.OnActivityCreated(savedInstanceState);
        }
    }
}