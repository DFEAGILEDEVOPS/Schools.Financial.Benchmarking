using System;
using System.ComponentModel.DataAnnotations;
using SFB.Web.ApplicationCore.Helpers.Constants;
using SFB.Web.ApplicationCore.Attributes;

namespace SFB.Web.ApplicationCore.Models
{
    public class BenchmarkCriteria
    {
        [PrettyName(SchoolCharacteristicsQuestions.LA_CODE)]
        [DBField(name: SchoolTrustFinanceDataFieldNames.LA, type: CriteriaFieldComparisonTypes.EQUALTO)]
        public int? LocalAuthorityCode { get; set; }

        [PrettyName(SchoolCharacteristicsQuestions.PFI)]
        [DBField(name: SchoolTrustFinanceDataFieldNames.PFI, type: CriteriaFieldComparisonTypes.EQUALTO)]
        public string[] Pfi { get; set; }

        [PrettyName(SchoolCharacteristicsQuestions.DOES_THE_SCHOOL_HAVE6_FORM)]
        [DBField(name: SchoolTrustFinanceDataFieldNames.HAS_6_FORM, type: CriteriaFieldComparisonTypes.EQUALTO)]
        public string SixthForm { get; set; }

        [PrettyName(SchoolCharacteristicsQuestions.GENDER_OF_PUPILS)]
        [DBField(name: SchoolTrustFinanceDataFieldNames.GENDER, type: CriteriaFieldComparisonTypes.EQUALTO)]
        public string[] Gender { get; set; }

        [PrettyName(SchoolCharacteristicsQuestions.SCHOOL_OVERALL_PHASE)]
        [DBField(name: SchoolTrustFinanceDataFieldNames.SCHOOL_OVERALL_PHASE, type: CriteriaFieldComparisonTypes.EQUALTO)]
        public string[] SchoolOverallPhase { get; set; }

        [PrettyName(SchoolCharacteristicsQuestions.SCHOOL_PHASE)]
        [DBField(name: SchoolTrustFinanceDataFieldNames.SCHOOL_PHASE, type: CriteriaFieldComparisonTypes.EQUALTO)]
        public string[] SchoolPhase { get; set; }

        [PrettyName(SchoolCharacteristicsQuestions.PERIOD_COVERED_BY_RETURN)]
        [DBField(name: SchoolTrustFinanceDataFieldNames.PERIOD_COVERED_BY_RETURN, type: CriteriaFieldComparisonTypes.EQUALTO)]
        public int? PeriodCoveredByReturn { get; set; }

        [PrettyName(SchoolCharacteristicsQuestions.TYPEOF_ESTABLISHMENT)]
        [DBField(name: SchoolTrustFinanceDataFieldNames.SCHOOL_TYPE, type: CriteriaFieldComparisonTypes.EQUALTO)]
        public string[] TypeOfEstablishment { get; set; }

        [PrettyName(SchoolCharacteristicsQuestions.URBAN_RURAL)]
        [DBField(name: SchoolTrustFinanceDataFieldNames.URBAN_RURAL, type: CriteriaFieldComparisonTypes.EQUALTO)]
        public string[] UrbanRural { get; set; }

        [PrettyName(SchoolCharacteristicsQuestions.GOVERNMENT_OFFICE)]
        [DBField(name: SchoolTrustFinanceDataFieldNames.REGION, type: CriteriaFieldComparisonTypes.EQUALTO)]
        public string[] GovernmentOffice { get; set; }

        [PrettyName(SchoolCharacteristicsQuestions.LONDON_WEIGHTING)]
        [DBField(name: SchoolTrustFinanceDataFieldNames.LONDON_WEIGHT, type: CriteriaFieldComparisonTypes.EQUALTO)]
        public string[] LondonWeighting { get; set; }
        
        [PrettyName(SchoolCharacteristicsQuestions.NUMBER_OF_PUPILS)]
        [Range(0, Int32.MaxValue)]
        [DBField(name: SchoolTrustFinanceDataFieldNames.NO_PUPILS, type: CriteriaFieldComparisonTypes.MIN)]
        public decimal? MinNoPupil { get; set; }

        [PrettyName(SchoolCharacteristicsQuestions.NUMBER_OF_PUPILS)]
        [Range(0, Int32.MaxValue)]
        [DBField(name: SchoolTrustFinanceDataFieldNames.NO_PUPILS, type: CriteriaFieldComparisonTypes.MAX)]
        public decimal? MaxNoPupil { get; set; }

        [PrettyName(SchoolCharacteristicsQuestions.PERCENTAGE_OF_ELIGIBLE_FREE_SCHOOL_MEALS)]
        [Range(0,100)]
        [DBField(name: SchoolTrustFinanceDataFieldNames.PERCENTAGE_FSM, type: CriteriaFieldComparisonTypes.MIN)]
        public decimal? MinPerFSM { get; set; }

        [PrettyName(SchoolCharacteristicsQuestions.PERCENTAGE_OF_ELIGIBLE_FREE_SCHOOL_MEALS)]
        [Range(0, 100)]
        [DBField(name: SchoolTrustFinanceDataFieldNames.PERCENTAGE_FSM, type: CriteriaFieldComparisonTypes.MAX)]
        public decimal? MaxPerFSM { get; set; }

        [PrettyName(SchoolCharacteristicsQuestions.PERCENTAGE_OF_PUPILS_WITH_STATEMENT_OF_SEN)]
        [Range(0, 100)]
        [DBField(name: SchoolTrustFinanceDataFieldNames.PERCENTAGE_OF_PUPILS_WITH_SEN, type: CriteriaFieldComparisonTypes.MIN)]
        public decimal? MinPerSEN { get; set; }

        [PrettyName(SchoolCharacteristicsQuestions.PERCENTAGE_OF_PUPILS_WITH_STATEMENT_OF_SEN)]
        [Range(0, 100)]
        [DBField(name: SchoolTrustFinanceDataFieldNames.PERCENTAGE_OF_PUPILS_WITH_SEN, type: CriteriaFieldComparisonTypes.MAX)]
        public decimal? MaxPerSEN { get; set; }

        [PrettyName(SchoolCharacteristicsQuestions.PERCENTAGE_OF_PUPILS_ON_SEN_REGISTER)]
        [Range(0, 100)]
        [DBField(name: SchoolTrustFinanceDataFieldNames.PERCENTAGE_OF_PUPILS_WITHOUT_SEN, type: CriteriaFieldComparisonTypes.MIN)]
        public decimal? MinPerSENReg { get; set; }

        [PrettyName(SchoolCharacteristicsQuestions.PERCENTAGE_OF_PUPILS_ON_SEN_REGISTER)]
        [Range(0, 100)]
        [DBField(name: SchoolTrustFinanceDataFieldNames.PERCENTAGE_OF_PUPILS_WITHOUT_SEN, type: CriteriaFieldComparisonTypes.MAX)]
        public decimal? MaxPerSENReg { get; set; }

        [PrettyName(SchoolCharacteristicsQuestions.PERCENTAGE_OF_PUPILS_WITH_EAL)]
        [Range(0, 100)]
        [DBField(name: SchoolTrustFinanceDataFieldNames.PERCENTAGE_OF_PUPILS_WITH_EAL, type: CriteriaFieldComparisonTypes.MIN)]
        public decimal? MinPerEAL { get; set; }

        [PrettyName(SchoolCharacteristicsQuestions.PERCENTAGE_OF_PUPILS_WITH_EAL)]
        [Range(0, 100)]
        [DBField(name: SchoolTrustFinanceDataFieldNames.PERCENTAGE_OF_PUPILS_WITH_EAL, type: CriteriaFieldComparisonTypes.MAX)]
        public decimal? MaxPerEAL { get; set; }

        [PrettyName(SchoolCharacteristicsQuestions.PERCENTAGE_BOARDERS)]
        [Range(0, 100)]
        [DBField(name: SchoolTrustFinanceDataFieldNames.PERCENTAGE_BOARDERS, type: CriteriaFieldComparisonTypes.MIN)]
        public decimal? MinPerBoarders { get; set; }

        [PrettyName(SchoolCharacteristicsQuestions.PERCENTAGE_BOARDERS)]
        [Range(0, 100)]
        [DBField(name: SchoolTrustFinanceDataFieldNames.PERCENTAGE_BOARDERS, type: CriteriaFieldComparisonTypes.MAX)]
        public decimal? MaxPerBoarders { get; set; }

        [PrettyName(SchoolCharacteristicsQuestions.ADMISSIONS_POLICY)]
        [DBField(name: SchoolTrustFinanceDataFieldNames.ADMISSION_POLICY, type: CriteriaFieldComparisonTypes.EQUALTO)]
        public string[] AdmPolicy { get; set; }

        [PrettyName(SchoolCharacteristicsQuestions.NUMBER_IN6_FORM)]
        [Range(0, Int32.MaxValue)]
        [DBField(name: SchoolTrustFinanceDataFieldNames.NUMBER_IN_6_FORM, type: CriteriaFieldComparisonTypes.MIN)]
        public decimal? MinNoSixthForm { get; set; }

        [PrettyName(SchoolCharacteristicsQuestions.NUMBER_IN6_FORM)]
        [Range(0, Int32.MaxValue)]
        [DBField(name: SchoolTrustFinanceDataFieldNames.NUMBER_IN_6_FORM, type: CriteriaFieldComparisonTypes.MAX)]
        public decimal? MaxNoSixthForm { get; set; }

        [PrettyName(SchoolCharacteristicsQuestions.LOWEST_AGE_PUPILS)]
        [Range(0, Int32.MaxValue)]
        [DBField(name: SchoolTrustFinanceDataFieldNames.LOWEST_AGE_PUPILS, type: CriteriaFieldComparisonTypes.MIN)]
        public decimal? MinLowestAgePupils { get; set; }

        [PrettyName(SchoolCharacteristicsQuestions.LOWEST_AGE_PUPILS)]
        [Range(0, Int32.MaxValue)]
        [DBField(name: SchoolTrustFinanceDataFieldNames.LOWEST_AGE_PUPILS, type: CriteriaFieldComparisonTypes.MAX)]
        public decimal? MaxLowestAgePupils { get; set; }

        [PrettyName(SchoolCharacteristicsQuestions.HIGHEST_AGE_PUPILS)]
        [Range(0, Int32.MaxValue)]
        [DBField(name: SchoolTrustFinanceDataFieldNames.HIGHEST_AGE_PUPILS, type: CriteriaFieldComparisonTypes.MIN)]
        public decimal? MinHighestAgePupils { get; set; }

        [PrettyName(SchoolCharacteristicsQuestions.HIGHEST_AGE_PUPILS)]
        [Range(0, Int32.MaxValue)]
        [DBField(name: SchoolTrustFinanceDataFieldNames.HIGHEST_AGE_PUPILS, type: CriteriaFieldComparisonTypes.MAX)]
        public decimal? MaxHighestAgePupils { get; set; }

        [PrettyName(SchoolCharacteristicsQuestions.PERCENTAGE_QUALIFIED_TEACHERS)]
        [Range(0, 100)]
        [DBField(name: SchoolTrustFinanceDataFieldNames.PERCENTAGE_QUALIFIED_TEACHERS, type: CriteriaFieldComparisonTypes.MIN)]
        public decimal? MinTeachersWithQualifiedTeacherStatus { get; set; }

        [PrettyName(SchoolCharacteristicsQuestions.PERCENTAGE_QUALIFIED_TEACHERS)]
        [Range(0, 100)]
        [DBField(name: SchoolTrustFinanceDataFieldNames.PERCENTAGE_QUALIFIED_TEACHERS, type: CriteriaFieldComparisonTypes.MAX)]
        public decimal? MaxTeachersWithQualifiedTeacherStatus { get; set; }

        [PrettyName(SchoolCharacteristicsQuestions.FULL_TIME_TA)]
        [Range(0, Int32.MaxValue)]
        [DBField(name: SchoolTrustFinanceDataFieldNames.FULL_TIME_TA, type: CriteriaFieldComparisonTypes.MIN)]
        public decimal? MinTotalNumberOfTeachingAssistantsFullTimeEquivalent { get; set; }

        [PrettyName(SchoolCharacteristicsQuestions.FULL_TIME_TA)]
        [Range(0, Int32.MaxValue)]
        [DBField(name: SchoolTrustFinanceDataFieldNames.FULL_TIME_TA, type: CriteriaFieldComparisonTypes.MAX)]
        public decimal? MaxTotalNumberOfTeachingAssistantsFullTimeEquivalent { get; set; }

        [PrettyName(SchoolCharacteristicsQuestions.FULL_TIME_OTHER)]
        [Range(0, Int32.MaxValue)]
        [DBField(name: SchoolTrustFinanceDataFieldNames.FULL_TIME_OTHER, type: CriteriaFieldComparisonTypes.MIN)]
        public decimal? MinFullTimeOther { get; set; }

        [PrettyName(SchoolCharacteristicsQuestions.FULL_TIME_OTHER)]
        [Range(0, Int32.MaxValue)]
        [DBField(name: SchoolTrustFinanceDataFieldNames.FULL_TIME_OTHER, type: CriteriaFieldComparisonTypes.MAX)]
        public decimal? MaxFullTimeOther { get; set; }

        [PrettyName(SchoolCharacteristicsQuestions.FULL_TIME_AUX)]
        [Range(0, Int32.MaxValue)]
        [DBField(name: SchoolTrustFinanceDataFieldNames.AUX_STAFF, type: CriteriaFieldComparisonTypes.MIN)]
        public decimal? MinFullTimeAux { get; set; }

        [PrettyName(SchoolCharacteristicsQuestions.FULL_TIME_AUX)]
        [Range(0, Int32.MaxValue)]
        [DBField(name: SchoolTrustFinanceDataFieldNames.AUX_STAFF, type: CriteriaFieldComparisonTypes.MAX)]
        public decimal? MaxFullTimeAux { get; set; }

        [PrettyName(SchoolCharacteristicsQuestions.SCHOOL_WORKFORCE_FTE)]
        [Range(0, Int32.MaxValue)]
        [DBField(name: SchoolTrustFinanceDataFieldNames.WORKFORCE_TOTAL, type: CriteriaFieldComparisonTypes.MIN)]
        public decimal? MinTotalSchoolWorkforceFullTimeEquivalent { get; set; }

        [PrettyName(SchoolCharacteristicsQuestions.SCHOOL_WORKFORCE_FTE)]
        [Range(0, Int32.MaxValue)]
        [DBField(name: SchoolTrustFinanceDataFieldNames.WORKFORCE_TOTAL, type: CriteriaFieldComparisonTypes.MAX)]
        public decimal? MaxTotalSchoolWorkforceFullTimeEquivalent { get; set; }

        [PrettyName(SchoolCharacteristicsQuestions.NUMBER_OF_TEACHERS_FTE)]
        [Range(0, Int32.MaxValue)]
        [DBField(name: SchoolTrustFinanceDataFieldNames.TEACHERS_TOTAL, type: CriteriaFieldComparisonTypes.MIN)]
        public decimal? MinTotalNumberOfTeachersFullTimeEquivalent { get; set; }

        [PrettyName(SchoolCharacteristicsQuestions.NUMBER_OF_TEACHERS_FTE)]
        [Range(0, Int32.MaxValue)]
        [DBField(name: SchoolTrustFinanceDataFieldNames.TEACHERS_TOTAL, type: CriteriaFieldComparisonTypes.MAX)]
        public decimal? MaxTotalNumberOfTeachersFullTimeEquivalent { get; set; }

        [PrettyName(SchoolCharacteristicsQuestions.SENIOR_LEADERSHIP_FTE)]
        [Range(0, Int32.MaxValue)]
        [DBField(name: SchoolTrustFinanceDataFieldNames.TEACHERS_LEADER, type: CriteriaFieldComparisonTypes.MIN)]
        public decimal? MinTotalNumberOfTeachersInTheLeadershipGroupFullTimeEquivalent { get; set; }

        [PrettyName(SchoolCharacteristicsQuestions.SENIOR_LEADERSHIP_FTE)]
        [Range(0, Int32.MaxValue)]
        [DBField(name: SchoolTrustFinanceDataFieldNames.TEACHERS_LEADER, type: CriteriaFieldComparisonTypes.MAX)]
        public decimal? MaxTotalNumberOfTeachersInTheLeadershipGroupFullTimeEquivalent { get; set; }

        [PrettyName(SchoolCharacteristicsQuestions.KS2_ACTUAL)]
        [Range(0, 100)]
        [DBField(name: SchoolTrustFinanceDataFieldNames.KS2_ACTUAL, type: CriteriaFieldComparisonTypes.MIN)]
        public decimal? MinKs2Actual { get; set; }

        [PrettyName(SchoolCharacteristicsQuestions.KS2_ACTUAL)]
        [Range(0, 100)]
        [DBField(name: SchoolTrustFinanceDataFieldNames.KS2_ACTUAL, type: CriteriaFieldComparisonTypes.MAX)]
        public decimal? MaxKs2Actual { get; set; }

        [PrettyName(SchoolCharacteristicsQuestions.KS2_PROGRESS)]
        [Range(-20, 20)]
        [DBField(name: SchoolTrustFinanceDataFieldNames.KS2_PROGRESS, type: CriteriaFieldComparisonTypes.MIN)]
        public decimal? MinKs2Progress { get; set; }

        [PrettyName(SchoolCharacteristicsQuestions.KS2_PROGRESS)]
        [Range(-20, 20)]
        [DBField(name: SchoolTrustFinanceDataFieldNames.KS2_PROGRESS, type: CriteriaFieldComparisonTypes.MAX)]
        public decimal? MaxKs2Progress { get; set; }

        [PrettyName(SchoolCharacteristicsQuestions.AVERAGE_ATTAINMENT_8)]
        [Range(0, 100)]
        [DBField(name: SchoolTrustFinanceDataFieldNames.AVERAGE_ATTAINMENT, type: CriteriaFieldComparisonTypes.MIN)]
        public decimal? MinAvAtt8 { get; set; }

        [PrettyName(SchoolCharacteristicsQuestions.AVERAGE_ATTAINMENT_8)]
        [Range(0, 100)]
        [DBField(name: SchoolTrustFinanceDataFieldNames.AVERAGE_ATTAINMENT, type: CriteriaFieldComparisonTypes.MAX)]
        public decimal? MaxAvAtt8 { get; set; }

        [PrettyName(SchoolCharacteristicsQuestions.PROGRESS_8_MEASURE)]
        [Range(-5, 5)]
        [DBField(name: SchoolTrustFinanceDataFieldNames.PROGRESS_8_MEASURE, type: CriteriaFieldComparisonTypes.MIN)]
        public decimal? MinP8Mea { get; set; }

        [PrettyName(SchoolCharacteristicsQuestions.PROGRESS_8_MEASURE)]
        [Range(-5, 5)]
        [DBField(name: SchoolTrustFinanceDataFieldNames.PROGRESS_8_MEASURE, type: CriteriaFieldComparisonTypes.MAX)]
        public decimal? MaxP8Mea { get; set; }

        [PrettyName(SchoolCharacteristicsQuestions.OFSTED_RATING)]
        [DBField(name: SchoolTrustFinanceDataFieldNames.OFSTED_RATING_NAME, type: CriteriaFieldComparisonTypes.EQUALTO)]
        public string[] OfstedRating { get; set; }

        [PrettyName(SchoolCharacteristicsQuestions.RR_TO_INCOME)]
        [DBField(name: SchoolTrustFinanceDataFieldNames.RR_TO_INCOME, type: CriteriaFieldComparisonTypes.MIN)]
        public decimal? MinRRToIncome { get; set; }

        [PrettyName(SchoolCharacteristicsQuestions.PER_PUPIL_EXP)]
        [DBField(name: SchoolTrustFinanceDataFieldNames.TOTAL_EXP_PP, type: CriteriaFieldComparisonTypes.MIN)]
        public decimal? MinPerPupilExp { get; set; }

        [PrettyName(SchoolCharacteristicsQuestions.PER_PUPIL_EXP)]
        [DBField(name: SchoolTrustFinanceDataFieldNames.TOTAL_EXP_PP, type: CriteriaFieldComparisonTypes.MAX)]
        public decimal? MaxPerPupilExp { get; set; }

        [PrettyName(SchoolCharacteristicsQuestions.PER_PUPIL_GF)]
        [DBField(name: SchoolTrustFinanceDataFieldNames.GRANT_FUNDING_PP, type: CriteriaFieldComparisonTypes.MIN)]
        public decimal? MinPerPupilGrantFunding { get; set; }

        [PrettyName(SchoolCharacteristicsQuestions.PER_PUPIL_GF)]
        [DBField(name: SchoolTrustFinanceDataFieldNames.GRANT_FUNDING_PP, type: CriteriaFieldComparisonTypes.MAX)]
        public decimal? MaxPerPupilGrantFunding { get; set; }

        [PrettyName(SchoolCharacteristicsQuestions.SPECIFIC_LEARNING_DIFFICULTY)]
        [Range(0, 100)]
        [DBField(name: SchoolTrustFinanceDataFieldNames.SPECIFIC_LEARNING_DIFFICULTY, type: CriteriaFieldComparisonTypes.MIN)]
        public decimal? MinSpecLearnDiff { get; set; }

        [PrettyName(SchoolCharacteristicsQuestions.SPECIFIC_LEARNING_DIFFICULTY)]
        [Range(0, 100)]
        [DBField(name: SchoolTrustFinanceDataFieldNames.SPECIFIC_LEARNING_DIFFICULTY, type: CriteriaFieldComparisonTypes.MAX)]
        public decimal? MaxSpecLearnDiff { get; set; }

        [PrettyName(SchoolCharacteristicsQuestions.MODERATE_LEARNING_DIFFICULTY)]
        [Range(0, 100)]
        [DBField(name: SchoolTrustFinanceDataFieldNames.MODERATE_LEARNING_DIFFICULTY, type: CriteriaFieldComparisonTypes.MIN)]
        public decimal? MinModLearnDiff { get; set; }

        [PrettyName(SchoolCharacteristicsQuestions.MODERATE_LEARNING_DIFFICULTY)]
        [Range(0, 100)]
        [DBField(name: SchoolTrustFinanceDataFieldNames.MODERATE_LEARNING_DIFFICULTY, type: CriteriaFieldComparisonTypes.MAX)]
        public decimal? MaxModLearnDiff { get; set; }

        [PrettyName(SchoolCharacteristicsQuestions.SEVERE_LEARNING_DIFFICULTY)]
        [Range(0, 100)]
        [DBField(name: SchoolTrustFinanceDataFieldNames.SEVERE_LEARNING_DIFFICULTY, type: CriteriaFieldComparisonTypes.MIN)]
        public decimal? MinSevLearnDiff { get; set; }

        [PrettyName(SchoolCharacteristicsQuestions.SEVERE_LEARNING_DIFFICULTY)]
        [Range(0, 100)]
        [DBField(name: SchoolTrustFinanceDataFieldNames.SEVERE_LEARNING_DIFFICULTY, type: CriteriaFieldComparisonTypes.MAX)]
        public decimal? MaxSevLearnDiff { get; set; }

        [PrettyName(SchoolCharacteristicsQuestions.PROF_LEARNING_DIFFICULTY)]
        [Range(0, 100)]
        [DBField(name: SchoolTrustFinanceDataFieldNames.PROF_LEARNING_DIFFICULTY, type: CriteriaFieldComparisonTypes.MIN)]
        public decimal? MinProfLearnDiff { get; set; }

        [PrettyName(SchoolCharacteristicsQuestions.PROF_LEARNING_DIFFICULTY)]
        [Range(0, 100)]
        [DBField(name: SchoolTrustFinanceDataFieldNames.PROF_LEARNING_DIFFICULTY, type: CriteriaFieldComparisonTypes.MAX)]
        public decimal? MaxProfLearnDiff { get; set; }

        [PrettyName(SchoolCharacteristicsQuestions.SOCIAL_HEALTH)]
        [Range(0, 100)]
        [DBField(name: SchoolTrustFinanceDataFieldNames.SOCIAL_HEALTH, type: CriteriaFieldComparisonTypes.MIN)]
        public decimal? MinSocialHealth { get; set; }

        [PrettyName(SchoolCharacteristicsQuestions.SOCIAL_HEALTH)]
        [Range(0, 100)]
        [DBField(name: SchoolTrustFinanceDataFieldNames.SOCIAL_HEALTH, type: CriteriaFieldComparisonTypes.MAX)]
        public decimal? MaxSocialHealth { get; set; }

        [PrettyName(SchoolCharacteristicsQuestions.SPEECH_NEEDS)]
        [Range(0, 100)]
        [DBField(name: SchoolTrustFinanceDataFieldNames.SPEECH_NEEDS, type: CriteriaFieldComparisonTypes.MIN)]
        public decimal? MinSpeechNeeds { get; set; }

        [PrettyName(SchoolCharacteristicsQuestions.SPEECH_NEEDS)]
        [Range(0, 100)]
        [DBField(name: SchoolTrustFinanceDataFieldNames.SPEECH_NEEDS, type: CriteriaFieldComparisonTypes.MAX)]
        public decimal? MaxSpeechNeeds { get; set; }

        [PrettyName(SchoolCharacteristicsQuestions.HEARING_IMPAIRMENT)]
        [Range(0, 100)]
        [DBField(name: SchoolTrustFinanceDataFieldNames.HEARING_IMPAIRMENT, type: CriteriaFieldComparisonTypes.MIN)]
        public decimal? MinHearingImpairment { get; set; }

        [PrettyName(SchoolCharacteristicsQuestions.HEARING_IMPAIRMENT)]
        [Range(0, 100)]
        [DBField(name: SchoolTrustFinanceDataFieldNames.HEARING_IMPAIRMENT, type: CriteriaFieldComparisonTypes.MAX)]
        public decimal? MaxHearingImpairment { get; set; }

        [PrettyName(SchoolCharacteristicsQuestions.VISUAL_IMPAIRMENT)]
        [Range(0, 100)]
        [DBField(name: SchoolTrustFinanceDataFieldNames.VISUAL_IMPAIRMENT, type: CriteriaFieldComparisonTypes.MIN)]
        public decimal? MinVisualImpairment { get; set; }

        [PrettyName(SchoolCharacteristicsQuestions.VISUAL_IMPAIRMENT)]
        [Range(0, 100)]
        [DBField(name: SchoolTrustFinanceDataFieldNames.VISUAL_IMPAIRMENT, type: CriteriaFieldComparisonTypes.MAX)]
        public decimal? MaxVisualImpairment { get; set; }

        [PrettyName(SchoolCharacteristicsQuestions.MULTI_SENSORY_IMPAIRMENT)]
        [Range(0, 100)]
        [DBField(name: SchoolTrustFinanceDataFieldNames.MULTI_SENSORY_IMPAIRMENT, type: CriteriaFieldComparisonTypes.MIN)]
        public decimal? MinMSImpairment { get; set; }

        [PrettyName(SchoolCharacteristicsQuestions.MULTI_SENSORY_IMPAIRMENT)]
        [Range(0, 100)]
        [DBField(name: SchoolTrustFinanceDataFieldNames.MULTI_SENSORY_IMPAIRMENT, type: CriteriaFieldComparisonTypes.MAX)]
        public decimal? MaxMSImpairment { get; set; }

        [PrettyName(SchoolCharacteristicsQuestions.PHYSICAL_DISABILITY)]
        [Range(0, 100)]
        [DBField(name: SchoolTrustFinanceDataFieldNames.PHYSICAL_DISABILITY, type: CriteriaFieldComparisonTypes.MIN)]
        public decimal? MinPhysicalDisability { get; set; }

        [PrettyName(SchoolCharacteristicsQuestions.PHYSICAL_DISABILITY)]
        [Range(0, 100)]
        [DBField(name: SchoolTrustFinanceDataFieldNames.PHYSICAL_DISABILITY, type: CriteriaFieldComparisonTypes.MAX)]
        public decimal? MaxPhysicalDisability { get; set; }

        [PrettyName(SchoolCharacteristicsQuestions.AUTISTIC_DISORDER)]
        [Range(0, 100)]
        [DBField(name: SchoolTrustFinanceDataFieldNames.AUTISTIC_DISORDER, type: CriteriaFieldComparisonTypes.MIN)]
        public decimal? MinAutisticDisorder { get; set; }

        [PrettyName(SchoolCharacteristicsQuestions.AUTISTIC_DISORDER)]
        [Range(0, 100)]
        [DBField(name: SchoolTrustFinanceDataFieldNames.AUTISTIC_DISORDER, type: CriteriaFieldComparisonTypes.MAX)]
        public decimal? MaxAutisticDisorder { get; set; }
        
        [PrettyName(SchoolCharacteristicsQuestions.OTHER_LEARNING_DIFF)]
        [Range(0, 100)]
        [DBField(name: SchoolTrustFinanceDataFieldNames.OTHER_LEARNING_DIFF, type: CriteriaFieldComparisonTypes.MIN)]
        public decimal? MinOtherLearningDiff { get; set; }

        [PrettyName(SchoolCharacteristicsQuestions.OTHER_LEARNING_DIFF)]
        [Range(0, 100)]
        [DBField(name: SchoolTrustFinanceDataFieldNames.OTHER_LEARNING_DIFF, type: CriteriaFieldComparisonTypes.MAX)]
        public decimal? MaxOtherLearningDiff { get; set; }

        [PrettyName(TrustCharacteristicsQuestions.NUMBER_OF_SCHOOLS)]
        [Range(2, Int32.MaxValue)]
        [DBField(name: SchoolTrustFinanceDataFieldNames.MEMBER_COUNT, type: CriteriaFieldComparisonTypes.MIN)]
        public int? MinNoSchools { get; set; }

        [PrettyName(TrustCharacteristicsQuestions.NUMBER_OF_SCHOOLS)]
        [Range(2, Int32.MaxValue)]
        [DBField(name: SchoolTrustFinanceDataFieldNames.MEMBER_COUNT, type: CriteriaFieldComparisonTypes.MAX)]
        public int? MaxNoSchools { get; set; }

        [PrettyName(TrustCharacteristicsQuestions.TOTAL_INCOME)]
        [Range(0, double.MaxValue)]
        [DBField(name: SchoolTrustFinanceDataFieldNames.TOTAL_INCOME, type: CriteriaFieldComparisonTypes.MIN)]
        public decimal? MinTotalInc { get; set; }

        [PrettyName(TrustCharacteristicsQuestions.TOTAL_INCOME)]
        [Range(0, double.MaxValue)]
        [DBField(name: SchoolTrustFinanceDataFieldNames.TOTAL_INCOME, type: CriteriaFieldComparisonTypes.MAX)]
        public decimal? MaxTotalInc { get; set; }

        [PrettyName(TrustCharacteristicsQuestions.SCHOOL_OVERALL_PHASE)]
        [Range(0, Int32.MaxValue)]
        [DBField(docName: SchoolTrustFinanceDataFieldNames.SCHOOL_OVERALL_PHASE_BREAKDOWN, name: SchoolTrustFinanceDataFieldNames.SCHOOL_OVERALL_PHASE_CROSS_PRIMARY, type: CriteriaFieldComparisonTypes.MIN)]
        public int? MinCrossPhasePrimary { get; set; }

        [PrettyName(TrustCharacteristicsQuestions.SCHOOL_OVERALL_PHASE)]
        [Range(0, Int32.MaxValue)]
        [DBField(docName: SchoolTrustFinanceDataFieldNames.SCHOOL_OVERALL_PHASE_BREAKDOWN, name: SchoolTrustFinanceDataFieldNames.SCHOOL_OVERALL_PHASE_CROSS_PRIMARY, type: CriteriaFieldComparisonTypes.MAX)]
        public int? MaxCrossPhasePrimary { get; set; }

        [PrettyName(TrustCharacteristicsQuestions.SCHOOL_OVERALL_PHASE)]
        [Range(0, Int32.MaxValue)]
        [DBField(docName: SchoolTrustFinanceDataFieldNames.SCHOOL_OVERALL_PHASE_BREAKDOWN, name: SchoolTrustFinanceDataFieldNames.SCHOOL_OVERALL_PHASE_CROSS_SECONDARY, type: CriteriaFieldComparisonTypes.MIN)]
        public int? MinCrossPhaseSecondary { get; set; }

        [PrettyName(TrustCharacteristicsQuestions.SCHOOL_OVERALL_PHASE)]
        [Range(0, Int32.MaxValue)]
        [DBField(docName: SchoolTrustFinanceDataFieldNames.SCHOOL_OVERALL_PHASE_BREAKDOWN, name: SchoolTrustFinanceDataFieldNames.SCHOOL_OVERALL_PHASE_CROSS_SECONDARY, type: CriteriaFieldComparisonTypes.MAX)]
        public int? MaxCrossPhaseSecondary { get; set; }

        [PrettyName(TrustCharacteristicsQuestions.SCHOOL_OVERALL_PHASE)]
        [Range(0, Int32.MaxValue)]
        [DBField(docName: SchoolTrustFinanceDataFieldNames.SCHOOL_OVERALL_PHASE_BREAKDOWN, name: SchoolTrustFinanceDataFieldNames.SCHOOL_OVERALL_PHASE_CROSS_SPECIAL, type: CriteriaFieldComparisonTypes.MIN)]
        public int? MinCrossPhaseSpecial { get; set; }

        [PrettyName(TrustCharacteristicsQuestions.SCHOOL_OVERALL_PHASE)]
        [Range(0, Int32.MaxValue)]
        [DBField(docName: SchoolTrustFinanceDataFieldNames.SCHOOL_OVERALL_PHASE_BREAKDOWN, name: SchoolTrustFinanceDataFieldNames.SCHOOL_OVERALL_PHASE_CROSS_SPECIAL, type: CriteriaFieldComparisonTypes.MAX)]
        public int? MaxCrossPhaseSpecial { get; set; }

        [PrettyName(TrustCharacteristicsQuestions.SCHOOL_OVERALL_PHASE)]
        [Range(0, Int32.MaxValue)]
        [DBField(docName: SchoolTrustFinanceDataFieldNames.SCHOOL_OVERALL_PHASE_BREAKDOWN, name: SchoolTrustFinanceDataFieldNames.SCHOOL_OVERALL_PHASE_CROSS_PRU, type: CriteriaFieldComparisonTypes.MIN)]
        public int? MinCrossPhasePru { get; set; }

        [PrettyName(TrustCharacteristicsQuestions.SCHOOL_OVERALL_PHASE)]
        [Range(0, Int32.MaxValue)]
        [DBField(docName: SchoolTrustFinanceDataFieldNames.SCHOOL_OVERALL_PHASE_BREAKDOWN, name: SchoolTrustFinanceDataFieldNames.SCHOOL_OVERALL_PHASE_CROSS_PRU, type: CriteriaFieldComparisonTypes.MAX)]
        public int? MaxCrossPhasePru { get; set; }

        [PrettyName(TrustCharacteristicsQuestions.SCHOOL_OVERALL_PHASE)]
        [Range(0, Int32.MaxValue)]
        [DBField(docName: SchoolTrustFinanceDataFieldNames.SCHOOL_OVERALL_PHASE_BREAKDOWN, name: SchoolTrustFinanceDataFieldNames.SCHOOL_OVERALL_PHASE_CROSS_AP, type: CriteriaFieldComparisonTypes.MIN)]
        public int? MinCrossPhaseAP { get; set; }

        [PrettyName(TrustCharacteristicsQuestions.SCHOOL_OVERALL_PHASE)]
        [Range(0, Int32.MaxValue)]
        [DBField(docName: SchoolTrustFinanceDataFieldNames.SCHOOL_OVERALL_PHASE_BREAKDOWN, name: SchoolTrustFinanceDataFieldNames.SCHOOL_OVERALL_PHASE_CROSS_AP, type: CriteriaFieldComparisonTypes.MAX)]
        public int? MaxCrossPhaseAP { get; set; }

        [PrettyName(TrustCharacteristicsQuestions.SCHOOL_OVERALL_PHASE)]
        [Range(0, Int32.MaxValue)]
        [DBField(docName: SchoolTrustFinanceDataFieldNames.SCHOOL_OVERALL_PHASE_BREAKDOWN, name: SchoolTrustFinanceDataFieldNames.SCHOOL_OVERALL_PHASE_CROSS_AT, type: CriteriaFieldComparisonTypes.MIN)]
        public int? MinCrossPhaseAT { get; set; }

        [PrettyName(TrustCharacteristicsQuestions.SCHOOL_OVERALL_PHASE)]
        [Range(0, Int32.MaxValue)]
        [DBField(docName: SchoolTrustFinanceDataFieldNames.SCHOOL_OVERALL_PHASE_BREAKDOWN, name: SchoolTrustFinanceDataFieldNames.SCHOOL_OVERALL_PHASE_CROSS_AT, type: CriteriaFieldComparisonTypes.MAX)]
        public int? MaxCrossPhaseAT { get; set; }
    }
}