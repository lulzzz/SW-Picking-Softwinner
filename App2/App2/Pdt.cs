using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using App2.Modal;
using ZSProduct;
using System.IO;
using System.Text;
using Java.IO;
using Console = System.Console;
using File = System.IO.File;
using Thread = System.Threading.Thread;

//-----------------------------------------------------------
namespace App2
{
    [Activity(Label = "Contagem de Stock ", Theme = "@style/Theme.AppCompat.Light.NoActionBar", ScreenOrientation = ScreenOrientation.Portrait)]
    public class Pdt : AppCompatActivity
    {
        //-----------------------------------------------------------
        public int Qtd { get; set; }
        private readonly List<AddProducttoListView> _cenas = new List<AddProducttoListView>();
        private AdapterListView _view;
        private int _clicked;

        //-----------------------------------------------------------
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Pdt);
            _view = new AdapterListView(this, _cenas);
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
                //var dataToFile = _cenas.Aggregate("", (current, item) => current + item.description + "," + item.qtd + "\n");
                //SendEmail();
                //FindViewById<ProgressBar>(Resource.Id.progressBar1).Visibility = ViewStates.Invisible;
                SaveData();
                Toast.MakeText(this, "Salvo", ToastLength.Short).Show();
            };
        }

        public void SaveData()
        {
            var downloadsFolder = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads);
            //var downloadsFolder = "/sdcard/emulated/0/";
            var filePath = Path.Combine(downloadsFolder.Path, "myfile.txt");

            using (var streamWriter = new StreamWriter(filePath, false))
            {
                //streamWriter.WriteLine(DateTime.UtcNow);
                foreach (var item in _cenas)
                {
                    streamWriter.WriteLine(item.description + ";" + item.qtd);
                }
                streamWriter.Close();
            }
        }

        //-----------------------------------------------------------
        private void DeleteProductofList()
        {
            RunOnUiThread(() =>
            {
                Console.WriteLine("Delete: " + _clicked);
                _cenas.RemoveAt(_clicked);
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
                Console.WriteLine("AddItem started");
                var manager = new ZsManager();
                var getNif = manager.GetItem("nif");
                var getUsername = manager.GetItem("username");
                var getPassword = manager.GetItem("password");
                var txtPdtCodBarras = FindViewById<EditText>(Resource.Id.txtPdtCodBarras);
                var zsHandler = new ZSClient(getUsername, getPassword, 0, getNif);
                zsHandler.Login();
                var existe = false;
                foreach (var item in _cenas)
                    if (item.barCode == txtPdtCodBarras.Text)
                        existe = true;

                if (!existe)
                {
                    var product = zsHandler.GetProductWithBarCode(txtPdtCodBarras.Text);
                    if (product != null)
                    {
                        Console.WriteLine("oaef: " + Qtd);
                        _cenas.Add(new AddProducttoListView(product.description, product.productCode, product.store, product.stock, product.barCode, product.pvp1, product.pvp2, product.reference, product.pcu, Qtd));
                        FindViewById<ListView>(Resource.Id.listView1).Adapter = _view;
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
                    Toast.MakeText(this, "Produto j� adicionado", ToastLength.Long).Show();
                    txtPdtCodBarras.Text = "";
                }
            });
        }

        public async void SendEmail()
        {
            var downloadsFolder = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads);
            var filePath = Path.Combine(downloadsFolder.Path, "myfile.txt");

            var createdFile = File.CreateText(filePath);
            await createdFile.WriteAsync("Hello World!");
            await createdFile.FlushAsync();

            var file = new Java.IO.File(filePath);
            file.SetReadable(true, false);

            var uri = Android.Net.Uri.FromFile(file);

            var email = new Intent(Intent.ActionSend);
            email.PutExtra(Intent.ExtraEmail, new[] { "andrefilsantos@gmail.com" });
            email.PutExtra(Intent.ExtraSubject, "Sample email with attachment");
            email.PutExtra(Intent.ExtraStream, uri);

            email.SetType("message/rfc822");

            StartActivity(Intent.CreateChooser(email, "Email"));
        }
    }
}