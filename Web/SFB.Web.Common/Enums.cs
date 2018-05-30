using System;

namespace SFB.Web.Common
{
    //Used in conjunction with entity's Financial Type to determine the target data groups in fibre-directory(DB collection)
    public enum EstablishmentType
    {
        Academies,
        Maintained,
        MAT,
        All
    }

    ////TODO: Can be merged with SchoolFinancialType
    //public enum EstablishmentType
    //{
    //    All,
    //    Academy,
    //    Maintained,
    //    MAT
    //}

    public enum ComparisonArea
    {
        All,
        LaCode,
        LaName,
        Location
    }

    [Flags]
    public enum ValidationIndicators
    {
        None = 0,
        Null = 1,
        Empty = 2,
        Zero = 4,
        Negative = 8,
        Number = 16,
        Real = 32,
        Bool = 64,
        InRange = 128,
        True = 256,
        False = 512
    }

    public enum ValidationAction
    {
        None,
        Throw,
        ReturnDefault,
        ReturnNull,
        ReturnZero,
        ReturnEmpty,
        ReturnFalse,
        ReturnFirst,
        ReturnLast
    }

    public enum CentralFinancingType
    {
        Include,
        Exclude
    }

    public enum MatFinancingType
    {
        TrustOnly,
        TrustAndAcademies,
        AcademiesOnly
    }

    public enum ChartFormat
    {
        Charts,
        Tables
    }
}
