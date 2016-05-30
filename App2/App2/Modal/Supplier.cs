using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace ZSProduct.Modal
{
	public class Supplier
	{
		private uint id;
		private string name;
		private string phone;
		private string mobilePhone;
		private string web;
		private string email;

		public Supplier (uint id, string name, string phone, string mobilePhone, string web, string email)
		{
			this.id = id;
			this.name = name;
			this.phone = phone;
			this.mobilePhone = mobilePhone;
			this.web = web;
			this.email = email;
		}

		public uint GetId () => id;

		public string GetName () => name;

		public string GetPhone () => phone;

		public string GetMobilePhone () => mobilePhone;

		public string GetWeb () => web;

		public string GetEmail () => email;

	}
}