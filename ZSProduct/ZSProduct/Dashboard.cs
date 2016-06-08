using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;

namespace ZSProduct
{
    [Activity(Label = "Dashboard", Theme = "@style/Theme.AppCompat.Light", ScreenOrientation = ScreenOrientation.Portrait)]
    public class Dashboard : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Dashboard);

            //Dialog d = new Dialog(this);
            //d.SetContentView(Resource.Layout.Settings);
            //d.Show();
            FindViewById<LinearLayout>(Resource.Id.lnlaDashboardProdDetails).Click += (sender, args) => { StartActivity(typeof(ProductFinder)); };
            FindViewById<LinearLayout>(Resource.Id.lnlaDashboardPdt).Click += (sender, args) => { StartActivity(typeof(Pdt)); };
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            menu.Add(new Java.Lang.String("Configurações"));
            menu.Add(new Java.Lang.String("Sobre"));
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.TitleFormatted.ToString())
            {
                case "Configurações":
                    StartActivity(typeof(Settings));
                    break;
                case "Sobre":
                    StartActivity(typeof(About));
                    break;
            }
            return true;
        }
    }
}