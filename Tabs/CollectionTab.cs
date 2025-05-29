using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley.GameData.Weapons;
using StardewValley.ItemTypeDefinitions;
using StardewValley.Menus;
using StardewValley.Objects;
using StardewValley;
using Microsoft.Xna.Framework;

namespace CollectionsMod
{
    internal abstract class CollectionTab
    {
        public IMonitor Monitor;

        public IModHelper Helper;

        public ModConfig Config;

        public int TabID;

        public Texture2D Icon;
        abstract public bool Enabled { get; }

        protected int iconWidth = 68;
        protected int iconHeight = 68;
        protected int componentHeight = 64;
        protected int maxColumns = 10;

        protected string name = "Unnamed Page";
        protected HashSet<string> excludeSet;
        protected List<string> customOrder;

        protected CollectionTab(IMonitor monitor, IModHelper helper, ModConfig config)
        {
            Monitor = monitor;
            Helper = helper;
            Config = config;
        }

        public virtual bool itemCollected(ParsedItemData itemdata)
        {
            return ModEntry.modData.CollectedItems.Contains(itemdata.QualifiedItemId) || Game1.player.tailoredItems.ContainsKey(itemdata.ItemId);
        }

        public ClickableTextureComponent getTabIcon(CollectionsPage page)
        {
            return new ClickableTextureComponent(
                 TabID.ToString() ?? "",
                 new Rectangle(page.xPositionOnScreen - 48, page.yPositionOnScreen + 64 * (2 + page.sideTabs.Count), 64, 64),
                 "",
                 name,
                 Icon,
                 new Rectangle(0, 0, 16, 16),
                 4f
             )
            {
                myID = 7009,
                upNeighborID = -99998,
                downNeighborID = -99998,
                rightNeighborID = 0
            };
        }

        public abstract List<ParsedItemData> GenerateItems(CollectionsPage _instance);

        public virtual ClickableTextureComponent GenerateItemIcon(int id, ParsedItemData itemdata, CollectionsPage _instance, int iconX, int iconY, bool hasCollected)
        {
            Texture2D texture = itemdata.GetTexture();
            Rectangle sourceRect = itemdata.GetSourceRect();

            int pageIdx = CollectionPageHelper.GetTabPageForTabID(_instance, TabID);
            int leftIdx = CollectionPageHelper.GetFirstSideTabOnPage(_instance, pageIdx).Value.myID;

            return new ClickableTextureComponent(itemdata.QualifiedItemId + " " + hasCollected, new Rectangle(iconX, iconY, 64, componentHeight), null, "", texture, sourceRect, 4f)
            {
                myID = id,
                rightNeighborID = (((id + 1) % maxColumns == 0) ? (-1) : (id + 1)),
                leftNeighborID = ((id % maxColumns == 0) ? leftIdx : (id - 1)),
                downNeighborID = ((iconY + iconWidth > _instance.yPositionOnScreen + _instance.height - iconHeight - 4) ? (-7777) : (id + maxColumns)),
                upNeighborID = ((id < maxColumns) ? 12347 : (id - maxColumns)),
                fullyImmutable = true
            };
        }

        public virtual void createDescription(CollectionsPage _instance, string id)
        {

        }

        public virtual void Populate(CollectionsPage _instance)
        {
            List<ParsedItemData> items = GenerateItems(_instance);

            _instance.sideTabs.Add(TabID, getTabIcon(_instance));
            _instance.collections.Add(TabID, new List<List<ClickableTextureComponent>>());

            _instance.collections[TabID].Clear();
            _instance.currentPage = 0;

            int totalItems = 0;
            
            int tabListX = _instance.xPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearSideBorder;
            int tabListY = _instance.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder - 16;

            foreach (ParsedItemData itemdata in items)
            {
                bool hasCollected = itemCollected(itemdata);

                
                int iconX = tabListX + totalItems % maxColumns * iconWidth;
                int iconY = tabListY + totalItems / maxColumns * iconHeight;
                if (iconY > _instance.yPositionOnScreen + _instance.height - 128)
                {
                    _instance.collections[TabID].Add(new List<ClickableTextureComponent>());
                    totalItems = 0;
                    iconX = tabListX;
                    iconY = tabListY;
                }
                if (_instance.collections[TabID].Count == 0)
                {
                    _instance.collections[TabID].Add(new List<ClickableTextureComponent>());
                }
                List<ClickableTextureComponent> tabList = _instance.collections[TabID].Last();

                ClickableTextureComponent icon = GenerateItemIcon(tabList.Count, itemdata, _instance, iconX, iconY, hasCollected);
                tabList.Add(icon);

                totalItems++;
            }
        }
    }
}