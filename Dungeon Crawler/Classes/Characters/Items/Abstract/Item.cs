﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dungeon_Crawler
{
    public abstract class Item : Sprite, IItem
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }

        public string TextureName { get; set; }

        public float HealthMultiplier { get; set; } = 1f;
        public float DefenseMultiplier { get; set; } = 1f;
        public float SpDefenseMultiplier { get; set; } = 1f;
        public float AttackMultiplier { get; set; } = 1f;
        public float SpAttackMultiplier { get; set; } = 1f;
        public float SpeedMultiplier { get; set; } = 1f;
        public float TimeBetweenActionsMultiplier { get; set; } = 1f;

        public float ResourceRegenerationFactorMultiplier { get; set; } = 1f;
        public float ExpMultiplier { get; set; } = 1f;

        public bool FreezeInvulnerability { get; set; } = false;
        public bool BurnInvulnerability { get; set; } = false;

        public Item(ContentManager content, Vector2 position) : base(position) { }
        public Item(ContentManager content) { }

        public void ApplyEffect(Character owner)
        {
            owner.Health = owner.Health * HealthMultiplier;
            owner.Defense = (int)Math.Ceiling(owner.Defense * DefenseMultiplier);
            owner.SpDefense = (int)Math.Ceiling(owner.SpDefense * SpDefenseMultiplier);
            owner.Attack = (int)Math.Ceiling(owner.Attack * AttackMultiplier);
            owner.SpAttack = (int)Math.Ceiling(owner.SpAttack * SpAttackMultiplier);
            owner.Speed = owner.Speed * SpeedMultiplier;
            if (owner is Player)
            {
                Player p = (Player) owner;
                p.ResourceRegenerationFactor = p.ResourceRegenerationFactor * ResourceRegenerationFactorMultiplier;
                p.timeBetweenActions = p.timeBetweenActions * TimeBetweenActionsMultiplier;
                //TODO: Add this property to Character class
                //((Player)Owner).ExpMultiplier = ExpMultiplier;
            }
            //TODO: Add these properties to Character class
            //Owner.FreezeInvulnerability = FreezeInvulnerability;
            //Owner.BurnInvulnerability = BurnInvulnerability;
        }
        public void RevertEffect(Character owner)
        {
            owner.Health = owner.Health / HealthMultiplier;
            owner.Defense = (int)Math.Floor(owner.Defense / DefenseMultiplier);
            owner.SpDefense = (int)Math.Floor(owner.SpDefense / SpDefenseMultiplier);
            owner.Attack = (int)Math.Floor(owner.Attack / AttackMultiplier);
            owner.SpAttack = (int)Math.Floor(owner.SpAttack / SpAttackMultiplier);
            owner.Speed = owner.Speed / SpeedMultiplier;
            if (owner is Player)
            {
                Player p = (Player)owner;
                p.ResourceRegenerationFactor = p.ResourceRegenerationFactor / ResourceRegenerationFactorMultiplier;
                p.timeBetweenActions = p.timeBetweenActions / TimeBetweenActionsMultiplier;
                //TODO: Add this property to Character class
                //((Player)Owner).ExpMultiplier = 1f;
            }
            /* TODO: Add these properties to Character class
            if (FreezeInvulnerability == true)
            {
                Owner.FreezeInvulnerability = false;
            }
            //Owner.FreezeInvulnerability *= HealthMultiplier;
            if (BurnInvulnerability == true)
            {
                Owner.BurnInvulnerability = false;
            }
            */
        }

        public void LoadTexture(ContentManager content)
        {
            Texture = content.Load<Texture2D>("items/" + TextureName);
            TextureData =
               new Color[Texture.Width * Texture.Height];
            Texture.GetData(TextureData);
            Origin = new Vector2(Texture.Width / 2, Texture.Height / 2);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Position, null, Color.White, 0.0f, Vector2.One, Size, SpriteEffects.None, Layers.Items);
        }
    }
}
