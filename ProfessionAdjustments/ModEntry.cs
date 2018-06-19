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

        private List<StardewValley.Object> toolTipsAlreadyAdjusted = new List<StardewValley.Object>();

        private double onePt4OverOnePt1 = 1.2727272727;

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            MenuEvents.MenuClosed += this.MenuEvents_MenuClosed;
            MenuEvents.MenuChanged += this.MenuEvents_MenuChanged;
            ControlEvents.MouseChanged += this.ControlEvents_MouseChanged;

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
                data["LevelUp_ProfessionDescription_Artisan"] = "Artisan goods(wine, cheese, oil, etc.) worth 10 % more.";
            }
        }

        /*********
        ** Private methods
        *********/

        /* MenuEvents_MenuClosed
         */
        private void MenuEvents_MenuClosed(object sender, EventArgsClickableMenuClosed e)
        {
            if (Game1.activeClickableMenu is ShopMenu)
            {
                this.toolTipsAlreadyAdjusted.Clear();
            }
        }

        /* ControlEvents_MouseChanged
         * 
         */
        private void ControlEvents_MouseChanged(object sender, EventArgsMouseStateChanged e) {

            var x = e.PriorPosition.X;
            var y = e.PriorPosition.Y;

            if (Game1.activeClickableMenu is ShopMenu)
            {
                var shop = Game1.activeClickableMenu as ShopMenu;

                foreach (ClickableComponent c in shop.inventory.inventory)
                {
                    if (c.containsPoint(x, y))
                    {
                        Item clickableComponent = shop.inventory.getItemFromClickableComponent(c);
                        bool isArtisan = this.ProfessionIsArtisan() && clickableComponent.Category == -26;
                        double artisanValue = isArtisan ? onePt4OverOnePt1 : 1;

                        this.Monitor.Log(isArtisan.ToString());

                        if (clickableComponent != null && shop.highlightItemToSell(clickableComponent))
                        {
                            double sellPercentage = (double)this.Helper.Reflection.GetField<float>(shop, "sellPercentage").GetValue();

                            if (clickableComponent is StardewValley.Object obj && clickableComponent.Category == -26 && !this.toolTipsAlreadyAdjusted.Contains(obj)) // -26 is artisan goods
                            {
                                obj.Price = (int)(obj.Price / onePt4OverOnePt1);
                                this.toolTipsAlreadyAdjusted.Add(obj);
                            }

                            //this.Helper.Reflection.GetField<int>(shop, "hoverPrice").SetValue((clickableComponent is StardewValley.Object ? (int)((double)(clickableComponent as StardewValley.Object).sellToStorePrice() * (sellPercentage / artisanValue)) : (int)((double)(clickableComponent.salePrice() / 2) * (sellPercentage / artisanValue))) * clickableComponent.Stack);
                        }
                    }
                }
            }
        }

        /* MenuEvents_MenuChanged
         */
        private void MenuEvents_MenuChanged(object sender, EventArgsClickableMenuChanged e) {

            if (Game1.activeClickableMenu is DialogueBox) {

                var menu = Game1.activeClickableMenu as DialogueBox;
                var dialogues = this.Helper.Reflection.GetField<List<String>>(menu, "dialogues").GetValue();

                if (dialogues.Count == 1 && dialogues[0] == "Go to sleep for the night?") {
                    foreach (Item item in Game1.getFarm().shippingBin)
                    {
                        if (this.ProfessionIsArtisan())
                        {

                            if (item is StardewValley.Object obj && item.Category == -26) // -26 is artisan goods
                            {
                                obj.Price = (int)(obj.Price / onePt4OverOnePt1);
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }
            }
        }
    
        private bool ProfessionIsArtisan() {
            return Game1.player.professions.Contains(4);
        }
    }
}