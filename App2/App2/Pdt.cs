using System;
using System.Collections.Generic;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using App2.Modal;
using ZSProduct;
using System.IO;
using Android.Content;
using Console = System.Console;
using Thread = System.Threading.Thread;

//-----------------------------------------------------------
namespace App2
{
    [Activity(Label = "Contagem de Stock ", Theme = "@style/Theme.AppCompat.Light.NoActionBar", ScreenOrientation = ScreenOrientation.Portrait)]
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
            _view = new AdapterListView(this, _productList);
            FindViewById<EditText>(Resource.Id.txtPdtCodBarras).KeyPress += (sender, e) =>
            {
                e.Handled = false;
                if (e.Event.Action != KeyEventActions.Down || e.KeyCode != Keycode.Enter) return;
                AddItemtoListView();
                e.Handled = true;
            };

            _view.OnDeleteClick += (sender, e) =>
            {
                _clicked = e.Position;
                var t = new Thread(DeleteProductofList);
                t.Start();
            };

            FindViewById<Button>(Resource.Id.btnPdtExportCsv).Click += (sender, args) =>
            {
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
                FindViewById<ListView>(Resource.Id.listView1).Adapter = _view;
            });
        }

        //-----------------------------------------------------------
        public void AddItemtoListView()
        {
            var transaction = FragmentManager.BeginTransaction();
            var dialogFragment = new DialogQtdPdt();
            dialogFragment.Show(transaction, "dialog_fragment");
            dialogFragment.OnChangedComplete += (sender, e) =>
            {
                Qtd = Convert.ToInt32(e.QtdSeted);
                var t = new Thread(AddItem);
                t.Start();
            };

        }

        //-----------------------------------------------------------
        public void AddItem()
        {
            RunOnUiThread(() =>
            {
                
                var getNif = _manager.GetItem("nif");
                var getUsername = _manager.GetItem("username");
                var getPassword = _manager.GetItem("password");
                var zsHandler = new ZSClient(getUsername, getPassword, 0, getNif);
                zsHandler.Login();
                Console.WriteLine("AddItem started " + zsHandler.StoreCount());
                var txtPdtCodBarras = FindViewById<EditText>(Resource.Id.txtPdtCodBarras);
                for (var i = 0; i < zsHandler.StoreCount(); i++)
                {
                    Console.WriteLine("FOR LOJA " + i);
                    zsHandler = new ZSClient(getUsername, getPassword, i, getNif);
                    zsHandler.Login();
                    var existe = false;
                    foreach (var item in _productList)
                        if (item.barCode == txtPdtCodBarras.Text)
                            existe = true;

                    if (!existe)
                    {
                        var product = zsHandler.GetProductWithBarCode(txtPdtCodBarras.Text);
                        if (product != null)
                        {
                            _productList.Add(new AddProducttoListView(product.description, product.productCode,
                                product.store, product.stock, product.barCode, product.pvp1, product.pvp2,
                                product.reference, product.pcu, Qtd));
                            FindViewById<ListView>(Resource.Id.listView1).Adapter = _view;
                            txtPdtCodBarras.Text = "";
                            break;
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
                        break;
                    }
                }
            });
        }
    }
}