using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using RoyT.AStar;
using System;
using System.Collections.Generic;

namespace Dungeon_Crawler
{
    class Warrior : Player
    {
        public int leapResourceCost = 30;
        public int leapDistance = 5;
        public int berserkerRageResourceCost = 10;
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
            UnTargetedAttack = new Annihilation();
        }

        internal void addHealth(int v)
        {
            CurrentHealth += v;
            if (CurrentHealth > Health)
                CurrentHealth = Health;
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
            if (Keyboard.GetState().IsKeyDown(Keys.D1) && pastKey.IsKeyUp(Keys.D1))
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
                        Global.Gui.WriteToConsole("Can't leap while moving");
                        return;
                    }
                    if (mx < 0 || mx >= level.map.Width || my < 0 || my >= level.map.Height)
                        return;

                    if (Global.CombatManager.DistanceBetween2Points(CellX, CellY, mx, my) <= leapDistance)
                    {
                        if (level.grid.GetCellCost(new Position(mx, my)) == 1.0f)
                        {
                            Leap(level, mx, my);
                        }
                        else
                        {
                            Global.Gui.WriteToConsole("Can't leap there");
                        }
                    }
                    else
                    {
                        Global.Gui.WriteToConsole("Can't leap that far");
                    }
                        
                }
                else
                {
                    Global.Gui.WriteToConsole("Not enough rage");
                }
            }
            pastKey = Keyboard.GetState();
        }

        private void Leap(Level level, int mx, int my)
        {
            Vector2 mousePos;
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
            Global.CombatManager.SetAnimationInArea("Leap2", "AnnihilationAttackAnim", mx, my, 1);
            List<Character> listOfEnemiesAround = Global.CombatManager.GetEnemiesInArea(mx, my, 1);
            if (listOfEnemiesAround.Count > 0)
            {
                foreach (Character defender in listOfEnemiesAround)
                {
                    Global.CombatManager.Attack(this, new Leap(), defender);
                }
            }
            CurrentResource -= leapResourceCost;
            Global.Camera.CenterOn(Center);
        }
        public override void Abillity2(Level level)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.D2) && pastKey2.IsKeyUp(Keys.D2))
            {
                if (CurrentResource >= berserkerRageResourceCost)
                {
                    timeBetweenActions = berserkerTimeBetweenActions;
                    isBerserkerRageOn = true;
                    isBerserkerShaderOn = true;
                    berserkerTimer = 0;
                    CurrentResource -= berserkerRageResourceCost;
                    if (CurrentResource < 0)
                        CurrentResource = 0;
                    Global.Gui.WriteToConsole("You are now in Berserker Rage");
                }
                else
                {
                    Global.Gui.WriteToConsole("Not enough rage");
                }               
            }
            pastKey2 = Keyboard.GetState();
        }
        public override void Abillity3(Level level)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.D3) && pastKey3.IsKeyUp(Keys.D3))
            {
                if (CurrentResource >= UnTargetedAttack.ManaCost)
                {
                    if (UnTargetedAttack.Use(this))
                        CurrentResource -= UnTargetedAttack.ManaCost;
                }
                else
                {
                    Global.Gui.WriteToConsole("Not enough rage");
                }
            }
            pastKey3 = Keyboard.GetState();
        }
    }
}
