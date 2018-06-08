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
        public float Mana { get; set; }
        public float CurrentMana { get; set; }
        public int CurrentManaPercent { get { return ((int)(CurrentMana / (double)Mana * 100)); } }

        public int teleportCost = 10;

        public MouseState mouse;
        public float rotation;
        public List<Item> inventory { get; set; }
        public int CurrentMapLevel { get; set; }
        public KeyboardState pastKey;
        public KeyboardState pastKey2;
        public KeyboardState pastKey3;
        public MouseState pastButton; //LMB
        public MouseState pastButton2; //RMB
        public ContentManager content;

        public float actionTimer = 0;
        public float timeBetweenActions =0.4f;

        //Attacks
        public ICharacterTargetedAttack BaseAttack;
        public IPositionTargetedAttack ProjectileAttack;
        public IPositionTargetedAttack ProjectileAttack2;
        public IUnTargetedAttack UnTargetedAttack;

        public Player(ContentManager content, int cellSize, int playerCurrentMapLevel, string name)
        {
            Level = 1;
            Speed = 4f; // TODO: move it to calculateStatistics();
            CurrentMana = Mana = 100; // TODO: move it to calculateStatistics();
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
            inventory = new List<Item>();
            _animationManager = new AnimationManager(_animations.First().Value);
            Name = name;

            //Set attacks
            setAttacks();
        }

        public virtual void setAttacks()
        {
            BaseAttack = new Pound();
            ProjectileAttack = new ShootArrow();
            UnTargetedAttack = new Annihilation();
        }

        public override void calculateStatistics()
        {
            Health = CurrentHealth = 70 + Level * 10;
            Defense = 15 + Level * 3;
            SpDefense = 70 + Level * 5;
            Attack = (int)Math.Floor(70 + Level * 2.5);
            SpAttack = 70 + Level * 3;
            //Speed = todo..
        }

        public abstract void BasicAttack(Level level);
        public abstract void SecondaryAttack(Level level);

        public void Abillity1(Level level,GraphicsDevice graphicsDevice)
        {
            if (Mouse.GetState().RightButton == ButtonState.Pressed && pastButton.RightButton == ButtonState.Released)
            {
                if (CurrentMana > teleportCost)
                {
                    MouseState mouse = Mouse.GetState();
                    Vector2 tempVector = new Vector2(mouse.X, mouse.Y);
                    Vector2 mousePos = Global.Camera.ScreenToWorld(tempVector);

                    int mx = (int)Math.Floor(mousePos.X / level.cellSize);
                    int my = (int)Math.Floor(mousePos.Y / level.cellSize);

                    if (mx < 0 || mx >= level.map.Width || my < 0 || my >= level.map.Height)
                        return;
                    if (level.grid.GetCellCost(new Position(mx,my))==1.0f)
                    {
                        level.grid.SetCellCost(new Position(CurrentCell.X, CurrentCell.Y), 1.0f);
                        level.grid.SetCellCost(new Position(mx, my), 5.0f);
                        mousePos.X = mx * level.cellSize + level.cellSize / 2 - getWidth() / 2;
                        mousePos.Y = my * level.cellSize + level.cellSize / 2 - getHeight() / 2;
                        Position = mousePos;
                        CurrentMana = CurrentMana - teleportCost;
                        Global.Camera.CenterOn(Center);
                    }
                }
                else
                {
                    Global.Gui.WriteToConsole("Not enough mana");
                }
            }
            pastButton = Mouse.GetState();
        }

        public override void Update(GameTime gameTime, Level level, GraphicsDevice graphicsDevice)
        {
            level.map.ComputeFov(CellX, CellY, 15, true);
            actionTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            HandleHitState(gameTime);
            HandleHealthState(gameTime);

            CellX = (int)Math.Floor(Center.X / level.cellSize);
            CellY = (int)Math.Floor(Center.Y / level.cellSize);
            if (CellX > 0 && CellX < level.map.Width && CellY > 0 && CellY < level.map.Height)
            {
                CurrentCell = level.map.GetCell(CellX, CellY);
            }

            if (CurrentMana < 100) CurrentMana = CurrentMana + 0.99f; //0.15

            if (currentHealthState != HealthState.Freeze)
            {
                if (currentActionState == ActionState.Standing)
                {
                    currentDirection = GetDirection();
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
                    //Abillity1(level, graphicsDevice);
                    SecondaryAttack(level);
                    BasicAttack(level);
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

                SetAnimations();
                _animationManager.Update(gameTime);
                Position += Velocity;
                Velocity = Vector2.Zero;
            }
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

        public string getItems()
        {
            if (inventory.Count == 0) return "Inventory is empty";
            string temp = "Inventory: ";
            Item[] itemArray = inventory.ToArray();
            for (int i = 0; i < inventory.Count; i++)
            {
                temp += itemArray[i].name + ", ";
            }

            return temp;
        }
    }
}
