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
        private readonly List<Product> _productsList = new List<Product>();
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Pdt);

            FindViewById<EditText>(Resource.Id.txtPdtCodBarras).KeyPress += (sender, e) =>
            {
                if (e.KeyCode == Android.Views.Keycode.Enter)
                    AddItem();
                else
                    e.Handled = false;
            };
        }

        private void AddItem()
        {
            var manager = new ZsManager();
            var getNif = manager.GetItem("nif");
            var getUsername = manager.GetItem("username");
            var getPassword = manager.GetItem("password");
            var txtPdtCodBarras = FindViewById<EditText>(Resource.Id.txtPdtCodBarras);
            var zsHandler = new ZSClient(getUsername, getPassword, 0, getNif);
            zsHandler.Login();
            var existe = false;
            foreach (var item in _productsList)
                if (item.barCode == txtPdtCodBarras.Text)
                    existe = true;

            if (!existe)
            {
                var product = zsHandler.GetProductWithBarCode(txtPdtCodBarras.Text);
                if (product != null)
                {
                    _productsList.Add(product);
                    FindViewById<ListView>(Resource.Id.listView1).Adapter = new AdapterListView(this, _productsList);
                }
                else
                {
                    Toast.MakeText(this, "Produto inexistente", ToastLength.Short).Show();
                    txtPdtCodBarras.Text = "iuug";
                }
            }
            else
                Toast.MakeText(this, "Produto já adicionado", ToastLength.Short).Show();

        }

    }
}