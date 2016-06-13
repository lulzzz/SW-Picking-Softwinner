using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Support.V7.App;
using Android.Widget;
using SupportToolbar = Android.Support.V7.Widget.Toolbar;

namespace ZSProduct
{
    [Activity(Label = "Painel de Controlo", Theme = "@style/Theme.DesignDemo", ScreenOrientation = ScreenOrientation.Portrait)]
    public class Dashboard : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.newDashboard);
            var toolBar = FindViewById<SupportToolbar>(Resource.Id.toolBar);
            SetSupportActionBar(toolBar);
            var ab = SupportActionBar;
            ab.SetHomeAsUpIndicator(Resource.Drawable.ic_arrow_back_white_18dp);
            ab.SetDisplayHomeAsUpEnabled(true);

            FindViewById<LinearLayout>(Resource.Id.lnlaDashboardProdDetails).Click += (sender, args) => { StartActivity(typeof(ProductFinder)); };
            FindViewById<LinearLayout>(Resource.Id.lnlaDashboardPdt).Click += (sender, args) => { StartActivity(typeof(Pdt)); };
            FindViewById<LinearLayout>(Resource.Id.lnlaDashboardSettings).Click += (sender, args) => { StartActivity(typeof(Settings)); };
            FindViewById<LinearLayout>(Resource.Id.lnlaDashboardAbout).Click += (sender, args) => { StartActivity(typeof(About)); };
        }
    }
}