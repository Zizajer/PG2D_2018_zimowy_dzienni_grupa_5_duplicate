using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dungeon_Crawler
{
    public interface IInventoryManager
    {
        Character Character { get; set; }

        List<Item> Inventory { get; set; }
        List<IUsableUpdatableItem> ItemsCurrentlyInUse { get; }

        bool TakeItem(Item item);
        bool TakeItem(Level level, GraphicsDevice graphicsDevice);
        void TakeItems(List<Item> items);

        bool UseItem(int i);

        bool DropItem(Level level, int i);
        void DropAllItems(Level level);

        bool DeleteItem(int i);
        bool DeleteItem(Item item);

        bool IsItemEligibleToBeTaken(Item item);
        bool IsItemEligibleToBeUsed(IUsableUpdatableItem item);

        byte UpdateItems(GameTime gameTime);
    }
}
