using System.Collections.Generic;
using SFB.Web.UI.Models;

namespace SFB.Web.UI.Helpers
{
    public interface IHierarchicalCustomChartBuilder
    {
        List<HierarchicalChartViewModel> BuildList();
    }

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
                            FieldName = "Total Expenditure",
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
                            FieldName = "Staff Total",
                        },
                        new CustomChartSelectionViewModel()
                        {
                            Name = "Teaching staff",
                            FieldName = "Teaching staff",
                        },
                        new CustomChartSelectionViewModel()
                        {
                            Name = "Supply staff",
                            FieldName = "Supply Staff"
                        },
                        new CustomChartSelectionViewModel()
                        {
                            Name = "Education support staff",
                            FieldName = "Education support staff"
                        },
                        new CustomChartSelectionViewModel()
                        {
                            Name = "Administrative and clerical staff",
                            FieldName = "Administrative and clerical staff",
                        },
                        new CustomChartSelectionViewModel()
                        {
                            Name = "Other staff costs",
                            FieldName = "Other Staff Costs",
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
                            FieldName = "Premises",
                        },
                        new CustomChartSelectionViewModel()
                        {
                            Name = "Premises staff",
                            FieldName = "Premises staff",
                        },
                        new CustomChartSelectionViewModel()
                        {
                            Name = "Cleaning and caretaking",
                            FieldName = "Cleaning and caretaking",
                        },
                        new CustomChartSelectionViewModel()
                        {
                            Name = "Maintenance and improvement",
                            FieldName = "Maintenance & Improvement",
                        },
                        new CustomChartSelectionViewModel()
                        {
                            Name = "PFI charges",
                            FieldName = "PFI Charges"
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
                            FieldName = "Occupation",
                        },
                        new CustomChartSelectionViewModel()
                        {
                            Name = "Energy",
                            FieldName = "Energy",
                        },
                        new CustomChartSelectionViewModel()
                        {
                            Name = "Water and sewerage",
                            FieldName = "Water and sewerage",
                        },
                        new CustomChartSelectionViewModel()
                        {
                            Name = "Rates",
                            FieldName = "Rates",
                        },
                        new CustomChartSelectionViewModel()
                        {
                            Name = "Other occupation costs",
                            FieldName = "Other occupation costs",
                        },
                        new CustomChartSelectionViewModel()
                        {
                            Name = "Other insurance premiums",
                            FieldName = "Other insurance premiums",
                        },
                        new CustomChartSelectionViewModel()
                        {
                            Name = "Catering expenditure",
                            FieldName = "Catering Exp"
                        },
                        new CustomChartSelectionViewModel()
                        {
                            Name = "Rent and rates",
                            FieldName = "Rent and Rates"
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
                            FieldName = "Supplies and Services"
                        },
                        new CustomChartSelectionViewModel()
                        {
                            Name = "Administrative supplies",
                            FieldName = "Administrative supplies - non educational"
                        },
                        new CustomChartSelectionViewModel()
                        {
                            Name = "Educational supplies",
                            FieldName = "Educational Supplies"
                        },
                        new CustomChartSelectionViewModel()
                        {
                            Name = "Bought-in professional services",
                            FieldName = "Brought in Professional Sevices"
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
                            FieldName = "Special facilities"
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
                            FieldName = "Cost of Finance"
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
                            FieldName = "Community Exp"
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
                            FieldName = "Total Income",
                            PercentageAvailable = false
                        },

                        new CustomChartSelectionViewModel()
                        {
                            Name = "Grant funding total",
                            FieldName = "Grant Funding"
                        },

                        new CustomChartSelectionViewModel()
                        {
                            Name = "Self-generated total",
                            FieldName = "Self Generated Funding"
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
                            FieldName = "Grant Funding"
                        },
                        new CustomChartSelectionViewModel()
                        {
                            Name = "Direct grants",
                            FieldName = "Direct Grant"
                        },
                        new CustomChartSelectionViewModel()
                        {
                            Name = "Community grants",
                            FieldName = "Community Grants"
                        },
                        new CustomChartSelectionViewModel()
                        {
                            Name = "Targeted grants",
                            FieldName = "Targeted Grants"
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
                            FieldName = "Self Generated Funding"
                        },
                        new CustomChartSelectionViewModel()
                        {
                            Name = "Community focused school facilities income"
                        },
                        new CustomChartSelectionViewModel()
                        {
                            Name = "Income from facilities and services",
                            FieldName = "Income from facilities and services"
                        },
                        new CustomChartSelectionViewModel()
                        {
                            Name = "Income from catering",
                            FieldName = "Income from catering"
                        },
                        new CustomChartSelectionViewModel()
                        {
                            Name = "Donations and/or voluntary funds",
                            FieldName = "Donations and/or voluntary funds"
                        },
                        new CustomChartSelectionViewModel()
                        {
                            Name = "Income from contributions to visits",
                            FieldName = "Income from contributions to visits etc"
                        },
                        new CustomChartSelectionViewModel()
                        {
                            Name = "Receipts from supply teacher insurance claims",
                            FieldName = "Receipts from supply teacher insurance claims"
                        },
                        new CustomChartSelectionViewModel()
                        {
                            Name = "Receipts from other insurance claims",
                            FieldName = "Receipts from other insurance claims"
                        },
                        new CustomChartSelectionViewModel()
                        {
                            Name = "Investment income",
                            FieldName = "Investment income"
                        },
                        new CustomChartSelectionViewModel()
                        {
                            Name = "Other self-generated income",
                            FieldName = "Other self-generated income"
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
                            FieldName = "In Year Balance",
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
                            Name = "School workforce (FTE)",
                            FieldName = "TotalSchoolWorkforceFullTimeEquivalent",
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
                            Name = "Teachers (FTE)",
                            FieldName = "TotalNumberOfTeachersFullTimeEquivalent",
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
                            Name = "Teachers with QTS (%)",
                            FieldName = "TeachersWithQualifiedTeacherStatus",
                            AbsoluteCountAvailable = true,
                            PerPupilAvailable = false,
                            PerTeacherAvailable = false,
                            AbsoluteMoneyAvailable = false,
                            PercentageAvailable = false
                        },
                        new CustomChartSelectionViewModel()
                        {
                            Name = "Senior leadership (FTE)",
                            FieldName = "TotalNumberOfTeachersInTheLeadershipGroupFullTimeEquivalent",
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
                            Name = "Teaching assistants (FTE)",
                            FieldName = "TotalNumberOfTeachingAssistantsFullTimeEquivalent",
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
                            Name = "Non-classroom support staff (FTE)",
                            FieldName = "TotalNumberOfNonClassroomBasedSchoolSupportStaffExcludingAuxiliaryStaffFullTimeEquivalent",
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
                            Name = "Auxiliary staff (FTE)",
                            FieldName = "TotalNumberOfAuxiliaryStaffFullTimeEquivalent",
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
                            FieldName = "TotalSchoolWorkforceHeadcount",
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