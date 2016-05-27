using System.Collections.Generic;
using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Widget;
using App2.Modal;
using ZSProduct;

namespace App2
{
    [Activity(Label = "Pdt", Theme = "@style/Theme.AppCompat.Light")]
    public class Pdt : AppCompatActivity
    {
        //List<Product> products = new List<Product>();
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Pdt);

            // Create your application here

            var btnPdtAdd = FindViewById<Button>(Resource.Id.btnPdtAdd);
            btnPdtAdd.Click += (sender, e) =>
            {
                AddItem();
            };
        }

        private void AddItem()
        {
            var manager = new ZsManager();
            var getNif = manager.GetItem("nif");
            var getUsername = manager.GetItem("username");
            var getPassword = manager.GetItem("password");
            var txtPdtCodBarras = FindViewById<Button>(Resource.Id.txtPdtCodBarras);
            var zsHandler = new ZSClient(getUsername, getPassword, 0, getNif);
            zsHandler.Login();
            var product = zsHandler.GetProductWithBarCode(txtPdtCodBarras.Text);
            //Toast.MakeText(this, product.description, ToastLength.Short).Show();
            //products.Add(product);
            var cenas = new List<string> {"[" + product.productCode + "] " + product.description};
            var adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, cenas);
            FindViewById<ListView>(Resource.Id.listView1).Adapter = adapter;
        }
    }
}