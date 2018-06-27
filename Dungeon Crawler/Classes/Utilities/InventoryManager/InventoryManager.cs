using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dungeon_Crawler
{
    public class InventoryManager : IInventoryManager
    {
        public Character Character { get; set; }

        public List<Item> Inventory { get; set; }
        public List<IUsableUpdatableItem> ItemsCurrentlyInUse  { get; private set; }

        public InventoryManager(Character character, List<Item> items)
        {
            Character = character;
            Inventory = items;
            ItemsCurrentlyInUse = new List<IUsableUpdatableItem>();
        }

        public virtual bool TakeItem(Item item)
        {
            if (IsItemEligibleToBeTaken(item))
            {
                Inventory.Add(item);
                item.ApplyEffect(Character);
                return true;
            }
            else
            {
                return false;
            }
        }

        public virtual bool TakeItem(Level level, GraphicsDevice graphicsDevice)
        {
            bool IsItemTaken = false;
            Item[] LevelItemArray = level.items.ToArray();
            for (int i = LevelItemArray.Length - 1; i >= 0; i--)
            {
                Item item = LevelItemArray[i];
                if (Collision.checkCollision(Character.getRectangle(), item.getRectangle()))
                {
                    if (TakeItem(item))
                    {
                        level.items.RemoveAt(i);
                        IsItemTaken = true;
                        break;
                    }
                }
            }
            return IsItemTaken;
        }

        public virtual void TakeItems(List<Item> items)
        {
            foreach (Item Item in items)
            {
                TakeItem(Item);
            }
        }

        public virtual bool UseItem(int i)
        {
            if (Inventory[i] is IUsableUpdatableItem UsableUpdatableItem)
            {
                if (IsItemEligibleToBeUsed(UsableUpdatableItem))
                {
                    UsableUpdatableItem.Use(Character);
                    ItemsCurrentlyInUse.Add(UsableUpdatableItem);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (Inventory[i] is IUsableItem UsableItem)
            {
                if (UsableItem.RemainingUsages > 0)
                {
                    UsableItem.Use(Character);
                    UsableItem.RemainingUsages--;
                }
                if (UsableItem.RemainingUsages == 0)
                {
                    DeleteItem(i);
                }
                return true;
            }
            else
            {
                return false; //Item is unusable
            }
        }

        public virtual bool DeleteItem(int i)
        {
            Inventory[i].RevertEffect(Character);
            Inventory.RemoveAt(i);
            return true;
        }

        public virtual bool DeleteItem(Item item)
        {
            item.RevertEffect(Character);
            Inventory.Remove(item);
            return true;
        }

        public virtual bool DropItem(Level level, int i)
        {
            Item DroppedItem = Inventory[i];
            DroppedItem.RevertEffect(Character);

            Vector2 ScatteringOffset = new Vector2(Global.random.Next(-8, 8), Global.random.Next(-8, 8));
            DroppedItem.Position = Character.Position + ScatteringOffset;
            level.items.Add(DroppedItem);

            Inventory.RemoveAt(i);
            return true;
        }

        public virtual void DropAllItems(Level level)
        {
            foreach (Item Item in Inventory)
            {
                Item.RevertEffect(Character);

                Vector2 ScatteringOffset = new Vector2(Global.random.Next(-8, 8), Global.random.Next(-8, 8));
                Item.Position = Character.Center + ScatteringOffset;

                level.items.Add(Item);
            }
            Inventory.Clear();
        }

        public virtual bool IsItemEligibleToBeTaken(Item item)
        {
            return true;
        }

        public virtual bool IsItemEligibleToBeUsed(IUsableUpdatableItem item)
        {
            foreach (IUsableUpdatableItem Item in ItemsCurrentlyInUse)
            {
                if (Item.Category.Equals(item.Category))
                {
                    return false;
                }
            }
            return true;
        }

        public virtual byte UpdateItems(GameTime gameTime)
        {
            byte DeletedItemsAfterUpdate = 0;
            for (int i = Inventory.Count - 1; i >= 0; i--)
            {
                Item Item = Inventory[i];
                if (Item is IUsableUpdatableItem UsableUpdatableItem)
                {
                    if (UsableUpdatableItem.IsCurrentlyInUse)
                    {
                        UsableUpdatableItem.Update(gameTime, Character);
                    }
                    else if (UsableUpdatableItem.HasRecentUsageJustFinished)
                    {
                        UsableUpdatableItem.RemainingUsages--;
                        UsableUpdatableItem.HasRecentUsageJustFinished = false;
                        ItemsCurrentlyInUse.Remove(UsableUpdatableItem);
                        if (UsableUpdatableItem.RemainingUsages == 0)
                        {
                            if (DeleteItem((Item)UsableUpdatableItem))
                            {
                                DeletedItemsAfterUpdate++;
                            }
                        }
                    }
                    continue;
                }
                if (Item is IUpdatableItem UpdatableItem)
                {
                    UpdatableItem.Update(gameTime, Character);
                }
            }
            return DeletedItemsAfterUpdate;
        }
    }
}
