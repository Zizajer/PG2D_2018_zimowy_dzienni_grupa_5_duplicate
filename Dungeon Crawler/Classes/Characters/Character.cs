using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;


namespace Dungeon_Crawler
{
    public abstract class Character
    {
        public enum ActionState { Moving, Standing };
        public enum HealthState { Normal, Freeze, Burn };
        public enum Directions { None, Top, Bottom, Left, Right, TopLeft, TopRight, BottomLeft, BottomRight};
        public enum FaceDirections { Up, Down, Left, Right};
        public FaceDirections currentFaceDirection;
        public Directions currentDirection;
        public ActionState currentActionState;
        public HealthState currentHealthState;

        public string Name { get; set; }

        public int Level;
        public int Experience;

        public float Health { get; set; }
        public float CurrentHealth { get; set; }
        public int CurrentHealthPercent { get { return ((int)(CurrentHealth / Health * 100)); } }

        public int Defense { get; set; }
        public int SpDefense { get; set; }
        public int Attack { get; set; }
        public int SpAttack { get; set; }
        public float Speed { get; set; }

        public List<Item> Inventory { get; set; }

        public bool isHitShaderOn = false;
        public float hitTimer=0;
        public float howLongShouldHitShaderApply = 0.25f;

        public bool isBurnShaderOn = false;
        public bool isFreezeShaderOn = false;
        public bool isInvisShaderOn = false;
        public bool isBerserkerShaderOn = false;
        public bool isBlackShaderOn = false;

        public bool shaderHealthStatePulse = true;
        public float shaderHealthStateTimer = 0;
        public float shaderHealthStatePulseValue = 1f;

        public float healthStateTimer = 0;
        public float howLongShouldHealthStateLast = Global.random.Next(5, 10);

        public int CellX { get; set; }
        public int CellY { get; set; }
        public AnimationManager _animationManager;
        protected Dictionary<String, Animation> _animations;
        public Vector2 _position;
        public Vector2 Position
        {
            get { return _position; }
            set
            {
                _position = value;
                if (_animationManager != null)
                    _animationManager.Position = _position;
            }
        }
        public Vector2 Center
        {
            get { return new Vector2(Position.X + getWidth() / 2, Position.Y + getHeight() / 2); }
        }

        public RogueSharp.Cell CurrentCell;
        public RogueSharp.Cell NextCell;
        public Vector2 Velocity;

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, Global.Camera.TranslationMatrix);
            if (isInvisShaderOn)
            {
                Global.Effects.InvisEffect.CurrentTechnique.Passes[0].Apply();
            }
            if (isBerserkerShaderOn)
            {
                Global.Effects.BerserkEffect.CurrentTechnique.Passes[0].Apply();
            }
            if (isBlackShaderOn)
            {
                Global.Effects.BlackEffect.CurrentTechnique.Passes[0].Apply();
            }
            if (isBurnShaderOn)
            {
                if(shaderHealthStatePulse == true)
                    Global.Effects.BurnEffect.CurrentTechnique.Passes[0].Apply();
            }
            if (isFreezeShaderOn)
            {
                if (shaderHealthStatePulse == true)
                    Global.Effects.FreezeEffect.CurrentTechnique.Passes[0].Apply();
            }
            if (isHitShaderOn)
            {
                Global.Effects.HitEffect.CurrentTechnique.Passes[0].Apply();
            }
            _animationManager.Draw(spriteBatch);
            spriteBatch.End();
        }

        public abstract void calculateBaseStatistics();

        public void HandleHitState(GameTime gameTime)
        {
            if (isHitShaderOn)
            {
                hitTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (hitTimer > howLongShouldHitShaderApply)
                {
                    hitTimer = 0;
                    isHitShaderOn = false;
                }
            }
        }

        public void HandleHealthState(GameTime gameTime)
        {
            if (currentHealthState != HealthState.Normal)
            {
                shaderHealthStateTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if(shaderHealthStatePulseValue>0.1f)
                    shaderHealthStatePulseValue -= (float)gameTime.ElapsedGameTime.TotalSeconds/7;
                if (shaderHealthStateTimer > shaderHealthStatePulseValue)
                {
                    shaderHealthStatePulse = !shaderHealthStatePulse;
                    shaderHealthStateTimer = 0;
                }
            }

            if (currentHealthState == HealthState.Freeze)
            {
                healthStateTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (healthStateTimer > howLongShouldHealthStateLast)
                {
                    healthStateTimer = 0;
                    currentHealthState = HealthState.Normal;
                    isFreezeShaderOn = false;
                    Global.Gui.WriteToConsole(Name + " is no longer frozen!");

                    shaderHealthStatePulseValue = 1f;
                    shaderHealthStatePulse = true;
                }
            }

            if (currentHealthState == HealthState.Burn)
            {
                healthStateTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                CurrentHealth -= CurrentHealth / 1000f;
                if (healthStateTimer > howLongShouldHealthStateLast)
                {
                    healthStateTimer = 0;
                    currentHealthState = HealthState.Normal;
                    isBurnShaderOn = false;
                    Global.Gui.WriteToConsole(Name + " is no longer burned!");

                    shaderHealthStatePulseValue = 1f;
                    shaderHealthStatePulse = true;
                }
            }
        }

        protected virtual void SetAnimations()
        {
            if (Velocity.X > 0)
            {
                _animationManager.Play(_animations["WalkRight"]);
                currentFaceDirection = FaceDirections.Right;
            } 
            else if (Velocity.X < 0)
            {
                _animationManager.Play(_animations["WalkLeft"]);
                currentFaceDirection = FaceDirections.Left;
            }    
            else if (Velocity.Y < 0)
            {
                _animationManager.Play(_animations["WalkUp"]);
                currentFaceDirection = FaceDirections.Up;
            }    
            else if (Velocity.Y > 0)
            {
                _animationManager.Play(_animations["WalkDown"]);
                currentFaceDirection = FaceDirections.Down;
            }    
            else 
                _animationManager.Stop();
        }
        public int getWidth()
        {
            return _animationManager._animation.FrameWidth-1;
        }
        public int getHeight()
        {
            return _animationManager._animation.FrameHeight-1;
        }

        public Rectangle getRectangle()
        {
            return new Rectangle((int)Math.Floor(Position.X), (int)Math.Floor(Position.Y),
               getWidth(), getHeight());
        }
        public Color[] getCurrentTextureData(GraphicsDevice graphicsDevice)
        {
            Texture2D multipleSpriteTexture = _animationManager._animation.Texture;

            Rectangle toCropRectangle = _animationManager.getCurrentFrameRectangle();

            Color[] singleTextureData = new Color[toCropRectangle.Width * toCropRectangle.Height];
            multipleSpriteTexture.GetData(0, toCropRectangle, singleTextureData, 0, singleTextureData.Length);

            return singleTextureData;
        }
        public abstract void Update(GameTime gameTime, Level level, GraphicsDevice graphicsDevice);

        public bool isCenterOfGivenCell(RogueSharp.Cell NextCell, Level level, GraphicsDevice graphicsDevice)
        {
            int PosX = NextCell.X * level.cellSize + level.cellSize / 2;
            int PosY = NextCell.Y * level.cellSize + level.cellSize / 2;

            if (Math.Abs(Center.Y - PosY) <= Speed && Math.Abs(Center.X - PosX) <= Speed)
                return true;
            else
                return false;
        }

        public void MoveToCenterOfGivenCell(RogueSharp.Cell NextCell, Level level, GraphicsDevice graphicsDevice)
        {
            int PosX = NextCell.X * level.cellSize + level.cellSize / 2;
            int PosY = NextCell.Y * level.cellSize + level.cellSize / 2;

            if (Math.Abs(Center.Y - PosY) > Speed)
            {
                if (Center.Y - PosY > Speed)
                {
                    Move(Directions.Top, level, graphicsDevice);
                }
                if (Center.Y - PosY < Speed)
                {
                    Move(Directions.Bottom, level, graphicsDevice);
                }
            }

            if (Math.Abs(Center.X - PosX) > Speed)
            {
                if (Center.X - PosX > Speed)
                {
                    Move(Directions.Left, level, graphicsDevice);
                }
                if (Center.X - PosX < Speed)
                {
                    Move(Directions.Right, level, graphicsDevice);
                }

            }
        }

        public void Move(Directions currentDirection, Level level, GraphicsDevice graphicsDevice)
        {
            if (currentDirection == Directions.Top)
                Velocity.Y = -Speed;

            if (currentDirection == Directions.Bottom)
                Velocity.Y = +Speed;

            if (currentDirection == Directions.Left)
                Velocity.X = -Speed;

            if (currentDirection == Directions.Right)
                Velocity.X = +Speed;

            if (currentDirection == Directions.TopLeft)
            {
                Velocity.X = -Speed;
                Velocity.Y = -Speed;
            }

            if (currentDirection == Directions.TopRight)
            {
                Velocity.X = +Speed;
                Velocity.Y = -Speed;
            }

            if (currentDirection == Directions.BottomLeft)
            {
                Velocity.X = -Speed;
                Velocity.Y = +Speed;
            }

            if (currentDirection == Directions.BottomRight)
            {
                Velocity.X = +Speed;
                Velocity.Y = +Speed;
            }
        }

        public virtual void TakeItem(Item item)
        {
            Inventory.Add(item);
            item.ApplyEffect(this);
        }

        public virtual bool TakeItem(Level level, GraphicsDevice graphicsDevice)
        {
            bool IsItemTaken = false;
            Item[] LevelItemArray = level.items.ToArray();
            for (int i = LevelItemArray.Length - 1; i >= 0; i--)
            {
                Item item = LevelItemArray[i];
                if (Collision.checkCollision(getRectangle(), item.getRectangle()))
                {
                    TakeItem(item);
                    level.items.RemoveAt(i);
                    IsItemTaken = true;
                    break;
                }
            }         
            return IsItemTaken;
        }

        public virtual void TakeItems(List<Item> items)
        {
            foreach (Item Item in items)
            {
                Inventory.Add(Item);
                Item.ApplyEffect(this);
            }
        }

        public virtual void UseItem(int i)
        {
            if (Inventory[i] is UsableItem UsableItem)
            {
                if (UsableItem.RemainingUsages > 0)
                {
                    UsableItem.Use(this);
                    UsableItem.RemainingUsages--;
                }
                if (UsableItem.RemainingUsages == 0)
                {
                    DeleteItem(i);
                }
            }
        }

        public virtual void DeleteItem(int i)
        {
            Inventory[i].RevertEffect(this);
            Inventory.RemoveAt(i);
        }

        public virtual void DropItem(Level level, int i)
        {
            Item DroppedItem = Inventory[i];
            DroppedItem.RevertEffect(this);

            Vector2 ScatteringOffset = new Vector2(Global.random.Next(-8, 8), Global.random.Next(-8, 8));
            DroppedItem.Position = Position + ScatteringOffset;
            level.items.Add(DroppedItem);

            Inventory.RemoveAt(i);
        }

        public virtual void DropAllItems(Level level)
        {
            foreach (Item Item in Inventory)
            {
                Item.RevertEffect(this);

                Vector2 ScatteringOffset = new Vector2(Global.random.Next(-8, 8), Global.random.Next(-8, 8));
                Item.Position = Center + ScatteringOffset;

                level.items.Add(Item);
            }
            Inventory.Clear();
        }

        public virtual void UpdateItems(GameTime gameTime)
        {
            foreach (Item Item in Inventory)
            {
                if (Item is UpdatableItem UpdatableItem)
                {
                    UpdatableItem.Update(gameTime, this);
                }
            }
        }

    }
}
