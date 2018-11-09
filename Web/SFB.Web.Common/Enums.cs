using System;
using System.ComponentModel;

namespace SFB.Web.Common
{
    public enum EstablishmentType
    {
        Academies,
        Maintained,
        MAT,
        All
    }

    public enum CentralFinancingType
    {
        [Description("including MAT central finance")]
        Include,
        [Description("excluding MAT central finance")]
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
}
