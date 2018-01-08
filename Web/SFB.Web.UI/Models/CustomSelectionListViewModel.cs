using System.Collections.Generic;
using SFB.Web.Domain.Helpers.Enums;

namespace SFB.Web.UI.Models
{
    public class CustomSelectionListViewModel
    {
        public string CentralFinance { get; set; }

        public List<HierarchicalChartViewModel> HierarchicalCharts { get; set; }

    }
}