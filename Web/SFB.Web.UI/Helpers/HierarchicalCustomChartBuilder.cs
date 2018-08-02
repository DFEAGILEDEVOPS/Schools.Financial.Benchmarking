using System.Collections.Generic;
using SFB.Web.UI.Models;

namespace SFB.Web.UI.Helpers
{
    public interface IHierarchicalCustomChartBuilder
    {
        List<HierarchicalChartViewModel> BuildList();
    }

    /// <summary>
    /// This class is used only for generating the JSON model of the custom charts
    /// </summary>
    public class HierarchicalCustomChartBuilder : IHierarchicalCustomChartBuilder
    {
        public List<HierarchicalChartViewModel> BuildList()
        {
            return new List<HierarchicalChartViewModel>()
            {
                //Total Expenditure
                new HierarchicalChartViewModel()
                {
                    GroupName = "Total expenditure",
                    Charts = new List<CustomChartSelectionViewModel>()
                    {
                        new CustomChartSelectionViewModel()
                        {
                            Name = "Total expenditure",                            
                            PercentageAvailable = false
                        }
                    }
                },

                //Staff
                new HierarchicalChartViewModel()
                {
                    GroupName = "Staff",
                    Charts = new List<CustomChartSelectionViewModel>()
                    {
                        new CustomChartSelectionViewModel()
                        {
                            Name = "Staff total",
                        },
                        new CustomChartSelectionViewModel()
                        {
                            Name = "Teaching staff",
                        },
                        new CustomChartSelectionViewModel()
                        {
                            Name = "Supply staff",
                        },
                        new CustomChartSelectionViewModel()
                        {
                            Name = "Education support staff",
                        },
                        new CustomChartSelectionViewModel()
                        {
                            Name = "Administrative and clerical staff",
                        },
                        new CustomChartSelectionViewModel()
                        {
                            Name = "Other staff costs",
                        }
                    }
                },

                //Premises
                new HierarchicalChartViewModel()
                {
                    GroupName = "Premises",
                    Charts = new List<CustomChartSelectionViewModel>()
                    {
                        new CustomChartSelectionViewModel()
                        {
                            Name = "Premises total",
                        },
                        new CustomChartSelectionViewModel()
                        {
                            Name = "Premises staff",
                        },
                        new CustomChartSelectionViewModel()
                        {
                            Name = "Cleaning and caretaking",
                        },
                        new CustomChartSelectionViewModel()
                        {
                            Name = "Maintenance and improvement",
                        },
                        new CustomChartSelectionViewModel()
                        {
                            Name = "PFI charges",
                        }
                    }
                },

                //Occupation
                new HierarchicalChartViewModel()
                {
                    GroupName = "Occupation",
                    Charts = new List<CustomChartSelectionViewModel>()
                    {
                        new CustomChartSelectionViewModel()
                        {
                            Name = "Occupation total",
                        },
                        new CustomChartSelectionViewModel()
                        {
                            Name = "Energy",
                        },
                        new CustomChartSelectionViewModel()
                        {
                            Name = "Water and sewerage",
                        },
                        new CustomChartSelectionViewModel()
                        {
                            Name = "Rates",
                        },
                        new CustomChartSelectionViewModel()
                        {
                            Name = "Other occupation costs",
                        },
                        new CustomChartSelectionViewModel()
                        {
                            Name = "Other insurance premiums",
                        },
                        new CustomChartSelectionViewModel()
                        {
                            Name = "Catering expenditure",
                        },
                        new CustomChartSelectionViewModel()
                        {
                            Name = "Rent and rates",
                        },
                    }
                },

                //Supplies and Services
                new HierarchicalChartViewModel()
                {
                    GroupName = "Supplies and services",
                    Charts = new List<CustomChartSelectionViewModel>()
                    {
                        new CustomChartSelectionViewModel()
                        {
                            Name = "Supplies and services total",
                        },
                        new CustomChartSelectionViewModel()
                        {
                            Name = "Administrative supplies",
                        },
                        new CustomChartSelectionViewModel()
                        {
                            Name = "Educational supplies",
                        },
                        new CustomChartSelectionViewModel()
                        {
                            Name = "Bought-in professional services",
                        }
                    }
                },

                //Special Facilities
                new HierarchicalChartViewModel()
                {
                    GroupName = "Special facilities",
                    Charts = new List<CustomChartSelectionViewModel>()
                    {
                        new CustomChartSelectionViewModel()
                        {
                            Name = "Special facilities",
                        },
                    }
                },

                //Cost of Finance
                new HierarchicalChartViewModel()
                {
                    GroupName = "Cost of finance",
                    Charts = new List<CustomChartSelectionViewModel>()
                    {
                        new CustomChartSelectionViewModel()
                        {
                            Name = "Cost of finance total",
                        },
                    }
                },

                //Community
                new HierarchicalChartViewModel()
                {
                    GroupName = "Community",
                    Charts = new List<CustomChartSelectionViewModel>()
                    {
                        new CustomChartSelectionViewModel()
                        {
                            Name = "Community expenditure total",
                        },
                    }
                },

                //Total Income
                new HierarchicalChartViewModel()
                {
                    GroupName = "Total income",
                    Charts = new List<CustomChartSelectionViewModel>()
                    {
                        new CustomChartSelectionViewModel()
                        {
                            Name = "Total income",
                            PercentageAvailable = false
                        },

                        new CustomChartSelectionViewModel()
                        {
                            Name = "Grant funding total",
                        },

                        new CustomChartSelectionViewModel()
                        {
                            Name = "Self-generated total",
                        }
                    }
                },

                //Grant Funding
                new HierarchicalChartViewModel()
                {
                    GroupName = "Grant funding",
                    Charts = new List<CustomChartSelectionViewModel>()
                    {
                        new CustomChartSelectionViewModel()
                        {
                            Name = "Grant funding total",
                        },
                        new CustomChartSelectionViewModel()
                        {
                            Name = "Direct grants",
                        },
                        new CustomChartSelectionViewModel()
                        {
                            Name = "Community grants",
                        },
                        new CustomChartSelectionViewModel()
                        {
                            Name = "Targeted grants",
                        },
                    }
                },

                //Self Generated
                new HierarchicalChartViewModel()
                {
                    GroupName = "Self generated",
                    Charts = new List<CustomChartSelectionViewModel>()
                    {
                        new CustomChartSelectionViewModel()
                        {
                            Name = "Self-generated funding total",
                        },
                        new CustomChartSelectionViewModel()
                        {
                            Name = "Community focused school facilities income"
                        },
                        new CustomChartSelectionViewModel()
                        {
                            Name = "Income from facilities and services",
                        },
                        new CustomChartSelectionViewModel()
                        {
                            Name = "Income from catering",
                        },
                        new CustomChartSelectionViewModel()
                        {
                            Name = "Donations and/or voluntary funds",
                        },
                        new CustomChartSelectionViewModel()
                        {
                            Name = "Income from contributions to visits",
                        },
                        new CustomChartSelectionViewModel()
                        {
                            Name = "Receipts from supply teacher insurance claims",
                        },
                        new CustomChartSelectionViewModel()
                        {
                            Name = "Receipts from other insurance claims",
                        },
                        new CustomChartSelectionViewModel()
                        {
                            Name = "Investment income",
                        },
                        new CustomChartSelectionViewModel()
                        {
                            Name = "Other self-generated income",
                        },
                    }
                },

                //In-Year Balance
                new HierarchicalChartViewModel()
                {
                    GroupName = "Balance",
                    Charts = new List<CustomChartSelectionViewModel>()
                    {
                        new CustomChartSelectionViewModel()
                        {
                            Name = "In-year balance",
                            PercentageAvailable = false
                        },
                    }
                },

                //Workforce
                new HierarchicalChartViewModel()
                {
                    GroupName = "Workforce",
                    Charts = new List<CustomChartSelectionViewModel>()
                    {
                        new CustomChartSelectionViewModel()
                        {
                            Name = "School workforce (Full Time Equivalent)",
                            AbsoluteCountAvailable = true,
                            HeadCountPerFTEAvailable = true,
                            NumberOfPupilsPerMeasureAvailable = true,
                            PerPupilAvailable = false,
                            PerTeacherAvailable = false,
                            AbsoluteMoneyAvailable = false,
                            PercentageAvailable = false
                        },
                        new CustomChartSelectionViewModel()
                        {
                            Name = "Teachers (Full Time Equivalent)",
                            AbsoluteCountAvailable = true,
                            HeadCountPerFTEAvailable = true,
                            PercentageOfWorkforceAvailable = true,
                            NumberOfPupilsPerMeasureAvailable = true,
                            PerPupilAvailable = false,
                            PerTeacherAvailable = false,
                            AbsoluteMoneyAvailable = false,
                            PercentageAvailable = false
                        },
                        new CustomChartSelectionViewModel()
                        {
                            Name = "Teachers with Qualified Teacher Status (%)",
                            AbsoluteCountAvailable = true,
                            PerPupilAvailable = false,
                            PerTeacherAvailable = false,
                            AbsoluteMoneyAvailable = false,
                            PercentageAvailable = false
                        },
                        new CustomChartSelectionViewModel()
                        {
                            Name = "Senior leadership (Full Time Equivalent)",
                            AbsoluteCountAvailable = true,
                            HeadCountPerFTEAvailable = true,
                            PercentageOfWorkforceAvailable = true,
                            NumberOfPupilsPerMeasureAvailable = true,
                            PerPupilAvailable = false,
                            PerTeacherAvailable = false,
                            AbsoluteMoneyAvailable = false,
                            PercentageAvailable = false
                        },
                        new CustomChartSelectionViewModel()
                        {
                            Name = "Teaching assistants (Full Time Equivalent)",
                            AbsoluteCountAvailable = true,
                            HeadCountPerFTEAvailable = true,
                            PercentageOfWorkforceAvailable = true,
                            NumberOfPupilsPerMeasureAvailable = true,
                            PerPupilAvailable = false,
                            PerTeacherAvailable = false,
                            AbsoluteMoneyAvailable = false,
                            PercentageAvailable = false
                        },
                        new CustomChartSelectionViewModel()
                        {
                            Name = "Non-classroom support staff (Full Time Equivalent)",
                            AbsoluteCountAvailable = true,
                            HeadCountPerFTEAvailable = true,
                            PercentageOfWorkforceAvailable = true,
                            NumberOfPupilsPerMeasureAvailable = true,
                            PerPupilAvailable = false,
                            PerTeacherAvailable = false,
                            AbsoluteMoneyAvailable = false,
                            PercentageAvailable = false
                        },
                        new CustomChartSelectionViewModel()
                        {
                            Name = "Auxiliary staff (Full Time Equivalent)",
                            AbsoluteCountAvailable = true,
                            HeadCountPerFTEAvailable = true,
                            PercentageOfWorkforceAvailable = true,
                            NumberOfPupilsPerMeasureAvailable = true,
                            PerPupilAvailable = false,
                            PerTeacherAvailable = false,
                            AbsoluteMoneyAvailable = false,
                            PercentageAvailable = false
                        },
                        new CustomChartSelectionViewModel()
                        {
                            Name = "School workforce (headcount)",
                            AbsoluteCountAvailable = true,
                            HeadCountPerFTEAvailable = true,
                            PercentageOfWorkforceAvailable = true,
                            NumberOfPupilsPerMeasureAvailable = true,
                            PerPupilAvailable = false,
                            PerTeacherAvailable = false,
                            AbsoluteMoneyAvailable = false,
                            PercentageAvailable = false
                        }
                    }
                }
            };
        }
    }
}