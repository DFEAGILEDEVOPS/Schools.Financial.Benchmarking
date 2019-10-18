using SFB.Web.ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFB.Web.UI.Services
{
    public interface ILocationSearchService
    {
        SuggestionQueryResult SuggestLocationName(string query);
    }
}
