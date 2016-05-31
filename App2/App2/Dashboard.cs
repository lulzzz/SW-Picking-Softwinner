using System;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Support.V7.App;

namespace App2
{
    [Activity(Label = "Menu Principal", Theme = "@style/Theme.AppCompat.Light", ScreenOrientation = ScreenOrientation.Portrait)]
    public class Dashboard : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Dashboard);

            FindViewById<ImageButton>(Resource.Id.btnConsultarDetalhes).Click += (sender, e) =>
            {
                StartActivity(typeof(ProductSearch));
            };

            FindViewById<ImageButton>(Resource.Id.btnPdt).Click += (sender, e) =>
            {
                try
                {
                    StartActivity(typeof(Pdt));
                }
                catch (Exception _e)
                {
                    Console.WriteLine("ERRO PDT: " + _e.Message + "\n" + _e.ToString());
                }
            };
            FindViewById<ImageButton>(Resource.Id.btnFindStock).Click += (sender, e) =>
            {
                StartActivity(typeof(ProductFinder));
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

