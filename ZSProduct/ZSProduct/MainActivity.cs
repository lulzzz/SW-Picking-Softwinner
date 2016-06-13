using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using ZSProduct.Modal;

namespace ZSProduct
{
    [Activity(Label = "ZSProduct", MainLauncher = true, Icon = "@drawable/icon", Theme = "@style/Theme.DesignDemo", ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : AppCompatActivity
    {
        private string _getNif;
        string _getUsername;
        string _getPassword;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Login);

            var btnLogin = FindViewById<Button>(Resource.Id.btnLoginLogin);
           // var btnPreencher = FindViewById<Button>(Resource.Id.btnLoginPreencher);
            var item1 = FindViewById<LinearLayout>(Resource.Id.item1);
            var item2 = FindViewById<LinearLayout>(Resource.Id.item2);
            var optionsItem1 = FindViewById<LinearLayout>(Resource.Id.optionsItem1);
            var optionsItem2 = FindViewById<LinearLayout>(Resource.Id.optionsItem2);
            var txtZsNif = FindViewById<TextView>(Resource.Id.txtLoginZSNif);
            var txtZsUsername = FindViewById<TextView>(Resource.Id.txtLoginZSUsername);
            var txtZsPassword = FindViewById<TextView>(Resource.Id.txtLoginZSPassword);
            var txtEtiIp = FindViewById<TextView>(Resource.Id.txtLoginEtiIP);
            var txtEtiUsername = FindViewById<TextView>(Resource.Id.txtLoginEtiUsername);
            var txtEtiPassword = FindViewById<TextView>(Resource.Id.txtLoginEtiPassword);
            var txtEtiPort = FindViewById<TextView>(Resource.Id.txtLoginEtiPort);
            var manager = new ZsManager();
            var optItem1Open = false;
            var optItem2Open = false;

            _getNif = manager.GetItem("nif");
            _getUsername = manager.GetItem("username");
            _getPassword = manager.GetItem("password");

            CheckLogin();

            optionsItem1.Visibility = ViewStates.Gone;
            optionsItem2.Visibility = ViewStates.Gone;

            item1.Click += (sender, args) =>
            {
                if (optItem2Open)
                {
                    optionsItem2.Visibility = ViewStates.Gone;
                    optItem2Open = false;
                }
                optionsItem1.Visibility = ViewStates.Visible;
                optItem1Open = true;
                //Matrix matrix = new Matrix();
                //img.SetScaleType(ImageView.ScaleType.Matrix);
                //matrix.PostRotate(45, 20, 20);
                //img.ImageMatrix = matrix;

            };

            item2.Click += (sender, args) =>
            {
                if (optItem1Open)
                {
                    optionsItem1.Visibility = ViewStates.Gone;
                    optItem1Open = false;
                }
                optionsItem2.Visibility = ViewStates.Visible;
                optItem2Open = true;
            };

            btnLogin.Click += (sender, e) =>
            {
                if (optItem1Open)
                {
                    if (string.IsNullOrWhiteSpace(txtZsNif.Text) || string.IsNullOrWhiteSpace(txtZsUsername.Text) || string.IsNullOrWhiteSpace(txtZsPassword.Text))
                        Snackbar.Make(sender as View, "Preencha todos os campos...", Snackbar.LengthShort).Show();
                    else
                    {
                        var zsHandler = new ZsClient(txtZsUsername.Text, txtZsPassword.Text, 0, txtZsNif.Text);
                        zsHandler.Login();
                        if (zsHandler.Login())
                        {
                            StartActivity(typeof(Dashboard));
                            var saveData = new ZsManager();
                            saveData.AddItem("zonesoft", "loginType");
                            saveData.AddItem(txtZsNif.Text, "nif");
                            saveData.AddItem(txtZsUsername.Text, "username");
                            saveData.AddItem(txtZsPassword.Text, "password");
                            saveData.AddItem(0, "storeToPdt");
                        }
                        else
                            Snackbar.Make(sender as View, "Dados incorretos...", Snackbar.LengthShort).Show();
                    }
                }
                else if (optItem2Open)
                {
                    if (string.IsNullOrWhiteSpace(txtEtiIp.Text) || string.IsNullOrWhiteSpace(txtEtiUsername.Text) || string.IsNullOrWhiteSpace(txtEtiPassword.Text) || string.IsNullOrEmpty(txtEtiPort.Text))
                        Snackbar.Make(sender as View, "Preencha todos os campos...", Snackbar.LengthShort).Show();
                    else
                    {
                        var etiHandler = new EtiClient(txtEtiUsername.Text, txtEtiPassword.Text, txtEtiIp.Text);
                        etiHandler.Login();
                        if (etiHandler.Login() == 1)
                        {
                            StartActivity(typeof(Dashboard));
                            var saveData = new ZsManager();
                            saveData.AddItem("eticadata", "loginType");
                            saveData.AddItem(txtEtiIp.Text, "ip");
                            saveData.AddItem(txtEtiUsername.Text, "username");
                            saveData.AddItem(txtEtiPassword.Text, "password");
                            saveData.AddItem(txtEtiPort.Text, "port");
                            saveData.AddItem(0, "storeToPdt");
                        }
                        else if (etiHandler.Login() == 0)
                            Snackbar.Make(sender as View, "Dados incorretos...", Snackbar.LengthShort).Show();
                        else if (etiHandler.Login() == -1)
                            Snackbar.Make(sender as View, "Servidor offline...", Snackbar.LengthShort).Show();
                    }
                }
            };
        }

        private void CheckLogin()
        {
            if (_getUsername != string.Empty || _getPassword != string.Empty || _getNif != string.Empty)
                StartActivity(new Intent(this, typeof(Dashboard)));
        }

        protected override void OnResume()
        {
            base.OnResume();
            CheckLogin();
        }
    }
}

