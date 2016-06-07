using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;
using Android.Widget;

namespace ZSProduct
{
    [Activity(Label = "Sobre", Theme = "@style/Theme.AppCompat.Light")]
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
    }
}