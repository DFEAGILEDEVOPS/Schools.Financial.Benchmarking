using System.Collections.Generic;
using SFB.Web.ApplicationCore;
using SFB.Web.ApplicationCore.Entities;

namespace SFB.Web.ApplicationCore.Models
{
    public class ComparisonResult
    {
        public List<SchoolTrustFinancialDataObject> BenchmarkSchools { get; set; }
        public BenchmarkCriteria BenchmarkCriteria { get; set; }
    }
}
