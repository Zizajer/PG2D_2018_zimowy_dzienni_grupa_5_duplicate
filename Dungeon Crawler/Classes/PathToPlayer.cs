using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RogueSharp;
using System.Collections.Generic;

namespace Dungeon_Crawler
{
    public class PathToPlayer
    {
        private readonly Player _player;
        private readonly IMap _map;
        private readonly Texture2D _sprite;
        private readonly PathFinder _pathFinder;
        private IEnumerable<Cell> _cells;

        public PathToPlayer(Player player, IMap map, Texture2D sprite)
        {
            _player = player;
            _map = map;
            _sprite = sprite;
            _pathFinder = new PathFinder(map);
        }
        public Cell FirstCell
        {
            get
            {
                return _cells.ElementAt(1);
            }
        }
        public void CreateFrom(int x, int y,GoalMap _goalMap)
        {
            _goalMap.ClearGoals();
            _goalMap.AddGoal(_player.X, _player.Y, 1);
            try
            {
                _cells = _goalMap.FindPath(x, y).Steps;
            }
            catch (PathNotFoundException)
            {

            }
             
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            if (_cells != null && GlobalVariables.DebugMode)
            {
                foreach (Cell cell in _cells)
                {
                    if (cell != null)
                    {
                        spriteBatch.Draw(_sprite, new Vector2(cell.X * _sprite.Width, cell.Y * _sprite.Height), null, Color.Blue * .2f, 0.0f, Vector2.One, 1.0f, SpriteEffects.None, Layers.Paths);
                    }
                }
            }
        }
    }
}
