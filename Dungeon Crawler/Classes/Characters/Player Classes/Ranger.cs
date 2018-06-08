using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using System;
namespace Dungeon_Crawler
{
    class Ranger : Player
    {
        public Ranger(ContentManager content, int cellSize, int playerCurrentMapLevel, string name) : base(content, cellSize, playerCurrentMapLevel, name)
        {
        }
        public override void calculateStatistics()
        {
            Health = CurrentHealth = 70 + Level * 10;
            Defense = 15 + Level * 3;
            SpDefense = 70 + Level * 5;
            Attack = (int)Math.Floor(70 + Level * 2.5);
            SpAttack = 70 + Level * 3;

            Speed = 4f;
            Resource = 100;
            CurrentResource = 50;
            ResourceRegenerationFactor = 0; //ranger doesnt regenerate focus
        }
        public override void setAttacks()
        {
            ProjectileAttack = new ShootArrow();
            ProjectileAttack2 = new PiercingArrow();
            //UnTargetedAttack = new Annihilation();
        }
        public override void BasicAttack(Level level)
        {
            if (Mouse.GetState().LeftButton == ButtonState.Pressed && pastButton.LeftButton == ButtonState.Released)
            {
                if (CurrentResource > ProjectileAttack.ManaCost)
                {
                    MouseState mouse = Mouse.GetState();
                    Vector2 tempVector = new Vector2(mouse.X, mouse.Y);
                    Vector2 mousePos = Global.Camera.ScreenToWorld(tempVector);
                    ProjectileAttack.Use(this, mousePos);
                }
                else
                {
                    Global.Gui.WriteToConsole("Not enough focus");
                }
            }
            pastButton = Mouse.GetState();
        }

        internal void addResource(int v)
        {
            CurrentResource += v;
            if (CurrentResource > Resource)
                CurrentResource = Resource;
        }

        public override void ManageResource()
        {

        }

        public override void SecondaryAttack(Level level)
        {
            if (Mouse.GetState().RightButton == ButtonState.Pressed && pastButton2.RightButton == ButtonState.Released)
            {
                if (CurrentResource > ProjectileAttack2.ManaCost)
                {
                    MouseState mouse = Mouse.GetState();
                    Vector2 tempVector = new Vector2(mouse.X, mouse.Y);
                    Vector2 mousePos = Global.Camera.ScreenToWorld(tempVector);
                    ProjectileAttack2.Use(this, mousePos);
                    CurrentResource -= ProjectileAttack2.ManaCost;
                }
                else
                {
                    Global.Gui.WriteToConsole("Not enough focus");
                }
            }
            pastButton2 = Mouse.GetState();
        }
    }
}
