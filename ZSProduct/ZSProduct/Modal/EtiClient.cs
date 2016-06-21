using System;
using System.Net;
using System.Text;
using Android.Widget;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace ZSProduct.Modal
{
    public class EtiClient
    {
        public string Username;
        public string Password;
        public string SqlServerAddress;
        public bool IsOnline;
        public uint Port;

        //-----------------------------------------------------------
        private Product _product;
        private string _request;
        private byte[] _dataBytes;
        private WebClient _wc;
        private byte[] _responseBytes;
        private string _responseString;
        private JObject _element;

        public EtiClient(string username, string password, string adress, uint port)
        {
            Username = username;
            Password = password;
            SqlServerAddress = adress;
            IsOnline = true;
            Port = port;
        }

        public int Login()
        {
            try
            {
                _request = "{\"username\":\"" + Username + "\",\"password\":\"" + Password + "\"}";
                Console.WriteLine("\n\n\n " + _request);
                _dataBytes = Encoding.UTF8.GetBytes(_request);
                _wc = new WebClient();
                _wc.Headers.Add(HttpRequestHeader.ContentType, "application/json; charset=utf-8");
                Console.WriteLine("LOGIN REQUEST: " + _request);
                _responseBytes = _wc.UploadData(new Uri("http://" + SqlServerAddress + ":" + Port + "/Authenticate"), "POST", _dataBytes);
                _responseString = Encoding.UTF8.GetString(_responseBytes);

                //Parse the string to a JsonObject
                _element = JObject.Parse(_responseString);
                if ((int)_element["Response"]["StatusCode"] != 200) return 0;
                if ((string)_element["Response"]["Content"] == "Success")
                {
                    IsOnline = true;
                    return 1;
                }
                else if ((string)_element["Response"]["Content"] == "Failed")
                {
                    IsOnline = false;
                    return 0;
                }
                return 0;
            }
            catch
            {
                Console.WriteLine("Falhou login. Verifique conecção.");
                IsOnline = false;
                return -1;
            }
        }

        //-----------------------------------------------------------
        public Product GetProductWithBarCode(string barCode)
        {
            if (IsOnline)
            {
                try
                {
                    _product = null;
                    _request = "{\"username\":\"" + Username + "\",\"password\":\"" + Password + "\"}";
                    Console.WriteLine("\n\n\n" + _request);
                    Console.WriteLine("GET PRODUCT WITH BAR CODE: " + _request);
                    _dataBytes = Encoding.UTF8.GetBytes(_request);
                    _wc = new WebClient();
                    _wc.Headers.Add(HttpRequestHeader.ContentType, "application/json; charset=utf-8");
                    _responseBytes =
                        _wc.UploadData(new Uri("http://" + SqlServerAddress + ":" + Port + "/GetProduct/" + barCode),
                            "POST", _dataBytes);
                    _responseString = Encoding.UTF8.GetString(_responseBytes);
                    if (_responseString != "")
                    {
                        Console.WriteLine();
                        var element = JObject.Parse(_responseString);
                        Console.WriteLine(element);
                        if ((int)element["Response"]["StatusCode"] == 200 && (string)element["Response"]["StatusMessage"] != "WARNING")
                        {
                            var stocks = JsonConvert.DeserializeObject<Dictionary<string, object>>(element["Response"]["Content"]["product"]["stock"].ToString()).ToDictionary(item => item.Key, item => Convert.ToInt32(item.Value));
                            Console.WriteLine("Produto obtido com sucesso");
                            _product = new Product((string)element["Response"]["Content"]["product"]["description"],
                                                   (string)element["Response"]["Content"]["product"]["code"],
                                                   stocks,
                                                   (string)element["Response"]["Content"]["product"]["barCode"],
                                                   Convert.ToDouble(element["Response"]["Content"]["product"]["PVP1"].ToString()),
                                                   (double)element["Response"]["Content"]["product"]["PVP2"],
                                                   (string)element["Response"]["Content"]["product"]["reference"],
                                                   (string)element["Response"]["Content"]["product"]["supplier"],
                                                   (string)element["Response"]["Content"]["product"]["lastPrice"],
                                                   (string)element["Response"]["Content"]["product"]["averageCost"]);
                            return _product;
                        }
                        return null;
                    }
                    return null;
                }
                catch
                {
                    Console.WriteLine("Produto sem stock");
                    IsOnline = false;
                    return null;
                }
            }
            return null;
        }
    }
}