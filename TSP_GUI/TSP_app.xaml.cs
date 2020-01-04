using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TSP_Lib;

namespace TSP_GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        BackgroundWorker simulationWorker;
        int nmbOfGenerations;
        int nmbOfCities;
        int sizeOfPopulation;
        int selectedGenerationIdx = 0;
        List<Population> generations;
        CitiesMap citiesMap;
        NewPopulationParameters newPopulationParameters;
        int nmbOfColumnsInChart = 20;
        bool isSimulationEnabled;
        public SeriesCollection SimulationResultCollection { get; set; }
        

        public MainWindow()
        {
            SimulationResultCollection = new SeriesCollection();

            isSimulationEnabled = false;
            InitializeComponent();
            simulationWorker = new BackgroundWorker();
            simulationWorker.DoWork += SimulationWorker_DoWork;
            simulationWorker.RunWorkerCompleted += SimulationWorker_RunWorkerCompleted;
            generations = new List<Population>();

            var series1 = new ColumnSeries
            {
                Values = new ChartValues<double> { },
                Title = "Best Generation",
                Fill = Brushes.Green,
                StrokeThickness = .5,
                PointGeometry = null //use a null geometry when you have many series
            };
            var series2 = new ColumnSeries
            {
                Values = new ChartValues<double> { },
                Title = "Current Generation",
                Fill = Brushes.Blue,
                StrokeThickness = .5,
                PointGeometry = null //use a null geometry when you have many series
            };
            var series3 = new ColumnSeries
            {
                Values = new ChartValues<double> { },
                Title = "Worst Generation",
                Fill = Brushes.Red,
                StrokeThickness = .5,
                PointGeometry = null //use a null geometry when you have many series
            };
            GenerationExplorer.Series.Add(series1);

            GenerationExplorer.Series.Add(series2);

            GenerationExplorer.Series.Add(series3);

            var series = new LineSeries
            {
                Values = new ChartValues<double> { },
                Fill = Brushes.Transparent,
                StrokeThickness = .5,
                PointGeometry = null
            };

            SimulationResultChart.Series = new SeriesCollection();
            SimulationResultChart.Series.Add(series);
        }

        private void SimulationWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            isSimulationEnabled = false;
            StopSimulationButton.IsEnabled = false;
            RunSimulationButton.IsEnabled = true;
            GenerationIdxSlider.Minimum = 0;
            GenerationIdxSlider.Maximum = generations.Count - 1;
            UpdateSelectedGenerationChart();
        }

        private void SimulationWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            for (int i = 0; i < nmbOfGenerations; i++)
            {
                generations.Add(new Population(generations.Last(), newPopulationParameters));
                SimulationResultChart.Series[0].Values.Add(generations.Last().BestRouteDistance);
                Application.Current.Dispatcher.BeginInvoke(new Action(() => { ProgressLabel.Content = string.Format("{0} / {1}", i, nmbOfGenerations); }));
                
                if (!isSimulationEnabled)
                {
                    break;
                }
            }
        }

        private void RunSimulationButton_Click(object sender, RoutedEventArgs e)
        {
            RunSimulationButton.IsEnabled = false;
            generations.Clear();
            nmbOfGenerations = int.Parse(NmbOfGenerationsTB.Text);
            nmbOfCities = int.Parse(NmbOfCitiesTB.Text);
            sizeOfPopulation = int.Parse(SizeOfPopulation.Text);
            newPopulationParameters = new NewPopulationParameters();
            newPopulationParameters.EliteSize = int.Parse(EliteSizeTB.Text);
            newPopulationParameters.MutationRate = int.Parse(MutationRateTB.Text);
            newPopulationParameters.MutationSize = int.Parse(MutationSizeTB.Text);
            nmbOfColumnsInChart = sizeOfPopulation > nmbOfColumnsInChart ? nmbOfColumnsInChart : sizeOfPopulation;
            citiesMap = new CitiesMap(nmbOfCities);


            SimulationResultChart.Series[0].Values.Clear();
            generations.Add(new Population(nmbOfCities, sizeOfPopulation, citiesMap));
            SimulationResultChart.Series[0].Values.Add(generations.Last().BestRouteDistance);
            isSimulationEnabled = true;

            simulationWorker.RunWorkerAsync();
            StopSimulationButton.IsEnabled = true;
        }

        private void NmbOfGenerationsTB_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }

        private static readonly Regex _regex = new Regex("[^0-9.-]+");
        private static bool IsTextAllowed(string text)
        {
            return !_regex.IsMatch(text);
        }

        private void NmbOfCitiesTB_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }

        private void SizeOfPopulation_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }

        private void MutationRateTB_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }

        private void MutationSizeTB_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }

        private void EliteSizeTB_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }

        private void GenerationIdxTB_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);

            if (!e.Handled)
            {
                if (int.Parse(e.Text) >= generations.Count)
                {
                    e.Handled = true;
                }
            }
        }

        private void GenerationIdxTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            selectedGenerationIdx = int.Parse(GenerationIdxTB.Text);
            UpdateSelectedGenerationChart();
        }

        private void PreviousGenerationButton_Click(object sender, RoutedEventArgs e)
        {
            if (selectedGenerationIdx > 0)
            {
                selectedGenerationIdx--;
            }

            GenerationIdxTB.Text = selectedGenerationIdx.ToString();
            UpdateSelectedGenerationChart();
        }

        private void NextGenerationButton_Click(object sender, RoutedEventArgs e)
        {
            if (selectedGenerationIdx < generations.Count - 1)
            {
                selectedGenerationIdx++;
            }

            GenerationIdxTB.Text = selectedGenerationIdx.ToString();
            UpdateSelectedGenerationChart();
        }

        private void UpdateSelectedGenerationChart()
        {
            if (generations == null)
            {
                return;
            }
            nmbOfColumnsInChart = generations.Count > nmbOfColumnsInChart ? nmbOfColumnsInChart : generations.Count;
            GenerationExplorer.Series[1].Values = new ChartValues<double>(generations[selectedGenerationIdx].GetDistanceRank().Take(nmbOfColumnsInChart));
            GenerationExplorer.Series[2].Values = new ChartValues<double>(generations[0].GetDistanceRank().Take(nmbOfColumnsInChart));
            GenerationExplorer.Series[0].Values = new ChartValues<double>(generations.Last().GetDistanceRank().Take(nmbOfColumnsInChart));
        }

        private void StopSimulationButton_Click(object sender, RoutedEventArgs e)
        {
           
            isSimulationEnabled = false;
            StopSimulationButton.IsEnabled = false;
            RunSimulationButton.IsEnabled = true;
            GenerationIdxSlider.Minimum = 0;
            GenerationIdxSlider.Maximum = generations.Count - 1;
        }

        private void GenerationIdxSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            GenerationIdxTB.Text = GenerationIdxSlider.Value.ToString();
            selectedGenerationIdx = (int)GenerationIdxSlider.Value;
            UpdateSelectedGenerationChart();
        }
    }
}
