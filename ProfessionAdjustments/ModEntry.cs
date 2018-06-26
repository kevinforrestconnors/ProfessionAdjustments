using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Tools;

namespace ProfessionAdjustments
{
    /// <summary>The main entry point.</summary>
    public class ModEntry : Mod, IAssetEditor
    {
        /*********
        ** Properties
        *********/
        /// <summary>The mod configuration.</summary>

        private List<Item> artisanGoodsShipped = new List<Item>();

        private Dictionary<string, int> itemsWithChangedValues = new Dictionary<string, int>();

        private double onePt4OverOnePt1 = 1.4 / 1.1;
        private double onePt4OverOnePt2 = 1.4 / 1.2;

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            PlayerEvents.InventoryChanged += this.PlayerEvents_InventoryChanged;
            SaveEvents.AfterCreate += this.SaveEvents_AfterCreate;
            SaveEvents.AfterLoad += this.SaveEvents_AfterLoad;
            SaveEvents.BeforeSave += this.SaveEvents_BeforeSave;
        }

        public bool CanEdit<T>(IAssetInfo asset)
        {
            // change 40% to 10% in Artisan description
            if (asset.AssetNameEquals("Strings/UI"))
                return true;

            return false;
        }

        public void Edit<T>(IAssetData asset)
        {
            if (asset.AssetNameEquals("Strings/UI"))
            {
                IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
                data["LevelUp_ProfessionDescription_Artisan"] = "Artisan goods you produce (wine, cheese, oil, etc.) worth 10% more.";
            }
        }

        /*********
        ** Private methods
        *********/

        private void SaveEvents_AfterCreate(object sender, EventArgs e)
        {
            ProfessionAdjustmentsData instance = this.Helper.ReadJsonFile<ProfessionAdjustmentsData>($"data/{Constants.SaveFolderName}.json") ?? new ProfessionAdjustmentsData();
            this.itemsWithChangedValues = instance.itemsWithChangedValues;
            this.Helper.WriteJsonFile($"data/{Constants.SaveFolderName}.json", instance);
        }

        private void SaveEvents_AfterLoad(object sender, EventArgs e)
        {
            ProfessionAdjustmentsData instance = this.Helper.ReadJsonFile<ProfessionAdjustmentsData>($"data/{Constants.SaveFolderName}.json") ?? new ProfessionAdjustmentsData();
            this.itemsWithChangedValues = instance.itemsWithChangedValues;
            this.Helper.WriteJsonFile($"data/{Constants.SaveFolderName}.json", instance);
        }

        private void SaveEvents_BeforeSave(object sender, EventArgs e)
        {
            ProfessionAdjustmentsData instance = this.Helper.ReadJsonFile<ProfessionAdjustmentsData>($"data/{Constants.SaveFolderName}.json");
            instance.itemsWithChangedValues = this.itemsWithChangedValues;
            this.Helper.WriteJsonFile($"data/{Constants.SaveFolderName}.json", instance);
        }

        private void PlayerEvents_InventoryChanged(object sender, EventArgsInventoryChanged e) {

            foreach (ItemStackChange itemStackChange in e.Added)
            {

                Item item = itemStackChange.Item;

                if (item is StardewValley.Object obj && obj.Category == -26)
                {
                    int _;

                    if (!this.itemsWithChangedValues.TryGetValue(obj.Name, out _)) {
                        if (this.ProfessionIsRancher() && (obj.Category == -5 || obj.Category == -6 || obj.Category == -18))
                        {
                            this.itemsWithChangedValues.Add(obj.Name, obj.Price);
                            obj.Price = (int)Math.Round(obj.Price / onePt4OverOnePt2);
                        }
                        else if (this.ProfessionIsArtisan())
                        {
                            this.itemsWithChangedValues.Add(obj.Name, obj.Price);
                            obj.Price = (int)Math.Round(obj.Price / onePt4OverOnePt1);
                        }
                    } else if (!this.ProfessionIsArtisan()) { // item found in list, but player has respecced into non-artisan
                        this.itemsWithChangedValues.Remove(obj.Name);    
                    }
                }
            }
        }
    
        private bool ProfessionIsArtisan() {
            return Game1.player.professions.Contains(4);
        }

        private bool ProfessionIsRancher() {
            return Game1.player.professions.Contains(0);
        }
    }
}