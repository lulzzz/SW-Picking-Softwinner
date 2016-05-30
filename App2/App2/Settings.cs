using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;
using Android.Widget;

namespace App2
{
	[Activity (Label = "Configura??es", Theme = "@style/Theme.AppCompat.Light")]
	public class Settings : AppCompatActivity
	{

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			SetContentView (Resource.Layout.Settings);
			ISharedPreferences preferences = Application.Context.GetSharedPreferences ("UserInfo", FileCreationMode.Private);
			var onlineButton = FindViewById<RadioButton> (Resource.Id.radioButtonOnline);
			var offlineButton = FindViewById<RadioButton> (Resource.Id.radioButtonOffline);
			var btnLogout = FindViewById<Button> (Resource.Id.btnEndSession);
			var saveSettings = FindViewById<Button> (Resource.Id.btnSave);

			int mode = preferences.GetInt ("mode", 1);
			if (mode == 1) {
				onlineButton.Checked = true;
			} else {
				offlineButton.Checked = true;
			}
			//
			onlineButton.Click += (sender, e) => {
				offlineButton.Checked = false;
				preferences.Edit ().PutInt ("mode", 1).Apply ();

			};
			offlineButton.Click += (sender, e) => {
				onlineButton.Checked = false;
				preferences.Edit ().PutInt ("mode", 0).Apply ();
			};
			saveSettings.Click += (sender, e) => {
				
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