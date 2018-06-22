using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using RoyT.AStar;

namespace Dungeon_Crawler
{
    class Mage : Player
    {
        private int Ability1LevelReq = 3;
        private int Ability2LevelReq = 5;
        private int Ability3LevelReq = 7;
        int teleportResourceCost = 60;
        public Mage(ContentManager content, int cellSize, int playerCurrentMapLevel, string name) : base(content, cellSize, playerCurrentMapLevel, name)
        {
        }
        public override void calculateBaseStatistics()
        {
            Health = CurrentHealth = 70;
            Defense = 15;
            SpDefense = 70;
            Attack = 50;
            SpAttack = 70;
            Speed = 3f;
            CurrentResource = Resource = 100;
            ResourceRegenerationFactor = 0.15f; //0.15 is fine
        }
        public override void calculateStatistics()
        {
            Health += 5;
            Defense += 3;
            SpDefense += 5;
            Attack += 3;
            SpAttack += 3;
            Speed += 0.05f;
            Resource += 15;
            ResourceRegenerationFactor += 0.01f;
        }
        public override void setAttacks()
        {
            ProjectileAttack = new Fireball();
            UnTargetedAttack = new EnergyBeam();
            UnTargetedAttack2 = new FrostNova();
            UnTargetedAttack3 = new UltimateExplosion();
        }
        public override void BasicAttack(Level level)
        {
            if (Mouse.GetState().LeftButton == ButtonState.Pressed)// && pastButton.LeftButton == ButtonState.Released)
            {
                if (actionTimer > timeBetweenActions)
                {
                    if (CurrentResource >= ProjectileAttack.ManaCost)
                    {
                        MouseState mouse = Mouse.GetState();
                        Vector2 tempVector = new Vector2(mouse.X, mouse.Y);
                        Vector2 mousePos = Global.Camera.ScreenToWorld(tempVector);
                        ProjectileAttack.Use(this, mousePos);
                        CurrentResource -= ProjectileAttack.ManaCost;
                        actionTimer = 0;
                    }
                    else
                    {
                        Global.Gui.WriteToConsole("Not enough mana");
                    }
                }
                else
                {
                    Global.Gui.WriteToConsole("Cant attack yet");
                }
            }
            //pastButton = Mouse.GetState();
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
        public override void PositionTargetedAttackFromItem(Level level)
        {
            if (ItemProjectileAttack != null)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.D4))// && pastButton.LeftButton == ButtonState.Released)
                {
                    if (actionTimer > timeBetweenActions)
                    {
                        if (CurrentResource >= ItemProjectileAttack.ManaCost)
                        {
                            MouseState mouse = Mouse.GetState();
                            Vector2 tempVector = new Vector2(mouse.X, mouse.Y);
                            Vector2 mousePos = Global.Camera.ScreenToWorld(tempVector);
                            ItemProjectileAttack.Use(this, mousePos);
                            CurrentResource -= ItemProjectileAttack.ManaCost;
                            actionTimer = 0;
                        }
                        else
                        {
                            Global.Gui.WriteToConsole("Not enough rage");
                        }
                    }
                    else
                    {
                        Global.Gui.WriteToConsole("Cant attack yet");
                    }
                }
            }
        }
        public override void Abillity1(Level level)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.D1) && pastKey.IsKeyUp(Keys.D1))
            {
                if (Global.hardMode && Level < Ability1LevelReq)
                {
                    pastKey = Keyboard.GetState();
                    Global.Gui.WriteToConsole("You need level " + Ability1LevelReq + " to use that ability");
                    return;
                }
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
                if (Global.hardMode && Level < Ability2LevelReq)
                {
                    pastKey2 = Keyboard.GetState();
                    Global.Gui.WriteToConsole("You need level " + Ability2LevelReq + " to use that ability");
                    return;
                }
                if (CurrentResource >= UnTargetedAttack2.ManaCost)
                {
                    if (UnTargetedAttack2.Use(this))
                        CurrentResource -= UnTargetedAttack2.ManaCost;
                }
                else
                {
                    Global.Gui.WriteToConsole("Not enough mana");
                }
            }
            pastKey2 = Keyboard.GetState();
        }

        public override void Abillity3(Level level)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.D3) && pastKey3.IsKeyUp(Keys.D3))
            {
                if (Global.hardMode && Level < Ability3LevelReq)
                {
                    pastKey3 = Keyboard.GetState();
                    Global.Gui.WriteToConsole("You need level " + Ability3LevelReq + " to use that ability");
                    return;
                }
                if (CurrentResource >= UnTargetedAttack3.ManaCost)
                {
                    if (UnTargetedAttack3.Use(this))
                        CurrentResource -= UnTargetedAttack3.ManaCost;
                }
                else
                {
                    Global.Gui.WriteToConsole("Not enough mana");
                }
            }
            pastKey3 = Keyboard.GetState();
        }
    }
}
