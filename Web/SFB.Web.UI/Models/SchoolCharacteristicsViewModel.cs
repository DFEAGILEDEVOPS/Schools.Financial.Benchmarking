using System.Collections.Generic;
using System.Linq;
using SFB.Web.Common;
using SFB.Web.Domain.Models;

namespace SFB.Web.UI.Models
{
    public class SchoolCharacteristicsViewModel : ViewModelBase
    {
        public SchoolViewModel BenchmarkSchool { get; set; }
        public BenchmarkCriteria BenchmarkCriteria { get; set; }
        public List<SchoolCharacteristic> SchoolCharacteristics { get; set; }

        public SchoolCharacteristicsViewModel(SchoolViewModel school, ComparisonListModel comparisonList, BenchmarkCriteria benchmarkCriteria)
        {
            base.ComparisonList = comparisonList;
            this.BenchmarkSchool = school;
            this.SchoolCharacteristics = BuildSchoolCharacteristics(school);
            this.BenchmarkCriteria = benchmarkCriteria;
        }

        public string this[string question] 
        {
            get
            {
                return SchoolCharacteristics.Find(s => s.Question == question).Value; 
            }
        }  

        private List<SchoolCharacteristic> BuildSchoolCharacteristics(SchoolViewModel schoolVM)
        {
            var latestSchoolData = schoolVM.HistoricalSchoolFinancialDataModels.Last();
            var list = new List<SchoolCharacteristic>();
            list.Add(new SchoolCharacteristic() { Question = SchoolCharacteristicsQuestions.NUMBER_OF_PUPILS, Value = latestSchoolData.PupilCount + " pupils" });
            list.Add(new SchoolCharacteristic() { Question = SchoolCharacteristicsQuestions.GENDER_OF_PUPILS, Value = latestSchoolData.Gender });
            list.Add(new SchoolCharacteristic() { Question = SchoolCharacteristicsQuestions.SCHOOL_PHASE, Value = latestSchoolData.SchoolPhase });
            list.Add(new SchoolCharacteristic() { Question = SchoolCharacteristicsQuestions.SCHOOL_OVERALL_PHASE, Value = latestSchoolData.SchoolOverallPhase });
            list.Add(new SchoolCharacteristic() { Question = SchoolCharacteristicsQuestions.TYPEOF_ESTABLISHMENT, Value = latestSchoolData.SchoolType });
            list.Add(new SchoolCharacteristic() { Question = SchoolCharacteristicsQuestions.URBAN_RURAL, Value = latestSchoolData.UrbanRural });
            list.Add(new SchoolCharacteristic() { Question = SchoolCharacteristicsQuestions.GOVERNMENT_OFFICE, Value = latestSchoolData.GovernmentOfficeRegion });
            list.Add(new SchoolCharacteristic() { Question = SchoolCharacteristicsQuestions.LONDON_BOROUGH, Value = latestSchoolData.LondonBorough });
            list.Add(new SchoolCharacteristic() { Question = SchoolCharacteristicsQuestions.LONDON_WEIGHTING, Value = latestSchoolData.LondonWeighting });
            list.Add(new SchoolCharacteristic() { Question = SchoolCharacteristicsQuestions.PERCENTAGE_OF_ELIGIBLE_FREE_SCHOOL_MEALS, Value = latestSchoolData.PercentageOfEligibleFreeSchoolMeals });
            list.Add(new SchoolCharacteristic() { Question = SchoolCharacteristicsQuestions.PERCENTAGE_OF_PUPILS_WITH_STATEMENT_OF_SEN, Value = latestSchoolData.PercentageOfPupilsWithSen });
            list.Add(new SchoolCharacteristic() { Question = SchoolCharacteristicsQuestions.PERCENTAGE_OF_PUPILS_ON_SEN_REGISTER, Value = latestSchoolData.PercentageOfPupilsWithoutSen });
            list.Add(new SchoolCharacteristic() { Question = SchoolCharacteristicsQuestions.PERCENTAGE_OF_PUPILS_WITH_EAL, Value = latestSchoolData.PercentageOfPupilsWithEal });
            list.Add(new SchoolCharacteristic() { Question = SchoolCharacteristicsQuestions.PERCENTAGE_BOARDERS, Value = latestSchoolData.PercentageBoarders });
            list.Add(new SchoolCharacteristic() { Question = SchoolCharacteristicsQuestions.ADMISSIONS_POLICY, Value = latestSchoolData.AdmissionPolicy });
            list.Add(new SchoolCharacteristic() { Question = SchoolCharacteristicsQuestions.PFI, Value = latestSchoolData.Pfi });
            list.Add(new SchoolCharacteristic() { Question = SchoolCharacteristicsQuestions.DOES_THE_SCHOOL_HAVE6_FORM, Value = latestSchoolData.DoesTheSchoolHave6Form });
            list.Add(new SchoolCharacteristic() { Question = SchoolCharacteristicsQuestions.NUMBER_IN6_FORM, Value = latestSchoolData.NumberIn6Form });
            list.Add(new SchoolCharacteristic() { Question = SchoolCharacteristicsQuestions.LOWEST_AGE_PUPILS, Value = latestSchoolData.LowestAgePupils });
            list.Add(new SchoolCharacteristic() { Question = SchoolCharacteristicsQuestions.HIGHEST_AGE_PUPILS, Value = latestSchoolData.HighestAgePupils });
            list.Add(new SchoolCharacteristic() { Question = SchoolCharacteristicsQuestions.PERCENTAGE_QUALIFIED_TEACHERS, Value = latestSchoolData.PercentageQualifiedTeachers });
            list.Add(new SchoolCharacteristic() { Question = SchoolCharacteristicsQuestions.FULL_TIME_TA, Value = latestSchoolData.FullTimeTA });
            list.Add(new SchoolCharacteristic() { Question = SchoolCharacteristicsQuestions.FULL_TIME_OTHER, Value = latestSchoolData.FullTimeOther });
            list.Add(new SchoolCharacteristic() { Question = SchoolCharacteristicsQuestions.FULL_TIME_ADMIN, Value = latestSchoolData.FullTimeAdmin });
            list.Add(new SchoolCharacteristic() { Question = SchoolCharacteristicsQuestions.SCHOOL_WORKFORCE_FTE, Value = latestSchoolData.TotalSchoolWorkforceFTE });
            list.Add(new SchoolCharacteristic() { Question = SchoolCharacteristicsQuestions.NUMBER_OF_TEACHERS_FTE, Value = latestSchoolData.TotalNumberOfTeachersFTE });
            list.Add(new SchoolCharacteristic() { Question = SchoolCharacteristicsQuestions.SENIOR_LEADERSHIP_FTE, Value = latestSchoolData.TotalSeniorTeachersFTE });
            list.Add(new SchoolCharacteristic() { Question = SchoolCharacteristicsQuestions.KS2_ACTUAL, Value = latestSchoolData.Ks2Actual });
            list.Add(new SchoolCharacteristic() { Question = SchoolCharacteristicsQuestions.KS2_PROGRESS, Value = latestSchoolData.Ks2Progress });
            list.Add(new SchoolCharacteristic() { Question = SchoolCharacteristicsQuestions.AVERAGE_ATTAINMENT_8, Value = latestSchoolData.AvAtt8 });
            list.Add(new SchoolCharacteristic() { Question = SchoolCharacteristicsQuestions.PROGRESS_8_MEASURE, Value = latestSchoolData.P8Mea });
            list.Add(new SchoolCharacteristic() { Question = SchoolCharacteristicsQuestions.OFSTED_RATING, Value = latestSchoolData.OfstedRating });
            list.Add(new SchoolCharacteristic() { Question = SchoolCharacteristicsQuestions.SPECIFIC_LEARNING_DIFFICULTY, Value = latestSchoolData.SpecificLearningDifficulty });
            list.Add(new SchoolCharacteristic() { Question = SchoolCharacteristicsQuestions.MODERATE_LEARNING_DIFFICULTY, Value = latestSchoolData.ModerateLearningDifficulty });
            list.Add(new SchoolCharacteristic() { Question = SchoolCharacteristicsQuestions.SEVERE_LEARNING_DIFFICULTY, Value = latestSchoolData.SevereLearningDifficulty });
            list.Add(new SchoolCharacteristic() { Question = SchoolCharacteristicsQuestions.PROF_LEARNING_DIFFICULTY, Value = latestSchoolData.ProfLearningDifficulty });
            list.Add(new SchoolCharacteristic() { Question = SchoolCharacteristicsQuestions.SOCIAL_HEALTH, Value = latestSchoolData.SocialHealth });
            list.Add(new SchoolCharacteristic() { Question = SchoolCharacteristicsQuestions.SPEECH_NEEDS, Value = latestSchoolData.SpeechNeeds });
            list.Add(new SchoolCharacteristic() { Question = SchoolCharacteristicsQuestions.HEARING_IMPAIRMENT, Value = latestSchoolData.HearingImpairment });
            list.Add(new SchoolCharacteristic() { Question = SchoolCharacteristicsQuestions.VISUAL_IMPAIRMENT, Value = latestSchoolData.VisualImpairment });
            list.Add(new SchoolCharacteristic() { Question = SchoolCharacteristicsQuestions.MULTI_SENSORY_IMPAIRMENT, Value = latestSchoolData.MultiSensoryImpairment });
            list.Add(new SchoolCharacteristic() { Question = SchoolCharacteristicsQuestions.PHYSICAL_DISABILITY, Value = latestSchoolData.PhysicalDisability });
            list.Add(new SchoolCharacteristic() { Question = SchoolCharacteristicsQuestions.AUTISTIC_DISORDER, Value = latestSchoolData.AutisticDisorder });
            list.Add(new SchoolCharacteristic() { Question = SchoolCharacteristicsQuestions.OTHER_LEARNING_DIFF, Value = latestSchoolData.OtherLearningDifficulty });

            return list;
        }
    }
}
