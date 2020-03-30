using Newtonsoft.Json;
using SFB.Web.ApplicationCore.Helpers.Constants;
using System.Collections.Generic;

namespace SFB.Web.ApplicationCore.Entities
{
    //TODO: some of these probably don't need to be decimal. Can be changed to double.
    //Also we need to decide if these fields can be non-nullable
    public class SchoolTrustFinancialDataObject
    {
        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.URN)]
        public int? URN{ get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.SCHOOL_NAME)]
        public string SchoolName { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.FINANCE_TYPE)]
        public string FinanceType { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.COMPANY_NUMBER)]
        public int? CompanyNumber { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.UID)]
        public int? UID { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.TRUST_COMPANY_NAME)]
        public string TrustOrCompanyName { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.OTHER_INCOME)]
        public decimal? OtherIncome { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.SEN)]
        public decimal? SEN { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.OTHER_DFE_GRANTS)]
        public decimal? OtherDfeGrants { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.OTHER_INCOME_GRANTS)]
        public decimal? OtherIncomeGrants { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.GOVERNMENT_SOURCE)]
        public decimal? GovernmentSource { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.ACADEMIES)]
        public decimal? Academies { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.NON_GOVERNMENT)]
        public decimal? NonGoverntment { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.INCOME_FROM_FACILITIES)]
        public decimal? IncomeFromFacilities { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.INCOME_FROM_CATERING)]
        public decimal? IncomeFromCatering { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.RECEIPTS_FROM_SUPPLY)]
        public decimal? ReceiptsFromSupply { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.RECEIPTS_FROM_OTHER)]
        public decimal? ReceiptsFromOther { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.DONATIONS)]
        public decimal? Donations { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.OTHER_SELF_GENERATED)]
        public decimal? OtherSelfGenerated { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.INVESTMENT_INCOME)]
        public decimal? Investmentincome { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.TEACHING_STAFF)]
        public decimal? TeachingStaff { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.SUPPLY_TEACHING_STAFF)]
        public decimal? SupplyTeachingStaff { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.EDUCATION_SUPPORT_STAFF)]
        public decimal? EducationSupportStaff { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.ADMIN_CLERIC_STAFF)]
        public decimal? AdministrativeClericalStaff { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.PREMISES_STAFF)]
        public decimal? PremisesStaff { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.CATERING_STAFF)]
        public decimal? CateringStaff { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.OTHER_STAFF)]
        public decimal? OtherStaff { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.INDIRECT_EMPLOYEE_EXPENSES)]
        public decimal? IndirectEmployeeExpenses { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.STAFF_DEV)]
        public decimal? StaffDevelopment { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.STAFF_INSURANCE)]
        public decimal? StaffInsurance { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.SUPPLY_TEACHER_INSURANCE)]
        public decimal? SupplyTeacherInsurance { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.BUILDING_GROUNDS)]
        public decimal? BuildingGroundsMaintenance { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.CLEANING)]
        public decimal? CleaningCaretaking { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.WATER_SEWERAGE)]
        public decimal? WaterSewerage { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.ENERGY)]
        public decimal? Energy { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.RATES)]
        public decimal? Rates { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.RENT_RATES)]
        public decimal? RentRates { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.OTHER_OCCUPATION)]
        public decimal? OtherOccupationCosts { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.SPECIAL_FACILITIES)]
        public decimal? Specialfacilities { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.LEARNING_RESOURCES)]
        public decimal? LearningResources { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.ICT_LEARNING_RESOURCES)]
        public decimal? ICTLearningResources { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.EXAM_FEES)]
        public decimal? ExaminationFees { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.EDUCATIONAL_CONSULTANCY)]
        public decimal? EducationalConsultancy { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.ADMIN_SUPPLIES)]
        public decimal? AdministrativeSupplies { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.AGENCY_TEACH_STAFF)]
        public decimal? AgencyTeachingStaff { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.CATERING_SUPPLIES)]
        public decimal? CateringSupplies { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.OTHER_INSURANCE)]
        public decimal? OtherInsurancePremiums { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.LEGAL_PROFESSIONAL)]
        public decimal? LegalProfessional { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.AUDITOR_COSTS)]
        public decimal? AuditorCosts { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.INTEREST_CHARGES)]
        public decimal? InterestCharges { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.DIRECT_REVENUE)]
        public decimal? DirectRevenue { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.PFI_CHARGES)]
        public decimal? PFICharges { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.IN_YEAR_BALANCE)]
        public decimal? InYearBalance { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.GRANT_FUNDING)]
        public decimal? GrantFunding { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.DIRECT_GRANT)]
        public decimal? DirectGrant { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.COMMUNITY_GRANTS)]
        public decimal? CommunityGrants { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.TARGETED_GRANTS)]
        public decimal? TargetedGrants { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.SELF_GENERATED_FUNDING)]
        public decimal? SelfGeneratedFunding { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.TOTAL_INCOME)]
        public decimal? TotalIncome { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.SUPPLY_STAFF)]
        public decimal? SupplyStaff { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.OTHER_STAFF_COSTS)]
        public decimal? OtherStaffCosts { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.STAFF_TOTAL)]
        public decimal? StaffTotal { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.MAINTENANCE_IMPROVEMENT)]
        public decimal? MaintenanceImprovement { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.GROUNDS_MAINTENANCE_IMPROVEMENT)]
        public decimal? GroundsMaintenanceImprovement { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.BUILDING_MAINTENANCE_IMPROVEMENT)]
        public decimal? BuildingMaintenanceImprovement { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.PREMISES)]
        public decimal? Premises { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.CATERING_EXP)]
        public decimal? CateringExp { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.OCCUPATION)]
        public decimal? Occupation { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.SUPPLIES_SERVICES)]
        public decimal? SuppliesServices { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.EDUCATIONAL_SUPPLIES)]
        public decimal? EducationalSupplies { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.BROUGHT_IN_SERVICES)]
        public decimal? BroughtProfessionalServices { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.BOUGHT_IN_OTHER)]
        public decimal? BoughtInOther { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.COMM_FOCUSED_STAFF)]
        public decimal? CommunityFocusedStaff { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.COMM_FOCUSED_SCHOOL)]
        public decimal? CommunityFocusedSchoolCosts { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.PRE_16_FUNDING)]
        public decimal? Pre16Funding { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.POST_16_FUNDING)]
        public decimal? Post16Funding { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.COMMUNITY_FOCUSED)]
        public decimal? CommunityFocusedFunding { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.ADDITIONAL_GRANT)]
        public decimal? AdditionalGrant { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.PUPIL_FOCUSED_FUNDING)]
        public decimal? PupilFocusedFunding { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.PUPIL_PREMIUM)]
        public decimal? PupilPremium { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.ESG)]
        public decimal? ESG { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.COST_OF_FINANCE)]
        public decimal? CostOfFinance { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.FUNDING_MINORITY)]
        public decimal? FundingMinority { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.COMMUNITY_EXP)]
        public decimal? CommunityExpenditure { get; set; }
        
        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.COMM_FOCUSED_SCHOOL_FACILITIES)]
        public decimal? CommFocusedSchoolFacilities { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.CONTRIBUTIONS_TO_VISITS)]
        public decimal? ContributionsToVisits { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.TOTAL_EXP)]
        public decimal? TotalExpenditure { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.REVENUE_RESERVE)]
        public decimal? RevenueReserve { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.NO_PUPILS)]
        public decimal? NoPupils { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.NO_TEACHERS)]
        public decimal? NoTeachers { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.MEMBER_COUNT)]
        public int? SchoolCount { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.PERIOD_COVERED_BY_RETURN)]
        public int? PeriodCoveredByReturn { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.PARTIAL_YEARS_PRESENT)]
        public bool? PartialYearsPresent { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.SCHOOL_OVERALL_PHASE)]
        public string OverallPhase { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.SCHOOL_PHASE)]
        public string Phase { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.SCHOOL_OVERALL_PHASE_BREAKDOWN)]
        public Dictionary<string, int> OverallPhaseBreakdown { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.DNS)]
        public bool DidNotSubmit { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.MAT_SAT)]
        public string MATSATCentralServices { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.WORKFORCE_PRESENT)]
        public bool WorkforcePresent { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.ADMISSION_POLICY)]
        public string AdmissionPolicy { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.GENDER)]
        public string Gender { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.SCHOOL_TYPE)]
        public string Type { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.URBAN_RURAL)]
        public string UrbanRural { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.REGION)]
        public string Region { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.LONDON_BOROUGH)]
        public string LondonBorough { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.LONDON_WEIGHT)]
        public string LondonWeight { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.PERCENTAGE_FSM)]
        public decimal? PercentageFSM { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.PERCENTAGE_OF_PUPILS_WITH_SEN)]
        public decimal? PercentagePupilsWSEN { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.PERCENTAGE_OF_PUPILS_WITHOUT_SEN)]
        public decimal? PercentagePupilsWOSEN { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.PERCENTAGE_OF_PUPILS_WITH_EAL)]
        public decimal? PercentagePupilsWEAL { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.PERCENTAGE_BOARDERS)]
        public decimal? PercentageBoarders { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.PFI)]
        public string PFI { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.HAS_6_FORM)]
        public string Has6Form { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.NUMBER_IN_6_FORM)]
        public decimal? NumberIn6Form { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.HIGHEST_AGE_PUPILS)]
        public int? HighestAgePupils { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.ADMIN_STAFF)]
        public decimal? AdminStaff { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.FULL_TIME_OTHER)]
        public decimal? FullTimeOther { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.FULL_TIME_OTHER_HC)]
        public decimal? FullTimeOtherHeadCount { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.PERCENTAGE_QUALIFIED_TEACHERS)]
        public decimal? PercentageQualifiedTeachers { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.LOWEST_AGE_PUPILS)]
        public int? LowestAgePupils { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.FULL_TIME_TA)]
        public decimal? FullTimeTA { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.FULL_TIME_TA_HEADCOUNT)]
        public decimal? FullTimeTAHeadcount { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.WORKFORCE_TOTAL)]
        public decimal? WorkforceTotal { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.TEACHERS_TOTAL)]
        public decimal? TeachersTotal { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.NUMBER_TEACHERS_HEADCOUNT)]
        public decimal? NumberTeachersHeadcount { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.NUMBER_TEACHERS_IN_LEADERSHIP)]
        public decimal? NumberTeachersInLeadershipHeadcount { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.TEACHERS_LEADER)]
        public decimal? TeachersLeader { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.AUX_STAFF)]
        public decimal? AuxStaff { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.AUX_STAFF_HC)]
        public decimal? AuxStaffHeadcount { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.WORKFORCE_HEADCOUNT)]
        public decimal? WorkforceHeadcount { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.KS2_ACTUAL)]
        public decimal? Ks2Actual { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.KS2_PROGRESS)]
        public decimal? Ks2Progress { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.AVERAGE_ATTAINMENT)]
        public decimal? AverageAttainment { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.PROGRESS_8_MEASURE)]
        public decimal? Progress8Measure { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.PROGRESS_8_BANDING)]
        public decimal? Progress8Banding { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.OFSTED_RATING_NAME)]
        public string OfstedRatingName { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.SPECIFIC_LEARNING_DIFFICULTY)]
        public decimal? SpecificLearningDiff { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.MODERATE_LEARNING_DIFFICULTY)]
        public decimal? ModerateLearningDiff { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.SEVERE_LEARNING_DIFFICULTY)]
        public decimal? SevereLearningDiff { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.PROF_LEARNING_DIFFICULTY)]
        public decimal? ProfLearningDiff { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.SOCIAL_HEALTH)]
        public decimal? SocialHealth { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.SPEECH_NEEDS)]
        public decimal? SpeechNeeds { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.HEARING_IMPAIRMENT)]
        public decimal? HearingImpairment { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.VISUAL_IMPAIRMENT)]
        public decimal? VisualImpairment { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.MULTI_SENSORY_IMPAIRMENT)]
        public decimal? MultiSensoryImpairment { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.PHYSICAL_DISABILITY)]
        public decimal? PhysicalDisability { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.AUTISTIC_DISORDER)]
        public decimal? AutisticDisorder { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.OTHER_LEARNING_DIFF)]
        public decimal? OtherLearningDiff { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.LA)]
        public int? LA  { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.OFSTED_RATING)]
        public decimal? OfstedRating { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.IS_PLACEHOLDER)]
        public bool IsPlaceholder { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.RR_TO_INCOME)]
        public decimal? RRPerIncomePercentage { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.GRANT_FUNDING_PP)]
        public decimal? PerPupilGrantFunding { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.TOTAL_EXP_PP)]
        public decimal? PerPupilTotalExpenditure { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.TEACHERS_MAIN_PAY)]
        public decimal? PerTeachersOnMainPay { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.TEACHERS_UPPER_LEADING_PAY)]
        public decimal? PerTeachersOnUpperOrLeadingPay { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDataFieldNames.TEACHERS_LEADERSHIP_PAY)]
        public decimal? PerTeachersOnLeadershipPay { get; set; }
    }
}
