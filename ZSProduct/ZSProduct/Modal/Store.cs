namespace ZSProduct.Modal
{
    public class Store
    {
        public uint Code;
        public string Description;
        public string Name;
        public string Address;

        public Store(uint code, string description, string name, string address)
        {
            Code = code;
            Description = description;
            Name = name;
            Address = address;
        }
    }
}