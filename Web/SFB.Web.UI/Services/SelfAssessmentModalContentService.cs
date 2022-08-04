using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using Newtonsoft.Json;
using SFB.Web.UI.Models;

namespace SFB.Web.UI.Services
{
    public interface ISelfAssessmentModalContentService
    {
        dynamic GetModalContent();
        SelfAssessmentModalModel GetById(int id);
        
        Dictionary<string, int> AssessmentAreasById();

        List<SelfAssessmentModalModel> GetAllSadModals();
    }

    public class SelfAssessmentModalContentService : ISelfAssessmentModalContentService
    {
        private readonly MemoryCache _memoryCache;
        private readonly dynamic _modalModels;

        public SelfAssessmentModalContentService(dynamic modalModels)
        {
            _memoryCache = MemoryCache.Default;
            _modalModels = modalModels;

            CacheItemPolicy policy = new CacheItemPolicy
            {
                SlidingExpiration = TimeSpan.FromDays(1)
            };
            _memoryCache.Set("SadModalModels", modalModels, policy);
        }

        public dynamic GetModalContent()
        {
            var modalModels = _memoryCache.Get("SadModalModels");
            if (modalModels is null)
            {
                CacheItemPolicy policy = new CacheItemPolicy
                {
                    SlidingExpiration = TimeSpan.FromDays(1)
                };
                _memoryCache.Set("SadModalModels", _modalModels, policy);
                
            }
            
            return modalModels;
        }

        public List<SelfAssessmentModalModel> GetAllSadModals()
        {
            var modals = (List<dynamic>)JsonConvert.DeserializeObject<List<dynamic>>(GetModalContent());
            var list = new List<SelfAssessmentModalModel>();

            foreach (var modal in modals)
            {
                list.Add(
                    new SelfAssessmentModalModel
                    {
                        Id = modal.id,
                        Title = modal.title,
                        Content = modal.textContent,
                        AssessmentArea = modal.assessmentArea
                    }
                );
            }

            return list;
        }

        public SelfAssessmentModalModel GetById(int id)
        {
            var modals = (List<dynamic>)JsonConvert.DeserializeObject<List<dynamic>>(GetModalContent());
            var modal = modals.FirstOrDefault(c => c.id == id);

            return new SelfAssessmentModalModel
            {
                Id = id,
                AssessmentArea = modal?.assessmentArea,
                Title = modal?.title,
                Content = modal?.textContent
            };
        }

        public Dictionary<string, int> AssessmentAreasById()
        {
            var modals = (List<dynamic>)JsonConvert.DeserializeObject<List<dynamic>>(GetModalContent());
            var dictionary = new Dictionary<string, int>();
            foreach (var modal in modals)
            {
                dictionary.Add((string)modal.assessmentArea, (int)modal.id);
                
            }

            return dictionary;
        }
    }
}