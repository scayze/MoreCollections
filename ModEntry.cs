
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.ItemTypeDefinitions;
using StardewValley.Menus;
using StardewValley.Objects;
using System.Reflection;
using xTile.Dimensions;

namespace CollectionsMod
{
    /// The mod entry point.
    internal sealed class ModEntry : Mod
    {
        private ModConfig Config;

        public static CModData modData;

        public static List<CollectionTab> collectionTabs = new List<CollectionTab> ();

       

        public override void Entry(IModHelper helper)
        {
            this.Config = this.Helper.ReadConfig<ModConfig>();

            // Collections
            collectionTabs.Add(new WeaponCollectionTab(Monitor, helper, Config));
            collectionTabs.Add(new HatsCollectionTab(Monitor, helper, Config));
            collectionTabs.Add(new ShirtsCollectionTab(Monitor, helper, Config));
            collectionTabs.Add(new PantsCollectionTab(Monitor, helper, Config));
            collectionTabs.Add(new BootsCollectionTab(Monitor, helper, Config));
            collectionTabs.Add(new RingsCollectionTab(Monitor, helper, Config));

            // Events
            helper.Events.Display.MenuChanged += OnMenuChange;
            helper.Events.Player.InventoryChanged += OnInventoryChange;
            helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            helper.Events.GameLoop.Saving += OnSaving;
            helper.Events.GameLoop.UpdateTicked += OnTicked;

            // Harmony Patches
            Harmony harmony = new Harmony(this.ModManifest.UniqueID);
            
            harmony.Patch(
               original: AccessTools.Method(typeof(CollectionsPage), nameof(CollectionsPage.createDescription)),
               prefix: new HarmonyMethod(typeof(Patches), nameof(Patches.CreateDescription))
            );

            harmony.Patch(
               original: AccessTools.Method(typeof(CollectionsPage), nameof(CollectionsPage.receiveLeftClick)),
               prefix: new HarmonyMethod(typeof(Patches), nameof(Patches.ReceiveLeftClick))
            );

            harmony.Patch(
               original: AccessTools.Method(typeof(CollectionsPage), nameof(CollectionsPage.performHoverAction)),
               postfix: new HarmonyMethod(typeof(Patches), nameof(Patches.PerformHoverAction))
            );

            var targetMethod = typeof(CollectionsPage).GetMethod(
                "draw",
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly
            );

            harmony.Patch(
               original: targetMethod,
               postfix: new HarmonyMethod(typeof(Patches), nameof(Patches.Draw))
            );


        }


        public static void InitialSaveScanning()
        {
            foreach (GameLocation loc in Game1.locations)
            {
                // Scan all Dressers for items
                foreach (Furniture furniture in loc.furniture)
                {
                    if (furniture is not StorageFurniture dresser) continue;
                    if (dresser.heldItems == null) continue;

                    foreach (Item item in dresser.heldItems)
                    {
                        if (item == null) continue;
                        modData.CollectedItems.Add(item.QualifiedItemId);
                    }
                    
                }

                // Scan all chests for items
                foreach (var pair in loc.Objects.Pairs)
                {
                    if (pair.Value is not Chest chest) continue;
                    if (!chest.playerChest.Value) continue;

                    foreach (Item item in chest.GetItemsForPlayer())
                    {
                        if (item == null) continue;
                        modData.CollectedItems.Add(item.QualifiedItemId);
                    }
                    
                }
            }

            foreach (Item item in Game1.player.Items)
            {
                if (item == null)
                {
                    continue;
                }
                modData.CollectedItems.Add(item.QualifiedItemId);
            }

            // You can wear Pans on the hat, however, if you already upgraded the pan to e.g. iridium,
            // you would never be able to the gold pant on the collection page. So we manually add lower tier hats
            if (modData.CollectedItems.Contains("(H)IridiumPanHat")) modData.CollectedItems.Add("(H)GoldPanHat");
            if (modData.CollectedItems.Contains("(H)GoldPanHat")) modData.CollectedItems.Add("(H)SteelPanHat");
            if (modData.CollectedItems.Contains("(H)SteelPanHat")) modData.CollectedItems.Add("(H)71");
        }

        private void OnInventoryChange(object? sender, InventoryChangedEventArgs e)
        {
            foreach(Item i in e.Added)
            {
                modData.CollectedItems.Add(i.QualifiedItemId);
            } 
        }

        private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
        {
            CModData data = Helper.Data.ReadSaveData<CModData>("weaponcollection-data") ?? new CModData();
            modData = data;

            InitialSaveScanning();
        }

        private void OnSaving(object? sender, SavingEventArgs e)
        {
            Helper.Data.WriteSaveData("weaponcollection-data", modData);
        }

        private void OnTicked(object? sender, UpdateTickedEventArgs e)
        {
            if (Game1.player != null && modData != null)
            {
                if (Game1.player.hat.Value != null) modData.CollectedItems.Add(Game1.player.hat.Value.QualifiedItemId);
                if (Game1.player.shirtItem.Value != null) modData.CollectedItems.Add(Game1.player.shirtItem.Value.QualifiedItemId);
                if (Game1.player.boots.Value != null) modData.CollectedItems.Add(Game1.player.boots.Value.QualifiedItemId);
                if (Game1.player.pantsItem.Value != null) modData.CollectedItems.Add(Game1.player.pantsItem.Value.QualifiedItemId);
                if (Game1.player.leftRing.Value != null) modData.CollectedItems.Add(Game1.player.leftRing.Value.QualifiedItemId);
                if (Game1.player.rightRing.Value != null) modData.CollectedItems.Add(Game1.player.rightRing.Value.QualifiedItemId);
            }     
        }

        private void OnMenuChange(object? sender, MenuChangedEventArgs e)
        {
            if (!(e.NewMenu is GameMenu menu))
            {
                return;
            }

            foreach (IClickableMenu page in menu.pages)
            {
                if (page is not CollectionsPage collectionPage) continue;

                foreach(CollectionTab tab in collectionTabs)
                {
                    if(tab.Enabled) tab.Populate(collectionPage);
                }

                CollectionPageHelper.RedrawSideTabs(collectionPage);
            }
        }
    }
}