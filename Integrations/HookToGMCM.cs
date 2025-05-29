using GenericModConfigMenu;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollectionsMod.Integrations
{
    internal class HookToGMCM
    {
        public static void Apply(ModEntry collectionsMod)
        {
            // get Generic Mod Config Menu's API (if it's installed)
            var configMenu = collectionsMod.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null)
            {
                collectionsMod.Monitor.Log("Generic Mod Config Menu not found. Skipping config menu registration.", LogLevel.Warn);
                return;
            }

            // register mod
            configMenu.Register(
                collectionsMod.ModManifest,
                () => {
                    collectionsMod.Config = new ModConfig();
                },
                () => collectionsMod.Helper.WriteConfig(collectionsMod.Config)
            );

            // add weapons collection tab
            configMenu.AddBoolOption(
                collectionsMod.ModManifest,
                name: () => "Enable Weapons",
                tooltip: () => "Enable the weapons collection page.",
                getValue: () => collectionsMod.Config.showWeaponTab,
                setValue: value => collectionsMod.Config.showWeaponTab = value
            );

            // add rings collection page
            configMenu.AddBoolOption(
                collectionsMod.ModManifest,
                name: () => "Enable Rings",
                tooltip: () => "Enable the rings collection page.",
                getValue: () => collectionsMod.Config.showRingsTab,
                setValue: value => {
                    collectionsMod.Config.showRingsTab = value;
                }
            );

            // add hats collection page
            configMenu.AddBoolOption(
                collectionsMod.ModManifest,
                name: () => "Enable Hats",
                tooltip: () => "Enable the hats collection page.",
                getValue: () => collectionsMod.Config.showHatsTab,
                setValue: value => {
                    collectionsMod.Config.showHatsTab = value;
                }
            );

            // add shirts collection page
            configMenu.AddBoolOption(
                collectionsMod.ModManifest,
                name: () => "Enable Shirts",
                tooltip: () => "Enable the shirts collection page.",
                getValue: () => collectionsMod.Config.showShirtsTab,
                setValue: value => {
                    collectionsMod.Config.showShirtsTab = value;
                }
            );

            // add pants collection page
            configMenu.AddBoolOption(
                collectionsMod.ModManifest,
                name: () => "Enable Pants",
                tooltip: () => "Enable the pants collection page.",
                getValue: () => collectionsMod.Config.showPantsTab,
                setValue: value => {
                    collectionsMod.Config.showPantsTab = value;
                }
            );

            // add shoes collection page
            configMenu.AddBoolOption(
                collectionsMod.ModManifest,
                name: () => "Enable Shoes",
                tooltip: () => "Enable the shoes collection page.",
                getValue: () => collectionsMod.Config.showBootsTab,
                setValue: value => {
                    collectionsMod.Config.showBootsTab = value;
                }
            );
        }
    }
}
