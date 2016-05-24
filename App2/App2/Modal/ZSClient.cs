//ZoneSoft Class
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Android.Content;
using ZSProduct.Modal;


namespace ZSProduct
{
    public class ZSClient
    {
        public string hash { get; set; }

        public int nLojas { get; set; }
        bool isAutenticated;
        string username;
        string password;
        string nif;
        int store;
        private List<Product> productInStore;
        private List<Store> stores;
        private List<Supplier> suppliers;

        //Constructor
        public ZSClient(string username, string password, int store, string nif)
        {
            this.username = username;
            this.password = password;
            this.store = store;
            this.nif = nif;
            this.productInStore = new List<Product>();
        }


        //-----------------------------------------------------------
        //Do login
        public bool Login()
        {
            try
            {
                String dataToPost = "{\"user\":{\"nif\":\"" + nif + "\",\"nome\":\"" + username + "\",\"password\":\"" + password + "\",\"loja\":\"" + store + "\"}}";
                byte[] dataBytes = Encoding.UTF8.GetBytes(dataToPost);
                WebClient wc = new WebClient();
                wc.Headers.Add(HttpRequestHeader.ContentType, "application/json; charset=utf-8");
                Console.WriteLine(dataToPost);
                byte[] responseBytes = wc.UploadData(new Uri("https://api.zonesoft.org/v1.5/auth/authenticate/"), "POST", dataBytes);
                string responseString = Encoding.UTF8.GetString(responseBytes);

                //Parse the string to a JsonObject
                var element = JObject.Parse(responseString);
                if (((int)element["Response"]["StatusCode"]) == 200)
                {
                    Console.WriteLine("Autenticado com sucesso");
                    //Get the other values
                    this.hash = (element["Response"]["Content"]["auth_hash"]).ToString();
                    this.isAutenticated = true;
                    Console.WriteLine(this.hash);
                    return true;
                }
                else
                    return false;
            }
            catch
            {
                Console.WriteLine("Falhou login.Verifique conecçao.");
                this.isAutenticated = false;
                return false;
            }


        }

        //-----------------------------------------------------------

        public string GetProductCode(string barCode)
        {
            if (isAutenticated)
            {
                try
                {
                    String request = "{\"auth_hash\":\"" + hash + "\",\"product\":[{\"codbarras\":\"" + barCode + "\"}]}";
                    byte[] dataBytes = Encoding.UTF8.GetBytes(request);
                    WebClient wc = new WebClient();
                    wc.Headers.Add(HttpRequestHeader.ContentType, "application/json; charset=utf-8");
                    Console.WriteLine(request);
                    byte[] responseBytes = wc.UploadData(new Uri("https://api.zonesoft.org/v1.6/Products/getInstance"), "POST", dataBytes);
                    string responseString = Encoding.UTF8.GetString(responseBytes);

                    //Parse the string to a JsonObject
                    var element = JObject.Parse(responseString);
                    if (((int)element["Response"]["StatusCode"]) == 200)
                    {
                        Console.WriteLine("Produto obtido com sucesso");
                        //Get the other values
                        string productCode = (element["Response"]["Content"]["product"][0]["codigo"]).ToString();
                        var deserializedProduct = JsonConvert.DeserializeObject(productCode);
                        Console.WriteLine(deserializedProduct.ToString());
                        return productCode;
                    }
                    return null;
                }
                catch
                {
                    Console.WriteLine("Falhou obter producto.Verifique conecçao.");
                    this.isAutenticated = false;
                    return null;
                }
            }
            return null;
        }

        //-----------------------------------------------------------

        public Product GetProductWithBarCode(string barCode) => productInStore.FirstOrDefault(item => item.GetBarCode() == barCode);

        //-----------------------------------------------------------

        public List<string> getProducts()
        {
            if (isAutenticated)
            {
                try
                {
                    string request = "{\"auth_hash\":\"" + hash + "\"}";
                    byte[] dataBytes = Encoding.UTF8.GetBytes(request);
                    WebClient wc = new WebClient();
                    wc.Headers.Add(HttpRequestHeader.ContentType, "application/json; charset=utf-8");
                    Console.WriteLine(request);
                    byte[] responseBytes = wc.UploadData(new Uri("https://api.zonesoft.org/v1.6/Products/getInstances"), "POST", dataBytes);
                    string responseString = Encoding.UTF8.GetString(responseBytes);

                    //Parse the string to a JsonObject
                    JObject element = JObject.Parse(responseString);
                    if (((int)element["Response"]["StatusCode"]) == 200)
                    {
                        Console.WriteLine("Produto obtido com sucesso");
                        //Get the values and build Product object for each element
                        var array = JArray.Parse(element["Response"]["Content"]["product"].ToString());
                        Console.WriteLine(array);
                        foreach (var content in array)
                        {
                            Product singleProduct = new Product(content["descricaocurta"].ToString(),
                                                    (uint)content["codigo"],
                                                    (uint)content["loja"],
                                                    (uint)content["qtdstock"],
                                                    (string)content["codbarras"],
                                                    (double)content["precovenda"],
                                                    (double)content["pvp2"]);
                            Console.WriteLine("Código: " + content["codigo"] + ": \n" + "Loja: " + content["loja"] + "\nStock: " + content["qtdstock"] + "\nCod. Barras : " + content["codbarras"] + "\npreco venda: " + content["precovenda"] + "\n pvp2: " + content["pvp2"]);
                            productInStore.Add(singleProduct);
                        }
                        return null;

                    }
                    return null;
                }
                catch
                {
                    Console.WriteLine("Falhou obter producto.Verifique conecçao.");
                    this.isAutenticated = false;
                    return null;
                }
            }
            return null;
        }

        //-----------------------------------------------------------

        public int getProductsCount() => productInStore.Count;

        //-----------------------------------------------------------

        public string getSuppliers()
        {
            if (isAutenticated)
            {
                try
                {
                    string request = "{\"auth_hash\":\"" + hash + "\"}";
                    byte[] dataBytes = Encoding.UTF8.GetBytes(request);
                    WebClient wc = new WebClient();
                    wc.Headers.Add(HttpRequestHeader.ContentType, "application/json; charset=utf-8");
                    Console.WriteLine(request);
                    byte[] responseBytes = wc.UploadData(new Uri("https://api.zonesoft.org/v1.6/Suppliers/getInstances"), "POST", dataBytes);
                    string responseString = Encoding.UTF8.GetString(responseBytes);

                    //Parse the string to a JsonObject
                    JObject element = JObject.Parse(responseString);
                    if (((int)element["Response"]["StatusCode"]) == 200)
                    {
                        Console.WriteLine("Dados do fornecedor obtidos com sucesso");
                        //Get the values and build Product object for each element
                        var array = JArray.Parse(element["Response"]["Content"]["product"].ToString());
                        foreach (var content in array)
                        {
                            var singleSupplier = new Supplier((uint)content["codigo"],
                                                    content["nome"].ToString(),
                                                    content["telefone"].ToString(),
                                                    content["telemovel"].ToString(),
                                                    content["web"].ToString(),
                                                    content["email"].ToString());
                            suppliers.Add(singleSupplier);
                        }
                    }
                    return null;
                }
                catch
                {
                    Console.WriteLine("Falhou obter dados do fornecedor. Verifique conecçao.");
                    this.isAutenticated = false;
                    return null;
                }
            }
            return null;
        }

        //-----------------------------------------------------------

        public string getSupplierNameWithId(int id) => (from item in suppliers where item.GetId() == id select item.GetName()).FirstOrDefault();

        //-----------------------------------------------------------

        public List<string> GetStores()
        {
            if (isAutenticated)
            {
                try
                {
                    List<Product> products = new List<Product>();
                    String request = "{\"auth_hash\":\"" + hash + "\"}";
                    byte[] dataBytes = Encoding.UTF8.GetBytes(request);
                    WebClient wc = new WebClient();
                    wc.Headers.Add(HttpRequestHeader.ContentType, "application/json; charset=utf-8");
                    Console.WriteLine(request);
                    byte[] responseBytes = wc.UploadData(new Uri("https://api.zonesoft.org/v1.6/Stores/getInstances"), "POST", dataBytes);
                    string responseString = Encoding.UTF8.GetString(responseBytes);

                    //Parse the string to a JsonObject
                    JObject element = JObject.Parse(responseString);
                    Console.WriteLine("\n\n\n\n AQUI \n\n\n");
                    Console.WriteLine(element);
                    if (((int)element["Response"]["StatusCode"]) == 200)
                    {
                        Console.WriteLine("Loja obtida com sucesso");
                        //Get the values and build Product object for each element
                        var array = JArray.Parse(element["Response"]["Content"]["product"].ToString());
                        Console.WriteLine(array);
                        foreach (var content in array)
                        {
                            Store singleProduct = new Store((uint)content["codigo"],
                                                    content["descricao"].ToString(),
                                                    content["designacao"].ToString(),
                                                    content["morada"].ToString());

                            stores.Add(singleProduct);
                        }
                        return null;

                    }
                    return null;
                }
                catch
                {
                    Console.WriteLine("Falhou obter lojas.Verifique conecçao.");
                    this.isAutenticated = false;
                    return null;
                }
            }
            return null;
        }

        //-----------------------------------------------------------

        public int GetStoresCount() => stores.Count;

        //-----------------------------------------------------------

        public string GetStockForStoresWithProductCode(string productCode)
        {
            if (isAutenticated)
            {
                try
                {
                    String request = "{\"auth_hash\":\"" + hash + "\",\"productstock\":{\"produto\":\"" + productCode + "\"}}";
                    Console.WriteLine(request);
                    byte[] dataBytes = Encoding.UTF8.GetBytes(request);
                    WebClient wc = new WebClient();
                    wc.Headers.Add(HttpRequestHeader.ContentType, "application/json; charset=utf-8");
                    Console.WriteLine(request);
                    byte[] responseBytes = wc.UploadData(new Uri("https://api.zonesoft.org/v1.5/ProductStocks/getCurrentStockInStores"), "POST", dataBytes);
                    string responseString = Encoding.UTF8.GetString(responseBytes);

                    //Parse the string to a JsonObject
                    var element = JObject.Parse(responseString);
                    if (((int)element["Response"]["StatusCode"]) == 200)
                    {
                        Console.WriteLine("Produto obtido com sucesso");
                        //Get the other values
                        var container = (element["Response"]["Content"]["productstock"]);
                        string stock = container[0]["stock"].ToString();
                        return stock;

                    }
                    return null;
                }
                catch
                {
                    Console.WriteLine("Falhou o acesso a internet.Verifique conecçao.");
                    this.isAutenticated = false;
                    return null;
                }


            }
            return null;
        }
    }
    //End of Class declaration
}
