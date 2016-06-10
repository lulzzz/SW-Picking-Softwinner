using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace ZSProduct
{
    [Activity(Label = "Sobre", Theme = "@style/Theme.DesignDemo", ScreenOrientation = ScreenOrientation.Portrait)]
    public class About : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.About);

            FindViewById<TextView>(Resource.Id.txtAboutSWSite).Click += delegate {
                var uri = Android.Net.Uri.Parse("http://www.softwinner.pt");
                var intent = new Intent(Intent.ActionView, uri);
                StartActivity(intent);
            };
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            menu.Add(new Java.Lang.String("Configurações"));
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.TitleFormatted.ToString())
            {
                case "Configurações":
                    StartActivity(typeof(Settings));
                    break;
            }
            return true;
        }
    }
}