using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using ZSProduct.Modal;
using Android.Support.V4.Widget;
using SupportToolbar = Android.Support.V7.Widget.Toolbar;

namespace ZSProduct
{
    [Activity(Label = "Detalhes do Produto", Theme = "@style/Theme.DesignDemo", ScreenOrientation = ScreenOrientation.Portrait)]
    public class ProductFinder : AppCompatActivity
    {
        public const int ETICADATA = -1;
        private readonly ZsManager _manager = new ZsManager();
        private ZsClient _zsClient;
        private string _nif;
        private string _username;
        private string _password;
        private EditText _txtBarCode;
        private TextView _txtCode;
        private TextView _txtRef;
        private TextView _txtDesc;
        private TextView _txtSupplier;
        private TextView _txtPvp1;
        private TextView _txtPvp2;
        private TextView _txtPcu;
        private TextView _txtPcm;
        private TextView _txtTotalStock;
        private Spinner _optProdFinderStores;

        private int _selectedStore;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.ProductFinder);
            var toolBar = FindViewById<SupportToolbar>(Resource.Id.toolBar);
            SetSupportActionBar(toolBar);
            var ab = SupportActionBar;
            ab.SetHomeAsUpIndicator(Resource.Drawable.ic_delete_black_18dp);
            ab.SetDisplayHomeAsUpEnabled(true);

            _nif = _manager.GetItem("nif");
            _username = _manager.GetItem("username");
            _password = _manager.GetItem("password");
            _zsClient = new ZsClient(_username, _password, _selectedStore, _nif);
            _zsClient.Login();
            BuildSpinner();
            _txtBarCode = FindViewById<EditText>(Resource.Id.txtProdFinderBarCode);
            _txtCode = FindViewById<TextView>(Resource.Id.txtProdFinderProductCode);
            _txtRef = FindViewById<TextView>(Resource.Id.txtProdFinderProductReference);
            _txtDesc = FindViewById<TextView>(Resource.Id.txtProdFinderDescription);
            _txtSupplier = FindViewById<TextView>(Resource.Id.txtProdFinderSupplier);
            _txtPvp1 = FindViewById<TextView>(Resource.Id.txtProdFinderPvp1);
            _txtPvp2 = FindViewById<TextView>(Resource.Id.txtProdFinderPvp2);
            _txtPcu = FindViewById<TextView>(Resource.Id.txtProdFinderPcu);
            _txtPcm = FindViewById<TextView>(Resource.Id.txtProdFinderPcm);
            _txtTotalStock = FindViewById<TextView>(Resource.Id.txtProdFinderStockTotal);

            _txtBarCode.KeyPress += (sender, e) =>
            {
                e.Handled = false;
                if (e.Event.Action != KeyEventActions.Down || e.KeyCode != Keycode.Enter) return;
                if (_selectedStore == ETICADATA)
                {
                    //Search in eticadata
                    _manager.CheckEticadataIntegration();
                    Console.WriteLine("Searching in eticadata SQL Server");
                    Console.WriteLine(_manager.Eticadata.Username);
                    Console.WriteLine(_manager.Eticadata.Password);
                    Console.WriteLine(_manager.Eticadata.ServerAddress);
                    var etiConn = new EtiClient("xx", "xxx", "192.168.174");
                    _txtTotalStock.Text = etiConn.GetStockForProductWithBarCode(_txtBarCode.Text);
                }
                else
                {
                    //Search in the zonesoft cloud
                    Console.WriteLine("Searching in zonesoft cloud");
                    _zsClient = new ZsClient(_username, _password, _selectedStore, _nif);
                    _zsClient.Login();
                    var product = _zsClient.GetProductWithBarCode(_txtBarCode.Text);

                    if (product != null)
                    {
                        var stock = _zsClient.GetStockInStoresWithProductCode(product.productCode.ToString());
                        //searchedBarCode.Text = barCodeEntered;
                        _txtBarCode.Text = product.barCode.ToString();
                        _txtCode.Text = product.productCode.ToString();
                        _txtRef.Text = product.reference.ToString();
                        _txtDesc.Text = product.description.ToString();
                        _txtSupplier.Text = product.supplierId.ToString();
                        _txtPvp1.Text = product.pvp1 == null ? "-" : product.pvp1 + " €";
                        _txtPvp2.Text = product.pvp2 == null ? "-" : product.pvp2 + " €";
                        _txtPcu.Text = product.pcu == null ? "-" : product.pcu + " €";
                        //_txtPcm.Text = product. == null ? "-" : product.pcu + " €";
                        _txtTotalStock.Text = stock.Sum(pos => pos.Value).ToString();
                    }
                    else
                    {
                        _txtBarCode.Text = "";
                        Toast.MakeText(this, "Producto não encontrado!", ToastLength.Long).Show();

                    }
                }
                e.Handled = true;
            };
        }

        //Build the spinner dinamic
        private void BuildSpinner()
        {
            _zsClient = new ZsClient(_username, _password, 0, _nif);
            _zsClient.Login();
            var stores = _zsClient.GetStoresList();
            var items = new List<string>();
            //Chech if ZSManager has eticadata integration
            if (_manager.HasEticadataIntegration)
            {
                items.Add("Servidor Eticadata");
            }
            foreach (var store in stores)
            {
                items.Add(store.description);
            }

            var adapter = new ArrayAdapter<String>(this, Android.Resource.Layout.SimpleSpinnerDropDownItem, items);
            var spinner = FindViewById<Spinner>(Resource.Id.optProdFinderStores);
            spinner.Adapter = adapter;
            spinner.ItemSelected += spinner_ItemSelected;
        }

        //Handler for the drop down list
        private void spinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            var spinner = (Spinner)sender;
            var storeTitle = spinner.GetItemAtPosition(e.Position).ToString();
            if (storeTitle.Contains("Servidor Eticadata"))
            {
                _selectedStore = ETICADATA;
                Console.WriteLine(storeTitle);
                ClearFields();
                Toast.MakeText(this, $"Loja  {spinner.GetItemAtPosition(e.Position)} selecionada", ToastLength.Long).Show();
            }
            else
            {
                _selectedStore = e.Position;
                ClearFields();
                Toast.MakeText(this, $"Loja  {spinner.GetItemAtPosition(e.Position)} selecionada", ToastLength.Long).Show();
            }
        }

        private void ClearFields()
        {
            _txtBarCode.Text = "";
            _txtCode.Text = "";
            _txtRef.Text = "";
            _txtDesc.Text = "";
            _txtSupplier.Text = "";
            _txtPvp1.Text = "";
            _txtPvp2.Text = "";
            _txtPcu.Text = "";
            _txtTotalStock.Text = "";
        }
    }
}