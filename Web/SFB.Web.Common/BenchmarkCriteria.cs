using System;
using System.ComponentModel.DataAnnotations;
using SFB.Web.Common.Attributes;

namespace SFB.Web.Common
{
    public class BenchmarkCriteria
    {
        [PrettyName(SchoolCharacteristicsQuestions.LA_CODE)]
        [DBField(name: SchoolFinanceDBFieldNames.LA, type: CriteriaFieldComparisonTypes.EQUALTO)]
        public int? LocalAuthorityCode { get; set; }

        [PrettyName(SchoolCharacteristicsQuestions.PFI)]
        [DBField(name: SchoolFinanceDBFieldNames.PFI, type: CriteriaFieldComparisonTypes.EQUALTO)]
        public string[] Pfi { get; set; }

        [PrettyName(SchoolCharacteristicsQuestions.DOES_THE_SCHOOL_HAVE6_FORM)]
        [DBField(name: SchoolFinanceDBFieldNames.HAS_6_FORM, type: CriteriaFieldComparisonTypes.EQUALTO)]
        public string SixthForm { get; set; }

        [PrettyName(SchoolCharacteristicsQuestions.GENDER_OF_PUPILS)]
        [DBField(name: SchoolFinanceDBFieldNames.GENDER, type: CriteriaFieldComparisonTypes.EQUALTO)]
        public string[] Gender { get; set; }        

        [PrettyName(SchoolCharacteristicsQuestions.SCHOOL_OVERALL_PHASE)]
        [DBField(name: SchoolFinanceDBFieldNames.SCHOOL_OVERALL_PHASE, type: CriteriaFieldComparisonTypes.EQUALTO)]
        public string[] SchoolOverallPhase { get; set; }

        [PrettyName(SchoolCharacteristicsQuestions.SCHOOL_PHASE)]
        [DBField(name: SchoolFinanceDBFieldNames.SCHOOL_PHASE, type: CriteriaFieldComparisonTypes.EQUALTO)]
        public string[] SchoolPhase { get; set; }

        [PrettyName(SchoolCharacteristicsQuestions.TYPEOF_ESTABLISHMENT)]
        [DBField(name: SchoolFinanceDBFieldNames.SCHOOL_TYPE, type: CriteriaFieldComparisonTypes.EQUALTO)]
        public string[] TypeOfEstablishment { get; set; }

        [PrettyName(SchoolCharacteristicsQuestions.URBAN_RURAL)]
        [DBField(name: SchoolFinanceDBFieldNames.URBAN_RURAL, type: CriteriaFieldComparisonTypes.EQUALTO)]
        public string[] UrbanRural { get; set; }

        [PrettyName(SchoolCharacteristicsQuestions.GOVERNMENT_OFFICE)]
        [DBField(name: SchoolFinanceDBFieldNames.REGION, type: CriteriaFieldComparisonTypes.EQUALTO)]
        public string[] GovernmentOffice { get; set; }

        [PrettyName(SchoolCharacteristicsQuestions.LONDON_WEIGHTING)]
        [DBField(name: SchoolFinanceDBFieldNames.LONDON_BOROUGH, type: CriteriaFieldComparisonTypes.EQUALTO)]
        public string[] LondonBorough { get; set; }

        [PrettyName(SchoolCharacteristicsQuestions.LONDON_WEIGHTING)]
        [DBField(name: SchoolFinanceDBFieldNames.LONDON_WEIGHT, type: CriteriaFieldComparisonTypes.EQUALTO)]
        public string[] LondonWeighting { get; set; }
        
        [PrettyName(SchoolCharacteristicsQuestions.NUMBER_OF_PUPILS)]
        [Range(0, Int32.MaxValue)]
        [DBField(name: SchoolFinanceDBFieldNames.NO_PUPILS, type: CriteriaFieldComparisonTypes.MIN)]
        public double? MinNoPupil { get; set; }

        [PrettyName(SchoolCharacteristicsQuestions.NUMBER_OF_PUPILS)]
        [Range(0, Int32.MaxValue)]
        [DBField(name: SchoolFinanceDBFieldNames.NO_PUPILS, type: CriteriaFieldComparisonTypes.MAX)]
        public double? MaxNoPupil { get; set; }

        [PrettyName(SchoolCharacteristicsQuestions.PERCENTAGE_OF_ELIGIBLE_FREE_SCHOOL_MEALS)]
        [Range(0,100)]
        [DBField(name: SchoolFinanceDBFieldNames.PERCENTAGE_FSM, type: CriteriaFieldComparisonTypes.MIN)]
        public double? MinPerFSM { get; set; }

        [PrettyName(SchoolCharacteristicsQuestions.PERCENTAGE_OF_ELIGIBLE_FREE_SCHOOL_MEALS)]
        [Range(0, 100)]
        [DBField(name: SchoolFinanceDBFieldNames.PERCENTAGE_FSM, type: CriteriaFieldComparisonTypes.MAX)]
        public double? MaxPerFSM { get; set; }

        [PrettyName(SchoolCharacteristicsQuestions.PERCENTAGE_OF_PUPILS_WITH_STATEMENT_OF_SEN)]
        [Range(0, 100)]
        [DBField(name: SchoolFinanceDBFieldNames.PERCENTAGE_OF_PUPILS_WITH_SEN, type: CriteriaFieldComparisonTypes.MIN)]
        public double? MinPerSEN { get; set; }

        [PrettyName(SchoolCharacteristicsQuestions.PERCENTAGE_OF_PUPILS_WITH_STATEMENT_OF_SEN)]
        [Range(0, 100)]
        [DBField(name: SchoolFinanceDBFieldNames.PERCENTAGE_OF_PUPILS_WITH_SEN, type: CriteriaFieldComparisonTypes.MAX)]
        public double? MaxPerSEN { get; set; }

        [PrettyName(SchoolCharacteristicsQuestions.PERCENTAGE_OF_PUPILS_ON_SEN_REGISTER)]
        [Range(0, 100)]
        [DBField(name: SchoolFinanceDBFieldNames.PERCENTAGE_OF_PUPILS_WITHOUT_SEN, type: CriteriaFieldComparisonTypes.MIN)]
        public double? MinPerSENReg { get; set; }

        [PrettyName(SchoolCharacteristicsQuestions.PERCENTAGE_OF_PUPILS_ON_SEN_REGISTER)]
        [Range(0, 100)]
        [DBField(name: SchoolFinanceDBFieldNames.PERCENTAGE_OF_PUPILS_WITHOUT_SEN, type: CriteriaFieldComparisonTypes.MAX)]
        public double? MaxPerSENReg { get; set; }

        [PrettyName(SchoolCharacteristicsQuestions.PERCENTAGE_OF_PUPILS_WITH_EAL)]
        [Range(0, 100)]
        [DBField(name: SchoolFinanceDBFieldNames.PERCENTAGE_OF_PUPILS_WITH_EAL, type: CriteriaFieldComparisonTypes.MIN)]
        public double? MinPerEAL { get; set; }

        [PrettyName(SchoolCharacteristicsQuestions.PERCENTAGE_OF_PUPILS_WITH_EAL)]
        [Range(0, 100)]
        [DBField(name: SchoolFinanceDBFieldNames.PERCENTAGE_OF_PUPILS_WITH_EAL, type: CriteriaFieldComparisonTypes.MAX)]
        public double? MaxPerEAL { get; set; }

        [PrettyName(SchoolCharacteristicsQuestions.PERCENTAGE_BOARDERS)]
        [Range(0, 100)]
        [DBField(name: SchoolFinanceDBFieldNames.PERCENTAGE_BOARDERS, type: CriteriaFieldComparisonTypes.MIN)]
        public double? MinPerBoarders { get; set; }

        [PrettyName(SchoolCharacteristicsQuestions.PERCENTAGE_BOARDERS)]
        [Range(0, 100)]
        [DBField(name: SchoolFinanceDBFieldNames.PERCENTAGE_BOARDERS, type: CriteriaFieldComparisonTypes.MAX)]
        public double? MaxPerBoarders { get; set; }

        [PrettyName(SchoolCharacteristicsQuestions.ADMISSIONS_POLICY)]
        [DBField(name: SchoolFinanceDBFieldNames.ADMISSION_POLICY, type: CriteriaFieldComparisonTypes.EQUALTO)]
        public string[] AdmPolicy { get; set; }

        [PrettyName(SchoolCharacteristicsQuestions.NUMBER_IN6_FORM)]
        [Range(0, Int32.MaxValue)]
        [DBField(name: SchoolFinanceDBFieldNames.NUMBER_IN_6_FORM, type: CriteriaFieldComparisonTypes.MIN)]
        public double? MinNoSixthForm { get; set; }

        [PrettyName(SchoolCharacteristicsQuestions.NUMBER_IN6_FORM)]
        [Range(0, Int32.MaxValue)]
        [DBField(name: SchoolFinanceDBFieldNames.NUMBER_IN_6_FORM, type: CriteriaFieldComparisonTypes.MAX)]
        public double? MaxNoSixthForm { get; set; }

        [PrettyName(SchoolCharacteristicsQuestions.LOWEST_AGE_PUPILS)]
        [Range(0, Int32.MaxValue)]
        [DBField(name: SchoolFinanceDBFieldNames.LOWEST_AGE_PUPILS, type: CriteriaFieldComparisonTypes.MIN)]
        public double? MinLowestAgePupils { get; set; }

        [PrettyName(SchoolCharacteristicsQuestions.LOWEST_AGE_PUPILS)]
        [Range(0, Int32.MaxValue)]
        [DBField(name: SchoolFinanceDBFieldNames.LOWEST_AGE_PUPILS, type: CriteriaFieldComparisonTypes.MAX)]
        public double? MaxLowestAgePupils { get; set; }

        [PrettyName(SchoolCharacteristicsQuestions.HIGHEST_AGE_PUPILS)]
        [Range(0, Int32.MaxValue)]
        [DBField(name: SchoolFinanceDBFieldNames.HIGHEST_AGE_PUPILS, type: CriteriaFieldComparisonTypes.MIN)]
        public double? MinHighestAgePupils { get; set; }

        [PrettyName(SchoolCharacteristicsQuestions.HIGHEST_AGE_PUPILS)]
        [Range(0, Int32.MaxValue)]
        [DBField(name: SchoolFinanceDBFieldNames.HIGHEST_AGE_PUPILS, type: CriteriaFieldComparisonTypes.MAX)]
        public double? MaxHighestAgePupils { get; set; }

        [PrettyName(SchoolCharacteristicsQuestions.PERCENTAGE_QUALIFIED_TEACHERS)]
        [Range(0, 100)]
        [DBField(name: SchoolFinanceDBFieldNames.PERCENTAGE_QUALIFIED_TEACHERS, type: CriteriaFieldComparisonTypes.MIN)]
        public double? MinTeachersWithQualifiedTeacherStatus { get; set; }

        [PrettyName(SchoolCharacteristicsQuestions.PERCENTAGE_QUALIFIED_TEACHERS)]
        [Range(0, 100)]
        [DBField(name: SchoolFinanceDBFieldNames.PERCENTAGE_QUALIFIED_TEACHERS, type: CriteriaFieldComparisonTypes.MAX)]
        public double? MaxTeachersWithQualifiedTeacherStatus { get; set; }

        [PrettyName(SchoolCharacteristicsQuestions.FULL_TIME_TA)]
        [Range(0, Int32.MaxValue)]
        [DBField(name: SchoolFinanceDBFieldNames.FULL_TIME_TA, type: CriteriaFieldComparisonTypes.MIN)]
        public double? MinTotalNumberOfTeachingAssistantsFullTimeEquivalent { get; set; }

        [PrettyName(SchoolCharacteristicsQuestions.FULL_TIME_TA)]
        [Range(0, Int32.MaxValue)]
        [DBField(name: SchoolFinanceDBFieldNames.FULL_TIME_TA, type: CriteriaFieldComparisonTypes.MAX)]
        public double? MaxTotalNumberOfTeachingAssistantsFullTimeEquivalent { get; set; }

        [PrettyName(SchoolCharacteristicsQuestions.FULL_TIME_OTHER)]
        [Range(0, Int32.MaxValue)]
        [DBField(name: SchoolFinanceDBFieldNames.FULL_TIME_OTHER, type: CriteriaFieldComparisonTypes.MIN)]
        public double? MinFullTimeOther { get; set; }

        [PrettyName(SchoolCharacteristicsQuestions.FULL_TIME_OTHER)]
        [Range(0, Int32.MaxValue)]
        [DBField(name: SchoolFinanceDBFieldNames.FULL_TIME_OTHER, type: CriteriaFieldComparisonTypes.MAX)]
        public double? MaxFullTimeOther { get; set; }

        [PrettyName(SchoolCharacteristicsQuestions.FULL_TIME_ADMIN)]
        [Range(0, Int32.MaxValue)]
        [DBField(name: SchoolFinanceDBFieldNames.ADMIN_STAFF, type: CriteriaFieldComparisonTypes.MIN)]
        public double? MinFullTimeAdmin { get; set; }

        [PrettyName(SchoolCharacteristicsQuestions.FULL_TIME_ADMIN)]
        [Range(0, Int32.MaxValue)]
        [DBField(name: SchoolFinanceDBFieldNames.ADMIN_STAFF, type: CriteriaFieldComparisonTypes.MAX)]
        public double? MaxFullTimeAdmin { get; set; }

        [PrettyName(SchoolCharacteristicsQuestions.SCHOOL_WORKFORCE_FTE)]
        [Range(0, Int32.MaxValue)]
        [DBField(name: SchoolFinanceDBFieldNames.WORKFORCE_TOTAL, type: CriteriaFieldComparisonTypes.MIN)]
        public double? MinTotalSchoolWorkforceFullTimeEquivalent { get; set; }

        [PrettyName(SchoolCharacteristicsQuestions.SCHOOL_WORKFORCE_FTE)]
        [Range(0, Int32.MaxValue)]
        [DBField(name: SchoolFinanceDBFieldNames.WORKFORCE_TOTAL, type: CriteriaFieldComparisonTypes.MAX)]
        public double? MaxTotalSchoolWorkforceFullTimeEquivalent { get; set; }

        [PrettyName(SchoolCharacteristicsQuestions.NUMBER_OF_TEACHERS_FTE)]
        [Range(0, Int32.MaxValue)]
        [DBField(name: SchoolFinanceDBFieldNames.TEACHERS_TOTAL, type: CriteriaFieldComparisonTypes.MIN)]
        public double? MinTotalNumberOfTeachersFullTimeEquivalent { get; set; }

        [PrettyName(SchoolCharacteristicsQuestions.NUMBER_OF_TEACHERS_FTE)]
        [Range(0, Int32.MaxValue)]
        [DBField(name: SchoolFinanceDBFieldNames.TEACHERS_TOTAL, type: CriteriaFieldComparisonTypes.MAX)]
        public double? MaxTotalNumberOfTeachersFullTimeEquivalent { get; set; }

        [PrettyName(SchoolCharacteristicsQuestions.SENIOR_LEADERSHIP_FTE)]
        [Range(0, Int32.MaxValue)]
        [DBField(name: SchoolFinanceDBFieldNames.TEACHERS_LEADER, type: CriteriaFieldComparisonTypes.MIN)]
        public double? MinTotalNumberOfTeachersInTheLeadershipGroupFullTimeEquivalent { get; set; }

        [PrettyName(SchoolCharacteristicsQuestions.SENIOR_LEADERSHIP_FTE)]
        [Range(0, Int32.MaxValue)]
        [DBField(name: SchoolFinanceDBFieldNames.TEACHERS_LEADER, type: CriteriaFieldComparisonTypes.MAX)]
        public double? MaxTotalNumberOfTeachersInTheLeadershipGroupFullTimeEquivalent { get; set; }

        [PrettyName(SchoolCharacteristicsQuestions.KS2_ACTUAL)]
        [Range(0, 100)]
        [DBField(name: SchoolFinanceDBFieldNames.KS2_ACTUAL, type: CriteriaFieldComparisonTypes.MIN)]
        public double? MinKs2Actual { get; set; }

        [PrettyName(SchoolCharacteristicsQuestions.KS2_ACTUAL)]
        [Range(0, 100)]
        [DBField(name: SchoolFinanceDBFieldNames.KS2_ACTUAL, type: CriteriaFieldComparisonTypes.MAX)]
        public double? MaxKs2Actual { get; set; }

        [PrettyName(SchoolCharacteristicsQuestions.KS2_PROGRESS)]
        [Range(-20, 20)]
        [DBField(name: SchoolFinanceDBFieldNames.KS2_PROGRESS, type: CriteriaFieldComparisonTypes.MIN)]
        public double? MinKs2Progress { get; set; }

        [PrettyName(SchoolCharacteristicsQuestions.KS2_PROGRESS)]
        [Range(-20, 20)]
        [DBField(name: SchoolFinanceDBFieldNames.KS2_PROGRESS, type: CriteriaFieldComparisonTypes.MAX)]
        public double? MaxKs2Progress { get; set; }

        [PrettyName(SchoolCharacteristicsQuestions.AVERAGE_ATTAINMENT_8)]
        [Range(0, 100)]
        [DBField(name: SchoolFinanceDBFieldNames.AVERAGE_ATTAINMENT, type: CriteriaFieldComparisonTypes.MIN)]
        public double? MinAvAtt8 { get; set; }

        [PrettyName(SchoolCharacteristicsQuestions.AVERAGE_ATTAINMENT_8)]
        [Range(0, 100)]
        [DBField(name: SchoolFinanceDBFieldNames.AVERAGE_ATTAINMENT, type: CriteriaFieldComparisonTypes.MAX)]
        public double? MaxAvAtt8 { get; set; }

        [PrettyName(SchoolCharacteristicsQuestions.PROGRESS_8_MEASURE)]
        [Range(-5, 5)]
        [DBField(name: SchoolFinanceDBFieldNames.PROGRESS_8_MEASURE, type: CriteriaFieldComparisonTypes.MIN)]
        public double? MinP8Mea { get; set; }

        [PrettyName(SchoolCharacteristicsQuestions.PROGRESS_8_MEASURE)]
        [Range(-5, 5)]
        [DBField(name: SchoolFinanceDBFieldNames.PROGRESS_8_MEASURE, type: CriteriaFieldComparisonTypes.MAX)]
        public double? MaxP8Mea { get; set; }

        [PrettyName(SchoolCharacteristicsQuestions.OFSTED_RATING)]
        [DBField(name: SchoolFinanceDBFieldNames.OFSTED_RATING_NAME, type: CriteriaFieldComparisonTypes.EQUALTO)]
        public string[] OfstedRating { get; set; }

        [PrettyName(SchoolCharacteristicsQuestions.SPECIFIC_LEARNING_DIFFICULTY)]
        [Range(0, 100)]
        [DBField(name: SchoolFinanceDBFieldNames.SPECIFIC_LEARNING_DIFFICULTY, type: CriteriaFieldComparisonTypes.MIN)]
        public double? MinSpecLearnDiff { get; set; }

        [PrettyName(SchoolCharacteristicsQuestions.SPECIFIC_LEARNING_DIFFICULTY)]
        [Range(0, 100)]
        [DBField(name: SchoolFinanceDBFieldNames.SPECIFIC_LEARNING_DIFFICULTY, type: CriteriaFieldComparisonTypes.MAX)]
        public double? MaxSpecLearnDiff { get; set; }

        [PrettyName(SchoolCharacteristicsQuestions.MODERATE_LEARNING_DIFFICULTY)]
        [Range(0, 100)]
        [DBField(name: SchoolFinanceDBFieldNames.MODERATE_LEARNING_DIFFICULTY, type: CriteriaFieldComparisonTypes.MIN)]
        public double? MinModLearnDiff { get; set; }

        [PrettyName(SchoolCharacteristicsQuestions.MODERATE_LEARNING_DIFFICULTY)]
        [Range(0, 100)]
        [DBField(name: SchoolFinanceDBFieldNames.MODERATE_LEARNING_DIFFICULTY, type: CriteriaFieldComparisonTypes.MAX)]
        public double? MaxModLearnDiff { get; set; }

        [PrettyName(SchoolCharacteristicsQuestions.SEVERE_LEARNING_DIFFICULTY)]
        [Range(0, 100)]
        [DBField(name: SchoolFinanceDBFieldNames.SEVERE_LEARNING_DIFFICULTY, type: CriteriaFieldComparisonTypes.MIN)]
        public double? MinSevLearnDiff { get; set; }

        [PrettyName(SchoolCharacteristicsQuestions.SEVERE_LEARNING_DIFFICULTY)]
        [Range(0, 100)]
        [DBField(name: SchoolFinanceDBFieldNames.SEVERE_LEARNING_DIFFICULTY, type: CriteriaFieldComparisonTypes.MAX)]
        public double? MaxSevLearnDiff { get; set; }

        [PrettyName(SchoolCharacteristicsQuestions.PROF_LEARNING_DIFFICULTY)]
        [Range(0, 100)]
        [DBField(name: SchoolFinanceDBFieldNames.PROF_LEARNING_DIFFICULTY, type: CriteriaFieldComparisonTypes.MIN)]
        public double? MinProfLearnDiff { get; set; }

        [PrettyName(SchoolCharacteristicsQuestions.PROF_LEARNING_DIFFICULTY)]
        [Range(0, 100)]
        [DBField(name: SchoolFinanceDBFieldNames.PROF_LEARNING_DIFFICULTY, type: CriteriaFieldComparisonTypes.MAX)]
        public double? MaxProfLearnDiff { get; set; }

        [PrettyName(SchoolCharacteristicsQuestions.SOCIAL_HEALTH)]
        [Range(0, 100)]
        [DBField(name: SchoolFinanceDBFieldNames.SOCIAL_HEALTH, type: CriteriaFieldComparisonTypes.MIN)]
        public double? MinSocialHealth { get; set; }

        [PrettyName(SchoolCharacteristicsQuestions.SOCIAL_HEALTH)]
        [Range(0, 100)]
        [DBField(name: SchoolFinanceDBFieldNames.SOCIAL_HEALTH, type: CriteriaFieldComparisonTypes.MAX)]
        public double? MaxSocialHealth { get; set; }

        [PrettyName(SchoolCharacteristicsQuestions.SPEECH_NEEDS)]
        [Range(0, 100)]
        [DBField(name: SchoolFinanceDBFieldNames.SPEECH_NEEDS, type: CriteriaFieldComparisonTypes.MIN)]
        public double? MinSpeechNeeds { get; set; }

        [PrettyName(SchoolCharacteristicsQuestions.SPEECH_NEEDS)]
        [Range(0, 100)]
        [DBField(name: SchoolFinanceDBFieldNames.SPEECH_NEEDS, type: CriteriaFieldComparisonTypes.MAX)]
        public double? MaxSpeechNeeds { get; set; }

        [PrettyName(SchoolCharacteristicsQuestions.HEARING_IMPAIRMENT)]
        [Range(0, 100)]
        [DBField(name: SchoolFinanceDBFieldNames.HEARING_IMPAIRMENT, type: CriteriaFieldComparisonTypes.MIN)]
        public double? MinHearingImpairment { get; set; }

        [PrettyName(SchoolCharacteristicsQuestions.HEARING_IMPAIRMENT)]
        [Range(0, 100)]
        [DBField(name: SchoolFinanceDBFieldNames.HEARING_IMPAIRMENT, type: CriteriaFieldComparisonTypes.MAX)]
        public double? MaxHearingImpairment { get; set; }

        [PrettyName(SchoolCharacteristicsQuestions.VISUAL_IMPAIRMENT)]
        [Range(0, 100)]
        [DBField(name: SchoolFinanceDBFieldNames.VISUAL_IMPAIRMENT, type: CriteriaFieldComparisonTypes.MIN)]
        public double? MinVisualImpairment { get; set; }

        [PrettyName(SchoolCharacteristicsQuestions.VISUAL_IMPAIRMENT)]
        [Range(0, 100)]
        [DBField(name: SchoolFinanceDBFieldNames.VISUAL_IMPAIRMENT, type: CriteriaFieldComparisonTypes.MAX)]
        public double? MaxVisualImpairment { get; set; }

        [PrettyName(SchoolCharacteristicsQuestions.MULTI_SENSORY_IMPAIRMENT)]
        [Range(0, 100)]
        [DBField(name: SchoolFinanceDBFieldNames.MULTI_SENSORY_IMPAIRMENT, type: CriteriaFieldComparisonTypes.MIN)]
        public double? MinMSImpairment { get; set; }

        [PrettyName(SchoolCharacteristicsQuestions.MULTI_SENSORY_IMPAIRMENT)]
        [Range(0, 100)]
        [DBField(name: SchoolFinanceDBFieldNames.MULTI_SENSORY_IMPAIRMENT, type: CriteriaFieldComparisonTypes.MAX)]
        public double? MaxMSImpairment { get; set; }

        [PrettyName(SchoolCharacteristicsQuestions.PHYSICAL_DISABILITY)]
        [Range(0, 100)]
        [DBField(name: SchoolFinanceDBFieldNames.PHYSICAL_DISABILITY, type: CriteriaFieldComparisonTypes.MIN)]
        public double? MinPhysicalDisability { get; set; }

        [PrettyName(SchoolCharacteristicsQuestions.PHYSICAL_DISABILITY)]
        [Range(0, 100)]
        [DBField(name: SchoolFinanceDBFieldNames.PHYSICAL_DISABILITY, type: CriteriaFieldComparisonTypes.MAX)]
        public double? MaxPhysicalDisability { get; set; }

        [PrettyName(SchoolCharacteristicsQuestions.AUTISTIC_DISORDER)]
        [Range(0, 100)]
        [DBField(name: SchoolFinanceDBFieldNames.AUTISTIC_DISORDER, type: CriteriaFieldComparisonTypes.MIN)]
        public double? MinAutisticDisorder { get; set; }

        [PrettyName(SchoolCharacteristicsQuestions.AUTISTIC_DISORDER)]
        [Range(0, 100)]
        [DBField(name: SchoolFinanceDBFieldNames.AUTISTIC_DISORDER, type: CriteriaFieldComparisonTypes.MAX)]
        public double? MaxAutisticDisorder { get; set; }
        
        [PrettyName(SchoolCharacteristicsQuestions.OTHER_LEARNING_DIFF)]
        [Range(0, 100)]
        [DBField(name: SchoolFinanceDBFieldNames.OTHER_LEARNING_DIFF, type: CriteriaFieldComparisonTypes.MIN)]
        public double? MinOtherLearningDiff { get; set; }

        [PrettyName(SchoolCharacteristicsQuestions.OTHER_LEARNING_DIFF)]
        [Range(0, 100)]
        [DBField(name: SchoolFinanceDBFieldNames.OTHER_LEARNING_DIFF, type: CriteriaFieldComparisonTypes.MAX)]
        public double? MaxOtherLearningDiff { get; set; }

        [PrettyName(TrustCharacteristicsQuestions.NUMBER_OF_SCHOOLS)]
        [Range(2, Int32.MaxValue)]
        [DBField(name: SchoolFinanceDBFieldNames.NO_SCHOOLS, type: CriteriaFieldComparisonTypes.MIN)]
        public int? MinNoSchools { get; set; }

        [PrettyName(TrustCharacteristicsQuestions.NUMBER_OF_SCHOOLS)]
        [Range(2, Int32.MaxValue)]
        [DBField(name: SchoolFinanceDBFieldNames.NO_SCHOOLS, type: CriteriaFieldComparisonTypes.MAX)]
        public int? MaxNoSchools { get; set; }

        [PrettyName(TrustCharacteristicsQuestions.TOTAL_INCOME)]
        [Range(0, double.MaxValue)]
        [DBField(name: SchoolFinanceDBFieldNames.TOTAL_INCOME, type: CriteriaFieldComparisonTypes.MIN)]
        public double? MinTotalInc { get; set; }

        [PrettyName(TrustCharacteristicsQuestions.TOTAL_INCOME)]
        [Range(0, double.MaxValue)]
        [DBField(name: SchoolFinanceDBFieldNames.TOTAL_INCOME, type: CriteriaFieldComparisonTypes.MAX)]
        public double? MaxTotalInc { get; set; }

        [PrettyName(TrustCharacteristicsQuestions.SCHOOL_OVERALL_PHASE)]
        [Range(0, Int32.MaxValue)]
        [DBField(docName: SchoolFinanceDBFieldNames.SCHOOL_OVERALL_PHASE_BREAKDOWN, name: SchoolFinanceDBFieldNames.SCHOOL_OVERALL_PHASE_CROSS_PRIMARY, type: CriteriaFieldComparisonTypes.MIN)]
        public int? MinCrossPhasePrimary { get; set; }

        [PrettyName(TrustCharacteristicsQuestions.SCHOOL_OVERALL_PHASE)]
        [Range(0, Int32.MaxValue)]
        [DBField(docName: SchoolFinanceDBFieldNames.SCHOOL_OVERALL_PHASE_BREAKDOWN, name: SchoolFinanceDBFieldNames.SCHOOL_OVERALL_PHASE_CROSS_PRIMARY, type: CriteriaFieldComparisonTypes.MAX)]
        public int? MaxCrossPhasePrimary { get; set; }

        [PrettyName(TrustCharacteristicsQuestions.SCHOOL_OVERALL_PHASE)]
        [Range(0, Int32.MaxValue)]
        [DBField(docName: SchoolFinanceDBFieldNames.SCHOOL_OVERALL_PHASE_BREAKDOWN, name: SchoolFinanceDBFieldNames.SCHOOL_OVERALL_PHASE_CROSS_SECONDARY, type: CriteriaFieldComparisonTypes.MIN)]
        public int? MinCrossPhaseSecondary { get; set; }

        [PrettyName(TrustCharacteristicsQuestions.SCHOOL_OVERALL_PHASE)]
        [Range(0, Int32.MaxValue)]
        [DBField(docName: SchoolFinanceDBFieldNames.SCHOOL_OVERALL_PHASE_BREAKDOWN, name: SchoolFinanceDBFieldNames.SCHOOL_OVERALL_PHASE_CROSS_SECONDARY, type: CriteriaFieldComparisonTypes.MAX)]
        public int? MaxCrossPhaseSecondary { get; set; }

        [PrettyName(TrustCharacteristicsQuestions.SCHOOL_OVERALL_PHASE)]
        [Range(0, Int32.MaxValue)]
        [DBField(docName: SchoolFinanceDBFieldNames.SCHOOL_OVERALL_PHASE_BREAKDOWN, name: SchoolFinanceDBFieldNames.SCHOOL_OVERALL_PHASE_CROSS_SPECIAL, type: CriteriaFieldComparisonTypes.MIN)]
        public int? MinCrossPhaseSpecial { get; set; }

        [PrettyName(TrustCharacteristicsQuestions.SCHOOL_OVERALL_PHASE)]
        [Range(0, Int32.MaxValue)]
        [DBField(docName: SchoolFinanceDBFieldNames.SCHOOL_OVERALL_PHASE_BREAKDOWN, name: SchoolFinanceDBFieldNames.SCHOOL_OVERALL_PHASE_CROSS_SPECIAL, type: CriteriaFieldComparisonTypes.MAX)]
        public int? MaxCrossPhaseSpecial { get; set; }

        [PrettyName(TrustCharacteristicsQuestions.SCHOOL_OVERALL_PHASE)]
        [Range(0, Int32.MaxValue)]
        [DBField(docName: SchoolFinanceDBFieldNames.SCHOOL_OVERALL_PHASE_BREAKDOWN, name: SchoolFinanceDBFieldNames.SCHOOL_OVERALL_PHASE_CROSS_PRU, type: CriteriaFieldComparisonTypes.MIN)]
        public int? MinCrossPhasePru { get; set; }

        [PrettyName(TrustCharacteristicsQuestions.SCHOOL_OVERALL_PHASE)]
        [Range(0, Int32.MaxValue)]
        [DBField(docName: SchoolFinanceDBFieldNames.SCHOOL_OVERALL_PHASE_BREAKDOWN, name: SchoolFinanceDBFieldNames.SCHOOL_OVERALL_PHASE_CROSS_PRU, type: CriteriaFieldComparisonTypes.MAX)]
        public int? MaxCrossPhasePru { get; set; }

        [PrettyName(TrustCharacteristicsQuestions.SCHOOL_OVERALL_PHASE)]
        [Range(0, Int32.MaxValue)]
        [DBField(docName: SchoolFinanceDBFieldNames.SCHOOL_OVERALL_PHASE_BREAKDOWN, name: SchoolFinanceDBFieldNames.SCHOOL_OVERALL_PHASE_CROSS_AP, type: CriteriaFieldComparisonTypes.MIN)]
        public int? MinCrossPhaseAP { get; set; }

        [PrettyName(TrustCharacteristicsQuestions.SCHOOL_OVERALL_PHASE)]
        [Range(0, Int32.MaxValue)]
        [DBField(docName: SchoolFinanceDBFieldNames.SCHOOL_OVERALL_PHASE_BREAKDOWN, name: SchoolFinanceDBFieldNames.SCHOOL_OVERALL_PHASE_CROSS_AP, type: CriteriaFieldComparisonTypes.MAX)]
        public int? MaxCrossPhaseAP { get; set; }

        [PrettyName(TrustCharacteristicsQuestions.SCHOOL_OVERALL_PHASE)]
        [Range(0, Int32.MaxValue)]
        [DBField(docName: SchoolFinanceDBFieldNames.SCHOOL_OVERALL_PHASE_BREAKDOWN, name: SchoolFinanceDBFieldNames.SCHOOL_OVERALL_PHASE_CROSS_AT, type: CriteriaFieldComparisonTypes.MIN)]
        public int? MinCrossPhaseAT { get; set; }

        [PrettyName(TrustCharacteristicsQuestions.SCHOOL_OVERALL_PHASE)]
        [Range(0, Int32.MaxValue)]
        [DBField(docName: SchoolFinanceDBFieldNames.SCHOOL_OVERALL_PHASE_BREAKDOWN, name: SchoolFinanceDBFieldNames.SCHOOL_OVERALL_PHASE_CROSS_AT, type: CriteriaFieldComparisonTypes.MAX)]
        public int? MaxCrossPhaseAT { get; set; }
    }
}