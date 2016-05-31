using System;
using System.Collections.Generic;
using System.Threading;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Support.V7.App;
using Android.Widget;
using App2.Modal;
using ZSProduct;

namespace App2
{
    [Activity(Label = "Contagem de Stock ", Theme = "@style/Theme.AppCompat.Light.NoActionBar", ScreenOrientation = ScreenOrientation.Portrait)]
    public class Pdt : AppCompatActivity
    {
        public int qtd { get; set; }
        private readonly List<AddProducttoListView> _cenas = new List<AddProducttoListView>();
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Pdt);
            FindViewById<Button>(Resource.Id.button1).Click += (sender, args) =>
            {
                AddItemtoListView();
            };
        }

        private void AddItemtoListView()
        {
            var transaction = FragmentManager.BeginTransaction();
            var dialogFragment = new DialogQtdPdt();
            dialogFragment.Show(transaction, "dialog_fragment");
            dialogFragment.OnChangedComplete += OnChangedCompleted;
        }

        void OnChangedCompleted(object sender, OnSetQtdEventArgs e)
        {
            qtd = Convert.ToInt32(e.QtdSeted);
            Console.WriteLine("\n\n\n\n\n\n\n\n OK PRESSED \n QTD:" + qtd + " \n\n\n\n\n\n\n\n\n\n\n\n");
            var t = new Thread(AddItem);
            t.Start();
        }

        private void AddItem()
        {
            RunOnUiThread(() =>
            {
                Console.WriteLine("AddItem started");
                var manager = new ZsManager();
                var getNif = manager.GetItem("nif");
                var getUsername = manager.GetItem("username");
                var getPassword = manager.GetItem("password");
                var txtPdtCodBarras = FindViewById<EditText>(Resource.Id.txtPdtCodBarras);
                var zsHandler = new ZSClient(getUsername, getPassword, 0, getNif);
                zsHandler.Login();
                var existe = false;
                foreach (var item in _cenas)
                    if (item.barCode == txtPdtCodBarras.Text)
                        existe = true;

                if (!existe)
                {
                    var product = zsHandler.GetProductWithBarCode(txtPdtCodBarras.Text);
                    if (product != null)
                    {
                        Console.WriteLine("oaef: " + qtd);
                        //description, productCode, store, stock, barCode, price, pvp2, reference, pcu, int Qtd
                        _cenas.Add(new AddProducttoListView(product.description, product.productCode, product.store, product.stock, product.barCode, product.pvp1, product.pvp2, product.reference, product.pcu, qtd));
                        FindViewById<ListView>(Resource.Id.listView1).Adapter = new AdapterListView(this, _cenas);
                    }
                    else
                    {
                        Toast.MakeText(this, "Produto inexistente", ToastLength.Long).Show();
                        Console.WriteLine("ProdutoInexistente");
                        txtPdtCodBarras.Text = "";
                    }
                }
                else
                {
                    Toast.MakeText(this, "Produto inexistente", ToastLength.Long).Show();
                    Console.WriteLine("Produto já adicionado");
                }
            });
        }
    }
}