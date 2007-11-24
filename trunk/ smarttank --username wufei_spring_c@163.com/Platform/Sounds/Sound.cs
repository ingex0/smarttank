using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Audio;
using GameBase.Helpers;
using System.IO;

namespace Platform.Sounds
{
    public static class Sound
    {
        static AudioEngine audioEngine;
        
        static WaveBank waveBank;
        
        static SoundBank soundBank;

        public static void Initial()
        {
            try
            {
                string dir = Directories.SoundDirectory;
                audioEngine = new AudioEngine( Path.Combine( dir, "SmartTank1.xgs" ) );
                waveBank = new WaveBank( audioEngine, Path.Combine( dir, "Wave Bank.xwb" ) );

                if (waveBank != null)
                {
                    soundBank = new SoundBank( audioEngine,
                        Path.Combine( dir, "Sound Bank.xsb" ) );
                }
                
            }
            catch (NoAudioHardwareException ex)
            {
                Log.Write( "Failed to create sound class: " + ex.ToString() );
            }
        }


        public static void PlayCue ( string cueName )
        {
            soundBank.PlayCue( cueName );
        }

        internal static void Clear ()
        {
            audioEngine.GetCategory( "Default" ).Stop( AudioStopOptions.Immediate );
        }
    }
}
