namespace App2.Modal
{
    internal class AddItemtoStockListView
    {
        public string Descricao;
        public int Stock;

        public AddItemtoStockListView(string descricao, int stock)
        {
            Descricao = descricao;
            Stock = stock;
        }
    }
}