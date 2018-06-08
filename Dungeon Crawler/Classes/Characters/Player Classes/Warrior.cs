using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Dungeon_Crawler
{
    class Warrior : Player
    {
        public Warrior(ContentManager content, int cellSize, int playerCurrentMapLevel, string name) : base(content, cellSize, playerCurrentMapLevel, name)
        {
        }
        public override void setAttacks()
        {
            BaseAttack = new Pound();
            ProjectileAttack = new ThrowWeapon();
            //UnTargetedAttack = new Annihilation();
        }

        public override void BasicAttack(Level level)
        {
            if (Mouse.GetState().LeftButton == ButtonState.Pressed && pastButton.LeftButton == ButtonState.Released)
            {
                if (actionTimer > timeBetweenActions)
                {
                    MouseState mouse = Mouse.GetState();
                    Vector2 tempVector = new Vector2(mouse.X, mouse.Y);
                    Vector2 mousePos = Global.Camera.ScreenToWorld(tempVector);
                    int mx = (int)Math.Floor(mousePos.X / level.cellSize);
                    int my = (int)Math.Floor(mousePos.Y / level.cellSize);

                    if (mx < 0 || mx >= level.map.Width || my < 0 || my >= level.map.Height)
                        return;

                    if (level.isBossLevel)
                    {
                        if (Global.CombatManager.IsEnemyAt(mx, my))
                        {
                            Character enemy = Global.CombatManager.EnemyAt(mx, my);
                            if (Global.CombatManager.DistanceBetween2Points(CellX, CellY, mx, my) <= 1)
                            {
                                BaseAttack.Use(this, enemy);
                                actionTimer = 0;
                            }
                            else
                            {
                                Global.Gui.WriteToConsole("You are too far away from this enemy");
                                return;
                            }
                        }
                        else
                        {
                            Global.Gui.WriteToConsole("There is no enemy here");
                            return;
                        }

                    }
                    else
                    {
                        if (Global.CombatManager.IsEnemyAt(mx, my))
                        {
                            Character enemy = Global.CombatManager.EnemyAt(mx, my);
                            if (Global.CombatManager.DistanceBetween2Points(CellX, CellY, mx, my) <= 1)
                            {
                                BaseAttack.Use(this, enemy);
                                actionTimer = 0;
                            }
                            else
                            {
                                Global.Gui.WriteToConsole("You are too far away from this enemy");
                                return;
                            }
                        }
                        else
                        {
                            Global.Gui.WriteToConsole("There is no enemy here");
                            return;
                        }
                    }
                }
                else
                {
                    Global.Gui.WriteToConsole("Cant attack yet");
                }
            }
            pastButton = Mouse.GetState();
        }

        public override void SecondaryAttack(Level level)
        {
            if (Mouse.GetState().RightButton == ButtonState.Pressed && pastButton2.RightButton == ButtonState.Released)
            {
                if (CurrentMana > ProjectileAttack.ManaCost)
                {
                    MouseState mouse = Mouse.GetState();
                    Vector2 tempVector = new Vector2(mouse.X, mouse.Y);
                    Vector2 mousePos = Global.Camera.ScreenToWorld(tempVector);
                    ProjectileAttack.Use(this, mousePos);
                    CurrentMana -= ProjectileAttack.ManaCost;
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
