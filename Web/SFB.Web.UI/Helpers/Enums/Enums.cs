using System.ComponentModel;

namespace SFB.Web.UI.Helpers.Enums
{
    public enum BenchmarkListOverwriteStrategy
    {
        Overwrite,
        Add        
    }

    public enum ComparisonListType
    {
        Statistical,
        CriteriaBased,
        UserCustomized
    }

    public enum ChartSchoolType
    {
        Academy,
        Maintained,
        Both
    }

    public enum ChartType
    {
        Regular,
        Total,
        CustomReport
    }

    public enum RevenueGroupType
    {
        Expenditure,
        Income,        
        Balance,
        AllExcludingSchoolPerf,
        AllIncludingSchoolPerf,
        Workforce,
        Custom
    }

    public enum ComparisonType
    {
        Basic,
        Advanced,
        Manual,
        BestInBreed
    }

    public enum UnitType
    {
        [Description("Absolute total")]
        AbsoluteMoney,
        [Description("Per teacher")]
        PerTeacher,
        [Description("Per pupil")]
        PerPupil,
        [Description("Percentage of total")]
        PercentageOfTotal,
        [Description("Total")]
        AbsoluteCount,
        [Description("Headcount per FTE")]
        HeadcountPerFTE,
        [Description("Percentage of workforce")]
        FTERatioToTotalFTE,
        [Description("Number of pupils per measure")]
        NoOfPupilsPerMeasure
    }

    public enum ChartGroupType
    {
        GrantFunding,
        SelfGenerated,
        Staff,
        Premises,
        Occupation,
        SuppliesAndServices,
        CostOfFinance,
        Community,
        Other,
        SpecialFacilities,
        InYearBalance,
        TotalIncome,
        TotalExpenditure,
        All,
        SP,
        Workforce
    }

    public enum ChartSubGroupType
    {
        SupplyStaff,
        OtherStaffCosts,
    }

    public enum CookieActions
    {
        SetDefault,        
        AddDefaultToList,
        Add,
        Remove,
        RemoveAll,
        UnsetDefault       
    }

    //public class CompareActions
    //{
    //    public const string ADD_TO_COMPARISON_LIST = "addtocompare";
    //    public const string REMOVE_FROM_COMPARISON_LIST = "removefromcompare";
    //    public const string MAKE_DEFAULT_BENCHMARK = "makedefaultbenchmark";
    //    public const string REMOVE_DEFAULT_BENCHMARK = "removedefaultbenchmark";
    //    public const string CLEAR_BENCHMARK_LIST = "clear";
    //}
}