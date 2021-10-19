using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using WPFUserInterface.Models;

namespace WPFUserInterface.Functions
{
    public static class DemoFunctions
    {
        #region Common

        /// <summary>
        /// Get some websites
        /// </summary>
        /// <returns>List of websites</returns>
        private static List<string> GetWebsites()
        {
            //Create list of some websites
            var websites = new List<string>
            {
                //"https://www.yahoo.com",
                "https://www.google.com",
                "https://www.microsoft.com",
                "https://www.cnn.com",
                "https://www.amazon.com",
                "https://www.facebook.com",
                "https://www.twitter.com",
                "https://www.codeproject.com",
                "https://www.stackoverflow.com",
                "https://en.wikipedia.org/wiki/.NET_Framework"
            };

            return websites;
        }

        #endregion Common

        #region Sync

        /// <summary>
        /// Download website sync
        /// </summary>
        /// <param name="websiteURL">Website URL</param>
        /// <returns>Website data model</returns>
        private static WebsiteDataModel DownloadWebsiteSync(string websiteURL)
        {
            //Create web client
            var webClient = new WebClient();

            //Create website data model
            var websiteDataModel = new WebsiteDataModel
            {
                Url = websiteURL,
                Data = webClient.DownloadString(websiteURL)
            };

            return websiteDataModel;
        }

        /// <summary>
        /// Run download sync
        /// </summary>
        /// <returns>List of website data models</returns>
        public static List<WebsiteDataModel> RunDownloadSync()
        {
            //Get some websites
            var websites = GetWebsites();

            //Prepare output
            var websiteDataModels = new List<WebsiteDataModel>();

            //For each website from the list of websites
            foreach (var website in websites)
            {
                //download website sync
                var websiteDataModel = DownloadWebsiteSync(website);

                //add downloaded website data model to the list of website data models
                websiteDataModels.Add(websiteDataModel);
            }

            return websiteDataModels;
        }

        #endregion Sync

        #region Parallel sync

        /// <summary>
        /// Run download parallel sync
        /// </summary>
        /// <returns>List of website data models</returns>
        public static List<WebsiteDataModel> RunDownloadParallelSync()
        {
            //Get some websites
            var websites = GetWebsites();

            //Prepare output
            var websiteDataModels = new List<WebsiteDataModel>();

            //For each website from the list of websites
            Parallel.ForEach(websites, website =>
            {
                //download website sync
                var websiteDataModel = DownloadWebsiteSync(website);

                //add downloaded website data model to the list of website data models
                websiteDataModels.Add(websiteDataModel);
            });

            return websiteDataModels;
        }

        #endregion Parallel sync

        #region Async

        /// <summary>
        /// Download website async
        /// </summary>
        /// <param name="websiteURL">Website URL</param>
        /// <returns>Website data model</returns>
        private static async Task<WebsiteDataModel> DownloadWebsiteAsync(string websiteURL)
        {
            //Create web client
            var webClient = new WebClient();

            //Create website data model
            var websiteDataModel = new WebsiteDataModel
            {
                Url = websiteURL,
                Data = await webClient.DownloadStringTaskAsync(websiteURL)
            };

            return websiteDataModel;
        }

        /// <summary>
        /// Run download async
        /// </summary>
        /// <param name="progress">Progress</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>List of website data models</returns>
        public static async Task<List<WebsiteDataModel>> RunDownloadAsync(IProgress<ProgressReportModel> progress, CancellationToken cancellationToken)
        {
            //Get some websites
            var websites = GetWebsites();

            //Prepare output
            var websiteDataModels = new List<WebsiteDataModel>();

            //Prepare progress report model
            var report = new ProgressReportModel();

            //For each website from the list of websites
            foreach (string website in websites)
            {
                //download website async
                var websiteDataModel = await DownloadWebsiteAsync(website);

                //add downloaded website data model to the list of website data models
                websiteDataModels.Add(websiteDataModel);

                //if cancellation requested, throw an OperationCanceledException (executes when hits)
                cancellationToken.ThrowIfCancellationRequested();

                //get websites downloaded
                report.WebsitesDownloaded = websiteDataModels;

                //get percentage complete
                report.PercentageComplete = websiteDataModels.Count * 100 / websites.Count;

                //report a progress update
                progress.Report(report);
            }

            return websiteDataModels;
        }

        #endregion Async

        #region Parallel async

        /// <summary>
        /// Run download parallel async
        /// </summary>
        /// <returns>List of website data models</returns>
        public static async Task<List<WebsiteDataModel>> RunDownloadParallelAsync()
        {
            //Get some websites
            var websites = GetWebsites();

            //Create list of tasks of type WebsiteDataModel
            var tasks = new List<Task<WebsiteDataModel>>();

            //For each website from the list of websites
            foreach (string website in websites)
                //download website async and add downloaded website data model to list of tasks
                tasks.Add(DownloadWebsiteAsync(website));

            //Get the results when all tasks are done
            var results = await Task.WhenAll(tasks);

            //Create the output
            var websiteDataModels = new List<WebsiteDataModel>(results);

            return websiteDataModels;
        }

        /// <summary>
        /// Run download parallel async
        /// </summary>
        /// <param name="progress">Progress</param>
        /// <returns>List of website data models</returns>
        public static async Task<List<WebsiteDataModel>> RunDownloadParallelAsync(IProgress<ProgressReportModel> progress)
        {
            //Get some websites
            var websites = GetWebsites();

            //Prepare output
            var websiteDataModels = new List<WebsiteDataModel>();

            //Prepare progress report model
            var report = new ProgressReportModel();

            await Task.Run(() =>
            {
                //For each website from the list of websites
                Parallel.ForEach(websites, website =>
                {
                    //download website sync
                    var websiteDataModel = DownloadWebsiteSync(website);

                    //add downloaded website data model to the list of website data models
                    websiteDataModels.Add(websiteDataModel);

                    //get websites downloaded
                    report.WebsitesDownloaded = websiteDataModels;

                    //get percentage complete
                    report.PercentageComplete = websiteDataModels.Count * 100 / websites.Count;

                    //report a progress update
                    progress.Report(report);
                });
            });

            #region Async

            //await Task.Run(() =>
            //{
            //    Parallel.ForEach(websites, async site =>
            //    {
            //        var websiteDataModel = await DownloadWebsiteAsync(site);
            //        websiteDataModels.Add(websiteDataModel);
            //        report.WebsitesDownloaded = websiteDataModels;
            //        report.PercentageComplete = websiteDataModels.Count * 100 / websites.Count;
            //        progress.Report(report);
            //    });
            //});

            #endregion Async

            return websiteDataModels;
        }

        #endregion Parallel async
    }
}
