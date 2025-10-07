using System.Windows;

namespace Stats_Mafia 
{
    /*
     * Display a setup button if the game has ended, or this is the first time the app has been run
     * Display the name of the player who's go it is to go next
     * Display a roll button to roll the dice
     * Track each players rolls and display them in a list or grid
     * Have an open histogram button next to each player's name to open a window showing a histogram of their rolls
     * Have a histogram button to open a window showing a histogram of all rolls
     * 
     */

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
    }
}