using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Dungeon_Crawler
{
    class Mage : Player
    {
        public Mage(ContentManager content, int cellSize, int playerCurrentMapLevel, string name) : base(content, cellSize, playerCurrentMapLevel, name)
        {
        }
        public override void setAttacks()
        {
            ProjectileAttack = new Fireball();
            UnTargetedAttack = new EnergyBeam();
        }
        public override void BasicAttack(Level level)
        {
            if (Mouse.GetState().LeftButton == ButtonState.Pressed && pastButton.LeftButton == ButtonState.Released)
            {
                if (CurrentMana > ProjectileAttack.ManaCost)
                {
                    MouseState mouse = Mouse.GetState();
                    Vector2 tempVector = new Vector2(mouse.X, mouse.Y);
                    Vector2 mousePos = Global.Camera.ScreenToWorld(tempVector);
                    ProjectileAttack.Use(this, mousePos);
                }
                else
                {
                    Global.Gui.WriteToConsole("Not enough mana");
                }
            }
            pastButton = Mouse.GetState();
        }
        public override void SecondaryAttack(Level level)
        {
            if (Mouse.GetState().RightButton == ButtonState.Pressed && pastButton2.RightButton == ButtonState.Released)
            {
                if (CurrentMana > UnTargetedAttack.ManaCost)
                {
                    UnTargetedAttack.Use(this);
                    CurrentMana -= UnTargetedAttack.ManaCost;
                }
                else
                {
                    Global.Gui.WriteToConsole("Not enough mana");
                }
            }
            pastButton2 = Mouse.GetState();
        }
    }
}
