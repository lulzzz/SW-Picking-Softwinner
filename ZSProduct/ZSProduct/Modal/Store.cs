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

namespace ZSProduct
{
	public class Store
	{
		public uint code;
		public string description;
		public string name;
		public string address;

		public Store (uint code, string description, string name, string address)
		{
			this.code = code;
			this.description = description;
			this.name = name;
			this.address = address;
		}

		public uint GetCode () => code;

		public string GetDescription () => description;

		public string GetName () => name;

		public string GetAddress () => address;
	}
}