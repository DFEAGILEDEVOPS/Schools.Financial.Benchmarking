using SFB.Web.Common;
using System.Collections.Generic;
using System.Linq;

namespace SFB.Web.UI.Models
{

    public class TrustCharacteristicsViewModel : ViewModelBase
    {
        public TrustViewModel BenchmarkTrust { get; set; }
        public TrustComparisonListModel TrustComparisonList { get; set; }
        public List<SchoolCharacteristic> TrustCharacteristics { get; set; }
        public BenchmarkCriteria BenchmarkCriteria { get; set; }


        public TrustCharacteristicsViewModel(TrustViewModel trust, TrustComparisonListModel trustComparisonList)
        {
            this.TrustComparisonList = trustComparisonList;
            this.BenchmarkTrust = trust;
            this.TrustCharacteristics = BuildTrustCharacteristics(trust);
            this.BenchmarkCriteria = new BenchmarkCriteria();
        }

        public string this[string question]
        {
            get
            {
                return TrustCharacteristics.Find(s => s.Question == question).Value;
            }
        }

        private List<SchoolCharacteristic> BuildTrustCharacteristics(TrustViewModel schoolVM)
        {
            var latestTrustData = schoolVM.HistoricalFinancialDataModels.Last();
            var list = new List<SchoolCharacteristic>();
            list.Add(new SchoolCharacteristic() { Question = TrustCharacteristicsQuestions.NUMBER_OF_PUPILS, Value = latestTrustData.PupilCount.ToString("N0") });
            list.Add(new SchoolCharacteristic() { Question = TrustCharacteristicsQuestions.NUMBER_OF_SCHOOLS, Value = latestTrustData.SchoolCount.ToString("N0") });
            list.Add(new SchoolCharacteristic() { Question = TrustCharacteristicsQuestions.TOTAL_INCOME, Value = latestTrustData.TotalIncome.ToString("C0") });

            return list;
        }
    }
}
