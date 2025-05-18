
using CollectionsMod.Integrations;
using GenericModConfigMenu;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using System.Reflection;

namespace CollectionsMod
{
    /// The mod entry point.
    internal sealed class ModEntry : Mod
    {
        internal ModConfig Config;

        public override void Entry(IModHelper helper)
        {
            this.Config = this.Helper.ReadConfig<ModConfig>();
            this.Config.UpdateEnableClothes();

            CollectionPageWeapons.Initialize(Monitor, Helper, Config);
            CollectionPageClothing.Initialize(Monitor, Helper, Config);
            Patches.Initialize(Monitor, Config);

            helper.Events.Display.MenuChanged += OnMenuChange;
            helper.Events.Player.InventoryChanged += OnInventoryChange;
            helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            helper.Events.GameLoop.Saving += OnSaving;
            helper.Events.GameLoop.UpdateTicked += OnTicked;
            helper.Events.GameLoop.GameLaunched += OnGameLaunched;

            var harmony = new Harmony(this.ModManifest.UniqueID);

            // example patch, you'll need to edit this for your patch
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

        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            HookToGMCM.Apply(this);
        }

        private void OnInventoryChange(object? sender, InventoryChangedEventArgs e)
        {
            foreach(Item i in e.Added)
            {
                Patches.modData.CollectedItems.Add(i.QualifiedItemId);
            } 
        }

        private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
        {
            CModData data = Helper.Data.ReadSaveData<CModData>("weaponcollection-data") ?? new CModData();
            Patches.modData = data;

            CollectionPageWeapons.InitialSaveScanning();
        }

        private void OnSaving(object? sender, SavingEventArgs e)
        {
            Helper.Data.WriteSaveData("weaponcollection-data", Patches.modData);
        }

        private void OnTicked(object? sender, UpdateTickedEventArgs e)
        {
            if (Game1.player != null && Patches.modData != null)
            {
                if (Game1.player.hat.Value != null) Patches.modData.CollectedItems.Add(Game1.player.hat.Value.QualifiedItemId);
                if (Game1.player.shirtItem.Value != null) Patches.modData.CollectedItems.Add(Game1.player.shirtItem.Value.QualifiedItemId);
                if (Game1.player.boots.Value != null) Patches.modData.CollectedItems.Add(Game1.player.boots.Value.QualifiedItemId);
                if (Game1.player.pantsItem.Value != null) Patches.modData.CollectedItems.Add(Game1.player.pantsItem.Value.QualifiedItemId);
                if (Game1.player.leftRing.Value != null) Patches.modData.CollectedItems.Add(Game1.player.leftRing.Value.QualifiedItemId);
                if (Game1.player.rightRing.Value != null) Patches.modData.CollectedItems.Add(Game1.player.rightRing.Value.QualifiedItemId);
            }     
        }

        private void OnMenuChange(object? sender, MenuChangedEventArgs e)
        {
            if (!(e.NewMenu is GameMenu menu))
                return;

            foreach (IClickableMenu page in menu.pages)
            {
                if (page is CollectionsPage collectionsPage)
                {
                    Patches.upButton = new ClickableTextureComponent(new Rectangle(page.xPositionOnScreen - 32, page.yPositionOnScreen + 92, 48, 44), Game1.mouseCursors, new Rectangle(421, 459, 11, 12), 4f)
                    {
                        myID = 708,
                        downNeighborID = -7777
                    };
                    Patches.downButton = new ClickableTextureComponent(new Rectangle(page.xPositionOnScreen - 32, page.yPositionOnScreen + page.height - 76, 48, 44), Game1.mouseCursors, new Rectangle(421, 472, 11, 12), 4f)
                    {
                        myID = 709,
                        upNeighborID = -7777
                    };


                    CollectionsPage collectionPage = (CollectionsPage)page;

                    if (Config.EnableWeapons)
                        CollectionPageWeapons.UpdateCollectionsPage(page as CollectionsPage);
                    if (Config.EnableClothes)
                        CollectionPageClothing.UpdateCollectionsPage(page as CollectionsPage);
                    CollectionPageClothing.RedrawSideTabs(collectionPage);
                }
            }
        }
    }
}