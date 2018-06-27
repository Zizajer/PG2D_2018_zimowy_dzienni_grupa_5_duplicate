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
    public class PlayerInventoryManager : InventoryManager
    {
        public int InventoryPickUpLimit { get; } = 5;
        public float MaxTotalMultiplier { get; } = 1.5f;

        private KeyboardState pastKey5; //F PICKING UP ITEMS
        private KeyboardState pastKey6; //Z DROPPING ITEMS
        private KeyboardState pastKey7; //R SWAPPING ITEMS
        private KeyboardState pastKey9; //E USING ITEMS
        private KeyboardState pastKey10; //Q EXAMINE SELECTED ITEM

        private int DrankPotions = 0; //Var for magic things

        public PlayerInventoryManager(Player character, List<Item> items) : base(character, items) { }

        public override bool TakeItem(Level level, GraphicsDevice graphicsDevice)
        {
            bool IsItemTaken = false;
            if (Keyboard.GetState().IsKeyDown(Keys.F) && pastKey5.IsKeyUp(Keys.F))
            {
                if (Inventory.Count < InventoryPickUpLimit)
                {
                    if (base.TakeItem(level, graphicsDevice))
                    {
                        Global.SoundManager.takeToInventory.Play();
                        Item Item = Inventory.ElementAt(Inventory.Count - 1);
                        Global.Gui.WriteToConsole("You picked up " + Item.Name);

                        if (Item.Category.Contains("PositionTargetedAttackItem"))
                        {
                            String AttackClassName = Item.Category.Split('_')[1];
                            ((Player)Character).ItemProjectileAttack = (IPositionTargetedAttack)Activator.CreateInstance(Type.GetType("Dungeon_Crawler." + AttackClassName));
                        }

                        IsItemTaken = true;
                    }
                    else
                    {
                        Global.Gui.WriteToConsole("You cant pick up more items of that kind");
                    }
                }
                else
                {
                    Global.Gui.WriteToConsole("You cant pick up more items");
                }

            }
            pastKey5 = Keyboard.GetState();
            return IsItemTaken;
        }

        public override bool DropItem(Level level, int i)
        {
            bool IsItemDropped = false;
            if (Keyboard.GetState().IsKeyDown(Keys.Z) && pastKey6.IsKeyUp(Keys.Z))
            {
                int temp = i;

                if (temp != -1)
                {                    
                    Global.SoundManager.dropFromInventory.Play();
                    Global.Gui.WriteToConsole("You dropped " + Inventory.ElementAt(i).Name);

                    if (Inventory.ElementAt(i).Category.Contains("PositionTargetedAttackItem"))
                    {
                        ((Player)Character).ItemProjectileAttack = null;
                    }
                    base.DropItem(level, i);

                    IsItemDropped = true;
                }
                else
                {
                    Global.Gui.WriteToConsole("You dont have any items to drop");
                }
            }
            pastKey6 = Keyboard.GetState();
            return IsItemDropped;
        }

        public void SwapItem(Level level, int i, GraphicsDevice graphicsDevice)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.R) && pastKey7.IsKeyUp(Keys.R))
            {
                int temp = i;

                if (temp != -1)
                {
                    if (base.TakeItem(level, graphicsDevice))
                    {
                        Global.SoundManager.takeToInventory.Play();
                        Global.Gui.WriteToConsole("You swapped " + Inventory.ElementAt(i).Name + " for " + Inventory.ElementAt(Inventory.Count - 1).Name);
                        base.DropItem(level, i);
                    }
                    else
                    {
                        Global.Gui.WriteToConsole("Nothing on the floor to swap for");
                    }
                }
                else
                {
                    Global.Gui.WriteToConsole("You dont have any items");
                }
            }
            pastKey7 = Keyboard.GetState();
        }

        public override bool UseItem(int i)
        {
            bool IsItemUsed = false;
            if (Keyboard.GetState().IsKeyDown(Keys.E) && pastKey9.IsKeyUp(Keys.E))
            {
                int temp = i;

                if (temp != -1)
                {
                    if (Inventory[i].Category.Equals("Potion"))
                    {
                        Global.SoundManager.mixtureDrink.Play();
                        DrankPotions++;
                    }
                    IsItemUsed = base.UseItem(i);
                    if (!IsItemUsed)
                    {
                        if (!(Inventory[i] is IUsableItem))
                        {
                            Global.Gui.WriteToConsole("You can't use this item.");
                        }
                        else
                        {
                            Global.Gui.WriteToConsole("You can't use this item right now.");
                        }
                    }
                }
                else
                {
                    return false;
                }
            }
            pastKey9 = Keyboard.GetState();
            return IsItemUsed;
        }

        public void ExamineItem(int i)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Q) && pastKey10.IsKeyUp(Keys.Q))
            {
                if (i != -1)
                {
                    Global.SoundManager.examineItem.Play();
                    Global.Gui.WriteToConsole(Inventory.ElementAt(i).Description);
                }
                else
                {
                    Global.Gui.WriteToConsole("Nothing to examine");
                }

            }
            pastKey10 = Keyboard.GetState();
        }

        public override bool IsItemEligibleToBeTaken(Item item)
        {
            //At first, check if item is attack giving item
            if (item.Category.Contains("AttackItem"))
            {
                foreach (Item Item in Inventory)
                {
                    if (Item.Category.Contains("AttackItem"))
                    {
                        return false;
                    }
                }
            }

            //Then, check multipliers
            float TotalHealthMultiplier = 1f;
            float TotalDefenseMultiplier = 1f;
            float TotalSpDefenseMultiplier = 1f;
            float TotalAttackMultiplier = 1f;
            float TotalSpAttackMultiplier = 1f;
            float TotalSpeedMultiplier = 1f;
            foreach (Item Item in Inventory)
            {
                TotalHealthMultiplier *= Item.HealthMultiplier;
                TotalDefenseMultiplier *= Item.DefenseMultiplier;
                TotalSpDefenseMultiplier *= Item.SpDefenseMultiplier;
                TotalAttackMultiplier *= Item.AttackMultiplier;
                TotalSpAttackMultiplier *= Item.SpAttackMultiplier;
                TotalSpeedMultiplier *= Item.SpeedMultiplier;
            }

            TotalHealthMultiplier *= item.HealthMultiplier;
            TotalDefenseMultiplier *= item.DefenseMultiplier;
            TotalSpDefenseMultiplier *= item.SpDefenseMultiplier;
            TotalAttackMultiplier *= item.AttackMultiplier;
            TotalSpAttackMultiplier *= item.SpAttackMultiplier;
            TotalSpeedMultiplier *= item.SpeedMultiplier;

            if (TotalHealthMultiplier > MaxTotalMultiplier || TotalDefenseMultiplier > MaxTotalMultiplier || TotalSpDefenseMultiplier > MaxTotalMultiplier || TotalAttackMultiplier > MaxTotalMultiplier
                || TotalSpAttackMultiplier > MaxTotalMultiplier || TotalSpeedMultiplier > MaxTotalMultiplier)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
