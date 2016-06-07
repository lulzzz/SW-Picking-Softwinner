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
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);

            var manager = new ZsManager();
            var getNif = manager.GetItem("nif");
            var getUsername = manager.GetItem("username");
            var getPassword = manager.GetItem("password");

            if (getUsername != string.Empty || getPassword != string.Empty || getNif != string.Empty)
                StartActivity(new Intent(this, typeof(Dashboard)));

            var txtNif = FindViewById<TextView>(Resource.Id.txtMainNif);
            var txtUsername = FindViewById<TextView>(Resource.Id.txtMainUsername);
            var txtPassword = FindViewById<TextView>(Resource.Id.txtMainPassword);
            var btnLogin = FindViewById<Button>(Resource.Id.btnMainLogin);
            var btnPreencher = FindViewById<Button>(Resource.Id.btnMainPreencher);

            btnLogin.Click += (sender, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtNif.Text) || string.IsNullOrWhiteSpace(txtUsername.Text) || string.IsNullOrWhiteSpace(txtPassword.Text))
                    Snackbar.Make(sender as View, "Preencha todos os campos...", Snackbar.LengthShort).Show();
                else
                {
                    var saveData = new ZsManager();
                    saveData.AddItem(txtNif.Text, "nif");
                    saveData.AddItem(txtUsername.Text, "username");
                    saveData.AddItem(txtPassword.Text, "password");
                    saveData.AddItem(0, "storeToPdt");
                    var zsHandler = new ZsClient(txtUsername.Text, txtPassword.Text, 0, txtNif.Text);
                    zsHandler.Login();
                    if (zsHandler.Login())
                    {
                        var dashboardActivity = new Intent(this, typeof(Dashboard));
                        dashboardActivity.PutExtra("nif", txtNif.Text);
                        dashboardActivity.PutExtra("username", txtUsername.Text);
                        dashboardActivity.PutExtra("password", txtPassword.Text);
                        StartActivity(dashboardActivity);
                    }
                    else
                        Snackbar.Make(sender as View, "Dados incorretos...", Snackbar.LengthShort).Show();
                }
            };

            btnPreencher.Click += (sender, e) =>
            {
                txtNif.Text = "509545700";
                txtUsername.Text = "hugo";
                txtPassword.Text = "hugo123";
            };
        }
    }
}

