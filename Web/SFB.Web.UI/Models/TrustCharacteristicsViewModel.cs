using SFB.Web.ApplicationCore.Helpers.Constants;
using SFB.Web.ApplicationCore.Models;
using SFB.Web.ApplicationCore;
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
            list.Add(new SchoolCharacteristic() { Question = TrustCharacteristicsQuestions.NUMBER_OF_PUPILS, Value = latestTrustData.PupilCount.GetValueOrDefault().ToString("N0") });
            list.Add(new SchoolCharacteristic() { Question = TrustCharacteristicsQuestions.NUMBER_OF_SCHOOLS, Value = latestTrustData.SchoolCount.GetValueOrDefault().ToString("N0") });
            list.Add(new SchoolCharacteristic() { Question = TrustCharacteristicsQuestions.TOTAL_INCOME, Value = latestTrustData.TotalIncome.GetValueOrDefault().ToString("C0") });
            list.Add(new SchoolCharacteristic() { Question = TrustCharacteristicsQuestions.SCHOOL_OVERALL_PHASE, Value = latestTrustData.SchoolOverallPhase });
            list.Add(new SchoolCharacteristic() { Question = TrustCharacteristicsQuestions.CROSS_PRIMARY, Value = latestTrustData.CrossPhaseBreakdownPrimary.ToString() });
            list.Add(new SchoolCharacteristic() { Question = TrustCharacteristicsQuestions.CROSS_SECONDARY, Value = latestTrustData.CrossPhaseBreakdownSecondary.ToString() });
            list.Add(new SchoolCharacteristic() { Question = TrustCharacteristicsQuestions.CROSS_SPECIAL, Value = latestTrustData.CrossPhaseBreakdownSpecial.ToString() });
            list.Add(new SchoolCharacteristic() { Question = TrustCharacteristicsQuestions.CROSS_PRU, Value = latestTrustData.CrossPhaseBreakdownPru.ToString() });
            list.Add(new SchoolCharacteristic() { Question = TrustCharacteristicsQuestions.CROSS_AP, Value = latestTrustData.CrossPhaseBreakdownAP.ToString() });
            list.Add(new SchoolCharacteristic() { Question = TrustCharacteristicsQuestions.CROSS_AT, Value = latestTrustData.CrossPhaseBreakdownAT.ToString() });

            return list;
        }
    }
}
