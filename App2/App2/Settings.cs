using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;
using Android.Widget;

namespace App2
{
	[Activity (Label = "Settings", Theme = "@style/Theme.AppCompat.Light")]
	public class Settings : AppCompatActivity
	{
		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			SetContentView (Resource.Layout.Settings);
			var btnLogout = FindViewById<Button> (Resource.Id.btnTerminarSessao);
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