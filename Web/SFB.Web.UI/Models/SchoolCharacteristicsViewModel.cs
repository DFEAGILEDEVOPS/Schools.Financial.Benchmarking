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

        public BenchmarkCriteriaRangeVM NumberOfPupilsCriteriaRangeVm { get; set; }
        public BenchmarkCriteriaMultipleChoiceVM SchoolOverallPhaseMultipleChoiceVM { get; set; }
        public BenchmarkCriteriaMultipleChoiceVM TypeOfEstabMaintainedMultipleChoiceVM { get; set; }
        public BenchmarkCriteriaMultipleChoiceVM TypeOfEstabAcademiesMultipleChoiceVM { get; set; }
        public BenchmarkCriteriaMultipleChoiceVM TypeOfEstabAllMultipleChoiceVM { get; set; }
        public BenchmarkCriteriaRangeVM PercEligibleSchoolMealsRangeVm { get; set; }
        public BenchmarkCriteriaRangeVM PercPupilsSenRegisterRangeVm { get; set; }
        public BenchmarkCriteriaRangeVM PercPupilsWithSenRangeVm { get; set; }
        public BenchmarkCriteriaRangeVM PercPupilsWithEalRangeVm { get; set; }

        public SchoolCharacteristicsViewModel(SchoolViewModel school, SchoolComparisonListModel comparisonList, BenchmarkCriteria benchmarkCriteria)
        {
            base.ComparisonList = comparisonList;
            this.BenchmarkSchool = school;
            this.SchoolCharacteristics = BuildSchoolCharacteristics(school);
            this.BenchmarkCriteria = benchmarkCriteria;
            this.BuildCriteriaVMs();
        }

        public SchoolCharacteristicsViewModel(SchoolViewModelWithNoDefaultSchool school, SchoolComparisonListModel comparisonList, BenchmarkCriteria benchmarkCriteria)
        {
            base.ComparisonList = comparisonList;
            this.SchoolCharacteristics = BuildSchoolCharacteristics(school);
            this.BenchmarkCriteria = benchmarkCriteria;
            this.BuildCriteriaVMs();
        }

        public string this[string question]
        {
            get
            {
                return SchoolCharacteristics.Find(s => s.Question == question).Value;
            }
        }

        private void BuildCriteriaVMs()
        {
            NumberOfPupilsCriteriaRangeVm = new BenchmarkCriteriaRangeVM(
                        question: SchoolCharacteristicsQuestions.NUMBER_OF_PUPILS,
                        homeSchoolValue: this[SchoolCharacteristicsQuestions.NUMBER_OF_PUPILS],
                        homeSchoolName: BenchmarkSchool.Name,
                        elementName: "NoPupil",
                        minValue: BenchmarkCriteria.MinNoPupil,
                        maxValue: BenchmarkCriteria.MaxNoPupil);

            SchoolOverallPhaseMultipleChoiceVM = new BenchmarkCriteriaMultipleChoiceVM(
                        question: SchoolCharacteristicsQuestions.SCHOOL_OVERALL_PHASE,
                        homeSchoolValue: this[SchoolCharacteristicsQuestions.SCHOOL_OVERALL_PHASE],
                        homeSchoolName: BenchmarkSchool.Name,
                        elementName: "SchoolOverallPhase",
                        options: new List<OptionVM>
                        {
                                new OptionVM("Nursery", "Nursery", BenchmarkCriteria.SchoolOverallPhase),
                                new OptionVM("Primary", "Primary", BenchmarkCriteria.SchoolOverallPhase,
                                    subMultipleChoiceOptions: new SubOptionsVM<BenchmarkCriteriaMultipleChoiceVM>
                                    {
                                        Name = "PrimarySubOptions",
                                        SubOptions =  new List<BenchmarkCriteriaMultipleChoiceVM>
                                        {
                                            new BenchmarkCriteriaMultipleChoiceVM(
                                                question: SchoolCharacteristicsQuestions.SCHOOL_PHASE,
                                                homeSchoolValue: this[SchoolCharacteristicsQuestions.SCHOOL_PHASE],
                                                homeSchoolName: BenchmarkSchool.Name,
                                                elementName: "SchoolPhase",
                                                options: new List<OptionVM>
                                                {
                                                    new OptionVM("Infant and junior", "Infant and junior", BenchmarkCriteria.SchoolPhase),
                                                    new OptionVM("Infant", "Infant", BenchmarkCriteria.SchoolPhase),
                                                    new OptionVM("Junior", "Junior", BenchmarkCriteria.SchoolPhase),
                                                    new OptionVM("Middle deemed primary", "Middle deemed primary", BenchmarkCriteria.SchoolPhase)
                                                }
                                            )
                                        }
                                    }
                                ),
                                new OptionVM("Secondary", "Secondary", BenchmarkCriteria.SchoolOverallPhase,
                                    subMultipleChoiceOptions: new SubOptionsVM<BenchmarkCriteriaMultipleChoiceVM>
                                    {
                                        Name = "SecondarySubOptions",
                                        SubOptions =  new List<BenchmarkCriteriaMultipleChoiceVM>
                                        {
                                            new BenchmarkCriteriaMultipleChoiceVM(
                                                question: SchoolCharacteristicsQuestions.SCHOOL_PHASE,
                                                homeSchoolValue: this[SchoolCharacteristicsQuestions.SCHOOL_PHASE],
                                                homeSchoolName: BenchmarkSchool.Name,
                                                elementName: "SchoolPhase",
                                                options: new List<OptionVM>
                                                {
                                                    new OptionVM("Middle deemed secondary", "Middle deemed secondary", BenchmarkCriteria.SchoolPhase),
                                                    new OptionVM("Secondary", "Secondary", BenchmarkCriteria.SchoolPhase)
                                                }
                                            )
                                        }
                                    }
                                ),
                                new OptionVM("Special", "Special", BenchmarkCriteria.SchoolOverallPhase),
                                new OptionVM("Pupil referral unit", "Pupil referral unit", BenchmarkCriteria.SchoolOverallPhase),
                                new OptionVM("All-through", "All-through", BenchmarkCriteria.SchoolOverallPhase)
                        });

            TypeOfEstabMaintainedMultipleChoiceVM = new BenchmarkCriteriaMultipleChoiceVM(
                question: SchoolCharacteristicsQuestions.TYPEOF_ESTABLISHMENT,
                homeSchoolValue: this[SchoolCharacteristicsQuestions.TYPEOF_ESTABLISHMENT],
                homeSchoolName: BenchmarkSchool.Name,
                elementName: "TypeOfEstablishment",
                options: new List<OptionVM>
                {
                    new OptionVM("Nursery", "Nursery", BenchmarkCriteria.TypeOfEstablishment),
                    new OptionVM("Community", "Community school", BenchmarkCriteria.TypeOfEstablishment),
                    new OptionVM("Community special", "Community special", BenchmarkCriteria.TypeOfEstablishment),
                    new OptionVM("Foundation", "Foundation school", BenchmarkCriteria.TypeOfEstablishment),
                    new OptionVM("Foundation special", "Foundation special", BenchmarkCriteria.TypeOfEstablishment),
                    new OptionVM("Voluntary aided", "Voluntary aided school", BenchmarkCriteria.TypeOfEstablishment),
                    new OptionVM("Voluntary controlled", "Voluntary controlled", BenchmarkCriteria.TypeOfEstablishment),
                    new OptionVM("Pupil referral unit", "Pupil referral unit", BenchmarkCriteria.TypeOfEstablishment),
                    new OptionVM("Miscellaneous", "Miscellaneous", BenchmarkCriteria.TypeOfEstablishment),
                    new OptionVM("Studio school", "Studio school", BenchmarkCriteria.TypeOfEstablishment),
                    new OptionVM("Academy converter", "Academy converter", BenchmarkCriteria.TypeOfEstablishment),
                    new OptionVM("Academy special converter", "Academy special converter", BenchmarkCriteria.TypeOfEstablishment),
                }
                );

            TypeOfEstabAcademiesMultipleChoiceVM = new BenchmarkCriteriaMultipleChoiceVM(
                question: SchoolCharacteristicsQuestions.TYPEOF_ESTABLISHMENT,
                homeSchoolValue: this[SchoolCharacteristicsQuestions.TYPEOF_ESTABLISHMENT],
                homeSchoolName: BenchmarkSchool.Name,
                elementName: "TypeOfEstablishment",
                options: new List<OptionVM>
                {
                    new OptionVM("Academy converter", "Academy converter", BenchmarkCriteria.TypeOfEstablishment),
                    new OptionVM("Academy special converter", "Academy special converter", BenchmarkCriteria.TypeOfEstablishment),
                    new OptionVM("Academy special sponsor led", "Academy special sponsor led", BenchmarkCriteria.TypeOfEstablishment),
                    new OptionVM("Academy sponsor led", "Academy sponsor led", BenchmarkCriteria.TypeOfEstablishment),
                    new OptionVM("City technology college", "City technology college", BenchmarkCriteria.TypeOfEstablishment),
                    new OptionVM("Free school", "Free school", BenchmarkCriteria.TypeOfEstablishment),
                    new OptionVM("University technical college", "University technical college", BenchmarkCriteria.TypeOfEstablishment),
                    new OptionVM("Studio school", "Studio school", BenchmarkCriteria.TypeOfEstablishment),
                    new OptionVM("Free school special", "Free school special", BenchmarkCriteria.TypeOfEstablishment),
                    new OptionVM("Free school - alternative provision", "Free school alternative provision", BenchmarkCriteria.TypeOfEstablishment),
                    new OptionVM("Pupil referral unit", "Pupil referral unit", BenchmarkCriteria.TypeOfEstablishment),
                    new OptionVM("Academy alternative provision converter", "Academy alternative provision converter", BenchmarkCriteria.TypeOfEstablishment),
                    new OptionVM("Academy alternative provision sponsor led", "Academy alternative provision sponsor led", BenchmarkCriteria.TypeOfEstablishment)
                }
                );

            TypeOfEstabAllMultipleChoiceVM = new BenchmarkCriteriaMultipleChoiceVM(
                question: SchoolCharacteristicsQuestions.TYPEOF_ESTABLISHMENT,
                homeSchoolValue: this[SchoolCharacteristicsQuestions.TYPEOF_ESTABLISHMENT],
                homeSchoolName: BenchmarkSchool.Name,
                elementName: "TypeOfEstablishment",
                options: new List<OptionVM>
                {
                    new OptionVM("Nursery", "Nursery", BenchmarkCriteria.TypeOfEstablishment),
                    new OptionVM("Community", "Community school", BenchmarkCriteria.TypeOfEstablishment),
                    new OptionVM("Community special", "Community special", BenchmarkCriteria.TypeOfEstablishment),
                    new OptionVM("Foundation", "Foundation school", BenchmarkCriteria.TypeOfEstablishment),
                    new OptionVM("Foundation special", "Foundation special", BenchmarkCriteria.TypeOfEstablishment),
                    new OptionVM("Voluntary aided", "Voluntary aided school", BenchmarkCriteria.TypeOfEstablishment),
                    new OptionVM("Voluntary controlled", "Voluntary controlled", BenchmarkCriteria.TypeOfEstablishment),
                    new OptionVM("Pupil referral unit", "Pupil referral unit", BenchmarkCriteria.TypeOfEstablishment),
                    new OptionVM("Academy converter", "Academy converter", BenchmarkCriteria.TypeOfEstablishment),
                    new OptionVM("Academy special converter", "Academy special converter", BenchmarkCriteria.TypeOfEstablishment),
                    new OptionVM("Academy sponsor led", "Academy sponsor led", BenchmarkCriteria.TypeOfEstablishment),
                    new OptionVM("Academy special sponsor led", "Academy special sponsor led", BenchmarkCriteria.TypeOfEstablishment),
                    new OptionVM("City technology college", "City technology college", BenchmarkCriteria.TypeOfEstablishment),
                    new OptionVM("Free school", "Free school", BenchmarkCriteria.TypeOfEstablishment),
                    new OptionVM("University technical college", "University technical college", BenchmarkCriteria.TypeOfEstablishment),
                    new OptionVM("Studio school", "Studio school", BenchmarkCriteria.TypeOfEstablishment),
                    new OptionVM("Free school special", "Free school special", BenchmarkCriteria.TypeOfEstablishment),
                    new OptionVM("Academy special sponsor led", "Academy special sponsor led", BenchmarkCriteria.TypeOfEstablishment),
                    new OptionVM("Free school - alternative provision", "Free school alternative provision", BenchmarkCriteria.TypeOfEstablishment),
                    new OptionVM("Academy alternative provision converter", "Academy alternative provision converter", BenchmarkCriteria.TypeOfEstablishment),
                    new OptionVM("Academy alternative provision sponsor led", "Academy alternative provision sponsor led", BenchmarkCriteria.TypeOfEstablishment),
                }
                );

            PercEligibleSchoolMealsRangeVm = new BenchmarkCriteriaRangeVM(
                question: SchoolCharacteristicsQuestions.PERCENTAGE_OF_ELIGIBLE_FREE_SCHOOL_MEALS,
                homeSchoolValue: this[SchoolCharacteristicsQuestions.PERCENTAGE_OF_ELIGIBLE_FREE_SCHOOL_MEALS],
                homeSchoolName: BenchmarkSchool.Name,
                elementName: "PerFSM",
                minValue: BenchmarkCriteria.MinPerFSM,
                maxValue: BenchmarkCriteria.MaxPerFSM
                );

            PercPupilsSenRegisterRangeVm = new BenchmarkCriteriaRangeVM(
                question: SchoolCharacteristicsQuestions.PERCENTAGE_OF_PUPILS_ON_SEN_REGISTER,
                homeSchoolValue: this[SchoolCharacteristicsQuestions.PERCENTAGE_OF_PUPILS_ON_SEN_REGISTER],
                homeSchoolName: BenchmarkSchool.Name,
                elementName: "PerSENReg",
                minValue: BenchmarkCriteria.MinPerSENReg,
                maxValue: BenchmarkCriteria.MaxPerSENReg
                );

            PercPupilsWithSenRangeVm = new BenchmarkCriteriaRangeVM(
                question: SchoolCharacteristicsQuestions.PERCENTAGE_OF_PUPILS_WITH_STATEMENT_OF_SEN,
                homeSchoolValue: this[SchoolCharacteristicsQuestions.PERCENTAGE_OF_PUPILS_WITH_STATEMENT_OF_SEN],
                homeSchoolName: BenchmarkSchool.Name,
                elementName: "PerSEN",
                minValue: BenchmarkCriteria.MinPerSEN,
                maxValue: BenchmarkCriteria.MaxPerSEN
                );

            PercPupilsWithEalRangeVm = new BenchmarkCriteriaRangeVM(
                question: SchoolCharacteristicsQuestions.PERCENTAGE_OF_PUPILS_WITH_EAL,
                homeSchoolValue: this[SchoolCharacteristicsQuestions.PERCENTAGE_OF_PUPILS_WITH_EAL],
                homeSchoolName: BenchmarkSchool.Name,
                elementName: "PerEAL",
                minValue: BenchmarkCriteria.MinPerEAL,
                maxValue: BenchmarkCriteria.MaxPerEAL
                );
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
