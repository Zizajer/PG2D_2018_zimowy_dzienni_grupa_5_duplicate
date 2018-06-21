using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using System;
namespace Dungeon_Crawler
{
    class Ranger : Player
    {
        int invisibilityResourceCost = 10;
        private int Ability1LevelReq = 3;
        private int Ability2LevelReq = 5;
        private int Ability3LevelReq = 7;
        public Ranger(ContentManager content, int cellSize, int playerCurrentMapLevel, string name) : base(content, cellSize, playerCurrentMapLevel, name)
        {
        }

        public override void calculateBaseStatistics()
        {
            Health = CurrentHealth = 70;
            Defense = 15;
            SpDefense = 50;
            Attack = 70;
            SpAttack = 50;
            Speed = 3f;
            Resource = 100;
            CurrentResource = 50;
            ResourceRegenerationFactor = 0; //ranger doesnt regenerate focus
        }
        public override void calculateStatistics()
        {
            Health += 10;
            Defense += 5;
            SpDefense += 3;
            Attack += 5;
            SpAttack += 3;
            Speed += 0.05f;
        }
        public override void setAttacks()
        {
            ProjectileAttack = new ShootArrow();
            ProjectileAttack2 = new ThrowShuriken();
            ProjectileAttack3 = new FrostTrap();
            ProjectileAttack4 = new MultiShot2();
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
                        actionTimer = 0;
                    }
                    else
                    {
                        Global.Gui.WriteToConsole("Not enough focus");
                    }
                }
                else
                {
                    Global.Gui.WriteToConsole("Cant attack yet");
                }
            }
            //pastButton = Mouse.GetState();
        }

        public override void SecondaryAttack(Level level)
        {
            if (Mouse.GetState().RightButton == ButtonState.Pressed && pastButton2.RightButton == ButtonState.Released)
            {
                if (CurrentResource >= ProjectileAttack2.ManaCost)
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
        public override void PositionTargetedAttackFromItem(Level level)
        {
            if (ProjectileAttack4 != null)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.D4))// && pastButton.LeftButton == ButtonState.Released)
                {
                    if (actionTimer > timeBetweenActions)
                    {
                        if (CurrentResource >= ProjectileAttack.ManaCost)
                        {
                            MouseState mouse = Mouse.GetState();
                            Vector2 tempVector = new Vector2(mouse.X, mouse.Y);
                            Vector2 mousePos = Global.Camera.ScreenToWorld(tempVector);
                            ProjectileAttack4.Use(this, mousePos);
                            CurrentResource -= ProjectileAttack.ManaCost;
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
                if (isRangerInvisible)
                {
                    isRangerInvisible = false;
                    isInvisShaderOn = false;
                    Global.Gui.WriteToConsole("You are no longer invisible");
                }
                else
                {
                    if (CurrentResource >= invisibilityResourceCost)
                    {
                        isRangerInvisible = true;
                        Global.Gui.WriteToConsole("You are now invisible");
                    }
                    else
                    {
                        Global.Gui.WriteToConsole("Not enough focus");
                    }
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
                if (CurrentResource >= ProjectileAttack3.ManaCost)
                {
                    ProjectileAttack3.Use(this, Position);
                    CurrentResource -= ProjectileAttack3.ManaCost;
                }
                else
                {
                    Global.Gui.WriteToConsole("Not enough focus");
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
                if (CurrentResource >= ProjectileAttack4.ManaCost)
                {
                    MouseState mouse = Mouse.GetState();
                    Vector2 tempVector = new Vector2(mouse.X, mouse.Y);
                    Vector2 mousePos = Global.Camera.ScreenToWorld(tempVector);
                    ProjectileAttack4.Use(this, mousePos);
                    CurrentResource -= ProjectileAttack4.ManaCost;
                }
                else
                {
                    Global.Gui.WriteToConsole("Not enough focus");
                }
            }
            pastKey3 = Keyboard.GetState();
        }
    }
}
