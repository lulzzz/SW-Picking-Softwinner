namespace ZSProduct.Modal
{
    public class Supplier
    {
        public uint Id;
        public string Name;
        public string Phone;
        public string MobilePhone;
        public string Web;
        public string Email;

        public Supplier(uint id, string name, string phone, string mobilePhone, string web, string email)
        {
            Id = id;
            Name = name;
            Phone = phone;
            MobilePhone = mobilePhone;
            Web = web;
            Email = email;
        }

        //public uint GetId () => id;

        //public string GetName () => name;

        //public string GetPhone () => phone;

        //public string GetMobilePhone () => mobilePhone;

        //public string GetWeb () => web;

        //public string GetEmail () => email;

    }
}