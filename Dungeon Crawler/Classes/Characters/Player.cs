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
    public class Player : Character
    {
        public float Mana { get; set; }
        public float CurrentMana { get; set; }
        public int CurrentManaPercent { get { return ((int)(CurrentMana / (double)Mana * 100)); } }

        public int teleportCost = 10;
        public int fireballCost = 10;
        public int exoriCost = 20;
        public Dictionary<string, Animation> _animationsExori;
        public int maxFireballsOnScreen = 20;
        public MouseState mouse;
        public float rotation;
        public List<Item> inventory { get; set; }
        public int CurrentMapLevel { get; set; }
        KeyboardState pastKey;
        KeyboardState pastKey2;
        MouseState pastButton;
        MouseState pastButton2;
        ContentManager content;

        float cantGoThereTimer = 0;
        float timeBetweencantGoThere = 1f;
        float actionTimer = 0;
        float timeBetweenActions=0.4f;

        public Player(ContentManager content, int cellSize, int playerCurrentMapLevel, string name)
        {
            Level = 1;
            Speed = 4f; // TODO: move it to calculateStatistics();
            CurrentMana = Mana = 100; // TODO: move it to calculateStatistics();
            calculateStatistics();

            currentState = State.Standing;
            _animations = new Dictionary<string, Animation>()
            {
                {"WalkUp",new Animation(content.Load<Texture2D>("player/Walkingup"),3 )},
                {"WalkDown",new Animation(content.Load<Texture2D>("player/WalkingDown"),3 )},
                {"WalkLeft",new Animation(content.Load<Texture2D>("player/WalkingLeft"),3 )},
                {"WalkRight",new Animation(content.Load<Texture2D>("player/WalkingRight"),3 )}
            };

            this.content = content;
            CurrentMapLevel = playerCurrentMapLevel;
            inventory = new List<Item>();
            _animationManager = new AnimationManager(_animations.First().Value);
            Name = name;
        }

        public override void calculateStatistics()
        {
            Health = CurrentHealth = 70 + Level * 10;
            Defense = 70 + Level * 3;
            SpDefense = 70 + Level * 5;
            Attack = (int)Math.Floor(70 + Level * 2.5);
            SpAttack = 70 + Level * 3;
            //Speed = todo..
        }

        public bool IsHitByProjectile(Level level, GraphicsDevice graphicsDevice)
        {
            EnemyProjectile projectile = null;
            for (int i = 0; i < level.enemyProjectiles.Count; i++)
            {
                projectile = level.enemyProjectiles[i];
                if (Collision.checkCollision(getRectangle(), this, projectile, graphicsDevice))
                {
                    isHitShaderOn = true;
                    projectile.isPlayerHit = true;
                    return true;
                }
            }
            return false;
        }
        public void Fireball(Level level)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Space) && pastKey.IsKeyUp(Keys.Space))
            {
                if (level.playerProjectiles.Count() < maxFireballsOnScreen && CurrentMana> fireballCost)
                {
                    MouseState mouse = Mouse.GetState();
                    Vector2 tempVector = new Vector2(mouse.X, mouse.Y);
                    Vector2 mousePos = Global.Camera.ScreenToWorld(tempVector);
                    float distanceX = mousePos.X - this.Position.X;
                    float distanceY = mousePos.Y - this.Position.Y;

                    rotation = (float)Math.Atan2(distanceY, distanceX);
                    Vector2 tempVelocity = new Vector2((float)Math.Cos(rotation) * 5f, ((float)Math.Sin(rotation)) * 5f) +Velocity/3;
                    Vector2 tempPosition = Center + tempVelocity * 3;

                    PlayerProjectile newProjectile = new PlayerProjectile(tempVelocity, tempPosition, level.fireball, rotation);

                    level.playerProjectiles.Add(newProjectile);
                    CurrentMana = CurrentMana - fireballCost;
                    Global.SoundManager.playPew();
                }
                else
                {
                    Global.Gui.WriteToConsole("Not enough mana");
                }
            }
            pastKey = Keyboard.GetState();
        }

        public void Teleport(Level level,GraphicsDevice graphicsDevice)
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

        public void AutoAttack(Level level, GraphicsDevice graphicsDevice, GameTime gameTime)
        {
            if (Mouse.GetState().LeftButton == ButtonState.Pressed && pastButton2.LeftButton == ButtonState.Released)
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

                    List<Character> listOfEnemiesAround = Global.CombatManager.IsEnemyInCellAround(CellX, CellY);

                    if(listOfEnemiesAround.Count == 0)
                    {
                        Global.Gui.WriteToConsole("There arent any enemies nearby");
                        return;
                    }
                    //boss case
                    if (listOfEnemiesAround.Count == 1)
                    {
                        Character enemy = listOfEnemiesAround[0];
                        if (enemy is Boss)
                        {
                            level.attackAnimations.Add(new AttackAnimation(content, mx, my, level.cellSize, gameTime));
                            Global.CombatManager.Attack(this, enemy);
                            actionTimer = 0;
                        }
                    }

                    //normal enemy level case
                    if (Global.CombatManager.IsEnemyAt(mx, my))
                    {
                        Character enemy = Global.CombatManager.EnemyAt(mx, my);
                        if (listOfEnemiesAround.Contains(enemy))
                        {
                            level.attackAnimations.Add(new AttackAnimation(content, enemy.CellX, enemy.CellY, level.cellSize, gameTime));
                            Global.CombatManager.Attack(this, enemy);
                            actionTimer = 0;
                        }
                    }
                }
                else
                {
                    Global.Gui.WriteToConsole("Cant attack yet");
                }
            }
            pastButton2 = Mouse.GetState();
        }

        public void Exori(Level level, GraphicsDevice graphicsDevice, GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.LeftShift) && pastKey2.IsKeyUp(Keys.LeftShift))
            {
                if (CurrentMana > exoriCost)
                {
                    List <RogueSharp.Cell> cellsAroundTheCellList = level.map.GetCellsInArea(CellX, CellY, 1).ToList();
                    foreach (RogueSharp.Cell cell in cellsAroundTheCellList)
                    {
                        if(cell.IsWalkable)
                            level.attackAnimations.Add(new AttackAnimation(content, cell.X, cell.Y, level.cellSize, gameTime));
                    }
                    
                    List<Character> listOfEnemiesAround = Global.CombatManager.IsEnemyInCellAround(CellX, CellY);
                    if (listOfEnemiesAround.Count > 0)
                    {
                        foreach (Character enemy in listOfEnemiesAround)
                        {
                            Global.CombatManager.Attack(this, enemy);
                        }
                    }
                    CurrentMana = CurrentMana - exoriCost;
                }
                else
                {
                    Global.Gui.WriteToConsole("Not enough mana");
                }
            }
            pastKey2 = Keyboard.GetState();
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

        public override void Update(GameTime gameTime, Level level, GraphicsDevice graphicsDevice)
        {
            level.map.ComputeFov(CellX, CellY, 15, true);
            actionTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            cantGoThereTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (isHitShaderOn)
            {
                hitTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (hitTimer > howLongShouldShaderApply)
                {
                    hitTimer = 0;
                    isHitShaderOn = false;
                }
            }

            CellX = (int)Math.Floor(Center.X / level.cellSize);
            CellY = (int)Math.Floor(Center.Y / level.cellSize);
            if (CellX > 0 && CellX < level.map.Width && CellY > 0 && CellY < level.map.Height)
            {
                CurrentCell = level.map.GetCell(CellX, CellY);
            }

            if (IsHitByProjectile(level, graphicsDevice))
            {
                int damage = 5;
                CurrentHealth -= damage;
                string tempString = "Demon Oak's giant fireball hit player for " + damage;
                Global.Gui.WriteToConsole(tempString);
            }

            if (CurrentMana < 100) CurrentMana = CurrentMana + 0.15f; //0.15

            if (currentState == State.Standing)
            {
                currentDirection = GetDirection();
                if (currentDirection != (int)Directions.None)
                {
                    RogueSharp.Cell futureNextCell= Collision.getCellFromDirection(CurrentCell, currentDirection, level);
                    if (CellX > 0 && CellX < level.map.Width && CellY > 0 && CellY < level.map.Height)
                    {
                        if (!Collision.checkCollisionInGivenCell(futureNextCell, level, graphicsDevice))
                        {
                            NextCell = futureNextCell;
                            currentState = State.Moving;
                            level.grid.SetCellCost(new Position(CurrentCell.X, CurrentCell.Y), 1.0f);
                            level.grid.SetCellCost(new Position(NextCell.X, NextCell.Y), 5.0f);
                        }
                        else
                        {
                            //there is a collision in current direction
                            //we check if it is on of joined directions (top-left top-right bottom-left bottom-right)
                            //we try separate direction for top-left we should try top, then left

                            
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
                                            currentState = State.Moving;
                                            level.grid.SetCellCost(new Position(CurrentCell.X, CurrentCell.Y), 1.0f);
                                            level.grid.SetCellCost(new Position(NextCell.X, NextCell.Y), 5.0f);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (cantGoThereTimer > timeBetweencantGoThere)
                                {
                                    cantGoThereTimer = 0;
                                    Global.Gui.WriteToConsole("Cant go there");
                                }
                            }
                        }
                    }
                    else
                    {
                        Global.Gui.WriteToConsole("Cant go there");
                    }
                }
                Teleport(level, graphicsDevice);
                Fireball(level);
                AutoAttack(level, graphicsDevice, gameTime);
                Exori(level, graphicsDevice, gameTime);
            }
            else //Moving
            {
                if (isCenterOfGivenCell(NextCell, level, graphicsDevice))
                {
                    currentState = State.Standing;
                }
                else
                {
                    MoveToCenterOfGivenCell(NextCell, level, graphicsDevice);
                    Global.Camera.CenterOn(Center);
                }
                Fireball(level);
                Exori(level, graphicsDevice, gameTime);
            }
            
            SetAnimations();
            _animationManager.Update(gameTime);
            Position += Velocity;
            Velocity = Vector2.Zero;
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
