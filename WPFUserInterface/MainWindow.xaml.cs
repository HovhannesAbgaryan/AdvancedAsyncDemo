using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using WPFUserInterface.Functions;
using WPFUserInterface.Models;

namespace WPFUserInterface
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Fields

        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        #endregion Fields

        #region Constructor

        public MainWindow()
        {
            InitializeComponent();
        }

        #endregion Constructor

        #region Events

        #region Sync

        /// <summary>
        /// Execute sync
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Routed event args</param>
        private void executeSync_Click(object sender, RoutedEventArgs e)
        {
            //Start new stopwatch
            var watch = Stopwatch.StartNew();

            //Run download sync
            var websiteDataModels = DemoFunctions.RunDownloadSync();

            //Print website data models
            Print(websiteDataModels);

            //Stop watch
            watch.Stop();

            //Print total execution time for sync in results window
            resultsWindow.Text += $"Total execution time for sync: { watch.ElapsedMilliseconds }ms";
        }

        #endregion Sync

        #region Parallel sync

        /// <summary>
        /// Execute parallel sync
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Routed event args</param>
        private void executeParallelSync_Click(object sender, RoutedEventArgs e)
        {
            //Start new stopwatch
            var watch = Stopwatch.StartNew();

            //Run download parallel sync
            var websiteDataModels = DemoFunctions.RunDownloadParallelSync();

            //Print website data models
            Print(websiteDataModels);

            //Stop watch
            watch.Stop();

            //Print total execution time for parallel sync in results window
            resultsWindow.Text += $"Total execution time for parallel sync: { watch.ElapsedMilliseconds }ms";
        }

        #endregion Parallel sync

        #region Async

        /// <summary>
        /// Execute async
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Routed event args</param>
        private async void executeAsync_Click(object sender, RoutedEventArgs e)
        {
            //Create new progress
            var progress = new Progress<ProgressReportModel>();

            //When progress is changed, report about that progress
            progress.ProgressChanged += ReportProgress;

            //Start new stopwatch
            var watch = Stopwatch.StartNew();

            try
            {
                //Run download async
                var websiteDataModels = await DemoFunctions.RunDownloadAsync(progress, cancellationTokenSource.Token);

                //Print website data models
                Print(websiteDataModels);
            }
            catch (OperationCanceledException)
            {
                //If operation was cancelled, print information about it in results window
                resultsWindow.Text += $"The async download was cancelled. { Environment.NewLine }";
            }

            //Stop watch
            watch.Stop();

            //Print total execution time for async in results window
            resultsWindow.Text += $"Total execution time for async: { watch.ElapsedMilliseconds }ms";
        }

        #endregion Async

        #region Parallel async

        /// <summary>
        /// Execute parallel async
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Routed event args</param>
        private async void executeParallelAsync_Click(object sender, RoutedEventArgs e)
        {
            //Create new progress
            var progress = new Progress<ProgressReportModel>();

            //When progress is changed, report about that progress
            progress.ProgressChanged += ReportProgress;

            //Start new stopwatch
            var watch = Stopwatch.StartNew();

            //Run download parallel async
            var websiteDataModels = await DemoFunctions.RunDownloadParallelAsync(progress);

            //Print website data models
            Print(websiteDataModels);

            //Stop watch
            watch.Stop();

            //Print total execution time for parallel async in results window
            resultsWindow.Text += $"Total execution time for parallel async: { watch.ElapsedMilliseconds }ms";
        }

        #endregion Parallel async

        #region Cancel operation

        /// <summary>
        /// Cancel operation
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Routed event args</param>
        private void cancelOperation_Click(object sender, RoutedEventArgs e)
        {
            cancellationTokenSource.Cancel();
        }

        #endregion Cancel operation

        #endregion Events

        #region Functions

        /// <summary>
        /// Print website data models
        /// </summary>
        /// <param name="websiteDataModels">List of website data models</param>
        private void Print(List<WebsiteDataModel> websiteDataModels)
        {
            //Clear results window
            resultsWindow.Text = "";

            //For each website data model from the list of website data models
            foreach (var websiteDataModel in websiteDataModels)
                //print information about website data model in results window
                resultsWindow.Text += $"{ websiteDataModel.Url } downloaded: { websiteDataModel.Data.Length } characters long.{ Environment.NewLine }";
        }

        /// <summary>
        /// Report progress
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="report">Progress report model</param>
        private void ReportProgress(object sender, ProgressReportModel report)
        {
            //Set value of dashboard progress to progress report model's percentage complete
            dashboardProgress.Value = report.PercentageComplete;

            //Print websites downloaded
            Print(report.WebsitesDownloaded);
        }

        #endregion Functions
    }
}
