using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Menus;

namespace CollectionsMod
{
    internal class CollectionPageHelper
    {
        public static int currentSideTabPage = 0;

        public static ClickableTextureComponent upButton;

        public static ClickableTextureComponent downButton;

        public static void RedrawSideTabs(CollectionsPage collectionPage)
        {

            upButton = new ClickableTextureComponent(new Rectangle(collectionPage.xPositionOnScreen - 32, collectionPage.yPositionOnScreen + 92, 48, 44), Game1.mouseCursors, new Rectangle(421, 459, 11, 12), 4f)
            {
                myID = 708,
                downNeighborID = -7777
            };
            downButton = new ClickableTextureComponent(new Rectangle(collectionPage.xPositionOnScreen - 32, collectionPage.yPositionOnScreen + collectionPage.height - 76, 48, 44), Game1.mouseCursors, new Rectangle(421, 472, 11, 12), 4f)
            {
                myID = 709,
                upNeighborID = -7777
            };

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

                if (currentSideTabPage == 1) tab.Value.bounds.Y += 64;

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
        }
    }
}
