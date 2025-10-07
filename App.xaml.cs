using Stats_Mafia.Utilities;
using Stats_Mafia.Windows;
using System.Windows;

namespace Stats_Mafia;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public readonly bool SpeechSynthesisEnabled = true;
    public readonly SpeechSystem.SpeechFilter SpeechFilter = SpeechSystem.SpeechFilter.All;
    
#if DEBUG
    public App()
    {
        //Histogram histogramWindow = new("Histogram title", [1, 1, 1, 2, 3, 3, 3, 3, 3, 3, 4, 4, 5, 6, 7, 8, 9, 10, 11, 12]);
        short[] data_1 = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 23, 24];
        short[] data_2 = new short[50];

        for (int I = 0; I < data_2.Length; I++)
        {
            data_2[I] = (short)(Random.Shared.Next(1, 25) + Random.Shared.Next(1, 25));
        }
        

        Histogram histogramWindow = Histogram.Create(data_2, true);



        histogramWindow.Show();
    }
#endif
}

