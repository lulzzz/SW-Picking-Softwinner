
using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.OS;
using Android.Widget;
using ZSProduct;
using App2.Modal;

namespace App2
{
    [Activity(Label = "Detalhes de  Produtos")]
    public class ProductFinder : Activity
    {
        ZSClient zsClient;
        int selectedStore;
        string nif;
        string username;
        string password;


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.ProductFinder);
            var manager = new ZsManager();
            nif = manager.GetItem("nif");
            username = manager.GetItem("username");
            password = manager.GetItem("password");

            buildSpinner();
            EditText txtBarCode = FindViewById<EditText>(Resource.Id.txtBarCode);
            TextView searchedBarCode = FindViewById<TextView>(Resource.Id.barCode);
            TextView txtCode = FindViewById<TextView>(Resource.Id.productCode);
            TextView txtRef = FindViewById<TextView>(Resource.Id.productRef);
            TextView txtDesc = FindViewById<TextView>(Resource.Id.productDescription);
            TextView txtSupplier = FindViewById<TextView>(Resource.Id.productSupplier);
            TextView txtPvp1 = FindViewById<TextView>(Resource.Id.productPVP1);
            TextView txtPvp2 = FindViewById<TextView>(Resource.Id.productPVP2);
            TextView txtstock = FindViewById<TextView>(Resource.Id.stock);
            TextView txtTotalstock = FindViewById<TextView>(Resource.Id.totalStockInStores);

            txtBarCode.TextChanged += (sender, e) =>
            {
                if (e.Text.Contains('\n') || e.Text.Contains('%'))
                {
                    String barCodeEntered = e.Text.ToString();
                    barCodeEntered = barCodeEntered.Remove(barCodeEntered.Count() - 1);
                    Console.WriteLine("Searching for barcode {0}", barCodeEntered);
                    //Clear the textField to the new search

                    var myProgressBar = new ProgressBar(this);
                    myProgressBar.Animate();

                    zsClient = new ZSClient(username, password, selectedStore, nif);
                    zsClient.Login();
                    var product = zsClient.GetProductWithBarCode(barCodeEntered);

                    if (product != null)
                    {
                        var stock = zsClient.GetStockInStoresWithProductCode(product.productCode.ToString());
                        searchedBarCode.Text = barCodeEntered;
                        txtCode.Text = product.productCode.ToString();
                        txtRef.Text = product.reference.ToString();
                        txtDesc.Text = product.description.ToString();
                        txtSupplier.Text = product.supplierId.ToString();
                        txtPvp1.Text = product.pvp1.ToString() + " €";
                        txtPvp2.Text = product.pvp2.ToString() + " €";
                        txtBarCode.Text = "";
                        txtstock.Text = stock.ElementAt(selectedStore).Value.ToString();
                        txtTotalstock.Text = stock.Sum(pos => pos.Value).ToString();
                    }
                    else
                    {
                        txtBarCode.Text = "";
                        Toast.MakeText(this, "Producto não encontrado!", ToastLength.Long).Show();

                    }

                }
            };

            FindViewById<TextView>(Resource.Id.totalStockInStores).Click += (sender, args) =>
            {
                //Toast.MakeText(this, "oaef", ToastLength.Short).Show();
                var transaction = FragmentManager.BeginTransaction();
                var dialogFragment = new DialogShowStocks();
                dialogFragment.Show(transaction, "dialog_fragment");
            };
        }
        //Build the spinner dinamic
        private void buildSpinner()
        {
            zsClient = new ZSClient(username, password, 0, nif);
            zsClient.Login();
            var stores = zsClient.GetStoresList();
            var items = new List<string>();
            foreach (var store in stores)
            {
                items.Add(store.description);
            }

            var adapter = new ArrayAdapter<String>(this, Android.Resource.Layout.SimpleSpinnerItem, items);
            var spinner = FindViewById<Spinner>(Resource.Id.storeSpinner);
            spinner.Adapter = adapter;
            spinner.ItemSelected += spinner_ItemSelected;
        }
        //Handler for the drop down list
        private void spinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            Spinner spinner = (Spinner)sender;
            selectedStore = e.Position;
            clearScreen();
            string toast = string.Format("Loja  {0} selecionada", spinner.GetItemAtPosition(e.Position));
            Toast.MakeText(this, toast, ToastLength.Long).Show();
        }

        private void clearScreen()
        {
            EditText txtBarCode = FindViewById<EditText>(Resource.Id.txtBarCode);
            TextView searchedBarCode = FindViewById<TextView>(Resource.Id.barCode);
            TextView txtCode = FindViewById<TextView>(Resource.Id.productCode);
            TextView txtRef = FindViewById<TextView>(Resource.Id.productRef);
            TextView txtDesc = FindViewById<TextView>(Resource.Id.productDescription);
            TextView txtSupplier = FindViewById<TextView>(Resource.Id.productSupplier);
            TextView txtPvp1 = FindViewById<TextView>(Resource.Id.productPVP1);
            TextView txtPvp2 = FindViewById<TextView>(Resource.Id.productPVP2);
            TextView txtstock = FindViewById<TextView>(Resource.Id.stock);
            TextView txtTotalstock = FindViewById<TextView>(Resource.Id.totalStockInStores);
            searchedBarCode.Text = "";
            txtCode.Text = "";
            txtRef.Text = "";
            txtDesc.Text = "";
            txtSupplier.Text = "";
            txtPvp1.Text = "";
            txtPvp2.Text = "";
            txtBarCode.Text = "";
            txtstock.Text = "";
            txtTotalstock.Text = "";
        }

    }
}

