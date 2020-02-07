using System.Collections.Generic;
using System.Linq;
using SFB.Web.ApplicationCore.Models;
using SFB.Web.ApplicationCore.Helpers.Constants;

namespace SFB.Web.UI.Models
{
    public class SchoolCharacteristicsViewModel : ViewModelBase
    {
        public SchoolViewModel BenchmarkSchool { get; set; }
        public BenchmarkCriteria BenchmarkCriteria { get; set; }
        public List<SchoolCharacteristic> SchoolCharacteristics { get; set; }

        public SchoolCharacteristicsViewModel(SchoolViewModel school, SchoolComparisonListModel comparisonList, BenchmarkCriteria benchmarkCriteria)
        {
            base.ComparisonList = comparisonList;
            this.BenchmarkSchool = school;
            this.SchoolCharacteristics = BuildSchoolCharacteristics(school);
            this.BenchmarkCriteria = benchmarkCriteria;
        }

        public SchoolCharacteristicsViewModel(SchoolViewModelWithNoDefaultSchool school, SchoolComparisonListModel comparisonList, BenchmarkCriteria benchmarkCriteria)
        {
            base.ComparisonList = comparisonList;
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
            var latestSchoolData = schoolVM?.HistoricalFinancialDataModels?.Last();
            var list = new List<SchoolCharacteristic>();
            list.Add(new SchoolCharacteristic() { Question = SchoolCharacteristicsQuestions.NUMBER_OF_PUPILS, Value = latestSchoolData == null ? null : latestSchoolData?.PupilCount + " pupils" });
            list.Add(new SchoolCharacteristic() { Question = SchoolCharacteristicsQuestions.GENDER_OF_PUPILS, Value = latestSchoolData?.Gender });
            //list.Add(new SchoolCharacteristic() { Question = SchoolCharacteristicsQuestions.SCHOOL_PHASE, Value = latestSchoolData?.SchoolPhase });
            list.Add(new SchoolCharacteristic() { Question = SchoolCharacteristicsQuestions.SCHOOL_OVERALL_PHASE, Value = latestSchoolData?.SchoolOverallPhase });
            list.Add(new SchoolCharacteristic() { Question = SchoolCharacteristicsQuestions.TYPEOF_ESTABLISHMENT, Value = latestSchoolData?.SchoolType });
            list.Add(new SchoolCharacteristic() { Question = SchoolCharacteristicsQuestions.URBAN_RURAL, Value = latestSchoolData?.UrbanRural });
            list.Add(new SchoolCharacteristic() { Question = SchoolCharacteristicsQuestions.GOVERNMENT_OFFICE, Value = latestSchoolData?.GovernmentOfficeRegion });
            //list.Add(new SchoolCharacteristic() { Question = SchoolCharacteristicsQuestions.LONDON_BOROUGH, Value = latestSchoolData?.LondonBorough });
            list.Add(new SchoolCharacteristic() { Question = SchoolCharacteristicsQuestions.LONDON_WEIGHTING, Value = latestSchoolData?.LondonWeighting });
            list.Add(new SchoolCharacteristic() { Question = SchoolCharacteristicsQuestions.PERCENTAGE_OF_ELIGIBLE_FREE_SCHOOL_MEALS, Value = latestSchoolData?.PercentageOfEligibleFreeSchoolMeals.ToString() });
            list.Add(new SchoolCharacteristic() { Question = SchoolCharacteristicsQuestions.PERCENTAGE_OF_PUPILS_WITH_STATEMENT_OF_SEN, Value = latestSchoolData?.PercentageOfPupilsWithSen.ToString() });
            list.Add(new SchoolCharacteristic() { Question = SchoolCharacteristicsQuestions.PERCENTAGE_OF_PUPILS_ON_SEN_REGISTER, Value = latestSchoolData?.PercentageOfPupilsWithoutSen.ToString() });
            list.Add(new SchoolCharacteristic() { Question = SchoolCharacteristicsQuestions.PERCENTAGE_OF_PUPILS_WITH_EAL, Value = latestSchoolData?.PercentageOfPupilsWithEal.ToString() });
            list.Add(new SchoolCharacteristic() { Question = SchoolCharacteristicsQuestions.PERCENTAGE_BOARDERS, Value = latestSchoolData?.PercentageBoarders.ToString() });
            list.Add(new SchoolCharacteristic() { Question = SchoolCharacteristicsQuestions.ADMISSIONS_POLICY, Value = latestSchoolData?.AdmissionPolicy });
            list.Add(new SchoolCharacteristic() { Question = SchoolCharacteristicsQuestions.PFI, Value = latestSchoolData?.Pfi });
            list.Add(new SchoolCharacteristic() { Question = SchoolCharacteristicsQuestions.DOES_THE_SCHOOL_HAVE6_FORM, Value = latestSchoolData?.DoesTheSchoolHave6Form });
            list.Add(new SchoolCharacteristic() { Question = SchoolCharacteristicsQuestions.NUMBER_IN6_FORM, Value = latestSchoolData?.NumberIn6Form.ToString() });
            list.Add(new SchoolCharacteristic() { Question = SchoolCharacteristicsQuestions.LOWEST_AGE_PUPILS, Value = latestSchoolData?.LowestAgePupils.ToString() });
            list.Add(new SchoolCharacteristic() { Question = SchoolCharacteristicsQuestions.HIGHEST_AGE_PUPILS, Value = latestSchoolData?.HighestAgePupils.ToString() });
            list.Add(new SchoolCharacteristic() { Question = SchoolCharacteristicsQuestions.PERCENTAGE_QUALIFIED_TEACHERS, Value = latestSchoolData?.PercentageQualifiedTeachers.ToString() });
            list.Add(new SchoolCharacteristic() { Question = SchoolCharacteristicsQuestions.FULL_TIME_TA, Value = latestSchoolData?.FullTimeTA.ToString() });
            list.Add(new SchoolCharacteristic() { Question = SchoolCharacteristicsQuestions.FULL_TIME_OTHER, Value = latestSchoolData?.FullTimeOther.ToString() });
            list.Add(new SchoolCharacteristic() { Question = SchoolCharacteristicsQuestions.FULL_TIME_AUX, Value = latestSchoolData?.FullTimeAux.ToString() });
            list.Add(new SchoolCharacteristic() { Question = SchoolCharacteristicsQuestions.SCHOOL_WORKFORCE_FTE, Value = latestSchoolData?.TotalSchoolWorkforceFTE.ToString() });
            list.Add(new SchoolCharacteristic() { Question = SchoolCharacteristicsQuestions.NUMBER_OF_TEACHERS_FTE, Value = latestSchoolData?.TotalNumberOfTeachersFTE.ToString() });
            list.Add(new SchoolCharacteristic() { Question = SchoolCharacteristicsQuestions.SENIOR_LEADERSHIP_FTE, Value = latestSchoolData?.TotalSeniorTeachersFTE.ToString() });
            list.Add(new SchoolCharacteristic() { Question = SchoolCharacteristicsQuestions.KS2_ACTUAL, Value = latestSchoolData?.Ks2Actual.ToString() });
            list.Add(new SchoolCharacteristic() { Question = SchoolCharacteristicsQuestions.KS2_PROGRESS, Value = latestSchoolData?.Ks2Progress.ToString() });
            list.Add(new SchoolCharacteristic() { Question = SchoolCharacteristicsQuestions.AVERAGE_ATTAINMENT_8, Value = latestSchoolData?.AvAtt8.ToString() });
            list.Add(new SchoolCharacteristic() { Question = SchoolCharacteristicsQuestions.PROGRESS_8_MEASURE, Value = latestSchoolData?.P8Mea.ToString() });
            list.Add(new SchoolCharacteristic() { Question = SchoolCharacteristicsQuestions.OFSTED_RATING, Value = latestSchoolData?.OfstedRating });
            list.Add(new SchoolCharacteristic() { Question = SchoolCharacteristicsQuestions.SPECIFIC_LEARNING_DIFFICULTY, Value = latestSchoolData?.SpecificLearningDifficulty.ToString() });
            list.Add(new SchoolCharacteristic() { Question = SchoolCharacteristicsQuestions.MODERATE_LEARNING_DIFFICULTY, Value = latestSchoolData?.ModerateLearningDifficulty.ToString() });
            list.Add(new SchoolCharacteristic() { Question = SchoolCharacteristicsQuestions.SEVERE_LEARNING_DIFFICULTY, Value = latestSchoolData?.SevereLearningDifficulty.ToString() });
            list.Add(new SchoolCharacteristic() { Question = SchoolCharacteristicsQuestions.PROF_LEARNING_DIFFICULTY, Value = latestSchoolData?.ProfLearningDifficulty.ToString() });
            list.Add(new SchoolCharacteristic() { Question = SchoolCharacteristicsQuestions.SOCIAL_HEALTH, Value = latestSchoolData?.SocialHealth.ToString() });
            list.Add(new SchoolCharacteristic() { Question = SchoolCharacteristicsQuestions.SPEECH_NEEDS, Value = latestSchoolData?.SpeechNeeds.ToString() });
            list.Add(new SchoolCharacteristic() { Question = SchoolCharacteristicsQuestions.HEARING_IMPAIRMENT, Value = latestSchoolData?.HearingImpairment.ToString() });
            list.Add(new SchoolCharacteristic() { Question = SchoolCharacteristicsQuestions.VISUAL_IMPAIRMENT, Value = latestSchoolData?.VisualImpairment.ToString() });
            list.Add(new SchoolCharacteristic() { Question = SchoolCharacteristicsQuestions.MULTI_SENSORY_IMPAIRMENT, Value = latestSchoolData?.MultiSensoryImpairment.ToString() });
            list.Add(new SchoolCharacteristic() { Question = SchoolCharacteristicsQuestions.PHYSICAL_DISABILITY, Value = latestSchoolData?.PhysicalDisability.ToString() });
            list.Add(new SchoolCharacteristic() { Question = SchoolCharacteristicsQuestions.AUTISTIC_DISORDER, Value = latestSchoolData?.AutisticDisorder.ToString() });
            list.Add(new SchoolCharacteristic() { Question = SchoolCharacteristicsQuestions.OTHER_LEARNING_DIFF, Value = latestSchoolData?.OtherLearningDifficulty.ToString() });

            return list;
        }
    }
}
