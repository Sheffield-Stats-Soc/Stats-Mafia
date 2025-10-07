using HdrHistogram;
using System.Windows;

namespace Stats_Mafia.Windows
{
    /*
     * Take in a collection of integer values
     * Display a histogram of the values in the range of minimum to maximum value in the collection on the x-axis, and the frequency of each value on the y-axis
     * Display a title of the histogram for what it is showing
     * Display 
     * 
     * 
     */

    /// <summary>
    /// Interaction logic for Histogram.xaml
    /// </summary>
    public partial class Histogram : Window
    {
        private readonly IEnumerable<short> _Values;
        private readonly ShortHistogram _Histogram;
        private readonly string _Title;

        public Histogram(string title, IEnumerable<short> values) 
        {
            _Values = values;
            _Title = title;
            _Histogram = new ShortHistogram(values.Min(), values.Max(), 5);

            foreach (short item in values)
            {
                _Histogram.RecordValue(item);
            }

            InitializeComponent();
        }
    }
}
