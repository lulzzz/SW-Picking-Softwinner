using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using ZSProduct.Modal;
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
        private int _selectedStore;
        private readonly IList<AddStockStore> _stockList = new List<AddStockStore>();

        protected override void OnCreate(Bundle savedInstanceState)
        {
            var searchingByBarCode = false;
            var searchingByCode = false;
            var searchingByReference = false;
            _stockList.Add(new AddStockStore("o", "a"));
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.ProductFinder);
            var toolBar = FindViewById<SupportToolbar>(Resource.Id.toolBar);
            SetSupportActionBar(toolBar);
            var ab = SupportActionBar;
            ab.SetHomeAsUpIndicator(Resource.Drawable.ic_arrow_back_white_18dp);
            ab.SetDisplayHomeAsUpEnabled(true);

            _nif = _manager.GetItem("nif");
            _username = _manager.GetItem("username");
            _password = _manager.GetItem("password");
            _zsClient = new ZsClient(_username, _password, 0, _nif);
            _zsClient.Login();
            //BuildSpinner();
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

            _txtBarCode.FocusChange += (sender, args) => { if (args.HasFocus) _txtBarCode.Text = ""; };

            _txtCode.FocusChange += (sender, args) => { if (args.HasFocus) _txtCode.Text = ""; };

            _txtRef.FocusChange += (sender, args) => { if (args.HasFocus) _txtRef.Text = ""; };

            _txtBarCode.KeyPress += (sender, e) =>
            {
                if (searchingByCode || searchingByReference) return;
                searchingByBarCode = true;
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
                    var barCode = _txtBarCode.Text;
                    var zsHandler = new ZsClient(_username, _password, 0, _nif);
                    zsHandler.Login();
                    var totalStores = zsHandler.StoreCount();
                    for (var i = 0; i < totalStores; i++)
                    {
                        Console.WriteLine("FOR LOJA " + i);
                        zsHandler = new ZsClient(_username, _password, i, _nif);
                        zsHandler.Login();
                        var product = zsHandler.GetProductWithBarCode(barCode);
                        if (product != null)
                        {
                            var stock = _zsClient.GetStockInStoresWithProductCode(product.ProductCode.ToString());
                            _txtBarCode.Text = product.BarCode.ToString();
                            _txtCode.Text = product.ProductCode.ToString();
                            _txtRef.Text = product.Reference.ToString();
                            _txtDesc.Text = product.Description.ToString();
                            _txtSupplier.Text = product.SupplierId.ToString();
                            _txtPvp1.Text = product.Pvp1 == null ? "-" : product.Pvp1 + " €";
                            _txtPvp2.Text = product.Pvp2 == null ? "-" : product.Pvp2 + " €";
                            _txtPcu.Text = product.Pcu == null ? "-" : product.Pcu + " €";
                            _txtPcm.Text = product.Pcm ?? "-";
                            for (int j = 0; j < totalStores; j++)
                                //_stockList.Add(new AddStockStore(i.ToString(), stock.ElementAt(i).ToString()));
                                _stockList.Add(new AddStockStore(i.ToString(), i.ToString()));
                            try { _txtTotalStock.Text = stock.Sum(pos => pos.Value).ToString(); }
                            catch (Exception) { _txtTotalStock.Text = "0"; }
                            break;
                        }
                        if (i != totalStores - 1) continue;
                        Toast.MakeText(this, "Produto não encontrado", ToastLength.Long).Show();
                        ClearFields();
                    }
                }
                e.Handled = true;
                searchingByBarCode = false;
            };

            _txtCode.KeyPress += (sender, e) =>
            {
                if (searchingByBarCode && searchingByReference) return;
                searchingByCode = true;
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
                    var code = _txtCode.Text;
                    var zsHandler = new ZsClient(_username, _password, 0, _nif);
                    zsHandler.Login();
                    var totalStores = zsHandler.StoreCount();
                    for (var i = 0; i < totalStores; i++)
                    {
                        Console.WriteLine("FOR LOJA " + i);
                        zsHandler = new ZsClient(_username, _password, i, _nif);
                        zsHandler.Login();
                        var product = zsHandler.GetProductWithCode(code);
                        if (product != null)
                        {
                            var stock = _zsClient.GetStockInStoresWithProductCode(product.ProductCode.ToString());
                            _txtBarCode.Text = product.BarCode.ToString();
                            _txtCode.Text = product.ProductCode.ToString();
                            _txtRef.Text = product.Reference.ToString();
                            _txtDesc.Text = product.Description.ToString();
                            _txtSupplier.Text = product.SupplierId.ToString();
                            _txtPvp1.Text = product.Pvp1 == null ? "-" : product.Pvp1 + " €";
                            _txtPvp2.Text = product.Pvp2 == null ? "-" : product.Pvp2 + " €";
                            _txtPcu.Text = product.Pcu == null ? "-" : product.Pcu + " €";
                            _txtPcm.Text = product.Pcm ?? "-";
                            try { _txtTotalStock.Text = stock.Sum(pos => pos.Value).ToString(); }
                            catch (Exception) { _txtTotalStock.Text = "0"; }
                            break;
                        }
                        if (i != totalStores - 1) continue;
                        Toast.MakeText(this, "Produto não encontrado", ToastLength.Long).Show();
                        ClearFields();
                    }
                }
                searchingByCode = false;
                e.Handled = true;
            };

            _txtRef.KeyPress += (sender, e) =>
            {
                if (searchingByCode || searchingByBarCode) return;
                searchingByReference = true;
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
                    _txtTotalStock.Text = etiConn.GetStockForProductWithBarCode(_txtRef.Text);
                }
                else
                {
                    //Search in the zonesoft cloud
                    Console.WriteLine("Searching in zonesoft cloud");
                    var refe = _txtRef.Text;
                    var zsHandler = new ZsClient(_username, _password, 0, _nif);
                    zsHandler.Login();
                    var totalStores = zsHandler.StoreCount();
                    for (var i = 0; i < totalStores; i++)
                    {
                        Console.WriteLine("FOR LOJA " + i);
                        zsHandler = new ZsClient(_username, _password, i, _nif);
                        zsHandler.Login();
                        var product = zsHandler.GetProductWithReference(refe);
                        if (product != null)
                        {
                            var stock = _zsClient.GetStockInStoresWithProductCode(product.ProductCode.ToString());
                            _txtBarCode.Text = product.BarCode.ToString();
                            _txtCode.Text = product.ProductCode.ToString();
                            _txtRef.Text = product.Reference.ToString();
                            _txtDesc.Text = product.Description.ToString();
                            _txtSupplier.Text = product.SupplierId.ToString();
                            _txtPvp1.Text = product.Pvp1 == null ? "-" : product.Pvp1 + " €";
                            _txtPvp2.Text = product.Pvp2 == null ? "-" : product.Pvp2 + " €";
                            _txtPcu.Text = product.Pcu == null ? "-" : product.Pcu + " €";
                            _txtPcm.Text = product.Pcm ?? "-";
                            try { _txtTotalStock.Text = stock.Sum(pos => pos.Value).ToString(); }
                            catch (Exception) { _txtTotalStock.Text = "0"; }
                            break;
                        }
                        if (i != totalStores - 1) continue;
                        Toast.MakeText(this, "Produto não encontrado", ToastLength.Long).Show();
                        ClearFields();
                    }
                }
                e.Handled = true;
                searchingByReference = false;
            };

            FindViewById<FloatingActionButton>(Resource.Id.fab).Click += (sender, args) =>
            {
                /*var stockOnStoresActivity = new Intent(this, typeof(StockOnStores));
                StartActivity(stockOnStoresActivity);*/

                //var d = new Dialog(this);
                //d.SetContentView(S);
                ////FindViewById<ListView>(Resource.Id.lstStocks).Adapter = new AdapterListViewStock(this, _stockList);
                //d.Show();
            };
        }

        //Build the spinner dinamic
        /*private void BuildSpinner()
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
        }*/

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