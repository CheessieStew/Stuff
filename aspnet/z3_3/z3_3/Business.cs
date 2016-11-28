using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace z3_3
{
    public class Model
    {
        public List<ShopItem> Items = new List<ShopItem>();
        private int _nextID = 0;
        public int NextId
        {
            get
            {
                _nextID++;
                return _nextID - 1;
            }
        }
        static Random RanGen = new Random();
        public Model()
        {
            for (int i = 0; i < 100; i++)
            {
                ShopItem o = new ShopItem();

                o.ID = NextId;
                o.price = (i % 20 + 3) * RanGen.NextDouble();
                o.name = "item #" + i.ToString();
                o.description = $"this is item #{i} that costs {o.price:0.00}$";
                o.picLink = $"pics\\item{i}.jpg";
                Items.Add(o);
            }
        }

        public static Model Instance
        {
            get
            {
                if (HttpContext.Current.Application["model"] == null)
                {
                    Model model = new Model();
                    HttpContext.Current.Application["model"] = model;
                }

                return (Model)HttpContext.Current.Application["model"];
            }
        }
    }


    public class ShopItem
    {
        public double price
        {
            get;
            set;
        }
        public int ID
        {
            get;
            set;
        }
        public string name
        {
            get;
            set;
        }
        public string picLink
        {
            get;
            set;
        }
        public string description
        {
            get;
            set;
        }

        public string Xml
        {
            get
            {
                return string.Format($"<data><shopItem name=\"{name}\" price=\"{price:s}\"/></data>");
            }
        }

        public void Update()
        {
        }

        public void Delete()
        {
        }
    }

    public class ShopItem_DataProvider
    {
        public ShopItem_DataProvider()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        /// <summary>
        /// Metoda dla źródła danych GridView
        /// 
        /// Parametry OrderBy, StartRow i RowCount zdefiniowane są
        /// w odpowiednich właściwościach źródła danych
        /// </summary>
        /// <param name="OrderBy"></param>
        /// <param name="StartRow"></param>
        /// <param name="RowCount"></param>
        /// <returns></returns>
        public List<ShopItem> Retrieve(string OrderBy, int StartRow, int RowCount)
        {
            if (RowCount > 0)
            {
                var res = Model.Instance.Items;
                switch(OrderBy)
                {
                    case "NAME":
                        res = res.OrderBy(si => si.name).ToList();
                        break;

                    case "PRICE":
                        res = res.OrderBy(si => si.price).ToList();
                        break;

                }
                return res.GetRange(StartRow, Math.Min(RowCount, Model.Instance.Items.Count - StartRow));
            }
            else
                return new List<ShopItem>(new ShopItem[] { Model.Instance.Items[StartRow] });
        }

        /// <summary>
        /// jw
        /// </summary>
        /// <returns></returns>
        public int SelectCount()
        {
            return Model.Instance.Items.Count;
        }

        /// <summary>
        /// Metoda dla źródła danych DetailsView
        /// 
        /// Parametr ID zdefiniowany jest w sekcji SelectParameters źródła danych
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public ShopItem Retrieve(int ID)
        {
            return Model.Instance.Items
                .Find(delegate (ShopItem item) { return item.ID == ID; });
        }

        public ShopItem Insert(ShopItem item)
        {
            // symulacja zapisu do bazy danych, które nadaje ID
            item.ID = Model.Instance.Items.Count + 1;

            Model.Instance.Items.Add(item);

            return item;
        }

        public ShopItem Update(ShopItem item)
        {
            // zmieniam bezpośrednio pola obiektu na liście

            return item;
        }

        public ShopItem Delete(int ID)
        {
            // tu wywołanie metody na bazie danych
            Model.Instance.Items.RemoveAt(Model.Instance.Items.FindIndex(si => si.ID == ID));
            return null;
        }
    }

}