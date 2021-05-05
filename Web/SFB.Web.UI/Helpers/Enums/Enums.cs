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
        CustomReport,
        OneClick
    }

    public enum TabType
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
        BestInClass,
        OneClick,
        EfficiencyTop,
        EfficiencyManual,
        Specials,
        FederationBasic
    }

    public enum UnitType
    {
        [Description("Absolute total")]
        AbsoluteMoney,
        [Description("Per teacher")]
        PerTeacher,
        [Description("Per pupil")]
        PerPupil,
        [Description("Percentage of total income")]
        PercentageOfTotalIncome,
        [Description("Percentage of total expenditure")]
        PercentageOfTotalExpenditure,
        [Description("Total")]
        AbsoluteCount,
        [Description("Headcount per FTE")]
        HeadcountPerFTE,
        [Description("Percentage of workforce")]
        FTERatioToTotalFTE,
        [Description("Number of pupils per staff role")]
        NoOfPupilsPerMeasure,
        [Description("Percentage")]
        PercentageTeachers
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
        Workforce,
        Custom
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
}