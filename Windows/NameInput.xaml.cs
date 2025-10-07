using System.Windows;

/*
 * Take in n many names and return them as a string collection (IEnurable<string>)
 * Names should have a delete button next to them to remove them if they are not needed
 * Names should not be duplicated (use a hashset to store them)
 * Names should not be null or empty or whitespace (string.IsNullOrWhiteSpace)
 * Names should be ASCII letters and less than 20 characters long (Use texbox setting to limit length and input, or use char.IsAsciiLetter(c) to check each char)
 * Duplicate names should not be allowed to click the add name button (disable button if name is duplicate)
 * 
 */

namespace Stats_Mafia.Windows
{
    /// <summary>
    /// Interaction logic for NameInput.xaml
    /// </summary>
    public partial class NameInput : Window
    {
        public NameInput()
        {
            InitializeComponent();
        }
    }
}
