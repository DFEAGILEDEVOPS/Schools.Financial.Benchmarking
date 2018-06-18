using System;
using Microsoft.Azure.Documents;
using SFB.Web.Common;

namespace SFB.Web.Domain.Models
{
    public class FinancialDataModel : IEquatable<FinancialDataModel>
    {
        public string Term { get; }
        public Document FinancialDataDocumentModel { get; }
        public string Id { get; private set; }    
        public EstablishmentType EstabType{ get; private set;}


        public FinancialDataModel(){}

        public FinancialDataModel(string id, string term, Document financialDataDocumentModel, EstablishmentType estabType)
        {
            Id = id;
            Term = term;
            FinancialDataDocumentModel = financialDataDocumentModel;
            EstabType = estabType;
        }

        public int LaNumber => TryGetInt("LA");

        #region Financial Data

        public double PupilCount
        {
            get
            {
                try
                {
                    if (FinancialDataDocumentModel != null)
                    {
                        return FinancialDataDocumentModel.GetPropertyValue<double>(SchoolFinanceDBFieldNames.NO_PUPILS);
                    }
                    return 0;
                }
                catch (Exception)
                {
                    return 0;
                }
            }
        }

        public double SchoolCount
        {
            get
            {
                try
                {
                    if (FinancialDataDocumentModel != null)
                    {
                        return FinancialDataDocumentModel.GetPropertyValue<double>(SchoolFinanceDBFieldNames.NO_SCHOOLS);
                    }
                    return 0;
                }
                catch (Exception)
                {
                    return 0;
                }
            }
        }

        public double TeacherCount
        {
            get
            {
                try
                {
                    if (FinancialDataDocumentModel != null)
                    {

                        return FinancialDataDocumentModel.GetPropertyValue<double>("No Teachers");
                    }
                    return 0;
                }
                catch(Exception)
                {
                    return 0;
                }
            }
        }

        public string MatNo
        {
            get
            {
                try
                {
                    if (FinancialDataDocumentModel != null)
                    {
                        return FinancialDataDocumentModel.GetPropertyValue<string>("MATNumber");
                    }
                    return string.Empty;
                }
                catch (Exception)
                {
                    return string.Empty;
                }
            }
        }

        public bool IsSAT
        {
            get
            {
                try
                {
                    if (FinancialDataDocumentModel != null)
                    {
                        return FinancialDataDocumentModel.GetPropertyValue<string>("MAT SAT or Central Services").Equals("SAT");
                    }
                    return false;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }


        public bool IsDNS
        {
            get
            {
                try
                {
                    if (FinancialDataDocumentModel != null)
                    {
                        return FinancialDataDocumentModel.GetPropertyValue<bool>("DNS");
                    }
                    return false;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        public decimal TotalIncome
        {
            get
            {
                if (FinancialDataDocumentModel != null)
                {
                    return FinancialDataDocumentModel.GetPropertyValue<decimal>("Total Income");
                }
                return 0;
            }
        }

        public decimal TotalExpenditure
        {
            get
            {
                if (FinancialDataDocumentModel != null)
                {
                    return FinancialDataDocumentModel.GetPropertyValue<decimal>("Total Expenditure");
                }
                return 0;
            }
        }

        public decimal InYearBalance
        {
            get
            {
                if (FinancialDataDocumentModel != null)
                {
                    return FinancialDataDocumentModel.GetPropertyValue<decimal>("In Year Balance");
                }
                return 0;

            }
        }

        public int PeriodCoveredByReturn
        {
            get
            {
                try
                {
                    if (FinancialDataDocumentModel != null)
                    {
                        return FinancialDataDocumentModel.GetPropertyValue<int>("Period covered by return");
                    }
                }
                catch (Exception)
                {
                    return 0;
                }

                return 0;
            }
        }

        public bool PartialYearsPresentInSubSchools
        {
            get
            {
                if (FinancialDataDocumentModel != null)
                {
                    return FinancialDataDocumentModel.GetPropertyValue<bool>("PartialYearsPresent");
                }
                return false;
            }
        }

        public bool WorkforceDataPresent
        {
            get
            {
                if (FinancialDataDocumentModel != null)
                {
                    return FinancialDataDocumentModel.GetPropertyValue<bool>("WorkforcePresent");
                }
                return false;
            }
        }

        #endregion

        #region Criteria Data

        public string AdmissionPolicy => GetString(SchoolFinanceDBFieldNames.ADMISSION_POLICY);

        public string Gender => GetString(SchoolFinanceDBFieldNames.GENDER);

        public string SchoolOverallPhase => GetString(SchoolFinanceDBFieldNames.SCHOOL_OVERALL_PHASE);

        public string SchoolPhase => GetString(SchoolFinanceDBFieldNames.SCHOOL_PHASE);

        public string SchoolType => GetString(SchoolFinanceDBFieldNames.SCHOOL_TYPE);

        public string UrbanRural => GetString(SchoolFinanceDBFieldNames.URBAN_RURAL);

        public string GovernmentOfficeRegion => GetString(SchoolFinanceDBFieldNames.REGION);

        public string LondonBorough => GetString(SchoolFinanceDBFieldNames.LONDON_BOROUGH);

        public string LondonWeighting => GetString(SchoolFinanceDBFieldNames.LONDON_WEIGHT);

        public string PercentageOfEligibleFreeSchoolMeals => GetString(SchoolFinanceDBFieldNames.PERCENTAGE_FSM);

        public string PercentageOfPupilsWithSen => GetString(SchoolFinanceDBFieldNames.PERCENTAGE_OF_PUPILS_WITH_SEN);

        public string PercentageOfPupilsWithoutSen => GetString(SchoolFinanceDBFieldNames.PERCENTAGE_OF_PUPILS_WITHOUT_SEN);

        public string PercentageOfPupilsWithEal => GetString(SchoolFinanceDBFieldNames.PERCENTAGE_OF_PUPILS_WITH_EAL);

        public string PercentageBoarders => GetString(SchoolFinanceDBFieldNames.PERCENTAGE_BOARDERS);

        public string Pfi => GetString(SchoolFinanceDBFieldNames.PFI);

        public string DoesTheSchoolHave6Form => GetString(SchoolFinanceDBFieldNames.HAS_6_FORM);

        public string NumberIn6Form => GetString(SchoolFinanceDBFieldNames.NUMBER_IN_6_FORM);

        public string HighestAgePupils => GetString(SchoolFinanceDBFieldNames.HIGHEST_AGE_PUPILS);

        public string FullTimeAdmin => GetString(SchoolFinanceDBFieldNames.ADMIN_STAFF);

        public string FullTimeOther => GetString(SchoolFinanceDBFieldNames.FULL_TIME_OTHER);

        public string PercentageQualifiedTeachers => GetString(SchoolFinanceDBFieldNames.PERCENTAGE_QUALIFIED_TEACHERS);

        public string LowestAgePupils => GetString(SchoolFinanceDBFieldNames.LOWEST_AGE_PUPILS);

        public string FullTimeTA => GetString(SchoolFinanceDBFieldNames.FULL_TIME_TA);

        public string TotalSchoolWorkforceFTE => GetString(SchoolFinanceDBFieldNames.WORKFORCE_TOTAL);

        public string TotalNumberOfTeachersFTE => GetString(SchoolFinanceDBFieldNames.TEACHERS_TOTAL);

        public string TotalSeniorTeachersFTE => GetString(SchoolFinanceDBFieldNames.TEACHERS_LEADER);

        public string Ks2Actual => GetString(SchoolFinanceDBFieldNames.KS2_ACTUAL);

        public string Ks2Progress => GetString(SchoolFinanceDBFieldNames.KS2_PROGRESS);

        public string AvAtt8 => GetString(SchoolFinanceDBFieldNames.AVERAGE_ATTAINMENT);

        public string P8Mea => GetString(SchoolFinanceDBFieldNames.PROGRESS_8_MEASURE);

        public string OfstedRating => GetString(SchoolFinanceDBFieldNames.OFSTED_RATING_NAME);

        public string SpecificLearningDifficulty => GetString(SchoolFinanceDBFieldNames.SPECIFIC_LEARNING_DIFFICULTY);

        public string ModerateLearningDifficulty => GetString(SchoolFinanceDBFieldNames.MODERATE_LEARNING_DIFFICULTY);

        public string SevereLearningDifficulty => GetString(SchoolFinanceDBFieldNames.SEVERE_LEARNING_DIFFICULTY);

        public string ProfLearningDifficulty => GetString(SchoolFinanceDBFieldNames.PROF_LEARNING_DIFFICULTY);

        public string SocialHealth => GetString(SchoolFinanceDBFieldNames.SOCIAL_HEALTH);
        
        public string SpeechNeeds => GetString(SchoolFinanceDBFieldNames.SPEECH_NEEDS);

        public string HearingImpairment => GetString(SchoolFinanceDBFieldNames.HEARING_IMPAIRMENT);

        public string VisualImpairment => GetString(SchoolFinanceDBFieldNames.VISUAL_IMPAIRMENT);

        public string MultiSensoryImpairment => GetString(SchoolFinanceDBFieldNames.MULTI_SENSORY_IMPAIRMENT);

        public string PhysicalDisability => GetString(SchoolFinanceDBFieldNames.PHYSICAL_DISABILITY);

        public string AutisticDisorder => GetString(SchoolFinanceDBFieldNames.AUTISTIC_DISORDER);

        public string OtherLearningDifficulty => GetString(SchoolFinanceDBFieldNames.OTHER_LEARNING_DIFF);

        public string CrossPhaseBreakdownPrimary => FinancialDataDocumentModel.GetPropertyValue<Document>(SchoolFinanceDBFieldNames.SCHOOL_OVERALL_PHASE_BREAKDOWN).GetPropertyValue<String>(SchoolFinanceDBFieldNames.SCHOOL_OVERALL_PHASE_CROSS_PRIMARY);
        public string CrossPhaseBreakdownSecondary => FinancialDataDocumentModel.GetPropertyValue<Document>(SchoolFinanceDBFieldNames.SCHOOL_OVERALL_PHASE_BREAKDOWN).GetPropertyValue<String>(SchoolFinanceDBFieldNames.SCHOOL_OVERALL_PHASE_CROSS_SECONDARY);
        public string CrossPhaseBreakdownSpecial => FinancialDataDocumentModel.GetPropertyValue<Document>(SchoolFinanceDBFieldNames.SCHOOL_OVERALL_PHASE_BREAKDOWN).GetPropertyValue<String>(SchoolFinanceDBFieldNames.SCHOOL_OVERALL_PHASE_CROSS_SPECIAL);
        public string CrossPhaseBreakdownPru => FinancialDataDocumentModel.GetPropertyValue<Document>(SchoolFinanceDBFieldNames.SCHOOL_OVERALL_PHASE_BREAKDOWN).GetPropertyValue<String>(SchoolFinanceDBFieldNames.SCHOOL_OVERALL_PHASE_CROSS_PRU);
        public string CrossPhaseBreakdownAP => FinancialDataDocumentModel.GetPropertyValue<Document>(SchoolFinanceDBFieldNames.SCHOOL_OVERALL_PHASE_BREAKDOWN).GetPropertyValue<String>(SchoolFinanceDBFieldNames.SCHOOL_OVERALL_PHASE_CROSS_AP);
        public string CrossPhaseBreakdownAT => FinancialDataDocumentModel.GetPropertyValue<Document>(SchoolFinanceDBFieldNames.SCHOOL_OVERALL_PHASE_BREAKDOWN).GetPropertyValue<String>(SchoolFinanceDBFieldNames.SCHOOL_OVERALL_PHASE_CROSS_AT);

        #endregion

        public decimal? GetDecimal(string fieldName)
        {
            decimal? result;
            try
            {
                result = FinancialDataDocumentModel.GetPropertyValue<decimal?>(fieldName);
            }
            catch
            {
                return null;
            }
            
            return result;
        }

        public bool Equals(FinancialDataModel other)
        {
            return (this.Id == other.Id);
        }

        public string GetString(string fieldName)
        {
            if (FinancialDataDocumentModel != null)
            {
                return FinancialDataDocumentModel.GetPropertyValue<string>(fieldName);
            }
            return string.Empty;
        }

        public int GetInt(string fieldName)
        {
            if (FinancialDataDocumentModel != null)
            {
                return FinancialDataDocumentModel.GetPropertyValue<int>(fieldName);
            }
            return 0;
        }

        private int TryGetInt(string fieldName)
        {
            if (FinancialDataDocumentModel != null)
            {
                try
                {
                    return FinancialDataDocumentModel.GetPropertyValue<int>(fieldName);
                }
                catch
                {
                    return 0;
                }
            }
            return 0;
        }


    }
}