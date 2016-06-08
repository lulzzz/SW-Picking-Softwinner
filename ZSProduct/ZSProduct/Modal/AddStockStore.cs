namespace ZSProduct.Modal
{
    public class AddStockStore
    {
        public string Loja;
        public string Stock;

        public AddStockStore(string stock, string loja)
        {
            Loja = loja;
            Stock = stock;
        }
    }
}