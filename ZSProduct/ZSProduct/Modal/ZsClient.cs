using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Newtonsoft.Json.Linq;

//-----------------------------------------------------------
namespace ZSProduct.Modal
{
    internal class ZsClient
    {
        //-----------------------------------------------------------
        public string Hash { get; set; }
        public bool IsAutenticated;
        public string Username;
        public string Password;
        public string Nif;
        public int Store;
        public List<Product> ProductInStore;
        public List<Store> Stores;

        //-----------------------------------------------------------
        private List<Supplier> _suppliers;
        private Product _product;
        private string _request;
        private byte[] _dataBytes;
        private WebClient _wc;
        private byte[] _responseBytes;
        private string _responseString;
        private JObject _element;

        //-----------------------------------------------------------
        //Constructor
        public ZsClient(string username, string password, int store, string nif)
        {
            Username = username;
            Password = password;
            Store = store;
            Nif = nif;
            ProductInStore = new List<Product>();
            Stores = new List<Store>();
        }


        //-----------------------------------------------------------
        //Do login
        public bool Login()
        {
            try
            {
                _request = "{\"user\":{\"nif\":\"" + Nif + "\",\"nome\":\"" + Username + "\",\"password\":\"" + Password + "\",\"loja\":\"" + Store + "\"}}";
                _dataBytes = Encoding.UTF8.GetBytes(_request);
                _wc = new WebClient();
                _wc.Headers.Add(HttpRequestHeader.ContentType, "application/json; charset=utf-8");
                Console.WriteLine("LOGIN REQUEST: " + _request);
                _responseBytes = _wc.UploadData(new Uri("https://api.zonesoft.org/v1.5/auth/authenticate/"), "POST", _dataBytes);
                _responseString = Encoding.UTF8.GetString(_responseBytes);

                //Parse the string to a JsonObject
                _element = JObject.Parse(_responseString);
                if ((int)_element["Response"]["StatusCode"] == 200)
                {
                    Console.WriteLine("Autenticado com sucesso");
                    //Get the other values
                    Hash = _element["Response"]["Content"]["auth_hash"].ToString();
                    IsAutenticated = true;
                    Console.WriteLine("HASH LOGIN: " + Hash);
                    return true;
                }
                return false;
            }
            catch
            {
                Console.WriteLine("Falhou login. Verifique conecção.");
                IsAutenticated = false;
                return false;
            }


        }

        //-----------------------------------------------------------

        public int GetProductsCount() => ProductInStore.Count;

        //-----------------------------------------------------------
        /*public string GetSuppliers()
        {
            if (IsAutenticated)
            {
                try
                {
                    string request = "{\"auth_hash\":\"" + Hash + "\"}";
                    byte[] dataBytes = Encoding.UTF8.GetBytes(request);
                    WebClient wc = new WebClient();
                    wc.Headers.Add(HttpRequestHeader.ContentType, "application/json; charset=utf-8");
                    Console.WriteLine(request);
                    byte[] responseBytes = wc.UploadData(
                                               new Uri("https://api.zonesoft.org/v1.6/Suppliers/getInstances"), "POST", dataBytes);
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
                            _suppliers.Add(singleSupplier);
                        }
                    }
                    return null;
                }
                catch
                {
                    Console.WriteLine("Falhou obter dados do fornecedor. Verifique conecçao.");
                    this.IsAutenticated = false;
                    return null;
                }
            }
            return null;
        }*/

        //-----------------------------------------------------------

        public string GetSupplierNameWithId(int id) => (from item in _suppliers where item.Id == id select item.Name).FirstOrDefault();

        //-----------------------------------------------------------
        public List<string> GetStores()
        {
            if (IsAutenticated)
            {
                try
                {
                    _request = "{\"auth_hash\":\"" + Hash + "\"}";
                    _dataBytes = Encoding.UTF8.GetBytes(_request);
                    _wc = new WebClient();
                    _wc.Headers.Add(HttpRequestHeader.ContentType, "application/json; charset=utf-8");
                    Console.WriteLine("GET STORES: " + _request);
                    _responseBytes = _wc.UploadData(new Uri("https://api.zonesoft.org/v1.6/Stores/getInstances"), "POST", _dataBytes);
                    _responseString = Encoding.UTF8.GetString(_responseBytes);
                    //Parse the string to a JsonObject
                    _element = JObject.Parse(_responseString);
                    //Console.WriteLine(element);
                    if ((int)_element["Response"]["StatusCode"] == 200)
                    {
                        Console.WriteLine("Loja obtida com sucesso");
                        //Get the values and build Product object for each element
                        var array = JArray.Parse(_element["Response"]["Content"]["product"].ToString());
                        foreach (var content in array)
                        {
                            var singleProduct = new Store((uint)content["codigo"],
                                                      content["descricao"].ToString(),
                                                      content["designacao"].ToString(),
                                                      content["morada"].ToString());

                            Stores.Add(singleProduct);
                        }
                        return null;

                    }
                    return null;
                }
                catch
                {
                    Console.WriteLine("Falhou obter lojas.Verifique conecçao.");
                    IsAutenticated = false;
                    return null;
                }
            }
            return null;
        }

        //-----------------------------------------------------------
        public Dictionary<string, int> GetStockInStoresWithProductCode(string productCode)
        {
            if (IsAutenticated)
            {
                try
                {
                    var stockInStores = new Dictionary<string, int>();
                    Console.WriteLine(productCode);
                    //var stockContainer = new List<string> ();
                    var request = "{\"auth_hash\":\"" + Hash + "\",\"productstock\":{\"produto\":\"" + productCode + "\"}}";
                    Console.WriteLine(request);
                    byte[] dataBytes = Encoding.UTF8.GetBytes(request);
                    WebClient wc = new WebClient();
                    wc.Headers.Add(HttpRequestHeader.ContentType, "application/json; charset=utf-8");
                    Console.WriteLine(request);
                    byte[] responseBytes =
                        wc.UploadData(new Uri("https://api.zonesoft.org/v1.6/ProductStocks/getCurrentStockInStores"),
                            "POST", dataBytes);
                    string responseString = Encoding.UTF8.GetString(responseBytes);

                    //Parse the string to a JsonObject
                    var element = JObject.Parse(responseString);
                    if ((int)element["Response"]["StatusCode"] == 200)
                    {
                        Console.WriteLine("Stock obtido com sucesso");
                        //Get the other values
                        var container = element["Response"]["Content"]["productstock"];
                        //Loop all the values to get the stock
                        foreach (var singleStore in container)
                        {
                            int stockProd;
                            try
                            {
                                stockProd = (int)singleStore["stock"];
                            }
                            catch (Exception)
                            {
                                stockProd = 0;
                            }
                            stockInStores.Add(singleStore["loja"].ToString(), stockProd);
                        }
                        return stockInStores;

                    }
                    return null;
                }
                catch
                {
                    Console.WriteLine("Produto sem stock");
                    IsAutenticated = false;
                    return null;
                }


            }
            return null;
        }

        //-----------------------------------------------------------
        public Product GetProductWithBarCode(string barCode)
        {
            if (IsAutenticated)
            {
                try
                {
                    _product = null;
                    _request = "{\"auth_hash\":\"" + Hash + "\",\"product\":{\"condition\":\"codbarras='" + barCode + "'\"}}";
                    Console.WriteLine("\n\n\n" + _request);
                    Console.WriteLine("GET PRODUCT WITH BAR CODE: " + _request);
                    _dataBytes = Encoding.UTF8.GetBytes(_request);
                    _wc = new WebClient();
                    _wc.Headers.Add(HttpRequestHeader.ContentType, "application/json; charset=utf-8");
                    _responseBytes = _wc.UploadData(new Uri("https://api.zonesoft.org/v1.6/Products/getInstances"), "POST", _dataBytes);
                    _responseString = Encoding.UTF8.GetString(_responseBytes);
                    if (_responseString != "")
                    {
                        var element = JObject.Parse(_responseString);
                        Console.WriteLine(element);
                        if ((int)element["Response"]["StatusCode"] == 200)
                        {
                            Console.WriteLine("Produto obtido com sucesso");
                            //Get the values and build Product object for each element
                            var array = JArray.Parse(element["Response"]["Content"]["product"].ToString());
                            _product = new Product((string)array[0]["descricao"],
                                (string)array[0]["codigo"],
                                null,
                                (string)array[0]["codbarras"],
                                (double)array[0]["precovenda"],
                                (double)array[0]["pvp2"],
                                (string)array[0]["referencia"],
                                (string)array[0]["fornecedor"],
                                (string)array[0]["ultprecocompra"]);
                            return _product;
                        }
                        return null;
                    }
                    return null;
                }
                catch
                {
                    Console.WriteLine("Produto sem stock");
                    IsAutenticated = false;
                    return null;
                }
            }
            return null;
        }

        //-----------------------------------------------------------
        public Product GetProductWithCode(string code)
        {
            if (IsAutenticated)
            {
                try
                {
                    _product = null;
                    _request = "{\"auth_hash\":\"" + Hash + "\",\"product\":{\"codigo\":\"" + code + "\"}}";
                    Console.WriteLine("GET PRODUCT WITH CODE: " + _request);
                    _dataBytes = Encoding.UTF8.GetBytes(_request);
                    _wc = new WebClient();
                    _wc.Headers.Add(HttpRequestHeader.ContentType, "application/json; charset=utf-8");
                    _responseBytes = _wc.UploadData(new Uri("https://api.zonesoft.org/v1.5/Products/getInstance"),
                        "POST", _dataBytes);
                    _responseString = Encoding.UTF8.GetString(_responseBytes);
                    if (_responseString != "")
                    {
                        var element = JObject.Parse(_responseString);
                        Console.WriteLine(element);
                        if ((int)element["Response"]["StatusCode"] == 200)
                        {
                            Console.WriteLine("Produto obtido com sucesso");
                            //Get the values and build Product object for each element
                            //var array = JArray.Parse(element["Response"]["Content"]["product"].ToString());
                            _product = new Product((string)element["Response"]["Content"]["product"]["descricao"],
                                (string)element["Response"]["Content"]["product"]["codigo"],
                                null,
                                (string)element["Response"]["Content"]["product"]["codbarras"],
                                (double)element["Response"]["Content"]["product"]["precovenda"],
                                (double)element["Response"]["Content"]["product"]["pvp2"],
                                (string)element["Response"]["Content"]["product"]["referencia"],
                                (string)element["Response"]["Content"]["product"]["fornecedor"],
                                (string)element["Response"]["Content"]["product"]["ultprecocompra"]);
                            return _product;
                        }
                        return null;
                    }
                }
                catch
                {
                    Console.WriteLine("Produto sem stock");
                    IsAutenticated = false;
                    return null;
                }
            }
            return null;
        }

        //-----------------------------------------------------------
        public Product GetProductWithReference(string reference)
        {
            try
            {
                _product = null;
                _request = "{\"auth_hash\":\"" + Hash + "\",\"product\":{\"condition\":\"referencia='" + reference + "'\"}}";
                Console.WriteLine("\n\n\n" + _request);
                Console.WriteLine("GET PRODUCT WITH BAR CODE: " + _request);
                _dataBytes = Encoding.UTF8.GetBytes(_request);
                _wc = new WebClient();
                _wc.Headers.Add(HttpRequestHeader.ContentType, "application/json; charset=utf-8");
                _responseBytes = _wc.UploadData(new Uri("https://api.zonesoft.org/v1.6/Products/getInstances"), "POST", _dataBytes);
                _responseString = Encoding.UTF8.GetString(_responseBytes);
                if (_responseString != "")
                {
                    var element = JObject.Parse(_responseString);
                    Console.WriteLine(element);
                    if ((int)element["Response"]["StatusCode"] == 200)
                    {
                        Console.WriteLine("Produto obtido com sucesso");
                        //Get the values and build Product object for each element
                        var array = JArray.Parse(element["Response"]["Content"]["product"].ToString());
                        _product = new Product((string)array[0]["descricao"],
                            (string)array[0]["codigo"],
                            null,
                            (string)array[0]["codbarras"],
                            (double)array[0]["precovenda"],
                            (double)array[0]["pvp2"],
                            (string)array[0]["referencia"],
                            (string)array[0]["fornecedor"],
                            (string)array[0]["ultprecocompra"]);
                        return _product;
                    }
                    return null;
                }
                return null;
            }
            catch (Exception)
            {
                IsAutenticated = false;
                return null;
            }
        }

        //-----------------------------------------------------------
        public int StoreCount()
        {
            try
            {
                if (IsAutenticated)
                {
                    var continuar = true;
                    var nLojas = 0;
                    do
                    {
                        var request = "{\"auth_hash\":\"" + Hash + "\",\"store\":{\"codigo\":\"" + nLojas + "\"}}";
                        var dataBytes = Encoding.UTF8.GetBytes(request);
                        var wc = new WebClient();
                        wc.Headers.Add(HttpRequestHeader.ContentType, "application/json; charset=utf-8");
                        var responseBytes = wc.UploadData(new Uri("https://api.zonesoft.org/v1.6/Stores/getInstance"), "POST", dataBytes);
                        var responseString = Encoding.UTF8.GetString(responseBytes);
                        var element = JObject.Parse(responseString);

                        if ((int)element["Response"]["StatusCode"] != 200)
                            continue;
                        if ((string)element["Response"]["Content"]["store"]["descricao"] == "")
                            continuar = false;
                        nLojas++;
                    } while (continuar);
                    return nLojas - 2; //Shop 0 and last store are added, but do not count
                }
                return 0;
            }
            catch (Exception)
            {
                IsAutenticated = false;
                return 0;
            }
        }

        //-----------------------------------------------------------
        public List<Store> GetStoresList()
        {
            try
            {
                var list = new List<Store>();
                for (uint i = 0; i <= StoreCount(); i++)
                {
                    _request = "{\"auth_hash\":\"" + Hash + "\",\"store\":{\"codigo\":\"" + i + "\"}}";
                    Console.WriteLine("GET STORES LIST: " + _request);
                    _dataBytes = Encoding.UTF8.GetBytes(_request);
                    _wc = new WebClient();
                    _wc.Headers.Add(HttpRequestHeader.ContentType, "application/json; charset=utf-8");
                    _responseBytes = _wc.UploadData(new Uri("https://api.zonesoft.org/v1.6/Stores/getInstance"), "POST", _dataBytes);
                    _responseString = Encoding.UTF8.GetString(_responseBytes);
                    if (_responseString != "")
                    {
                        _element = JObject.Parse(_responseString);
                        if ((int)_element["Response"]["StatusCode"] == 200)
                        {
                            //Get the values and build Product object for each element
                            var store = new Store((uint)_element["Response"]["Content"]["store"]["codigo"],
                                                  (string)_element["Response"]["Content"]["store"]["descricao"],
                                                  (string)_element["Response"]["Content"]["store"]["designacao"],
                                                  (string)_element["Response"]["Content"]["store"]["morada"]);
                            list.Add(store);
                        }
                    }
                    else
                    {
                        Console.WriteLine("Store without response");
                    }
                }
                return list;
            }
            catch (Exception)
            {
                IsAutenticated = false;
                return null;
            }
        }

        //-----------------------------------------------------------
        public string GetStoreDescription(int storeCode)
        {
            try
            {
                var description = "";
                _request = "{\"auth_hash\":\"" + Hash + "\",\"store\":{\"codigo\":\"" + storeCode + "\"}}";
                Console.WriteLine("GET STORE DESCRIPTION: " + _request);
                _dataBytes = Encoding.UTF8.GetBytes(_request);
                _wc = new WebClient();
                _wc.Headers.Add(HttpRequestHeader.ContentType, "application/json; charset=utf-8");
                _responseBytes = _wc.UploadData(new Uri("https://api.zonesoft.org/v1.6/Stores/getInstance"), "POST", _dataBytes);
                _responseString = Encoding.UTF8.GetString(_responseBytes);
                if (_responseString != "")
                {
                    _element = JObject.Parse(_responseString);
                    if ((int)_element["Response"]["StatusCode"] == 200)
                        description = (string)_element["Response"]["Content"]["store"]["descricao"];
                }
                else
                    Console.WriteLine("Store without response");
                return description;
            }
            catch (Exception)
            {
                IsAutenticated = false;
                return null;
            }
        }

        //-----------------------------------------------------------
        public string GetSupplierNameWithCode(int supplierCode)
        {
            try
            {
                var name = "";
                _request = "{\"auth_hash\":\"" + Hash + "\",\"supplier\":{\"codigo\":\"" + supplierCode + "\"}}";
                Console.WriteLine("GET STORE DESCRIPTION: " + _request);
                _dataBytes = Encoding.UTF8.GetBytes(_request);
                _wc = new WebClient();
                _wc.Headers.Add(HttpRequestHeader.ContentType, "application/json; charset=utf-8");
                _responseBytes = _wc.UploadData(new Uri("https://api.zonesoft.org/v1.6/Suppliers/getInstance"), "POST", _dataBytes);
                _responseString = Encoding.UTF8.GetString(_responseBytes);
                if (_responseString != "")
                {
                    _element = JObject.Parse(_responseString);
                    if ((int)_element["Response"]["StatusCode"] == 200)
                        name = (string)_element["Response"]["Content"]["supplier"]["nome"];
                }
                else
                    Console.WriteLine("Supplier without response");
                return name;
            }
            catch (Exception)
            {
                IsAutenticated = false;
                return null;
            }
        }
    }
}