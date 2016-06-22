using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using ZSProduct.Modal;
using static Android.Views.ViewGroup.LayoutParams;
using String = Java.Lang.String;
using SupportToolbar = Android.Support.V7.Widget.Toolbar;

//-----------------------------------------------------------
namespace ZSProduct
{
    //-----------------------------------------------------------
    [Activity(Label = "@string/productFinderLabel", Theme = "@style/Theme.DesignDemo", ScreenOrientation = ScreenOrientation.Portrait)]
    public class ProductFinder : AppCompatActivity
    {
        //-----------------------------------------------------------
        private ZsManager _manager;
        private ZsClient _zsClient;
        private EtiClient _etiClient;
        private string _nif;
        private string _username;
        private string _password;
        private string _ip;
        private string _loginType;
        private uint _port;
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
        private bool _searchingByBarCode;
        private bool _searchingByCode;
        private bool _searchingByReference;
        private bool _searchComplete;
        private BackgroundWorker _mWorker;
        private Product _product;
        private int _stock;
        public Dictionary<string, int> Stock;
        public List<AddStockStore> ListStocks;

        //-----------------------------------------------------------
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.ProductFinder);
            var toolBar = FindViewById<SupportToolbar>(Resource.Id.toolBar);
            SetSupportActionBar(toolBar);
            toolBar.SetNavigationIcon(Resource.Drawable.ic_arrow_back_white_18dp);
            toolBar.NavigationClick += (sender, args) => OnBackPressed();

            _manager = new ZsManager(ApplicationContext);
            _mWorker = new BackgroundWorker();
            _username = _manager.GetItem("username");
            _password = _manager.GetItem("password");
            _loginType = _manager.GetItem("loginType");

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
            ListStocks = new List<AddStockStore>();
            _stock = 0;

            if (_loginType == "zonesoft")
            {
                _nif = _manager.GetItem("nif");
                _zsClient = new ZsClient(_username, _password, 0, _nif);
                _zsClient.Login();
            }
            else
            {
                _txtRef.Enabled = false;
                _ip = _manager.GetItem("ip");
                _port = Convert.ToUInt32(_manager.GetItem("port"));
                _etiClient = new EtiClient(_username, _password, _ip, _port);
                _etiClient.Login();
            }

            _txtBarCode.Click += (sender, args) => _txtBarCode.Text = "";

            _txtCode.Click += (sender, args) =>
            {
                if (_loginType != "eticadata") _txtCode.Text = "";
            };

            _txtRef.Click += (sender, args) => _txtRef.Text = "";

            _txtBarCode.KeyPress += (sender, e) =>
            {
                if (_searchingByCode || _searchingByReference) return;
                if (_searchComplete)
                {
                    _txtBarCode.Text = "";
                    _searchComplete = false;
                }
                _searchingByBarCode = true;
                e.Handled = false;
                if (e.Event.Action != KeyEventActions.Down || e.KeyCode != Keycode.Enter) return;
                //Search in the zonesoft cloud
                var transaction = FragmentManager.BeginTransaction();
                var dialogFragment = new DialogLoading();
                dialogFragment.Show(transaction, "dialog_fragment");
                var barCode = _txtBarCode.Text;
                ZsClient zsHandler = null;
                _product = null;
                _mWorker.DoWork += (a, b) =>
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
                            _product = null;
                        }
                    }
                    else
                    {
                        if (b.Cancel) return;
                        Console.WriteLine("Search on Eticadata");
                        barCode = null;
                        barCode = _txtBarCode.Text;
                        _product = null;
                        var etiHandler = new EtiClient(_username, _password, _ip, _port);
                        etiHandler.Login();
                        var product = etiHandler.GetProductWithBarCode(barCode);
                        if (product != null)
                        {
                            _product = product;
                            Stock = product.Stock;
                            _stock = 0;
                            foreach (var item in new List<int>(product.Stock.Values))
                                _stock += item;
                            b.Cancel = true;
                        }
                    }

                };
                _mWorker.RunWorkerAsync();
                _mWorker.RunWorkerCompleted += (o, args) =>
                {
                    if (_manager.GetItem("loginType") == "zonesoft")
                    {
                        zsHandler.Login();
                        string stock;
                        if (_product != null)
                            Stock = zsHandler.GetStockInStoresWithProductCode(_product.ProductCode.ToString());
                        for (var x = 0; x > 11; x++)
                            ListStocks.Add(new AddStockStore(x.ToString(), x.ToString()));
                        try
                        {
                            stock = Stock.Sum(pos => pos.Value).ToString();
                        }
                        catch (Exception)
                        {
                            stock = "0";
                        }
                        dialogFragment.Dismiss();
                        if (_product != null)
                        {
                            UpdateFields(_product.BarCode,
                                _product.ProductCode,
                                _product.Reference,
                                _product.Description,
                                stock,
                                _product.Pvp1 > 0 ? "€" + _product.Pvp1 : "-",
                                _product.Pvp2 > 0 ? "€" + _product.Pvp2 : "-",
                                zsHandler.GetSupplierNameWithCode(Convert.ToInt32(_product.Supplier)),
                                Convert.ToDecimal(_product.Pcu) > 0 ? "€" + _product.Pcu : "-",
                                "-");
                        }
                        else
                        {
                            if (_manager.HasInternet() && _zsClient.IsAutenticated)
                                Toast.MakeText(this, "Produto não encontrado", ToastLength.Long).Show();
                            else if (!_manager.HasInternet())
                                Toast.MakeText(this, "Verifique a sua conexão", ToastLength.Long).Show();
                            else
                                Toast.MakeText(this, "Servidor offline", ToastLength.Long).Show();
                            ClearFields("barCode");
                        }
                    }
                    else
                    {
                        dialogFragment.Dismiss();
                        if (_product != null)
                        {
                            UpdateFields(_product.BarCode,
                                _product.ProductCode,
                                _product.Reference,
                                _product.Description,
                                _stock + " un.",
                                _product.Pvp1 > 0 ? "€ " + _product.Pvp1 : "-",
                                _product.Pvp2 > 0 ? "€ " + _product.Pvp2 : "-",
                                _product.Supplier.ToString(),
                                Convert.ToDouble(_product.Pcu) > 0 ? "€ " + _product.Pcu : "-",
                                Convert.ToDouble(_product.Pcm) > 0 ? "€ " + _product.Pcm : "-");
                        }
                        else
                        {
                            if (_manager.HasInternet() && _etiClient.IsOnline)
                                Toast.MakeText(this, "Produto não encontrado", ToastLength.Long).Show();
                            else if (!_manager.HasInternet())
                                Toast.MakeText(this, "Verifique a sua conexão", ToastLength.Long).Show();
                            else
                                Toast.MakeText(this, "Servidor offline", ToastLength.Long).Show();
                            ClearFields("barCode");
                        }
                    }
                    _searchComplete = true;
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
                        ListStocks.Add(new AddStockStore(x.ToString(), x.ToString()));
                    try { stock = Stock.Sum(pos => pos.Value).ToString(); } catch (Exception) { stock = "0"; }
                    dialogFragment.Dismiss();

                    UpdateFields(_product.BarCode,
                                  _product.ProductCode,
                                  _product.Reference,
                                  _product.Description,
                                  stock,
                                  _product.Pvp1 > 0 ? "€" + _product.Pvp1 : "-",
                                  _product.Pvp2 > 0 ? "€" + _product.Pvp2 : "-",
                                  zsHandler.GetSupplierNameWithCode(Convert.ToInt32(_product.Supplier)),
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
                        ListStocks.Add(new AddStockStore(x.ToString(), x.ToString()));
                    try { stock = Stock.Sum(pos => pos.Value).ToString(); } catch (Exception) { stock = "0"; }
                    dialogFragment.Dismiss();
                    UpdateFields(_product.BarCode,
                                 _product.ProductCode,
                                 _product.Reference,
                                 _product.Description,
                                 stock,
                                 _product.Pvp1 > 0 ? "€" + _product.Pvp1 : "-",
                                 _product.Pvp2 > 0 ? "€" + _product.Pvp2 : "-",
                                 zsHandler.GetSupplierNameWithCode(Convert.ToInt32(_product.Supplier)),
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
            menu.Add(new String("Configurações"));
            menu.Add(new String("Sobre"));
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