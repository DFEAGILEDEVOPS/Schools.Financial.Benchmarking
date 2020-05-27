using System.Collections.Generic;
using System.Linq;

namespace SFB.Web.ApplicationCore.Models
{
    public class EfficiencyMetricModel
    {      
        public int URN { get; set; }
        public int Rank { get; set; }

        public string Name { get; set; }

        public string Phase { get; set; }

        public string LocalAuthority { get; set; }

        public List<EfficiencyMetricNeighbourModel> NeighbourDataModels { get; set; }

        public EfficiencyMetricModel(int urn, int rank, string name, string phase, string localAuthority, List<EfficiencyMetricNeighbourModel> neighbourDataModels)
        {
            URN = urn;
            Rank = rank;
            Name = name;
            Phase = phase;
            LocalAuthority = localAuthority;
            NeighbourDataModels = neighbourDataModels;
        }

    }
}
