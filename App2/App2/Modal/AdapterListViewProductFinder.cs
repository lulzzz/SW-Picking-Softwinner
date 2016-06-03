using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Views;
using Android.Widget;
using ZSProduct;

namespace App2.Modal
{
    internal class AdapterListViewProductFinder : BaseAdapter<AddItemtoStockListView>
    {
        private readonly Dictionary<string, int> _stocks;
        private readonly Activity _context;
        private List<AddItemtoStockListView> _list;

        public AdapterListViewProductFinder(Activity context, Dictionary<string, int> stocks)
        {
            _context = context;
            _stocks = stocks;
        }

        public override long GetItemId(int position) => position;

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            //_list.Add(new AddItemtoStockListView());
            for (int i = 0; i < _stocks.Count; i++)
            {
                _list.Add(new AddItemtoStockListView(i.ToString(), Convert.ToInt32(_stocks.ElementAt(i).Value)));
            }

            //_stocks.ElementAt()
            var teste = new Pdt();
            Console.WriteLine("AdapterListView.cs" + teste.Qtd);
            var item = _list[position];
            var view = convertView ?? _context.LayoutInflater.Inflate(Resource.Layout.ListViewRow_ProductFinder, null);
            //Editar campos a mostrar
            view.FindViewById<TextView>(Resource.Id.txtAdapterStore).Text = item.Descricao; //Nome da Loja
            view.FindViewById<TextView>(Resource.Id.txtAdapterStock).Text = item.Stock.ToString(); //Stock
            return view;
        }

        public override int Count => _list.Count;

        public override AddItemtoStockListView this[int pos] => _list[pos];
    }
}