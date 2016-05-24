using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using ZSProduct;

namespace App2
{
    [Activity(Label = "Dashboard", Theme = "@style/Theme.AppCompat.Light")]
    public class Dashboard : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Dashboard);
            ClearFields();
            var nif = Intent.GetStringExtra("nif");
            var username = Intent.GetStringExtra("username");
            var password = Intent.GetStringExtra("password");

            var txtCodBarras = FindViewById<EditText>(Resource.Id.txtCodigoBarras);
            var txtCodigo = FindViewById<TextView>(Resource.Id.txtCodigo);
            //var txtReferencia = FindViewById<TextView>(Resource.Id.txtRef);
            var txtDescricao = FindViewById<TextView>(Resource.Id.txtDescricao);
            var txtFornecedor = FindViewById<TextView>(Resource.Id.txtFornecedor);
            var txtPvp1 = FindViewById<TextView>(Resource.Id.txtPVP1);
            var txtPvp2 = FindViewById<TextView>(Resource.Id.txtPVP2);

            var zsHandler =  new ZSClient(username, password, 0, nif);
            zsHandler.Login();
            zsHandler.getProducts();

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
        //-----------------------------------------------------------
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            menu.Add(new Java.Lang.String("Settings"));
            menu.Add(new Java.Lang.String("About"));
            return true;
        }

        //-----------------------------------------------------------
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item.TitleFormatted.ToString() == "Settings")
            {
                var settings = new Intent(this, typeof(Settings));
                StartActivity(settings);
            }
            else if (item.TitleFormatted.ToString() == "About")
            {
                var about = new Intent(this, typeof(About));
                StartActivity(about);
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