using System.Collections.Generic;

namespace ZSProduct.Modal
{
    public class AddProducttoListView : Product
    {
        public double Qtd;

        public AddProducttoListView(string description, string productCode, Dictionary<string, int> stock, string barCode, double price, double pvp2, string reference, string supplier, string pcu, double qtd) : base(description, productCode, stock, barCode, price, pvp2, reference, supplier, pcu)
        {
            Qtd = qtd;
        }
    }
}