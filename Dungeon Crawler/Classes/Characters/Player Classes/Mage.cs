using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using RoyT.AStar;

namespace Dungeon_Crawler
{
    class Mage : Player
    {
        int teleportResourceCost = 60;
        public Mage(ContentManager content, int cellSize, int playerCurrentMapLevel, string name) : base(content, cellSize, playerCurrentMapLevel, name)
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
            CurrentResource = Resource = 100;
            ResourceRegenerationFactor = 0.15f; //0.15 is fine
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
                if (CurrentResource >= ProjectileAttack.ManaCost)
                {
                    MouseState mouse = Mouse.GetState();
                    Vector2 tempVector = new Vector2(mouse.X, mouse.Y);
                    Vector2 mousePos = Global.Camera.ScreenToWorld(tempVector);
                    ProjectileAttack.Use(this, mousePos);
                    CurrentResource -= ProjectileAttack.ManaCost;
                }
                else
                {
                    Global.Gui.WriteToConsole("Not enough mana");
                }
            }
            pastButton = Mouse.GetState();
        }
        public override void ManageResource()
        {
            CurrentResource += ResourceRegenerationFactor;
            if (CurrentResource > Resource)
                CurrentResource = Resource;
        }
        public override void SecondaryAttack(Level level)
        {
            if (Mouse.GetState().RightButton == ButtonState.Pressed && pastButton2.RightButton == ButtonState.Released)
            {
                if (CurrentResource >= UnTargetedAttack.ManaCost)
                {
                    if(UnTargetedAttack.Use(this))
                        CurrentResource -= UnTargetedAttack.ManaCost;
                }
                else
                {
                    Global.Gui.WriteToConsole("Not enough mana");
                }
            }
            pastButton2 = Mouse.GetState();
        }
        public override void Abillity1(Level level)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.D1) && pastKey.IsKeyUp(Keys.D1))
            {
                if (CurrentResource >= teleportResourceCost)
                {
                    MouseState mouse = Mouse.GetState();
                    Vector2 tempVector = new Vector2(mouse.X, mouse.Y);
                    Vector2 mousePos = Global.Camera.ScreenToWorld(tempVector);

                    int mx = (int)Math.Floor(mousePos.X / level.cellSize);
                    int my = (int)Math.Floor(mousePos.Y / level.cellSize);

                    if (currentDirection != Directions.None)
                    {
                        Global.Gui.WriteToConsole("Can't teleport while moving");
                        return;
                    }
                    if (mx < 0 || mx >= level.map.Width || my < 0 || my >= level.map.Height)
                        return;
                    if (level.grid.GetCellCost(new Position(mx, my)) == 1.0f)
                    {
                        //play blue particle
                        Global.CombatManager.SetAnimation("Teleportation", "MagicAnim", CellX, CellY);
                        level.grid.SetCellCost(new Position(CurrentCell.X, CurrentCell.Y), 1.0f);
                        level.grid.SetCellCost(new Position(mx, my), 5.0f);
                        mousePos.X = mx * level.cellSize + level.cellSize / 2 - getWidth() / 2;
                        mousePos.Y = my * level.cellSize + level.cellSize / 2 - getHeight() / 2;
                        Position = mousePos;
                        //change player facing when ,,leaving tp"
                        if (mx > CellX)
                        {
                            _animationManager.Play(_animations["WalkRight"]);
                            currentFaceDirection = FaceDirections.Right;
                        }
                        else if (mx < CellX)
                        {
                            _animationManager.Play(_animations["WalkLeft"]);
                            currentFaceDirection = FaceDirections.Left;
                        }
                        else
                        {
                            if (my > CellY)
                            {
                                _animationManager.Play(_animations["WalkDown"]);
                                currentFaceDirection = FaceDirections.Down;
                            }
                            else
                            {
                                _animationManager.Play(_animations["WalkUp"]);
                                currentFaceDirection = FaceDirections.Up;
                            }
                        }
                        //play red particle
                        Global.CombatManager.SetAnimation("Teleportation2", "MagicAnim2", mx, my);
                        CurrentResource -= teleportResourceCost;
                        Global.Camera.CenterOn(Center);
                    }
                    else
                    {
                        Global.Gui.WriteToConsole("Can't teleport there");
                    }
                }
                else
                {
                    Global.Gui.WriteToConsole("Not enough mana");
                }
            }
            pastKey = Keyboard.GetState();
        }
        public override void Abillity2(Level level)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.D2) && pastKey2.IsKeyUp(Keys.D2))
            {

            }
            pastKey2 = Keyboard.GetState();
        }
    }
}
