using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dungeon_Crawler
{
    public interface IItem
    {
        string Name { get; set; }
        string Description { get; set; }
        string Category { get; set; }

        string TextureName { get; set; }

        float HealthMultiplier { get; set; }
        float DefenseMultiplier { get; set; }
        float SpDefenseMultiplier { get; set; }
        float AttackMultiplier { get; set; }
        float SpAttackMultiplier { get; set; }
        float SpeedMultiplier { get; set; }
        float TimeBetweenActionsMultiplier { get; set; }

        float ResourceRegenerationFactorMultiplier { get; set; }
        float ExpMultiplier { get; set; }

        bool FreezeInvulnerability { get; set; }
        bool BurnInvulnerability { get; set; }

        void ApplyEffect(Character owner);

        void RevertEffect(Character owner);

        void LoadTexture(ContentManager content);

        void Draw(SpriteBatch spriteBatch);
    }
}
