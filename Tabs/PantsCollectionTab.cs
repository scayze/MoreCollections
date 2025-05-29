using StardewValley.ItemTypeDefinitions;
using StardewValley;
using StardewValley.Menus;
using System.Data;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley.Objects;
using System.Reflection;

namespace CollectionsMod
{
    internal class PantsCollectionTab : CollectionTab
    {
        public override bool Enabled { get => Config.showPantsTab; }
        public PantsCollectionTab(IMonitor monitor, IModHelper helper, ModConfig config) : base(monitor, helper, config)
        {
            name = "Pants";
            this.TabID = 5661;
            this.Icon = Helper.ModContent.Load<Texture2D>(getIconFolderPath() + "PantsCursor.png");
            this.excludeSet = config.excludedPantsIDs;
            this.customOrder = config.customPantsOrder;
        }

        public override void createDescription(CollectionsPage _instance, string id)
        {
            FieldInfo fieldInfo = typeof(CollectionsPage).GetField("hoverItem", BindingFlags.NonPublic | BindingFlags.Instance);
            string itemID = ItemRegistry.GetMetadata(id).LocalItemId;

            fieldInfo.SetValue(_instance, new Clothing(itemID));
        }


        public override List<ParsedItemData> GenerateItems(CollectionsPage _instance)
        {
            PantsDataDefinition definition = new PantsDataDefinition();
            IEnumerable<string> orderedData = definition.GetAllIds()
                .Where(entry => !excludeSet.Contains(entry))
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
