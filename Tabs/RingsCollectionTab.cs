using StardewValley.ItemTypeDefinitions;
using StardewValley;
using StardewValley.Menus;
using System.Data;
using StardewValley.Tools;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using StardewValley.Objects;
using System.Reflection;

namespace CollectionsMod
{
    internal class RingsCollectionTab : CollectionTab
    {
        public override bool Enabled { get => Config.showRingsTab; }
        public RingsCollectionTab(IMonitor monitor, IModHelper helper, ModConfig config) : base(monitor, helper, config)
        {
            this.name = "Rings";
            this.TabID = 5663;
            this.Icon = Helper.ModContent.Load<Texture2D>(getIconFolderPath() + "RingsCursor.png");
            this.excludeSet = config.excludedRingIDs;
            this.customOrder = config.customRingOrder;
        }
        public override void createDescription(CollectionsPage _instance, string id)
        {
            FieldInfo fieldInfo = typeof(CollectionsPage).GetField("hoverItem", BindingFlags.NonPublic | BindingFlags.Instance);
            string itemID = ItemRegistry.GetMetadata(id).LocalItemId;

            fieldInfo.SetValue(_instance, new Ring(itemID));
        }

        public override List<ParsedItemData> GenerateItems(CollectionsPage _instance)
        {
            //Remove Wedding Ring in singleplayer
            bool isSingleplayer = !(Game1.IsMultiplayer || Game1.IsClient || Game1.IsServer);

            ObjectDataDefinition definition = new ObjectDataDefinition();
            IEnumerable<string> orderedData = definition.GetAllIds()
                .Where(entry => !excludeSet.Contains(entry) && ItemRegistry.GetData(entry).Category == -96)
                .Where(entry => !(isSingleplayer && entry == "801"))
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
