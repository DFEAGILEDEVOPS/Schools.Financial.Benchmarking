﻿using SFB.Web.Common;
using System;

namespace SFB.Web.Domain.Models
{
    public class BestInClassCriteria : IEquatable<BestInClassCriteria>
    {
        public EstablishmentType EstablishmentType { get; set; }
        public string OverallPhase { get; set; }
        public string UrbanRural { get; set; }
        public decimal? Ks2ProgressScoreMin { get; set; }
        public decimal? Ks2ProgressScoreMax { get; set; }
        public decimal? Ks4ProgressScoreMin { get; set; }
        public decimal? Ks4ProgressScoreMax { get; set; }
        public decimal NoPupilsMin { get; set; }
        public decimal NoPupilsMax { get; set; }
        public decimal PercentageFSMMin { get; set; }
        public decimal PercentageFSMMax { get; set; }
        public decimal PercentageSENMin { get; set; }
        public decimal PercentageSENMax { get; set; }

        public bool Equals(BestInClassCriteria other)
        {
            return this.EstablishmentType == other.EstablishmentType
                && this.OverallPhase == other.OverallPhase
                && this.UrbanRural == other.UrbanRural
                && this.Ks2ProgressScoreMin == other.Ks2ProgressScoreMin
                && this.Ks2ProgressScoreMax == other.Ks2ProgressScoreMax      
                && this.Ks4ProgressScoreMin == other.Ks4ProgressScoreMin
                && this.Ks4ProgressScoreMax == other.Ks4ProgressScoreMax
                && this.PercentageFSMMin == other.PercentageFSMMin
                && this.PercentageFSMMax == other.PercentageFSMMax
                && this.PercentageSENMin == other.PercentageSENMin
                && this.PercentageSENMax == other.PercentageSENMax
                && this.NoPupilsMin == other.NoPupilsMin
                && this.NoPupilsMax == other.NoPupilsMax;
        }
    }
}