using System;
using Microsoft.Azure.Documents;
using SFB.Web.Common;
using SFB.Web.Common.DataObjects;

namespace SFB.Web.Domain.Models
{
    public class FinancialDataModel : IEquatable<FinancialDataModel>
    {
        public string Term { get; }
        public SchoolTrustFinancialDataObject FinancialDataObjectModel { get; }
        public string Id { get; private set; }    
        public EstablishmentType EstabType{ get; private set;}


        public FinancialDataModel(){}

        public FinancialDataModel(string id, string term, SchoolTrustFinancialDataObject financialDataObject, EstablishmentType estabType)
        {
            Id = id;
            Term = term;
            FinancialDataObjectModel = financialDataObject;
            EstabType = estabType;
        }

        public int LaNumber => FinancialDataObjectModel.LA;

        #region Financial Data

        public double PupilCount
        {
            get
            {
                try
                {
                    if (FinancialDataObjectModel != null)
                    {
                        return FinancialDataObjectModel.NoPupils;
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
                    if (FinancialDataObjectModel != null)
                    {
                        return FinancialDataObjectModel.SchoolCount;
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
                    if (FinancialDataObjectModel != null)
                    {

                        return FinancialDataObjectModel.NoTeachers;
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
                    if (FinancialDataObjectModel != null)
                    {
                        return FinancialDataObjectModel.MATNumber;
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
                    if (FinancialDataObjectModel != null)
                    {
                        return FinancialDataObjectModel.MATSATCentralServices.Equals("SAT");
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
                    if (FinancialDataObjectModel != null)
                    {
                        return FinancialDataObjectModel.DidNotSubmit;
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
                if (FinancialDataObjectModel != null)
                {
                    return FinancialDataObjectModel.TotalIncome;
                }
                return 0;
            }
        }

        public decimal TotalExpenditure
        {
            get
            {
                if (FinancialDataObjectModel != null)
                {
                    return FinancialDataObjectModel.TotalExpenditure;
                }
                return 0;
            }
        }

        public decimal InYearBalance
        {
            get
            {
                if (FinancialDataObjectModel != null)
                {
                    return FinancialDataObjectModel.InYearBalance;
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
                    if (FinancialDataObjectModel != null)
                    {
                        return FinancialDataObjectModel.PeriodCoveredByReturn;
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
                if (FinancialDataObjectModel != null)
                {
                    return FinancialDataObjectModel.PartialYearsPresent;
                }
                return false;
            }
        }

        public bool WorkforceDataPresent
        {
            get
            {
                if (FinancialDataObjectModel != null)
                {
                    return FinancialDataObjectModel.WorkforcePresent;
                }
                return false;
            }
        }

        #endregion

        #region Criteria Data

        public string AdmissionPolicy => FinancialDataObjectModel.AdmissionPolicy;

        public string Gender => FinancialDataObjectModel.Gender;

        public string SchoolOverallPhase => FinancialDataObjectModel.OverallPhase;

        public string SchoolPhase => FinancialDataObjectModel.Phase;

        public string SchoolType => FinancialDataObjectModel.Type;

        public string UrbanRural => FinancialDataObjectModel.UrbanRural;

        public string GovernmentOfficeRegion => FinancialDataObjectModel.Region;

        public string LondonBorough => FinancialDataObjectModel.LondonBorough;

        public string LondonWeighting => FinancialDataObjectModel.LondonWeight;

        public decimal PercentageOfEligibleFreeSchoolMeals => FinancialDataObjectModel.PercentageFSM;

        public decimal PercentageOfPupilsWithSen => FinancialDataObjectModel.PercentagePupilsWSEN;

        public decimal PercentageOfPupilsWithoutSen => FinancialDataObjectModel.PercentagePupilsWOSEN;

        public decimal PercentageOfPupilsWithEal => FinancialDataObjectModel.PercentagePupilsWEAL;

        public decimal PercentageBoarders => FinancialDataObjectModel.PercentageBoarders;

        public string Pfi => FinancialDataObjectModel.PFI;

        public string DoesTheSchoolHave6Form => FinancialDataObjectModel.Has6Form;

        public string NumberIn6Form => FinancialDataObjectModel.NumberIn6Form;

        public decimal HighestAgePupils => FinancialDataObjectModel.HighestAgePupils;

        public decimal FullTimeAdmin => FinancialDataObjectModel.AdminStaff;

        public decimal FullTimeOther => FinancialDataObjectModel.FullTimeOther;

        public decimal PercentageQualifiedTeachers => FinancialDataObjectModel.PercentageQualifiedTeachers;

        public decimal LowestAgePupils => FinancialDataObjectModel.LowestAgePupils;

        public decimal FullTimeTA => FinancialDataObjectModel.FullTimeTA;

        public decimal TotalSchoolWorkforceFTE => FinancialDataObjectModel.WorkforceTotal;

        public decimal TotalNumberOfTeachersFTE => FinancialDataObjectModel.TeachersTotal;

        public decimal TotalSeniorTeachersFTE => FinancialDataObjectModel.TeachersLeader;

        public decimal Ks2Actual => FinancialDataObjectModel.Ks2Actual;

        public decimal Ks2Progress => FinancialDataObjectModel.Ks2Progress;

        public decimal AvAtt8 => FinancialDataObjectModel.AverageAttainment;

        public decimal P8Mea => FinancialDataObjectModel.Progress8Measure;

        public string OfstedRating => FinancialDataObjectModel.OfstedRatingName;

        public decimal SpecificLearningDifficulty => FinancialDataObjectModel.SpecificLearningDiff;

        public decimal ModerateLearningDifficulty => FinancialDataObjectModel.ModerateLearningDiff;

        public decimal SevereLearningDifficulty => FinancialDataObjectModel.SevereLearningDiff;

        public decimal ProfLearningDifficulty => FinancialDataObjectModel.ProfLearningDiff;

        public decimal SocialHealth => FinancialDataObjectModel.SocialHealth;
        
        public decimal SpeechNeeds => FinancialDataObjectModel.SpeechNeeds;

        public decimal HearingImpairment => FinancialDataObjectModel.HearingImpairment;

        public decimal VisualImpairment => FinancialDataObjectModel.VisualImpairment;

        public decimal MultiSensoryImpairment => FinancialDataObjectModel.MultiSensoryImpairment;

        public decimal PhysicalDisability => FinancialDataObjectModel.PhysicalDisability;

        public decimal AutisticDisorder => FinancialDataObjectModel.AutisticDisorder;

        public decimal OtherLearningDifficulty => FinancialDataObjectModel.OtherLearningDiff;

        public int CrossPhaseBreakdownPrimary => FinancialDataObjectModel.OverallPhaseBreakdown[SchoolTrustFinanceDBFieldNames.SCHOOL_OVERALL_PHASE_CROSS_PRIMARY];
        public int CrossPhaseBreakdownSecondary => FinancialDataObjectModel.OverallPhaseBreakdown[SchoolTrustFinanceDBFieldNames.SCHOOL_OVERALL_PHASE_CROSS_SECONDARY];
        public int CrossPhaseBreakdownSpecial => FinancialDataObjectModel.OverallPhaseBreakdown[SchoolTrustFinanceDBFieldNames.SCHOOL_OVERALL_PHASE_CROSS_SPECIAL];
        public int CrossPhaseBreakdownPru => FinancialDataObjectModel.OverallPhaseBreakdown[SchoolTrustFinanceDBFieldNames.SCHOOL_OVERALL_PHASE_CROSS_PRU];
        public int CrossPhaseBreakdownAP => FinancialDataObjectModel.OverallPhaseBreakdown[SchoolTrustFinanceDBFieldNames.SCHOOL_OVERALL_PHASE_CROSS_AP];
        public int CrossPhaseBreakdownAT => FinancialDataObjectModel.OverallPhaseBreakdown[SchoolTrustFinanceDBFieldNames.SCHOOL_OVERALL_PHASE_CROSS_AT];

        #endregion

        public bool Equals(FinancialDataModel other)
        {
            return (this.Id == other.Id);
        }

        //public decimal? GetDecimal(string fieldName)
        //{
        //    decimal? result;
        //    try
        //    {
        //        result = FinancialDataObjectModel.GetPropertyValue<decimal?>(fieldName);
        //    }
        //    catch
        //    {
        //        return null;
        //    }

        //    return result;
        //}

        //public string GetString(string fieldName)
        //{
        //    if (FinancialDataObjectModel != null)
        //    {
        //        return FinancialDataObjectModel.GetPropertyValue<string>(fieldName);
        //    }
        //    return string.Empty;
        //}

        //public int GetInt(string fieldName)
        //{
        //    if (FinancialDataObjectModel != null)
        //    {
        //        return FinancialDataObjectModel.GetPropertyValue<int>(fieldName);
        //    }
        //    return 0;
        //}

        //private int TryGetInt(string fieldName)
        //{
        //    if (FinancialDataObjectModel != null)
        //    {
        //        try
        //        {
        //            return FinancialDataObjectModel.GetPropertyValue<int>(fieldName);
        //        }
        //        catch
        //        {
        //            return 0;
        //        }
        //    }
        //    return 0;
        //}


    }
}