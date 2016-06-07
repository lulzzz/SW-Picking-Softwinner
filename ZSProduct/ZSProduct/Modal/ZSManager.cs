using Android.App;
using Android.Content;
using System.Collections.Generic;
using System.IO;


namespace ZSProduct.Modal
{
    internal class ZsManager
    {
        public readonly ISharedPreferences Preferences = Application.Context.GetSharedPreferences("UserInfo", FileCreationMode.Private);
        private readonly string _nif;
        private readonly string _username;
        private readonly string _password;
        private Context _context;
        //private ConnectivityManager _connectivityManager;
        //public ZSClient ZsClient;

        public ZsManager()
        {
            _nif = GetItem("nif");
            _username = GetItem("username");
            _password = GetItem("password");
            //ZsClient = new ZSClient(_username, _password, 0, _nif);
        }

        public ZsManager(Context context)
        {
            _context = context;
        }

        //-----------------------------------------------------------
        public void SaveData(List<AddProducttoListView> _prodList)
        {
            //var downloadsFolder = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads);
            var downloadsFolder = "/storage/emulated/0/ZSProduct";
            var filePath = Path.Combine(downloadsFolder/*.Path*/, "export.csv");

            using (var streamWriter = new StreamWriter(filePath, false))
            {
                foreach (var item in _prodList)
                {
                    streamWriter.WriteLine(item.Description + ";" + item.qtd);
                }
                streamWriter.Close();
            }
        }

        public struct EticadataSettings
        {
            public string Username;
            public string Password;
            public string ServerAddress;
        }

        public EticadataSettings Eticadata;

        public bool HasEticadataIntegration
        {
            get
            {
                return GetItem("eticadataIntegration") == "1";
            }
            set
            {
                if (value)
                {
                    HasEticadataIntegration = true;
                    AddItem("1", "eticadataIntegration");
                    Eticadata.Username = GetItem("eticadataUsernameAPI");
                    Eticadata.Password = GetItem("eticadataPasswordAPI");
                    Eticadata.ServerAddress = GetItem("sqlServerAdress");
                }
                else
                {
                    HasEticadataIntegration = false;
                    AddItem("0", "eticadataIntegration");
                    Eticadata.Username = null;
                    Eticadata.Password = null;
                    Eticadata.ServerAddress = null;
                }


            }
        }

        public bool CheckEticadataIntegration()
        {
            if (HasEticadataIntegration)
            {
                Eticadata.Username = GetItem("eticadataUsernameAPI");
                Eticadata.Password = GetItem("eticadataPasswordAPI");
                Eticadata.ServerAddress = GetItem("sqlServerAdress");
                return true;
            }
            Eticadata.Username = null;
            Eticadata.Password = null;
            Eticadata.ServerAddress = null;
            return false;
        }

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