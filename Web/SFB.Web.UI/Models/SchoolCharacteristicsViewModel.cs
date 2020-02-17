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
        public BenchmarkCriteriaMultipleChoiceVM SchoolPhaseMultipleChoiceVM { get; set; }
        public BenchmarkCriteriaMultipleChoiceVM TypeOfEstabMaintainedMultipleChoiceVM { get; set; }
        public BenchmarkCriteriaMultipleChoiceVM TypeOfEstabAcademiesMultipleChoiceVM { get; set; }
        public BenchmarkCriteriaMultipleChoiceVM TypeOfEstabAllMultipleChoiceVM { get; set; }
        public BenchmarkCriteriaRangeVM PercEligibleSchoolMealsRangeVm { get; set; }
        public BenchmarkCriteriaRangeVM PercPupilsSenRegisterRangeVm { get; set; }
        public BenchmarkCriteriaRangeVM PercPupilsWithSenRangeVm { get; set; }
        public BenchmarkCriteriaRangeVM PercPupilsWithEalRangeVm { get; set; }
        public BenchmarkCriteriaMultipleChoiceVM UrbanRuralMultipleChoiceVM { get; set; }
        public BenchmarkCriteriaMultipleChoiceVM LondonWeightingMultipleChoiceVM { get; set; }
        public BenchmarkCriteriaMultipleChoiceVM GovernmentOfficeMultipleChoiceVM { get; set; }
        public BenchmarkCriteriaMultipleChoiceVM PfiMultipleChoiceVM { get; set; }
        public BenchmarkCriteriaRangeVM NumberIn6FormRangeVm { get; set; }
        public BenchmarkCriteriaMultipleChoiceVM GenderMultipleChoiceVM { get; set; }
        public BenchmarkCriteriaRangeVM LowAgeRangeVm { get; set; }
        public BenchmarkCriteriaRangeVM HighAgeRangeVm { get; set; }
        public BenchmarkCriteriaRangeVM PercBoardersRangeVm { get; set; }
        public BenchmarkCriteriaMultipleChoiceVM AdmissionMultipleChoiceVM { get; set; }

        public SchoolCharacteristicsViewModel(SchoolViewModel school, SchoolComparisonListModel comparisonList, BenchmarkCriteria benchmarkCriteria)
        {
            base.ComparisonList = comparisonList;
            this.BenchmarkSchool = school;
            this.SchoolCharacteristics = BuildSchoolCharacteristics(school);
            this.BenchmarkCriteria = benchmarkCriteria;
            this.BuildGeneralCriteriaVMs();
        }

        public SchoolCharacteristicsViewModel(SchoolViewModelWithNoDefaultSchool school, SchoolComparisonListModel comparisonList, BenchmarkCriteria benchmarkCriteria)
        {
            base.ComparisonList = comparisonList;
            this.SchoolCharacteristics = BuildSchoolCharacteristics(school);
            this.BenchmarkCriteria = benchmarkCriteria;
            this.BuildGeneralCriteriaVMs();
        }

        public string this[string question]
        {
            get
            {
                return SchoolCharacteristics.Find(s => s.Question == question).Value;
            }
        }

        private void BuildGeneralCriteriaVMs()
        {
            NumberOfPupilsCriteriaRangeVm = new BenchmarkCriteriaRangeVM(
                        question: SchoolCharacteristicsQuestions.NUMBER_OF_PUPILS,
                        homeSchoolValue: this[SchoolCharacteristicsQuestions.NUMBER_OF_PUPILS],
                        homeSchoolName: BenchmarkSchool.Name,
                        elementName: "NoPupil",
                        minValue: BenchmarkCriteria.MinNoPupil,
                        maxValue: BenchmarkCriteria.MaxNoPupil);

            SchoolPhaseMultipleChoiceVM = new BenchmarkCriteriaMultipleChoiceVM(
                        question: SchoolCharacteristicsQuestions.SCHOOL_PHASE,
                        homeSchoolValue: this[SchoolCharacteristicsQuestions.SCHOOL_PHASE],
                        homeSchoolName: BenchmarkSchool.Name,
                        elementName: "SchoolPhase",
                        options: new List<OptionVM>
                        {
                            new OptionVM("Nursery", "Nursery", BenchmarkCriteria.SchoolPhase),
                            new OptionVM("Infant and junior", "Infant and junior", BenchmarkCriteria.SchoolPhase),
                            new OptionVM("Infant", "Infant", BenchmarkCriteria.SchoolPhase),
                            new OptionVM("Junior", "Junior", BenchmarkCriteria.SchoolPhase),
                            new OptionVM("Middle deemed primary", "Middle deemed primary", BenchmarkCriteria.SchoolPhase),
                            new OptionVM("Middle deemed secondary", "Middle deemed secondary", BenchmarkCriteria.SchoolPhase),
                            new OptionVM("Secondary", "Secondary", BenchmarkCriteria.SchoolPhase),
                            new OptionVM("Special", "Special", BenchmarkCriteria.SchoolPhase),
                            new OptionVM("Pupil referral unit", "Pupil referral unit", BenchmarkCriteria.SchoolPhase),
                            new OptionVM("All-through", "All-through", BenchmarkCriteria.SchoolPhase)
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
                });

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
                });

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
                });

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

            UrbanRuralMultipleChoiceVM = new BenchmarkCriteriaMultipleChoiceVM(
                question: SchoolCharacteristicsQuestions.URBAN_RURAL,
                homeSchoolValue: this[SchoolCharacteristicsQuestions.URBAN_RURAL],
                homeSchoolName: BenchmarkSchool.Name,
                elementName: "UrbanRural",
                options: new List<OptionVM>
                {
                    new OptionVM("Hamlet and isolated dwelling", "Hamlet and isolated dwelling", BenchmarkCriteria.UrbanRural),
                    new OptionVM("Rural and village", "Rural and village", BenchmarkCriteria.UrbanRural),
                    new OptionVM("Town and fringe", "Town and fringe", BenchmarkCriteria.UrbanRural),
                    new OptionVM("Urban and city", "Urban and city", BenchmarkCriteria.UrbanRural),
                    new OptionVM("Conurbation", "Conurbation", BenchmarkCriteria.UrbanRural),
                });

            LondonWeightingMultipleChoiceVM = new BenchmarkCriteriaMultipleChoiceVM(
                question: SchoolCharacteristicsQuestions.LONDON_WEIGHTING,
                homeSchoolValue: this[SchoolCharacteristicsQuestions.LONDON_WEIGHTING],
                homeSchoolName: BenchmarkSchool.Name,
                elementName: "LondonWeighting",
                options: new List<OptionVM>
                {
                    new OptionVM("Inner", "Inner", BenchmarkCriteria.LondonWeighting),
                    new OptionVM("Outer", "Outer", BenchmarkCriteria.LondonWeighting),
                    new OptionVM("Neither", "Neither", BenchmarkCriteria.LondonWeighting)
                });

            GovernmentOfficeMultipleChoiceVM = new BenchmarkCriteriaMultipleChoiceVM(
                question: SchoolCharacteristicsQuestions.GOVERNMENT_OFFICE,
                homeSchoolValue: this[SchoolCharacteristicsQuestions.GOVERNMENT_OFFICE],
                homeSchoolName: BenchmarkSchool.Name,
                elementName: "GovernmentOffice",
                options: new List<OptionVM>
                {
                    new OptionVM("East Midlands", "East Midlands", BenchmarkCriteria.GovernmentOffice),
                    new OptionVM("East of England", "East of England", BenchmarkCriteria.GovernmentOffice),
                    new OptionVM("London", "London", BenchmarkCriteria.GovernmentOffice),
                    new OptionVM("North East", "North East", BenchmarkCriteria.GovernmentOffice),
                    new OptionVM("North West", "North West", BenchmarkCriteria.GovernmentOffice),
                    new OptionVM("South East", "South East", BenchmarkCriteria.GovernmentOffice),
                    new OptionVM("South West", "South West", BenchmarkCriteria.GovernmentOffice),
                    new OptionVM("West Midlands", "West Midlands", BenchmarkCriteria.GovernmentOffice),
                    new OptionVM("Yorkshire and the Humber", "Yorkshire and the Humber", BenchmarkCriteria.GovernmentOffice)
                });

            PfiMultipleChoiceVM = new BenchmarkCriteriaMultipleChoiceVM(
                question: SchoolCharacteristicsQuestions.PFI,
                homeSchoolValue: this[SchoolCharacteristicsQuestions.PFI],
                homeSchoolName: BenchmarkSchool.Name,
                elementName: "Pfi",
                options: new List<OptionVM>
                {
                    new OptionVM("Part of PFI", "Part of PFI", BenchmarkCriteria.Pfi),
                    new OptionVM("Not part of PFI", "Not Part Of PFI", BenchmarkCriteria.Pfi)
                });

            NumberIn6FormRangeVm = new BenchmarkCriteriaRangeVM(
                question: SchoolCharacteristicsQuestions.NUMBER_IN6_FORM,
                homeSchoolValue: this[SchoolCharacteristicsQuestions.NUMBER_IN6_FORM],
                homeSchoolName: BenchmarkSchool.Name,
                elementName: "NoSixthForm",
                minValue: BenchmarkCriteria.MinNoSixthForm,
                maxValue: BenchmarkCriteria.MaxNoSixthForm
            );

            GenderMultipleChoiceVM = new BenchmarkCriteriaMultipleChoiceVM(
                question: SchoolCharacteristicsQuestions.GENDER_OF_PUPILS,
                homeSchoolValue: this[SchoolCharacteristicsQuestions.GENDER_OF_PUPILS],
                homeSchoolName: BenchmarkSchool.Name,
                elementName: "Gender",
                options: new List<OptionVM>
                {
                    new OptionVM("Boys", "Boys", BenchmarkCriteria.Gender),
                    new OptionVM("Girls", "Girls", BenchmarkCriteria.Gender),
                    new OptionVM("Mixed", "Mixed", BenchmarkCriteria.Gender)
                }
            );

            LowAgeRangeVm = new BenchmarkCriteriaRangeVM(
                question: SchoolCharacteristicsQuestions.LOWEST_AGE_PUPILS,
                homeSchoolValue: this[SchoolCharacteristicsQuestions.LOWEST_AGE_PUPILS],
                homeSchoolName: BenchmarkSchool.Name,
                elementName: "LowestAgePupils",
                minValue: BenchmarkCriteria.MinLowestAgePupils,
                maxValue: BenchmarkCriteria.MaxLowestAgePupils
            );

            HighAgeRangeVm = new BenchmarkCriteriaRangeVM(
                question: SchoolCharacteristicsQuestions.HIGHEST_AGE_PUPILS,
                homeSchoolValue: this[SchoolCharacteristicsQuestions.HIGHEST_AGE_PUPILS],
                homeSchoolName: BenchmarkSchool.Name,
                elementName: "HighestAgePupils",
                minValue: BenchmarkCriteria.MinHighestAgePupils,
                maxValue: BenchmarkCriteria.MaxHighestAgePupils
            );

            PercBoardersRangeVm = new BenchmarkCriteriaRangeVM(
                question: SchoolCharacteristicsQuestions.PERCENTAGE_BOARDERS,
                homeSchoolValue: this[SchoolCharacteristicsQuestions.PERCENTAGE_BOARDERS],
                homeSchoolName: BenchmarkSchool.Name,
                elementName: "PerBoarders",
                minValue: BenchmarkCriteria.MinPerBoarders,
                maxValue: BenchmarkCriteria.MaxPerBoarders
            );

            AdmissionMultipleChoiceVM = new BenchmarkCriteriaMultipleChoiceVM(
                question: SchoolCharacteristicsQuestions.ADMISSIONS_POLICY,
                homeSchoolValue: this[SchoolCharacteristicsQuestions.ADMISSIONS_POLICY],
                homeSchoolName: BenchmarkSchool.Name,
                elementName: "AdmPolicy",
                options: new List<OptionVM>
                {
                    new OptionVM("Comprehensive", "Comprehensive", BenchmarkCriteria.AdmPolicy),
                    new OptionVM("Selective", "Selective", BenchmarkCriteria.AdmPolicy),
                    new OptionVM("Modern", "Modern", BenchmarkCriteria.AdmPolicy),
                    new OptionVM("Non-selective", "Non-selective", BenchmarkCriteria.AdmPolicy),
                    new OptionVM("N/A", "N/A", BenchmarkCriteria.AdmPolicy),
                }
            );
        }

        private List<SchoolCharacteristic> BuildSchoolCharacteristics(SchoolViewModel schoolVM)
        {
            var latestSchoolData = schoolVM?.HistoricalFinancialDataModels?.Last();
            var list = new List<SchoolCharacteristic>();
            list.Add(new SchoolCharacteristic() { Question = SchoolCharacteristicsQuestions.NUMBER_OF_PUPILS, Value = latestSchoolData == null ? null : latestSchoolData?.PupilCount + " pupils" });
            list.Add(new SchoolCharacteristic() { Question = SchoolCharacteristicsQuestions.GENDER_OF_PUPILS, Value = latestSchoolData?.Gender });
            list.Add(new SchoolCharacteristic() { Question = SchoolCharacteristicsQuestions.SCHOOL_PHASE, Value = latestSchoolData?.SchoolPhase });
            //list.Add(new SchoolCharacteristic() { Question = SchoolCharacteristicsQuestions.SCHOOL_OVERALL_PHASE, Value = latestSchoolData?.SchoolOverallPhase });
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
