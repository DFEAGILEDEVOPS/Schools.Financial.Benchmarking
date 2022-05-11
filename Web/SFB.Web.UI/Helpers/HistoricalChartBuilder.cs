using System.Collections.Generic;
using System.Linq;
using SFB.Web.ApplicationCore.Helpers.Constants;
using SFB.Web.ApplicationCore.Helpers.Enums;
using SFB.Web.UI.Helpers.Enums;
using SFB.Web.UI.Models;

namespace SFB.Web.UI.Helpers
{
    public interface IHistoricalChartBuilder
    {
        List<ChartViewModel> Build(TabType tabType, EstablishmentType estabType);

        List<ChartViewModel> Build(TabType tabType, ChartGroupType chartGroup, EstablishmentType estabType, UnitType unit = UnitType.AbsoluteCount);
    }

    public class HistoricalChartBuilder : IHistoricalChartBuilder
    {
        public List<ChartViewModel> Build(TabType tabType, EstablishmentType estabType)
        {
            var chartList = Build(tabType);
            if (estabType == EstablishmentType.Academies || estabType == EstablishmentType.MAT)
            {
                chartList = FilterAcademyOnlyCharts(chartList);
            }
            else
            {
                chartList = FilterMaintainedOnlyCharts(chartList);
            }
            return chartList;
        }

        public List<ChartViewModel> Build(TabType tabType, ChartGroupType chartGroup,
            EstablishmentType estabType, UnitType unit)
        {
            var chartList = Build(tabType, chartGroup);

            RemoveIrrelevantCharts(unit, chartList);

            if (estabType == EstablishmentType.Academies || estabType == EstablishmentType.MAT)
            {
                chartList = FilterAcademyOnlyCharts(chartList);
            }
            else
            {
                chartList = FilterMaintainedOnlyCharts(chartList);
            }

            return chartList;
        }

        private void RemoveIrrelevantCharts(UnitType unit, List<ChartViewModel> chartList)
        {
            switch (unit)
            {
                case UnitType.PercentageOfTotalExpenditure:
                    chartList.RemoveAll(c => c.Id == 1);//Total expenditure
                    break;
                case UnitType.PercentageOfTotalIncome:
                    chartList.RemoveAll(c => c.Id == 33);//Total income
                    break;
                case UnitType.HeadcountPerFTE:
                    chartList.RemoveAll(c => c.Id == 57);//School workforce (headcount)
                    chartList.RemoveAll(c => c.Id == 52);//Teachers with Qualified Teacher Status (%)
                    break;
                case UnitType.FTERatioToTotalFTE:
                    chartList.RemoveAll(c => c.Id == 52);//Teachers with Qualified Teacher Status (%)
                    chartList.RemoveAll(c => c.Id == 50);//School workforce (Full Time Equivalent)
                    chartList.RemoveAll(c => c.Id == 57);//School workforce (headcount)
                    break;
                case UnitType.NoOfPupilsPerMeasure:
                    chartList.RemoveAll(c => c.Id == 52);//Teachers with Qualified Teacher Status (%)
                    break;
            }
        }

        private List<ChartViewModel> Build(TabType tabType)
        {
            return
                BuildChartList()
                    .Where(c => (tabType == TabType.AllIncludingSchoolPerf || c.TabType == tabType)
                                 || (tabType == TabType.AllExcludingSchoolPerf && c.TabType != TabType.AllIncludingSchoolPerf))
                    .ToList();
        }

        private List<ChartViewModel> Build(TabType tabType, ChartGroupType chartGroup)
        {
            var chartList =
                BuildChartList().Where(c => ((tabType == TabType.AllIncludingSchoolPerf || c.TabType == tabType) && (chartGroup == ChartGroupType.All || c.ChartGroup == chartGroup))
                                            || ((tabType == TabType.AllExcludingSchoolPerf && c.TabType != TabType.AllIncludingSchoolPerf) && (chartGroup == ChartGroupType.All || c.ChartGroup == chartGroup))
                                      )
                    .ToList();
            return chartList;
        }

        private List<ChartViewModel> FilterMaintainedOnlyCharts(List<ChartViewModel> chartList)
        {
            chartList =
                chartList.Where(
                        c =>
                            c.ChartSchoolType == ChartSchoolType.Maintained ||
                            c.ChartSchoolType == ChartSchoolType.Both)
                    .ToList();

            chartList.ForEach(c => c.SubCharts = c.SubCharts?.Where(s => s.ChartSchoolType == ChartSchoolType.Maintained || s.ChartSchoolType == ChartSchoolType.Both).ToList());
            return chartList;
        }

        private List<ChartViewModel> FilterAcademyOnlyCharts(List<ChartViewModel> chartList)
        {
            chartList =
                chartList.Where(
                        c =>
                            c.ChartSchoolType == ChartSchoolType.Academy ||
                            c.ChartSchoolType == ChartSchoolType.Both)
                    .ToList();

            chartList.ForEach(c => c.SubCharts = c.SubCharts?.Where(s => s.ChartSchoolType == ChartSchoolType.Academy || s.ChartSchoolType == ChartSchoolType.Both).ToList());
            return chartList;
        }

        private List<ChartViewModel> BuildChartList()
        {
            return new List<ChartViewModel>()
            {
                //Total Expenditure
                new ChartViewModel()
                {
                    Id = 1,
                    Name = "Total expenditure",
                    FieldName = SchoolTrustFinanceDataFieldNames.TOTAL_EXP,
                    TabType = TabType.Expenditure,
                    ChartGroup = ChartGroupType.TotalExpenditure,
                    ChartSchoolType = ChartSchoolType.Both,
                    ChartType = ChartType.Total,
                    MoreInfo = @""
                },

                new ChartViewModel()
                {
                    Id = 2,
                    Name = "Staff total",
                    FieldName = SchoolTrustFinanceDataFieldNames.STAFF_TOTAL,
                    TabType = TabType.Expenditure,
                    ChartGroup = ChartGroupType.TotalExpenditure,
                    ChartSchoolType = ChartSchoolType.Both,
                    ChartType = ChartType.Total,
                    MoreInfo = @"",
                    DrillInto = ChartGroupType.Staff,
                },

                new ChartViewModel()
                {
                    Id = 8,
                    Name = "Premises total",
                    FieldName = SchoolTrustFinanceDataFieldNames.PREMISES,
                    TabType = TabType.Expenditure,
                    ChartGroup = ChartGroupType.TotalExpenditure,
                    ChartSchoolType = ChartSchoolType.Both,
                    MoreInfo = @"",
                    ChartType = ChartType.Total,
                    DrillInto = ChartGroupType.Premises,
                },

                new ChartViewModel()
                {
                    Id = 13,
                    Name = "Occupation total",
                    FieldName = SchoolTrustFinanceDataFieldNames.OCCUPATION,
                    TabType = TabType.Expenditure,
                    ChartGroup = ChartGroupType.TotalExpenditure,
                    ChartSchoolType = ChartSchoolType.Both,
                    MoreInfo = @"",
                    ChartType = ChartType.Total,
                    DrillInto = ChartGroupType.Occupation,
                    HelpTooltip = Constants.HelpTooltipText.OccupationChartHelp,
                },

                new ChartViewModel()
                {
                    Id = 21,
                    Name = "Supplies and services total",
                    FieldName = SchoolTrustFinanceDataFieldNames.SUPPLIES_SERVICES,
                    TabType = TabType.Expenditure,
                    ChartGroup = ChartGroupType.TotalExpenditure,
                    ChartSchoolType = ChartSchoolType.Both,
                    MoreInfo = @"",
                    ChartType = ChartType.Total,
                    DrillInto = ChartGroupType.SuppliesAndServices,
                },
                
                new ChartViewModel()
                {
                    Id = 51001,
                    Name = "Interest charges for loans and banking",
                    FieldName = SchoolTrustFinanceDataFieldNames.INTEREST_LOANS_BANKING,
                    TabType = TabType.Expenditure, 
                    ChartGroup = ChartGroupType.TotalExpenditure,
                    ChartSchoolType = ChartSchoolType.Both,
                    MoreInfo = @"<p>This includes:</p>
                        <ul>
                        <li>interest paid on overdrafts and other liabilities</li>
                        </ul>
                        <p>It excludes: </p>
                        <ul>
                        <li>interest received</li>
                        </ul>",
                    ChartType = ChartType.Total
                },

                new ChartViewModel()
                {
                    Id = 27,
                    Name = "Cost of finance total",
                    FieldName = SchoolTrustFinanceDataFieldNames.COST_OF_FINANCE,
                    TabType = TabType.Expenditure,
                    ChartGroup = ChartGroupType.TotalExpenditure,
                    ChartSchoolType = ChartSchoolType.Both,
                    MoreInfo = @"",
                    ChartType = ChartType.Total,
                    DrillInto = ChartGroupType.CostOfFinance
                },

                new ChartViewModel()
                {
                    Id = 30,
                    Name = "Community expenditure total",
                    FieldName = SchoolTrustFinanceDataFieldNames.COMMUNITY_EXP,
                    TabType = TabType.Expenditure,
                    ChartGroup = ChartGroupType.TotalExpenditure,
                    ChartSchoolType = ChartSchoolType.Maintained,
                    MoreInfo = @"",
                    ChartType = ChartType.Total,
                    DrillInto = ChartGroupType.Community
                },

                new ChartViewModel()
                {
                    Id = 26,
                    Name = "Special facilities total",
                    FieldName = SchoolTrustFinanceDataFieldNames.SPECIAL_FACILITIES,
                    TabType = TabType.Expenditure,
                    ChartGroup = ChartGroupType.TotalExpenditure,
                    ChartSchoolType = ChartSchoolType.Both,
                    MoreInfo = @"<p>This includes: </p>

                                <ul>
                                <li>swimming pools and sports centres</li>
                                <li>boarding provision</li>
                                <li>rural studies and farm units</li>
                                <li>payments by your school to another school for the benefit of pupils at the other school</li>
                                <li>pupil inter-site travel, eg moving between sites</li>
                                <li>expenses relating to before and after school clubs</li>
                                <li>delegated home to school transport</li>
                                <li>indirect employee expenses and agency staff expenses relating to a special facility</li>
                                <li>purchase of trading items for re-sale, <span aria-label=""for example"">eg</span> school uniforms, books, stationery</li>
                                <li>donations paid by the school to a charity</li>
                                <li>community education with a benefit to the pupils at the school </li>
                                </ul>

                                <p>It excludes: </p>

                                <ul>
                                <li>staff costs associated with managing and supporting the special facility for directly employed staff</li>
                                <li>staff teaching in the special facility</li>
                                <li>school trips</li>
                                <li>residential special schools</li>
                                <li>any community-focused expenditure</li>
                                </ul>",
                    ChartType = ChartType.Total,
                    DrillInto =  ChartGroupType.SpecialFacilities
                },

                //Staff
                new ChartViewModel()
                {
                    Id = 2,
                    Name = "Staff total",
                    FieldName = SchoolTrustFinanceDataFieldNames.STAFF_TOTAL,
                    TabType = TabType.Expenditure,
                    ChartGroup = ChartGroupType.Staff,
                    ChartSchoolType = ChartSchoolType.Both,
                    ChartType = ChartType.Total,
                    MoreInfo = @"",
                },
                new ChartViewModel()
                {
                    Id = 3,
                    Name = "Teaching staff",
                    FieldName = SchoolTrustFinanceDataFieldNames.TEACHING_STAFF,
                    TabType = TabType.Expenditure,
                    ChartGroup = ChartGroupType.Staff,
                    ChartSchoolType = ChartSchoolType.Both,
                    MoreInfo = @"<p>This is expenditure on teaching staff. </p>
                                <p>It includes:</p>
                                <ul>
                                <li>costs for teachers employed directly by the school, including supernumerary/peripatetic teachers on short-term contracts</li>
                                <li>all contracted full-time and part time teachers paid within the scope of the Education Act 2002</li>
                                <li>gross pay, including allowances, maternity pay and the employer's contributions to national insurance and teachers’ pensions </li>
                                <li>teaching and learning responsibilities (TLR) </li>
                                </ul>
                                <p>It excludes:</p>
                                <ul>
                                <li>any teachers employed casually and directly, <span aria-label=""for example"">eg</span> supply teachers</li>
                                <li>any teachers not employed directly by the school, <span aria-label=""for example"">eg</span> agency staff</li>
                                </ul>"
                },
                new ChartViewModel()
                {
                    Id = 4,
                    Name = "Supply staff",
                    FieldName = SchoolTrustFinanceDataFieldNames.SUPPLY_STAFF,
                    TabType = TabType.Expenditure,
                    ChartGroup = ChartGroupType.Staff,
                    ChartSchoolType = ChartSchoolType.Both,
                    MoreInfo = @"<p>This includes:</p>
                                <ul>
                                    <li>supply teaching staff</li>
                                    <li>supply teacher insurance</li>
                                    <li>agency supply teaching staff</li>
                                </ul>",
                    SubCharts = new List<ChartViewModel>()
                    {
                        new ChartViewModel()
                        {
                            Id = 41001,
                            Name = "Supply teaching staff",
                            FieldName = SchoolTrustFinanceDataFieldNames.SUPPLY_TEACHING_STAFF,
                            TabType = TabType.Expenditure,
                            ChartGroup = ChartGroupType.Staff,
                            ChartSchoolType = ChartSchoolType.Both,
                            MoreInfo =
                                @"<p>This is expenditure on salaries and wages for supply teaching staff. It consists of gross pay including allowances, maternity pay and the employer's contributions to national insurance and superannuation.</p>

                                <p>This applies to staff employed directly by the school who are covering absence for: </p>

                                <ul>
                                <li>curriculum release</li>
                                <li>long-term absence</li>
                                <li>sickness absence</li>
                                <li>training absence </li>
                                </ul>

                                <p>It excludes:</p>

                                <ul>
                                <li>supply teachers not employed directly by the school (ie paid via an agency or another third party), regardless of the period of cover</li>
                                </ul>"
                        },
                        new ChartViewModel()
                        {
                            Id = 41002,
                            Name = "Supply teacher insurance",
                            FieldName = SchoolTrustFinanceDataFieldNames.SUPPLY_TEACHER_INSURANCE,
                            TabType = TabType.Expenditure,
                            ChartGroup = ChartGroupType.Staff,
                            ChartSchoolType = ChartSchoolType.Both,
                            MoreInfo = @"<p>This includes:</p>

                                <ul>
                                <li>premiums paid to insurers for supply teacher cover</li>
                                <li>sums de-delegated by the local authority for centrally managed schemes for teaching staff supply cover (long-term sickness, maternity, trade union and public duties) </li>
                                </ul>

                                <p>It excludes:</p>
                                <ul>
                                <li>premiums paid to insurers for cover other than for teacher absence</li>
                                <li>vehicle insurance</li>
                                <li>accident and public liability insurance for people not employed directly by the school</li>
                                <li>school trip insurance</li>
                                <li>premises related insurance</li>
                                <li>non-teaching cover supervisors</li>
                                </ul>"
                        },
                        new ChartViewModel()
                        {
                            Id = 41003,
                            Name = "Agency supply teaching staff",
                            FieldName = SchoolTrustFinanceDataFieldNames.AGENCY_TEACH_STAFF,
                            TabType = TabType.Expenditure,
                            ChartGroup = ChartGroupType.Staff,
                            ChartSchoolType = ChartSchoolType.Both,
                            MoreInfo = @"
                                <p>This is the money paid to an agency for teaching staff that have been brought in to cover teacher absence. <p/>
                                <p>This includes:</p>

                                <ul>
                                <li>cover of any period and for all reasons including illness, absence for training, and any leave</li>
                                </ul>

                                <p>This excludes:</p>

                                <ul>
                                <li>supply teachers employed directly by the school</li>
                                </ul>"
                        }
                    }
                },
                new ChartViewModel()
                {
                    Id = 5,
                    Name = "Education support staff",
                    FieldName = SchoolTrustFinanceDataFieldNames.EDUCATION_SUPPORT_STAFF,
                    TabType = TabType.Expenditure,
                    ChartGroup = ChartGroupType.Staff,
                    ChartSchoolType = ChartSchoolType.Both,
                    MoreInfo =
                        @"<p>This covers expenditure on the salaries of permanent support staff employed directly by the school. It takes account of allowances, maternity pay and the employer's contributions to national insurance and superannuation.</p>

                                <p>It includes:</p>

                                <ul>
                                <li>childcare staff</li>
                                <li>classroom assistants/learning support assistants</li>
                                <li>examination invigilators and examination officers</li>
                                <li>foreign language assistants</li>
                                <li>librarians</li>
                                <li>nursery assistants</li>
                                <li>pianists</li>
                                <li>residential childcare officers at a residential special school</li>
                                <li>supply education support staff</li>
                                <li>workshop, technology and science technicians</li>
                                <li>educational welfare officers</li>
                                <li>cover supervisors</li>
                                <li>staff employed to follow up attendance issues </li>
                                </ul>


                                <p>It excludes:</p>

                                <ul>
                                <li>education support staff not employed directly by the school</li>
                                </ul>"
                },
                new ChartViewModel()
                {
                    Id = 6,
                    Name = "Administrative and clerical staff",
                    FieldName = SchoolTrustFinanceDataFieldNames.ADMIN_CLERIC_STAFF,
                    TabType = TabType.Expenditure,
                    ChartGroup = ChartGroupType.Staff,
                    ChartSchoolType = ChartSchoolType.Both,
                    MoreInfo =
                        @"<p>This covers expenditure on salaries and wages of administrative and clerical staff employed directly by the school. It consists of gross pay, including allowances, maternity pay and the employer's contributions to national insurance and superannuation.</p>

                                <p>It includes:</p>

                                <ul>
                                <li>business managers and bursars</li>
                                <li>clerk to the governing body</li>
                                <li>receptionists</li>
                                <li>school secretaries</li>
                                <li>other administrative staff</li>
                                <li>telephonists</li>
                                <li>typists </li>
                                <li>IT Manager </li>
                                </ul>

                                <p>It excludes: </p>

                                <ul>
                                <li>administrative and clerical staff not employed directly by the school</li>
                                <li>administrative and clerical staff employed to manage and support the school's special facilities</li>
                                <li>IT teachers, even where they have responsibility for managing IT systems within the school</li>
                                </ul>"
                },
                new ChartViewModel()
                {
                    Id = 7,
                    Name = "Other staff costs",
                    FieldName = SchoolTrustFinanceDataFieldNames.OTHER_STAFF_COSTS,
                    TabType = TabType.Expenditure,
                    ChartGroup = ChartGroupType.Staff,
                    ChartSchoolType = ChartSchoolType.Both,
                    MoreInfo = @"<p>This includes:</p>
                                <ul>
                                <li>cost of other staff</li>
                                <li>indirect employee expenses</li>
                                <li>staff development and training</li>
                                <li>staff-related insurance</li>
                                </ul>",
                    SubCharts = new List<ChartViewModel>()
                    {
                        new ChartViewModel()
                        {
                            Id = 71001,
                            Name = "Other staff",
                            FieldName = SchoolTrustFinanceDataFieldNames.OTHER_STAFF,
                            TabType = TabType.Expenditure,
                            ChartGroup = ChartGroupType.Staff,
                            ChartSchoolType = ChartSchoolType.Both,
                            MoreInfo =
                                @"<p>This is expenditure on salaries and wages of other staff employed directly by the school. It consists of gross pay, including bonus and allowances, maternity pay and the employer's contributions to national insurance and superannuation.</p>

                                <p>It includes:</p>

                                <ul>
                                <li>mealtime assistants and midday supervisors</li>
                                <li>boarding staff of a residential school e.g. laundry assistants and night time social workers</li>
                                <li>escorts (for pupils with medical or special educational needs)</li>
                                <li>liaison officers</li>
                                <li>staff employed to manage and support pupil-focused special facilities available at the school</li>
                                <li>staff supervising students during clubs and breaks</li>
                                <li>supply cost of other staff</li>
                                <li>youth workers</li>
                                <li>nurses and medical staff </li>
                                </ul>

                                <p>It excludes:</p>

                                <ul>
                                <li>the cost of other staff not employed directly by the school</li>
                                </ul>"
                        },
                        new ChartViewModel()
                        {
                            Id = 71002,
                            Name = "Indirect employee expenses",
                            FieldName = SchoolTrustFinanceDataFieldNames.INDIRECT_EMPLOYEE_EXPENSES,
                            ChartSchoolType = ChartSchoolType.Both,
                            TabType = TabType.Expenditure,
                            ChartGroup = ChartGroupType.Staff,
                            MoreInfo = @"<p>This includes: </p>

                                <ul>
                                <li>recruitment costs, e.g. advertising, interviews, relocation expenses</li>
                                <li>employee travel and subsistence</li>
                                <li>duty meals</li>
                                <li>pensions payments including any premature retirement payments made by the school and pension deficit payments, where these are paid separately from pension contributions </li>
                                <li>lump sum compensation and redundancy payments and medical fees</li>
                                <li>car leasing expenditure where the cars are for staff use</li>
                                <li>teacher inter-site travel costs</li>
                                <li>childcare vouchers</li>
                                <li>payments to site service officers (caretakers, school keepers) for expenses such as house gas, rates, council taxes, electricity and telephone rental</li>
                                <li>car parking fees </li>
                                </ul>

                                <p>This excludes: </p>

                                <ul>
                                <li>salary costs</li>
                                <li>any cost for persons not employed directly by the school</li>
                                </ul>"
                        },
                        new ChartViewModel()
                        {
                            Id = 71003,
                            Name = "Staff development and training",
                            FieldName = SchoolTrustFinanceDataFieldNames.STAFF_DEV,
                            TabType = TabType.Expenditure,
                            ChartGroup = ChartGroupType.Staff,
                            ChartSchoolType = ChartSchoolType.Both,
                            MoreInfo = @"<p>This includes:</p>

                                <ul>
                                <li>development and training costs for all staff (directly and not directly employed) at the school</li>
                                <li>the cost of all in-service training courses and other development opportunities</li>
                                <li>cost of equipment and resources to provide in-service training </li>
                                </ul>

                                <p>It excludes:</p>
                                <ul>
                                <li>the cost of supply staff used to cover teacher absence</li>
                                </ul>"
                        },
                        new ChartViewModel()
                        {
                            Id = 71004,
                            Name = "Staff related insurance",
                            FieldName = SchoolTrustFinanceDataFieldNames.STAFF_INSURANCE,
                            ChartSchoolType = ChartSchoolType.Both,
                            TabType = TabType.Expenditure,
                            ChartGroup = ChartGroupType.Staff,
                            MoreInfo = @"<p>This includes:</p>

                                <ul>
                                <li>cover for non-teaching staff absence including unqualified cover supervisors</li>
                                <li>employee related insurance for accident and liability, assault, fidelity guarantee, libel and slander</li>
                                <li>sums de-delegated by the local authority for centrally managed schemes for nonteaching staff supply cover (long-term sickness, maternity, trade union and public duties) </li>
                                </ul>                    
                                <p>It excludes:</p>
                                <ul>
                                <li>insurance premiums paid to cover teaching absence for staff directly employed by the school</li>
                                <li>premises related insurance</li>
                                <li>vehicle insurance</li>
                                <li>accident and public liability insurance for persons not employed directly by the school</li>
                                <li>school trip insurance</li>
                                </ul>"
                        }
                    }
                },

                //Premises
                new ChartViewModel()
                {
                    Id = 8,
                    Name = "Premises total",
                    FieldName = SchoolTrustFinanceDataFieldNames.PREMISES,
                    TabType = TabType.Expenditure,
                    ChartGroup = ChartGroupType.Premises,
                    ChartSchoolType = ChartSchoolType.Both,
                    MoreInfo = @"",
                    ChartType = ChartType.Total,
                },
                new ChartViewModel()
                {
                    Id = 9,
                    Name = "Premises staff",
                    FieldName = SchoolTrustFinanceDataFieldNames.PREMISES_STAFF,
                    TabType = TabType.Expenditure,
                    ChartGroup = ChartGroupType.Premises,
                    ChartSchoolType = ChartSchoolType.Both,
                    MoreInfo = @"
                                <p>This covers expenditure on salaries and wages of premises staff employed directly by the school. It consists of gross pay including allowances, maternity pay and the employer's contributions to national insurance and superannuation.</p>

                                <p>It includes:</p>

                                <ul>
                                <li>caretakers</li>
                                <li>cleaners</li>
                                <li>grounds staff</li>
                                <li>maintenance staff</li>
                                <li>porters</li>
                                <li>messengers</li>
                                <li>security staff </li>
                                </ul>

                                <p>It excludes:</p>

                                <ul>
                                <li>premises staff not employed directly by the school</li>
                                <li>premises staff employed to manage and support the school's special facilities</li>
                                </ul>"
                },
                new ChartViewModel()
                {
                    Id = 10,
                    Name = "Cleaning and caretaking",
                    FieldName = SchoolTrustFinanceDataFieldNames.CLEANING,
                    TabType = TabType.Expenditure,
                    ChartGroup = ChartGroupType.Premises,
                    ChartSchoolType = ChartSchoolType.Both,
                    MoreInfo = @"<p>This includes: </p>
                                <ul>
                                <li>supplies used in cleaning and caretaking</li>
                                <li>the cost of equipment such as floor polishers, vacuum cleaners and other hardware</li>
                                <li>charges by contractors for providing a cleaning service</li>
                                <li>charges by contractors for providing a caretaking service</li>
                                <li>related professional and technical services</li>
                                </ul>

                                <p>It excludes:</p>
                                <ul>
                                <li>the cost of staff where they are directly employed by the school</li>
                                <ul>"
                },
                new ChartViewModel()
                {
                    Id = 11,
                    Name = "Maintenance and improvement",
                    FieldName = SchoolTrustFinanceDataFieldNames.MAINTENANCE_IMPROVEMENT,
                    TabType = TabType.Expenditure,
                    ChartGroup = ChartGroupType.Premises,
                    ChartSchoolType = ChartSchoolType.Both,
                    MoreInfo = @"<p>This includes:</p>
                                <ul>
                                <li>building maintenance and improvement</li>
                                <li>grounds maintenance and improvement</li>
                                </ul>",
                    SubCharts = new List<ChartViewModel>()
                    {
                        new ChartViewModel
                        {
                            Id = 11001,
                            Name = "Grounds maintenance and improvement",
                            FieldName = SchoolTrustFinanceDataFieldNames.GROUNDS_MAINTENANCE_IMPROVEMENT,
                            TabType = TabType.Expenditure,
                            ChartGroup = ChartGroupType.Premises,
                            ChartSchoolType = ChartSchoolType.Maintained,
                            MoreInfo = @"<p>This includes:</p>
                                <ul>
                                <li>maintenance and improvement on gardens and grounds, including car parking, play areas, playground equipment, sports fields and pitches on the school campus</li>
                                <li>related professional and technical services, including labour costs where supplied as part of the contract/service </li>
                                </ul>

                                <p>It excludes: </p>

                                <ul>
                                <li>staff directly employed by the school</li>
                                <li>the cost of improvements above the de minimis level</li>
                                <li>the cost of maintenance and improvement of special facilities or community-focused facilities </li>
                                </ul>"
                        },
                        new ChartViewModel
                        {
                            Id = 11002,
                            Name = "Building maintenance and improvement",
                            FieldName = SchoolTrustFinanceDataFieldNames.BUILDING_MAINTENANCE_IMPROVEMENT,
                            TabType = TabType.Expenditure,
                            ChartGroup = ChartGroupType.Premises,
                            ChartSchoolType = ChartSchoolType.Maintained,
                            MoreInfo = @"<p>This includes:</p>

                                <ul>
                                <li>charges by contractors for repairs, maintenance and improvements</li>
                                <li>related professional and technical services, including labour costs where supplied as part of the contract</li>
                                <li>costs of materials and equipment used by directly employed staff for internal and external repair, maintenance and improvement to buildings </li>
                                <li>fixtures and fittings, <span aria-label=""for example"">eg</span> carpets and curtains </li>

                                </ul>

                                <p>It excludes: </p>

                                <ul>
                                <li>the cost of premises staff who are directly employed by the school</li>
                                <li>the cost of improvements above the school/local authority de minimis level</li>
                                <li>the cost of maintenance and improvement of special facilities or community-focused facilities</li>
                                </ul>"
                        },
                    }
                },
                new ChartViewModel()
                {
                    Id = 12,
                    Name = "PFI charges",
                    FieldName = SchoolTrustFinanceDataFieldNames.PFI_CHARGES,
                    TabType = TabType.Expenditure,
                    ChartGroup = ChartGroupType.Premises,
                    ChartSchoolType = ChartSchoolType.Both,
                    MoreInfo = @"Prior to 2020-2021 PFI charges for Maintained schools were included within Bought in professional services under Supplies and Services"
                },

                //Occupation
                new ChartViewModel()
                {
                    Id = 13,
                    Name = "Occupation total",
                    FieldName = SchoolTrustFinanceDataFieldNames.OCCUPATION,
                    TabType = TabType.Expenditure,
                    ChartGroup = ChartGroupType.Occupation,
                    ChartSchoolType = ChartSchoolType.Both,
                    MoreInfo = @"",
                    ChartType = ChartType.Total,
                    HelpTooltip = Constants.HelpTooltipText.OccupationChartHelp,
                },
                new ChartViewModel()
                {
                    Id = 14,
                    Name = "Energy",
                    FieldName = SchoolTrustFinanceDataFieldNames.ENERGY,
                    TabType = TabType.Expenditure,
                    ChartGroup = ChartGroupType.Occupation,
                    ChartSchoolType = ChartSchoolType.Both,
                    MoreInfo = @"<p>This includes:</p>

                                <ul>
                                <li>all costs related to fuel and energy, including fuel oil, solid fuel, electricity and gas</li>
                                <li>repayment of SALIX loans (ie repayment for funding for energy efficiency projects)
                                </ul>

                                <p>It excludes:</p>

                                <ul>
                                <li>costs arising from repairs or maintenance to energy supplies</li>
                                <ul>"
                },
                new ChartViewModel()
                {
                    Id = 15,
                    Name = "Water and sewerage",
                    FieldName = SchoolTrustFinanceDataFieldNames.WATER_SEWERAGE,
                    TabType = TabType.Expenditure,
                    ChartGroup = ChartGroupType.Occupation,
                    ChartSchoolType = ChartSchoolType.Both,
                    MoreInfo = @"<p>This includes:</p>
                                <ul>
                                <li>all costs related to water and sewerage</li>
                                <li>the emptying of septic tanks</li>
                                </ul>
                                <p>It excludes:</p>
                                <ul>
                                <li>any costs arising from repairs or maintenance to water or sewerage systems</li>
                                </ul>"
                },
                new ChartViewModel()
                {
                    Id = 16,
                    Name = "Rates",
                    FieldName = SchoolTrustFinanceDataFieldNames.RATES,
                    TabType = TabType.Expenditure,
                    ChartGroup = ChartGroupType.Occupation,
                    ChartSchoolType = ChartSchoolType.Maintained,
                    MoreInfo = @"
                                <p>This includes:</p>

                                <ul>
                                <li>non-domestic rates expenditure (NNDR)</li>
                                </ul>

                                <p>This is separate from other occupation costs because it is imposed and therefore not a controllable expense. Unlike other items where there will be some element of control, it is a difficult area to benchmark.</p>"
                },
                new ChartViewModel()
                {
                    Id = 17,
                    Name = "Other occupation costs",
                    FieldName = SchoolTrustFinanceDataFieldNames.OTHER_OCCUPATION,
                    TabType = TabType.Expenditure,
                    ChartGroup = ChartGroupType.Occupation,
                    ChartSchoolType = ChartSchoolType.Both,
                    MoreInfo = @"<p>This includes:</p>

                                <ul>
                                <li>rents, lease or hire charges for premises
                                <li>refuse collection
                                <li>hygiene services, e.g. paper towels, toilet rolls, hand driers
                                <li>security patrols and services
                                <li>CCTV/burglar alarm maintenance contracts 
                                <li>landlord's service charges
                                <li>health and safety costs, including fire-fighting equipment
                                <li>electrical testing and pest control 
                                </ul>

                                <p>This excludes:</p>
                                <ul>
                                <li>costs for staff where they are directly employed by the school
                                <li>emptying the septic tanks 
                                </ul>"
                },
                new ChartViewModel()
                {
                    Id = 18,
                    Name = "Other insurance premiums",
                    FieldName = SchoolTrustFinanceDataFieldNames.OTHER_INSURANCE,
                    TabType = TabType.Expenditure,
                    ChartGroup = ChartGroupType.Occupation,
                    ChartSchoolType = ChartSchoolType.Both,
                    MoreInfo = @"<p>This includes: </p>
                                <ul>
                                <li>sums de-delegated by the local authority for centrally managed insurance schemes</li>
                                <li>premises related insurance</li>
                                <li>vehicle insurance</li>
                                <li>accident and public liability insurance for people not employed directly by the school</li>
                                <li>school trip insurance</li>
                                <li>sums de-delegated by the local authority for contingencies </li>
                                </ul>

                                <p>This excludes: </p>

                                <ul>
                                <li>insurance for supply teacher cover </li>
                                <li>other staff insurance cover</li>
                                </ul>"
                },
                new ChartViewModel()
                {
                    Id = 19,
                    Name = "Catering expenditure",
                    FieldName = SchoolTrustFinanceDataFieldNames.CATERING_EXP,
                    TabType = TabType.Expenditure,
                    ChartGroup = ChartGroupType.Occupation,
                    ChartSchoolType = ChartSchoolType.Both,
                    MoreInfo = @"",
                    SubCharts = new List<ChartViewModel>()
                    {
                        new ChartViewModel()
                        {
                            Id = 19001,
                            Name = "Catering staff",
                            FieldName = SchoolTrustFinanceDataFieldNames.CATERING_STAFF,
                            TabType = TabType.Expenditure,
                            ChartGroup = ChartGroupType.Occupation,
                            ChartSchoolType = ChartSchoolType.Both,
                            MoreInfo =
                                @"<p>This covers expenditure on salaries and wages of catering staff employed directly by the school. It consists of gross pay including allowances, maternity pay and the employer's contributions to national insurance and superannuation.</p>
                                <p>It includes:</p>
                                <ul>
                                <li>cashiers</li>
                                <li>chefs and cooks</li>
                                <li>kitchen porters</li>
                                <li>servers</li>
                                <li>snack bar staff </li>
                                </ul>
                                <p>It excludes: </p>
                                <ul>
                                <li>catering staff not employed directly by the school</li>
                                <li>catering staff employed to manage and support the school's special facilities </li>
                                </ul>"
                        },
                        new ChartViewModel()
                        {
                            Id = 19002,
                            Name = "Catering supplies",
                            FieldName = SchoolTrustFinanceDataFieldNames.CATERING_SUPPLIES,
                            TabType = TabType.Expenditure,
                            ChartGroup = ChartGroupType.Occupation,
                            ChartSchoolType = ChartSchoolType.Both,
                            MoreInfo = @"
                            <p>This includes:</p>

                            <ul>
                            <li>non-capital catering equipment</li>
                            <li>provisions</li>
                            <li>other supplies used in catering, e.g. cleaning materials, protective clothing</li>
                            <li>purchase, rent, lease or hire of catering vending machines</li>
                            <li>full cost of service contract</li>
                            <li>related professional and technical services</li>
                            <li>repairs and maintenance of kitchen equipment, including safety checks</li>
                            <li>cost of providing free school meals and milk</li>

                            </ul>

                            <p>It excludes: </p>

                            <ul>
                            <li>the cost of staff where they are directly employed by the school</li>
                            <li>the cost of any kitchen or catering equipment above the de minimis level </li>
                            </ul>"
                        },
                    }
                },
                new ChartViewModel()
                {
                    Id = 20,
                    Name = "Rent and rates",
                    FieldName = SchoolTrustFinanceDataFieldNames.RENT_RATES,
                    TabType = TabType.Expenditure,
                    ChartGroup = ChartGroupType.Occupation,
                    ChartSchoolType = ChartSchoolType.Academy,
                    MoreInfo = @"<p>This includes the amount incurred for:</p>
                                <ul>
                                <li>national non-domestic rates (NNDR)</li>
                                <li>business rates</li>
                                <li>operating leases/rental of buildings</li>
                                </ul>
                                "
                },

                //Supplies and Services
                new ChartViewModel()
                {
                    Id = 21,
                    Name = "Supplies and services total",
                    FieldName = SchoolTrustFinanceDataFieldNames.SUPPLIES_SERVICES,
                    TabType = TabType.Expenditure,
                    ChartGroup = ChartGroupType.SuppliesAndServices,
                    ChartSchoolType = ChartSchoolType.Both,
                    MoreInfo = @"",
                    ChartType = ChartType.Total,
                },
                new ChartViewModel()
                {
                    Id = 22,
                    Name = "Administrative supplies",
                    FieldName = SchoolTrustFinanceDataFieldNames.ADMIN_SUPPLIES,
                    TabType = TabType.Expenditure,
                    ChartGroup = ChartGroupType.SuppliesAndServices,
                    ChartSchoolType = ChartSchoolType.Both,
                    MoreInfo = @"<p>This includes:</p>

                                <ul>
                                <li>administrative stationery</li>
                                <li>administrative printing</li>
                                <li>administrative reprographics</li>
                                <li>postage</li>
                                <li>bank charges</li>
                                <li>advertising (but not for recruitment)</li>
                                <li>telephone charges (not dedicated internet lines)</li>
                                <li>medical and domestic supplies</li>
                                <li>purchase, hire or maintenance contracts of ICT or other equipment not to be used for teaching</li>
                                <li>purchase, hire, lease and maintenance of furniture and equipment not used for teaching</li>
                                <li>subscriptions, publications, periodicals and copyright fees not related to the curriculum</li>
                                <li>school publications <span aria-label=""for example"">eg</span> parents' report and school brochure</li>
                                <li>any governors’ expenses </li>
                                <li>marketing costs for school prospectuses </li>
                                </ul>

                                <p>This excludes: </p>

                                <ul>
                                <li>any costs directly attributable to the curriculum</li>
                                <li>material costs directly attributable to another specific service grouping</li>
                                </ul>"
                },
                new ChartViewModel()
                {
                    Id = 23,
                    Name = "Educational supplies",
                    FieldName = SchoolTrustFinanceDataFieldNames.EDUCATIONAL_SUPPLIES,
                    TabType = TabType.Expenditure,
                    ChartGroup = ChartGroupType.SuppliesAndServices,
                    ChartSchoolType = ChartSchoolType.Both,
                    MoreInfo = @"<p>This includes:</p>
                                <ul>
                                <li>learning resources (not ICT equipment)</li>
                                <li>ICT learning resources</li>
                                <li>examination fees</li>
                                </ul>",
                    SubCharts = new List<ChartViewModel>()
                    {
                        new ChartViewModel
                        {
                            Id = 23001,
                            Name = "ICT learning resources",
                            FieldName = SchoolTrustFinanceDataFieldNames.ICT_LEARNING_RESOURCES,
                            TabType = TabType.Expenditure,
                            ChartGroup = ChartGroupType.SuppliesAndServices,
                            ChartSchoolType = ChartSchoolType.Both,
                            MoreInfo = @"<p>This includes:</p>

                                <ul>
                                <li>educational software including site or other licences</li>
                                <li>hardware including keyboards, monitors and printers</li>
                                <li>purchase, lease, hire or maintenance contracts of ICT used for teaching</li>
                                <li>costs of broadband, ISDN, ASDL or other dedicated phone lines </li>
                                <li>the costs of test and examination entry fees and any accreditation costs related to pupils. This includes GCSEs, A/AS levels and the European Baccalaureate</li>
                                <li>administrative costs, <span aria-label=""for example"">eg</span> external marking </li>
                                </ul>

                                <p>This excludes:</p>

                                <ul>
                                <li>resources that are used for administrative purposes </li>
                                <li>ICT expenditure that is over the de minimis level </li>
                                <li>the cost of examination resources, like test papers themselves</li>
                                </ul>"
                        },
                        new ChartViewModel
                        {
                            Id = 23002,
                            Name = "Learning resources",
                            FieldName = SchoolTrustFinanceDataFieldNames.LEARNING_RESOURCES,
                            TabType = TabType.Expenditure,
                            ChartGroup = ChartGroupType.SuppliesAndServices,
                            ChartSchoolType = ChartSchoolType.Both,
                            MoreInfo = @"<p>This includes:</p>

                                <ul>
                                <li>achievement gifts and prizes awarded to pupils</li>
                                <li>books (library and text books)</li>
                                <li>charges for the school library</li>
                                <li>classroom and learning equipment (excluding ICT equipment)</li>
                                <li>curriculum transport, including minibus expenses</li>
                                <li>furniture used for teaching purposes</li>
                                <li>pupil travel for work experience placements</li>
                                <li>purchase, lease, hire or maintenance contracts of audio-visual or other equipment</li>
                                <li>reprographic resources and equipment used specifically for teaching purposes</li>
                                <li>school trips and educational visits</li>
                                <li>servicing and repairs to musical instruments and PE equipment </li>
                                <li>subscriptions, publications, periodicals and copyright fees</li>
                                <li>teaching materials</li>
                                <li>television licence fees used for teaching purposes</li>
                                <li>payments to alternative provision services including pupil referral units, non-maintained special schools (NMSS) and independent schools</li>
                                <li>primary school PIP examination costs</li>
                                <li>payments made to students who get the 16-19 Bursary Fund </li>
                                </ul>

                                <p>This excludes:</p>

                                <ul>
                                <li>curriculum ICT costs</li>
                                <li>resources that are used for administrative purposes </li>
                                </ul>"
                        },
                        new ChartViewModel
                        {
                            Id = 23003,
                            Name = "Examination fees",
                            FieldName = SchoolTrustFinanceDataFieldNames.EXAM_FEES,
                            TabType = TabType.Expenditure,
                            ChartGroup = ChartGroupType.SuppliesAndServices,
                            ChartSchoolType = ChartSchoolType.Both,
                            MoreInfo = @"<p>This includes:</p>

                                <ul>
                                <li>the costs of test and examination entry fees and any accreditation costs related to pupils. This includes GCSEs, A/AS levels and the European Baccalaureate</li>
                                <li>administrative costs, <span aria-label=""for example"">eg</span> external marking </li>
                                </ul>

                                <p>This excludes:</p>

                                <ul>
                                <li>the cost of examination resources, like test papers themselves</li>
                                </ul>"
                        },
                    }
                },
                new ChartViewModel()
                {
                    Id = 24,
                    Name = "Bought-in professional services",
                    FieldName = SchoolTrustFinanceDataFieldNames.BROUGHT_IN_SERVICES,
                    TabType = TabType.Expenditure,
                    ChartGroup = ChartGroupType.SuppliesAndServices,
                    ChartSchoolType = ChartSchoolType.Both,
                    MoreInfo = @"<p>This includes:</p>
                                <ul>
                                <li>educational consultancy</li>
                                <li>bought-in professional services</li>
                                <li>legal and professional</li>
                                <li>auditor costs</li>
                                </ul>",
                    SubCharts = new List<ChartViewModel>()
                    {
                        new ChartViewModel
                        {
                            Id = 24001,
                            Name = "Educational consultancy",
                            FieldName = SchoolTrustFinanceDataFieldNames.EDUCATIONAL_CONSULTANCY,
                            TabType = TabType.Expenditure,
                            ChartGroup = ChartGroupType.SuppliesAndServices,
                            ChartSchoolType = ChartSchoolType.Both,
                            MoreInfo = @"<p>This covers professional consultancy and advice to staff and governors.</p>"
                        },
                        new ChartViewModel
                        {
                            Id = 24005,
                            Name = "Bought in Professional services, other, not PFI",
                            FieldName = SchoolTrustFinanceDataFieldNames.BOUGHT_IN_NOT_PFI,
                            TabType = TabType.Expenditure,
                            ChartGroup = ChartGroupType.SuppliesAndServices,
                            ChartSchoolType = ChartSchoolType.Maintained,
                            MoreInfo =
                                @"<p>This covers professional services, consultancy and advice to staff and governors purchased from the local authority or an external party. </p>

                            <p>This includes:</p>

                            <ul>
                            <li>management</li>
                            <li>finance</li>
                            <li>legal</li>
                            <li>personnel</li>
                            <li>premises</li>
                            <li>clerking service, if a clerk is not directly employed by the school</li>
                            <li>management fee on PPP contracts</li>
                            <li>free school meals (FSM) eligibility checking, including sums de-delegated by the local authority</li>
                            <li>any security personnel employed to bank revenue funding </li>
                            </ul>

                            <p>It excludes: </p>

                            <ul>
                            <li>cost of staff where they are directly employed by the school</li>
                            <li>consultancy and advice for curriculum</li>
                            <li>PFI charges</li>
                            </ul>"
                        },
                        new ChartViewModel
                        {
                            Id = 24002,
                            Name = "Bought in professional services - other",
                            FieldName = SchoolTrustFinanceDataFieldNames.BOUGHT_IN_OTHER,
                            TabType = TabType.Expenditure,
                            ChartGroup = ChartGroupType.SuppliesAndServices,
                            ChartSchoolType = ChartSchoolType.Maintained,
                            MoreInfo =
                                @"<p>This covers professional services, consultancy and advice to staff and governors purchased from the local authority or an external party. </p>

                            <p>This includes:</p>

                            <ul>
                            <li>management</li>
                            <li>finance</li>
                            <li>legal</li>
                            <li>personnel</li>
                            <li>premises</li>
                            <li>clerking service, if a clerk is not directly employed by the school</li>
                            <li>management fee on PPP contracts</li>
                            <li>free school meals (FSM) eligibility checking, including sums de-delegated by the local authority</li>
                            <li>any security personnel employed to bank revenue funding </li>
                            </ul>

                            <p>It excludes: </p>

                            <ul>
                            <li>cost of staff where they are directly employed by the school</li>
                            <li>consultancy and advice for curriculum</li>
                            </ul>"
                        },
                        new ChartViewModel
                        {
                            Id = 24003,
                            Name = "Legal and professional",
                            FieldName = SchoolTrustFinanceDataFieldNames.LEGAL_PROFESSIONAL,
                            TabType = TabType.Expenditure,
                            ChartGroup = ChartGroupType.SuppliesAndServices,
                            ChartSchoolType = ChartSchoolType.Academy,
                            MoreInfo = @"
                                        <p>This covers professional services, consultancy and advice to staff and governors purchased from the local authority or an external party. </p>
                                        <p>It includes:</p>
                                        <ul>
                                        <li>management</li>
                                        <li>finance</li>
                                        <li>legal</li>
                                        <li>personnel</li>
                                        <li>premises</li>
                                        <li>clerking service, if a clerk is not directly employed by the school</li>
                                        <li>management fee on PPP contracts</li>
                                        <li>free school meals (FSM) eligibility checking, including sums de-delegated by the local authority</li>
                                        <li>any security personnel employed to bank revenue funding</li>
                                        </ul>
                                        <p>It excludes:
                                        <ul>
                                        <li>cost of staff where they are directly employed by the school</li>
                                        <li>consultancy and advice for curriculum</li>
                                        </ul>
                                        "
                        },
                        new ChartViewModel
                        {
                            Id = 24004,
                            Name = "Auditor costs",
                            FieldName = SchoolTrustFinanceDataFieldNames.AUDITOR_COSTS,
                            TabType = TabType.Expenditure,
                            ChartGroup = ChartGroupType.SuppliesAndServices,
                            ChartSchoolType = ChartSchoolType.Academy,
                            MoreInfo = @"<p>This covers expenditure for any audit costs. </p>"
                        },
                    }
                },

                //Special Facilities
                new ChartViewModel()
                {
                    Id = 26,
                    Name = "Special facilities total",
                    FieldName = SchoolTrustFinanceDataFieldNames.SPECIAL_FACILITIES,
                    TabType = TabType.Expenditure,
                    ChartGroup = ChartGroupType.SpecialFacilities,
                    ChartSchoolType = ChartSchoolType.Both,
                    MoreInfo = @"<p>This includes: </p>

                                <ul>
                                <li>swimming pools and sports centres</li>
                                <li>boarding provision</li>
                                <li>rural studies and farm units</li>
                                <li>payments by your school to another school for the benefit of pupils at the other school</li>
                                <li>pupil inter-site travel, <span aria-label=""for example"">eg</span> moving between sites</li>
                                <li>expenses relating to before and after school clubs</li>
                                <li>delegated home to school transport</li>
                                <li>indirect employee expenses and agency staff expenses relating to a special facility</li>
                                <li>purchase of trading items for re-sale, <span aria-label=""for example"">eg</span> school uniforms, books, stationery</li>
                                <li>donations paid by the school to a charity</li>
                                <li>community education with a benefit to the pupils at the school </li>
                                </ul>

                                <p>It excludes: </p>

                                <ul>
                                <li>staff costs associated with managing and supporting the special facility for directly employed staff</li>
                                <li>staff teaching in the special facility</li>
                                <li>school trips</li>
                                <li>residential special schools</li>
                                <li>any community-focused expenditure</li>
                                </ul>",
                    ChartType = ChartType.Total
                },

                //Cost of Finance
                new ChartViewModel()
                {
                    Id = 27,
                    Name = "Cost of finance total",
                    FieldName = SchoolTrustFinanceDataFieldNames.COST_OF_FINANCE,
                    TabType = TabType.Expenditure,
                    ChartGroup = ChartGroupType.CostOfFinance,
                    ChartSchoolType = ChartSchoolType.Both,
                    MoreInfo = @"",
                    ChartType = ChartType.Total
                },
                new ChartViewModel
                {
                    Id = 28,
                    Name = "Loan interest",
                    FieldName = SchoolTrustFinanceDataFieldNames.INTEREST_CHARGES,
                    TabType = TabType.Expenditure,
                    ChartGroup = ChartGroupType.CostOfFinance,
                    ChartSchoolType = ChartSchoolType.Both,
                    MoreInfo = @"<p>This includes:</p>

                                <ul>
                                <li>interest paid on overdrafts and other liabilities</li>
                                </ul>

                                <p>It excludes:</p>
                                <ul>
                                <li>interest received</li>
                                </ul>"
                },
                new ChartViewModel
                {
                    Id = 29,
                    Name = "Direct revenue financing (revenue contributions to capital)",
                    FieldName = SchoolTrustFinanceDataFieldNames.DIRECT_REVENUE,
                    TabType = TabType.Expenditure,
                    ChartGroup = ChartGroupType.CostOfFinance,
                    ChartSchoolType = ChartSchoolType.Both,
                    MoreInfo = @"<p>This includes:</p>

                        <ul>
                        <li>all amounts transferred to CI04 to be accumulated to fund capital works. This may include receipts from insurance claims for capital losses received into income under I11 </li>
                        <li>any amount transferred to a local authority reserve to part fund a capital scheme which is being delivered by the local authority</li>
                        <li>any repayment of principal on a capital loan from the local authority</li>
                        </ul>

                        <p>It excludes: </p>

                        <ul>
                        <li>funds specifically provided for capital purposes</li>
                        </ul>"
                },

                //Community
                new ChartViewModel()
                {
                    Id = 30,
                    Name = "Community expenditure total",
                    FieldName = SchoolTrustFinanceDataFieldNames.COMMUNITY_EXP,
                    TabType = TabType.Expenditure,
                    ChartGroup = ChartGroupType.Community,
                    ChartSchoolType = ChartSchoolType.Maintained,
                    ChartType = ChartType.Total,
                    MoreInfo = @"",
                },
                new ChartViewModel
                {
                    Id = 31,
                    Name = "Community focused school staff",
                    FieldName = SchoolTrustFinanceDataFieldNames.COMM_FOCUSED_STAFF,
                    TabType = TabType.Expenditure,
                    ChartGroup = ChartGroupType.Community,
                    ChartSchoolType = ChartSchoolType.Maintained,
                    MoreInfo =
                        @"<p>This is expenditure on salaries and wages of staff employed directly by the school for community purposes. It consists of gross pay including allowances, maternity pay and the employer's contributions to national insurance and superannuation. </p>

                        <p>It includes:</p>
                        <ul>
                        <li>the cost of all staff employed directly by the school for community-focused activities</li>
                        <li>adult education tutors, where the school manages an adult education programme </li>
                        </ul>

                        <p>It excludes: </p>

                        <ul>
                        <li>the cost of school staff who are not employed directly by the school for community-focused activities</li>
                        </ul>"
                },
                new ChartViewModel
                {
                    Id = 32,
                    Name = "Community focused school costs",
                    FieldName = SchoolTrustFinanceDataFieldNames.COMM_FOCUSED_SCHOOL,
                    ChartSchoolType = ChartSchoolType.Maintained,
                    TabType = TabType.Expenditure,
                    ChartGroup = ChartGroupType.Community,
                    MoreInfo = @"
                        <p>This includes:</p>
                        <ul>
                        <li>all running costs associated with a community-focused school activity or facility</li>
                        <li>recruitment costs and materials</li>
                        </ul>

                        <p>It excludes: </p>

                        <ul>
                        <li>any community-focused running costs that are incurred as a result of a third party delivering the activity where they’re not directly employed or contracted by the school.</li>
                        </ul>"
                },

                //Total Income
                new ChartViewModel()
                {
                    Id = 33,
                    Name = "Total income",
                    FieldName = SchoolTrustFinanceDataFieldNames.TOTAL_INCOME,
                    TabType = TabType.Income,
                    ChartGroup = ChartGroupType.TotalIncome,
                    ChartSchoolType = ChartSchoolType.Both,
                    ChartType = ChartType.Total,
                    MoreInfo = @""
                },

                new ChartViewModel()
                {
                    Id = 34,
                    Name = "Grant funding total",
                    FieldName = SchoolTrustFinanceDataFieldNames.GRANT_FUNDING,
                    TabType = TabType.Income,
                    ChartGroup = ChartGroupType.TotalIncome,
                    ChartSchoolType = ChartSchoolType.Both,
                    ChartType = ChartType.Total,
                    DrillInto = ChartGroupType.GrantFunding,
                    MoreInfo = @""
                },

                new ChartViewModel()
                {
                    Id = 38,
                    Name = "Self-generated funding total",
                    FieldName = SchoolTrustFinanceDataFieldNames.SELF_GENERATED_FUNDING,
                    TabType = TabType.Income,
                    ChartGroup = ChartGroupType.TotalIncome,
                    ChartSchoolType = ChartSchoolType.Both,
                    ChartType = ChartType.Total,
                    DrillInto = ChartGroupType.SelfGenerated,
                    MoreInfo = @""
                },

                new ChartViewModel()
                {
                    Id = 51002,
                    Name = "Direct revenue financing (capital reserves transfers)",
                    FieldName = SchoolTrustFinanceDataFieldNames.DIRECT_REVENUE_FINANCING,
                    TabType = TabType.Income,
                    ChartGroup = ChartGroupType.TotalIncome,
                    ChartSchoolType = ChartSchoolType.Both,
                    ChartType = ChartType.Total,
                    MoreInfo = @"<p>This includes:</p>
                        <ul>
                        <li>all amounts transferred to CI04 to be accumulated to fund capital works. This may include receipts from insurance claims for capital losses received into income under I11</li>
                        <li>any amount transferred to a local authority reserve to part fund a capital scheme which is being delivered by the local authority</li>
                        <li>any repayment of principal on a capital loan from the local authority</li>
                        </ul>
                        <p>It excludes: </p>
                        <ul>
                        <li>funds specifically provided for capital purposes</li>
                        </ul>"
                },

                //In-Year Balance
                new ChartViewModel()
                {
                    Id = 48,
                    Name = "In-year balance",
                    FieldName = SchoolTrustFinanceDataFieldNames.IN_YEAR_BALANCE,
                    TabType = TabType.Balance,
                    ChartGroup = ChartGroupType.InYearBalance,
                    ChartSchoolType = ChartSchoolType.Both,
                    MoreInfo = @"",
                    ChartType = ChartType.Total
                },
                new ChartViewModel()
                {
                    Id = 49,
                    Name = "Revenue reserve",
                    FieldName = SchoolTrustFinanceDataFieldNames.REVENUE_RESERVE,
                    TabType = TabType.Balance,
                    ChartGroup = ChartGroupType.InYearBalance,
                    ChartSchoolType = ChartSchoolType.Both,
                    MoreInfo = @"
                    <p>For local authority maintained schools and single academy trusts, reserves are legally associated with one school and appear under that school in the charts.</p>
                    <h3 class=""heading-small"">For local authority maintained schools, revenue reserves include:</h3>
                    <ul><li>the school’s committed and uncommitted revenue balance, plus the community-focused extended school revenue balance.</li></ul>                    
                    <h3 class=""heading-small"">For single academy trusts, revenue reserves include:</h3>		
                    <ul><li>the closing balance (restricted and unrestricted funds) carried forward from the previous year, plus total income in the current year (revenue, funds inherited on conversion/transfer and contributions from academies to trust) minus total expenditure in the current year.</li></ul>		                    
                    <p>For multi-academy trusts, the trust is the legal entity and all revenue reserves belong legally to the trust. We aggregate all declared reserves to trust level and they appear under the trust. The total can be seen by looking up the trust on the website, selecting ‘Balance' and choosing ‘Trust and academies' or ‘Trust only’ from the dropdown under ‘Central financing’.</p>
                    <p>For single academies within a multi-academy trusts, we've estimated a value per academy by apportioning the trust’s reserves on a pro-rata basis using the FTE number of pupils in each academy within that MAT. This can be seen by looking up the academy, selecting ‘Balance’ and choosing 'Include share of trust finance’ from the dropdown under ‘Central financing’.</p>",
                    ChartType = ChartType.Total
                },


                //Grant Funding
                new ChartViewModel()
                {
                    Id = 34,
                    Name = "Grant funding total",
                    FieldName = SchoolTrustFinanceDataFieldNames.GRANT_FUNDING,
                    TabType = TabType.Income,
                    ChartGroup = ChartGroupType.GrantFunding,
                    ChartSchoolType = ChartSchoolType.Both,
                    ChartType = ChartType.Total,
                    MoreInfo = @""
                },
                new ChartViewModel()
                {
                    Id = 35,
                    Name = "Direct grants",
                    FieldName = SchoolTrustFinanceDataFieldNames.DIRECT_GRANT,
                    TabType = TabType.Income,
                    ChartGroup = ChartGroupType.GrantFunding,
                    ChartSchoolType = ChartSchoolType.Both,
                    MoreInfo = @"<p>Where there is funding, this includes:</p>
                                <ul>
                                    <li>pre-16 funding</li>
                                    <li>post-16 funding</li>
                                    <li>DfE/EFA revenue grants</li>
                                    <li>other DfE/EFA revenue grants</li>
                                    <li>other income (local authority and other government grants)</li>
                                    <li>government source (non-grant)</li>
                                </ul>",
                    SubCharts = new List<ChartViewModel>()
                    {
                        new ChartViewModel
                        {
                            Id = 35001,
                            Name = "Pre-16 and post-16 funding",
                            FieldName = SchoolTrustFinanceDataFieldNames.PRE_POST_16_FUNDING,
                            TabType = TabType.Income,
                            ChartGroup = ChartGroupType.GrantFunding,
                            ChartSchoolType = ChartSchoolType.Both,
                            MoreInfo =
                                @"<p>For pre-16 this is the major share of funding provided by the Education Funding Agency to the school.</p>
                                <p>For post-16 funding it includes</p>
                                <ul>
                                <li>funding from public sources for sixth-form students</li>
                                <li>Education Funding Agency (EFA) funding</li>
                                <li>additional learning support funding for sixth forms from the EFA within their main EFA budget allocations</li>
                                <li>16-19 bursary fund </li>
                                <li>post-16 high needs place funding (elements 1 & 2) </li>
                                </ul>

                                <p>For post-16 funding it excludes:</p>
                                <ul>
                                <li>voluntary sources of funding for sixth-form students</li>
                                <li>any balances carried forward from previous years</li>
                                <li>high needs top-up funding (element 3) given by the local authority to the school</li>
                                </ul>"
                        },
                        new ChartViewModel
                        {
                            Id = 35003,
                            Name = "Other DfE/EFA revenue grants",
                            FieldName = SchoolTrustFinanceDataFieldNames.OTHER_DFE_GRANTS,
                            TabType = TabType.Income,
                            ChartGroup = ChartGroupType.GrantFunding,
                            ChartSchoolType = ChartSchoolType.Both,
                            MoreInfo = @""
                        },
                        new ChartViewModel
                        {
                            Id = 35004,
                            Name = "Other income (local authority and other government grants)",
                            FieldName = SchoolTrustFinanceDataFieldNames.OTHER_INCOME_GRANTS,
                            TabType = TabType.Income,
                            ChartGroup = ChartGroupType.GrantFunding,
                            ChartSchoolType = ChartSchoolType.Both,
                            MoreInfo = @"
                                <p>This includes:</p>
                                <ul>
                                <li>income from the National College of Teaching and Leadership</li>
                                <li>the total of all development and other non-capital grants from government </li>
                                <li>Salix loans </li>
                                <li>year 7 catch-up premium</li>
                                <li>the School Direct salaried programme </li>
                                </ul>
                                <p>It excludes:</p>
                                <ul>
                                <li>payments by government agencies for goods or services provided by the school</li>
                                <li>Big Lottery Fund</li>
                                <li>grants not funded through government</li>
                                <li>any balances carried forward from previous years</li>
                                </ul>
                                "
                        },
                        new ChartViewModel
                        {
                            Id = 35005,
                            Name = "Government source (non-grant)",
                            FieldName = SchoolTrustFinanceDataFieldNames.GOVERNMENT_SOURCE,
                            TabType = TabType.Income,
                            ChartGroup = ChartGroupType.GrantFunding,
                            ChartSchoolType = ChartSchoolType.Academy,
                            MoreInfo = @""
                        },
                    }
                },
                new ChartViewModel()
                {
                    Id = 36,
                    Name = "Community grants",
                    FieldName = SchoolTrustFinanceDataFieldNames.COMMUNITY_GRANTS,
                    TabType = TabType.Income,
                    ChartGroup = ChartGroupType.GrantFunding,
                    ChartSchoolType = ChartSchoolType.Both,
                    MoreInfo = @"<p>Where there is funding, this includes:</p>
                                <ul>
                                <li>academies</li>
                                <li>non-government</li>
                                <li>other funding</li>
                                <li>community-focused school funding and/or grants</li>
                                <li>additional grant for schools</li>
                                </ul>",
                    SubCharts = new List<ChartViewModel>()
                    {
                        new ChartViewModel
                        {
                            Id = 36001, 
                            Name = "Academies",
                            FieldName = SchoolTrustFinanceDataFieldNames.ACADEMIES,
                            ChartSchoolType = ChartSchoolType.Academy,
                            TabType = TabType.Income,
                            ChartGroup = ChartGroupType.GrantFunding,
                            MoreInfo = @"<p>This includes:</p>
                                <ul>
                                <li>any non-grant funding received from another academy or academy trust.</li>
                                </ul>
                                "
                        },
                        new ChartViewModel
                        {
                            Id = 36002,
                            Name = "Non-government",
                            FieldName = SchoolTrustFinanceDataFieldNames.NON_GOVERNMENT,
                            ChartSchoolType = ChartSchoolType.Academy,
                            TabType = TabType.Income,
                            ChartGroup = ChartGroupType.GrantFunding,
                            MoreInfo = @"<p>This includes:</p>
                                <ul>
                                <li>the amount of any non-grant funding received from any non-government organisation.</li>
                                </lu>
                                "
                        },
                        new ChartViewModel
                        {
                            Id = 36003,
                            Name = "Other Funding",
                            FieldName = SchoolTrustFinanceDataFieldNames.OTHER_INCOME,
                            ChartSchoolType = ChartSchoolType.Academy,
                            TabType = TabType.Income,
                            ChartGroup = ChartGroupType.GrantFunding,
                        },
                        new ChartViewModel
                        {
                            Id = 36004,
                            Name = "Community focussed school funding and/or grants",
                            FieldName = SchoolTrustFinanceDataFieldNames.COMMUNITY_FOCUSED,
                            ChartSchoolType = ChartSchoolType.Maintained,
                            TabType = TabType.Income,
                            ChartGroup = ChartGroupType.GrantFunding,
                            MoreInfo = @"<p>This includes: </p>
                                        <ul><li>sources of funding for community-focused activities </li></ul>
                                        <p>It excludes:</p>
                                        <ul><li>any funding for pupil-focused extended school activities</li></ul>"
                        },
                        new ChartViewModel
                        {
                            Id = 36005,
                            Name = "Additional grant for schools",
                            FieldName = SchoolTrustFinanceDataFieldNames.ADDITIONAL_GRANT,
                            ChartSchoolType = ChartSchoolType.Maintained,
                            TabType = TabType.Income,
                            ChartGroup = ChartGroupType.GrantFunding,
                            MoreInfo = @"<p>This includes:</p>
                                <ul>
                                <li>additional grant funding for secondary schools to release a PE teacher to work with local primary schools</li>
                                <li>primary PE and sports grant </li>
                                <li>universal infant free school meal funding </li>
                                </ul>

                                <p>It excludes:</p>

                                <ul>
                                <li>any other source of funding or income for the above activities</li>
                                </ul>"
                        },
                    }
                },
                new ChartViewModel()
                {
                    Id = 37,
                    Name = "Targeted grants",
                    FieldName = SchoolTrustFinanceDataFieldNames.TARGETED_GRANTS,
                    TabType = TabType.Income,
                    ChartGroup = ChartGroupType.GrantFunding,
                    ChartSchoolType = ChartSchoolType.Both,
                    MoreInfo = @"<p>Where there is funding, this includes:</p>
                                <ul>
                                <li>pupil-focused extended school funding and/or grants</li>
                                <li>pupil premium</li>
                                <li>ESG</li>
                                <li>SEN</li>
                                <li>funding for minority ethnic pupils</li>
                                </ul>",
                    SubCharts = new List<ChartViewModel>()
                    {
                        new ChartViewModel
                        {
                            Id = 37001,
                            Name = "Pupil focused extended school funding and/or grants",
                            FieldName = SchoolTrustFinanceDataFieldNames.PUPIL_FOCUSED_FUNDING,
                            TabType = TabType.Income,
                            ChartGroup = ChartGroupType.GrantFunding,
                            ChartSchoolType = ChartSchoolType.Maintained,
                            MoreInfo = @"<p>This includes: </p>

                                <ul>
                                <li>any government funds for pupil-focused extended school activities</li>
                                <li>other sources of funding for pupil-focused extended school activities </li>
                                </ul>

                                <p>It excludes: </p>

                                <ul>
                                <li>charges for these activities</li>
                                <li>funding which is to be attributed to a community-focused activity </li>
                                </ul>"
                        },
                        new ChartViewModel
                        {
                            Id = 37002,
                            Name = "Pupil premium",
                            FieldName = SchoolTrustFinanceDataFieldNames.PUPIL_PREMIUM,
                            TabType = TabType.Income,
                            ChartGroup = ChartGroupType.GrantFunding,
                            ChartSchoolType = ChartSchoolType.Both,
                            MoreInfo = @"<p>This includes: </p>

                                <ul>
                                <li>pupil premium funding</li>
                                <li>pupil premium funding received directly from local authorities other than the school’s maintaining authority</li>
                                <li>summer school funding </li>
                                </ul>

                                <p>It excludes: </p>
                                <ul>
                                <li>any other source of funding for deprived pupils</li>
                                <li>any balances carried forward from previous years</li>
                                <li>early years pupil premium</li>
                                </ul>"
                        },
                        new ChartViewModel
                        {
                            Id = 377003,
                            Name = "ESG",
                            FieldName = SchoolTrustFinanceDataFieldNames.ESG,
                            TabType = TabType.Income,
                            ChartGroup = ChartGroupType.GrantFunding,
                            ChartSchoolType = ChartSchoolType.Academy,
                            MoreInfo =
                                @"<p>This is the value of the Education Services Grant provided by the Education Funding Agency to the school.</p>"
                        },
                        new ChartViewModel
                        {
                            Id = 37004,
                            Name = "High needs top-up funding",
                            FieldName = SchoolTrustFinanceDataFieldNames.SEN,
                            TabType = TabType.Income,
                            ChartGroup = ChartGroupType.GrantFunding,
                            ChartSchoolType = ChartSchoolType.Both,
                            MoreInfo = @"<p>This is funding outside the school budget share. </p>

                                        <p>It includes: </p>

                                        <ul>
                                        <li>high needs top-up funding (from any commissioner - home local authority, other local authority or other school) </li>
                                        <li>any top-up funding (element 3) from any local authority for sixth-form students with high needs </li>
                                        </ul>

                                        <p>It excludes: </p>

                                        <ul>
                                        <li>voluntary sources of funding for high needs pupils</li>
                                        <li>place funding delegated by the local authority to a special unit or resourced provision in a mainstream school, to a special school, or a pupil referral unit </li>
                                        <li>special educational needs (SEN) budget within your school’s budget share</li>
                                        <li>funding for SEN or alternative provision support services commissioned by a local authority for delivery under a service level agreement</li>
                                        <li>any balances carried forward from previous years</li>
                                        </ul>"
                        },
                        new ChartViewModel
                        {
                            Id = 37005,
                            Name = "Funding for minority ethnic pupils",
                            FieldName = SchoolTrustFinanceDataFieldNames.FUNDING_MINORITY,
                            TabType = TabType.Income,
                            ChartGroup = ChartGroupType.GrantFunding,
                            ChartSchoolType = ChartSchoolType.Maintained,
                            MoreInfo =
                                @"<p>This is any devolved funding which is allocated in addition to the school’s budget share. </p>

                                    <p>It includes:</p>

                                    <ul>
                                    <li>any public funds intended to promote access and opportunity for minority ethnic pupils in support of English as an additional language or to raise attainment </li>
                                    </ul>

                                    <p>It excludes:</p>

                                    <ul>
                                    <li>voluntary sources of funds for minority ethnic and traveler pupils </li>
                                    <li>any balances carried forward from previous years</li>
                                    </ul>"
                        },
                    }
                },

                //Self Generated
                new ChartViewModel()
                {
                    Id = 38,
                    Name = "Self-generated funding total",
                    FieldName = SchoolTrustFinanceDataFieldNames.SELF_GENERATED_FUNDING,
                    TabType = TabType.Income,
                    ChartGroup = ChartGroupType.SelfGenerated,
                    ChartSchoolType = ChartSchoolType.Both,
                    MoreInfo = @"",
                    ChartType = ChartType.Total
                },
                new ChartViewModel()
                {
                    Id = 39,
                    Name = "Community focused school facilities income",
                    FieldName = SchoolTrustFinanceDataFieldNames.COMM_FOCUSED_SCHOOL_FACILITIES,
                    TabType = TabType.Income,
                    ChartGroup = ChartGroupType.SelfGenerated,
                    ChartSchoolType = ChartSchoolType.Maintained,
                    MoreInfo = @"
                                <p>This is income from community-focused school facilities and activities. </p>
                                <p>This includes: </p>
                                <ul><li>income from facilities or activities where schools have directly employed someone or contracted a third party to run a community-focused facility  </li></ul>
                                <p>It excludes:</p>
                                <ul><li>income from facilities which are primarily for the benefit of pupils and the school</li></ul>"
                },
                new ChartViewModel()
                {
                    Id = 40,
                    Name = "Income from facilities and services",
                    FieldName = SchoolTrustFinanceDataFieldNames.INCOME_FROM_FACILITIES,
                    TabType = TabType.Income,
                    ChartGroup = ChartGroupType.SelfGenerated,
                    ChartSchoolType = ChartSchoolType.Both,
                    MoreInfo = @"<p>This includes:</p>

                                <ul>
                                <li>income from meals provided to external customers, including other schools</li>
                                <li>income from assets such as the hire of premises, equipment or other facilities</li>
                                <li>all other income the school receives from facilities and services, e.g. income for consultancy, training courses and examination fees</li>
                                <li>any interest payments received from bank accounts held in the school's name or used to fund school activities</li>
                                <li>income from the sale of school uniforms, materials, private phone calls, photocopying, publications, books</li>
                                <li>income from before and after school clubs</li>
                                <li>income from the re-sale of items to pupils e.g. musical instruments, classroom resources, commission on photographs</li>
                                <li>income from non-catering vending machines</li>
                                <li>income from a pupil-focused special facility</li>
                                <li>rental of school premises including deductions from salaries where staff live on site</li>
                                <li>income from universities for student/teacher placements</li>
                                <li>income from energy/feed in tariffs</li>
                                <li>income from SEN and alternative provision support services commissioned by a local authority or another school, for delivery under a service level agreement </li>
                                </ul>

                                <p>It excludes:</p> 

                                <ul>
                                <li>payments received from other schools for which you haven’t provided a service</li>
                                <li>income from community-focused special facilities</li>
                                <li>high needs place funding</li>
                                <li>high needs top-up funding</li>
                                <li>any balances carried forward from previous years</li>
                                </ul>"
                },
                new ChartViewModel()
                {
                    Id = 41,
                    Name = "Income from catering",
                    FieldName = SchoolTrustFinanceDataFieldNames.INCOME_FROM_CATERING,
                    TabType = TabType.Income,
                    ChartGroup = ChartGroupType.SelfGenerated,
                    ChartSchoolType = ChartSchoolType.Both,
                    MoreInfo = @"<p>This includes: </p>

                                <ul>
                                <li>income from catering, school milk, and catering vending machines</li>
                                <li>any payments received from catering contractors, e.g. where a contractor has previously overcharged the school </li>
                                </ul>

                                <p>It excludes:</p> 

                                <ul>
                                <li>receipts for catering for external customers</li>
                                <li>income from non-catering vending machines</li>
                                <li>any balances carried forward from previous years</li>
                                </ul>"
                },
                new ChartViewModel()
                {
                    Id = 42,
                    Name = "Donations and/or voluntary funds",
                    FieldName = SchoolTrustFinanceDataFieldNames.DONATIONS,
                    TabType = TabType.Income,
                    ChartGroup = ChartGroupType.SelfGenerated,
                    ChartSchoolType = ChartSchoolType.Both,
                    MoreInfo =
                        @"<p>This is income from private sources under the control of the governing body, including:</p>

                                <ul>
                                <li>income provided from foundation, diocese or trust funds </li>
                                <li>business sponsorship</li>
                                <li>income from fundraising activities</li>
                                <li>contributions from parents (not expressly requested by the school) that are used to provide educational benefits</li>
                                </ul>

                                <p>It excludes:</p>

                                <ul>
                                <li>any contributions or donations that are not used for the benefit of students’ learning or the school </li>
                                <li>any balances available in trust funds or other private or non-public accounts</li>
                                <li>any balances carried forward from previous years</li>
                                </ul>"
                },
                new ChartViewModel()
                {
                    Id = 43,
                    Name = "Income from contributions to visits",
                    FieldName = SchoolTrustFinanceDataFieldNames.CONTRIBUTIONS_TO_VISITS,
                    TabType = TabType.Income,
                    ChartGroup = ChartGroupType.SelfGenerated,
                    ChartSchoolType = ChartSchoolType.Maintained,
                    MoreInfo = @"<p>This includes: </p>

                                <ul>
                                <li>income from parental contributions requested by the school, e.g. educational visits, field trips, boarding fees, and payments to the school for damage done by pupils </li>
                                </ul>

                                <p>It excludes: </p>
                                <ul>

                                <li>donations and voluntary funds not expressly requested by the school</li>
                                <li>any balances carried forward from previous years</li>
                                </ul>"
                },
                new ChartViewModel()
                {
                    Id = 44,
                    Name = "Receipts from supply teacher insurance claims",
                    FieldName = SchoolTrustFinanceDataFieldNames.RECEIPTS_FROM_SUPPLY,
                    TabType = TabType.Income,
                    ChartGroup = ChartGroupType.SelfGenerated,
                    ChartSchoolType = ChartSchoolType.Both,
                    MoreInfo = @"<p>This includes:</p>

                                <ul>
                                <li>payments from staff absence insurance schemes (including those offered by the local authority) to cover the cost of supply teachers </li>
                                </ul>

                                <p>It excludes:</p> 

                                <ul>
                                <li>insurance receipts for any other claim, for example absence of non-teaching staff, or building, contents, and public liability</li>
                                <li>any balances carried forward from previous years</li>
                                </ul>"
                },
                new ChartViewModel()
                {
                    Id = 45,
                    Name = "Receipts from other insurance claims",
                    FieldName = SchoolTrustFinanceDataFieldNames.RECEIPTS_FROM_OTHER,
                    TabType = TabType.Income,
                    ChartGroup = ChartGroupType.SelfGenerated,
                    ChartSchoolType = ChartSchoolType.Maintained,
                    MoreInfo = @""
                },
                new ChartViewModel()
                {
                    Id = 46,
                    Name = "Investment income",
                    FieldName = SchoolTrustFinanceDataFieldNames.INVESTMENT_INCOME,
                    TabType = TabType.Income,
                    ChartGroup = ChartGroupType.SelfGenerated,
                    ChartSchoolType = ChartSchoolType.Academy,
                    MoreInfo = @"<p>This includes:</p>
                                <ul>
                                <li>interest</li>
                                <li>dividend income</li>
                                <li>other investment income</li>
                                </ul>
                                "
                },
                new ChartViewModel()
                {
                    Id = 47,
                    Name = "Other self-generated income",
                    FieldName = SchoolTrustFinanceDataFieldNames.OTHER_SELF_GENERATED,
                    TabType = TabType.Income,
                    ChartGroup = ChartGroupType.SelfGenerated,
                    ChartSchoolType = ChartSchoolType.Academy,
                    MoreInfo = @"<p>This includes income as a result of:</p>
                                <ul>
                                <li>fundraising activity</li>
                                <li>lettings</li>
                                <li>non-governmental grants</li>
                                <li>commercial sponsorship</li>
                                <li>consultancy</li>
                                </ul>
                                "
                },

                //Workforce
                new ChartViewModel()
                {
                    Id = 50,
                    Name = "School workforce (Full Time Equivalent)",
                    FieldName = SchoolTrustFinanceDataFieldNames.WORKFORCE_TOTAL,
                    TabType = TabType.Workforce,
                    ChartGroup = ChartGroupType.Workforce,
                    ChartSchoolType = ChartSchoolType.Both,
                    MoreInfo = @"<p>This is the full-time equivalent (Full Time Equivalent) of the total school workforce.</p>

                    <p>It includes:</p>
                    <ul>
                    <li>classroom Teachers (Full Time Equivalent)</li>
                    <li>Senior leadership (Full Time Equivalent)</li>
                    <li>teaching assistants (Full Time Equivalent)</li>
                    <li>non-classroom-based support staff</li>
                    </ul>",
                    ChartType = ChartType.Total
                },
                new ChartViewModel()
                {
                    Id = 51,
                    Name = "Total number of teachers (Full Time Equivalent)",
                    FieldName = SchoolTrustFinanceDataFieldNames.TEACHERS_TOTAL,
                    TabType = TabType.Workforce,
                    ChartGroup = ChartGroupType.Workforce,
                    ChartSchoolType = ChartSchoolType.Both,
                    MoreInfo = @"<p>This is the full-time equivalent of all classroom and leadership teachers.</p>",
                    ChartType = ChartType.Total
                },
                new ChartViewModel()
                {
                    Id = 52,
                    Name = "Teachers with Qualified Teacher Status (%)",
                    FieldName = SchoolTrustFinanceDataFieldNames.PERCENTAGE_QUALIFIED_TEACHERS,
                    TabType = TabType.Workforce,
                    ChartGroup = ChartGroupType.Workforce,
                    ChartSchoolType = ChartSchoolType.Both,
                    MoreInfo =
                        @"<p>This is the number of teachers with Qualified Teacher Status divided by the total number of teachers in the school. </p>",
                    ChartType = ChartType.Total
                },
                new ChartViewModel()
                {
                    Id = 53,
                    Name = "Senior leadership (Full Time Equivalent)",
                    FieldName = SchoolTrustFinanceDataFieldNames.TEACHERS_LEADER,
                    TabType = TabType.Workforce,
                    ChartGroup = ChartGroupType.Workforce,
                    ChartSchoolType = ChartSchoolType.Both,
                    MoreInfo = @"<p>This is the full-time equivalent of senior leadership roles.</p>

                    <p>It includes:</p>
                    <ul>
                    <li>headteachers</li>
                    <li>deputy headteachers</li>
                    <li>assistant headteachers</li>
                    </ul>",
                    ChartType = ChartType.Total
                },
                new ChartViewModel()
                {
                    Id = 54,
                    Name = "Teaching assistants (Full Time Equivalent)",
                    FieldName = SchoolTrustFinanceDataFieldNames.FULL_TIME_TA,
                    TabType = TabType.Workforce,
                    ChartGroup = ChartGroupType.Workforce,
                    ChartSchoolType = ChartSchoolType.Both,
                    MoreInfo = @"<p>This is the full-time equivalent of teaching assistants.</p>

                    <p>It includes:</p>
                    <ul>
                    <li>teaching assistants</li>
                    <li>higher level teaching assistants</li>
                    <li>minority ethnic and special educational needs support staff</li>
                    </ul>",
                    ChartType = ChartType.Total
                },
                new ChartViewModel()
                {
                    Name = "Non-classroom support staff - excluding auxiliary staff (Full Time Equivalent)",
                    FieldName =SchoolTrustFinanceDataFieldNames.FULL_TIME_OTHER,
                    TabType = TabType.Workforce,
                    ChartGroup = ChartGroupType.Workforce,
                    ChartSchoolType = ChartSchoolType.Both,
                    MoreInfo = @"<p>This is the full-time equivalent of non-classroom-based support staff.</p>

                    <p>It excludes:</p>
                    <ul>
                    <li>auxiliary staff</li>
                    <li>third party support staff</li>
                    </ul>",
                    ChartType = ChartType.Total
                },
                new ChartViewModel()
                {
                    Id = 56,
                    Name = "Auxiliary staff (Full Time Equivalent)",
                    FieldName = SchoolTrustFinanceDataFieldNames.AUX_STAFF,
                    TabType = TabType.Workforce,
                    ChartGroup = ChartGroupType.Workforce,
                    ChartSchoolType = ChartSchoolType.Both,
                    MoreInfo = @"<p>This is the full-time equivalent of full and part-time auxiliary staff.</p>

                    <p>It includes:</p>
                    <ul>
                    <li>catering</li>
                    <li>school maintenance staff</li>
                    </ul>",
                    ChartType = ChartType.Total
                },
                new ChartViewModel()
                {
                    Id = 57,
                    Name = "School workforce (headcount)",
                    FieldName = SchoolTrustFinanceDataFieldNames.WORKFORCE_HEADCOUNT,
                    TabType = TabType.Workforce,
                    ChartGroup = ChartGroupType.Workforce,
                    ChartSchoolType = ChartSchoolType.Both,
                    MoreInfo = @"<p>This is the total headcount of the school workforce.</p>

                    <p>It includes:</p>
                    <ul>
                    <li>full and part-time teachers (including school leadership teachers)</li>
                    <li>teaching assistants</li>
                    <li>non-classroom-based support staff</li>
                    </ul>",
                    ChartType = ChartType.Total
                },

                //School performance (for download only)
                new ChartViewModel()
                {
                    Name = "Key Stage 2 attainment",
                    FieldName = SchoolTrustFinanceDataFieldNames.KS2_ACTUAL,
                    TabType = TabType.AllIncludingSchoolPerf,
                    ChartGroup = ChartGroupType.SP,
                    ChartSchoolType = ChartSchoolType.Both,
                },

                new ChartViewModel()
                {
                    Name = "Key Stage 2 progress",
                    FieldName = SchoolTrustFinanceDataFieldNames.KS2_PROGRESS,
                    TabType = TabType.AllIncludingSchoolPerf,
                    ChartGroup = ChartGroupType.SP,
                    ChartSchoolType = ChartSchoolType.Both,
                },

                new ChartViewModel()
                {
                    Name = "Average attainment",
                    FieldName = SchoolTrustFinanceDataFieldNames.AVERAGE_ATTAINMENT,
                    TabType = TabType.AllIncludingSchoolPerf,
                    ChartGroup = ChartGroupType.SP,
                    ChartSchoolType = ChartSchoolType.Both,
                },

                new ChartViewModel()
                {
                    Name = "Progress 8 measure",
                    FieldName = SchoolTrustFinanceDataFieldNames.PROGRESS_8_MEASURE,
                    TabType = TabType.AllIncludingSchoolPerf,
                    ChartGroup = ChartGroupType.SP,
                    ChartSchoolType = ChartSchoolType.Both,
                },

                new ChartViewModel()
                {
                    Name = "Ofsted rating",
                    FieldName = SchoolTrustFinanceDataFieldNames.OFSTED_RATING,
                    TabType = TabType.AllIncludingSchoolPerf,
                    ChartGroup = ChartGroupType.SP,
                    ChartSchoolType = ChartSchoolType.Both,
                }
            };
        }
    }
}