using StardewValley.ItemTypeDefinitions;
using StardewValley;
using StardewValley.Menus;
using System.Data;
using StardewValley.Tools;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using Microsoft.Xna.Framework;
using System.Reflection;
using StardewValley.Objects;

namespace CollectionsMod
{
    internal class ShirtsCollectionTab : CollectionTab
    {
        public override bool Enabled { get => Config.showShirtsTab; }
        public ShirtsCollectionTab(IMonitor monitor, IModHelper helper, ModConfig config) : base(monitor, helper, config)
        {
            name = "Shirts";
            this.TabID = 5660;
            this.Icon = Helper.ModContent.Load<Texture2D>("assets/LooseSprites/ShirtsCursor.png");
            this.excludeSet = config.excludedShirtIDs;
            this.customOrder = config.customShirtOrder;
        }

        public override ClickableTextureComponent GenerateItemIcon(int id, ParsedItemData itemdata, CollectionsPage _instance, int iconX, int iconY, bool hasCollected)
        {
            Texture2D texture = itemdata.GetTexture();
            Rectangle sourceRect = itemdata.GetSourceRect();

            int pageIdx = CollectionPageHelper.GetTabPageForTabID(_instance, TabID);
            int leftIdx = CollectionPageHelper.GetFirstSideTabOnPage(_instance, pageIdx).Value.myID;

            Rectangle dyedSourceRect = new Rectangle(sourceRect.Left + 128, sourceRect.Top, sourceRect.Width, sourceRect.Height);
            return new CustomClickableTextureComponent(itemdata.QualifiedItemId + " " + hasCollected, new Rectangle(iconX, iconY, 64, componentHeight), null, "", texture, sourceRect, dyedSourceRect, 4f)
            {
                myID = id,
                rightNeighborID = (((id + 1) % maxColumns == 0) ? (-1) : (id + 1)),
                leftNeighborID = ((id % maxColumns == 0) ? leftIdx : (id - 1)),
                downNeighborID = ((iconY + iconWidth > _instance.yPositionOnScreen + _instance.height - iconHeight - 4) ? (-7777) : (id + maxColumns)),
                upNeighborID = ((id < maxColumns) ? 12347 : (id - maxColumns)),
                fullyImmutable = true
            };
        }

        public override void createDescription(CollectionsPage _instance, string id)
        {
            FieldInfo fieldInfo = typeof(CollectionsPage).GetField("hoverItem", BindingFlags.NonPublic | BindingFlags.Instance);
            string itemID = ItemRegistry.GetMetadata(id).LocalItemId;

            fieldInfo.SetValue(_instance, new Clothing(itemID));
        }

        public override List<ParsedItemData> GenerateItems(CollectionsPage _instance)
        {
            List<string> maleUniques = new List<string>()
            {
                "1041", "1202" , "1130", "1133", "1136", "1177"
            };

            List<string> femaleUniques = new List<string>()
            {
                "1038", "1201" , "1129", "1132", "1152", "1176"
            };

            ShirtDataDefinition definition = new ShirtDataDefinition();
            IEnumerable<string> orderedData = definition.GetAllIds()
                .Where(entry => !excludeSet.Contains(entry))
                .Where(entry => !(!Config.showHeartEventItems && entry == "1127"))
                .Where(entry => !(Game1.player.IsMale && femaleUniques.Contains(entry))) // Filter female unique clothes when playing as boi
                .Where(entry => !(!Game1.player.IsMale && maleUniques.Contains(entry)))  // Filter male unique clothes when playing as girl
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
