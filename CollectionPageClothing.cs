using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley.ItemTypeDefinitions;
using StardewValley.Menus;
using StardewValley;
using Microsoft.Xna.Framework;
using StardewValley.Objects;
using StardewValley.GameData.Shirts;
using System.Globalization;
using StardewValley.GameData.Pets;
using StardewValley.GameData.Pants;
using System.Collections;

namespace CollectionsMod
{
    internal class CollectionPageClothing
    {
        public static IMonitor Monitor;

        public static IModHelper Helper;

        public static ModConfig Config;

        public static readonly int tabID = 7525;
        public static readonly int hatsID = 7526;
        public static readonly int shirtsID = 7527;
        public static readonly int pantsID = 7528;
        public static readonly int ringsID = 7529;
        public static readonly int shoesID = 7530;

        public static ClickableTextureComponent hatsButton;
        public static ClickableTextureComponent shirtsButton;
        public static ClickableTextureComponent pantsButton;
        public static ClickableTextureComponent ringsButton;
        public static ClickableTextureComponent bootsButton;

        public static int currentSideTabPage = 0;

        public enum SubPage {
            NONE,
            HAT,
            SHIRT,
            PANT,
            RING,
            BOOT
        }

        public static SubPage subPage = SubPage.NONE;

        internal static void Initialize(IMonitor monitor, IModHelper helper, ModConfig config)
        {
            Monitor = monitor;
            Helper = helper;
            Config = config;
        }
        public static void RedrawSideTabs(CollectionsPage collectionPage)
        {
            var tabKeys = collectionPage.sideTabs.Keys.ToList();
            const int maxPerPage = 8;

            int totalPages = (int)Math.Ceiling(tabKeys.Count / (float)maxPerPage);
            currentSideTabPage = Math.Clamp(currentSideTabPage, 0, Math.Max(0, totalPages - 1));

            int start = currentSideTabPage * maxPerPage;
            int end = Math.Min(start + maxPerPage, tabKeys.Count);

            // Hide all tabs first
            foreach (var key in tabKeys)
                collectionPage.sideTabs[key].bounds.Y = -1000;

            // Show and position only the visible tabs
            for (int i = start; i < end; i++)
            {
                var tab = collectionPage.sideTabs[tabKeys[i]];
                int visibleIndex = i - start;
                tab.bounds.Y = collectionPage.yPositionOnScreen - 44 + 64 * (2 + visibleIndex);

                // Set up/down neighbor IDs for visible tabs
                if (visibleIndex == 0 && Patches.upButton != null && currentSideTabPage > 0)
                {
                    tab.upNeighborID = Patches.upButton.myID;
                    tab.upNeighborImmutable = true;
                }
                else if (visibleIndex > 0)
                {
                    tab.upNeighborID = collectionPage.sideTabs[tabKeys[i - 1]].myID;
                    tab.upNeighborImmutable = false;
                }
                else
                {
                    tab.upNeighborID = -1;
                    tab.upNeighborImmutable = true;
                }

                if (visibleIndex == end - start - 1 && Patches.downButton != null && currentSideTabPage < totalPages - 1)
                {
                    tab.downNeighborID = Patches.downButton.myID;
                    tab.downNeighborImmutable = true;
                }
                else if (visibleIndex < end - start - 1)
                {
                    tab.downNeighborID = collectionPage.sideTabs[tabKeys[i + 1]].myID;
                    tab.downNeighborImmutable = false;
                }
                else
                {
                    tab.downNeighborID = -1;
                    tab.downNeighborImmutable = true;
                }

                // Special handling for your custom tabs
                if (tabKeys[i] == CollectionPageClothing.tabID)
                {
                    tab.rightNeighborID = 7011;
                    tab.rightNeighborImmutable = true;
                    tab.downNeighborID = -1;
                    tab.downNeighborImmutable = true;
                }
                else if (tabKeys[i] == CollectionPageWeapons.tabID)
                {
                    tab.upNeighborID = 708;
                    tab.upNeighborImmutable = true;
                }
            }

            // After positioning visible tabs...
            if (Patches.upButton != null && currentSideTabPage > 0)
            {
                // Place upButton just above the first visible tab
                if (tabKeys.Count > start)
                {
                    var firstTab = collectionPage.sideTabs[tabKeys[start]];
                    Patches.upButton.bounds.X = firstTab.bounds.X + firstTab.bounds.Width / 2 - Patches.upButton.bounds.Width / 2;
                    Patches.upButton.bounds.Y = firstTab.bounds.Y - Patches.upButton.bounds.Height - 4; // 4px gap
                }
            }

            // Set up/down button neighbor IDs
            if (Patches.upButton != null)
            {
                Patches.upButton.downNeighborID = (start < end) ? collectionPage.sideTabs[tabKeys[start]].myID : -7777;
            }
            if (Patches.downButton != null)
            {
                Patches.downButton.upNeighborID = (end > start) ? collectionPage.sideTabs[tabKeys[end - 1]].myID : -7777;
            }

            if (Patches.downButton != null && (currentSideTabPage + 1) * maxPerPage < tabKeys.Count)
            {
                if (tabKeys.Count > 0 && end > start)
                {
                    var lastTab = collectionPage.sideTabs[tabKeys[end - 1]];
                    Patches.downButton.bounds.X = lastTab.bounds.X + lastTab.bounds.Width / 2 - Patches.downButton.bounds.Width / 2;
                    Patches.downButton.bounds.Y = lastTab.bounds.Y + lastTab.bounds.Height + 4; // 4px gap
                }
            }
        }

        public static void UpdateCollectionsPage(CollectionsPage page)
        {
            // Conditional path based on Earthy mod
            string spritePath = "assets/LooseSprites/";

            if (Helper.ModRegistry.IsLoaded("DaisyNiko.EarthyInterface"))
            {
                spritePath += "Earthy/";
                if (Helper.ModRegistry.IsLoaded("Taiyo.VanillaTweaks.UI"))
                {
                    spritePath += "VT/";
                }
            }

            var tabOrder = TabUtils.GetEnabledTabOrder(Config);
            int myIndex = tabOrder.IndexOf(tabID);
            if (myIndex == -1)
                return; // Not enabled

            // TAB
            page.sideTabs.Add(
                tabID,
                new ClickableTextureComponent(
                    tabID.ToString() ?? "",
                    new Rectangle(page.xPositionOnScreen - 48, page.yPositionOnScreen + 64 * (2 + page.sideTabs.Count), 64, 64),
                    "",
                    "Clothing",
                    Helper.ModContent.Load<Texture2D>(spritePath + "ClothingCursor.png"),
                    new Rectangle(0, 0, 16, 16),
                    4f
                )
                {
                    myID = 7009 + myIndex,
                    upNeighborID = -99998,
                    downNeighborID = -99998,
                    rightNeighborID = 7011,
                    rightNeighborImmutable = true
                }
            ); // <- LOOR

            // LITTLE TABS

            // Add buttons conditionally based on config options
            int buttonIndex = 0;

            // HATS
            if (Config.EnableHats)
            {
                hatsButton = new ClickableTextureComponent(
                    "collectionHats",
                    new Rectangle(page.xPositionOnScreen + 128 * (buttonIndex + 1) - 64, page.yPositionOnScreen + 116, 128, 64),
                    "",
                    "",
                    Helper.ModContent.Load<Texture2D>(spritePath + "ButtonHat.png"),
                    new Rectangle(0, 0, 32, 16),
                    4f
                )
                {
                    myID = 7011 + buttonIndex,
                    leftNeighborID = 7010 + buttonIndex,
                    upNeighborID = -99998,
                    downNeighborID = -99998,
                    rightNeighborID = 7012 + buttonIndex
                };
                buttonIndex++;
            }

            // SHIRTS
            if (Config.EnableShirts)
            {
                shirtsButton = new ClickableTextureComponent(
                    "collectionShirts",
                    new Rectangle(page.xPositionOnScreen + 128 * (buttonIndex + 1) - 64, page.yPositionOnScreen + 116, 128, 64),
                    "",
                    "",
                    Helper.ModContent.Load<Texture2D>(spritePath + "ButtonShirt.png"),
                    new Rectangle(0, 0, 32, 16),
                    4f
                )
                {
                    myID = 7011 + buttonIndex,
                    leftNeighborID = 7010 + buttonIndex,
                    upNeighborID = -99998,
                    downNeighborID = -99998,
                    rightNeighborID = 7012 + buttonIndex
                };
                buttonIndex++;
            }

            // PANTS
            if (Config.EnablePants)
            {
                pantsButton = new ClickableTextureComponent(
                    "collectionPants",
                    new Rectangle(page.xPositionOnScreen + 128 * (buttonIndex + 1) - 64, page.yPositionOnScreen + 116, 128, 64),
                    "",
                    "",
                    Helper.ModContent.Load<Texture2D>(spritePath + "ButtonPants.png"),
                    new Rectangle(0, 0, 32, 16),
                    4f
                )
                {
                    myID = 7011 + buttonIndex,
                    leftNeighborID = 7010 + buttonIndex,
                    upNeighborID = -99998,
                    downNeighborID = -99998,
                    rightNeighborID = 7012 + buttonIndex
                };
                buttonIndex++;
            }

            // RINGS
            if (Config.EnableRings)
            {
                ringsButton = new ClickableTextureComponent(
                    "collectionRings",
                    new Rectangle(page.xPositionOnScreen + 128 * (buttonIndex + 1) - 64, page.yPositionOnScreen + 116, 128, 64),
                    "",
                    "",
                    Helper.ModContent.Load<Texture2D>(spritePath + "ButtonRings.png"),
                    new Rectangle(0, 0, 32, 16),
                    4f
                )
                {
                    myID = 7011 + buttonIndex,
                    leftNeighborID = 7010 + buttonIndex,
                    upNeighborID = -99998,
                    downNeighborID = -99998,
                    rightNeighborID = 7012 + buttonIndex
                };
                buttonIndex++;
            }

            // SHOES
            if (Config.EnableShoes)
            {
                bootsButton = new ClickableTextureComponent(
                    "collectionShoes",
                    new Rectangle(page.xPositionOnScreen + 128 * (buttonIndex + 1) - 64, page.yPositionOnScreen + 116, 128, 64),
                    "",
                    "",
                    Helper.ModContent.Load<Texture2D>(spritePath + "ButtonShoes.png"),
                    new Rectangle(0, 0, 32, 16),
                    4f
                )
                {
                    myID = 7011 + buttonIndex,
                    leftNeighborID = 7010 + buttonIndex,
                    upNeighborID = -99998,
                    downNeighborID = -99998,
                    rightNeighborID = 7012 + buttonIndex
                };
                buttonIndex++;
            }


            page.collections.Add(tabID, new List<List<ClickableTextureComponent>>());


            if (Game1.player.IsMale)
            {
                Config.excludedShirtIDs.Add("1041");
                Config.excludedShirtIDs.Add("1202");
                Config.excludedShirtIDs.Add("1130");
                Config.excludedShirtIDs.Add("1133");
                Config.excludedShirtIDs.Add("1136");
                Config.excludedShirtIDs.Add("1177");
            }
            else
            {
                Config.excludedShirtIDs.Add("1038");
                Config.excludedShirtIDs.Add("1201");
                Config.excludedShirtIDs.Add("1129");
                Config.excludedShirtIDs.Add("1132");
                Config.excludedShirtIDs.Add("1152");
                Config.excludedShirtIDs.Add("1176");
            }


            // Emilies shit clothes
            if (!Config.showHeartEventItems)
            {
                Config.excludedShoeIDs.Add("804");
                Config.excludedShirtIDs.Add("1127");
                Config.excludedHatIDs.Add("41");
            }

            if( !(Game1.IsMultiplayer || Game1.IsClient || Game1.IsServer))
            {
                Config.excludedRingIDs.Add("801");
            }

            page.collections[tabID].Add(new List<ClickableTextureComponent>());
        }

        public static void InitialSaveScanning()
        {
            foreach(GameLocation loc in Game1.locations)
            {
                ScanLocation(loc);
            }

            foreach (Item item in Game1.player.Items)
            {
                if (item == null)
                {
                    continue;
                }
                Patches.modData.CollectedItems.Add(item.QualifiedItemId);
                Monitor.Log($"Found Item " + item.QualifiedItemId + "/" + item.DisplayName + " at Players Inventory", LogLevel.Trace);
            }
        }

        public static void ScanLocation(GameLocation location)
        {
            foreach (var pair in location.Objects.Pairs)
            {
                if (pair.Value is Chest chest && chest.playerChest.Value)
                {
                    foreach (Item item in chest.GetItemsForPlayer())
                    {
                        Patches.modData.CollectedItems.Add(item.QualifiedItemId);
                        Monitor.Log($"Found Item " + item.QualifiedItemId + "/" + item.DisplayName + " at " + location.name, LogLevel.Trace);
                    }
                }
            }
        }

        public static void PopulateTab(CollectionsPage _instance, BaseItemDataDefinition definition, HashSet<string> excludeList, List<string> customOrder, Func<string,bool> collectedFunc)
        {
            if (Patches.modData.CollectedItems.Contains("(H)IridiumPanHat")) Patches.modData.CollectedItems.Add("(H)GoldPanHat");
            if (Patches.modData.CollectedItems.Contains("(H)GoldPanHat")) Patches.modData.CollectedItems.Add("(H)SteelPanHat");
            if (Patches.modData.CollectedItems.Contains("(H)SteelPanHat")) Patches.modData.CollectedItems.Add("(H)71");

            _instance.collections[tabID].Clear();
            _instance.currentPage = 0;

            int totalItems = 0;
            int maxRowSize = 10;
            int tabListX = _instance.xPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearSideBorder;
            int tabListY = _instance.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + 64;


            IEnumerable<string> orderedData = definition.GetAllIds()
                .Where(entry => !excludeList.Contains(entry))
                .OrderBy(entry =>
                {
                    int index = customOrder.IndexOf(entry);
                    return index == -1 ? int.MaxValue : index;
                });

            //If we checking for rings HACK
            if(definition is ObjectDataDefinition)
            {
                orderedData = orderedData
                    .Where(entry => ItemRegistry.GetData(entry).Category == -96);

            }

            foreach (string shirtKey in orderedData)
            {
                ParsedItemData itemdata = definition.GetData(shirtKey);

                bool hasCollected = collectedFunc.Invoke(shirtKey) || Patches.modData.CollectedItems.Contains(itemdata.QualifiedItemId);

                int iconWidth = 68;
                int iconHeight = 68;
                int iconX = tabListX + totalItems % maxRowSize * iconWidth;
                int iconY = tabListY + totalItems / maxRowSize * iconHeight;
                if (iconY > _instance.yPositionOnScreen + _instance.height - 128)
                {
                    _instance.collections[tabID].Add(new List<ClickableTextureComponent>());
                    totalItems = 0;
                    iconX = tabListX;
                    iconY = tabListY;
                }
                if (_instance.collections[tabID].Count == 0)
                {
                    _instance.collections[tabID].Add(new List<ClickableTextureComponent>());
                }
                List<ClickableTextureComponent> tabList = _instance.collections[tabID].Last();
                int componentHeight = 64;
                Texture2D texture = itemdata.GetTexture();
                Rectangle sourceRect = itemdata.GetSourceRect();

                if(definition is ShirtDataDefinition)
                {
                    Rectangle dyedSourceRect = new Rectangle(sourceRect.Left + 128, sourceRect.Top, sourceRect.Width, sourceRect.Height);
                    tabList.Add(new CustomClickableTextureComponent(itemdata.QualifiedItemId + " " + hasCollected, new Rectangle(iconX, iconY, 64, componentHeight), null, "", texture, sourceRect, dyedSourceRect, 4f)
                    {
                        myID = tabList.Count,
                        rightNeighborID = (((tabList.Count + 1) % maxRowSize == 0) ? (-1) : (tabList.Count + 1)),
                        leftNeighborID = ((tabList.Count % maxRowSize == 0) ? 7001 : (tabList.Count - 1)),
                        downNeighborID = ((iconY + iconWidth > _instance.yPositionOnScreen + _instance.height - iconHeight - 4) ? (-7777) : (tabList.Count + maxRowSize)),
                        upNeighborID = ((tabList.Count < maxRowSize) ? 12347 : (tabList.Count - maxRowSize)),
                        fullyImmutable = true
                    });
                }
                else
                {
                    tabList.Add(new ClickableTextureComponent(itemdata.QualifiedItemId + " " + hasCollected, new Rectangle(iconX, iconY, 64, componentHeight), null, "", texture, sourceRect, 4f)
                    {
                        myID = tabList.Count,
                        rightNeighborID = (((tabList.Count + 1) % maxRowSize == 0) ? (-1) : (tabList.Count + 1)),
                        leftNeighborID = ((tabList.Count % maxRowSize == 0) ? 7001 : (tabList.Count - 1)),
                        downNeighborID = ((iconY + iconWidth > _instance.yPositionOnScreen + _instance.height - iconHeight - 4) ? (-7777) : (tabList.Count + maxRowSize)),
                        upNeighborID = ((tabList.Count < maxRowSize) ? 12347 : (tabList.Count - maxRowSize)),
                        fullyImmutable = true
                    });
                }
                totalItems++;

            }
        }
    }
}