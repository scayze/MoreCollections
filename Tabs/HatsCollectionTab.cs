using StardewValley.ItemTypeDefinitions;
using StardewValley;
using StardewValley.Menus;
using System.Data;
using StardewValley.Tools;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using Microsoft.Xna.Framework;
using StardewValley.Objects;
using System.Reflection;

namespace CollectionsMod
{
    internal class HatsCollectionTab : CollectionTab
    {
        public override bool Enabled { get => Config.showHatsTab; }
        public HatsCollectionTab(IMonitor monitor, IModHelper helper, ModConfig config) : base(monitor, helper, config)
        {
            name = "Hats";
            this.TabID = 5659;
            this.Icon = Helper.ModContent.Load<Texture2D>("assets/LooseSprites/HatsCursor.png");
            this.excludeSet = config.excludedHatIDs;
            this.customOrder = config.customHatOrder;

            this.maxColumns = 9;
            this.iconWidth = 75;
            this.iconHeight = 80;
        }

        public override void createDescription(CollectionsPage _instance, string id)
        {
            FieldInfo fieldInfo = typeof(CollectionsPage).GetField("hoverItem", BindingFlags.NonPublic | BindingFlags.Instance);
            string itemID = ItemRegistry.GetMetadata(id).LocalItemId;

            fieldInfo.SetValue(_instance, new Hat(itemID));
        }

        public override List<ParsedItemData> GenerateItems(CollectionsPage _instance)
        {
            HatDataDefinition definition = new HatDataDefinition();
            IEnumerable<string> orderedData = definition.GetAllIds()
                .Where(entry => !excludeSet.Contains(entry))
                .Where(entry => !(!Config.showHeartEventItems && entry == "41"))
                .OrderBy(entry =>
                {
                    int index = customOrder.IndexOf(entry);
                    return index == -1 ? int.MaxValue : index;
                }
            );

            List<ParsedItemData> data = orderedData.Select(s => definition.GetData(s)).ToList();
            return data;
        }
    }
}
