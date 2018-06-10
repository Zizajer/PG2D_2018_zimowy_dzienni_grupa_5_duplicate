using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using RoyT.AStar;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dungeon_Crawler
{
    public abstract class Player : Character
    {
        public float Resource { get; set; }
        public float CurrentResource { get; set; }
        public int CurrentResourcePercent { get { return ((int)(CurrentResource / (double)Resource * 100)); } }
        public float ResourceRegenerationFactor { get; set; }

        public MouseState mouse;
        public int CurrentMapLevel { get; set; }

        public KeyboardState pastKey; //1
        public KeyboardState pastKey2; //2
        public KeyboardState pastKey3; //3
        public KeyboardState pastKey4; //CTRL

        //Inventory management vars
        public KeyboardState pastKey5; //F PICKING UP ITEMS
        public KeyboardState pastKey6; //Z DROPING ITEMS
        public KeyboardState pastKey7; //R SWAPING ITEMS
        public KeyboardState pastKey8; //Tab SCROLLING INVENTORY
        public KeyboardState pastKey9; //E USING ITEMS
        public KeyboardState pastKey10; //Q EXAMINE SELECTED ITEM
        public KeyboardState pastKey11; //P FIRE EXTUINGISHER
        public short SelectedItem = -1; // -1 == No item in inventory yet
        public int inventoryPickUpLimit = 5;
        public int DrankPotions = 0; //Magic var

        public MouseState pastButton; //LMB
        public MouseState pastButton2; //RMB
        public ContentManager content;

        public float actionTimer = 0;
        public float timeBetweenActions = 0.4f;

        //Attacks
        public ICharacterTargetedAttack BaseAttack;
        public IPositionTargetedAttack ProjectileAttack;
        public IPositionTargetedAttack ProjectileAttack2;
        public IPositionTargetedAttack ProjectileAttack3;
        public IPositionTargetedAttack ProjectileAttack4;
        public IUnTargetedAttack UnTargetedAttack;
        public IUnTargetedAttack UnTargetedAttack2;
        public IUnTargetedAttack UnTargetedAttack3;

        public bool isRangerInvisible = false;
        public float invisDecay = 0.3f;

        public bool isBerserkerRageOn = false;
        public float berserkerTimer;
        public float howLongShouldBerserkerWork = 10f;
        public float normalTimeBetweenActions = 0.4f;
        public float berserkerTimeBetweenActions = 0.2f;

        public Player(ContentManager content, int cellSize, int playerCurrentMapLevel, string name)
        {
            Level = 1;
            calculateStatistics();
            currentActionState = ActionState.Standing;
            currentHealthState = HealthState.Normal;

            _animations = new Dictionary<string, Animation>()
            {
                {"WalkUp",new Animation(content.Load<Texture2D>("player/"+Global.playerClass+"/Walkingup"),3 )},
                {"WalkDown",new Animation(content.Load<Texture2D>("player/"+Global.playerClass+"/WalkingDown"),3 )},
                {"WalkLeft",new Animation(content.Load<Texture2D>("player/"+Global.playerClass+"/WalkingLeft"),3 )},
                {"WalkRight",new Animation(content.Load<Texture2D>("player/"+Global.playerClass+"/WalkingRight"),3 )}
            };

            this.content = content;
            CurrentMapLevel = playerCurrentMapLevel;
            _animationManager = new AnimationManager(_animations.First().Value);
            Name = name;

            Inventory = new List<Item>();

            setAttacks();
        }

        public abstract void setAttacks();
        public abstract void BasicAttack(Level level);
        public abstract void SecondaryAttack(Level level);
        public abstract void Abillity1(Level level);
        public abstract void Abillity2(Level level);
        public abstract void Abillity3(Level level);
        public abstract void ManageResource();

        public override void Update(GameTime gameTime, Level level, GraphicsDevice graphicsDevice)
        {
            CellX = (int)Math.Floor(Center.X / level.cellSize);
            CellY = (int)Math.Floor(Center.Y / level.cellSize);
            if (CellX > 0 && CellX < level.map.Width && CellY > 0 && CellY < level.map.Height)
            {
                CurrentCell = level.map.GetCell(CellX, CellY);
            }

            if (!isRangerInvisible) {
                level.map.ComputeFov(CellX, CellY, 15, true);
                isInvisShaderOn = false;
            }
            else
            {
                level.map.ComputeFov(0, 0, 1, false);
                isInvisShaderOn = true;
                CurrentResource -= invisDecay;
                invisDecay += 0.02f;
                if (CurrentResource < 0)
                {
                    Global.Gui.WriteToConsole("You are no longer invisible");
                    CurrentResource = 0;
                    isRangerInvisible = false;
                    isInvisShaderOn = false;
                    invisDecay = 0.3f;
                }

            }
            
            actionTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (isBerserkerRageOn)
            {
                berserkerTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

                if(berserkerTimer > howLongShouldBerserkerWork)
                {
                    timeBetweenActions = normalTimeBetweenActions;
                    isBerserkerRageOn = false;
                    isBerserkerShaderOn = false;
                    Global.Gui.WriteToConsole("You are no longer in Berserker Rage");
                    berserkerTimer = 0;
                }
            }


            HandleHitState(gameTime);
            HandleHealthState(gameTime);

            ManageResource();

            if (currentHealthState != HealthState.Freeze)
            {
                if (currentActionState == ActionState.Standing)
                {
                    if (ChangeDirection(level))
                    {
                        return;
                    }
                    else
                    {
                        currentDirection = GetDirection();
                    }

                    if (currentDirection != (int)Directions.None)
                    {
                        RogueSharp.Cell futureNextCell = Collision.getCellFromDirection(CurrentCell, currentDirection, level);
                        if (CellX > 0 && CellX < level.map.Width && CellY > 0 && CellY < level.map.Height)
                        {
                            if (!Collision.checkCollisionInGivenCell(futureNextCell, level, graphicsDevice))
                            {
                                NextCell = futureNextCell;
                                currentActionState = ActionState.Moving;
                                level.grid.SetCellCost(new Position(CurrentCell.X, CurrentCell.Y), 1.0f);
                                level.grid.SetCellCost(new Position(NextCell.X, NextCell.Y), 5.0f);
                            }
                            else
                            {
                                //there is a collision in current direction
                                //we check if it is one of joined directions (top-left top-right bottom-left bottom-right)
                                //we try separate direction (for top-left we should try top, then left)

                                List<Character.Directions> dirList = Collision.checkIfOneOfDoubleDirectionsIsOk(CurrentCell, currentDirection, level, graphicsDevice);
                                if (dirList.Count > 0)
                                {
                                    foreach (Character.Directions newdir in dirList)
                                    {
                                        RogueSharp.Cell futureNextCell2 = Collision.getCellFromDirection(CurrentCell, newdir, level);
                                        if (CellX > 0 && CellX < level.map.Width && CellY > 0 && CellY < level.map.Height)
                                        {
                                            if (!Collision.checkCollisionInGivenCell(futureNextCell2, level, graphicsDevice))
                                            {
                                                NextCell = futureNextCell2;
                                                currentActionState = ActionState.Moving;
                                                level.grid.SetCellCost(new Position(CurrentCell.X, CurrentCell.Y), 1.0f);
                                                level.grid.SetCellCost(new Position(NextCell.X, NextCell.Y), 5.0f);
                                                break;
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    Global.Gui.WriteToConsole("Cant go there");
                                }
                            }
                        }
                        else
                        {
                            Global.Gui.WriteToConsole("Cant go there");
                        }
                    }
                    
                }
                else //Moving
                {
                    if (isCenterOfGivenCell(NextCell, level, graphicsDevice))
                    {
                        currentActionState = ActionState.Standing;
                    }
                    else
                    {
                        MoveToCenterOfGivenCell(NextCell, level, graphicsDevice);
                        Global.Camera.CenterOn(Center);
                    }
                }

                BasicAttack(level);
                SecondaryAttack(level);
                Abillity1(level);
                Abillity2(level);
                Abillity3(level);
                CoreAbility(level);

                ExamineSelectedItem();
                DropItem(level, SelectedItem);
                TakeItem(level, graphicsDevice);
                SwapItem(level, SelectedItem, graphicsDevice);
                SelectItem();
                UseItem(SelectedItem);

                SetAnimations();
                _animationManager.Update(gameTime);
                Position += Velocity;
                Velocity = Vector2.Zero;
            }
        }

        public void CoreAbility(Level level)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.P) && pastKey11.IsKeyUp(Keys.P))
            {
                if (DrankPotions >= 10)
                {
                    Global.CombatManager.SetAnimation("??", "Wtf", CellX, CellY);
                    Global.Gui.WriteToConsole(Name + " used his pee pee... and he accidentally peed himself!");
                    if (currentHealthState == HealthState.Burn)
                    {
                        healthStateTimer = 9999;
                    }
                    DrankPotions = 0;
                }
                else
                {
                    Global.Gui.WriteToConsole("Not yet...");
                }
            }
            pastKey11 = Keyboard.GetState();
        }

        public bool ChangeDirection(Level level)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.LeftControl))
            {
                if (Keyboard.GetState().IsKeyDown(Keys.W))
                {
                    _animationManager.Play(_animations["WalkUp"]);
                    currentFaceDirection = FaceDirections.Up;
                    return true;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.S))
                {
                    _animationManager.Play(_animations["WalkDown"]);
                    currentFaceDirection = FaceDirections.Down;
                    return true;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.A))
                {
                    _animationManager.Play(_animations["WalkLeft"]);
                    currentFaceDirection = FaceDirections.Left;
                    return true;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.D))
                {
                    _animationManager.Play(_animations["WalkRight"]);
                    currentFaceDirection = FaceDirections.Right;
                    return true;
                }
            }
            return false;
        }

        public virtual Directions GetDirection()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.W) && Keyboard.GetState().IsKeyDown(Keys.A))
                return Directions.TopLeft;

            if (Keyboard.GetState().IsKeyDown(Keys.W) && Keyboard.GetState().IsKeyDown(Keys.D))
                return Directions.TopRight;

            if (Keyboard.GetState().IsKeyDown(Keys.S) && Keyboard.GetState().IsKeyDown(Keys.A))
                return Directions.BottomLeft;

            if (Keyboard.GetState().IsKeyDown(Keys.S) && Keyboard.GetState().IsKeyDown(Keys.D))
                return Directions.BottomRight;

            if (Keyboard.GetState().IsKeyDown(Keys.W))
                return Directions.Top;

            if (Keyboard.GetState().IsKeyDown(Keys.S))
                return Directions.Bottom;

            if (Keyboard.GetState().IsKeyDown(Keys.A))
                return Directions.Left;

            if (Keyboard.GetState().IsKeyDown(Keys.D))
                return Directions.Right;

            return Directions.None;
        }
        public override bool TakeItem(Level level, GraphicsDevice graphicsDevice)
        {
            bool IsItemTaken = false;
            if (Keyboard.GetState().IsKeyDown(Keys.F) && pastKey5.IsKeyUp(Keys.F)) {
                if (Inventory.Count < inventoryPickUpLimit)
                {
                    if (base.TakeItem(level, graphicsDevice))
                    {
                        Global.Gui.WriteToConsole("You picked up " + Inventory.ElementAt(Inventory.Count-1).Name);
                        IsItemTaken = true;
                        if (SelectedItem == -1)
                        {
                            SelectedItem = 0;
                        }
                    }
                }
                else
                {
                    Global.Gui.WriteToConsole("You cant pick up more items");
                }
               
            }
            pastKey5 = Keyboard.GetState();
            return IsItemTaken;
        }

        public override void DropItem(Level level, int i)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Z) && pastKey6.IsKeyUp(Keys.Z))
            {
                if (SelectedItem != -1)
                {
                    if (SelectedItem == 0)
                    {
                        if (Inventory.Count == 1)
                        {
                            SelectedItem = -1;
                        }
                    }
                    else
                    {
                        SelectedItem--;
                    }
                    Global.Gui.WriteToConsole("You dropped " + Inventory.ElementAt(i).Name);
                    base.DropItem(level, i);
                    
                }
                else
                {
                    Global.Gui.WriteToConsole("You dont have any items to drop");
                }
            }
            pastKey6 = Keyboard.GetState();
        }

        public void SwapItem(Level level, int i, GraphicsDevice graphicsDevice)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.R) && pastKey7.IsKeyUp(Keys.R))
            {
                if (SelectedItem != -1)
                {
                    if (base.TakeItem(level, graphicsDevice))
                    {
                        Global.Gui.WriteToConsole("You swapped " + Inventory.ElementAt(i).Name + " for " + Inventory.ElementAt(Inventory.Count - 1).Name);
                        base.DropItem(level, i);
                    }
                    else
                    {
                        Global.Gui.WriteToConsole("Nothing on the floor to swap for");
                    }
                }
                else
                {
                    Global.Gui.WriteToConsole("You dont have any items");
                }
            }
            pastKey7 = Keyboard.GetState();
        }

        public void SelectItem()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Tab) && pastKey8.IsKeyUp(Keys.Tab))
            {
                if (Inventory.Count > 0)
                {
                    if (SelectedItem + 1 < Inventory.Count)
                    {
                        SelectedItem++;
                    }
                    else
                    {
                        SelectedItem = 0;
                    }
                }
                else
                {
                    Global.Gui.WriteToConsole("You dont have any items");
                }
                
            }
            pastKey8 = Keyboard.GetState();
        }

        public void UseItem(int i)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.E) && pastKey9.IsKeyUp(Keys.E))
            {
                if (SelectedItem != -1)
                {
                    Item Item = Inventory[i];
                    if (Item is UsableItem)
                    {
                        if (SelectedItem == 0)
                        {
                            if (Inventory.Count == 1)
                            {
                                SelectedItem = -1;
                            }
                        }
                        else
                        {
                            SelectedItem--;
                        }
                        ((UsableItem)Item).Use(this);

                        if (Item.Category.Equals("Potion"))
                        {
                            DrankPotions++;
                        }

                        Inventory.RemoveAt(i);
                    }
                    else
                    {
                        Global.Gui.WriteToConsole("You can't use this item.");
                    }
                }
            }
            pastKey9 = Keyboard.GetState();
        }
        public void ExamineSelectedItem()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Q) && pastKey10.IsKeyUp(Keys.Q))
            {
                if (SelectedItem != -1)
                {
                    Global.Gui.WriteToConsole(Inventory.ElementAt(SelectedItem).Description);
                }
                else
                {
                    Global.Gui.WriteToConsole("Nothing to examine");
                }

            }
            pastKey10 = Keyboard.GetState();
        }
    }
}
