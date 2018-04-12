using System.Collections.Generic;
using Microsoft.Azure.Documents;
using SFB.Web.Common;

namespace SFB.Web.Domain.Models
{
    public class ComparisonResult
    {
        public List<Document> BenchmarkSchools { get; set; }
        public BenchmarkCriteria BenchmarkCriteria { get; set; }
    }
}
