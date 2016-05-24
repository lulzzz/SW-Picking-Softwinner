using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using ZSProduct;

namespace App2
{
    [Activity(Label = "Dashboard", Icon = "@drawable/icon", MainLauncher = true,Theme = "@style/Theme.AppCompat.Light")]
    public class Dashboard : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Dashboard);
            ClearFields();

            var pref = Application.Context.GetSharedPreferences("UserInfo", FileCreationMode.Private);
            var nif = pref.GetString("nif", string.Empty);
            var username = pref.GetString("username", string.Empty);
            var password = pref.GetString("password", string.Empty);

            if (username == string.Empty || password == string.Empty)
                StartActivity(new Intent(this, typeof(MainActivity)));
            else
            {
                var txtCodBarras = FindViewById<EditText>(Resource.Id.txtCodigoBarras);
                var txtCodigo = FindViewById<TextView>(Resource.Id.txtCodigo);
                //var txtReferencia = FindViewById<TextView>(Resource.Id.txtRef);
                var txtDescricao = FindViewById<TextView>(Resource.Id.txtDescricao);
                var txtFornecedor = FindViewById<TextView>(Resource.Id.txtFornecedor);
                var txtPvp1 = FindViewById<TextView>(Resource.Id.txtPVP1);
                var txtPvp2 = FindViewById<TextView>(Resource.Id.txtPVP2);

                var zsHandler = new ZSClient(username, password, 0, nif);
                zsHandler.Login();
                zsHandler.GetProducts();
                Console.WriteLine("\n\n\n\n CONCLUIDO \n\n\n\n");
                zsHandler.TotalStores();
                Toast.MakeText(this, "Total Lojas: " + zsHandler.TotalStores(), ToastLength.Short).Show();

                txtCodBarras.TextChanged += (sender, e) =>
                {
                    if (zsHandler.GetProductWithBarCode(txtCodBarras.Text) != null)
                    {
                        var product = zsHandler.GetProductWithBarCode(txtCodBarras.Text);
                        //txtCodRef.Text = product.GetProductCode().ToString();
                        txtCodigo.Text = product.GetProductCode().ToString();
                        txtDescricao.Text = product.GetDescription();
                        txtPvp1.Text = $"€{product.GetPriceOfSale():f2}";
                        txtPvp2.Text = $"€{product.GetPvp2():f2}";
                        //txtFornecedor.Text = zsHandler.getSupplierNameWithId((int)product.GetSupplier());
                        txtFornecedor.Text = product.GetSupplier().ToString();
                        //txtStock.Text = product.GetStock().ToString();
                    }
                    else
                        ClearFields();
                };
            }

        }
        //-----------------------------------------------------------
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            menu.Add(new Java.Lang.String("Settings"));
            menu.Add(new Java.Lang.String("Test"));
            menu.Add(new Java.Lang.String("About"));
            return true;
        }

        //-----------------------------------------------------------
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.TitleFormatted.ToString())
            {
                case "Settings":
                    var settings = new Intent(this, typeof(Settings));
                    StartActivity(settings);
                    break;
                case "Test":
                    var test = new Intent(this, typeof(test1));
                    StartActivity(test);
                    break;
                case "About":
                    var about = new Intent(this, typeof(About));
                    StartActivity(about);
                    break;
            }
            return true;
        }

        //-----------------------------------------------------------
        private void ClearFields()
        {
            FindViewById<EditText>(Resource.Id.txtCodigoBarras);
            FindViewById<TextView>(Resource.Id.txtCodigo).Text = "";
            FindViewById<TextView>(Resource.Id.txtRef).Text = "";
            FindViewById<TextView>(Resource.Id.txtDescricao).Text = "";
            FindViewById<TextView>(Resource.Id.txtFornecedor).Text = "";
            FindViewById<TextView>(Resource.Id.txtPVP1).Text = "";
            FindViewById<TextView>(Resource.Id.txtPVP2).Text = "";
        }
    }
}