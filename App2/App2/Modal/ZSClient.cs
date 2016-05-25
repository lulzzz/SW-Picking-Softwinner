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


namespace ZSProduct
{
    public class ZSClient
    {
        public string hash { get; set; }
        bool _isAutenticated;
        string username;
        string password;
        string nif;
        int store;
        private List<Product> productInStore;
        private List<Store> stores;
        private List<Supplier> suppliers;
        Product product;

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
                    this._isAutenticated = true;
                    Console.WriteLine(this.hash);
                    return true;
                }
                else
                    return false;
            }
            catch
            {
                Console.WriteLine("Falhou login.Verifique conecçao.");
                this._isAutenticated = false;
                return false;
            }


        }

        //-----------------------------------------------------------

        public string GetProductCode(string barCode)
        {
            if (_isAutenticated)
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
                    this._isAutenticated = false;
                    return null;
                }
            }
            return null;
        }

        //-----------------------------------------------------------

        public Product GetProductWithBarCode(string barCode) => productInStore.FirstOrDefault(item => item.GetBarCode() == barCode);

        //-----------------------------------------------------------

        public List<string> GetProducts()
        {
            if (_isAutenticated)
            {
                try
                {
                    var continuar = true;
                    var val = 0;
                    var nRespostas = 0;
                    do
                    {
                        Console.WriteLine("\n\n\n\nval= " + val + "\n\n\n\n");
                        var request = "{\"auth_hash\":\"" + hash + "\",\"product\":{\"offset\":\"" + val + "\"}}";
                        var dataBytes = Encoding.UTF8.GetBytes(request);
                        var wc = new WebClient();
                        wc.Headers.Add(HttpRequestHeader.ContentType, "application/json; charset=utf-8");
                        Console.WriteLine(request);
                        var responseBytes = wc.UploadData(new Uri("https://api.zonesoft.org/v1.6/Products/getInstances"), "POST", dataBytes);
                        var responseString = Encoding.UTF8.GetString(responseBytes);

                        //Parse the string to a JsonObject
                        var element = JObject.Parse(responseString);
                        if (((int)element["Response"]["StatusCode"]) == 200)
                        {
                            Console.WriteLine("Produto obtido com sucesso");
                            //Get the values and build Product object for each element
                            var array = JArray.Parse(element["Response"]["Content"]["product"].ToString());
                            Console.WriteLine(array);
                            foreach (var content in array)
                            {
                                var singleProduct = new Product(content["descricaocurta"].ToString(),
                                    (uint)content["codigo"],
                                    (uint)content["loja"],
                                    (uint)content["qtdstock"],
                                    (string)content["codbarras"],
                                    (double)content["precovenda"],
                                    (double)content["pvp2"],
                                    (string)content["referencia"],
                                    (string)content["ultprecocompra"]);
                                //Console.WriteLine("Código: " + content["codigo"] + ": \n" + "Loja: " + content["loja"] +
                                //                  "\nStock: " + content["qtdstock"] + "\nCod. Barras : " +
                                //                  content["codbarras"] + "\npreco venda: " + content["precovenda"] +
                                //                  "\n pvp2: " + content["pvp2"]);
                                productInStore.Add(singleProduct);
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
                }
                catch
                {
                    Console.WriteLine("Falhou obter producto.Verifique conecçao.");
                    _isAutenticated = false;
                    return null;
                }
            }
            return null;
        }

        //-----------------------------------------------------------

        public int GetProductsCount() => productInStore.Count;

        //-----------------------------------------------------------

        public string GetSuppliers()
        {
            if (_isAutenticated)
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
                    this._isAutenticated = false;
                    return null;
                }
            }
            return null;
        }

        //-----------------------------------------------------------

        public string GetSupplierNameWithId(int id) => (from item in suppliers where item.GetId() == id select item.GetName()).FirstOrDefault();

        //-----------------------------------------------------------

        public List<string> GetStores()
        {
            if (_isAutenticated)
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
                    this._isAutenticated = false;
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
            if (_isAutenticated)
            {
                try
                {
                    var request = "{\"auth_hash\":\"" + hash + "\",\"productstock\":{\"produto\":\"" + productCode + "\"}}";
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
                    this._isAutenticated = false;
                    return null;
                }


            }
            return null;
        }

        //-----------------------------------------------------------

        public Product GetProductDetailsWithBarCode(string barCode)
        {
            var request = "{\"auth_hash\":\"" + hash + "\",\"product\":{\"condition\":\"" + "codbarras=\'" + barCode + "\'\"}}\'";
            var dataBytes = Encoding.UTF8.GetBytes(request);
            var wc = new WebClient();
            wc.Headers.Add(HttpRequestHeader.ContentType, "application/json; charset=utf-8");
            Console.WriteLine(request);
            var responseBytes = wc.UploadData(new Uri("https://api.zonesoft.org/v1.6/Products/getInstances"), "POST", dataBytes);
            var responseString = Encoding.UTF8.GetString(responseBytes);

            //Parse the string to a JsonObject
            var element = JObject.Parse(responseString);
            if (((int)element["Response"]["StatusCode"]) == 200)
            {
                Console.WriteLine("Produto obtido com sucesso");
                //Get the values and build Product object for each element
                var array = JArray.Parse(element["Response"]["Content"]["product"].ToString());
                Console.WriteLine(array);
                product = new Product(element["Content"]["product"][0]["descricaocurta"].ToString(),
                    (uint)element["Content"]["product"][0]["codigo"],
                    (uint)element["Content"]["product"][0]["loja"],
                    (uint)element["Content"]["product"][0]["qtdstock"],
                    (string)element["Content"]["product"][0]["codbarras"],
                    (double)element["Content"]["product"][0]["precovenda"],
                    (double)element["Content"]["product"][0]["pvp2"],
                    (string)element["Content"]["product"][0]["referencia"],
                    (string)element["Content"]["product"][0]["ultprecocompra"]);
            }
            return product;
        }

        //-----------------------------------------------------------

        public int TotalStores()
        {
            var continuar = true;
            var nLojas = 0;
            do
            {
                var request = "{\"auth_hash\":\"" + hash + "\",\"store\":{\"codigo\":\"" + nLojas + "\"}}";
                var dataBytes = Encoding.UTF8.GetBytes(request);
                var wc = new WebClient();
                wc.Headers.Add(HttpRequestHeader.ContentType, "application/json; charset=utf-8");
                var responseBytes = wc.UploadData(new Uri("https://api.zonesoft.org/v1.6/Stores/getInstance"), "POST", dataBytes);
                var responseString = Encoding.UTF8.GetString(responseBytes);
                var element = JObject.Parse(responseString);

                if ((int)element["Response"]["StatusCode"] != 200) continue;
                if ((string)element["Response"]["Content"]["store"]["descricao"] == "")
                    continuar = false;
                nLojas++;
                Thread.Sleep(500);
            } while (continuar);
            return nLojas - 2; //Shop 0 and last store are added, but do not count
        }
    }
}