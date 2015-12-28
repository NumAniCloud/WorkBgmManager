using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkingBgmManager.ViewModel
{
    class PlaybackState
    {
        public string StateName { get; }
        public bool CanPlay { get; }
        public bool CanResume { get; }
        public bool CanPause { get; }

        private PlaybackState(string name, bool canPlay, bool canResume, bool canPause)
        {
            CanPlay = canPlay;
            CanResume = canResume;
            CanPause = canPause;
        }

        public static PlaybackState GetStoppingState() => new PlaybackState("Stopping", true, false, false);
        public static PlaybackState GetPlayingState() => new PlaybackState("Playing", false, false, true);
        public static PlaybackState GetFadingOutState() => new PlaybackState("FadingOut", false, false, false);
        public static PlaybackState GetPausingState() => new PlaybackState("Pausing", true, true, false);
        public static PlaybackState GetFadingInState() => new PlaybackState("FadingIn", false, false, false);
    }
}
