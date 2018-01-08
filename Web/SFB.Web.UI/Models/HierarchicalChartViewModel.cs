using System.Collections.Generic;

namespace SFB.Web.UI.Models
{
    public class HierarchicalChartViewModel
    {
        public string GroupName { get; set; }

        public List<CustomChartSelectionViewModel> Charts { get; set; }

        public HierarchicalChartViewModel()
        {
            Charts = new List<CustomChartSelectionViewModel>();
        }
    }
}