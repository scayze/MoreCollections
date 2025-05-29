using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Menus;
using System.Collections;

namespace CollectionsMod
{
    internal class CollectionPageHelper
    {
        public static int currentSideTabPage = 0;

        public static int maxPerPage = 8;

        public static ClickableTextureComponent upButton;

        public static ClickableTextureComponent downButton;

        public static int GetTabPageForTabID(CollectionsPage page, int tabID)
        {
            var sideTabList = page.sideTabs.ToList();
            sideTabList.Sort((x, y) => x.Key.CompareTo(y.Key));
            int idx = sideTabList.FindIndex((x) => x.Key == tabID);
            return idx / maxPerPage;
        }
        public static KeyValuePair<int, ClickableTextureComponent> GetFirstSideTabOnPage(CollectionsPage page, int pageIdx)
        {
            var sideTabList = page.sideTabs.ToList();
            sideTabList.Sort((x, y) => x.Key.CompareTo(y.Key));
         
            return sideTabList[maxPerPage * pageIdx];
        }

        public static void RedrawSideTabs(CollectionsPage collectionPage)
        {

            upButton = new ClickableTextureComponent(new Rectangle(collectionPage.xPositionOnScreen - 32, collectionPage.yPositionOnScreen + 92, 48, 44), Game1.mouseCursors, new Rectangle(421, 459, 11, 12), 4f)
            {
                myID = 708,
                downNeighborID = -99998,
            };
            downButton = new ClickableTextureComponent(new Rectangle(collectionPage.xPositionOnScreen - 32, collectionPage.yPositionOnScreen + collectionPage.height - 76, 48, 44), Game1.mouseCursors, new Rectangle(421, 472, 11, 12), 4f)
            {
                myID = 709,
                upNeighborID = -99998,
            };

            var sideTabList = collectionPage.sideTabs.ToList();
            sideTabList.Sort((x, y) => x.Key.CompareTo(y.Key));


            for (int i = 0; i < sideTabList.Count; i++)
            {
                KeyValuePair<int, ClickableTextureComponent> tab = sideTabList[i];
                int drawIndex = i - maxPerPage * currentSideTabPage;

                if (drawIndex < 0 || drawIndex >= maxPerPage)
                {
                    tab.Value.bounds.Y = -100;
                    tab.Value.upNeighborID = -1;
                    tab.Value.downNeighborID = -1;
                    tab.Value.rightNeighborID = -1;
                    tab.Value.leftNeighborID = -1;
                }
                else
                {
                    tab.Value.bounds.Y = collectionPage.yPositionOnScreen - 44 + 64 * (2 + drawIndex);
                    if (currentSideTabPage > 0) tab.Value.bounds.Y += 64;


                    tab.Value.downNeighborID = ClickableComponent.SNAP_AUTOMATIC;
                    tab.Value.upNeighborID = ClickableComponent.SNAP_AUTOMATIC;
                }


                if (drawIndex == 0 && currentSideTabPage > 0)
                {
                    tab.Value.upNeighborID = 708;
                }
                if (drawIndex == maxPerPage -1 && currentSideTabPage == 0)
                {
                    tab.Value.downNeighborID = 709;
                }

                if (i == sideTabList.Count - 1)
                {
                    tab.Value.downNeighborID = -1;
                }
            }
        }
    }
}
