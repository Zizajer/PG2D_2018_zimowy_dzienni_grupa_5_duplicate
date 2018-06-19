using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dungeon_Crawler
{
    public class BitterPowder : Item, IUsableUpdatableItem
    {
        public bool IsCurrentlyInUse { get; private set; }
        public bool HasRecentUsageJustFinished { get; set; }
        public int RemainingUsages { get; set; }

        private float Timer;
        private float TotalEffectTime;

        public BitterPowder(ContentManager content, Vector2 position) : base(content, position)
        {
            Initialize(content);
        }
        public BitterPowder(ContentManager content) : base(content)
        {
            Initialize(content);
        }
        private void Initialize(ContentManager content)
        {
            Name = "Bitter Powder";
            Description = "Makes you superhero for a while. Ask your doctor or pharmacist for advice before taking this substance.";
            Category = "UsableAllStatsBooster";
            RemainingUsages = 1;

            TextureName = "whitePowder";
            LoadTexture(content);


        }
        public void Use(Character owner)
        {
            DefenseMultiplier = 1.3f;
            SpDefenseMultiplier = 1.3f;
            AttackMultiplier = 1.3f;
            SpAttackMultiplier = 1.3f;
            SpeedMultiplier = 1.3f;
            TimeBetweenActionsMultiplier = 0.9f;

            ApplyEffect(owner);
            TotalEffectTime = 5;

            IsCurrentlyInUse = true;
            HasRecentUsageJustFinished = false;
        }

        public void Update(GameTime gameTime, Character owner)
        {
            if (Timer < TotalEffectTime)
            {
                Timer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            else
            {
                Timer = 0;
                RevertEffect(owner);

                DefenseMultiplier = 1f;
                SpDefenseMultiplier = 1f;
                AttackMultiplier = 1f;
                SpAttackMultiplier = 1f;
                SpeedMultiplier = 1f;
                TimeBetweenActionsMultiplier = 1f;

                if (Global.random.Next(0, 2) == 0)
                {
                    owner.CurrentHealthPercent -= 15;
                }

                IsCurrentlyInUse = false;
                HasRecentUsageJustFinished = true;
            }
        }
    }
}
