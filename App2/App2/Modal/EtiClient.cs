using System;
using System.Text;
using System.Net;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;


namespace ZSProduct
{
	public class EtiClient
	{
		public string username;
		public string password;
		public string sqlServerAddress;
		public bool isOnline;
		public uint port;

		public EtiClient (string username, string password, string adress)
		{
			this.username = username;
			this.password = password;
			this.sqlServerAddress = adress;
			isOnline = true;
			port = 8080;
		}

		public string GetStockForProductWithBarCode (string barCode)
		{
			if (isOnline) {
				try {
					String request = "{\"product\":\"" + barCode + "\"}";
					byte[] dataBytes = Encoding.UTF8.GetBytes (request);
					WebClient wc = new WebClient ();
					wc.Headers.Add (HttpRequestHeader.ContentType, "application/json; charset=utf-8");
					Console.WriteLine (request);
					byte[] responseBytes = wc.UploadData (new Uri ("http://192.168.100.174:" + port + "/") + barCode,
						                       "POST", dataBytes);
					string responseString = Encoding.UTF8.GetString (responseBytes);
		
					//Parse the string to a JsonObject
					var element = JObject.Parse (responseString);
					if (((int)element ["Response"] ["StatusCode"]) == 200) {
						Console.WriteLine ("Produto obtido com sucesso");
						//var array = JArray.Parse (element ["Response"] ["Content"] ["ProductStock"].ToString ());
						//Get the other values
						string productCode = (element ["Response"] ["Content"] ["ProductStock"] ["code"]).ToString ();
						string productStock = (element ["Response"] ["Content"] ["ProductStock"] ["stock"]).ToString ();
						//var deserializedProduct = JsonConvert.DeserializeObject (productCode);
						//Console.WriteLine (deserializedProduct.ToString ());
						return productStock;
					}
					return null;
				} catch {
					Console.WriteLine ("Falhou obter producto.Verifique conecçao.");
					this.isOnline = false;
					return null;
				}
			}
			return null;
		}
	}
}

