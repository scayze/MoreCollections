using StardewValley.ItemTypeDefinitions;
using StardewValley;
using StardewValley.Menus;
using System.Data;
using StardewValley.Tools;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using System.Reflection;

namespace CollectionsMod
{
    internal class WeaponCollectionTab : CollectionTab
    {
        public override bool Enabled { get => Config.showWeaponTab; }
        public WeaponCollectionTab(IMonitor monitor, IModHelper helper, ModConfig config) : base(monitor, helper, config)
        {
            this.name = "Weapons";
            this.TabID = 5658;
            this.Icon = Helper.ModContent.Load<Texture2D>(getIconFolderPath() + "WeaponCursor.png");
            this.excludeSet = config.excludedWeaponIDs;
            this.customOrder = config.customWeaponOrder;
        }

        public override void createDescription(CollectionsPage _instance, string id)
        {
            FieldInfo fieldInfo = typeof(CollectionsPage).GetField("hoverItem", BindingFlags.NonPublic | BindingFlags.Instance);
            string itemID = ItemRegistry.GetMetadata(id).LocalItemId;

            fieldInfo.SetValue(_instance, new MeleeWeapon(itemID));

            if (id == "32" || id == "33" || id == "34")
            {
                fieldInfo.SetValue(_instance, new Slingshot(itemID));
            }
        }

        public override List<ParsedItemData> GenerateItems(CollectionsPage _instance)
        {
            WeaponDataDefinition definition = new WeaponDataDefinition();
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
