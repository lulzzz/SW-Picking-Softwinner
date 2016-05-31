using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Support.V7.App;

namespace App2
{
    [Activity(Label = "About", Theme = "@style/Theme.AppCompat.Light", ScreenOrientation = ScreenOrientation.Portrait)]
    public class About : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.About);
        }
    }
}