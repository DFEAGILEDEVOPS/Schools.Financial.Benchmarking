namespace SFB.Web.UI.Models
{
    public class CustomChartSelectionViewModel
    {
        private bool _perPupilAvailable = true;
        private bool _perTeacherAvailable = true;
        private bool _percentageExpenditureAvailable = true;
        private bool _percentageIncomeAvailable = true;
        private bool _absoluteMoneyAvailable = true;

        public int Id { get; set; }

        public string Name { get; set; }

        public bool PerPupilAvailable
        {
            get { return _perPupilAvailable; }
            set { _perPupilAvailable = value; }
        }

        public bool PerPupilSelected { get; set; }

        public bool PerTeacherAvailable
        {
            get { return _perTeacherAvailable; }
            set { _perTeacherAvailable = value; }
        }

        public bool PerTeacherSelected { get; set; }

        public bool PercentageExpenditureAvailable
        {
            get { return _percentageExpenditureAvailable; }
            set { _percentageExpenditureAvailable = value; }
        }

        public bool PercentageIncomeAvailable
        {
            get { return _percentageIncomeAvailable; }
            set { _percentageIncomeAvailable = value; }
        }

        public bool PercentageExpenditureSelected { get; set; }
        public bool PercentageIncomeSelected { get; set; }

        public bool AbsoluteMoneyAvailable
        {
            get { return _absoluteMoneyAvailable; }
            set { _absoluteMoneyAvailable = value; }
        }

        public bool AbsoluteMoneySelected { get; set; }

        public bool AbsoluteCountAvailable { get; set; }
        public bool AbsoluteCountSelected { get; set; }

        public bool HeadCountPerFTEAvailable { get; set; }
        public bool HeadCountPerFTESelected { get; set; }

        public bool PercentageOfWorkforceAvailable { get; set; }
        public bool PercentageOfWorkforceSelected { get; set; }

        public bool NumberOfPupilsPerMeasureAvailable { get; set; }
        public bool NumberOfPupilsPerMeasureSelected { get; set; }

        public bool PercentageTeachersAvailable { get; set; }
        public bool PercentageTeachersSelected { get; set; }
    }
}