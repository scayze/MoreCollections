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
                    collectionsMod.Config.UpdateEnableClothes();
                },
                () => collectionsMod.Helper.WriteConfig(collectionsMod.Config)
            );

            // add weapons collection tab
            configMenu.AddBoolOption(
                collectionsMod.ModManifest,
                name: () => "Enable Weapons",
                tooltip: () => "Enable the weapons collection page.",
                getValue: () => collectionsMod.Config.EnableWeapons,
                setValue: value => collectionsMod.Config.EnableWeapons = value
            );

            // add clothing collection tab
            configMenu.AddBoolOption(
                collectionsMod.ModManifest,
                name: () => "Enable Clothing",
                tooltip: () => "Enable the clothing collection page.",
                getValue: () => collectionsMod.Config.EnableClothes,
                setValue: value => collectionsMod.Config.EnableClothes = value
            );

            // add rings collection page
            configMenu.AddBoolOption(
                collectionsMod.ModManifest,
                name: () => "Enable Rings",
                tooltip: () => "Enable the rings collection page.",
                getValue: () => collectionsMod.Config.EnableRings,
                setValue: value => {
                    collectionsMod.Config.EnableRings = value;
                    collectionsMod.Config.UpdateEnableClothes();
                }
            );

            // add hats collection page
            configMenu.AddBoolOption(
                collectionsMod.ModManifest,
                name: () => "Enable Hats",
                tooltip: () => "Enable the hats collection page.",
                getValue: () => collectionsMod.Config.EnableHats,
                setValue: value => {
                    collectionsMod.Config.EnableHats = value;
                    collectionsMod.Config.UpdateEnableClothes();
                }
            );

            // add shirts collection page
            configMenu.AddBoolOption(
                collectionsMod.ModManifest,
                name: () => "Enable Shirts",
                tooltip: () => "Enable the shirts collection page.",
                getValue: () => collectionsMod.Config.EnableShirts,
                setValue: value => {
                    collectionsMod.Config.EnableShirts = value;
                    collectionsMod.Config.UpdateEnableClothes();
                }
            );

            // add pants collection page
            configMenu.AddBoolOption(
                collectionsMod.ModManifest,
                name: () => "Enable Pants",
                tooltip: () => "Enable the pants collection page.",
                getValue: () => collectionsMod.Config.EnablePants,
                setValue: value => {
                    collectionsMod.Config.EnablePants = value;
                    collectionsMod.Config.UpdateEnableClothes();
                }
            );

            // add shoes collection page
            configMenu.AddBoolOption(
                collectionsMod.ModManifest,
                name: () => "Enable Shoes",
                tooltip: () => "Enable the shoes collection page.",
                getValue: () => collectionsMod.Config.EnableShoes,
                setValue: value => {
                    collectionsMod.Config.EnableShoes = value;
                    collectionsMod.Config.UpdateEnableClothes();
                }
            );
        }
    }
}
