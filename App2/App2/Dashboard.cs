
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Support.V7.App;

namespace App2
{
	[Activity (Label = "Menu Principal", Theme = "@style/Theme.AppCompat.Light")]			
	public class Dashboard : AppCompatActivity
	{
		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
		

	

		}

		public override bool OnCreateOptionsMenu (IMenu menu)
		{
			menu.Add (new Java.Lang.String ("Configuracoes"));
			menu.Add (new Java.Lang.String ("Test"));
			menu.Add (new Java.Lang.String ("Acerca"));
			return true;
		}

		public override bool OnOptionsItemSelected (IMenuItem item)
		{
			switch (item.TitleFormatted.ToString ()) {
			case "Settings":
				var settings = new Intent (this, typeof(Settings));
				StartActivity (settings);
				break;
			case "About":
				var about = new Intent (this, typeof(About));
				StartActivity (about);
				break;
			}
			return true;
		}
	}
}

