using System.Collections.Generic;

namespace CollectionsMod
{
    internal static class TabUtils
    {
        public static List<int> GetEnabledTabOrder(ModConfig config)
        {
            var order = new List<int>();
            if (config.EnableWeapons)
                order.Add(CollectionPageWeapons.tabID);
            if (config.EnableClothes)
                order.Add(CollectionPageClothing.tabID);
            return order;
        }
    }
}