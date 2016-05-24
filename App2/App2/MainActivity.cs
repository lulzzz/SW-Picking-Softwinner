using System;
using Android.App;
using Android.Content;
using Android.Widget;
using Android.OS;
using Android.Support.V7.App;
using ZSProduct;
using AlertDialog = Android.App.AlertDialog;

namespace App2
{
    [Activity(Label = "ZSProduct_V2_5", MainLauncher = true, Icon = "@drawable/icon", Theme = "@style/Theme.AppCompat.Light.NoActionBar")]
    public class MainActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Main);

            var btnLogin = FindViewById<Button>(Resource.Id.btnLogin);

            btnLogin.Click += (sender, e) =>
            {
                var txtNif = FindViewById<EditText>(Resource.Id.txtNif);
                var txtUsename = FindViewById<EditText>(Resource.Id.txtUsername);
                var txtPassword = FindViewById<EditText>(Resource.Id.txtPassword);
                var nif = txtNif.Text.ToString();
                var username = txtUsename.Text;
                var password = txtPassword.Text;

                if (nif != "" && username != "" && password != "")
                {
                    var zsHandler = new ZSClient(username, password, 0, nif);
                    zsHandler.Login();
                    if (zsHandler.Login())
                    {
                        //var stockLoja1 = FindViewById<TextView>(Resource.Id.textView1);
                        Console.WriteLine(zsHandler.hash);
                        //stockLoja1.Text = stockLoja1.Text + zsHandler.hash;
                        var activity2 = new Intent(this, typeof(Dashboard));
                        activity2.PutExtra("nif", nif);
                        activity2.PutExtra("username", username);
                        activity2.PutExtra("password", password);
                        StartActivity(activity2);
                    }
                    else
                    {
                        new AlertDialog.Builder(this)
                            .SetTitle("Erro")
                            .SetMessage("Dados Incorretos...")
                            .SetPositiveButton("Ok", (EventHandler<DialogClickEventArgs>) null)
                            .Show();
                    }
                }
                else
                    Toast.MakeText(this, "Preencha todos os campos!", ToastLength.Short).Show();
            };

            var btnFill = FindViewById<Button>(Resource.Id.btnPreencher);
            btnFill.Click += (sender, e) =>
            {
                var txtNif = FindViewById<EditText>(Resource.Id.txtNif);
                var txtUsename = FindViewById<EditText>(Resource.Id.txtUsername);
                var txtPassword = FindViewById<EditText>(Resource.Id.txtPassword);
                txtNif.Text = "509545700";
                txtUsename.Text = "hugo";
                txtPassword.Text = "hugo123";
            };
        }
    }
}

