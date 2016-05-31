
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using ZSProduct;
using System.Runtime.InteropServices;
using Android.Content.PM;
using App2.Modal;

namespace App2
{
    [Activity(Label = "Procurar Produtos", ScreenOrientation = ScreenOrientation.Portrait)]
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

            var txtBarCode = FindViewById<EditText>(Resource.Id.txtBarCode);
            var searchedBarCode = FindViewById<TextView>(Resource.Id.barCode);
            var txtCode = FindViewById<TextView>(Resource.Id.productCode);
            var txtRef = FindViewById<TextView>(Resource.Id.productRef);
            var txtDesc = FindViewById<TextView>(Resource.Id.productDescription);
            var txtSupplier = FindViewById<TextView>(Resource.Id.productSupplier);
            var txtPvp1 = FindViewById<TextView>(Resource.Id.productPVP1);
            var txtPvp2 = FindViewById<TextView>(Resource.Id.productPVP2);
            var txtstock = FindViewById<TextView>(Resource.Id.stock);
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
                        searchedBarCode.Text = barCodeEntered;
                        txtCode.Text = product.productCode.ToString();
                        txtRef.Text = product.reference.ToString();
                        txtDesc.Text = product.description.ToString();
                        txtSupplier.Text = product.supplierId.ToString();
                        txtPvp1.Text = product.pvp1.ToString() + " €";
                        txtPvp2.Text = product.pvp2.ToString() + " €";
                        txtBarCode.Text = "";
                        txtstock.Text = zsClient.GetStockForStoresWithProductCode(product.productCode.ToString());
                    }
                    else
                    {
                        txtBarCode.Text = "";
                        Toast.MakeText(this, "Producto não encontrado!", ToastLength.Long).Show();

                    }

                }
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
            spinner.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(spinner_ItemSelected);
        }
        //Handler for the drop down list
        private void spinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            Spinner spinner = (Spinner)sender;
            selectedStore = e.Position;
            string toast = string.Format("Loja  {0} selecionada", spinner.GetItemAtPosition(e.Position));
            Toast.MakeText(this, toast, ToastLength.Long).Show();
        }

    }
}

