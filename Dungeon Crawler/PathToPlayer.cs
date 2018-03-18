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
                return _cells.First();
            }
        }
        public void CreateFrom(int x, int y)
        {
            try
            {
                _cells = _pathFinder.ShortestPath(_map.GetCell(x, y), _map.GetCell(_player.X, _player.Y)).Steps;
            }
            catch (System.ArgumentOutOfRangeException)
            {

            }
            catch (PathNotFoundException)
            {
            }
            
            
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            if (_cells != null && Global.DebugMode)
            {
                foreach (Cell cell in _cells)
                {
                    if (cell != null)
                    {
                        spriteBatch.Draw(_sprite, new Vector2(cell.X * _sprite.Width, cell.Y * _sprite.Height), null, null, null, 0.0f, Vector2.One, Color.Blue * .2f, SpriteEffects.None, LayerDepth.Paths);
                    }
                }
            }
        }
    }
}
