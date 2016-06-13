using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using ZSProduct.Modal;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ZSProduct
{
    [Activity(Label = "Configura��es", Theme = "@style/Theme.AppCompat.Light", ScreenOrientation = ScreenOrientation.Portrait)]
    public class Settings : AppCompatActivity
    {
        private readonly ZsManager _manager = new ZsManager();
        private ZsClient _zsClient;
        private int _selectedStore;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Settings);
            var manager = new ZsManager();
            var btnLogout = FindViewById<Button>(Resource.Id.btnSettingsEndSession);
            var saveSettings = FindViewById<Button>(Resource.Id.btnSettingsSave);
            var txtEmail = FindViewById<TextView>(Resource.Id.txtSettingsEmail);
            BuildSpinner();
            //Eticadata Settings

            var serverAddress = FindViewById<EditText>(Resource.Id.txtSettingsServerAddress);
            var username = FindViewById<EditText>(Resource.Id.txtSettingsUsernameAPI);
            var password = FindViewById<EditText>(Resource.Id.txtSettingsPasswordAPI);
            var eticadataIntegratorSwitch = FindViewById<ToggleButton>(Resource.Id.tbtnSettingsIntegratorSwith);
            var layoutSettings = FindViewById<LinearLayout>(Resource.Id.linearLayoutEticadata);


            if (manager.GetItem("eticadataIntegration") == "0")
            {
                //0 means the integration is OFF
                for (var i = 0; i < layoutSettings.ChildCount; i++)
                {
                    var view = layoutSettings.GetChildAt(i);
                    view.Enabled = (false); // Or whatever you want to do with the view.
                }
            }
            else
            {
                //1 means the integration is ON
                eticadataIntegratorSwitch.Checked = true;
                serverAddress.Text = manager.GetItem("sqlServerAdress");
                username.Text = manager.GetItem("eticadataUsernameAPI");
                password.Text = manager.GetItem("eticadataPasswordAPI");
                //layoutSettings.Enabled = true;
                for (var i = 0; i < layoutSettings.ChildCount; i++)
                {
                    var view = layoutSettings.GetChildAt(i);
                    view.Enabled = true; // Or whatever you want to do with the view.
                }
            }


            if (manager.HasEmail())
                txtEmail.Text = manager.GetItem("emailToCSV");


            //************************************Events handling*****************************************
            eticadataIntegratorSwitch.Click += (sender, e) =>
            {
                var button = (ToggleButton)sender;
                Console.WriteLine(button.Checked);
                if (button.Checked)
                {

                    for (int i = 0; i < layoutSettings.ChildCount; i++)
                    {
                        var view = layoutSettings.GetChildAt(i);
                        view.Enabled = (true); // Or whatever you want to do with the view.
                    }
                }
                else
                {
                    manager.AddItem("0", "eticadataIntegration");
                    for (int i = 0; i < layoutSettings.ChildCount; i++)
                    {
                        var view = layoutSettings.GetChildAt(i);
                        view.Enabled = (false); // Or whatever you want to do with the view.
                    }
                }

            };
            saveSettings.Click += (sender, e) =>
            {
                //Check the required parametersfor eticadata integration
                if (eticadataIntegratorSwitch.Checked && serverAddress.Text.Length > 0 && username.Text.Length > 0 && password.Text.Length > 0)
                {
                    manager.AddItem("1", "eticadataIntegration");
                    manager.AddItem(serverAddress.Text, "sqlServerAdress");
                    manager.AddItem(username.Text, "eticadataUsernameAPI");
                    manager.AddItem(password.Text, "eticadataPasswordAPI");
                    manager.AddItem(txtEmail.Text, "emailToCSV");
                    manager.AddItem(manager.HasEticadataIntegration ? _selectedStore--.ToString() : _selectedStore.ToString(), "storeToPdt");
                    Toast.MakeText(this, "Integra��o com sucesso", ToastLength.Short).Show();
                    StartActivity(typeof(MainActivity));
                }
                if (eticadataIntegratorSwitch.Checked && (serverAddress.Text.Length <= 0 || username.Text.Length <= 0 || password.Text.Length <= 0))
                {
                    Toast.MakeText(this, "Faltam parametros para activar a integracao com o Eticadata !!!", ToastLength.Short).Show();
                }
                else
                {
                    manager.AddItem(txtEmail.Text, "emailToCSV");
                    StartActivity(typeof(MainActivity));
                }

            };

            btnLogout.Click += (sender, e) =>
            {
                var pref = Application.Context.GetSharedPreferences("UserInfo", FileCreationMode.Private);
                var edit = pref.Edit();
                edit.Clear();
                edit.Apply();
                Toast.MakeText(this, "Sess�o terminada com sucesso!", ToastLength.Short).Show();
                StartActivity(typeof(MainActivity));
            };
        }

        private void BuildSpinner()
        {
            _zsClient = new ZsClient(_manager.GetItem("username"), _manager.GetItem("password"), 0, _manager.GetItem("nif"));
            _zsClient.Login();
            var stores = _zsClient.GetStoresList();
            var items = new List<string>();
            //Chech if ZSManager has eticadata integration
            if (_manager.HasEticadataIntegration)
                items.Add("Servidor Eticadata");
            items.AddRange(stores.Select(store => store.Description));

            var adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerDropDownItem, items);
            var spinner = FindViewById<Spinner>(Resource.Id.optProdFinderStores);
            spinner.Adapter = adapter;
            spinner.ItemSelected += spinner_ItemSelected;
        }

        //Handler for the drop down list
        private void spinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            var spinner = (Spinner)sender;
            _selectedStore = e.Position;
            Toast.MakeText(this, $"Loja  {spinner.GetItemAtPosition(_selectedStore)} selecionada", ToastLength.Long).Show();

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