using System.Diagnostics;
using System.Speech.Synthesis;
using System.Windows;

namespace Stats_Mafia.Utilities
{
    public class SpeechSystem
    {
        [Flags]
        public enum SpeechFilter
        {

            /// <summary>
            /// Used to denote that no speech events can run
            /// </summary>
            None = 0,

            /// <summary>
            /// Used to designate speech events that run when the user interacts with the UI can run
            /// </summary>
            Interact = 1,

            /// <summary>
            /// Used to designate speech events that run on window creation can run
            /// </summary>
            WindowCreated = 2,

            /// <summary>
            /// Used to designate speech events that run whenever a  window is updated can run
            /// </summary>
            WindowUpdated = 4,

            /// <summary>
            /// Used to designate speech events that are designed to deliver important information to the user can run
            /// </summary>
            Information = 8,

            All = Interact | WindowCreated | WindowUpdated | Information,
        }

        private SpeechSynthesizer Speaker { get; }

        /// <summary>
        /// Flag used to denote whether new speech can overwrite the currently playing speech
        /// </summary>
        private bool PlayingUninterruptableSpeech { get; set; } = false;


        public SpeechSystem()
        {
            Speaker = new SpeechSynthesizer();
            Speaker.SpeakCompleted += Speaker_SpeakCompleted;
#if DEBUG
            foreach (InstalledVoice voice in Speaker.GetInstalledVoices())
            {
                Debug.WriteLine(voice);
            }
#endif
        }

        private void Speaker_SpeakCompleted(object? sender, SpeakCompletedEventArgs e)
        {
            PlayingUninterruptableSpeech = false;
        }

        public void Speak(string speech, SpeechFilter filterType, bool uninterruptable = false)
        {
            if (PlayingUninterruptableSpeech)
                return;

            if (Application.Current is App app)
            {
                // Don't queue any sound if speech is disabled or if the corresponding speech filter is not set
                if (!app.SpeechSynthesisEnabled)
                    return;

                if (!app.SpeechFilter.HasFlag(filterType))
                    return;

                Speaker.SpeakAsyncCancelAll();
                Speaker.SpeakAsync(speech);

                PlayingUninterruptableSpeech = uninterruptable;
            }
            
        }
        
    }
}
