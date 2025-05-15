
using HarmonyLib;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;

namespace CollectionsMod
{
    /// The mod entry point.
    public class CModData 
    {
        public HashSet<string> CollectedItems { get; set; } = new HashSet<string>();
    }
}