using System.Collections.Generic;

namespace SFB.Web.ApplicationCore.Models
{
    public class EfficiencyMetricModel
    {      
        public int URN { get; set; }
        public int Rank { get; set; }

        public string Name { get; set; }

        public List<EfficiencyMetricNeighbourModel> NeighbourDataModels { get; set; }

        public EfficiencyMetricModel(int urn, int rank, string name, List<EfficiencyMetricNeighbourModel> neighbourDataModels)
        {
            URN = urn;
            Rank = rank;
            Name = name;
            NeighbourDataModels = neighbourDataModels;
        }

    }
}
