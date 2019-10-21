namespace SFB.Web.ApplicationCore.Helpers
{
    public static class SchoolFormatHelpers
    {
        public static string FinancialTermFormatAcademies(int endYear)
        {
            return $"{endYear - 1} / {endYear}";
        }

        public static string FinancialTermFormatMaintained(int endYear)
        {
            return $"{endYear - 1} - {endYear}";
        }

    }
}