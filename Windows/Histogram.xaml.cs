using HdrHistogram;
using Stats_Mafia.Utilities;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

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
        public const double AXIS_LINE_THICKNESS = 5.0;
        public const double SPACE_BETWEEN_BARS = 2.0;
        public const double AXIS_PADDING = 55.0;

        /// <summary>
        /// Backing field that stores the collection of values of this histogram
        /// </summary>
        private IEnumerable<short> _Values;

        /// <summary>
        /// Histogram object that allows for statistical operations to be performed on the values
        /// </summary>
        private ShortHistogram _Histogram;

        /// <summary>
        /// Stores the title of this histogram
        /// </summary>
        private string HistogramTitle = "Histogram";

        /// <summary>
        /// Stores the label of the x-axis
        /// </summary>
        private string XAxisLabel = "X-Axis";

        /// <summary>
        /// Stores the label of the y-axis
        /// </summary>
        private string YAxisLabel = "Frequency";

        /// <summary>
        /// Determines whether or not the mouse can select bars on the histogram
        /// </summary>
        public bool MouseInspection { get; set; } = false;

        private Histogram(IEnumerable<short> values) 
        {
            _Values = values;
            _Histogram = new ShortHistogram(values.Min(), values.Max(), 5);

         
            foreach (short item in values)
            {
                _Histogram.RecordValue(item);
            }

            InitializeComponent();
        }

        /// <summary>
        /// Updates the values of the histogram and redraws it
        /// </summary>
        /// <param name="values">The new list of values that will be used to draw the histogram</param>
        public void UpdateValues(IEnumerable<short> values)
        {
            _Values = values;
            _Histogram = new ShortHistogram(values.Min(), values.Max(), 5);
            foreach (short item in values)
            {
                _Histogram.RecordValue(item);
            }
            if (DisplayCanvas is not null)
            {
                RenderHistogram(DisplayCanvas);
            }
        }

        /// <summary>
        /// Draws the histogram when the canvas is loaded
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DisplayCanvas_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is Canvas canvas)
            {
                RenderHistogram(canvas);
            }
        }

        /// <summary>
        /// Draws the histogram on the provided canvas
        /// </summary>
        /// <param name="canvas"></param>
        private void RenderHistogram(Canvas canvas, Point? mousePosition = null)
        {
            // Clear the canvas of any previous drawings
            canvas.Children.Clear();

            // We convert the number of bars to a HashSet so we only get the unique values
            HashSet<short> uniqueValues = [.. _Values.Order()];
            int barCount = uniqueValues.Count;
            int maxFrequency = GetMaxFrequency();

            // Calculate the height of each frequency unit and the width of each bar
            double frequencyHeight = (canvas.MinHeight - AXIS_PADDING * 1.1) / maxFrequency;
            double barWidth = (canvas.ActualWidth - 2 * AXIS_PADDING) / barCount;

            CalculateFrequencyResolution(in maxFrequency, out int frequencyResolution, out int frequencyLines);

            #region // Draw the histogram bars
            for (int I = 0; I < barCount; I++)
            {
                long frequency = _Histogram.GetCountAtValue(uniqueValues.ElementAt(I));
                Debug.Assert(frequency <= maxFrequency, "Frequency is somehow larger than the maximum allowable frequency");

                Rectangle bar = new()
                {
                    Width = barWidth - SPACE_BETWEEN_BARS,
                    Height = frequencyHeight * frequency,
                    Fill = new SolidColorBrush(new Color().HSVToRGB((I * 360f / barCount) % 360, 1, 0.5f)),
                    RadiusY = 5.0,
                    RadiusX = 5.0,
                    Stroke = new SolidColorBrush(new Color().HSVToRGB((I * 360f / barCount) % 360, 1, 0.3f))
                };
                Canvas.SetLeft(bar,  I * barWidth + AXIS_PADDING);
                Canvas.SetBottom(bar, AXIS_PADDING);
                canvas.Children.Add(bar);
            }
            #endregion

            #region // Draw the inspected histogram highlight if the mouse is over a bar
            {
                               if (MouseInspection && mousePosition is not null)
                {
                    double barPosition = double.Floor((mousePosition.Value.X - AXIS_PADDING) / barWidth);
                    if (barPosition >= 0 && barPosition < barCount)
                    {
                        short inspectedValue = uniqueValues.ElementAt((int)barPosition);
                        long inspectedFrequency = _Histogram.GetCountAtValue(inspectedValue);
                        double barHeight = frequencyHeight * inspectedFrequency;
                        if ((canvas.ActualHeight - mousePosition.Value.Y) > AXIS_PADDING && (canvas.ActualHeight - mousePosition.Value.Y) < AXIS_PADDING + barHeight)
                        {
                            Rectangle highlightBar = new()
                            {
                                Width = barWidth - SPACE_BETWEEN_BARS,
                                Height = frequencyHeight * inspectedFrequency,
                                Fill = Brushes.Transparent,
                                Stroke = Brushes.Black,
                                StrokeThickness = 2.0,
                                RadiusY = 5.0,
                                RadiusX = 5.0,
                            };
                            Canvas.SetLeft(highlightBar, barPosition * barWidth + AXIS_PADDING);
                            Canvas.SetBottom(highlightBar, AXIS_PADDING);
                            canvas.Children.Add(highlightBar);
                        }
                    }
                }
            }
            #endregion

            #region // Draw the axis lines
            {
                const double AXIS_X = AXIS_PADDING - (AXIS_LINE_THICKNESS / 2.0 + SPACE_BETWEEN_BARS);


                // Draw the x-axis line
                Line xAxisDivider = new()
                {
                    X1 = AXIS_X,
                    X2 = AXIS_PADDING * 13.2,
                    Stroke = Brushes.Gray,
                    StrokeThickness = AXIS_LINE_THICKNESS,
                    StrokeEndLineCap = PenLineCap.Round,
                };

                Canvas.SetBottom(xAxisDivider, AXIS_PADDING);
                canvas.Children.Add(xAxisDivider);

                Line yAxisDivider = new()
                {
                    X1 = AXIS_X,
                    X2 = AXIS_X,
                    Y2 = AXIS_PADDING,
                    Y1 = canvas.MinHeight - AXIS_PADDING * 0.05,
                    Stroke = Brushes.Gray,
                    StrokeThickness = AXIS_LINE_THICKNESS,
                    StrokeEndLineCap = PenLineCap.Round,
                };

                Canvas.SetBottom(yAxisDivider, AXIS_PADDING);
                canvas.Children.Add(yAxisDivider);
            }


            #endregion

            #region // Label the axis
            {
                TextBlock xLabel = new()
                {
                    Text = "X-Axis",
                    FontSize = 16,
                    FontWeight = FontWeights.DemiBold,
                    Width = canvas.ActualWidth,
                    Foreground = Brushes.Black,
                    TextAlignment = TextAlignment.Center,
                };
                Canvas.SetBottom(xLabel, AXIS_PADDING * 0.25);
                canvas.Children.Add(xLabel);

                TextBlock yLabel = new()
                {
                    Text = "Frequency",
                    FontSize = 16,
                    FontWeight = FontWeights.DemiBold,
                    Foreground = Brushes.Black,
                    TextAlignment = TextAlignment.Center,
                    RenderTransform = new RotateTransform(-90)
                };
     
                Canvas.SetLeft(yLabel, AXIS_PADDING * 0.1);
                Canvas.SetTop(yLabel, canvas.ActualHeight / 2);
                canvas.Children.Add(yLabel);

            }
            #endregion

            #region // Draw frequency on the Y-axis
            {
                for (int I = 0; I <= frequencyLines; I++)
                {
                    double yPosition = AXIS_PADDING + (I * frequencyResolution * frequencyHeight);
                    Line frequencyLine = new()
                    {
                        X1 = AXIS_PADDING - (AXIS_LINE_THICKNESS / 2.0 + SPACE_BETWEEN_BARS),
                        X2 = AXIS_PADDING * 1.1,
                        Y1 = 0,
                        Y2 = 0,
                        Stroke = Brushes.LightGray,
                        StrokeThickness = 1.0,
                        StrokeDashArray = [2, 2],
                    };
                    Canvas.SetBottom(frequencyLine, yPosition);
                    canvas.Children.Add(frequencyLine);


                    TextBlock frequencyLabel = new()
                    {
                        Text = (I * frequencyResolution).ToString(),
                        FontSize = 12,
                        FontWeight = FontWeights.Normal,
                        Foreground = Brushes.Black,
                        TextAlignment = TextAlignment.Right,
                        Width = AXIS_PADDING - (AXIS_LINE_THICKNESS / 2.0 + SPACE_BETWEEN_BARS) - 5.0
                    };
                    Canvas.SetBottom(frequencyLabel, yPosition - 8);
                    canvas.Children.Add(frequencyLabel);
                }
            }
            #endregion

            #region // Draw the data points of the x-axis
            {
                for (int I = 0; I < barCount; I++)
                {
                    double xPosition = I * barWidth + AXIS_PADDING + (barWidth / 2.0);
                    TextBlock dataPointLabel = new()
                    {
                        Text = uniqueValues.ElementAt(I).ToString(),
                        FontSize = 12,
                        FontWeight = FontWeights.Normal,
                        Foreground = Brushes.Black,
                        TextAlignment = TextAlignment.Center,
                        Width = barWidth
                    };
                    Canvas.SetLeft(dataPointLabel, xPosition - (barWidth / 2.0));
                    Canvas.SetBottom(dataPointLabel, AXIS_PADDING - 20.0);
                    canvas.Children.Add(dataPointLabel);
                }
            }
            #endregion

            #region // Draw the inspected histogram value if the mouse is over a bar
            {
                if (MouseInspection && mousePosition is not null)
                {
                    double barPosition = double.Floor((mousePosition.Value.X - AXIS_PADDING) / barWidth);

                    if (barPosition >= 0 && barPosition < barCount)
                    {
                        short inspectedValue = uniqueValues.ElementAt((int)barPosition);
                        long inspectedFrequency = _Histogram.GetCountAtValue(inspectedValue);

                        double barHeight = frequencyHeight * inspectedFrequency;
                        if ((canvas.ActualHeight - mousePosition.Value.Y) > AXIS_PADDING && (canvas.ActualHeight - mousePosition.Value.Y) < AXIS_PADDING + barHeight)
                        {
                            TextBlock inspectionLabel = new()
                            {
                                Text = $"Value: {inspectedValue} | {YAxisLabel}: {inspectedFrequency}",
                                FontSize = 14,
                                FontWeight = FontWeights.SemiBold,
                                Foreground = Brushes.Black,
                                Padding = new Thickness(5),
                                TextAlignment = TextAlignment.Center,
                            };

                            inspectionLabel.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                            inspectionLabel.Arrange(new Rect(inspectionLabel.DesiredSize));

                            double labelXPosition = mousePosition.Value.X + 10;
                            double labelYPosition = mousePosition.Value.Y - 10;

                            if (labelXPosition + inspectionLabel.ActualWidth > canvas.ActualWidth)
                            {
                                labelXPosition = mousePosition.Value.X - inspectionLabel.ActualWidth - 10;
                            }
                            if (labelYPosition - inspectionLabel.ActualHeight < 0)
                            {
                                labelYPosition = mousePosition.Value.Y + inspectionLabel.ActualHeight + 10;
                            }

                            #region // Label background
                            Rectangle inspectionLabelBackground = new()
                            {
                                Width = inspectionLabel.ActualWidth,
                                Height = inspectionLabel.ActualHeight,
                                Fill = Brushes.LightGray,
                                Opacity = 0.8,
                                RadiusY = 5.0,
                                RadiusX = 5.0,
                            };
                            Canvas.SetLeft(inspectionLabelBackground, labelXPosition);
                            Canvas.SetTop(inspectionLabelBackground, labelYPosition);
                            canvas.Children.Add(inspectionLabelBackground);
                            #endregion

                            Canvas.SetLeft(inspectionLabel, labelXPosition);
                            Canvas.SetTop(inspectionLabel, labelYPosition);
                            canvas.Children.Add(inspectionLabel);
                        }
                    }
                }
            }
            #endregion

            #region Render statistical values
            {
                StringBuilder infoBuilder = new();
                infoBuilder.Append($" Min: {_Histogram.LowestTrackableValue} |");
                infoBuilder.Append($" Max: {_Histogram.GetMaxValue()} |");
                infoBuilder.Append($" Range: {_Histogram.GetMaxValue() - _Histogram.LowestEquivalentValue(long.MinValue)} | ");
                infoBuilder.Append($" Count: {_Histogram.TotalCount} |");
                infoBuilder.Append($" Mean: {_Histogram.GetMean():F2} |");
                infoBuilder.Append($" Median: {_Histogram.GetValueAtPercentile(50):F2} |");
                infoBuilder.Append($" StdDev: {_Histogram.GetStdDeviation():F2} |");
                infoBuilder.Append($" 90th Percentile: {_Histogram.GetValueAtPercentile(90):F2} |");
                infoBuilder.Append($" 99th Percentile: {_Histogram.GetValueAtPercentile(99):F2}");

                TextBlock statisticalInformation = new()
                {
                    Text = infoBuilder.ToString(),
                    Width = canvas.ActualWidth,
                    FontSize = 12,
                    FontWeight = FontWeights.Normal,
                    Foreground = Brushes.Black,
                    TextAlignment = TextAlignment.Center,
                };

                Canvas.SetLeft(statisticalInformation, 0);
                Canvas.SetBottom(statisticalInformation, 0);
                canvas.Children.Add(statisticalInformation);
            }
            #endregion
        }

        /// <summary>
        /// Returns the frequency of the most common value in the collection
        /// </summary>
        /// <returns></returns>
        private int GetMaxFrequency()
        {
            HashSet<short> uniqueValues = [.. _Values.Order()];
            return (int)uniqueValues.Select(x => _Histogram.GetCountAtValue(x)).Max();
        }

        /// <summary>
        /// Calculates an appropriate number for the gaps between the frequency values on the y-axis
        /// </summary>
        /// <param name="maxFrequency">The highest frequency that the graph will need to display</param>
        /// <param name="frequencyResolution">The interval at which, each frequency will be displayed</param>
        /// <param name="frequencyLines">The number of frequency intervals that will fit inside the frequency range</param>
        private static void CalculateFrequencyResolution(in int maxFrequency, out int frequencyResolution, out int frequencyLines)
        {
            const int MAX_FREQUENCY_LINES = 10;
            frequencyResolution = 1;

            while (maxFrequency / MAX_FREQUENCY_LINES > frequencyResolution)
            {
                frequencyResolution *= 5;
            }

            frequencyLines = maxFrequency / frequencyResolution;
        }

        /// <summary>
        /// Redraw the histogram if the size of the canvas changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DisplayCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (sender is Canvas canvas)
            {
                RenderHistogram(canvas);
            }
        }

        private void DisplayCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Canvas canvas)
            {
                if (MouseInspection)
                {
                    RenderHistogram(canvas, e.GetPosition(canvas));
                }
            }
        }

        private void DisplayCanvas_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Canvas canvas)
            {
                if (MouseInspection)
                {
                    RenderHistogram(canvas);
                }
            }
        }

        /// <summary>
        /// Creates a histogram from the provided collection of values
        /// </summary>
        /// <param name="values">The collection of values to be displayed in the histogram</param>
        /// <returns></returns>
        public static Histogram Create(IEnumerable<short> values)
        {
            return new(values);
        }

        /// <summary>
        /// Creates a histogram from the provided collection of values
        /// </summary>
        /// <param name="values">The collection of values to be displayed in the histogram</param>
        /// <param name="mouseInspection">A value indicating whether the mouse can be used to inspect values on the histogram</param>
        /// <returns></returns>
        public static Histogram Create(IEnumerable<short> values, bool mouseInspection)
        {
            return new(values)
            {
                MouseInspection = mouseInspection
            };
        }

        /// <summary>
        /// Creates a histogram from the provided collection of values
        /// </summary>
        /// <param name="values">The collection of values to be displayed in the histogram</param>
        /// <param name="title">The title of the histogram</param>
        /// <param name="xAxis">The name of the x-axis of the histogram</param>
        /// <returns></returns>
        public static Histogram Create(IEnumerable<short> values, string title, string xAxis)
        {
            return new(values)
            {
                HistogramTitle = title,
                XAxisLabel = xAxis

            };
        }

        /// <summary>
        /// Creates a histogram from the provided collection of values
        /// </summary>
        /// <param name="values">The collection of values to be displayed in the histogram</param>
        /// <param name="title">The title of the histogram</param>
        /// <param name="xAxis">The name of the x-axis of the histogram</param>
        /// <param name="mouseInspection">A value indicating whether the mouse can be used to inspect values on the histogram</param>
        /// <returns></returns>
        public static Histogram Create(IEnumerable<short> values, string title, string xAxis, bool mouseInspection)
        {
            return new(values)
            {
                HistogramTitle = title,
                XAxisLabel = xAxis,
                MouseInspection = mouseInspection
            };
        }

        /// <summary>
        /// Creates a histogram from the provided collection of values
        /// </summary>
        /// <param name="values">The collection of values to be displayed in the histogram</param>
        /// <param name="title">The title of the histogram</param>
        /// <param name="xAxis">The name of the x-axis of the histogram</param>
        /// <param name="yAxis">The name of the y-axis of the histogram</param>
        /// <returns></returns>
        public static Histogram Create(IEnumerable<short> values, string title, string xAxis, string yAxis)
        {
            return new(values)
            {
                HistogramTitle = title,
                XAxisLabel = xAxis,
                YAxisLabel = yAxis
            };
        }

        /// <summary>
        /// Creates a histogram from the provided collection of values
        /// </summary>
        /// <param name="values">The collection of values to be displayed in the histogram</param>
        /// <param name="title">The title of the histogram</param>
        /// <param name="xAxis">The name of the x-axis of the histogram</param>
        /// <param name="yAxis">The name of the y-axis of the histogram</param>
        /// <param name="mouseInspection">A value indicating whether the mouse can be used to inspect values on the histogram</param>
        /// <returns></returns>
        public static Histogram Create(IEnumerable<short> values, string title, string xAxis, string yAxis, bool mouseInspection)
        {
            return new(values)
            {
                HistogramTitle = title,
                XAxisLabel = xAxis,
                YAxisLabel = yAxis
            };
        }
    }
}
