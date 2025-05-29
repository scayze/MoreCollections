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
    internal class BootsCollectionTab : CollectionTab
    {
        public override bool Enabled { get => Config.showBootsTab; }
        public BootsCollectionTab(IMonitor monitor, IModHelper helper, ModConfig config) : base(monitor, helper, config)
        {
            name = "Boots";
            this.TabID = 5662;
            this.Icon = Helper.ModContent.Load<Texture2D>(getIconFolderPath() + "BootsCursor.png");
            this.excludeSet = config.excludedShoeIDs;
            this.customOrder = config.customShoeOrder;
        }

        public override void createDescription(CollectionsPage _instance, string id)
        {
            FieldInfo fieldInfo = typeof(CollectionsPage).GetField("hoverItem", BindingFlags.NonPublic | BindingFlags.Instance);
            string itemID = ItemRegistry.GetMetadata(id).LocalItemId;

            fieldInfo.SetValue(_instance, new Boots(itemID));
        }

        public override List<ParsedItemData> GenerateItems(CollectionsPage _instance)
        {
            BootsDataDefinition definition = new BootsDataDefinition();
            IEnumerable<string> orderedData = definition.GetAllIds()
                .Where(entry => !excludeSet.Contains(entry))
                .Where(entry => !(!Config.showHeartEventItems && entry == "804"))
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
