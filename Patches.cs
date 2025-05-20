using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

namespace CollectionsMod
{
    internal static class Patches
    {

        internal static void PerformHoverAction(CollectionsPage __instance, int x, int y)
        {
            CollectionPageHelper.upButton.tryHover(x, y, 0.5f);
            CollectionPageHelper.downButton.tryHover(x, y, 0.5f);
        }
        internal static void Draw(CollectionsPage __instance, SpriteBatch b)
        {
            if (CollectionPageHelper.currentSideTabPage == 1) CollectionPageHelper.upButton.draw(b);
            if (CollectionPageHelper.currentSideTabPage == 0) CollectionPageHelper.downButton.draw(b);
        }

        internal static bool ReceiveLeftClick(CollectionsPage __instance, int x, int y, bool playSound = true)
        {

            if (CollectionPageHelper.upButton.containsPoint(x, y))
            {
                CollectionPageHelper.currentSideTabPage = 0;
                CollectionPageHelper.RedrawSideTabs(__instance);
            }
            if (CollectionPageHelper.downButton.containsPoint(x, y))
            {
                CollectionPageHelper.currentSideTabPage = 1;
                CollectionPageHelper.RedrawSideTabs(__instance);
            }
            return true;
        }

        internal static bool CreateDescription(CollectionsPage __instance, ref string __result, string id)
        {
            if (id == "???") return true;

            CollectionTab currentTab = ModEntry.collectionTabs.Find((t) => t.TabID == __instance.currentTab);

            if (currentTab != null)
            {
                string itemID = ItemRegistry.GetMetadata(id).LocalItemId;
                currentTab.createDescription(__instance, itemID);
                return false;
            }

            return true;
        }
    }
}
