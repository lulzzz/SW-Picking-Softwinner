using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using ZSProduct.Modal;
using SupportToolbar = Android.Support.V7.Widget.Toolbar;

//-----------------------------------------------------------
namespace ZSProduct
{
    //-----------------------------------------------------------
    [Activity(Label = "Detalhes do Produto", Theme = "@style/Theme.DesignDemo", ScreenOrientation = ScreenOrientation.Portrait)]
    public class ProductFinder : AppCompatActivity
    {
        //-----------------------------------------------------------
        public const int Eticadata = -1;

        //-----------------------------------------------------------
        private readonly ZsManager _manager = new ZsManager();
        private ZsClient _zsClient;
        private EtiClient _etiClient;
        private string _nif;
        private string _username;
        private string _password;
        private string _ip;
        private string _loginType;
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
        private bool _searchingByBarCode;
        private bool _searchingByCode;
        private bool _searchingByReference;
        private List<AddStockStore> _listStocks;
        private BackgroundWorker _mWorker;
        private Product _product;

        public Dictionary<string, int> Stock;

        //-----------------------------------------------------------
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.ProductFinder);
            var toolBar = FindViewById<SupportToolbar>(Resource.Id.toolBar);
            SetSupportActionBar(toolBar);
            var ab = SupportActionBar;
            ab.SetHomeAsUpIndicator(Resource.Drawable.ic_arrow_back_white_18dp);
            ab.SetDisplayHomeAsUpEnabled(true);
            _mWorker = new BackgroundWorker();
            _username = _manager.GetItem("username");
            _password = _manager.GetItem("password");
            _loginType = _manager.GetItem("loginType");
            if (_loginType == "zonesoft")
            {
                _nif = _manager.GetItem("nif");
                _zsClient = new ZsClient(_username, _password, 0, _nif);
                _zsClient.Login();
            }
            else
            {
                _ip = _manager.GetItem("ip");
                _etiClient = new EtiClient(_username, _password, _ip);
                _etiClient.Login();
            }
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

            _txtBarCode.Click += (sender, args) => _txtBarCode.Text = "";

            _txtCode.Click += (sender, args) => _txtCode.Text = "";

            _txtRef.Click += (sender, args) => _txtRef.Text = "";

            _txtBarCode.KeyPress += (sender, e) =>
            {
                if (_searchingByCode || _searchingByReference) return;
                _searchingByBarCode = true;
                e.Handled = false;
                if (e.Event.Action != KeyEventActions.Down || e.KeyCode != Keycode.Enter) return;
                //Search in the zonesoft cloud
                var transaction = FragmentManager.BeginTransaction();
                var dialogFragment = new DialogLoading();
                dialogFragment.Show(transaction, "dialog_fragment");
                Console.WriteLine("Searching in zonesoft cloud");
                var barCode = _txtBarCode.Text;
                ZsClient zsHandler = null;
                _mWorker.DoWork += (o, a) =>
                {
                    zsHandler = new ZsClient(_username, _password, 0, _nif);
                    zsHandler.Login();
                    var totalStores = zsHandler.StoreCount();
                    if (_loginType == "zonesoft")
                    {
                        Console.WriteLine("Search on ZONE SOFT");
                        for (var i = 0; i < totalStores; i++)
                        {
                            Console.WriteLine("FOR LOJA " + i);
                            zsHandler = new ZsClient(_username, _password, i, _nif);
                            zsHandler.Login();
                            var product = zsHandler.GetProductWithBarCode(barCode);
                            if (product != null)
                            {
                                _product = product;
                                break;
                            }
                            if (i != totalStores - 1) continue;
                            Toast.MakeText(this, "Produto não encontrado", ToastLength.Long).Show();
                            ClearFields("barCode");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Search on Eticadata");
                        var etiHandler = new EtiClient(_username, _password, _ip);
                        etiHandler.Login();
                        var product = etiHandler.GetProductWithBarCode(barCode);
                        if (product != null)
                        {
                            //_stock = zsHandler.GetStockInStoresWithProductCode(product.ProductCode.ToString());
                            _txtBarCode.Text = product.BarCode.ToString();
                            _txtCode.Text = product.ProductCode.ToString();
                            _txtRef.Text = product.Reference.ToString();
                            _txtDesc.Text = product.Description.ToString();
                            //_txtSupplier.Text = zsHandler.GetSupplierNameWithCode((int)product.SupplierId);
                            _txtPvp1.Text = product.Pvp1 == null ? "-" : product.Pvp1 + " €";
                            _txtPvp2.Text = product.Pvp2 == null ? "-" : product.Pvp2 + " €";
                            _txtPcu.Text = product.Pcu == null ? "-" : product.Pcu + " €";
                            _txtPcm.Text = product.Pcm ?? "-";
                            _txtTotalStock.Text = product.Stock.ToString();
                            /*for (var x = 0; x > totalStores; x++)
                                _listStocks.Add(new AddStockStore(x.ToString(), x.ToString()));
                            try
                            {
                                _txtTotalStock.Text = _stock.Sum(pos => pos.Value).ToString();
                            }
                            catch (Exception)
                            {
                                _txtTotalStock.Text = "0";
                            }*/
                            //_stock = stock;
                        }
                    }

                };
                _mWorker.RunWorkerAsync();
                _mWorker.RunWorkerCompleted += (o, args) =>
                {
                    zsHandler.Login();
                    string stock;
                    Stock = zsHandler.GetStockInStoresWithProductCode(_product.ProductCode.ToString());
                    for (var x = 0; x > 11; x++)
                        _listStocks.Add(new AddStockStore(x.ToString(), x.ToString()));
                    try { stock = Stock.Sum(pos => pos.Value).ToString(); } catch (Exception) { stock = "0"; }
                    dialogFragment.Dismiss();
                    UpdateFields(_product.BarCode,
                                 _product.ProductCode,
                                 _product.Reference,
                                 _product.Description,
                                 stock,
                                 _product.Pvp1 > 0 ? "€" + _product.Pvp1 : "-",
                                 _product.Pvp2 > 0 ? "€" + _product.Pvp2 : "-",
                                 zsHandler.GetSupplierNameWithCode((int)_product.SupplierId),
                                 Convert.ToDecimal(_product.Pcu) > 0 ? "€" + _product.Pcu : "-",
                                 "-");

                };
                e.Handled = true;
                _searchingByBarCode = false;
            };

            _txtCode.KeyPress += (sender, e) =>
            {
                if (_searchingByBarCode && _searchingByReference) return;
                _searchingByCode = true;
                e.Handled = false;
                if (e.Event.Action != KeyEventActions.Down || e.KeyCode != Keycode.Enter) return;
                var transaction = FragmentManager.BeginTransaction();
                var dialogFragment = new DialogLoading();
                dialogFragment.Show(transaction, "dialog_fragment");
                //Search in the zonesoft cloud
                Console.WriteLine("Searching in zonesoft cloud");
                var code = _txtCode.Text;
                var zsHandler = new ZsClient(_username, _password, 0, _nif);
                _mWorker.DoWork += (o, a) =>
                {
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
                            _product = product;
                            break;
                        }
                        if (i != totalStores - 1) continue;
                        Toast.MakeText(this, "Produto não encontrado", ToastLength.Long).Show();
                        ClearFields("code");
                    }
                };
                _mWorker.RunWorkerAsync();
                _mWorker.RunWorkerCompleted += (o, args) =>
                {
                    zsHandler.Login();
                    string stock;
                    Stock = zsHandler.GetStockInStoresWithProductCode(_product.ProductCode.ToString());
                    for (var x = 0; x > 11; x++)
                        _listStocks.Add(new AddStockStore(x.ToString(), x.ToString()));
                    try { stock = Stock.Sum(pos => pos.Value).ToString(); } catch (Exception) { stock = "0"; }
                    dialogFragment.Dismiss();

                    UpdateFields(_product.BarCode,
                                  _product.ProductCode,
                                  _product.Reference,
                                  _product.Description,
                                  stock,
                                  _product.Pvp1 > 0 ? "€" + _product.Pvp1 : "-",
                                  _product.Pvp2 > 0 ? "€" + _product.Pvp2 : "-",
                                  zsHandler.GetSupplierNameWithCode((int)_product.SupplierId),
                                  Convert.ToDecimal(_product.Pcu) > 0 ? "€" + _product.Pcu : "-",
                                  "-");

                };
                _searchingByCode = false;
                e.Handled = true;
            };

            _txtRef.KeyPress += (sender, e) =>
            {
                if (_searchingByCode || _searchingByBarCode) return;
                _searchingByReference = true;
                e.Handled = false;
                if (e.Event.Action != KeyEventActions.Down || e.KeyCode != Keycode.Enter) return;
                var transaction = FragmentManager.BeginTransaction();
                var dialogFragment = new DialogLoading();
                dialogFragment.Show(transaction, "dialog_fragment");

                //Search in the zonesoft cloud
                Console.WriteLine("Searching in zonesoft cloud");
                var refe = _txtRef.Text;
                var zsHandler = new ZsClient(_username, _password, 0, _nif);
                _mWorker.DoWork += (o, a) =>
                {
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
                            _product = product;
                            break;
                        }
                        if (i != totalStores - 1) continue;
                        Toast.MakeText(this, "Produto não encontrado", ToastLength.Long).Show();
                        ClearFields("ref");
                    }
                };
                _mWorker.RunWorkerAsync();
                _mWorker.RunWorkerCompleted += (o, args) =>
                {
                    zsHandler.Login();
                    string stock;
                    Stock = zsHandler.GetStockInStoresWithProductCode(_product.ProductCode.ToString());
                    for (var x = 0; x > 11; x++)
                        _listStocks.Add(new AddStockStore(x.ToString(), x.ToString()));
                    try { stock = Stock.Sum(pos => pos.Value).ToString(); } catch (Exception) { stock = "0"; }
                    dialogFragment.Dismiss();

                    UpdateFields(_product.BarCode,
                                 _product.ProductCode,
                                 _product.Reference,
                                 _product.Description,
                                 stock,
                                 _product.Pvp1 > 0 ? "€" + _product.Pvp1 : "-",
                                 _product.Pvp2 > 0 ? "€" + _product.Pvp2 : "-",
                                 zsHandler.GetSupplierNameWithCode((int)_product.SupplierId),
                                 Convert.ToDecimal(_product.Pcu) > 0 ? "€" + _product.Pcu : "-",
                                 "-");

                };
                e.Handled = true;
                _searchingByReference = false;
            };

            FindViewById<FloatingActionButton>(Resource.Id.fab).Click += (sender, args) =>
            {
                if (Stock == null)
                {
                    Toast.MakeText(this, "Não tem stock...", ToastLength.Short).Show();
                    return;
                }
                var transition = FragmentManager.BeginTransaction();
                var showStocks = new DialogStock(Stock);
                showStocks.Show(transition, "dialog fragment");
            };
        }

        //-----------------------------------------------------------
        private void UpdateFields(string barCode, string code, string reference, string description, string stock, string pvp1, string pvp2, string supplier, string pcu, string pcm)
        {
            _txtBarCode.Text = barCode;
            _txtCode.Text = code;
            _txtRef.Text = reference;
            _txtDesc.Text = description;
            _txtSupplier.Text = supplier;
            _txtPvp1.Text = pvp1;
            _txtPvp2.Text = pvp2;
            _txtPcu.Text = pcu;
            _txtPcm.Text = pcm;
            _txtTotalStock.Text = stock;
        }

        //-----------------------------------------------------------
        private void ClearFields(string from)
        {
            if (from != "barCode")
                _txtBarCode.Text = "";
            if (from != "code")
                _txtCode.Text = "";
            if (from != "ref")
                _txtRef.Text = "";
            _txtDesc.Text = "";
            _txtSupplier.Text = "";
            _txtPvp1.Text = "";
            _txtPvp2.Text = "";
            _txtPcm.Text = "";
            _txtPcu.Text = "";
            _txtTotalStock.Text = "";
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
    }
}