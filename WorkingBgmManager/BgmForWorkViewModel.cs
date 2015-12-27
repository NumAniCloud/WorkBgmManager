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
        private Manager manager { get; set; }
        private ReactiveProperty<Sound> sound { get; set; } = new ReactiveProperty<Sound>();
        private Playback playback { get; set; }

        private ReactiveProperty<bool> isPlaying { get; set; } = new ReactiveProperty<bool>(false);
        private ReactiveProperty<bool> isFading { get; set; } = new ReactiveProperty<bool>(false);
        private ReactiveProperty<bool> isPaused { get; set; } = new ReactiveProperty<bool>(false);

        public ReactiveProperty<string> FileName { get; set; } = new ReactiveProperty<string>("No File Loaded");
        public ReactiveProperty<float> Volume { get; set; } = new ReactiveProperty<float>(0.8f);
        public ReactiveProperty<bool> DoFadeInOut { get; set; } = new ReactiveProperty<bool>(false);

        public ReactiveCommand LoadCommand { get; set; } = new ReactiveCommand();
        public ReactiveCommand PlayCommand { get; set; }
        public ReactiveCommand LoopingPlayCommand { get; set; }
        public ReactiveCommand PauseCommand { get; set; }
        public ReactiveCommand ResumeCommand { get; set; }

        public BgmForWorkViewModel()
        {
            manager = new Manager();

            var soundIsNotNull = sound.Select(x => x != null);
            var notBeFading = isFading.Select(x => !x);
            PlayCommand = isPlaying.Select(x => !x)
                .And(notBeFading)
                .And(soundIsNotNull)
                .ToReactiveCommand();
            LoopingPlayCommand = isPlaying.Select(x => !x)
                .And(notBeFading)
                .And(soundIsNotNull)
                .ToReactiveCommand();
            PauseCommand = isPlaying
                .And(notBeFading)
                .And(isPaused.Select(x => !x))
                .ToReactiveCommand();
            ResumeCommand = isPaused.And(notBeFading).ToReactiveCommand();

            LoadCommand.Subscribe(u => Load());
            PlayCommand.Subscribe(u => Play());
            LoopingPlayCommand.Subscribe(u => PlayLooping());
            PauseCommand.Subscribe(u => Pause());
            ResumeCommand.Subscribe(u => Resume());

            Volume.Subscribe(v => playback?.SetVolume(v));
        }

        public void Initialize()
        {
            manager.Initialize();
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
                isPaused.Value = false;
                isPlaying.Value = false;
            }
        }

        private void Play()
        {
            if(sound != null)
            {
                sound.Value.IsLoopingMode = false;
                playback = manager.Play(sound.Value);
                playback.SetVolume(Volume.Value);
                isPlaying.Value = true;
                isPaused.Value = false;
            }
        }

        private void PlayLooping()
        {
            if(sound != null)
            {
                sound.Value.IsLoopingMode = true;
                playback = manager.Play(sound.Value);
                playback.SetVolume(Volume.Value);
                isPlaying.Value = true;
                isPaused.Value = false;
            }
        }

        private void Pause()
        {
            if(playback != null)
            {
                IObservable<long> finished;
                if(DoFadeInOut.Value)
                {
                    playback.FadeOut(1);
                    finished = Observable.Timer(TimeSpan.FromSeconds(1.1));
                    isFading.Value = true;
                }
                else
                {
                    finished = Observable.Return((long)0);
                }
                isPaused.Value = true;
                finished.Subscribe(t =>
                {
                    playback.Pause();
                    isPlaying.Value = false;
                    isFading.Value = false;
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
            if(DoFadeInOut.Value)
            {
                playback.FadeIn(1);
                isFading.Value = true;
                Observable.Timer(TimeSpan.FromSeconds(1.1)).Subscribe(x => isFading.Value = false);
            }
            else
            {
                playback.SetVolume(Volume.Value);
            }
            isPlaying.Value = true;
            isPaused.Value = false;
        }
    }
}
