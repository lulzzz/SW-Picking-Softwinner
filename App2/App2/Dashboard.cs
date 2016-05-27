using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Support.V7.App;

namespace App2
{
    [Activity(Label = "Menu Principal", Theme = "@style/Theme.AppCompat.Light")]
    public class Dashboard : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Dashboard);

            FindViewById<Button>(Resource.Id.btnConsultarDetalhes).Click += (sender, e) =>
            {
                StartActivity(typeof(ProductSearch));
            };

            FindViewById<Button>(Resource.Id.btnPdt).Click += (sender, e) =>
            {
                StartActivity(typeof(Pdt));
            };
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

