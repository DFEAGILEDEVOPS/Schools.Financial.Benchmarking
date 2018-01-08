using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Common.EntitySql;

namespace SFB.Web.UI.Models
{
    public class JourneyStepViewModel : ViewModelBase
    {
        public Journey Journey { get; set; }
        public List<Journey> Fork { get; set; }
        public int Position { get; set; }
        public string BodyPartial { get; set; }
        public string BackUrl { get; set; }
        public string BackAction { get; set; }
        public Func<int, Func<Dictionary<string, object>>> GetForkData { get; set; }
    }
}