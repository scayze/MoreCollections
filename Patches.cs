using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.ItemTypeDefinitions;
using StardewValley.Menus;
using StardewValley.Objects;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CollectionsMod
{
    internal class Patches
    {
        private static IMonitor Monitor;

        public static CModData modData;

        public static ModConfig Config;

        public static ClickableTextureComponent upButton;

        public static ClickableTextureComponent downButton;

        // call this method from your Entry class
        internal static void Initialize(IMonitor monitor, ModConfig config)
        {
            Monitor = monitor;
            Config = config;
        }

        internal static void PerformHoverAction(CollectionsPage __instance, int x, int y)
        {
            upButton.tryHover(x, y, 0.5f);
            downButton.tryHover(x, y, 0.5f);
        }
        internal static void Draw(CollectionsPage __instance, SpriteBatch b)
        {
            if (CollectionPageClothing.currentSideTabPage == 1) upButton.draw(b);
            if (CollectionPageClothing.currentSideTabPage == 0) downButton.draw(b);
            if (__instance.currentTab == CollectionPageClothing.tabID)
            {
                CollectionPageClothing.hatsButton.draw(b);
                CollectionPageClothing.shirtsButton.draw(b);
                CollectionPageClothing.pantsButton.draw(b);
                CollectionPageClothing.ringsButton.draw(b);
                CollectionPageClothing.bootsButton.draw(b);
            }
        }

        static void ResetClothButtonScale()
        {
            CollectionPageClothing.hatsButton.scale = 4.0f;
            CollectionPageClothing.shirtsButton.scale = 4.0f;
            CollectionPageClothing.pantsButton.scale = 4.0f;
            CollectionPageClothing.ringsButton.scale = 4.0f;
            CollectionPageClothing.bootsButton.scale = 4.0f;
        }

        internal static bool ReceiveLeftClick(CollectionsPage __instance, int x, int y, bool playSound = true)
        {

            if (upButton.containsPoint(x, y))
            {
                CollectionPageClothing.currentSideTabPage = 0;
                CollectionPageClothing.RedrawSideTabs(__instance);
            }
            if (downButton.containsPoint(x, y))
            {
                CollectionPageClothing.currentSideTabPage = 1;
                CollectionPageClothing.RedrawSideTabs(__instance);
            }
            if (__instance.currentTab == CollectionPageClothing.tabID)
            {
                if (CollectionPageClothing.hatsButton.containsPoint(x, y))
                {
                    ResetClothButtonScale();
                    CollectionPageClothing.hatsButton.scale -= 0.3f;
                    CollectionPageClothing.subPage = CollectionPageClothing.SubPage.HAT; 
                    CollectionPageClothing.PopulateTab(__instance, new HatDataDefinition(), Config.excludedHatIDs, Config.customHatOrder, (string x) => Game1.player.tailoredItems.ContainsKey(x));
                }
                else if (CollectionPageClothing.shirtsButton.containsPoint(x, y))
                {
                    ResetClothButtonScale();
                    CollectionPageClothing.shirtsButton.scale -= 0.3f;
                    CollectionPageClothing.subPage = CollectionPageClothing.SubPage.SHIRT;
                    CollectionPageClothing.PopulateTab(__instance, new ShirtDataDefinition(), Config.excludedShirtIDs, Config.customShirtOrder, (string x) => Game1.player.tailoredItems.ContainsKey(x));
                }
                else if (CollectionPageClothing.pantsButton.containsPoint(x, y))
                {
                    ResetClothButtonScale();
                    CollectionPageClothing.pantsButton.scale -= 0.3f;
                    CollectionPageClothing.subPage = CollectionPageClothing.SubPage.PANT;
                    CollectionPageClothing.PopulateTab(__instance, new PantsDataDefinition(), Config.excludedPantsIDs, Config.customPantsOrder, (string x) => Game1.player.tailoredItems.ContainsKey(x));
                }
                else if (CollectionPageClothing.ringsButton.containsPoint(x, y))
                {
                    ResetClothButtonScale();
                    CollectionPageClothing.ringsButton.scale -= 0.3f;
                    CollectionPageClothing.subPage = CollectionPageClothing.SubPage.RING;
                    CollectionPageClothing.PopulateTab(__instance, new ObjectDataDefinition(), Config.excludedRingIDs, Config.customRingOrder, (string x) => Game1.player.tailoredItems.ContainsKey(x));
                }
                else if (CollectionPageClothing.bootsButton.containsPoint(x, y))
                {
                    ResetClothButtonScale();
                    CollectionPageClothing.bootsButton.scale -= 0.3f;
                    CollectionPageClothing.subPage = CollectionPageClothing.SubPage.BOOT;
                    CollectionPageClothing.PopulateTab(__instance, new BootsDataDefinition(), Config.excludedShoeIDs, Config.customShoeOrder, (string x) => Game1.player.tailoredItems.ContainsKey(x));
                }
            }
            return true;
        }

        internal static bool CreateDescription(CollectionsPage __instance, ref string __result, string id)
        {
            
            if (id == "???") return true;

            FieldInfo fieldInfo = typeof(CollectionsPage).GetField("hoverItem", BindingFlags.NonPublic | BindingFlags.Instance);
            string itemID = ItemRegistry.GetMetadata(id).LocalItemId;

            if (__instance.currentTab == CollectionPageWeapons.tabID)
            {
                fieldInfo.SetValue(__instance, new MeleeWeapon(itemID));


                if(id == "32" ||id == "33" || id == "34")
                {
                    fieldInfo.SetValue(__instance, new Slingshot(itemID));
                }
                __result = "";
                return false;

            }
            if (__instance.currentTab == CollectionPageClothing.tabID)
            {
                if (CollectionPageClothing.subPage == CollectionPageClothing.SubPage.NONE) return false;
                if (CollectionPageClothing.subPage == CollectionPageClothing.SubPage.HAT)
                {
                    fieldInfo.SetValue(__instance, new Hat(itemID));
                    __result = "";
                    return false;
                }
                if (CollectionPageClothing.subPage == CollectionPageClothing.SubPage.SHIRT)
                {
                    fieldInfo.SetValue(__instance, new Clothing(itemID));
                    __result = "";
                    return false;
                }
                if (CollectionPageClothing.subPage == CollectionPageClothing.SubPage.PANT)
                {
                    fieldInfo.SetValue(__instance, new Clothing(itemID));
                    __result = "";
                    return false;
                }
                if (CollectionPageClothing.subPage == CollectionPageClothing.SubPage.RING)
                {
                    fieldInfo.SetValue(__instance, new Ring(itemID));
                    __result = "";
                    return false;
                }
                if (CollectionPageClothing.subPage == CollectionPageClothing.SubPage.BOOT)
                {
                    fieldInfo.SetValue(__instance, new Boots(itemID));
                    __result = "";
                    return false;
                }
            }

            return true;
        }
    }
}
