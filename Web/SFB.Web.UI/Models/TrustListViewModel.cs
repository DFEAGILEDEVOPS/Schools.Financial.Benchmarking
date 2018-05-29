using System.Collections.Generic;

namespace SFB.Web.UI.Models
{
    public class TrustListViewModel : ViewModelListBase<TrustViewModel>
    {
        public TrustListViewModel(List<TrustViewModel> modelList, string orderBy = "")
        {
            base.ModelList = modelList;
            base.OrderBy = orderBy;
        }
    }
}