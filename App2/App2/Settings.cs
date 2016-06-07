using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Support.V7.App;
using Android.Widget;
using App2.Modal;
using Java.Security;

namespace App2
{
	[Activity (Label = "Configurações", Theme = "@style/Theme.AppCompat.Light", ScreenOrientation = ScreenOrientation.Portrait)]
	public class Settings : AppCompatActivity
	{

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			SetContentView (Resource.Layout.Settings);
			var manager = new ZsManager ();
			var btnLogout = FindViewById<Button> (Resource.Id.btnEndSession);
			var saveSettings = FindViewById<Button> (Resource.Id.btnSave);
			var txtEmail = FindViewById<TextView> (Resource.Id.txtSettingsEmail);

			//Eticadata Settings

			var serverAddress = FindViewById<EditText> (Resource.Id.serverAddress);
			var username = FindViewById<EditText> (Resource.Id.usernameAPI);
			var password = FindViewById<EditText> (Resource.Id.passwordAPI);
			ToggleButton eticadataIntegratorSwitch = FindViewById<ToggleButton> (Resource.Id.integratorSwith);
			var layoutSettings = FindViewById<LinearLayout> (Resource.Id.linearLayoutEticadata);


			if (manager.GetItem ("eticadataIntegration") == "0") {
				//0 means the integration is OFF
				for (int i = 0; i < layoutSettings.ChildCount; i++) {
					var view = layoutSettings.GetChildAt (i);
					view.Enabled = (false); // Or whatever you want to do with the view.
				}
			} else {
				//1 means the integration is ON
				eticadataIntegratorSwitch.Checked = true;
				serverAddress.Text = manager.GetItem ("sqlServerAdress");
				username.Text = manager.GetItem ("eticadataUsernameAPI");
				password.Text = manager.GetItem ("eticadataPasswordAPI");
				//layoutSettings.Enabled = true;
				for (int i = 0; i < layoutSettings.ChildCount; i++) {
					var view = layoutSettings.GetChildAt (i);
					view.Enabled = (true); // Or whatever you want to do with the view.
				}
			}


			if (manager.HasEmail ())
				txtEmail.Text = manager.GetItem ("emailToCSV");


			//************************************Events handling*****************************************
			eticadataIntegratorSwitch.Click += (object sender, System.EventArgs e) => {
				ToggleButton button = (ToggleButton)sender;
				System.Console.WriteLine (button.Checked);
				if (button.Checked) {

					for (int i = 0; i < layoutSettings.ChildCount; i++) {
						var view = layoutSettings.GetChildAt (i);
						view.Enabled = (true); // Or whatever you want to do with the view.
					}
				} else {
					manager.AddItem ("0", "eticadataIntegration");
					for (int i = 0; i < layoutSettings.ChildCount; i++) {
						var view = layoutSettings.GetChildAt (i);
						view.Enabled = (false); // Or whatever you want to do with the view.
					}
				}

			};
			saveSettings.Click += (sender, e) => {
				//Check the required parametersfor eticadata integration
				if (eticadataIntegratorSwitch.Checked && serverAddress.Text.Length > 0 && username.Text.Length > 0 && password.Text.Length > 0) {
					manager.AddItem ("1", "eticadataIntegration");
					manager.AddItem (serverAddress.Text, "sqlServerAdress");
					manager.AddItem (username.Text, "eticadataUsernameAPI");
					manager.AddItem (password.Text, "eticadataPasswordAPI");
					manager.AddItem (txtEmail.Text, "emailToCSV");
					Toast.MakeText (this, "Integracao com sucesso", ToastLength.Short).Show ();
					StartActivity (typeof(MainActivity));
				} 
				if ((eticadataIntegratorSwitch.Checked) && (serverAddress.Text.Length <= 0 || username.Text.Length <= 0 || password.Text.Length <= 0)) {
					Toast.MakeText (this, "Faltam parametros para activar a integracao com o Eticadata !!!", ToastLength.Short).Show ();
				} else {
					manager.AddItem (txtEmail.Text, "emailToCSV");
					StartActivity (typeof(MainActivity));
				}

			};


			btnLogout.Click += (sender, e) => {
				var pref = Application.Context.GetSharedPreferences ("UserInfo", FileCreationMode.Private);
				var edit = pref.Edit ();
				edit.Clear ();
				edit.Apply ();
				Toast.MakeText (this, "Sess?o terminada com sucesso!", ToastLength.Short).Show ();
				StartActivity (typeof(MainActivity));
			};
		}
		//public override void Finish()
		//{
		//    Toast.MakeText(this, "Fechar!", ToastLength.Short).Show();
		//}

	}
}