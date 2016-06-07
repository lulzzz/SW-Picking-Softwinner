using System;
using Android.App;
using Android.Content;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Android.Net;
using ZSProduct;
using Android.OS;
using Android.Support.V7.App;
using Android.Widget;

namespace App2.Modal
{
	[Activity]
	public class ZsManager : AppCompatActivity
	{
		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			SetContentView (Resource.Layout.Pdt);
		}

		//-----------------------------------------------------------
		public readonly ISharedPreferences Preferences = Application.Context.GetSharedPreferences ("UserInfo", FileCreationMode.Private);
		private readonly string _nif;
		private readonly string _username;
		private readonly string _password;
		private Context _context;
		private ConnectivityManager _connectivityManager;
		public ZSClient ZsClient;

		public struct EticadataSettings
		{
			public string username;
			public string password;
			public string serverAddress;
		}

		public EticadataSettings eticadata;

		public bool hasEticadataIntegration {
			get {
				return (GetItem ("eticadataIntegration") == "1");
			}
			set {
				if (value == true) {
					hasEticadataIntegration = true;
					AddItem ("1", "eticadataIntegration");
					eticadata.username = GetItem ("eticadataUsernameAPI");
					eticadata.password = GetItem ("eticadataPasswordAPI");
					eticadata.serverAddress = GetItem ("sqlServerAdress");
				} else {
					hasEticadataIntegration = false;
					AddItem ("0", "eticadataIntegration");
					eticadata.username = null;
					eticadata.password = null;
					eticadata.serverAddress = null;
				}

			
			}
		}

		public enum NetworkingMode
		{
			Offline = 0,
			Online = 1
		}

		public ZsManager (Context context)
		{
			_context = context;
		}

		public ZsManager ()
		{
			_nif = GetItem ("nif");
			_username = GetItem ("username");
			_password = GetItem ("password");
			ZsClient = new ZSClient (_username, _password, 0, _nif);
			checkEticadataIntegration ();

		}

		public bool checkEticadataIntegration ()
		{
			if (hasEticadataIntegration) {
				eticadata.username = GetItem ("eticadataUsernameAPI");
				eticadata.password = GetItem ("eticadataPasswordAPI");
				eticadata.serverAddress = GetItem ("sqlServerAdress");
				return true;
			} else {
				eticadata.username = null;
				eticadata.password = null;
				eticadata.serverAddress = null;
				return false;
			}
		}

		public void DownloadStoreAsync ()
		{
			var task = new Thread (() => {
				Console.WriteLine ("Connecting to ZS...");
				//Do long run
				var zsHandler = new ZSClient (_username, _password, 0, _nif);
				zsHandler.Login ();
				//zsHandler.GetProducts();
				Console.WriteLine ("\n\n\n\n CONCLUIDO \n\n\n\n");
				//zsHandler.TotalStores();
			});
			task.Start ();
		}

		//-----------------------------------------------------------
		public void SaveData (List<AddProducttoListView> _prodList)
		{
			var downloadsFolder = Android.OS.Environment.GetExternalStoragePublicDirectory (Android.OS.Environment.DirectoryDownloads);
			var filePath = Path.Combine (downloadsFolder.Path, "export.csv");

			using (var streamWriter = new StreamWriter (filePath, false)) {
				foreach (var item in _prodList) {
					streamWriter.WriteLine (item.description + ";" + item.qtd);
				}
				streamWriter.Close ();
			}
		}

		//-----------------------------------------------------------
		public bool HasEmail ()
		{
			if (GetItem ("emailToCSV") != "")
				return true;
			return false;
		}

		//public bool HasInternet()
		//{
		//    _connectivityManager = (ConnectivityManager)GetSystemService(ConnectivityService);
		//    var activeConnection = _connectivityManager.ActiveNetworkInfo;
		//    var isOnline = (activeConnection != null) && activeConnection.IsConnected;
		//    if (isOnline)
		//        return true;
		//    Toast.MakeText(_context, "Verifique conexão à rede", ToastLength.Long);
		//    return false;
		//}

		//-----------------------------------------------------------
		public void AddItem (string data, string name) => Preferences.Edit().PutString(name, data).Apply();

		//-----------------------------------------------------------
		public void AddItem (int data, string name) => Preferences.Edit().PutInt(name, data).Apply();

		//-----------------------------------------------------------
		public void AddItem (bool data, string name) => Preferences.Edit().PutBoolean(name, data).Apply();

		//-----------------------------------------------------------
		public string GetItem (string name) => Preferences.GetString(name, string.Empty);

	}
}