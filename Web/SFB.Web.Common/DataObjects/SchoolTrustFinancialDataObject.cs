using Newtonsoft.Json;
using System.Collections.Generic;

namespace SFB.Web.Common.DataObjects
{
    //TODO: some of these probably don't need to be decimal. Can be changed to double.
    //Also we need to decide if these fields can be non-nullable
    public class SchoolTrustFinancialDataObject
    {
        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.URN)]
        public int? URN{ get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.SCHOOL_NAME)]
        public string SchoolName { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.FINANCE_TYPE)]
        public string FinanceType { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.MAT_NUMBER)]
        public string MATNumber { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.TRUST_COMPANY_NAME)]
        public string TrustOrCompanyName { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.OTHER_INCOME)]
        public decimal? OtherIncome { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.SEN)]
        public decimal? SEN { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.OTHER_DFE_GRANTS)]
        public decimal? OtherDfeGrants { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.OTHER_INCOME_GRANTS)]
        public decimal? OtherIncomeGrants { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.GOVERNMENT_SOURCE)]
        public decimal? GovernmentSource { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.ACADEMIES)]
        public decimal? Academies { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.NON_GOVERNMENT)]
        public decimal? NonGoverntment { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.INCOME_FROM_FACILITIES)]
        public decimal? IncomeFromFacilities { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.INCOME_FROM_CATERING)]
        public decimal? IncomeFromCatering { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.RECEIPTS_FROM_SUPPLY)]
        public decimal? ReceiptsFromSupply { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.RECEIPTS_FROM_OTHER)]
        public decimal? ReceiptsFromOther { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.DONATIONS)]
        public decimal? Donations { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.OTHER_SELF_GENERATED)]
        public decimal? OtherSelfGenerated { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.INVESTMENT_INCOME)]
        public decimal? Investmentincome { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.TEACHING_STAFF)]
        public decimal? TeachingStaff { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.SUPPLY_TEACHING_STAFF)]
        public decimal? SupplyTeachingStaff { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.EDUCATION_SUPPORT_STAFF)]
        public decimal? EducationSupportStaff { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.ADMIN_CLERIC_STAFF)]
        public decimal? AdministrativeClericalStaff { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.PREMISES_STAFF)]
        public decimal? PremisesStaff { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.CATERING_STAFF)]
        public decimal? CateringStaff { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.OTHER_STAFF)]
        public decimal? OtherStaff { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.INDIRECT_EMPLOYEE_EXPENSES)]
        public decimal? IndirectEmployeeExpenses { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.STAFF_DEV)]
        public decimal? StaffDevelopment { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.STAFF_INSURANCE)]
        public decimal? StaffInsurance { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.SUPPLY_TEACHER_INSURANCE)]
        public decimal? SupplyTeacherInsurance { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.BUILDING_GROUNDS)]
        public decimal? BuildingGroundsMaintenance { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.CLEANING)]
        public decimal? CleaningCaretaking { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.WATER_SEWERAGE)]
        public decimal? WaterSewerage { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.ENERGY)]
        public decimal? Energy { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.RATES)]
        public decimal? Rates { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.RENT_RATES)]
        public decimal? RentRates { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.OTHER_OCCUPATION)]
        public decimal? OtherOccupationCosts { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.SPECIAL_FACILITIES)]
        public decimal? Specialfacilities { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.LEARNING_RESOURCES)]
        public decimal? LearningResources { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.ICT_LEARNING_RESOURCES)]
        public decimal? ICTLearningResources { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.EXAM_FEES)]
        public decimal? ExaminationFees { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.EDUCATIONAL_CONSULTANCY)]
        public decimal? EducationalConsultancy { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.ADMIN_SUPPLIES)]
        public decimal? AdministrativeSupplies { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.AGENCY_TEACH_STAFF)]
        public decimal? AgencyTeachingStaff { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.CATERING_SUPPLIES)]
        public decimal? CateringSupplies { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.OTHER_INSURANCE)]
        public decimal? OtherInsurancePremiums { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.LEGAL_PROFESSIONAL)]
        public decimal? LegalProfessional { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.AUDITOR_COSTS)]
        public decimal? AuditorCosts { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.INTEREST_CHARGES)]
        public decimal? InterestCharges { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.DIRECT_REVENUE)]
        public decimal? DirectRevenue { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.PFI_CHARGES)]
        public decimal? PFICharges { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.IN_YEAR_BALANCE)]
        public decimal? InYearBalance { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.GRANT_FUNDING)]
        public decimal? GrantFunding { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.DIRECT_GRANT)]
        public decimal? DirectGrant { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.COMMUNITY_GRANTS)]
        public decimal? CommunityGrants { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.TARGETED_GRANTS)]
        public decimal? TargetedGrants { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.SELF_GENERATED_FUNDING)]
        public decimal? SelfGeneratedFunding { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.TOTAL_INCOME)]
        public decimal? TotalIncome { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.SUPPLY_STAFF)]
        public decimal? SupplyStaff { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.OTHER_STAFF_COSTS)]
        public decimal? OtherStaffCosts { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.STAFF_TOTAL)]
        public decimal? StaffTotal { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.MAINTENANCE_IMPROVEMENT)]
        public decimal? MaintenanceImprovement { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.GROUNDS_MAINTENANCE_IMPROVEMENT)]
        public decimal? GroundsMaintenanceImprovement { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.BUILDING_MAINTENANCE_IMPROVEMENT)]
        public decimal? BuildingMaintenanceImprovement { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.PREMISES)]
        public decimal? Premises { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.CATERING_EXP)]
        public decimal? CateringExp { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.OCCUPATION)]
        public decimal? Occupation { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.SUPPLIES_SERVICES)]
        public decimal? SuppliesServices { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.EDUCATIONAL_SUPPLIES)]
        public decimal? EducationalSupplies { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.BROUGHT_IN_SERVICES)]
        public decimal? BroughtProfessionalServices { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.BOUGHT_IN_OTHER)]
        public decimal? BoughtInOther { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.COMM_FOCUSED_STAFF)]
        public decimal? CommunityFocusedStaff { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.COMM_FOCUSED_SCHOOL)]
        public decimal? CommunityFocusedSchoolCosts { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.PRE_16_FUNDING)]
        public decimal? Pre16Funding { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.POST_16_FUNDING)]
        public decimal? Post16Funding { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.COMMUNITY_FOCUSED)]
        public decimal? CommunityFocusedFunding { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.ADDITIONAL_GRANT)]
        public decimal? AdditionalGrant { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.PUPIL_FOCUSED_FUNDING)]
        public decimal? PupilFocusedFunding { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.PUPIL_PREMIUM)]
        public decimal? PupilPremium { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.ESG)]
        public decimal? ESG { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.COST_OF_FINANCE)]
        public decimal? CostOfFinance { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.FUNDING_MINORITY)]
        public decimal? FundingMinority { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.COMMUNITY_EXP)]
        public decimal? CommunityExpenditure { get; set; }
        
        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.COMM_FOCUSED_SCHOOL_FACILITIES)]
        public decimal? CommFocusedSchoolFacilities { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.CONTRIBUTIONS_TO_VISITS)]
        public decimal? ContributionsToVisits { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.TOTAL_EXP)]
        public decimal? TotalExpenditure { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.REVENUE_RESERVE)]
        public decimal? RevenueReserve { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.NO_PUPILS)]
        public decimal? NoPupils { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.NO_TEACHERS)]
        public decimal? NoTeachers { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.MEMBER_COUNT)]
        public int? SchoolCount { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.PERIOD_COVERED_BY_RETURN)]
        public int? PeriodCoveredByReturn { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.PARTIAL_YEARS_PRESENT)]
        public bool PartialYearsPresent { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.SCHOOL_OVERALL_PHASE)]
        public string OverallPhase { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.SCHOOL_PHASE)]
        public string Phase { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.SCHOOL_OVERALL_PHASE_BREAKDOWN)]
        public Dictionary<string, int> OverallPhaseBreakdown { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.DNS)]
        public bool DidNotSubmit { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.MAT_SAT)]
        public string MATSATCentralServices { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.WORKFORCE_PRESENT)]
        public bool WorkforcePresent { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.ADMISSION_POLICY)]
        public string AdmissionPolicy { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.GENDER)]
        public string Gender { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.SCHOOL_TYPE)]
        public string Type { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.URBAN_RURAL)]
        public string UrbanRural { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.REGION)]
        public string Region { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.LONDON_BOROUGH)]
        public string LondonBorough { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.LONDON_WEIGHT)]
        public string LondonWeight { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.PERCENTAGE_FSM)]
        public decimal? PercentageFSM { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.PERCENTAGE_OF_PUPILS_WITH_SEN)]
        public decimal? PercentagePupilsWSEN { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.PERCENTAGE_OF_PUPILS_WITHOUT_SEN)]
        public decimal? PercentagePupilsWOSEN { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.PERCENTAGE_OF_PUPILS_WITH_EAL)]
        public decimal? PercentagePupilsWEAL { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.PERCENTAGE_BOARDERS)]
        public decimal? PercentageBoarders { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.PFI)]
        public string PFI { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.HAS_6_FORM)]
        public string Has6Form { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.NUMBER_IN_6_FORM)]
        public decimal? NumberIn6Form { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.HIGHEST_AGE_PUPILS)]
        public int? HighestAgePupils { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.ADMIN_STAFF)]
        public decimal? AdminStaff { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.FULL_TIME_OTHER)]
        public decimal? FullTimeOther { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.FULL_TIME_OTHER_HC)]
        public decimal? FullTimeOtherHeadCount { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.PERCENTAGE_QUALIFIED_TEACHERS)]
        public decimal? PercentageQualifiedTeachers { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.LOWEST_AGE_PUPILS)]
        public int? LowestAgePupils { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.FULL_TIME_TA)]
        public decimal? FullTimeTA { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.FULL_TIME_TA_HEADCOUNT)]
        public decimal? FullTimeTAHeadcount { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.WORKFORCE_TOTAL)]
        public decimal WorkforceTotal { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.TEACHERS_TOTAL)]
        public decimal TeachersTotal { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.NUMBER_TEACHERS_HEADCOUNT)]
        public decimal NumberTeachersHeadcount { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.NUMBER_TEACHERS_IN_LEADERSHIP)]
        public decimal? NumberTeachersInLeadershipHeadcount { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.TEACHERS_LEADER)]
        public decimal? TeachersLeader { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.AUX_STAFF)]
        public decimal? AuxStaff { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.AUX_STAFF_HC)]
        public decimal? AuxStaffHeadcount { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.WORKFORCE_HEADCOUNT)]
        public decimal? WorkforceHeadcount { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.KS2_ACTUAL)]
        public decimal? Ks2Actual { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.KS2_PROGRESS)]
        public decimal? Ks2Progress { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.AVERAGE_ATTAINMENT)]
        public decimal? AverageAttainment { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.PROGRESS_8_MEASURE)]
        public decimal? Progress8Measure { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.OFSTED_RATING_NAME)]
        public string OfstedRatingName { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.SPECIFIC_LEARNING_DIFFICULTY)]
        public decimal? SpecificLearningDiff { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.MODERATE_LEARNING_DIFFICULTY)]
        public decimal? ModerateLearningDiff { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.SEVERE_LEARNING_DIFFICULTY)]
        public decimal? SevereLearningDiff { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.PROF_LEARNING_DIFFICULTY)]
        public decimal? ProfLearningDiff { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.SOCIAL_HEALTH)]
        public decimal? SocialHealth { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.SPEECH_NEEDS)]
        public decimal? SpeechNeeds { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.HEARING_IMPAIRMENT)]
        public decimal? HearingImpairment { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.VISUAL_IMPAIRMENT)]
        public decimal? VisualImpairment { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.MULTI_SENSORY_IMPAIRMENT)]
        public decimal? MultiSensoryImpairment { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.PHYSICAL_DISABILITY)]
        public decimal? PhysicalDisability { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.AUTISTIC_DISORDER)]
        public decimal? AutisticDisorder { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.OTHER_LEARNING_DIFF)]
        public decimal? OtherLearningDiff { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.LA)]
        public int? LA  { get; set; }

        [JsonProperty(PropertyName = SchoolTrustFinanceDBFieldNames.OFSTED_RATING)]
        public decimal? OfstedRating { get; set; }
    }
}
