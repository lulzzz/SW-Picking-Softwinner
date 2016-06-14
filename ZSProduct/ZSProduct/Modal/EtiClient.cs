using System;
using System.Net;
using System.Text;
using Newtonsoft.Json.Linq;

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

        public EtiClient(string username, string password, string adress)
        {
            Username = username;
            Password = password;
            SqlServerAddress = adress;
            IsOnline = true;
            Port = 8080;
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
                        //var element = JObject.Parse("{CPU:'Intel',Drives:['DVD read/writer','500 gigabyte hard drive']}");
                        Console.WriteLine();
                        var element = JObject.Parse(_responseString);
                        //Console.WriteLine("\n\n\n\n\n");
                        //Console.WriteLine(element);
                        //Console.WriteLine("\n\n\n\n\n");
                        Console.WriteLine(element);
                        if ((int)element["Response"]["StatusCode"] == 200 /*&& element["Response"]["StatusMessage"]*/)
                        {
                            Console.WriteLine("Produto obtido com sucesso");
                            //Get the values and build Product object for each element
                            var desc = (double)element["Response"]["Content"]["product"]["stock"];
                            Console.WriteLine(desc);
                            _product = new Product("agaeg", "shf", 12, desc,
                                (string)element["Response"]["Content"]["product"]["code"], 10, 9, "agag", 8, "dfhdf",
                                "a");
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


        /*public string GetStockForProductWithBarCode(string barCode)
        {
            if (IsOnline)
            {
                try
                {
                    String request = "{\"product\":\"" + barCode + "\"}";
                    byte[] dataBytes = Encoding.UTF8.GetBytes(request);
                    WebClient wc = new WebClient();
                    wc.Headers.Add(HttpRequestHeader.ContentType, "application/json; charset=utf-8");
                    Console.WriteLine(request);
                    byte[] responseBytes = wc.UploadData(new Uri("http://192.168.100.174:" + Port + "/") + barCode,
                                               "POST", dataBytes);
                    string responseString = Encoding.UTF8.GetString(responseBytes);

                    //Parse the string to a JsonObject
                    var element = JObject.Parse(responseString);
                    if (((int)element["Response"]["StatusCode"]) == 200)
                    {
                        Console.WriteLine("Produto obtido com sucesso");
                        //var array = JArray.Parse (element ["Response"] ["Content"] ["ProductStock"].ToString ());
                        //Get the other values
                        string productCode = (element["Response"]["Content"]["ProductStock"]["code"]).ToString();
                        string productStock = (element["Response"]["Content"]["ProductStock"]["stock"]).ToString();
                        //var deserializedProduct = JsonConvert.DeserializeObject (productCode);
                        //Console.WriteLine (deserializedProduct.ToString ());
                        return productStock;
                    }
                    return null;
                }
                catch
                {
                    Console.WriteLine("Falhou obter producto.Verifique conecçao.");
                    this.IsOnline = false;
                    return null;
                }
            }
            return null;
        }*/
    }
}