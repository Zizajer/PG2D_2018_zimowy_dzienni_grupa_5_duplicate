using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using RoyT.AStar;
using System;

namespace Dungeon_Crawler
{
    class Warrior : Player
    {
        public int leapResourceCost = 30;
        public Warrior(ContentManager content, int cellSize, int playerCurrentMapLevel, string name) : base(content, cellSize, playerCurrentMapLevel, name)
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
            CurrentResource = 0;
            ResourceRegenerationFactor = -0.1f; //rage should decay slowly
        }
        public override void setAttacks()
        {
            BaseAttack = new Pound();
            ProjectileAttack = new ThrowWeapon();
            //UnTargetedAttack = new Annihilation();
        }

        internal void addResource(int v)
        {
            CurrentResource += v;
            if (CurrentResource > Resource)
                CurrentResource = Resource;
        }

        public override void ManageResource()
        {
            CurrentResource += ResourceRegenerationFactor;
            if (CurrentResource < 0)
                CurrentResource = 0;
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
            pastButton2 = Mouse.GetState();
        }
        public override void Abillity1(Level level)
        {
            if (Mouse.GetState().RightButton == ButtonState.Pressed && pastButton.RightButton == ButtonState.Released)
            {
                if (CurrentResource >= leapResourceCost)
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
                        CurrentResource -= leapResourceCost;
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
            pastButton = Mouse.GetState();
        }
    }
}
