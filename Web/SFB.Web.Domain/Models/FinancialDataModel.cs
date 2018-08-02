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
                        return FinancialDataDocumentModel.GetPropertyValue<double>(DBFieldNames.NO_PUPILS);
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
                        return FinancialDataDocumentModel.GetPropertyValue<double>(DBFieldNames.NO_SCHOOLS);
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
                try
                {
                    if (FinancialDataDocumentModel != null)
                    {
                        return FinancialDataDocumentModel.GetPropertyValue<decimal>("Total Income");
                    }
                    return 0;
                }
                catch (Exception)
                {
                    return 0;
                }
            }
        }

        public decimal TotalExpenditure
        {
            get
            {
                try
                {
                    if (FinancialDataDocumentModel != null)
                    {
                        return FinancialDataDocumentModel.GetPropertyValue<decimal>("Total Expenditure");                        
                    }
                    return 0;
                }
                catch (Exception)
                {
                    return 0;
                }
            }
        }

        public decimal InYearBalance
        {
            get
            {
                try
                {
                    if (FinancialDataDocumentModel != null)
                    {
                        return FinancialDataDocumentModel.GetPropertyValue<decimal>("In Year Balance");
                    }
                    return 0;
                }
                catch (Exception)
                {
                    return 0;
                }

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

        public string AdmissionPolicy => GetString(DBFieldNames.ADMISSION_POLICY);

        public string Gender => GetString(DBFieldNames.GENDER);

        public string SchoolOverallPhase => GetString(DBFieldNames.SCHOOL_OVERALL_PHASE);

        public string SchoolPhase => GetString(DBFieldNames.SCHOOL_PHASE);

        public string SchoolType => GetString(DBFieldNames.SCHOOL_TYPE);

        public string UrbanRural => GetString(DBFieldNames.URBAN_RURAL);

        public string GovernmentOfficeRegion => GetString(DBFieldNames.REGION);

        public string LondonBorough => GetString(DBFieldNames.LONDON_BOROUGH);

        public string LondonWeighting => GetString(DBFieldNames.LONDON_WEIGHT);

        public string PercentageOfEligibleFreeSchoolMeals => GetString(DBFieldNames.PERCENTAGE_FSM);

        public string PercentageOfPupilsWithSen => GetString(DBFieldNames.PERCENTAGE_OF_PUPILS_WITH_SEN);

        public string PercentageOfPupilsWithoutSen => GetString(DBFieldNames.PERCENTAGE_OF_PUPILS_WITHOUT_SEN);

        public string PercentageOfPupilsWithEal => GetString(DBFieldNames.PERCENTAGE_OF_PUPILS_WITH_EAL);

        public string PercentageBoarders => GetString(DBFieldNames.PERCENTAGE_BOARDERS);

        public string Pfi => GetString(DBFieldNames.PFI);

        public string DoesTheSchoolHave6Form => GetString(DBFieldNames.HAS_6_FORM);

        public string NumberIn6Form => GetString(DBFieldNames.NUMBER_IN_6_FORM);

        public string HighestAgePupils => GetString(DBFieldNames.HIGHEST_AGE_PUPILS);

        public string FullTimeAdmin => GetString(DBFieldNames.ADMIN_STAFF);

        public string FullTimeOther => GetString(DBFieldNames.FULL_TIME_OTHER);

        public string PercentageQualifiedTeachers => GetString(DBFieldNames.PERCENTAGE_QUALIFIED_TEACHERS);

        public string LowestAgePupils => GetString(DBFieldNames.LOWEST_AGE_PUPILS);

        public string FullTimeTA => GetString(DBFieldNames.FULL_TIME_TA);

        public string TotalSchoolWorkforceFTE => GetString(DBFieldNames.WORKFORCE_TOTAL);

        public string TotalNumberOfTeachersFTE => GetString(DBFieldNames.TEACHERS_TOTAL);

        public string TotalSeniorTeachersFTE => GetString(DBFieldNames.TEACHERS_LEADER);

        public string Ks2Actual => GetString(DBFieldNames.KS2_ACTUAL);

        public string Ks2Progress => GetString(DBFieldNames.KS2_PROGRESS);

        public string AvAtt8 => GetString(DBFieldNames.AVERAGE_ATTAINMENT);

        public string P8Mea => GetString(DBFieldNames.PROGRESS_8_MEASURE);

        public string OfstedRating => GetString(DBFieldNames.OFSTED_RATING_NAME);

        public string SpecificLearningDifficulty => GetString(DBFieldNames.SPECIFIC_LEARNING_DIFFICULTY);

        public string ModerateLearningDifficulty => GetString(DBFieldNames.MODERATE_LEARNING_DIFFICULTY);

        public string SevereLearningDifficulty => GetString(DBFieldNames.SEVERE_LEARNING_DIFFICULTY);

        public string ProfLearningDifficulty => GetString(DBFieldNames.PROF_LEARNING_DIFFICULTY);

        public string SocialHealth => GetString(DBFieldNames.SOCIAL_HEALTH);
        
        public string SpeechNeeds => GetString(DBFieldNames.SPEECH_NEEDS);

        public string HearingImpairment => GetString(DBFieldNames.HEARING_IMPAIRMENT);

        public string VisualImpairment => GetString(DBFieldNames.VISUAL_IMPAIRMENT);

        public string MultiSensoryImpairment => GetString(DBFieldNames.MULTI_SENSORY_IMPAIRMENT);

        public string PhysicalDisability => GetString(DBFieldNames.PHYSICAL_DISABILITY);

        public string AutisticDisorder => GetString(DBFieldNames.AUTISTIC_DISORDER);

        public string OtherLearningDifficulty => GetString(DBFieldNames.OTHER_LEARNING_DIFF);

        public string CrossPhaseBreakdownPrimary => FinancialDataDocumentModel.GetPropertyValue<Document>(DBFieldNames.SCHOOL_OVERALL_PHASE_BREAKDOWN).GetPropertyValue<String>(DBFieldNames.SCHOOL_OVERALL_PHASE_CROSS_PRIMARY);
        public string CrossPhaseBreakdownSecondary => FinancialDataDocumentModel.GetPropertyValue<Document>(DBFieldNames.SCHOOL_OVERALL_PHASE_BREAKDOWN).GetPropertyValue<String>(DBFieldNames.SCHOOL_OVERALL_PHASE_CROSS_SECONDARY);
        public string CrossPhaseBreakdownSpecial => FinancialDataDocumentModel.GetPropertyValue<Document>(DBFieldNames.SCHOOL_OVERALL_PHASE_BREAKDOWN).GetPropertyValue<String>(DBFieldNames.SCHOOL_OVERALL_PHASE_CROSS_SPECIAL);
        public string CrossPhaseBreakdownPru => FinancialDataDocumentModel.GetPropertyValue<Document>(DBFieldNames.SCHOOL_OVERALL_PHASE_BREAKDOWN).GetPropertyValue<String>(DBFieldNames.SCHOOL_OVERALL_PHASE_CROSS_PRU);
        public string CrossPhaseBreakdownAP => FinancialDataDocumentModel.GetPropertyValue<Document>(DBFieldNames.SCHOOL_OVERALL_PHASE_BREAKDOWN).GetPropertyValue<String>(DBFieldNames.SCHOOL_OVERALL_PHASE_CROSS_AP);
        public string CrossPhaseBreakdownAT => FinancialDataDocumentModel.GetPropertyValue<Document>(DBFieldNames.SCHOOL_OVERALL_PHASE_BREAKDOWN).GetPropertyValue<String>(DBFieldNames.SCHOOL_OVERALL_PHASE_CROSS_AT);

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