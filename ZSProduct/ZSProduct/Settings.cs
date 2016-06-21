using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using ZSProduct.Modal;
using SupportToolbar = Android.Support.V7.Widget.Toolbar;

//-----------------------------------------------------------
namespace ZSProduct
{
    [Activity(Label = "@string/settingsLabel", Theme = "@style/Theme.DesignDemo", ScreenOrientation = ScreenOrientation.Portrait)]
    public class Settings : AppCompatActivity
    {
        //-----------------------------------------------------------
        private readonly ZsManager _manager = new ZsManager();
        private ZsClient _zsClient;
        private ImageView _imgSettingsLoginType;
        private TextView _txtSettingsSessionInfo;
        private TextView _lblSettingsSignOut;
        private EditText _txtSettingsEmailCsv;
        private CheckBox _chkSettingsCsvToEmail;
        private int _selectedStore;
        private BackgroundWorker _mWorker;
        private List<string> _items;

        //-----------------------------------------------------------
        protected override void OnCreate(Bundle savedInstanceState)
        {
            //-----------------------------------------------------------
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Settings);
            _mWorker = new BackgroundWorker();

            //-----------------------------------------------------------
            var toolBar = FindViewById<SupportToolbar>(Resource.Id.toolBar);
            SetSupportActionBar(toolBar);
            toolBar.SetNavigationIcon(Resource.Drawable.ic_arrow_back_white_18dp);
            toolBar.NavigationClick += (sender, args) => OnBackPressed();

            //-----------------------------------------------------------
            _imgSettingsLoginType = FindViewById<ImageView>(Resource.Id.imgSettingsLoginType);
            _txtSettingsSessionInfo = FindViewById<TextView>(Resource.Id.txtSettingsSessionInfo);
            _lblSettingsSignOut = FindViewById<TextView>(Resource.Id.lblSettingsSignOut);
            _txtSettingsEmailCsv = FindViewById<EditText>(Resource.Id.txtSettingsEmailCsv);
            _chkSettingsCsvToEmail = FindViewById<CheckBox>(Resource.Id.chkSettingsCsvToEmail);
            FindViewById<Spinner>(Resource.Id.optSettingsStores).Visibility = ViewStates.Gone;
            //-----------------------------------------------------------
            if (_manager.GetItem("loginType") != "zonesoft")
            {
                FindViewById<LinearLayout>(Resource.Id.lnlaSettingsChangeWarehouse).Visibility = ViewStates.Gone;
            }

            _mWorker.DoWork += (o, a) =>
            {
                if (_manager.GetItem("loginType") == "zonesoft")
                {
                    _zsClient = new ZsClient(_manager.GetItem("username"), _manager.GetItem("password"), 0,
                        _manager.GetItem("nif"));
                    _zsClient.Login();
                    var stores = _zsClient.GetStoresList();
                    _items = new List<string>();
                    _items.AddRange(stores.Select(store => store.Description));
                }
            };
            _mWorker.RunWorkerAsync();
            _mWorker.RunWorkerCompleted += (o, args) =>
            {
                if (_manager.GetItem("loginType") == "zonesoft")
                {
                    FindViewById<LinearLayout>(Resource.Id.lnlaLoadStoreList).Visibility = ViewStates.Gone;
                    FindViewById<Spinner>(Resource.Id.optSettingsStores).Visibility = ViewStates.Visible;
                    var adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerDropDownItem, _items);
                    var spinner = FindViewById<Spinner>(Resource.Id.optSettingsStores);
                    spinner.Adapter = adapter;
                    spinner.ItemSelected += spinner_ItemSelected;
                }
            };

            _chkSettingsCsvToEmail.Click += (sender, args) =>
        {
            _txtSettingsEmailCsv.Visibility = _chkSettingsCsvToEmail.Checked ? ViewStates.Visible : ViewStates.Gone;
        };

            _lblSettingsSignOut.Click += (sender, args) =>
            {
                var pref = Application.Context.GetSharedPreferences("UserInfo", FileCreationMode.Private);
                var edit = pref.Edit();
                edit.Clear();
                edit.Apply();
                Toast.MakeText(this, "Sessão terminada com sucesso!", ToastLength.Short).Show();
                StartActivity(typeof(MainActivity));
            };

            //-----------------------------------------------------------
            if (_manager.HasEmail())
                _txtSettingsEmailCsv.Text = _manager.GetItem("emailToCSV");

            if (_manager.GetItem("loginType") == "zonesoft")
            {
                _imgSettingsLoginType.SetImageResource(Resource.Drawable.logo_zonesoft);
                _txtSettingsSessionInfo.Text = "NIF: " + _manager.GetItem("nif") +
                                               "\nUsername: " + _manager.GetItem("username") + "\n";
            }
            else
            {
                _imgSettingsLoginType.SetImageResource(Resource.Drawable.logo_eticadata);
                _txtSettingsSessionInfo.Text = "Servidor: " + _manager.GetItem("ip") +
                                               "\nUsername: " + _manager.GetItem("username") +
                                               "\nPorta: " + _manager.GetItem("port") + "\n";
            }
        }

        public override void OnBackPressed()
        {
            var a = _selectedStore;
            _manager.AddItem(_selectedStore.ToString(), "storeToPdt");
            if (!_chkSettingsCsvToEmail.Checked)
                base.OnBackPressed();
            else if (_chkSettingsCsvToEmail.Checked && string.IsNullOrEmpty(_txtSettingsEmailCsv.Text))
                Toast.MakeText(this, "Por favor, preencha o e-mail..", ToastLength.Short).Show();
            else if (_chkSettingsCsvToEmail.Checked && !string.IsNullOrEmpty(_txtSettingsEmailCsv.Text))
            {
                Toast.MakeText(this, "Agora, quando exportar o ficheiro CSV, vai poder enviá-lo para o email " + _txtSettingsEmailCsv.Text + "!", ToastLength.Short).Show();
                _manager.AddItem(_txtSettingsEmailCsv.Text, "emailToCSV");
                base.OnBackPressed();
            }
        }

        //Handler for the drop down list
        private void spinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            //var spinner = (Spinner)sender;
            _selectedStore = e.Position;
            // Toast.MakeText(this, $"Loja  {spinner.GetItemAtPosition(_selectedStore)} selecionada", ToastLength.Long).Show();

        }


        //****************************************** MENU ***************************************\\
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            menu.Add(new Java.Lang.String("Sobre"));
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.TitleFormatted.ToString())
            {
                case "Sobre":
                    StartActivity(typeof(About));
                    break;
            }
            return true;
        }
    }
}