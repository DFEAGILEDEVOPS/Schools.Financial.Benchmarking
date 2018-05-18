using SFB.Web.UI.Models;
using System.Collections.Generic;
using System.Linq;
using SFB.Web.Common;
using SFB.Web.UI.Helpers.Enums;

namespace SFB.Web.UI.Helpers
{
    public interface IBenchmarkChartBuilder
    {
        List<ChartViewModel> Build(RevenueGroupType revenueGroup, EstablishmentType schoolType);

        List<ChartViewModel> Build(RevenueGroupType revenueGroup, ChartGroupType chartGroup,
            EstablishmentType schoolType);
    }

    public class BenchmarkChartBuilder : IBenchmarkChartBuilder
    {
        public List<ChartViewModel> Build(RevenueGroupType revenueGroup, EstablishmentType schoolType)
        {
            var chartList = Build(revenueGroup);

            if (schoolType != EstablishmentType.All)
            {
                if (schoolType == EstablishmentType.Academy || schoolType == EstablishmentType.MAT)
                {
                    chartList = chartList.Where(c =>
                            c.ChartSchoolType == ChartSchoolType.Academy || c.ChartSchoolType == ChartSchoolType.Both)
                        .ToList();
                }
                else
                {
                    chartList = chartList.Where(c =>
                            c.ChartSchoolType == ChartSchoolType.Maintained ||
                            c.ChartSchoolType == ChartSchoolType.Both)
                        .ToList();
                }
            }

            return chartList;
        }

        public List<ChartViewModel> Build(RevenueGroupType revenueGroup, ChartGroupType chartGroup,
            EstablishmentType schoolType)
        {
            var chartList = Build(revenueGroup, chartGroup);

            if (schoolType != EstablishmentType.All)
            {
                if (schoolType == EstablishmentType.Academy || schoolType == EstablishmentType.MAT)
                {
                    chartList =
                        chartList.Where(
                                c =>
                                    c.ChartSchoolType == ChartSchoolType.Academy ||
                                    c.ChartSchoolType == ChartSchoolType.Both)
                            .ToList();
                }
                else
                {
                    chartList =
                        chartList.Where(
                                c =>
                                    c.ChartSchoolType == ChartSchoolType.Maintained ||
                                    c.ChartSchoolType == ChartSchoolType.Both)
                            .ToList();
                }
            }
            return chartList;
        }

        private List<ChartViewModel> Build(RevenueGroupType revenueGroup)
        {
            return
                BuildChartList()
                    .Where(c => (revenueGroup == RevenueGroupType.AllIncludingSchoolPerf || c.RevenueGroup == revenueGroup)
                                || (revenueGroup == RevenueGroupType.AllExcludingSchoolPerf && c.RevenueGroup != RevenueGroupType.AllIncludingSchoolPerf))
                    .ToList();
        }

        private List<ChartViewModel> Build(RevenueGroupType revenueGroup, ChartGroupType chartGroup)
        {
            var chartList =
                BuildChartList().Where(c => ((revenueGroup == RevenueGroupType.AllIncludingSchoolPerf || c.RevenueGroup == revenueGroup) && (chartGroup == ChartGroupType.All || c.ChartGroup == chartGroup))
                                            || ((revenueGroup == RevenueGroupType.AllExcludingSchoolPerf && c.RevenueGroup != RevenueGroupType.AllIncludingSchoolPerf) && (chartGroup == ChartGroupType.All || c.ChartGroup == chartGroup))
                    )
                    .ToList();
            return chartList;
        }

        private List<ChartViewModel> BuildChartList()
        {
            return new List<ChartViewModel>()
            {
                //Total Expenditure
                new ChartViewModel()
                {
                    Name = "Total expenditure",
                    FieldName = "Total Expenditure",
                    RevenueGroup = RevenueGroupType.Expenditure,
                    ChartGroup = ChartGroupType.TotalExpenditure,
                    ChartSchoolType = ChartSchoolType.Both,
                    ChartType = ChartType.Total,
                    MoreInfo = @"",
                    Downloadable = true
                },

                new ChartViewModel()
                {
                    Name = "Staff total",
                    FieldName = "Staff Total",
                    RevenueGroup = RevenueGroupType.Expenditure,
                    ChartGroup = ChartGroupType.TotalExpenditure,
                    ChartSchoolType = ChartSchoolType.Both,
                    ChartType = ChartType.Total,
                    DrillInto = ChartGroupType.Staff,
                    MoreInfo = @""
                },

                new ChartViewModel()
                {
                    Name = "Premises total",
                    FieldName = "Premises",
                    RevenueGroup = RevenueGroupType.Expenditure,
                    ChartGroup = ChartGroupType.TotalExpenditure,
                    ChartSchoolType = ChartSchoolType.Both,
                    MoreInfo = @"",
                    DrillInto = ChartGroupType.Premises,
                    ChartType = ChartType.Total
                },

                new ChartViewModel()
                {
                    Name = "Occupation total",
                    FieldName = "Occupation",
                    RevenueGroup = RevenueGroupType.Expenditure,
                    ChartGroup = ChartGroupType.TotalExpenditure,
                    ChartSchoolType = ChartSchoolType.Both,
                    DrillInto = ChartGroupType.Occupation,
                    MoreInfo = @"",
                    ChartType = ChartType.Total,
                    HelpTooltip = "These are costs associated with occupying the school building. They include energy, water, sewerage, rates, insurance, and catering."
                },

                new ChartViewModel()
                {
                    Name = "Supplies and services total",
                    FieldName = "Supplies and Services",
                    RevenueGroup = RevenueGroupType.Expenditure,
                    ChartGroup = ChartGroupType.TotalExpenditure,
                    DrillInto = ChartGroupType.SuppliesAndServices,
                    ChartSchoolType = ChartSchoolType.Both,
                    ChartType = ChartType.Total
                },

                new ChartViewModel()
                {
                    Name = "Cost of finance total",
                    FieldName = "Cost of Finance",
                    RevenueGroup = RevenueGroupType.Expenditure,
                    ChartGroup = ChartGroupType.TotalExpenditure,
                    ChartSchoolType = ChartSchoolType.Both,
                    DrillInto = ChartGroupType.CostOfFinance,
                    MoreInfo = @"",
                    ChartType = ChartType.Total
                },

                new ChartViewModel()
                {
                    Name = "Community expenditure total",
                    FieldName = "Community Exp",
                    RevenueGroup = RevenueGroupType.Expenditure,
                    ChartGroup = ChartGroupType.TotalExpenditure,
                    ChartSchoolType = ChartSchoolType.Maintained,
                    DrillInto = ChartGroupType.Community,
                    MoreInfo = @"",
                    ChartType = ChartType.Total
                },

                new ChartViewModel()
                {
                    Name = "Other total",
                    FieldName = "Other",
                    RevenueGroup = RevenueGroupType.Expenditure,
                    ChartGroup = ChartGroupType.TotalExpenditure,
                    ChartSchoolType = ChartSchoolType.Academy,
                    MoreInfo = @"",
                    ChartType = ChartType.Total,
                    Downloadable = true
                },

                new ChartViewModel()
                {
                    Name = "Special facilities total",
                    FieldName = "Special facilities",
                    RevenueGroup = RevenueGroupType.Expenditure,
                    ChartGroup = ChartGroupType.TotalExpenditure,
                    ChartSchoolType = ChartSchoolType.Both,
                    DrillInto = ChartGroupType.SpecialFacilities,
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

                //Staff
                new ChartViewModel()
                {
                    Name = "Staff total",
                    FieldName = "Staff Total",
                    RevenueGroup = RevenueGroupType.Expenditure,
                    ChartGroup = ChartGroupType.Staff,
                    ChartSchoolType = ChartSchoolType.Both,
                    ChartType = ChartType.Total,
                    MoreInfo = @""
                },
                new ChartViewModel()
                {
                    Name = "Teaching staff",
                    FieldName = "Teaching staff",
                    RevenueGroup = RevenueGroupType.Expenditure,
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
                                </ul>",
                    Downloadable = true
                },
                new ChartViewModel()
                {
                    Name = "Supply staff",
                    FieldName = "Supply Staff",
                    RevenueGroup = RevenueGroupType.Expenditure,
                    ChartGroup = ChartGroupType.Staff,
                    ChartSchoolType = ChartSchoolType.Both,
                    MoreInfo = @"<p>This includes:</p>
                                <ul>
                                    <li>supply teaching staff</li>
                                    <li>supply teacher insurance</li>
                                    <li>agency supply teaching staff</li>
                                </ul>",
                    TableColumns = new List<DataTableColumnViewModel>
                    {
                        new DataTableColumnViewModel()
                        {
                            Name = "Supply teaching staff",
                            FieldName = "Supply teaching staff",
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
                        new DataTableColumnViewModel()
                        {
                            Name = "Supply teacher insurance",
                            FieldName = "Supply teacher insurance",
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
                        new DataTableColumnViewModel()
                        {
                            Name = "Agency supply teaching staff",
                            FieldName = "Agency supply teaching staff",
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
                        },
                    }
                },
                new ChartViewModel()
                {
                    Name = "Education support staff",
                    FieldName = "Education support staff",
                    RevenueGroup = RevenueGroupType.Expenditure,
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
                                </ul>",
                    Downloadable = true
                },
                new ChartViewModel()
                {
                    Name = "Administrative and clerical staff",
                    FieldName = "Administrative and clerical staff",
                    RevenueGroup = RevenueGroupType.Expenditure,
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
                                </ul>",
                    Downloadable = true
                },
                new ChartViewModel()
                {
                    Name = "Other staff costs",
                    FieldName = "Other Staff Costs",
                    RevenueGroup = RevenueGroupType.Expenditure,
                    ChartGroup = ChartGroupType.Staff,
                    ChartSchoolType = ChartSchoolType.Both,
                    MoreInfo = @"<p>This includes:</p>
                                <ul>
                                <li>cost of other staff</li>
                                <li>indirect employee expenses</li>
                                <li>staff development and training</li>
                                <li>staff-related insurance</li>
                                </ul>",
                    TableColumns = new List<DataTableColumnViewModel>
                    {
                        new DataTableColumnViewModel()
                        {
                            Name = "Other staff",
                            FieldName = "Other staff",
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
                        new DataTableColumnViewModel()
                        {
                            Name = "Indirect employee expenses",
                            FieldName = "Indirect employee expenses",
                            ChartSchoolType = ChartSchoolType.Both,
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
                        new DataTableColumnViewModel()
                        {
                            Name = "Staff development and training",
                            FieldName = "Staff development and training",
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
                        new DataTableColumnViewModel()
                        {
                            Name = "Staff related insurance",
                            FieldName = "Staff-related insurance",
                            ChartSchoolType = ChartSchoolType.Both,
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
                        },
                    }
                },

                //Premises
                new ChartViewModel()
                {
                    Name = "Premises total",
                    FieldName = "Premises",
                    RevenueGroup = RevenueGroupType.Expenditure,
                    ChartGroup = ChartGroupType.Premises,
                    ChartSchoolType = ChartSchoolType.Both,
                    MoreInfo = @"",
                    ChartType = ChartType.Total
                },
                new ChartViewModel()
                {
                    Name = "Premises staff",
                    FieldName = "Premises staff",
                    RevenueGroup = RevenueGroupType.Expenditure,
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
                                </ul>",
                    Downloadable = true
                },
                new ChartViewModel()
                {
                    Name = "Cleaning and caretaking",
                    FieldName = "Cleaning and caretaking",
                    RevenueGroup = RevenueGroupType.Expenditure,
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
                                <ul>",
                    Downloadable = true
                },
                new ChartViewModel()
                {
                    Name = "Maintenance and improvement",
                    FieldName = "Maintenance & Improvement",
                    RevenueGroup = RevenueGroupType.Expenditure,
                    ChartGroup = ChartGroupType.Premises,
                    ChartSchoolType = ChartSchoolType.Both,
                    MoreInfo = @"<p>This includes:</p>
                                <ul>
                                <li>building maintenance and improvement</li>
                                <li>grounds maintenance and improvement</li>
                                </ul>",
                    TableColumns = new List<DataTableColumnViewModel>
                    {
                        new DataTableColumnViewModel
                        {
                            Name = "Grounds maintenance and improvement",
                            FieldName = "Grounds maintenance and improvement",
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
                        new DataTableColumnViewModel
                        {
                            Name = "Building maintenance and improvement",
                            FieldName = "Building maintenance and improvement",
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
                        new DataTableColumnViewModel
                        {
                            Name = "Building and grounds maintenance and improvement",
                            FieldName = "Building and Grounds maintenance and improvement",
                            ChartSchoolType = ChartSchoolType.Academy,
                            MoreInfo = @""
                        }
                    }
                },
                new ChartViewModel()
                {
                    Name = "PFI charges",
                    FieldName = "PFI Charges",
                    RevenueGroup = RevenueGroupType.Expenditure,
                    ChartGroup = ChartGroupType.Premises,
                    ChartSchoolType = ChartSchoolType.Academy,
                    MoreInfo = @"",
                    Downloadable = true
                },

                //Occupation
                new ChartViewModel()
                {
                    Name = "Occupation total",
                    FieldName = "Occupation",
                    RevenueGroup = RevenueGroupType.Expenditure,
                    ChartGroup = ChartGroupType.Occupation,
                    ChartSchoolType = ChartSchoolType.Both,
                    MoreInfo = @"",
                    ChartType = ChartType.Total,
                    HelpTooltip = "These are costs associated with occupying the school building. They include energy, water, sewerage, rates, insurance, and catering."
                },
                new ChartViewModel()
                {
                    Name = "Energy",
                    FieldName = "Energy",
                    RevenueGroup = RevenueGroupType.Expenditure,
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
                                <ul>",
                    Downloadable = true
                },
                new ChartViewModel()
                {
                    Name = "Water and sewerage",
                    FieldName = "Water and sewerage",
                    RevenueGroup = RevenueGroupType.Expenditure,
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
                                </ul>",
                    Downloadable = true
                },
                new ChartViewModel()
                {
                    Name = "Rates",
                    FieldName = "Rates",
                    RevenueGroup = RevenueGroupType.Expenditure,
                    ChartGroup = ChartGroupType.Occupation,
                    ChartSchoolType = ChartSchoolType.Maintained,
                    MoreInfo = @"
                                <p>This includes:</p>

                                <ul>
                                <li>non-domestic rates expenditure (NNDR)</li>
                                </ul>

                                <p>This is separate from other occupation costs because it is imposed and therefore not a controllable expense. Unlike other items where there will be some element of control, it is a difficult area to benchmark.</p>",
                    Downloadable = true
                },
                new ChartViewModel()
                {
                    Name = "Other occupation costs",
                    FieldName = "Other occupation costs",
                    RevenueGroup = RevenueGroupType.Expenditure,
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
                                </ul>",
                    Downloadable = true
                },
                new ChartViewModel()
                {
                    Name = "Other insurance premiums",
                    FieldName = "Other insurance premiums",
                    RevenueGroup = RevenueGroupType.Expenditure,
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
                                </ul>",
                    Downloadable = true
                },
                new ChartViewModel()
                {
                    Name = "Catering expenditure",
                    FieldName = "Catering Exp",
                    RevenueGroup = RevenueGroupType.Expenditure,
                    ChartGroup = ChartGroupType.Occupation,
                    ChartSchoolType = ChartSchoolType.Both,
                    MoreInfo = @"",
                    TableColumns = new List<DataTableColumnViewModel>
                    {
                        new DataTableColumnViewModel()
                        {
                            Name = "Catering staff",
                            FieldName = "Catering staff",
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
                        new DataTableColumnViewModel()
                        {
                            Name = "Catering supplies",
                            FieldName = "Catering supplies",
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
                        }
                    }
                },
                new ChartViewModel()
                {
                    Name = "Rent and rates",
                    FieldName = "Rent and Rates",
                    RevenueGroup = RevenueGroupType.Expenditure,
                    ChartGroup = ChartGroupType.Occupation,
                    ChartSchoolType = ChartSchoolType.Academy,
                    MoreInfo = @"<p>This includes the amount incurred for:</p>
                                <ul>
                                <li>national non-domestic rates (NNDR)</li>
                                <li>business rates</li>
                                <li>operating leases/rental of buildings</li>
                                </ul>
                                ",
                    Downloadable = true
                },

                //Supplies and Services
                new ChartViewModel()
                {
                    Name = "Supplies and services total",
                    FieldName = "Supplies and Services",
                    RevenueGroup = RevenueGroupType.Expenditure,
                    ChartGroup = ChartGroupType.SuppliesAndServices,
                    ChartSchoolType = ChartSchoolType.Both,
                    ChartType = ChartType.Total
                },
                new ChartViewModel()
                {
                    Name = "Administrative supplies",
                    FieldName = "Administrative supplies - non educational",
                    RevenueGroup = RevenueGroupType.Expenditure,
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
                                </ul>",
                    Downloadable = true
                },
                new ChartViewModel()
                {
                    Name = "Educational supplies",
                    FieldName = "Educational Supplies",
                    RevenueGroup = RevenueGroupType.Expenditure,
                    ChartGroup = ChartGroupType.SuppliesAndServices,
                    ChartSchoolType = ChartSchoolType.Both,
                    MoreInfo = @"<p>This includes:</p>
                                <ul>
                                <li>learning resources (not ICT equipment)</li>
                                <li>ICT learning resources</li>
                                <li>examination fees</li>
                                </ul>",
                    TableColumns = new List<DataTableColumnViewModel>
                    {
                        new DataTableColumnViewModel
                        {
                            Name = "ICT learning resources",
                            FieldName = "ICT learning resources",
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
                        new DataTableColumnViewModel
                        {
                            Name = "Learning resources",
                            FieldName = "Learning resources (not ICT equipment)",
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
                        new DataTableColumnViewModel
                        {
                            Name = "Examination fees",
                            FieldName = "Examination fees",
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
                        }
                    }
                },
                new ChartViewModel()
                {
                    Name = "Bought-in professional services",
                    FieldName = "Brought in Professional Sevices",
                    RevenueGroup = RevenueGroupType.Expenditure,
                    ChartGroup = ChartGroupType.SuppliesAndServices,
                    ChartSchoolType = ChartSchoolType.Both,
                    MoreInfo = @"<p>This includes:</p>
                                <ul>
                                <li>educational consultancy</li>
                                <li>bought-in professional services</li>
                                <li>legal and professional</li>
                                <li>auditor costs</li>
                                </ul>",
                    TableColumns = new List<DataTableColumnViewModel>
                    {
                        new DataTableColumnViewModel
                        {
                            Name = "Educational consultancy",
                            FieldName = "Educational Consultancy",
                            ChartSchoolType = ChartSchoolType.Both,
                            MoreInfo = @"<p>This covers professional consultancy and advice to staff and governors.</p>"
                        },
                        new DataTableColumnViewModel
                        {
                            Name = "Bought in professional services - other",
                            FieldName = "Bought in professional services / other",
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
                        new DataTableColumnViewModel
                        {
                            Name = "Legal and professional",
                            FieldName = "Legal & Professional",
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
                        new DataTableColumnViewModel
                        {
                            Name = "Auditor costs",
                            FieldName = "Auditor costs",
                            ChartSchoolType = ChartSchoolType.Academy,
                            MoreInfo = @"<p>This covers expenditure for any audit costs. </p>"
                        }
                    }
                },

                //Special Facilities
                new ChartViewModel()
                {
                    Name = "Special facilities",
                    FieldName = "Special facilities",
                    RevenueGroup = RevenueGroupType.Expenditure,
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
                    ChartType = ChartType.Total,
                    Downloadable = true
                },

                //Cost of Finance
                new ChartViewModel()
                {
                    Name = "Cost of finance total",
                    FieldName = "Cost of Finance",
                    RevenueGroup = RevenueGroupType.Expenditure,
                    ChartGroup = ChartGroupType.CostOfFinance,
                    ChartSchoolType = ChartSchoolType.Both,
                    MoreInfo = @"",
                    ChartType = ChartType.Total,
                    TableColumns = new List<DataTableColumnViewModel>
                    {
                        new DataTableColumnViewModel
                        {
                            Name = "Loan interest",
                            FieldName = "Interest charges for Loan and Bank",
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
                        new DataTableColumnViewModel
                        {
                            Name = "Direct revenue financing (revenue contributions to capital)",
                            FieldName = "Direct revenue financing (Revenue contributions to capital)",
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
                        }
                    }
                },

                //Community
                new ChartViewModel()
                {
                    Name = "Community expenditure total",
                    FieldName = "Community Exp",
                    RevenueGroup = RevenueGroupType.Expenditure,
                    ChartGroup = ChartGroupType.Community,
                    ChartSchoolType = ChartSchoolType.Maintained,
                    MoreInfo = @"",
                    ChartType = ChartType.Total,
                    TableColumns = new List<DataTableColumnViewModel>
                    {
                        new DataTableColumnViewModel
                        {
                            Name = "Community focused school staff",
                            FieldName = "Community focused school staff",
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
                        new DataTableColumnViewModel
                        {
                            Name = "Community focused school costs",
                            FieldName = "Community focused school costs",
                            ChartSchoolType = ChartSchoolType.Maintained,
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
                        }
                    }
                },

                //Total Income
                new ChartViewModel()
                {
                    Name = "Total income",
                    FieldName = "Total Income",
                    RevenueGroup = RevenueGroupType.Income,
                    ChartGroup = ChartGroupType.TotalIncome,
                    ChartSchoolType = ChartSchoolType.Both,
                    ChartType = ChartType.Total,
                    MoreInfo = @"",
                    Downloadable = true
                },

                new ChartViewModel()
                {
                    Name = "Grant funding total",
                    FieldName = "Grant Funding",
                    RevenueGroup = RevenueGroupType.Income,
                    ChartGroup = ChartGroupType.TotalIncome,
                    ChartSchoolType = ChartSchoolType.Both,
                    ChartType = ChartType.Total,
                    DrillInto = ChartGroupType.GrantFunding,
                    MoreInfo = @""
                },

                new ChartViewModel()
                {
                    Name = "Self-generated total",
                    FieldName = "Self Generated Funding",
                    RevenueGroup = RevenueGroupType.Income,
                    ChartGroup = ChartGroupType.TotalIncome,
                    ChartSchoolType = ChartSchoolType.Both,
                    DrillInto = ChartGroupType.SelfGenerated,
                    ChartType = ChartType.Total,
                    MoreInfo = @""
                },

                //In-Year Balance
                new ChartViewModel()
                {
                    Name = "In-year balance",
                    FieldName = "In Year Balance",
                    RevenueGroup = RevenueGroupType.Balance,
                    ChartGroup = ChartGroupType.InYearBalance,
                    ChartSchoolType = ChartSchoolType.Both,
                    MoreInfo = @"",
                    ChartType = ChartType.Total,
                    Downloadable = true
                },
                new ChartViewModel()
                {
                    Name = "Revenue reserve",
                    FieldName = "Revenue Reserve",
                    RevenueGroup = RevenueGroupType.Balance,
                    ChartGroup = ChartGroupType.InYearBalance,
                    ChartSchoolType = ChartSchoolType.Both,
                    MoreInfo = @"<h3 class=""heading-small"">For local authority maintained schools, this includes:</h3>
                    <ul><li>the school’s committed and uncommitted revenue balance, plus the community-focused extended school revenue balance.</li></ul>
                    
                    <h3 class=""heading-small"">For academies, this includes:</h3>
		
		                    <ul><li>the closing balance (restricted and unrestricted funds) carried forward from the previous year, plus total income in the current year (revenue, funds inherited on conversion/transfer and contributions from academies to trust) minus total expenditure in the current year.</li></ul>
		                    
		                    <p>For multi-academy trusts (MATs), we've estimated a value per academy by apportioning it on a pro-rata basis using the FTE number of pupils in each academy within that MAT.</p>",
                    ChartType = ChartType.Total,
                    Downloadable = true
                },


                //Grant Funding
                new ChartViewModel()
                {
                    Name = "Grant funding total",
                    FieldName = "Grant Funding",
                    RevenueGroup = RevenueGroupType.Income,
                    ChartGroup = ChartGroupType.GrantFunding,
                    ChartSchoolType = ChartSchoolType.Both,
                    ChartType = ChartType.Total,
                    MoreInfo = @""
                },
                new ChartViewModel()
                {
                    Name = "Direct grants",
                    FieldName = "Direct Grant",
                    RevenueGroup = RevenueGroupType.Income,
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
                    TableColumns = new List<DataTableColumnViewModel>
                    {
                        new DataTableColumnViewModel
                        {
                            Name = "Pre-16 funding",
                            FieldName = "Pre-16 Funding",
                            ChartSchoolType = ChartSchoolType.Both,
                            MoreInfo =
                                @"<p>This is the major share of funding provided by the Education Funding Agency to the school.</p>"
                        },
                        new DataTableColumnViewModel
                        {
                            Name = "Funding for sixth-form students",
                            FieldName = "Post-16 Funding",
                            ChartSchoolType = ChartSchoolType.Both,
                            MoreInfo = @"<p>This includes:</p> 
                                <ul>
                                <li>funding from public sources for sixth-form students</li>
                                <li>Education Funding Agency (EFA) funding</li>
                                <li>additional learning support funding for sixth forms from the EFA within their main EFA budget allocations</li>
                                <li>16-19 bursary fund </li>
                                <li>post-16 high needs place funding (elements 1 & 2) </li>
                                </ul>

                                <p>It excludes:</p>

                                <ul>
                                <li>voluntary sources of funding for sixth-form students</li>
                                <li>any balances carried forward from previous years</li>
                                <li>high needs top-up funding (element 3) given by the local authority to the school</li>
                                </ul>"
                        },
                        new DataTableColumnViewModel
                        {
                            Name = "Other DfE/EFA revenue grants",
                            FieldName = "Other DfE/EFA Revenue Grants",
                            ChartSchoolType = ChartSchoolType.Both,
                            MoreInfo = @""
                        },
                        new DataTableColumnViewModel
                        {
                            Name = "Other income (local authority and other government grants)",
                            FieldName = "Other income (LA & other Government grants)",
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
                        new DataTableColumnViewModel
                        {
                            Name = "Government source (non-grant)",
                            FieldName = "Government source (non-grant)",
                            ChartSchoolType = ChartSchoolType.Academy,
                            MoreInfo = @""
                        }
                    }
                },
                new ChartViewModel()
                {
                    Name = "Community grants",
                    FieldName = "Community Grants",
                    RevenueGroup = RevenueGroupType.Income,
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
                    TableColumns = new List<DataTableColumnViewModel>
                    {
                        new DataTableColumnViewModel
                        {
                            Name = "Academies",
                            FieldName = "Academies",
                            ChartSchoolType = ChartSchoolType.Academy,
                            MoreInfo = @"<p>This includes:</p>
                                <ul>
                                <li>any non-grant funding received from another academy or academy trust.</li>
                                </ul>
                                "
                        },
                        new DataTableColumnViewModel
                        {
                            Name = "Non-government",
                            FieldName = "Non- Government",
                            ChartSchoolType = ChartSchoolType.Academy,
                            MoreInfo = @"<p>This includes:</p>
                                <ul>
                                <li>the amount of any non-grant funding received from any non-government organisation.</li>
                                </lu>
                                "
                        },
                        new DataTableColumnViewModel
                        {
                            Name = "Other Funding",
                            FieldName = "Other Income",
                            ChartSchoolType = ChartSchoolType.Academy,
                        },
                        new DataTableColumnViewModel
                        {
                            Name = "Community focussed school funding and/or grants",
                            FieldName = "Community focussed school funding and/or grants",
                            ChartSchoolType = ChartSchoolType.Maintained,
                            MoreInfo = @"<p>This includes: </p>
                                        <ul><li>sources of funding for community-focused activities </li></ul>
                                        <p>It excludes:</p>
                                        <ul><li>any funding for pupil-focused extended school activities</li></ul>"
                        },
                        new DataTableColumnViewModel
                        {
                            Name = "Additional grant for schools",
                            FieldName = "Additional grant for schools",
                            ChartSchoolType = ChartSchoolType.Maintained,
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
                        }
                    }
                },
                new ChartViewModel()
                {
                    Name = "Targeted grants",
                    FieldName = "Targeted Grants",
                    RevenueGroup = RevenueGroupType.Income,
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
                    TableColumns = new List<DataTableColumnViewModel>
                    {
                        new DataTableColumnViewModel
                        {
                            Name = "Pupil focused extended school funding and/or grants",
                            FieldName = "Pupil focussed extended school funding and/or grants",
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
                        new DataTableColumnViewModel
                        {
                            Name = "Pupil premium",
                            FieldName = "Pupil Premium",
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
                        new DataTableColumnViewModel
                        {
                            Name = "ESG",
                            FieldName = "ESG",
                            ChartSchoolType = ChartSchoolType.Academy,
                            MoreInfo =
                                @"<p>This is the value of the Education Services Grant provided by the Education Funding Agency to the school.</p>"
                        },
                        new DataTableColumnViewModel
                        {
                            Name = "High needs top-up funding",
                            FieldName = "SEN",
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
                        new DataTableColumnViewModel
                        {
                            Name = "Funding for minority ethnic pupils",
                            FieldName = "Funding for minority ethnic pupils",
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
                        }
                    }
                },

                //Self Generated
                new ChartViewModel()
                {
                    Name = "Self-generated funding total",
                    FieldName = "Self Generated Funding",
                    RevenueGroup = RevenueGroupType.Income,
                    ChartGroup = ChartGroupType.SelfGenerated,
                    ChartSchoolType = ChartSchoolType.Both,
                    MoreInfo = @"",
                    ChartType = ChartType.Total
                },
                new ChartViewModel()
                {
                    Name = "Community focused school facilities income",
                    FieldName = "Community focused school facilities income",
                    RevenueGroup = RevenueGroupType.Income,
                    ChartGroup = ChartGroupType.SelfGenerated,
                    ChartSchoolType = ChartSchoolType.Maintained,
                    MoreInfo = @"
                                <p>This is income from community-focused school facilities and activities. </p>
                                <p>This includes: </p>
                                <ul><li>income from facilities or activities where schools have directly employed someone or contracted a third party to run a community-focused facility  </li></ul>
                                <p>It excludes:</p>
                                <ul><li>income from facilities which are primarily for the benefit of pupils and the school</li></ul>",
                    Downloadable = true
                },
                new ChartViewModel()
                {
                    Name = "Income from facilities and services",
                    FieldName = "Income from facilities and services",
                    RevenueGroup = RevenueGroupType.Income,
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
                                </ul>",
                    Downloadable = true
                },
                new ChartViewModel()
                {
                    Name = "Income from catering",
                    FieldName = "Income from catering",
                    RevenueGroup = RevenueGroupType.Income,
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
                                </ul>",
                    Downloadable = true
                },
                new ChartViewModel()
                {
                    Name = "Donations and/or voluntary funds",
                    FieldName = "Donations and/or voluntary funds",
                    RevenueGroup = RevenueGroupType.Income,
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
                                </ul>",
                    Downloadable = true
                },
                new ChartViewModel()
                {
                    Name = "Income from contributions to visits",
                    FieldName = "Income from contributions to visits etc",
                    RevenueGroup = RevenueGroupType.Income,
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
                                </ul>",
                    Downloadable = true
                },
                new ChartViewModel()
                {
                    Name = "Receipts from supply teacher insurance claims",
                    FieldName = "Receipts from supply teacher insurance claims",
                    RevenueGroup = RevenueGroupType.Income,
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
                                </ul>",
                    Downloadable = true
                },
                new ChartViewModel()
                {
                    Name = "Receipts from other insurance claims",
                    FieldName = "Receipts from other insurance claims",
                    RevenueGroup = RevenueGroupType.Income,
                    ChartGroup = ChartGroupType.SelfGenerated,
                    ChartSchoolType = ChartSchoolType.Maintained,
                    MoreInfo = @"",
                    Downloadable = true
                },
                new ChartViewModel()
                {
                    Name = "Investment income",
                    FieldName = "Investment income",
                    RevenueGroup = RevenueGroupType.Income,
                    ChartGroup = ChartGroupType.SelfGenerated,
                    ChartSchoolType = ChartSchoolType.Academy,
                    MoreInfo = @"<p>This includes:</p>
                                <ul>
                                <li>interest</li>
                                <li>dividend income</li>
                                <li>other investment income</li>
                                </ul>
                                ",
                    Downloadable = true
                },
                new ChartViewModel()
                {
                    Name = "Other self-generated income",
                    FieldName = "Other self-generated income",
                    RevenueGroup = RevenueGroupType.Income,
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
                                ",
                    Downloadable = true
                },

                //Workforce
                new ChartViewModel()
                {
                    Name = "School workforce (Full Time Equivalent)",
                    FieldName = DBFieldNames.WORKFORCE_TOTAL,
                    RevenueGroup = RevenueGroupType.Workforce,
                    ChartGroup = ChartGroupType.Workforce,
                    ChartSchoolType = ChartSchoolType.Both,
                    MoreInfo = @"<p>This is the full-time equivalent (Full Time Equivalent) of the total school workforce.</p>

                    <p>It includes:</p>
                    <ul>
                    <li>classroom teachers (Full Time Equivalent)</li>
                    <li>senior leadership (Full Time Equivalent)</li>
                    <li>teaching assistants (Full Time Equivalent)</li>
                    <li>non-classroom-based support staff</li>
                    </ul>",
                    ChartType = ChartType.Total,
                    Downloadable = true,
                    ShowValue = UnitType.AbsoluteCount
                },
                new ChartViewModel()
                {
                    Name = "Teachers (Full Time Equivalent)",
                    FieldName = DBFieldNames.TEACHERS_TOTAL,
                    RevenueGroup = RevenueGroupType.Workforce,
                    ChartGroup = ChartGroupType.Workforce,
                    ChartSchoolType = ChartSchoolType.Both,
                    MoreInfo = @"<p>This is the full-time equivalent of all classroom and leadership teachers.</p>",
                    ChartType = ChartType.Total,
                    Downloadable = true,
                    ShowValue = UnitType.AbsoluteCount
                },
                new ChartViewModel()
                {
                    Name = "Teachers with Qualified Teacher Status (%)",
                    FieldName = DBFieldNames.PERCENTAGE_QUALIFIED_TEACHERS,
                    RevenueGroup = RevenueGroupType.Workforce,
                    ChartGroup = ChartGroupType.Workforce,
                    ChartSchoolType = ChartSchoolType.Both,
                    MoreInfo =
                        @"<p>This is the number of teachers with Qualified Teacher Status divided by the total number of teachers in the school. </p>",
                    ChartType = ChartType.Total,
                    Downloadable = true,
                    ShowValue = UnitType.AbsoluteCount
                },
                new ChartViewModel()
                {
                    Name = "Senior leadership (Full Time Equivalent)",
                    FieldName = DBFieldNames.TEACHERS_LEADER,
                    RevenueGroup = RevenueGroupType.Workforce,
                    ChartGroup = ChartGroupType.Workforce,
                    ChartSchoolType = ChartSchoolType.Both,
                    MoreInfo = @"<p>This is the full-time equivalent of senior leadership roles.</p>

                    <p>It includes:</p>
                    <ul>
                    <li>headteachers</li>
                    <li>deputy headteachers</li>
                    <li>assistant headteachers</li>
                    </ul>",
                    ChartType = ChartType.Total,
                    Downloadable = true,
                    ShowValue = UnitType.AbsoluteCount
                },
                new ChartViewModel()
                {
                    Name = "Teaching assistants (Full Time Equivalent)",
                    FieldName = DBFieldNames.FULL_TIME_TA,
                    RevenueGroup = RevenueGroupType.Workforce,
                    ChartGroup = ChartGroupType.Workforce,
                    ChartSchoolType = ChartSchoolType.Both,
                    MoreInfo = @"<p>This is the full-time equivalent of teaching assistants.</p>

                    <p>It includes:</p>
                    <ul>
                    <li>teaching assistants</li>
                    <li>higher level teaching assistants</li>
                    <li>minority ethnic and special educational needs support staff</li>
                    </ul>",
                    ChartType = ChartType.Total,
                    Downloadable = true,
                    ShowValue = UnitType.AbsoluteCount
                },
                new ChartViewModel()
                {
                    Name = "Non-classroom support staff – excluding auxiliary staff  (Full Time Equivalent)",
                    FieldName = DBFieldNames.FULL_TIME_OTHER,
                    RevenueGroup = RevenueGroupType.Workforce,
                    ChartGroup = ChartGroupType.Workforce,
                    ChartSchoolType = ChartSchoolType.Both,
                    MoreInfo = @"<p>This is the full-time equivalent of non-classroom-based support staff.</p>

                    <p>It excludes:</p>
                    <ul>
                    <li>auxiliary staff</li>
                    <li>third party support staff</li>
                    </ul>",
                    ChartType = ChartType.Total,
                    Downloadable = true,
                    ShowValue = UnitType.AbsoluteCount
                },
                new ChartViewModel()
                {
                    Name = "Auxiliary staff (Full Time Equivalent)",
                    FieldName = DBFieldNames.AUX_STAFF,
                    RevenueGroup = RevenueGroupType.Workforce,
                    ChartGroup = ChartGroupType.Workforce,
                    ChartSchoolType = ChartSchoolType.Both,
                    MoreInfo = @"<p>This is the full-time equivalent of full and part-time auxiliary staff.</p>

                    <p>It includes:</p>
                    <ul>
                    <li>catering</li>
                    <li>school maintenance staff</li>
                    </ul>",
                    ChartType = ChartType.Total,
                    Downloadable = true,
                    ShowValue = UnitType.AbsoluteCount
                },
                new ChartViewModel()
                {
                    Name = "School workforce (headcount)",
                    FieldName = DBFieldNames.WORKFORCE_HEADCOUNT,
                    RevenueGroup = RevenueGroupType.Workforce,
                    ChartGroup = ChartGroupType.Workforce,
                    ChartSchoolType = ChartSchoolType.Both,
                    MoreInfo = @"<p>This is the total headcount of the school workforce.</p>

                    <p>It includes:</p>
                    <ul>
                    <li>full and part-time teachers (including school leadership teachers)</li>
                    <li>teaching assistants</li>
                    <li>non-classroom-based support staff</li>
                    </ul>",
                    ChartType = ChartType.Total,
                    Downloadable = true,
                    ShowValue = UnitType.AbsoluteCount
                },

                //School performance (for download only)
                new ChartViewModel()
                {
                    Name = "Key Stage 2 attainment",
                    FieldName = DBFieldNames.KS2_ACTUAL,
                    RevenueGroup = RevenueGroupType.AllIncludingSchoolPerf,
                    ChartGroup = ChartGroupType.SP,
                    ChartSchoolType = ChartSchoolType.Both,
                    Downloadable = true
                },

                new ChartViewModel()
                {
                    Name = "Key Stage 2 progress",
                    FieldName = DBFieldNames.KS2_PROGRESS,
                    RevenueGroup = RevenueGroupType.AllIncludingSchoolPerf,
                    ChartGroup = ChartGroupType.SP,
                    ChartSchoolType = ChartSchoolType.Both,
                    Downloadable = true
                },

                new ChartViewModel()
                {
                    Name = "Average attainment",
                    FieldName = DBFieldNames.AVERAGE_ATTAINMENT,
                    RevenueGroup = RevenueGroupType.AllIncludingSchoolPerf,
                    ChartGroup = ChartGroupType.SP,
                    ChartSchoolType = ChartSchoolType.Both,
                    Downloadable = true
                },

                new ChartViewModel()
                {
                    Name = "Progress 8 measure",
                    FieldName = DBFieldNames.PROGRESS_8_MEASURE,
                    RevenueGroup = RevenueGroupType.AllIncludingSchoolPerf,
                    ChartGroup = ChartGroupType.SP,
                    ChartSchoolType = ChartSchoolType.Both,
                    Downloadable = true
                },

                new ChartViewModel()
                {
                    Name = "Ofsted rating",
                    FieldName = DBFieldNames.OFSTED_RATING,
                    RevenueGroup = RevenueGroupType.AllIncludingSchoolPerf,
                    ChartGroup = ChartGroupType.SP,
                    ChartSchoolType = ChartSchoolType.Both,
                    Downloadable = true
                }
            };
        }
    }
}