using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace orion
{
    public class Audio
    {
        public SoundEffect sfx_changeOption;
        public SoundEffect sfx_explosion1;
        public SoundEffect sfx_explosion2;
        public SoundEffect sfx_nuke;
        public SoundEffect sfx_playerShoot;
        public SoundEffect sfx_powerUp;
        public SoundEffect sfx_rocketLaunch;
        public SoundEffect sfx_rockSmash;
        public SoundEffect sfx_select;
        public SoundEffect sfx_playerDie;
        public SoundEffect sfx_hit;
        public SoundEffect sfx_turretFire;
        public SoundEffect sfx_alarm;

        public Song song_Level1;
        public Song song_Level2;
        public Song song_Level3;
        public Song song_Level4;
        public Song song_Boss;
        public Song song_Title;
        public Song song_GameOver;

        public float soundVolume = 1;
        public bool soundOn = true;
        public float musicVolume = .75f;
        bool musicOn = true;
        Game1 theGame;

        List<SoundEffectInstance> playingSounds = new List<SoundEffectInstance>();

        public Audio() 
        {
            MediaPlayer.IsRepeating = true;
        }

        public void loadAudio(Game1 game)
        {
            theGame = game;

            sfx_changeOption = game.Content.Load<SoundEffect>("sfx_changeOption");
            sfx_explosion1 = game.Content.Load<SoundEffect>("sfx_explosion1");
            sfx_explosion2 = game.Content.Load<SoundEffect>("sfx_explosion2");
            sfx_nuke = game.Content.Load<SoundEffect>("sfx_nuke");
            sfx_playerShoot = game.Content.Load<SoundEffect>("sfx_playerShoot");
            sfx_powerUp = game.Content.Load<SoundEffect>("sfx_powerUp");
            sfx_rocketLaunch = game.Content.Load<SoundEffect>("sfx_rocketLaunch");
            sfx_rockSmash = game.Content.Load<SoundEffect>("sfx_rockSmash");
            sfx_select = game.Content.Load<SoundEffect>("sfx_select");
            sfx_playerDie = game.Content.Load<SoundEffect>("sfx_playerDie");
            sfx_turretFire = game.Content.Load<SoundEffect>("sfx_turretFire");
            sfx_hit = game.Content.Load<SoundEffect>("sfx_playerHit");
            sfx_alarm = game.Content.Load<SoundEffect>("sfx_alarm");

            song_Title = game.Content.Load<Song>("song1");
            song_Boss = game.Content.Load<Song>("song2");
            song_GameOver = game.Content.Load<Song>("song3");
            song_Level1 = game.Content.Load<Song>("song4");
            song_Level2 = game.Content.Load<Song>("song5");
            song_Level3 = game.Content.Load<Song>("song6");
            song_Level4 = game.Content.Load<Song>("song7");
        }

        public void update()
        {
            if (playingSounds.Count > 100)
            {
                for (int i = 0; i < playingSounds.Count; i++)
                {
                    if (playingSounds[i].State == SoundState.Stopped)
                        playingSounds.Remove(playingSounds[i]);
                }
            }
        }


        public void soundPlay(SoundEffect sound)
        {
            playingSounds.Add(sound.CreateInstance());

            if (!soundOn) return;
            playingSounds[playingSounds.Count-1].Volume = soundVolume;
            playingSounds[playingSounds.Count-1].Stop(true);
            playingSounds[playingSounds.Count-1].Play();
        }

        public void soundPlay(SoundEffect sound, float pitch)
        {
            playingSounds.Add(sound.CreateInstance());

            if (!soundOn) return;
            playingSounds[playingSounds.Count - 1].Pitch = pitch;
            playingSounds[playingSounds.Count - 1].Volume = soundVolume;
            playingSounds[playingSounds.Count - 1].Stop(true);
            playingSounds[playingSounds.Count - 1].Play();
        }

        public void musicPlay(Song song)
        {
            if (!musicOn) return;
            MediaPlayer.Volume = musicVolume;
            MediaPlayer.Play(song);

        }

        public void toggleMusic()
        {
            if (musicOn)
            {
                MediaPlayer.Pause();
                musicOn = false;
            }
            else
            {
                MediaPlayer.Resume();
                musicOn = true;
            }
        }
    }
}
