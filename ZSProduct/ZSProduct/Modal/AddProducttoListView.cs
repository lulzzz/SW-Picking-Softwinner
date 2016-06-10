namespace ZSProduct.Modal
{
    public class AddProducttoListView : Product
    {
        public int qtd;

        public AddProducttoListView(string description, string productCode, uint store, uint stock, string barCode, double price, double pvp2, string reference, uint supplier, string pcu, int Qtd) : base(description, productCode, store, stock, barCode, price, pvp2, reference, supplier, pcu)
        {
            qtd = Qtd;
        }
    }
}