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
            int key = 0;
            int num = 0;

            int index = 0;
            int drawnIndex = 0;

            int maxPerPage = 8;

            foreach (KeyValuePair<int, ClickableTextureComponent> tab in collectionPage.sideTabs)
            {
                int currentKey = tab.Key;
                if (collectionPage.sideTabs[currentKey].bounds.Y > num)
                {
                    key = currentKey;
                }

                tab.Value.bounds.Y = collectionPage.yPositionOnScreen - 44 + 64 * (2 + drawnIndex);

                if(currentSideTabPage == 1) tab.Value.bounds.Y += 64;

                if (index >= maxPerPage && currentSideTabPage == 0 || index < maxPerPage && currentSideTabPage == 1)
                {
                    tab.Value.bounds.Y = -100;
                }
                else
                {
                    drawnIndex++;
                }
                

                if (collectionPage.sideTabs[currentKey].downNeighborID == -1)
                {
                    collectionPage.sideTabs[currentKey].downNeighborID = -99998;
                }
                index++;
            }



            collectionPage.sideTabs[0].upNeighborID = -1;
            collectionPage.sideTabs[0].upNeighborImmutable = true;


            collectionPage.sideTabs[7].downNeighborID = 709;
            collectionPage.sideTabs[7].downNeighborImmutable = true;

            collectionPage.sideTabs[tabID].downNeighborID = -1;
            collectionPage.sideTabs[tabID].downNeighborImmutable = true;

            collectionPage.sideTabs[CollectionPageWeapons.tabID].upNeighborID = 708;
            collectionPage.sideTabs[CollectionPageWeapons.tabID].upNeighborImmutable = true;

            collectionPage.sideTabs[tabID].rightNeighborID = 7026;
            collectionPage.sideTabs[tabID].rightNeighborImmutable = true;

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
                    myID = 7010,
                    upNeighborID = -99998,
                    downNeighborID = -99998,
                    rightNeighborID = 7011,
                    rightNeighborImmutable = true
                }
            ); // <- LOOR

            // LITTLE TABS

            //HAT
            hatsButton = new ClickableTextureComponent(
                "collectionHats",
                new Rectangle(page.xPositionOnScreen + 128 * 1 - 64, page.yPositionOnScreen + 116, 128, 64),
                "",
                "",
                    Helper.ModContent.Load<Texture2D>(spritePath + "ButtonHat.png"),
                new Rectangle(0, 0, 32, 16),
                4f
            )
            {
                myID = 7011,
                leftNeighborID = 7010,
                upNeighborID = -99998,
                downNeighborID = -99998,
                rightNeighborID = 7012
            };

            //SHIT
            shirtsButton = new ClickableTextureComponent(
                "collectionShirts",
                new Rectangle(page.xPositionOnScreen + 128 * 2 - 64, page.yPositionOnScreen + 116, 128, 64),
                "",
                "",
                    Helper.ModContent.Load<Texture2D>(spritePath + "ButtonShirt.png"),
                new Rectangle(0, 0, 32, 16),
                4f
            )
            {
                myID = 7012,
                leftNeighborID = 7011,
                upNeighborID = -99998,
                downNeighborID = -99998,
                rightNeighborID = 7013
            };

            //PANTS
            pantsButton= new ClickableTextureComponent(
                "collectionPants",
                new Rectangle(page.xPositionOnScreen + 128 * 3 - 64, page.yPositionOnScreen + 116, 128, 64),
                "",
                "",
                    Helper.ModContent.Load<Texture2D>(spritePath + "ButtonPants.png"),
                new Rectangle(0, 0, 32, 16),
                4f
            )
            {
                myID = 7013,
                leftNeighborID = 7012,
                upNeighborID = -99998,
                downNeighborID = -99998,
                rightNeighborID = 7014
            };

            //RINGS
            ringsButton =  new ClickableTextureComponent(
                "collectionRings",
                new Rectangle(page.xPositionOnScreen + 128 * 4 - 64, page.yPositionOnScreen + 116, 128, 64),
                "",
                "",
                    Helper.ModContent.Load<Texture2D>(spritePath + "ButtonRings.png"),
                new Rectangle(0, 0, 32, 16),
                4f
            )
            {
                myID = 7014,
                leftNeighborID = 7013,
                upNeighborID = -99998,
                downNeighborID = -99998,
                rightNeighborID = 7015
            };

            //SHOES
            bootsButton = new ClickableTextureComponent(
                "collectionShoes",
                new Rectangle(page.xPositionOnScreen + 128 * 5 - 64, page.yPositionOnScreen + 116, 128, 64),
                "",
                "",
                    Helper.ModContent.Load<Texture2D>(spritePath + "ButtonShoes.png"),
                new Rectangle(0, 0, 32, 16),
                4f
            )
            {
                myID = 7015,
                leftNeighborID = 7014,
                upNeighborID = -99998,
                downNeighborID = -99998,
                rightNeighborID = 0
            };
            

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