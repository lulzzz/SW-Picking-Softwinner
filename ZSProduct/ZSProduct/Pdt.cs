using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Android.Content;
using Android.Support.Design.Widget;
using Android.Views.InputMethods;
using ZSProduct.Modal;
using Thread = System.Threading.Thread;
using SupportToolbar = Android.Support.V7.Widget.Toolbar;

namespace ZSProduct
{
    [Activity(Label = "@string/pdtLabel", Theme = "@style/Theme.DesignDemo", ScreenOrientation = ScreenOrientation.Portrait)]
    public class Pdt : AppCompatActivity
    {
        //-----------------------------------------------------------
        public double Qtd { get; set; }

        //-----------------------------------------------------------
        private readonly List<AddProducttoListView> _productList = new List<AddProducttoListView>();
        private AdapterListView _view;
        private int _clicked;
        private readonly ZsManager _manager = new ZsManager();
        private EditText _txtPdtCodBarras;
        public DialogLoading DialogFragment;
        private BackgroundWorker _mWorker;
        private Product _product;

        //-----------------------------------------------------------
        protected override void OnCreate(Bundle savedInstanceState)
        {
            //-----------------------------------------------------------
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Pdt);

            //-----------------------------------------------------------
            var toolBar = FindViewById<SupportToolbar>(Resource.Id.toolBar);
            SetSupportActionBar(toolBar);
            toolBar.SetNavigationIcon(Resource.Drawable.ic_arrow_back_white_18dp);
            toolBar.NavigationClick += (sender, args) => OnBackPressed();

            //-----------------------------------------------------------
            _view = new AdapterListView(this, _productList);
            _txtPdtCodBarras = FindViewById<EditText>(Resource.Id.txtPdtBarCode);
            _mWorker = new BackgroundWorker();

            //-----------------------------------------------------------
            FindViewById<EditText>(Resource.Id.txtPdtBarCode).KeyPress += (sender, e) =>
            {
                e.Handled = false;
                if (e.Event.Action != KeyEventActions.Down || e.KeyCode != Keycode.Enter) return;
                OpenDialog("add");
                e.Handled = true;
            };

            //-----------------------------------------------------------
            _view.OnDeleteClick += (sender, e) =>
            {
                _clicked = e.Position;
                var t = new Thread(DeleteProductofList);
                t.Start();
            };

            _view.OnEditClick += (sender, e) =>
            {
                _clicked = e.Position;
                var t = new Thread(EditProductofList);
                t.Start();
            };

            //-----------------------------------------------------------

            _txtPdtCodBarras.Click += (sender, args) => _txtPdtCodBarras.Text = "";

            FindViewById<FloatingActionButton>(Resource.Id.fabPdt).Click += (sender, args) =>
            {
                if (_productList.Count <= 0) { Toast.MakeText(this, "A lista encontra-se vazia...", ToastLength.Short); return; }
                _manager.SaveData(_productList);
                Toast.MakeText(this, "Exportado com sucesso!", ToastLength.Short).Show();
                if (_manager.HasEmail())
                    SendEmail("/storage/emulated/0/SWPicking.imp");
                else
                    Toast.MakeText(this, "Guardado em " + "SWPicking/stock.imp", ToastLength.Short).Show();
            };
        }

        //-----------------------------------------------------------
        public void SendEmail(string filePath)
        {
            var file = new Java.IO.File(filePath);
            file.SetReadable(true, false);

            var uri = Android.Net.Uri.FromFile(file);

            var email = new Intent(Intent.ActionSend);
            email.PutExtra(Intent.ExtraEmail, new[] { _manager.GetItem("emailToCSV") });
            email.PutExtra(Intent.ExtraSubject, "[SW PICKING] Contagem de Stock.");
            email.PutExtra(Intent.ExtraText, "Contagem retirada no dia " + DateTime.Now.ToString(CultureInfo.InvariantCulture));
            email.PutExtra(Intent.ExtraStream, uri);

            email.SetType("message/rfc822");

            StartActivity(Intent.CreateChooser(email, "Email"));

        }

        //-----------------------------------------------------------
        private void DeleteProductofList()
        {
            RunOnUiThread(() =>
            {
                Console.WriteLine("Delete: " + _clicked);
                _productList.RemoveAt(_clicked);
                FindViewById<ListView>(Resource.Id.lstPdtProducts).Adapter = _view;
            });
        }

        //-----------------------------------------------------------
        private void EditProductofList()
        {
            RunOnUiThread(() =>
            {
                Console.WriteLine("Edit: " + _clicked);
                OpenDialog("edit");
            });
        }

        //-----------------------------------------------------------
        public void OpenDialog(string from)
        {
            if (from == "add")
            {
                var transaction = FragmentManager.BeginTransaction();
                DialogFragment = new DialogLoading();
                DialogFragment.Show(transaction, "dialog_fragment");
                new Thread(AddItem).Start();
            }
            else
                new Thread(EditItem).Start();
        }

        //-----------------------------------------------------------
        public void EditItem()
        {
            RunOnUiThread(() =>
            {
                var transaction = FragmentManager.BeginTransaction();
                var dialogFragment = new DialogQtdPdt(_productList[_clicked].Qtd);
                dialogFragment.Show(transaction, "dialog_fragment");
                dialogFragment.OnChangedComplete += (sender, e) =>
                {
                    Qtd = Convert.ToDouble(e.QtdSeted.ToString().Replace(".", ","));
                    DialogFragment.Dismiss();
                    _productList[_clicked].Qtd = Qtd;
                    FindViewById<ListView>(Resource.Id.lstPdtProducts).Adapter = _view;
                };
            });
        }

        //-----------------------------------------------------------
        public void AddItem()
        {
            RunOnUiThread(() =>
            {
                if (_manager.GetItem("loginType") == "zonesoft")
                {
                    _mWorker.DoWork += (a, b) =>
                    {
                        var getNif = _manager.GetItem("nif");
                        var getUsername = _manager.GetItem("username");
                        var getPassword = _manager.GetItem("password");
                        var getWarehouse = _manager.GetItem("storeToPdt");
                        var zsHandler = new ZsClient(getUsername, getPassword, Convert.ToInt32(getWarehouse), getNif);
                        zsHandler.Login();
                        Console.WriteLine("AddItem started " + zsHandler.StoreCount());
                        var barCode = _txtPdtCodBarras.Text;
                        _product = zsHandler.GetProductWithBarCode(barCode);
                    };
                }
                else
                {
                    _mWorker.DoWork += (a, b) =>
                    {
                        var etiHandler = new EtiClient(_manager.GetItem("username"), _manager.GetItem("password"),
                            _manager.GetItem("ip"), Convert.ToUInt32(_manager.GetItem("port")));
                        etiHandler.Login();
                        var barCode = _txtPdtCodBarras.Text;
                        _product = etiHandler.GetProductWithBarCode(barCode);
                    };
                }
                _mWorker.RunWorkerAsync();

                var done = false;
                _mWorker.RunWorkerCompleted += (o, args) =>
                {
                    var existe = false;
                    foreach (var item in _productList)
                        if (item.BarCode == _txtPdtCodBarras.Text)
                            existe = true;
                    if (done) return;
                    if (!existe)
                    {
                        if (_product != null)
                        {
                            DialogFragment.Dismiss();
                            var transaction = FragmentManager.BeginTransaction();
                            var dialogFragment = new DialogQtdPdt(1);
                            dialogFragment.Show(transaction, "dialog_fragment");
                            dialogFragment.OnChangedComplete += (sender, e) =>
                            {
                                try
                                {
                                    Qtd = Convert.ToDouble(e.QtdSeted.ToString().Replace(".", ","));
                                }
                                catch
                                {
                                    Qtd = 0;
                                }
                                if (Qtd > 0)
                                {
                                    _productList.Add(new AddProducttoListView(_product.Description, _product.ProductCode,
                                        _product.Stock, _product.BarCode, _product.Pvp1, _product.Pvp2,
                                        _product.Reference, _product.Supplier, _product.Pcu, Qtd));
                                    FindViewById<ListView>(Resource.Id.lstPdtProducts).Adapter = _view;
                                }
                                else
                                    Toast.MakeText(this, "Não é possivel adicionar um produto sem quantidade...", ToastLength.Short).Show();
                                done = true;
                            };
                            _txtPdtCodBarras.Text = "";
                            done = true;
                        }
                        else
                        {
                            DialogFragment.Dismiss();
                            Toast.MakeText(this, "Produto inexistente", ToastLength.Long).Show();
                            _txtPdtCodBarras.Text = "";
                        }
                    }
                    else
                    {
                        DialogFragment.Dismiss();
                        Toast.MakeText(this, "Produto já adicionado", ToastLength.Long).Show();
                        _txtPdtCodBarras.Text = "";
                    }
                };
            });
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