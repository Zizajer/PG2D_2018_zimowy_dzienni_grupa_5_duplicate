using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;

namespace Dungeon_Crawler
{
    public class SoundManager
    {
        public SoundEffect gameover { get; set; }
        public SoundEffect portalactivated { get; set; }
        public SoundEffect mageBasicAttack { get; set; }
        public SoundEffect mageSecondaryAttack { get; set; }
        public SoundEffect mageAbillity1 { get; set; }
        public SoundEffect mageAbillity2 { get; set; }
        public SoundEffect mageAbillity3 { get; set; }
        public SoundEffect rangerBasicAttack { get; set; }
        public SoundEffect rangerSecondaryAttack { get; set; }
        public SoundEffect rangerAbillity1 { get; set; }
        public SoundEffect rangerAbillity2 { get; set; }
        public SoundEffect rangerAbillity3 { get; set; }
        public SoundEffect warriorBasicAttack { get; set; }
        public SoundEffect warriorBasicAttackAtHit { get; set; }
        public SoundEffect warriorSecondaryAttack { get; set; }
        public SoundEffect warriorAbillity1 { get; set; }
        public SoundEffect warriorAbillity2 { get; set; }
        public SoundEffect warriorAbillity3 { get; set; }
        public SoundEffect launchGameClick { get; set; }
        public SoundEffect buttonClick { get; set; }
        public SoundEffect changeInInventory { get; set; }
        public SoundEffect dropFromInventory { get; set; }
        public SoundEffect takeToInventory { get; set; }
        public SoundEffect examineItem { get; set; }
        public SoundEffect mixtureDrink { get; set; }
        public SoundEffect playerHurt { get; set; }
        public SoundEffect playerDead { get; set; }
        public SoundEffect teleportLevel { get; set; }
        public Song menuSong { get; set; }
        public Song inGameSong { get; set; }


        public SoundManager(ContentManager content)
        {
            gameover = content.Load<SoundEffect>("sounds/gameover");
            portalactivated = content.Load<SoundEffect>("sounds/portalactivated");
            mageBasicAttack = content.Load<SoundEffect>("sounds/mage/basicAttack");
            mageSecondaryAttack = content.Load<SoundEffect>("sounds/mage/secondaryAttack");
            mageAbillity1 = content.Load<SoundEffect>("sounds/mage/abillity1");
            mageAbillity2 = content.Load<SoundEffect>("sounds/mage/abillity2");
            mageAbillity3 = content.Load<SoundEffect>("sounds/mage/abillity3");
            rangerBasicAttack = content.Load<SoundEffect>("sounds/ranger/basicAttack");
            rangerSecondaryAttack = content.Load<SoundEffect>("sounds/ranger/secondaryAttack");
            rangerAbillity1 = content.Load<SoundEffect>("sounds/ranger/abillity1");
            rangerAbillity2 = content.Load<SoundEffect>("sounds/ranger/abillity2");
            rangerAbillity3 = content.Load<SoundEffect>("sounds/ranger/abillity3");
            warriorBasicAttack = content.Load<SoundEffect>("sounds/warrior/basicAttack");
            warriorBasicAttackAtHit = content.Load<SoundEffect>("sounds/warrior/basicAttackAtHit");
            warriorSecondaryAttack = content.Load<SoundEffect>("sounds/warrior/secondaryAttack");
            warriorAbillity1 = content.Load<SoundEffect>("sounds/warrior/abillity1");
            warriorAbillity2 = content.Load<SoundEffect>("sounds/warrior/abillity2");
            warriorAbillity3 = content.Load<SoundEffect>("sounds/warrior/abillity3");
            launchGameClick = content.Load<SoundEffect>("sounds/menu/launchGame");
            buttonClick = content.Load<SoundEffect>("sounds/menu/buttonClick");
            changeInInventory = content.Load<SoundEffect>("sounds/inventory/changeItem");
            dropFromInventory = content.Load<SoundEffect>("sounds/inventory/dropItem");
            takeToInventory = content.Load<SoundEffect>("sounds/inventory/takeItem");
            examineItem = content.Load<SoundEffect>("sounds/inventory/examineItem");
            mixtureDrink = content.Load<SoundEffect>("sounds/inventory/bottle");
            playerHurt = content.Load<SoundEffect>("sounds/hurt");
            playerDead = content.Load<SoundEffect>("sounds/dead");
            teleportLevel = content.Load<SoundEffect>("sounds/levelTeleport");
            menuSong = content.Load<Song>("sounds/music/menuSong");
            inGameSong = content.Load<Song>("sounds/music/inGameSong");
            MediaPlayer.IsRepeating = true;
        }

        public void playMenuSong() {
            MediaPlayer.Stop();
            MediaPlayer.Play(menuSong);
        }

        public void playInGameSong()
        {
            MediaPlayer.Stop();
            MediaPlayer.Play(inGameSong);
        }
    }
}