using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Dungeon_Crawler.Character;

namespace Dungeon_Crawler
{
    public class PlayerInventoryManagerFacade
    {
        public Player Player { get; set; }
        public List<Item> Inventory { get; set; }
        public const int EMPTY_INVENTORY = -1;
        public short SelectedItem { get; private set; } = EMPTY_INVENTORY;
        public PlayerInventoryManager PlayerInventoryManager { get; }

        private KeyboardState PastScrollKey; //Tab

        public PlayerInventoryManagerFacade(Player player, List<Item> items)
        {
            Player = player;
            Inventory = items;

            PlayerInventoryManager = new PlayerInventoryManager(player, items);
        }
        public void Update(GameTime gameTime, GraphicsDevice graphicsDevice, Level level)
        {
            if (Player.currentHealthState != HealthState.Freeze)
            {
                SelectItem();
                PlayerInventoryManager.ExamineItem(SelectedItem);
                if (PlayerInventoryManager.DropItem(level, SelectedItem))
                {
                    if (SelectedItem == 0)
                    {
                        if (Inventory.Count == 1)
                        {
                            SelectedItem = -1;
                        }
                    }
                    else
                    {
                        SelectedItem--;
                    }
                }
                if (PlayerInventoryManager.TakeItem(level, graphicsDevice))
                {
                    if (SelectedItem == -1)
                    {
                        SelectedItem = 0;
                    }
                }
                PlayerInventoryManager.SwapItem(level, SelectedItem, graphicsDevice);
                PlayerInventoryManager.UseItem(SelectedItem);
            }

            byte DeletedItems = PlayerInventoryManager.UpdateItems(gameTime);
            if (DeletedItems > 0)
            {
                if (SelectedItem == 0)
                {
                    if (Inventory.Count == 1)
                    {
                        SelectedItem = -1;
                    }
                }
                else
                {
                    SelectedItem -= DeletedItems;
                }
            }
        }

        private void SelectItem()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Tab) && PastScrollKey.IsKeyUp(Keys.Tab))
            {
                if (Inventory.Count > 0)
                {
                    Global.SoundManager.changeInInventory.Play();
                    if (SelectedItem + 1 < Inventory.Count)
                    {
                        SelectedItem++;
                    }
                    else
                    {
                        SelectedItem = 0;
                    }
                }
                else
                {
                    Global.Gui.WriteToConsole("You dont have any items");
                }

            }
            PastScrollKey = Keyboard.GetState();
        }
    }
}
