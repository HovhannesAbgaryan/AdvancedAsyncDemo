using System.Collections.Generic;

namespace WPFUserInterface.Models
{
    /// <summary>
    /// Progress report model
    /// </summary>
    public class ProgressReportModel
    {
        /// <summary>
        /// Websites downloaded
        /// </summary>
        public List<WebsiteDataModel> WebsitesDownloaded { get; set; } = new List<WebsiteDataModel>();

        /// <summary>
        /// Percentage complete (%)
        /// </summary>
        public int PercentageComplete { get; set; } = 0;
    }
}
