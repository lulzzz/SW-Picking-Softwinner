using System;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Support.V7.App;
using Android.Widget;
using ZSProduct.Modal;
using SupportToolbar = Android.Support.V7.Widget.Toolbar;

namespace ZSProduct
{
    [Activity(Label = "@string/dashboardLabel", Theme = "@style/Theme.DesignDemo", ScreenOrientation = ScreenOrientation.Portrait)]
    public class Dashboard : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Dashboard);
            var toolBar = FindViewById<SupportToolbar>(Resource.Id.toolBar);
            SetSupportActionBar(toolBar);
            FindViewById<LinearLayout>(Resource.Id.lnlaDashboardProdDetails).Click += (sender, args) => { StartActivity(typeof(ProductFinder)); };
            FindViewById<LinearLayout>(Resource.Id.lnlaDashboardPdt).Click += (sender, args) => { StartActivity(typeof(Pdt)); };
            FindViewById<LinearLayout>(Resource.Id.lnlaDashboardSettings).Click += (sender, args) => { StartActivity(typeof(Settings)); };
            FindViewById<LinearLayout>(Resource.Id.lnlaDashboardAbout).Click += (sender, args) => { StartActivity(typeof(About)); };

            if (!new ZsManager(ApplicationContext).HasInternet())
            {
                new Android.Support.V7.App.AlertDialog.Builder(this)
                    .SetTitle("Conex�o")
                    .SetMessage("A conex�o a uma rede � essencial para o correto funcionamento da aplica��o.\nPor favor, verifique a sua conex�o")
                    .SetPositiveButton("OK", (sender, args) => Console.WriteLine("N�o tem internet"))
                    .Show();
            }
        }
    }
}