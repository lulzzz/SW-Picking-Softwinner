using System;
using Android.App;
using Android.Content;
using System.Collections.Generic;
using System.IO;
using Android.Net;
using Android.OS;


namespace ZSProduct.Modal
{
    internal class ZsManager : Activity
    {
        public readonly ISharedPreferences Preferences = Application.Context.GetSharedPreferences("UserInfo", FileCreationMode.Private);
        public EticadataSettings Eticadata;

        private readonly string _nif;
        private readonly string _username;
        private readonly string _password;
        private Context _context;
        private ConnectivityManager connectivityManager;
        private NetworkInfo activeConnection;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Console.WriteLine("Created");
        }

        //-----------------------------------------------------------
        public ZsManager()
        {
            _nif = GetItem("nif");
            _username = GetItem("username");
            _password = GetItem("password");
        }

        //-----------------------------------------------------------
        public bool HasInternet()
        {
            connectivityManager = (ConnectivityManager)_context.GetSystemService(ConnectivityService);
            activeConnection = connectivityManager.ActiveNetworkInfo;

            return (activeConnection != null) && activeConnection.IsConnected;
        }

        //-----------------------------------------------------------
        public ZsManager(Context context)
        {
            _context = context;
            _nif = GetItem("nif");
            _username = GetItem("username");
            _password = GetItem("password");
        }

        //-----------------------------------------------------------
        public void SaveData(List<AddProducttoListView> _prodList)
        {
            //var downloadsFolder = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads);
            // const string downloadsFolder = "/storage/emulated/0/SWProduct";
            const string filePath = "/storage/emulated/0/SWPicking.imp";

            using (var streamWriter = new StreamWriter(filePath, false))
            {
                foreach (var item in _prodList)
                {
                    streamWriter.WriteLine(item.BarCode + "|" + item.Qtd + "\n");
                }
                streamWriter.Close();
            }
        }

        //-----------------------------------------------------------
        public struct EticadataSettings
        {
            public string Username;
            public string Password;
            public string ServerAddress;
            public string Port;
        }

        //-----------------------------------------------------------
        public bool HasEmail()
        {
            if (GetItem("emailToCSV") != "")
                return true;
            return false;
        }

        //-----------------------------------------------------------
        public void AddItem(string data, string name) => Preferences.Edit().PutString(name, data).Apply();

        //-----------------------------------------------------------
        public void AddItem(int data, string name) => Preferences.Edit().PutInt(name, data).Apply();

        //-----------------------------------------------------------
        public void AddItem(bool data, string name) => Preferences.Edit().PutBoolean(name, data).Apply();

        //-----------------------------------------------------------
        public string GetItem(string name) => Preferences.GetString(name, string.Empty);
    }
}