using System;
using SFB.Web.ApplicationCore.Helpers.Constants;
using SFB.Web.ApplicationCore.Helpers.Enums;
using SFB.Web.ApplicationCore.Entities;

namespace SFB.Web.ApplicationCore.Models
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

        public int? LaNumber => FinancialDataObjectModel?.LA;

        #region Financial Data

        public decimal? PupilCount
        {
            get
            {
                try
                {
                    if (FinancialDataObjectModel != null && FinancialDataObjectModel.NoPupils != null)
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

        public decimal? SchoolCount
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

        public decimal? TeacherCount
        {
            get
            {
                try
                {
                    if (FinancialDataObjectModel != null && FinancialDataObjectModel.NoTeachers != null)
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

        public int? CompanyNo
        {
            get
            {
                try
                {
                    if (FinancialDataObjectModel != null)
                    {
                        return FinancialDataObjectModel.CompanyNumber;
                    }
                    return null;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        public bool IsMAT
        {
            get
            {
                try
                {
                    if (FinancialDataObjectModel != null)
                    {
                        return FinancialDataObjectModel.MATSATCentralServices.Equals("MAT");
                    }
                    return false;
                }
                catch (Exception)
                {
                    return false;
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

        public bool IsPlaceHolder
        {
            get
            {
                try
                {
                    if (FinancialDataObjectModel != null)
                    {
                        return FinancialDataObjectModel.IsPlaceholder;
                    }
                    return false;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        public decimal? TotalIncome => FinancialDataObjectModel?.TotalIncome;

        public decimal? TotalExpenditure => FinancialDataObjectModel?.TotalExpenditure;

        public decimal? InYearBalance => FinancialDataObjectModel?.InYearBalance;
        
        public bool IsReturnsComplete
        {
            get
            {
                return this.EstabType == EstablishmentType.MAT || FinancialDataObjectModel?.PeriodCoveredByReturn >= 12;
            }
        }

        public bool IsReturnsDNS
        {
            get
            {
                return (FinancialDataObjectModel == null || FinancialDataObjectModel.DidNotSubmit);
            }
        }

        public bool PartialYearsPresentInSubSchools
        {
            get
            {
                if (FinancialDataObjectModel != null)
                {
                    return FinancialDataObjectModel.PartialYearsPresent.GetValueOrDefault();
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

        public string SchoolOverallPhase => FinancialDataObjectModel?.OverallPhase;

        public string SchoolPhase => FinancialDataObjectModel?.Phase;

        public string SchoolType => FinancialDataObjectModel.Type;

        public string UrbanRural => FinancialDataObjectModel.UrbanRural;

        public string GovernmentOfficeRegion => FinancialDataObjectModel.Region;

        public string LondonBorough => FinancialDataObjectModel.LondonBorough;

        public string LondonWeighting => FinancialDataObjectModel.LondonWeight;

        public decimal? PercentageOfEligibleFreeSchoolMeals => FinancialDataObjectModel.PercentageFSM;

        public decimal? PercentageOfPupilsWithSen => FinancialDataObjectModel.PercentagePupilsWSEN;

        public decimal? PercentageOfPupilsWithoutSen => FinancialDataObjectModel.PercentagePupilsWOSEN;

        public decimal? PercentageOfPupilsWithEal => FinancialDataObjectModel.PercentagePupilsWEAL;

        public decimal? PercentageBoarders => FinancialDataObjectModel.PercentageBoarders;

        public string Pfi => FinancialDataObjectModel.PFI;

        public string DoesTheSchoolHave6Form => FinancialDataObjectModel.Has6Form;

        public decimal? NumberIn6Form => FinancialDataObjectModel.NumberIn6Form;

        public decimal? HighestAgePupils => FinancialDataObjectModel.HighestAgePupils;

        public decimal? FullTimeAdmin => FinancialDataObjectModel.AdminStaff;

        public decimal? FullTimeAux => FinancialDataObjectModel.AuxStaff;

        public decimal? FullTimeOther => FinancialDataObjectModel.FullTimeOther;

        public decimal? PercentageQualifiedTeachers => FinancialDataObjectModel.PercentageQualifiedTeachers;

        public decimal? LowestAgePupils => FinancialDataObjectModel.LowestAgePupils;

        public decimal? FullTimeTA => FinancialDataObjectModel.FullTimeTA;

        public decimal? TotalSchoolWorkforceFTE => FinancialDataObjectModel.WorkforceTotal;

        public decimal? TotalNumberOfTeachersFTE => FinancialDataObjectModel.TeachersTotal;

        public decimal? TotalSeniorTeachersFTE => FinancialDataObjectModel.TeachersLeader;

        public decimal? Ks2Actual => FinancialDataObjectModel.Ks2Actual;

        public decimal? Ks2Progress
        {
            get
            {
                if(FinancialDataObjectModel == null)
                {
                    return null;
                }
                return FinancialDataObjectModel.Ks2Progress.HasValue ? decimal.Round(FinancialDataObjectModel.Ks2Progress.GetValueOrDefault(), 2, MidpointRounding.AwayFromZero) : (decimal?)null;
            }
        }

        public decimal? AvAtt8 => FinancialDataObjectModel.AverageAttainment;

        public decimal? P8Mea
        {
            get
            {
                if (FinancialDataObjectModel == null)
                {
                    return null;
                }
                return FinancialDataObjectModel.Progress8Measure.HasValue ? decimal.Round(FinancialDataObjectModel.Progress8Measure.GetValueOrDefault(), 2, MidpointRounding.AwayFromZero) : (decimal?)null;

            }
        }

        public decimal? P8Banding => FinancialDataObjectModel?.Progress8Banding;
        
        public decimal? ProgressScore
        {
            get
            {
                if(this.SchoolOverallPhase == "Secondary" || this.SchoolOverallPhase == "All-through")
                {
                    return this.P8Mea;
                }
                else
                {
                    return this.Ks2Progress;
                }
            }
        }

        public string OfstedRating => FinancialDataObjectModel.OfstedRatingName;

        public decimal? SpecificLearningDifficulty => FinancialDataObjectModel.SpecificLearningDiff;

        public decimal? ModerateLearningDifficulty => FinancialDataObjectModel.ModerateLearningDiff;

        public decimal? SevereLearningDifficulty => FinancialDataObjectModel.SevereLearningDiff;

        public decimal? ProfLearningDifficulty => FinancialDataObjectModel.ProfLearningDiff;

        public decimal? SocialHealth => FinancialDataObjectModel.SocialHealth;

        public decimal? SpeechNeeds => FinancialDataObjectModel.SpeechNeeds;

        public decimal? HearingImpairment => FinancialDataObjectModel.HearingImpairment;

        public decimal? VisualImpairment => FinancialDataObjectModel.VisualImpairment;

        public decimal? MultiSensoryImpairment => FinancialDataObjectModel.MultiSensoryImpairment;

        public decimal? PhysicalDisability => FinancialDataObjectModel.PhysicalDisability;

        public decimal? AutisticDisorder => FinancialDataObjectModel.AutisticDisorder;

        public decimal? OtherLearningDifficulty => FinancialDataObjectModel.OtherLearningDiff;

        public decimal? RRPerIncomePercentage => FinancialDataObjectModel.RRPerIncomePercentage;

        public decimal? PerPupilTotalExpenditure => FinancialDataObjectModel.PerPupilTotalExpenditure;

        public decimal? PerPupilGrantFunding => FinancialDataObjectModel.PerPupilGrantFunding;

        public int? CrossPhaseBreakdownPrimary
        {
            get
            {
                if (FinancialDataObjectModel?.OverallPhaseBreakdown != null && FinancialDataObjectModel.OverallPhaseBreakdown.ContainsKey(SchoolTrustFinanceDataFieldNames.SCHOOL_OVERALL_PHASE_CROSS_PRIMARY))
                {
                    return FinancialDataObjectModel.OverallPhaseBreakdown[SchoolTrustFinanceDataFieldNames.SCHOOL_OVERALL_PHASE_CROSS_PRIMARY];
                }

                return 0;
            }
        }

        public int? CrossPhaseBreakdownSecondary
        {
            get
            {
                if (FinancialDataObjectModel?.OverallPhaseBreakdown != null && FinancialDataObjectModel.OverallPhaseBreakdown.ContainsKey(SchoolTrustFinanceDataFieldNames.SCHOOL_OVERALL_PHASE_CROSS_SECONDARY))
                {
                    return FinancialDataObjectModel.OverallPhaseBreakdown[SchoolTrustFinanceDataFieldNames.SCHOOL_OVERALL_PHASE_CROSS_SECONDARY];
                }

                return 0;
            }
        }

        public int? CrossPhaseBreakdownSpecial
        {
            get
            {
                if (FinancialDataObjectModel?.OverallPhaseBreakdown != null && FinancialDataObjectModel.OverallPhaseBreakdown.ContainsKey(SchoolTrustFinanceDataFieldNames.SCHOOL_OVERALL_PHASE_CROSS_SPECIAL))
                {
                    return FinancialDataObjectModel.OverallPhaseBreakdown[SchoolTrustFinanceDataFieldNames.SCHOOL_OVERALL_PHASE_CROSS_SPECIAL];
                }

                return 0;
            }
        }

        public int? CrossPhaseBreakdownPru
        {
            get
            {
                if (FinancialDataObjectModel?.OverallPhaseBreakdown != null && FinancialDataObjectModel.OverallPhaseBreakdown.ContainsKey(SchoolTrustFinanceDataFieldNames.SCHOOL_OVERALL_PHASE_CROSS_PRU))
                {
                    return FinancialDataObjectModel.OverallPhaseBreakdown[SchoolTrustFinanceDataFieldNames.SCHOOL_OVERALL_PHASE_CROSS_PRU];
                }

                return 0;
            }
        }

        public int? CrossPhaseBreakdownAP
        {
            get
            {
                if (FinancialDataObjectModel?.OverallPhaseBreakdown != null && FinancialDataObjectModel.OverallPhaseBreakdown.ContainsKey(SchoolTrustFinanceDataFieldNames.SCHOOL_OVERALL_PHASE_CROSS_AP))
                {
                    return FinancialDataObjectModel.OverallPhaseBreakdown[SchoolTrustFinanceDataFieldNames.SCHOOL_OVERALL_PHASE_CROSS_AP];
                }

                return 0;
            }
        }

        public int? CrossPhaseBreakdownAT
        {
            get
            {
                if (FinancialDataObjectModel?.OverallPhaseBreakdown != null && FinancialDataObjectModel.OverallPhaseBreakdown.ContainsKey(SchoolTrustFinanceDataFieldNames.SCHOOL_OVERALL_PHASE_CROSS_AT))
                {
                    return FinancialDataObjectModel.OverallPhaseBreakdown[SchoolTrustFinanceDataFieldNames.SCHOOL_OVERALL_PHASE_CROSS_AT];
                }

                return 0;
            }
        }

        #endregion

        public bool Equals(FinancialDataModel other)
        {
            return (this.Id == other.Id);
        }       

    }
}