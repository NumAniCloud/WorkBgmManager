using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reactive.Bindings;
using System.Reactive.Linq;
using osm;
using System.Windows.Forms;
using System.Reactive;
using System.IO;
using WorkingBgmManager.ViewModel;

namespace WorkingBgmManager
{
    static class HelperEx
    {
        public static IObservable<bool> And(this IObservable<bool> first, IObservable<bool> second)
        {
            return first.CombineLatest(second, (v1, v2) => v1 && v2);
        }
    }

    public class BgmForWorkViewModel
    {
        private static readonly float FadeTime = 1.6f;

        private BuildEventHandler eventHandler { get; set; }

        private Manager manager { get; set; }
        private ReactiveProperty<Sound> sound { get; set; } = new ReactiveProperty<Sound>();
        private Playback playback { get; set; }
        private ReactiveProperty<PlaybackState> playbackState { get; set; }

        public ReactiveProperty<string> FileName { get; set; } = new ReactiveProperty<string>("No File Loaded");
        public ReactiveProperty<float> Volume { get; set; } = new ReactiveProperty<float>(0.7f);
        public ReactiveProperty<bool> DoFadeInOut { get; set; } = new ReactiveProperty<bool>(false);
        public ReactiveProperty<bool> DoLoop { get; set; } = new ReactiveProperty<bool>(true);

        public ReactiveCommand LoadCommand { get; set; } = new ReactiveCommand();
        public ReactiveCommand PlayCommand { get; set; }
        public ReactiveCommand PauseCommand { get; set; }
        public ReactiveCommand ResumeCommand { get; set; }

        public BgmForWorkViewModel()
        {
            manager = new Manager();
            playbackState = new ReactiveProperty<PlaybackState>(PlaybackState.GetStoppingState());
            
            PlayCommand = playbackState.Select(x => x.CanPlay).ToReactiveCommand();
            PauseCommand = playbackState.Select(x => x.CanPause).ToReactiveCommand();
            ResumeCommand = playbackState.Select(x => x.CanResume).ToReactiveCommand();

            LoadCommand.Subscribe(u => Load());
            PlayCommand.Subscribe(u => Play());
            PauseCommand.Subscribe(u => Pause());
            ResumeCommand.Subscribe(u => Resume());

            Volume.Subscribe(v => playback?.SetVolume(v));

            DoLoop.CombineLatest(sound, (v1, v2) => v1)
                .Where(x => sound.Value != null)
                .Subscribe(x => sound.Value.IsLoopingMode = x);
            
            eventHandler = new BuildEventHandler();
            eventHandler.OnEnterRunMode
                .Where(u => playbackState.Value.CanPause)
                .Subscribe(u => Pause());
            eventHandler.OnEnterDesignMode
                .Where(u => playbackState.Value.CanResume)
                .Subscribe(u => Resume());
        }

        public void Initialize()
        {
            manager.Initialize();
            eventHandler.Initialize();
        }

        public void Dispose()
        {
            manager.Dispose();
        }

        private void Load()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            DialogResult result = openFileDialog.ShowDialog();
            if(result == DialogResult.OK)
            {
                playback?.Stop();
                playback = null;
                sound.Value = manager.CreateSound(openFileDialog.FileName);
                FileName.Value = Path.GetFileName(openFileDialog.FileName);
                playbackState.Value = PlaybackState.GetStoppingState();
            }
        }

        private void Play()
        {
            if(sound != null)
            {
                playback = manager.Play(sound.Value);
                playback.SetVolume(Volume.Value);
                playbackState.Value = PlaybackState.GetPlayingState();
            }
        }

        private void Pause()
        {
            if(playback != null)
            {
                IObservable<long> finished;
                if(DoFadeInOut.Value)
                {
                    playback.Fade(FadeTime, 0.01f);
                    playbackState.Value = PlaybackState.GetFadingOutState();
                    finished = Observable.Timer(TimeSpan.FromSeconds(FadeTime + 0.1f));
                }
                else
                {
                    finished = Observable.Return((long)0);
                }
                finished.Subscribe(t =>
                {
                    playback.Pause();
                    playbackState.Value = PlaybackState.GetPausingState();
                });
            }
        }

        private void Resume()
        {
            if(sound == null || playback == null)
            {
                return;
            }

            playback.Resume();

            IObservable<long> finished;
            if(DoFadeInOut.Value)
            {
                playback.FadeIn(FadeTime);
                playbackState.Value = PlaybackState.GetFadingInState();
                finished = Observable.Timer(TimeSpan.FromSeconds(FadeTime + 0.1f));
            }
            else
            {
                playback.SetVolume(Volume.Value);
                finished = Observable.Return((long)0);
            }
            finished.Subscribe(x => playbackState.Value = PlaybackState.GetPlayingState());
        }
    }
}
