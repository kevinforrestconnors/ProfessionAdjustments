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
    public class ModEntry : Mod
    {
        /*********
        ** Properties
        *********/
        /// <summary>The mod configuration.</summary>

        private List<Item> artisanGoodsShipped = new List<Item>();

        private List<StardewValley.Object> toolTipsAlreadyAdjusted = new List<StardewValley.Object>();

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {

            SaveEvents.BeforeSave += this.SaveEvents_BeforeSave;
            SaveEvents.AfterSave += this.SaveEvents_AfterSave;
            MenuEvents.MenuClosed += this.MenuEvents_MenuClosed;
            MenuEvents.MenuChanged += this.MenuEvents_MenuChanged;
            PlayerEvents.InventoryChanged += this.PlayerEvents_InventoryChanged;
            ControlEvents.MouseChanged += this.ControlEvents_MouseChanged;

        }

        /*********
        ** Private methods
        *********/

        /* SaveEvents_BeforeSave
         * 
         */
        private void SaveEvents_BeforeSave(object sender, EventArgs eventArgs) 
        {
        }

        /* SaveEvents_AfterSave
         * 
         */
        private void SaveEvents_AfterSave(object sender, EventArgs eventArgs)
        {
        }

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

            this.Monitor.Log("Line 71");
            var x = e.PriorPosition.X;
            this.Monitor.Log("Line 73");
            var y = e.PriorPosition.Y;
            this.Monitor.Log("Line 75");
            if (Game1.activeClickableMenu is ShopMenu)
            {
                this.Monitor.Log("Line 78");
                var shop = Game1.activeClickableMenu as ShopMenu;

                this.Monitor.Log("Line 81");
                foreach (ClickableComponent c in shop.inventory.inventory)
                {
                    if (c.containsPoint(x, y))
                    {
                        this.Monitor.Log("Line 86");
                        Item clickableComponent = shop.inventory.getItemFromClickableComponent(c);
                        bool isArtisan = this.ProfessionIsArtisan() && clickableComponent.Category == -26;
                        double artisanValue = isArtisan ? 1.27272727272 : 1;

                        this.Monitor.Log(isArtisan.ToString());

                        if (clickableComponent != null && shop.highlightItemToSell(clickableComponent))
                        {
                            double sellPercentage = (double)this.Helper.Reflection.GetField<float>(shop, "sellPercentage").GetValue();

                            if (clickableComponent is StardewValley.Object obj && clickableComponent.Category == -26 && !this.toolTipsAlreadyAdjusted.Contains(obj)) // -26 is artisan goods
                            {
                                obj.Price = (int)(obj.Price / 1.2727272727272);
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
                                obj.Price = (int)(obj.Price / 1.2727272727272);
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

        /* PlayerEvents_InventoryChanged
                 * 
                 * 
                 * 
                 */
        private void PlayerEvents_InventoryChanged(object sender, EventArgsInventoryChanged e)
        {
            //if (Game1.activeClickableMenu is ShopMenu) {

            //    for (int i = 0; i < e.Removed.Count; i++)
            //    {

            //        Item item = e.Removed[i].Item;

            //        if (this.ProfessionIsArtisan() && item.Category == -26)
            //        {
            //            if (item is StardewValley.Object obj && item.Category == -26) // -26 is artisan goods
            //            {
            //                int deficit = (int)(obj.Price * 0.3f);
            //                Game1.player.money -= deficit; 

            //            }
            //        }
            //    }

            //    for (int j = 0; j < e.QuantityChanged.Count; j++)
            //    {
            //        Item item = e.QuantityChanged[j].Item;


            //        if (this.ProfessionIsArtisan() && item.Category == -26)
            //        {

            //            int quantitySold = e.QuantityChanged[j].StackChange;

            //            if (item is StardewValley.Object obj && item.Category == -26) // -26 is artisan goods
            //            {
            //                int deficit = (int)(quantitySold * (obj.Price * 0.3f));
            //                Game1.player.money -= deficit;
            //            }
            //        }
            //    }
            //}
        }
    
        private bool ProfessionIsArtisan() {
            return Game1.player.professions.Contains(4);
        }
    }
}