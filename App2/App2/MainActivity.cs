using System;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Media;
using Android.Widget;
using Android.OS;
using Android.Support.V7.App;
using App2.Modal;
using ZSProduct;
using AlertDialog = Android.App.AlertDialog;

//Add Features
namespace App2
{
	[Activity (Label = "ZSProduct_V2_5", Icon = "@drawable/icon", MainLauncher = true, Theme = "@style/Theme.AppCompat.Light.NoActionBar", ScreenOrientation = ScreenOrientation.Portrait)]
	public class MainActivity : AppCompatActivity
	{
		protected override void OnCreate (Bundle bundle)
		{   
			base.OnCreate (bundle);

			SetContentView (Resource.Layout.Main);

            var manager = new ZsManager ();
			//manager.preferences.Edit ().Clear ().Apply ();
			var getNif = manager.GetItem ("nif");
			var getUsername = manager.GetItem ("username");
			var getPassword = manager.GetItem ("password");
			//var getMode = manager.preferences.GetInt ("mode", 1);

			if (getUsername != string.Empty || getPassword != string.Empty || getNif != string.Empty)
				StartActivity (new Intent (this, typeof(Dashboard)));
			else {
				var btnLogin = FindViewById<Button> (Resource.Id.btnLogin);

				btnLogin.Click += (sender, e) => {
					var txtNif = FindViewById<EditText> (Resource.Id.txtNif);
					var txtUsename = FindViewById<EditText> (Resource.Id.txtUsername);
					var txtPassword = FindViewById<EditText> (Resource.Id.txtPassword);
					var nif = txtNif.Text.ToString ();
					var username = txtUsename.Text;
					var password = txtPassword.Text;

					if (nif != "" && username != "" && password != "") {
						var saveData = new ZsManager ();
						saveData.AddItem (nif, "nif");
						saveData.AddItem (username, "username");
						saveData.AddItem (password, "password");
						saveData.AddItem (1, "mode");
						var zsHandler = new ZSClient (username, password, 0, nif);
						zsHandler.Login ();
						if (zsHandler.Login ()) {
							var dashboardActivity = new Intent (this, typeof(Dashboard));
							dashboardActivity.PutExtra ("nif", nif);
							dashboardActivity.PutExtra ("username", username);
							dashboardActivity.PutExtra ("password", password);
							StartActivity (dashboardActivity);
						} else {
							new AlertDialog.Builder (this)
                                .SetTitle ("Erro")
                                .SetMessage ("Dados Incorretos...")
                                .SetPositiveButton ("Ok", (EventHandler<DialogClickEventArgs>)null)
                                .Show ();
						}
					} else
						Toast.MakeText (this, "Preencha todos os campos!", ToastLength.Short).Show ();

				};

				var btnFill = FindViewById<Button> (Resource.Id.btnPreencher);
				btnFill.Click += (sender, e) => {
					var txtNif = FindViewById<EditText> (Resource.Id.txtNif);
					var txtUsename = FindViewById<EditText> (Resource.Id.txtUsername);
					var txtPassword = FindViewById<EditText> (Resource.Id.txtPassword);
					txtNif.Text = "509545700";
					txtUsename.Text = "hugo";
					txtPassword.Text = "hugo123";
				};
			}
		}
	}
}

