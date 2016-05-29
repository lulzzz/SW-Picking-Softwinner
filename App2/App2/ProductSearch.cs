using System;
using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using App2.Modal;
using ZSProduct;

namespace App2
{
    [Activity(Label = "Detalhes de Produto", Icon = "@drawable/icon", Theme = "@style/Theme.AppCompat.Light")]
    public class ProductSearch : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            /*
             * MODES:
             *      0 - OFFLINE
             *      1 - ONLINE
             */

            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.ProductSearch);

            var manager = new ZsManager();
            var nif = manager.GetItem("nif");
            var username = manager.GetItem("username");
            var password = manager.GetItem("password");
            var mode = manager.GetItem("mode");
            var searchingByBarCode = false;
            var searchingByProductCode = false;
            var searchingByReference = false;
            ClearFields("all");
            //Testing ...
            //manager.DownloadStoreAsync ();
            //manager.zsClient.Login ();
            //
            //var simpleProduct = manager.zsClient.GetProductWithBarCode ("1000000700497");
            //simpleProduct.printProductDetails ();
            //Product.printProductDetails (simpleProduct);

            var txtCodBarras = FindViewById<EditText>(Resource.Id.txtCodBarras);
            var txtCodigo = FindViewById<TextView>(Resource.Id.txtCodigo);
            var txtReferencia = FindViewById<TextView>(Resource.Id.txtRef);
            var txtDescricao = FindViewById<TextView>(Resource.Id.txtDescricao);
            var txtFornecedor = FindViewById<TextView>(Resource.Id.txtFornecedor);
            var txtPvp1 = FindViewById<TextView>(Resource.Id.txtPvp1);
            var txtPvp2 = FindViewById<TextView>(Resource.Id.txtPvp2);
            var txtPcu = FindViewById<TextView>(Resource.Id.txtPcu);
            var zsHandler = new ZSClient(username, password, 0, nif);
            zsHandler.Login();

            //-----------------------------------------------------------
            txtCodBarras.TextChanged += (sender, e) =>
            {
                if (!searchingByProductCode && !searchingByProductCode)
                {
                    Console.WriteLine("\n\n\n\n\n AQUI:: txtCodBarras.TextChanged \n\n\n\n\n");
                    searchingByBarCode = true;
                    if (mode == "0")
                    {

                    }
                    else if (mode == "1")
                    {
                        if (txtCodBarras.Text != "" && zsHandler.GetProductWithBarCode(txtCodBarras.Text) != null)
                        {
                            var product = zsHandler.GetProductWithBarCode(txtCodBarras.Text);
                            txtCodigo.Text = product.ProductCode.ToString();
                            txtReferencia.Text = product.Reference.ToString();
                            txtDescricao.Text = product.Description;
                            txtFornecedor.Text = product.Supplier; //TO SUPPLIER NAME
                            txtPvp1.Text = $"€{product.PriceOfSale:f2}";
                            txtPvp2.Text = $"€{product.Pvp2:f2}";
                            txtPcu.Text = $"€{product.Pcu:f2}";
                            searchingByBarCode = false;
                        }
                        else
                            ClearFields("codBarras");
                    }
                }
            };

            txtCodigo.TextChanged += (sender, e) =>
            {
                if (!searchingByBarCode && !searchingByReference)
                {
                    Console.WriteLine("\n\n\n\n\n AQUI:: txtCodigo.TextChanged \n\n\n\n\n");
                    searchingByProductCode = true;
                    if (mode == "0")
                    {

                    }
                    else if (mode == "1")
                    {
                        if (txtCodigo.Text != "" && zsHandler.GetProductWithCode(txtCodigo.Text) != null)
                        {
                            var product = zsHandler.GetProductWithCode(txtCodigo.Text);
                            txtCodBarras.Text = product.BarCode;
                            txtReferencia.Text = product.Reference.ToString();
                            txtDescricao.Text = product.Description;
                            txtFornecedor.Text = product.Supplier.ToString(); //TO SUPPLIER NAME
                            txtPvp1.Text = $"€{product.PriceOfSale:f2}";
                            txtPvp2.Text = $"€{product.Pvp2:f2}";
                            txtPcu.Text = $"€{product.Pcu:f2}";
                            searchingByProductCode = false;
                        }
                        else
                            ClearFields("codigo");
                    }
                }
            };

            txtReferencia.TextChanged += (sender, e) =>
            {
                if (!searchingByBarCode && !searchingByBarCode)
                {
                    Console.WriteLine("\n\n\n\n\n AQUI:: txtReferencia.TextChanged \n\n\n\n\n");
                    searchingByReference = true;
                    if (mode == "0")
                    {

                    }
                    else if (mode == "1")
                    {
                        if (txtReferencia.Text != "" && zsHandler.GetProductWithRef(txtReferencia.Text) != null)
                        {
                            var product = zsHandler.GetProductWithRef(txtReferencia.Text);
                            txtCodBarras.Text = product.BarCode;
                            txtCodigo.Text = product.ProductCode.ToString();
                            txtDescricao.Text = product.Description;
                            txtFornecedor.Text = product.Supplier.ToString(); //TO SUPPLIER NAME
                            txtPvp1.Text = $"?{product.PriceOfSale:f2}";
                            txtPvp2.Text = $"?{product.Pvp2:f2}";
                            txtPcu.Text = $"?{product.Pcu:f2}";
                            searchingByReference = false;
                        }
                        else
                            ClearFields("ref");
                    }
                }
            };
        }

        //-----------------------------------------------------------
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            menu.Add(new Java.Lang.String("Configurações"));
            menu.Add(new Java.Lang.String("Sobre"));
            return true;
        }


        //-----------------------------------------------------------
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

        //-----------------------------------------------------------
        private void ClearFields(string from)
        {
            switch (from)
            {
                case "codBarras":
                    FindViewById<TextView>(Resource.Id.txtCodigo).Text = "";
                    FindViewById<TextView>(Resource.Id.txtRef).Text = "";
                    break;
                case "codigo":
                    FindViewById<EditText>(Resource.Id.txtCodBarras).Text = "";
                    FindViewById<TextView>(Resource.Id.txtRef).Text = "";
                    break;
                case "ref":
                    FindViewById<EditText>(Resource.Id.txtCodBarras).Text = "";
                    FindViewById<TextView>(Resource.Id.txtCodigo).Text = "";
                    break;
                default:
                    FindViewById<EditText>(Resource.Id.txtCodBarras).Text = "";
                    FindViewById<TextView>(Resource.Id.txtCodigo).Text = "";
                    FindViewById<TextView>(Resource.Id.txtRef).Text = "";
                    break;
            }

            FindViewById<TextView>(Resource.Id.txtDescricao).Text = "";
            FindViewById<TextView>(Resource.Id.txtFornecedor).Text = "";
            FindViewById<TextView>(Resource.Id.txtPvp1).Text = "";
            FindViewById<TextView>(Resource.Id.txtPvp2).Text = "";
        }
    }
}