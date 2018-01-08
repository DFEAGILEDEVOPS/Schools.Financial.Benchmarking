using System;

namespace SFB.Web.Domain.Helpers.Enums
{
    /// <summary>
    /// This should match to SFB.Web.Domain.Helpers.Constants.DataGroups
    /// It might be better to merge these
    /// </summary>
    public enum SchoolFinancialType
    {
        Academies,
        Maintained
    }

    public enum EstablishmentType
    {
        All,
        Academy,
        Maintained,
        MAT
    }

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
        TrustAndAcademies
    }
}
