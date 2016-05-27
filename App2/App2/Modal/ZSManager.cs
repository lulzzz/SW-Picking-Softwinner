using Android.App;
using Android.Content;
using System;
using Java.Lang;
using ZSProduct;

namespace App2.Modal
{
    public class ZsManager
    {
        //-----------------------------------------------------------
        private readonly ISharedPreferences _pref = Application.Context.GetSharedPreferences("UserInfo", FileCreationMode.Private);
        private readonly string _nif;
        private readonly string _username;
        private readonly string _password;
        private readonly uint _storeCounter;
        private NetworkingMode _mode;
        public ZSClient zsClient;

        public enum NetworkingMode
        {
            OFFLINE = 0,
            ONLINE = 1
        }

        //public ZsManager() { }

        public ZsManager(/*string userName, string password, string nif*/)
        {
            _nif = GetItem("nif");
            _username = GetItem("username");
            _password = GetItem("password");
            _mode = GetItem("mode") == "1" ? NetworkingMode.ONLINE : NetworkingMode.OFFLINE;
            zsClient = new ZSClient(_username, _password, 0, _nif);
        }

        public void DownloadStoreAsync()
        {
            var task = new Thread(() =>
            {
                Console.WriteLine("Connecting to ZS...");
                //Do long run
                var zsHandler = new ZSClient(_username, _password, 0, _nif);
                zsHandler.Login();
                //zsHandler.GetProducts();
                Console.WriteLine("\n\n\n\n CONCLUIDO \n\n\n\n");
                //zsHandler.TotalStores();
            });
            task.Start();
        }

        //-----------------------------------------------------------
        public void AddItem(string data, string name) => _pref.Edit().PutString(name, data).Apply();

        //-----------------------------------------------------------
        public void AddItem(int data, string name) => _pref.Edit().PutInt(name, data).Apply();

        //-----------------------------------------------------------
        public void AddItem(bool data, string name) => _pref.Edit().PutBoolean(name, data).Apply();

        //-----------------------------------------------------------
        public string GetItem(string name) => _pref.GetString(name, string.Empty);
    }
}