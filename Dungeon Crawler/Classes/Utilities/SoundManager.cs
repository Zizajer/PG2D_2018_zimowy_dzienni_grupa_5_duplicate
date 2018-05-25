using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;

namespace Dungeon_Crawler
{
    public class SoundManager
    {
        Song gameover;
        Song pew;
        Song portalactivated;

        public SoundManager(ContentManager content)
        {
            gameover = content.Load<Song>("sounds/gameover");
            pew = content.Load<Song>("sounds/pew");
            portalactivated = content.Load<Song>("sounds/portalactivated");
        }

        public void playPew()
        {
            MediaPlayer.Play(pew);
        }
        public void playGameOver()
        {
            MediaPlayer.Play(gameover);
        }
        public void playPortalActivated()
        {
            MediaPlayer.Play(portalactivated);
        }
    }
}