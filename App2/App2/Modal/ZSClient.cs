//ZoneSoft Class
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using ZSProduct.Modal;
using String = System.String;
using System.Threading;
using Android.Content;
using Android.Widget;
using Android.Renderscripts;
using System.Security.Policy;
using Android.Locations;


namespace ZSProduct
{
	public class ZSClient
	{
		public string hash { get; set; }

		public  bool isAutenticated;
		public string username;
		public string password;
		public string nif;
		public int store;
		public List<Product> productInStore;
		public List<Store> stores;
		public List<Supplier> suppliers;
		Product product;
					

		//Constructor
		public ZSClient (string username, string password, int store, string nif)
		{
			this.username = username;
			this.password = password;
			this.store = store;
			this.nif = nif;
			this.productInStore = new List<Product> ();
			this.stores = new List<Store> ();
		}


		//-----------------------------------------------------------
		//Do login
		public bool Login ()
		{
			try {
				String dataToPost = "{\"user\":{\"nif\":\"" + nif + "\",\"nome\":\"" + username + "\",\"password\":\"" +
				                    password + "\",\"loja\":\"" + store + "\"}}";
				byte[] dataBytes = Encoding.UTF8.GetBytes (dataToPost);
				WebClient wc = new WebClient ();
				wc.Headers.Add (HttpRequestHeader.ContentType, "application/json; charset=utf-8");
				Console.WriteLine (dataToPost);
				byte[] responseBytes = wc.UploadData (new Uri ("https://api.zonesoft.org/v1.5/auth/authenticate/"), "POST",
					                       dataBytes);
				string responseString = Encoding.UTF8.GetString (responseBytes);

				//Parse the string to a JsonObject
				var element = JObject.Parse (responseString);
				if (((int)element ["Response"] ["StatusCode"]) == 200) {
					Console.WriteLine ("Autenticado com sucesso");
					//Get the other values
					this.hash = (element ["Response"] ["Content"] ["auth_hash"]).ToString ();
					this.isAutenticated = true;
					Console.WriteLine (this.hash);
					return true;
				} else
					return false;
			} catch {
				Console.WriteLine ("Falhou login.Verifique conecçao.");
				this.isAutenticated = false;
				return false;
			}


		}

		//-----------------------------------------------------------

		public string GetProductCode (string barCode)
		{
			if (isAutenticated) {
				try {
					String request = "{\"auth_hash\":\"" + hash + "\",\"product\":[{\"codbarras\":\"" + barCode +
					                 "\"}]}";
					byte[] dataBytes = Encoding.UTF8.GetBytes (request);
					WebClient wc = new WebClient ();
					wc.Headers.Add (HttpRequestHeader.ContentType, "application/json; charset=utf-8");
					Console.WriteLine (request);
					byte[] responseBytes = wc.UploadData (new Uri ("https://api.zonesoft.org/v1.6/Products/getInstance"),
						                       "POST", dataBytes);
					string responseString = Encoding.UTF8.GetString (responseBytes);

					//Parse the string to a JsonObject
					var element = JObject.Parse (responseString);
					if (((int)element ["Response"] ["StatusCode"]) == 200) {
						Console.WriteLine ("Produto obtido com sucesso");
						//Get the other values
						string productCode = (element ["Response"] ["Content"] ["product"] [0] ["codigo"]).ToString ();
						var deserializedProduct = JsonConvert.DeserializeObject (productCode);
						Console.WriteLine (deserializedProduct.ToString ());
						return productCode;
					}
					return null;
				} catch {
					Console.WriteLine ("Falhou obter producto.Verifique conecçao.");
					this.isAutenticated = false;
					return null;
				}
			}
			return null;
		}

		//-----------------------------------------------------------

		//public Product GetProductWithBarCode (string barCode) => productInStore.FirstOrDefault(item => item.GetBarCode() == barCode);

		//-----------------------------------------------------------

		public List<string> GetProducts ()
		{
			if (isAutenticated) {
				try {
					var continuar = true;
					var val = 0;
					var nRespostas = 0;
					do {
						Console.WriteLine ("\n\n\n\nval= " + val + "\n\n\n\n");
						var request = "{\"auth_hash\":\"" + hash + "\",\"product\":{\"offset\":\"" + val + "\"}}";
						var dataBytes = Encoding.UTF8.GetBytes (request);
						var wc = new WebClient ();
						wc.Headers.Add (HttpRequestHeader.ContentType, "application/json; charset=utf-8");
						Console.WriteLine (request);
						var responseBytes = wc.UploadData (
							                    new Uri ("https://api.zonesoft.org/v1.6/Products/getInstances"), "POST", dataBytes);
						var responseString = Encoding.UTF8.GetString (responseBytes);

						//Parse the string to a JsonObject
						var element = JObject.Parse (responseString);
						if (((int)element ["Response"] ["StatusCode"]) == 200) {
							Console.WriteLine ("Produto obtido com sucesso");
							//Get the values and build Product object for each element
							var array = JArray.Parse (element ["Response"] ["Content"] ["product"].ToString ());
							//Console.WriteLine (array);
							foreach (var content in array) {
								var singleProduct = new Product (content ["descricaocurta"].ToString (),
									                    (uint)content ["codigo"],
									                    (uint)content ["loja"],
									                    (uint)content ["qtdstock"],
									                    (string)content ["codbarras"],
									                    (double)content ["precovenda"],
									                    (double)content ["pvp2"],
									                    (string)content ["referencia"],
									                    (string)content ["ultprecocompra"]);
								//Console.WriteLine("Código: " + content["codigo"] + ": \n" + "Loja: " + content["loja"] +
								//                  "\nStock: " + content["qtdstock"] + "\nCod. Barras : " +
								//                  content["codbarras"] + "\npreco venda: " + content["precovenda"] +
								//                  "\n pvp2: " + content["pvp2"]);
								productInStore.Add (singleProduct);
								nRespostas++;
							}
						}
						if (nRespostas != 250)
							continuar = false;
						nRespostas = 0;
						val += 250;
						//Thread.Sleep(500);
					} while (continuar);

					return null;
				} catch {
					Console.WriteLine ("Falhou obter producto.Verifique conecçao.");
					isAutenticated = false;
					return null;
				}
			}
			return null;
		}

		//-----------------------------------------------------------

		public int GetProductsCount () => productInStore.Count;

		//-----------------------------------------------------------

		public string GetSuppliers ()
		{
			if (isAutenticated) {
				try {
					string request = "{\"auth_hash\":\"" + hash + "\"}";
					byte[] dataBytes = Encoding.UTF8.GetBytes (request);
					WebClient wc = new WebClient ();
					wc.Headers.Add (HttpRequestHeader.ContentType, "application/json; charset=utf-8");
					Console.WriteLine (request);
					byte[] responseBytes = wc.UploadData (
						                       new Uri ("https://api.zonesoft.org/v1.6/Suppliers/getInstances"), "POST", dataBytes);
					string responseString = Encoding.UTF8.GetString (responseBytes);

					//Parse the string to a JsonObject
					JObject element = JObject.Parse (responseString);
					if (((int)element ["Response"] ["StatusCode"]) == 200) {
						Console.WriteLine ("Dados do fornecedor obtidos com sucesso");
						//Get the values and build Product object for each element
						var array = JArray.Parse (element ["Response"] ["Content"] ["product"].ToString ());
						foreach (var content in array) {
							var singleSupplier = new Supplier ((uint)content ["codigo"],
								                     content ["nome"].ToString (),
								                     content ["telefone"].ToString (),
								                     content ["telemovel"].ToString (),
								                     content ["web"].ToString (),
								                     content ["email"].ToString ());
							suppliers.Add (singleSupplier);
						}
					}
					return null;
				} catch {
					Console.WriteLine ("Falhou obter dados do fornecedor. Verifique conecçao.");
					this.isAutenticated = false;
					return null;
				}
			}
			return null;
		}

		//-----------------------------------------------------------

		public string GetSupplierNameWithId (int id)
		            => (from item in suppliers where item.GetId() == id select item.GetName()).FirstOrDefault();

		//-----------------------------------------------------------

		public List<string> GetStores ()
		{
			if (isAutenticated) {
				try {
					List<Product> products = new List<Product> ();
					String request = "{\"auth_hash\":\"" + hash + "\"}";
					byte[] dataBytes = Encoding.UTF8.GetBytes (request);
					WebClient wc = new WebClient ();
					wc.Headers.Add (HttpRequestHeader.ContentType, "application/json; charset=utf-8");
					Console.WriteLine (request);
					byte[] responseBytes = wc.UploadData (new Uri ("https://api.zonesoft.org/v1.6/Stores/getInstances"),
						                       "POST", dataBytes);
					string responseString = Encoding.UTF8.GetString (responseBytes);

					//Parse the string to a JsonObject
						

					JObject element = JObject.Parse (responseString);
					Console.WriteLine ("\n\n\n\n AQUI \n\n\n");
					Console.WriteLine (element);
					if (((int)element ["Response"] ["StatusCode"]) == 200) {
						Console.WriteLine ("Loja obtida com sucesso");
						//Get the values and build Product object for each element
						var array = JArray.Parse (element ["Response"] ["Content"] ["product"].ToString ());
						//Console.WriteLine (array);
						foreach (var content in array) {
							Store singleProduct = new Store ((uint)content ["codigo"],
								                      content ["descricao"].ToString (),
								                      content ["designacao"].ToString (),
								                      content ["morada"].ToString ());

							stores.Add (singleProduct);
						}
						return null;

					}
					return null;
				} catch {
					Console.WriteLine ("Falhou obter lojas.Verifique conecçao.");
					this.isAutenticated = false;
					return null;
				}
			}
			return null;
		}

		//-----------------------------------------------------------



		//-----------------------------------------------------------

		public string GetStockForStoresWithProductCode (string productCode)
		{
			if (isAutenticated) {
				try {
					var request = "{\"auth_hash\":\"" + hash + "\",\"productstock\":{\"produto\":\"" + productCode +
					              "\"}}";
					Console.WriteLine (request);
					byte[] dataBytes = Encoding.UTF8.GetBytes (request);
					WebClient wc = new WebClient ();
					wc.Headers.Add (HttpRequestHeader.ContentType, "application/json; charset=utf-8");
					Console.WriteLine (request);
					byte[] responseBytes =
						wc.UploadData (new Uri ("https://api.zonesoft.org/v1.6/ProductStocks/getCurrentStockInStores"),
							"POST", dataBytes);
					string responseString = Encoding.UTF8.GetString (responseBytes);

					//Parse the string to a JsonObject
					var element = JObject.Parse (responseString);
					if (((int)element ["Response"] ["StatusCode"]) == 200) {
						Console.WriteLine ("Produto obtido com sucesso");
						//Get the other values
						var container = (element ["Response"] ["Content"] ["productstock"]);
						string stock = container [0] ["stock"].ToString ();
						return stock;

					}
					return "0";
				} catch {
					Console.WriteLine ("Produto sem stock");
					this.isAutenticated = false;
					return "0";
				}


			}
			return null;
		}

		//-----------------------------------------------------------

		public Product GetProductWithBarCode (string barCode)
		{
			var request = "{\"auth_hash\":\"" + hash + "\",\"product\":{\"condition\":\"codbarras='" + barCode + "'\"}}";
			var dataBytes = Encoding.UTF8.GetBytes (request);
			var wc = new WebClient ();
			wc.Headers.Add (HttpRequestHeader.ContentType, "application/json; charset=utf-8");
			var responseBytes = wc.UploadData (new Uri ("https://api.zonesoft.org/v1.6/Products/getInstances"), "POST",
				                    dataBytes);
			var responseString = Encoding.UTF8.GetString (responseBytes);
			if (responseString != "") {
				var element = JObject.Parse (responseString);
				if ((int)element ["Response"] ["StatusCode"] == 200) {
					Console.WriteLine ("Produto obtido com sucesso");
					//Get the values and build Product object for each element
					var array = JArray.Parse (element ["Response"] ["Content"] ["product"].ToString ());
					Console.WriteLine (array);
					product = new Product (array [0] ["descricao"].ToString (),
						(uint)array [0] ["codigo"],
						(uint)array [0] ["loja"],
						(uint)array [0] ["qtdstock"],
						(string)array [0] ["codbarras"],
						(double)array [0] ["precovenda"],
						(double)array [0] ["pvp2"],
						(string)array [0] ["referencia"],
						(string)array [0] ["ultprecocompra"]);
					return product;
				}
			} else {
				return null;
			}
			return null;

		}

		//-----------------------------------------------------------

		public int StoreCount ()
		{
			if (this.isAutenticated) {
				var continuar = true;
				var nLojas = 0;
				do {
					var request = "{\"auth_hash\":\"" + hash + "\",\"store\":{\"codigo\":\"" + nLojas + "\"}}";
					var dataBytes = Encoding.UTF8.GetBytes (request);
					var wc = new WebClient ();
					wc.Headers.Add (HttpRequestHeader.ContentType, "application/json; charset=utf-8");
					var responseBytes = wc.UploadData (new Uri ("https://api.zonesoft.org/v1.6/Stores/getInstance"), "POST", dataBytes);
					var responseString = Encoding.UTF8.GetString (responseBytes);
					var element = JObject.Parse (responseString);

					if ((int)element ["Response"] ["StatusCode"] != 200)
						continue;
					if ((string)element ["Response"] ["Content"] ["store"] ["descricao"] == "")
						continuar = false;
					nLojas++;
					Thread.Sleep (500);
				} while (continuar);
				return nLojas - 2; //Shop 0 and last store are added, but do not count
			} else
				return 0;
		}

		public List<Store> GetStoresList ()
		{
			int totalStores = this.StoreCount ();
			var list = new List<Store> ();
			for (uint i = 0; i <= totalStores; i++) {
				var request = "{\"auth_hash\":\"" + hash + "\",\"store\":{\"codigo\":\"" + i + "\"}}";
				var dataBytes = Encoding.UTF8.GetBytes (request);
				var wc = new WebClient ();
				wc.Headers.Add (HttpRequestHeader.ContentType, "application/json; charset=utf-8");
				var responseBytes = wc.UploadData (new Uri ("https://api.zonesoft.org/v1.6/Stores/getInstance"), "POST", dataBytes);
				var responseString = Encoding.UTF8.GetString (responseBytes);
				if (responseString != "") {
					var element = JObject.Parse (responseString);
					if ((int)element ["Response"] ["StatusCode"] == 200) {
						//Get the values and build Product object for each element

						string description = (element ["Response"] ["Content"] ["store"] ["descricao"].ToString ());
						uint code = ((uint)element ["Response"] ["Content"] ["store"] ["codigo"]);
						string name = (element ["Response"] ["Content"] ["store"] ["designacao"].ToString ());
						string address = (element ["Response"] ["Content"] ["store"] ["morada"].ToString ());

							
						var store = new Store (code, description, name, address);
						list.Add (store);
					}
				} else {
					Console.WriteLine ("Store without response");
				}
			}
			return list;
		}
	}
}