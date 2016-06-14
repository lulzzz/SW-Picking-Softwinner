namespace ZSProduct.Modal
{
    public class AddProducttoListView : Product
    {
        public int Qtd;

        public AddProducttoListView(string description, string productCode, uint store, double stock, string barCode, double price, double pvp2, string reference, uint supplier, string pcu, int qtd) : base(description, productCode, store, stock, barCode, price, pvp2, reference, supplier, pcu)
        {
            Qtd = qtd;
        }
    }
}