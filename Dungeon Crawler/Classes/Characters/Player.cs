using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using RoyT.AStar;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        public KeyboardState pastKey11; //P FIRE EXTUINGISHER
        public KeyboardState pastKey12; //` Tilde stats toggle

        public short SelectedItem = -1; // -1 == No item in inventory yet
        public int inventoryPickUpLimit = 5;
        public int DrankPotions = 0; //Magic var

        public readonly float MaxTotalMultiplier = 1.5f;

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
        public IPositionTargetedAttack ItemProjectileAttack;
        public IUnTargetedAttack UnTargetedAttack;
        public IUnTargetedAttack UnTargetedAttack2;
        public IUnTargetedAttack UnTargetedAttack3;

        public bool isRangerInvisible = false;
        public float invisDecay = 0.3f;

        public bool isBerserkerRageOn = false;
        public float berserkerTimer;
        public float howLongShouldBerserkerWork = 10f;
        public float normalTimeBetweenActions;
        public float berserkerTimeBetweenActions = 0.3f;

        public override IInventoryManager InventoryManager { get { return InventoryManagerFacade.PlayerInventoryManager; } }
        public PlayerInventoryManagerFacade InventoryManagerFacade;

        public Player(ContentManager content, int cellSize, int playerCurrentMapLevel, string name)
        {
            Level = 1;
            calculateBaseStatistics();
            Experience = 0;

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
            normalTimeBetweenActions = timeBetweenActions;
            InventoryManagerFacade = new PlayerInventoryManagerFacade(this, new List<Item>());

            setAttacks();
        }
        public abstract void setAttacks();
        public abstract void calculateStatistics();
        public abstract void BasicAttack(Level level);
        public abstract void SecondaryAttack(Level level);
        public abstract void PositionTargetedAttackFromItem(Level level);
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
            if (CurrentHealth > 0)
            {
                if (calculateExpForNextLevel(Level + 1) < Experience)
                {
                    Global.SoundManager.playerLevelUP.Play();
                    calculateStatistics();
                    CurrentResource = Resource;
                    CurrentHealth = Health;
                    Global.Gui.nextLevel(Level);
                    Level++;
                }
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

                if (berserkerTimer > howLongShouldBerserkerWork)
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
                PositionTargetedAttackFromItem(level);
                Abillity1(level);
                Abillity2(level);
                Abillity3(level);

                CoreAbility(level);

                ToggleStatsAllocationMenu();
                ToggleStats();

                SetAnimations();
                _animationManager.Update(gameTime);
                Position += Velocity;
                Velocity = Vector2.Zero;
            }
            InventoryManagerFacade.Update(gameTime, graphicsDevice, level);
        }

        private int calculateExpForNextLevel(int x)
        {
            return (int)Math.Ceiling(50 * Math.Pow(x, 3) / 3 - 100 * Math.Pow(x, 2) + 850 * x / 3 - 200);
        }

        private void ToggleStats()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.OemTilde) && pastKey12.IsKeyUp(Keys.OemTilde)) { 
                Global.Gui.drawingStats = !Global.Gui.drawingStats;
            }

            pastKey12 = Keyboard.GetState();
        }

        private void ToggleStatsAllocationMenu()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.O) && Global.pastKey13.IsKeyUp(Keys.O))
            {
                if (Global.CurrentGameState == Global.Gamestates.isGameActive)
                {
                    Global.CurrentGameState = Global.Gamestates.isStatsMenu;
                }
            }

            Global.pastKey13 = Keyboard.GetState();
        }

        public void CoreAbility(Level level)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.P) && pastKey11.IsKeyUp(Keys.P))
            {
                if (DrankPotions >= 10)
                {
                    Global.SoundManager.playerPiss.Play();
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
    }
}
