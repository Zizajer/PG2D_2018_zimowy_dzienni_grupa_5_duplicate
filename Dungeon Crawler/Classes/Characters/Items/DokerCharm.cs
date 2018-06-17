using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dungeon_Crawler
{
    public class DokerCharm : UpdatableItem
    {
        public DokerCharm(ContentManager content, Vector2 position) : base(content, position)
        {
            Initialize(content);
        }
        public DokerCharm(ContentManager content) : base(content)
        {
            Initialize(content);
        }
        private void Initialize(ContentManager content)
        {
            Name = "Doker charm";
            Description = "Keeps your vital forces at steady level. Legend says that its owner will live for at least 2036 years.";
            Category = "Doker charm";

            TextureName = "dokerCharm";
            LoadTexture(content);
        }
        public override void Update(GameTime gameTime, Character character)
        {
            if (character.CurrentHealthPercent < 30)
            {
                character.CurrentHealthPercent += 0.03f;
            }
        }
    }
}

