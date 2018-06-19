using Newtonsoft.Json;
using System.Collections.Generic;

namespace SFB.Web.Common.DataObjects
{
    public class SchoolTrustFinancialDataObject
    {
        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.MAT_NUMBER)]
        public string MATNumber;

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.TRUST_COMPANY_NAME)]
        public string TrustOrCompanyName;

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.OTHER_INCOME)]
        public decimal OtherIncome;

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.SEN)]
        public decimal SEN;

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.OTHER_DFE_GRANTS)]
        public decimal OtherDfeGrants;

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.OTHER_INCOME_GRANTS)]
        public decimal OtherIncomeGrants;

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.GOVERNMENT_SOURCE)]
        public decimal GovernmentSource;

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.ACADEMIES)]
        public decimal Academies;

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.NON_GOVERNMENT)]
        public decimal NonGoverntment;

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.INCOME_FROM_FACILITIES)]
        public decimal IncomeFromFacilities;

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.INCOME_FROM_CATERING)]
        public decimal IncomeFromCatering;

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.RECEIPTS_FROM_SUPPLY)]
        public decimal ReceiptsFromSupply;

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.DONATIONS)]
        public decimal Donations;

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.OTHER_SELF_GENERATED)]
        public decimal OtherSelfGenerated;

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.INVESTMENT_INCOME)]
        public decimal Investmentincome;

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.TEACHING_STAFF)]
        public decimal TeachingStaff;

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.SUPPLY_TEACHING_STAFF)]
        public decimal SupplyTeachingStaff;

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.EDUCATION_SUPPORT_STAFF)]
        public decimal EducationSupportStaff;

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.ADMIN_CLERIC_STAFF)]
        public decimal AdministrativeClericalStaff;

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.PREMISES_STAFF)]
        public decimal PremisesStaff;

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.CATERING_STAFF)]
        public decimal CateringStaff;

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.OTHER_STAFF)]
        public decimal OtherStaff;

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.INDIRECT_EMPLOYEE_EXPENSES)]
        public decimal IndirectEmployeeExpenses;

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.STAFF_DEV)]
        public decimal StaffDevelopment;

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.STAFFF_INSURANCE)]
        public decimal StaffInsurance;

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.SUPPLY_TEACHER_INSURANCE)]
        public decimal SupplyTeacherInsurance;

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.BUILDING_GROUNDS)]
        public decimal BuildingGroundsMaintenance;

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.CLEANING)]
        public decimal CleaningCaretaking;

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.WATER_SEWERAGE)]
        public decimal WaterSewerage;

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.ENERGY)]
        public decimal Energy;

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.RENT_RATES)]
        public decimal RentRates;

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.OTHER_OCCUPATION)]
        public decimal OtherOccupationCosts;

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.SPECIAL_FACILITIES)]
        public decimal Specialfacilities;

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.LEARNING_RESOURCES)]
        public decimal LearningResources;

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.ICT_LEARNING_RESOURCES)]
        public decimal ICTLearningResources;

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.EXAM_FEES)]
        public decimal ExaminationFees;

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.EDUCATIONAL_CONSULTANCY)]
        public decimal EducationalConsultancy;

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.ADMIN_SUPPLIES)]
        public decimal AdministrativeSupplies;

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.AGENCY_TEACH_STAFF)]
        public decimal AgencyTeachingStaff;

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.CATERING_SUPPLIES)]
        public decimal CateringSupplies;

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.OTHER_INSURANCE)]
        public decimal OtherInsurancePremiums;

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.LEGAL_PROFESSIONAL)]
        public decimal LegalProfessional;

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.AUDITOR_COSTS)]
        public decimal AuditorCosts;

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.INTEREST_CHARGES)]
        public decimal InterestCharges;

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.DIRECT_REVENUE)]
        public decimal DirectRevenue;

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.PFI_CHARGES)]
        public decimal PFICharges;

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.IN_YEAR_BALANCE)]
        public decimal InYearBalance;

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.GRANT_FUNDING)]
        public decimal GrantFunding;

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.DIRECT_GRANT)]
        public decimal DirectGrant;

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.COMMUNITY_GRANTS)]
        public decimal CommunityGrants;

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.TARGETED_GRANTS)]
        public decimal TargetedGrants;

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.SELF_GENERATED_FUNDING)]
        public decimal SelfGeneratedFunding;

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.TOTAL_INCOME)]
        public decimal TotalIncome;

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.SUPPLY_STAFF)]
        public decimal SupplyStaff;

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.OTHER_STAFF_COSTS)]
        public decimal OtherStaffCosts;

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.STAFF_TOTAL)]
        public decimal StaffTotal;

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.MAINTENANCE_IMPROVEMENT)]
        public decimal MaintenanceImprovement;

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.PREMISES)]
        public decimal Premises;

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.CATERING_EXP)]
        public decimal CateringExp;

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.OCCUPATION)]
        public decimal Occupation;

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.SUPPLIES_SERVICES)]
        public decimal SuppliesServices;

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.EDUCATIONAL_SUPPLIES)]
        public decimal EducationalSupplies;

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.BROUGHT_IN_SERVICES)]
        public decimal BroughtProfessionalServices;

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.COST_OF_FINANCE)]
        public decimal CostOfFinance;

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.TOTAL_EXP)]
        public decimal TotalExpenditure;

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.REVENUE_RESERVE)]
        public decimal RevenueReserve;

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.NO_PUPILS)]
        public double NoPupils;

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.NO_TEACHERS)]
        public double NoTeachers;

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.MEMBER_COUNT)]
        public int SchoolCount;

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.PERIOD_COVERED_BY_RETURN)]
        public int PeriodCoveredByReturn;

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.PARTIAL_YEARS_PRESENT)]
        public bool PartialYearsPresent;

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.SCHOOL_OVERALL_PHASE)]
        public string OverallPhase;

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.SCHOOL_PHASE)]
        public string Phase;

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.SCHOOL_OVERALL_PHASE_BREAKDOWN)]
        public Dictionary<string, int> OverallPhaseBreakdown;

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.DNS)]
        public bool DidNotSubmit;

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.MAT_SAT)]
        public string MATSATCentralServices;

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.WORKFORCE_PRESENT)]
        public bool WorkforcePresent;

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.ADMISSION_POLICY)]
        public string AdmissionPolicy;

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.GENDER)]
        public string Gender;

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.SCHOOL_TYPE)]
        public string Type;

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.URBAN_RURAL)]
        public string UrbanRural;

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.REGION)]
        public string Region;

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.LONDON_BOROUGH)]
        public string LondonBorough;

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.LONDON_WEIGHT)]
        public string LondonWeight;

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.PERCENTAGE_FSM)]
        public string PercentageFSM;

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.PERCENTAGE_OF_PUPILS_WITH_SEN)]
        public string PercentagePupilsWSEN;

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.PERCENTAGE_OF_PUPILS_WITHOUT_SEN)]
        public string PercentagePupilsWOSEN;

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.PERCENTAGE_OF_PUPILS_WITH_EAL)]
        public string PercentagePupilsWEAL;

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.PERCENTAGE_BOARDERS)]
        public string PercentageBoarders;

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.PFI)]
        public string PFI;

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.HAS_6_FORM)]
        public string Has6Form;

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.NUMBER_IN_6_FORM)]
        public string NumberIn6Form;

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.HIGHEST_AGE_PUPILS)]
        public string HighestAgePupils;

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.ADMIN_STAFF)]
        public string AdminStaff;

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.FULL_TIME_OTHER)]
        public string FullTimeOther;

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.PERCENTAGE_QUALIFIED_TEACHERS)]
        public string PercentageQualifiedTeachers;

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.LOWEST_AGE_PUPILS)]
        public string LowestAgePupils;

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.FULL_TIME_TA)]
        public string FullTimeTA;

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.WORKFORCE_TOTAL)]
        public string WorkforceTotal;

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.TEACHERS_TOTAL)]
        public string TeachersTotal;

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.TEACHERS_LEADER)]
        public string TeachersLeader;

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.KS2_ACTUAL)]
        public string Ks2Actual;

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.KS2_PROGRESS)]
        public string Ks2Progress;

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.AVERAGE_ATTAINMENT)]
        public string AverageAttainment;

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.PROGRESS_8_MEASURE)]
        public string Progress8Measure;

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.OFSTED_RATING_NAME)]
        public string OfstedRatingName;

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.SPECIFIC_LEARNING_DIFFICULTY)]
        public string SpecificLearningDiff;

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.MODERATE_LEARNING_DIFFICULTY)]
        public string ModerateLearningDiff;

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.SEVERE_LEARNING_DIFFICULTY)]
        public string SevereLearningDiff;

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.PROF_LEARNING_DIFFICULTY)]
        public string ProfLearningDiff;

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.SOCIAL_HEALTH)]
        public string SocialHealth;

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.SPEECH_NEEDS)]
        public string SpeechNeeds;

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.HEARING_IMPAIRMENT)]
        public string HearingImpairment;

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.VISUAL_IMPAIRMENT)]
        public string VisualImpairment;

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.MULTI_SENSORY_IMPAIRMENT)]
        public string MultiSensoryImpairment;

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.PHYSICAL_DISABILITY)]
        public string PhysicalDisability;

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.AUTISTIC_DISORDER)]
        public string AutisticDisorder;

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.OTHER_LEARNING_DIFF)]
        public string OtherLearningDiff;

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.LA)]
        public int LA ;


    }
}
