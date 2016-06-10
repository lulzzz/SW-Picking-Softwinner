using System;
using System.Collections.Generic;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Android.Content;
using Android.Support.Design.Widget;
using ZSProduct.Modal;
using System.IO;
using Console = System.Console;
using Thread = System.Threading.Thread;
using SupportToolbar = Android.Support.V7.Widget.Toolbar;

namespace ZSProduct
{
    [Activity(Label = "PDT", Theme = "@style/Theme.DesignDemo", ScreenOrientation = ScreenOrientation.Portrait)]
    public class Pdt : AppCompatActivity
    {
        //-----------------------------------------------------------
        public int Qtd { get; set; }
        private readonly List<AddProducttoListView> _productList = new List<AddProducttoListView>();
        private AdapterListView _view;
        private int _clicked;


        private readonly ZsManager _manager = new ZsManager(Application.Context);

        //-----------------------------------------------------------
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Pdt);
            var toolBar = FindViewById<SupportToolbar>(Resource.Id.toolBar);
            SetSupportActionBar(toolBar);
            var ab = SupportActionBar;
            ab.SetHomeAsUpIndicator(Resource.Drawable.ic_arrow_back_white_18dp);
            ab.SetDisplayHomeAsUpEnabled(true);

            _view = new AdapterListView(this, _productList);
            FindViewById<EditText>(Resource.Id.txtPdtBarCode).KeyPress += (sender, e) =>
            {
                e.Handled = false;
                if (e.Event.Action != KeyEventActions.Down || e.KeyCode != Keycode.Enter) return;
                OpenDialog("add");
                e.Handled = true;
            };

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

            FindViewById<FloatingActionButton>(Resource.Id.fabPdt).Click += (sender, args) =>
            {
                if (_productList.Count <= 0) { Toast.MakeText(this, "A lista encontra-se vazia...", ToastLength.Short); return; }
                _manager.SaveData(_productList);
                Toast.MakeText(this, "Exportado com sucesso!", ToastLength.Short).Show();
                if (_manager.HasEmail())
                    SendEmail(Path.Combine(Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads).Path, "export.csv"));
                else
                    Toast.MakeText(this, "Guardado em " + Path.Combine(Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads).Path, "export.csv"), ToastLength.Short).Show();
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
            email.PutExtra(Intent.ExtraSubject, "Your CSV file.");
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
            var transaction = FragmentManager.BeginTransaction();
            var dialogFragment = new DialogQtdPdt();
            dialogFragment.Show(transaction, "dialog_fragment");
            dialogFragment.OnChangedComplete += (sender, e) =>
            {
                Qtd = Convert.ToInt32(e.QtdSeted);
                var t = from == "add" ? new Thread(AddItem) : new Thread(EditItem);
                t.Start();
            };

        }

        //-----------------------------------------------------------
        public void EditItem()
        {
            RunOnUiThread(() =>
            {
                _productList[_clicked].qtd = Qtd;
                FindViewById<ListView>(Resource.Id.lstPdtProducts).Adapter = _view;
            });
        }

        //-----------------------------------------------------------
        public void AddItem()
        {
            RunOnUiThread(() =>
            {
                var getNif = _manager.GetItem("nif");
                var getUsername = _manager.GetItem("username");
                var getPassword = _manager.GetItem("password");
                var getWarehouse = _manager.GetItem("storeToPdt");
                Toast.MakeText(this, "bla: " + getWarehouse, ToastLength.Short).Show();
                var zsHandler = new ZsClient(getUsername, getPassword, Convert.ToInt32(getWarehouse), getNif);
                zsHandler.Login();
                Console.WriteLine("AddItem started " + zsHandler.StoreCount());
                var txtPdtCodBarras = FindViewById<EditText>(Resource.Id.txtPdtBarCode);
                var barCode = txtPdtCodBarras.Text;
                var existe = false;
                foreach (var item in _productList)
                    if (item.BarCode == txtPdtCodBarras.Text)
                        existe = true;
                zsHandler.Login();
                if (!existe)
                {
                    var product = zsHandler.GetProductWithBarCode(barCode);
                    if (product != null)
                    {
                        _productList.Add(new AddProducttoListView(product.Description, product.ProductCode,
                            product.Store, product.Stock, product.BarCode, product.Pvp1, product.Pvp2,
                            product.Reference, product.SupplierId, product.Pcu, Qtd));
                        FindViewById<ListView>(Resource.Id.lstPdtProducts).Adapter = _view;
                        txtPdtCodBarras.Text = "";
                    }
                    else
                    {
                        Toast.MakeText(this, "Produto inexistente", ToastLength.Long).Show();
                        txtPdtCodBarras.Text = "";
                    }
                }
                else
                {
                    Toast.MakeText(this, "Produto já adicionado", ToastLength.Long).Show();
                    txtPdtCodBarras.Text = "";
                }
            });
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