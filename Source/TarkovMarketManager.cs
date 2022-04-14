using System;
using System.Collections.ObjectModel;
using System.Net;
using System.Text.Json;

namespace eft_dma_radar
{
    internal static class TarkovMarketManager
    {
        /// <summary>
        /// Minimum value (roubles) of loot to show.
        /// Set via Config.json
        /// </summary>
        private static readonly int MinLootValue = 0;
        /// <summary>
        /// Minimum value (roubles) of important loot to show. Important loot is emphasized with a blue icon/text.
        /// Set via Config.json
        /// </summary>
        private static readonly int MinImportantLootValue = 0;
        /// <summary>
        /// Unaltered loot list from Tarkov Market.
        /// </summary>
        public static ReadOnlyCollection<TarkovMarketItem> AllLoot { get; }
        /// <summary>
        /// Loot item filter to display valuable items.
        /// </summary>
        public static ReadOnlyDictionary<string, LootItem> ItemFilter { get; }

        #region Static_Constructor
        static TarkovMarketManager()
        {
            var itemFilter = new Dictionary<string, LootItem>();
            var config = Program.Config; // get ref to config
            MinLootValue = config.MinLootValue;
            MinImportantLootValue = config.MinImportantLootValue;
            var marketItems = new List<TarkovMarketItem>();
            // Get Market Loot
            if (!File.Exists("market.json") ||
            File.GetLastWriteTime("market.json").AddHours(24) < DateTime.Now) // only update every 24h
            {
#pragma warning disable SYSLIB0014 // Type or member is obsolete
                using (WebClient client = new WebClient())
                {
                    WebRequest request = WebRequest.Create(@"https://market_master.filter-editor.com/data/marketData_en.json");
#pragma warning restore SYSLIB0014 // Type or member is obsolete
                    using WebResponse response = request.GetResponse();
                    string json = client.DownloadString(response.ResponseUri);
                    marketItems = JsonSerializer.Deserialize<List<TarkovMarketItem>>(json);
                    File.WriteAllText("market.json", json);
                }
            }
            else
            {
                var json = File.ReadAllText("market.json");
                marketItems = JsonSerializer.Deserialize<List<TarkovMarketItem>>(json);
            }
            if (marketItems is not null)
            {
                marketItems = marketItems.Where(x => x.isFunctional).ToList(); // Filter list to functional only
                var allLoot = new List<TarkovMarketItem>(marketItems); // save a copy to ref when needed
                AllLoot = new(allLoot); // move to readonly collection
            }
            else throw new NullReferenceException("Market Data is null!");
            // Get Manual "Important" loot
            if (File.Exists("importantLoot.txt")) // Each line contains a BSG ID (item id) and nothing else
            {
                foreach (var i in File.ReadAllLines("importantLoot.txt"))
                {
                    var id = i.Split('#')[0].Trim(); // strip # comment char
                    var marketItem = marketItems.FirstOrDefault(x => x.bsgId.Equals(id));
                    if (marketItem is not null)
                    {
                        itemFilter.TryAdd(id, new LootItem()
                        {
                            Label = $"!!{marketItem.shortName}",
                            Important = true
                        });
                    }
                }
            }
            else
            {
                File.WriteAllText("importantLoot.txt", "bsgID ## comment(s) go here, one entry per line");
            }
            // Filter list down further 
            marketItems = marketItems.Where(x => x.avg24hPrice >= MinLootValue || x.traderPrice >= MinLootValue).ToList();
            foreach (var item in marketItems) // Add rest of loot to filter
            {
                var value = GetItemValue(item);
                itemFilter.TryAdd(item.bsgId, new LootItem()
                {
                    Label = $"[{FormatNumber(value)}] {item.shortName}",
                    Important = IsItemImportant(value)
                });
            }
            ItemFilter = new(itemFilter); // update readonly ref
        }
        #endregion

        #region Methods

        private static string FormatNumber(int num)
        {
            if (num >= 1000000)
                return (num / 1000000D).ToString("0.##") + "M";
            else if (num >= 1000)
                return (num / 1000D).ToString("0") + "K";

            else return num.ToString();
        }

        private static int GetItemValue(TarkovMarketItem item)
        {
            if (item.avg24hPrice > item.traderPrice)
                return item.avg24hPrice;
            else
                return item.traderPrice;
        }

        private static bool IsItemImportant(int value)
        {
            if (value >= MinImportantLootValue) return true;
            else return false;
        }
        #endregion
    }

    #region Classes
    /// <summary>
    /// Class JSON Representation of Tarkov Market Data.
    /// </summary>
    public class TarkovMarketItem
    {
        public string uid { get; set; }
        public string name { get; set; }
        public List<string> tags { get; set; }
        public string shortName { get; set; }
        public int price { get; set; }
        public int basePrice { get; set; }
        public int avg24hPrice { get; set; }
        public int avg7daysPrice { get; set; }
        public string traderName { get; set; }
        public int traderPrice { get; set; }
        public string traderPriceCur { get; set; }
        public DateTime updated { get; set; }
        public int slots { get; set; }
        public double diff24h { get; set; }
        public double diff7days { get; set; }
        public string icon { get; set; }
        public string link { get; set; }
        public string wikiLink { get; set; }
        public string img { get; set; }
        public string imgBig { get; set; }
        public string bsgId { get; set; }
        public bool isFunctional { get; set; }
        public string reference { get; set; }
        public string apiKey { get; set; }
    }
    #endregion
}