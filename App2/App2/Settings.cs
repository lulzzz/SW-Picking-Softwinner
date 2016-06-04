using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Support.V7.App;
using Android.Widget;
using App2.Modal;

namespace App2
{
	[Activity (Label = "Configurações", Theme = "@style/Theme.AppCompat.Light", ScreenOrientation = ScreenOrientation.Portrait)]
	public class Settings : AppCompatActivity
	{

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			SetContentView (Resource.Layout.Settings);
			var manager= new ZsManager();
			var btnLogout = FindViewById<Button> (Resource.Id.btnEndSession);
			var saveSettings = FindViewById<Button> (Resource.Id.btnSave);
		    var txtEmail = FindViewById<TextView>(Resource.Id.txtSettingsEmail);

		    if (manager.HasEmail())
		        txtEmail.Text = manager.GetItem("emailToCSV");

			//int mode = preferences.GetInt ("mode", 1);
			
			saveSettings.Click += (sender, e) => {
               manager.AddItem(txtEmail.Text, "emailToCSV");
                StartActivity(typeof(MainActivity));
            };
			btnLogout.Click += (sender, e) => {
				var pref = Application.Context.GetSharedPreferences ("UserInfo", FileCreationMode.Private);
				var edit = pref.Edit ();
				edit.Clear ();
				edit.Apply ();
				Toast.MakeText (this, "Sessão terminada com sucesso!", ToastLength.Short).Show ();
				StartActivity (typeof(MainActivity));
			};
		}
		//public override void Finish()
		//{
		//    Toast.MakeText(this, "Fechar!", ToastLength.Short).Show();
		//}

	}
}