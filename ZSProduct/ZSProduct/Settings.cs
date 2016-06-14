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
    [Activity(Label = "Configurações", Theme = "@style/Theme.DesignDemo", ScreenOrientation = ScreenOrientation.Portrait)]
    public class Settings : AppCompatActivity
    {
        //-----------------------------------------------------------
        private readonly ZsManager _manager = new ZsManager();
        private ImageView _imgSettingsLoginType;
        private TextView _txtSettingsSessionInfo;
        private TextView _lblSettingsSignOut;
        private EditText _txtSettingsEmailCsv;
        private CheckBox _chkSettingsCsvToEmail;

        //-----------------------------------------------------------
        protected override void OnCreate(Bundle savedInstanceState)
        {
            //-----------------------------------------------------------
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.newSettings);

            //-----------------------------------------------------------
            var toolBar = FindViewById<SupportToolbar>(Resource.Id.toolBar);
            SetSupportActionBar(toolBar);
            var ab = SupportActionBar;
            ab.SetHomeAsUpIndicator(Resource.Drawable.ic_arrow_back_white_18dp);
            ab.SetDisplayHomeAsUpEnabled(true);

            //-----------------------------------------------------------
            _imgSettingsLoginType = FindViewById<ImageView>(Resource.Id.imgSettingsLoginType);
            _txtSettingsSessionInfo = FindViewById<TextView>(Resource.Id.txtSettingsSessionInfo);
            _lblSettingsSignOut = FindViewById<TextView>(Resource.Id.lblSettingsSignOut);
            _txtSettingsEmailCsv = FindViewById<EditText>(Resource.Id.txtSettingsEmailCsv);
            _chkSettingsCsvToEmail = FindViewById<CheckBox>(Resource.Id.chkSettingsCsvToEmail);

            //-----------------------------------------------------------
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