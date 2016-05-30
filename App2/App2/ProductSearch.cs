using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using App2.Modal;
using ZSProduct;

namespace App2
{
	[Activity (Label = "Detalhes de Produto", Icon = "@drawable/icon", Theme = "@style/Theme.AppCompat.Light")]
	public class ProductSearch : AppCompatActivity
	{
		protected override void OnCreate (Bundle savedInstanceState)
		{
			/*
             * MODES:
             *      0 - OFFLINE
             *      1 - ONLINE
             */

			base.OnCreate (savedInstanceState);
			SetContentView (Resource.Layout.ProductSearch);

			var manager = new ZsManager ();
			var nif = manager.GetItem ("nif");
			var username = manager.GetItem ("username");
			var password = manager.GetItem ("password");
			//var mode = manager.GetItem("mode");
			ClearFields ();
			//Testing ...
			//manager.DownloadStoreAsync ();
			//manager.zsClient.Login ();
			//
			//var simpleProduct = manager.zsClient.GetProductWithBarCode ("1000000700497");
			//simpleProduct.printProductDetails ();
			//Product.printProductDetails (simpleProduct);

			var txtCodBarras = FindViewById<EditText> (Resource.Id.txtCodBarras);
			var txtCodigo = FindViewById<TextView> (Resource.Id.txtCodigo);
			var txtReferencia = FindViewById<TextView> (Resource.Id.txtRef);
			var txtDescricao = FindViewById<TextView> (Resource.Id.txtDescricao);
			var txtFornecedor = FindViewById<TextView> (Resource.Id.txtFornecedor);
			var txtPvp1 = FindViewById<TextView> (Resource.Id.txtPvp1);
			var txtPvp2 = FindViewById<TextView> (Resource.Id.txtPvp2);
			var txtPcu = FindViewById<TextView> (Resource.Id.txtPcu);
			var zsHandler = new ZSClient (username, password, 0, nif);
			zsHandler.Login ();
			//-----------------------------------------------------------
			txtCodBarras.TextChanged += (sender, e) => {
				//zsHandler.GetProductDetailsWithBarCode("1000000700497");
				//if (manager.GetItem("mode") == "0")
				//{
				//    if (zsHandler.GetProductWithBarCode(txtCodBarras.Text) != null)
				//    {
				//        zsHandler.GetProducts();
				//        var product = zsHandler.GetProductWithBarCode(txtCodBarras.Text);
				//        txtCodigo.Text = product.GetProductCode().ToString();
				//        txtReferencia.Text = product.GetReference();
				//        txtDescricao.Text = product.GetDescription().ToString();
				//        txtFornecedor.Text = product.GetSupplier().ToString();
				//        txtPvp1.Text = $"?{product.GetPriceOfSale():f2}";
				//        txtPvp2.Text = $"?{product.GetPvp2():f2}";
				//        txtPcu.Text = product.GetPcu();
				//    }
				//    else
				//        ClearFields();
				//}
				//else if (manager.GetItem("mode") == "1")
				//{

				//}
			};
		}

		//-----------------------------------------------------------
		public override bool OnCreateOptionsMenu (IMenu menu)
		{
			menu.Add (new Java.Lang.String ("Configuracoes"));
			menu.Add (new Java.Lang.String ("Test"));
			menu.Add (new Java.Lang.String ("Acerca"));
			return true;
		}


		//-----------------------------------------------------------
		public override bool OnOptionsItemSelected (IMenuItem item)
		{
			switch (item.TitleFormatted.ToString ()) {
			case "Settings":
				var settings = new Intent (this, typeof(Settings));
				StartActivity (settings);
				break;
			case "About":
				var about = new Intent (this, typeof(About));
				StartActivity (about);
				break;
			}
			return true;
		}

		//-----------------------------------------------------------
		private void ClearFields ()
		{
			FindViewById<EditText> (Resource.Id.txtCodBarras).Text = "";
			FindViewById<TextView> (Resource.Id.txtCodigo).Text = "";
			FindViewById<TextView> (Resource.Id.txtRef).Text = "";
			FindViewById<TextView> (Resource.Id.txtDescricao).Text = "";
			FindViewById<TextView> (Resource.Id.txtFornecedor).Text = "";
			FindViewById<TextView> (Resource.Id.txtPvp1).Text = "";
			FindViewById<TextView> (Resource.Id.txtPvp2).Text = "";
		}
	}
}