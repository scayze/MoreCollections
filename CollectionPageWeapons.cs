using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley.ItemTypeDefinitions;
using StardewValley.Menus;
using StardewValley;
using Microsoft.Xna.Framework;
using StardewValley.GameData.Weapons;
using StardewValley.Objects;

namespace CollectionsMod
{
    internal class CollectionPageWeapons
    {
        public static IMonitor Monitor;

        public static IModHelper Helper;

        public static readonly int tabID = 5525;

        internal static void Initialize(IMonitor monitor, IModHelper helper)
        {
            Monitor = monitor;
            Helper = helper;
        }

        public static void UpdateCollectionsPage(CollectionsPage page)
        {
            try
            {
                page.sideTabs.Add(

                    tabID,
                    new ClickableTextureComponent(
                        tabID.ToString() ?? "",
                        new Rectangle(page.xPositionOnScreen - 48, page.yPositionOnScreen + 64 * (2 + page.sideTabs.Count), 64, 64),
                        "",
                        "Weapons",
                        Helper.ModContent.Load<Texture2D>("assets/LooseSprites/WeaponCursor.png"),
                        new Rectangle(0, 0, 16, 16),
                        4f
                    )
                    {
                        myID = 7009,
                        upNeighborID = -99998,
                        downNeighborID = -99998,
                        rightNeighborID = 0
                    }
                );

                page.collections.Add(tabID, new List<List<ClickableTextureComponent>>());
                Populate(page);
            }
            catch (Exception ex)
            {
                Monitor.Log("Error: " + ex.Message);
            }
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
                        if (item == null) return;
                        Patches.modData.CollectedItems.Add(item.QualifiedItemId);
                        Monitor.Log($"Found Item " + item.QualifiedItemId + "/" + item.DisplayName + " at " + location.name, LogLevel.Trace);
                    }
                }
            }
        }

        public static void Populate(CollectionsPage _instance)
        {
            try
            {
                if (_instance.collections[tabID].Count > 0)
                {
                    _instance.collections[tabID].Clear();
                }
                int totalItems = 0;
                int maxRowSize = 10;
                int tabListX = _instance.xPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearSideBorder;
                int tabListY = _instance.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder - 16;

                // Step 1: Define weapons to exclude
                HashSet<string> excludedWeaponIDs = new()
                {
                    "49", "34", "47", "53", "66" // Add any other IDs you want to skip
                };

                
                // Step 2: Define the custom order of weapon IDs
                List<string> customOrder = new()
                {
                    "0", "11", "12", "16", "17", "22", 
                    "43", "1", "20", "31", "24", 
                    "44", "15", "6", "27", 
                    "65", "18", "21", "19", "26", "32", "33", 
                    "5", "10", "14", "7", "51", "46", 
                    "13", "8", "60", "3", "52", "48", "28", 
                    "50", "45", "23", "2", "9", "56", 
                    "59", "61", "29", "57", "54", "4", "55", 
                    "58", "64", "62", "63", "42", "39", 
                    "25", "37", "36", "38", "30", "41", "35", "40" // Put the weapon IDs you want, in your desired order
                    //(i put the enters myself for more clear, but idk if possible)
                };
                

                // Step 3: Sort weapon data according to custom order (excluding undesired ones)
                IEnumerable<KeyValuePair<string, WeaponData>> orderedWeapons = Game1.weaponData
                    .Where(entry => !excludedWeaponIDs.Contains(entry.Key))
                    .OrderBy(entry =>
                    {
                        int index = customOrder.IndexOf(entry.Key);
                        return index == -1 ? int.MaxValue : index; // Push anything not listed to the end
                    });


                foreach (KeyValuePair<string, WeaponData> weaponEntry in orderedWeapons)
                {

                    WeaponDataDefinition def = new WeaponDataDefinition();
                    ParsedItemData itemdata = def.GetData(weaponEntry.Key);

                    bool hasCollected = Patches.modData.CollectedItems.Contains(itemdata.QualifiedItemId);

                    int iconWidth = 68;
                    int iconHeight = 68;
                    int weaponX = tabListX + totalItems % maxRowSize * iconWidth;
                    int weaponY = tabListY + totalItems / maxRowSize * iconHeight;
                    if (weaponY > _instance.yPositionOnScreen + _instance.height)
                    {
                        _instance.collections[tabID].Add(new List<ClickableTextureComponent>());
                        totalItems = 0;
                        weaponX = tabListX;
                        weaponY = tabListY;
                    }
                    if (_instance.collections[tabID].Count == 0)
                    {
                        _instance.collections[tabID].Add(new List<ClickableTextureComponent>());
                    }
                    List<ClickableTextureComponent> weaponTabList = _instance.collections[tabID].Last();
                    int componentHeight = 64;
                    Texture2D texture = itemdata.GetTexture();
                    Rectangle sourceRect = itemdata.GetSourceRect();

                    weaponTabList.Add(new ClickableTextureComponent(itemdata.QualifiedItemId + " " + hasCollected, new Rectangle(weaponX, weaponY, 64, componentHeight), null, "", texture, sourceRect, 4f)
                    {
                        myID = weaponTabList.Count,
                        rightNeighborID = (((weaponTabList.Count + 1) % maxRowSize == 0) ? (-1) : (weaponTabList.Count + 1)),
                        leftNeighborID = ((weaponTabList.Count % maxRowSize == 0) ? 7001 : (weaponTabList.Count - 1)),
                        downNeighborID = ((weaponY + iconWidth > _instance.yPositionOnScreen + _instance.height - iconHeight - 4) ? (-7777) : (weaponTabList.Count + maxRowSize)),
                        upNeighborID = ((weaponTabList.Count < maxRowSize) ? 12347 : (weaponTabList.Count - maxRowSize)),
                        fullyImmutable = true
                    });
                    totalItems++;
                }
                Monitor.Log("test" + totalItems);
            }
            catch (Exception ex)
            {
                Monitor.Log("Error: " + ex.Message);
            }
        }
    }
}