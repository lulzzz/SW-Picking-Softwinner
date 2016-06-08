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

        public EtiClient(string username, string password, string adress)
        {
            Username = username;
            Password = password;
            SqlServerAddress = adress;
            IsOnline = true;
            Port = 8080;
        }

        public string GetStockForProductWithBarCode(string barCode)
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
        }
    }
}