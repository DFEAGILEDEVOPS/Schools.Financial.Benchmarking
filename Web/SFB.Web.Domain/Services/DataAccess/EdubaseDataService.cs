using System;
using System.Collections.Generic;
using System.Configuration;
using System.Dynamic;
using System.Linq;
using System.Text;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using SFB.Web.Domain.Helpers.Constants;

namespace SFB.Web.Domain.Services.DataAccess
{
    public class EdubaseDataService : IEdubaseDataService
    {
        private static readonly string DatabaseId = ConfigurationManager.AppSettings["database"];
        private static DocumentClient _client;
        private readonly IFinancialDataService _financialDataService;

        public EdubaseDataService()
        {
            _client = new DocumentClient(new Uri(ConfigurationManager.AppSettings["endpoint"]),
                ConfigurationManager.AppSettings["authKey"],
                new ConnectionPolicy
                {
                    ConnectionMode = ConnectionMode.Direct,
                    ConnectionProtocol = Protocol.Tcp
                });
            _financialDataService = new FinancialDataService();
            _financialDataService.GetActiveCollectionByDataGroup(DataGroups.Edubase);
        }


        public dynamic GetSchoolByUrn(string urn)
        {
            var collectionName = _financialDataService.GetActiveCollectionByDataGroup(DataGroups.Edubase);
            var query = "SELECT c['URN'], c['EstablishmentName'], c['OverallPhase'], c['PhaseOfEducation'], c['TypeOfEstablishment'], c['Street'], c['Town'], c['Location'], c['Postcode'], c['Trusts'], " +
                        " c['LAName'], c['LACode'], c['EstablishmentNumber'], c['TelephoneNum'], c['NumberOfPupils'], c['StatutoryLowAge'], c['StatutoryHighAge'], c['HeadFirstName'], " +
                        "c['HeadLastName'], c['OfficialSixthForm'], c['SchoolWebsite'], c['OfstedRating'], c['OfstedLastInsp'], c['FinanceType']" +
                        $" FROM c WHERE c.URN={urn}";
            var result = _client.CreateDocumentQuery<Document>(UriFactory.CreateDocumentCollectionUri(DatabaseId, collectionName), query, new FeedOptions() { MaxItemCount = 1 }).ToList().FirstOrDefault();
            return result;
        }

        
        public dynamic GetSchoolByLaEstab(string laEstab)
        {
            var collectionName = _financialDataService.GetActiveCollectionByDataGroup(DataGroups.Edubase);
            var query = "SELECT c['URN'], c['EstablishmentName'], c['OverallPhase'], c['TypeOfEstablishment'], c['Street'], c['Town'], c['Location'], c['Postcode'], c['Trusts'], " +
                        " c['LAName'], c['LACode'], c['EstablishmentNumber'], c['TelephoneNum'], c['NumberOfPupils'], c['StatutoryLowAge'], c['StatutoryHighAge'], c['HeadFirstName'], " +
                        "c['HeadLastName'], c['OfficialSixthForm'], c['SchoolWebsite'], c['OfstedRating'], c['FinanceType']" +
                        $" FROM c WHERE c.LACode={laEstab.Substring(0,3)} and c.EstablishmentNumber={laEstab.Substring(3)}";
            var result = _client.CreateDocumentQuery<Document>(UriFactory.CreateDocumentCollectionUri(DatabaseId, collectionName), query, new FeedOptions() { MaxItemCount = 1 }).ToList().FirstOrDefault();
            return result;
        }

        public dynamic GetSponsorByName(string name)
        {
            var collectionName = _financialDataService.GetActiveCollectionByDataGroup(DataGroups.Edubase);
            var query = $"SELECT c.URN, c.EstablishmentName FROM c WHERE c['SchoolSponsorFlag']='Linked to a sponsor' AND c['Trusts']='{name}'";
            var matches = _client.CreateDocumentQuery<Document>(UriFactory.CreateDocumentCollectionUri(DatabaseId, collectionName), query).ToList();

            dynamic result = new ExpandoObject();
            result.Results = matches;

            return result;
        }

        public dynamic GetMultipleSchoolsByUrns(List<string> urns)
        {
            var collectionName = _financialDataService.GetActiveCollectionByDataGroup(DataGroups.Edubase);
            var sb = new StringBuilder();
            urns.ForEach(u => sb.Append(u + ","));
            var query = "SELECT c['URN'], c['EstablishmentName'], c['OverallPhase'], c['TypeOfEstablishment'], c['Street'], c['Town'], c['Postcode'], c['FinanceType']" +
                        $" FROM c WHERE c.URN IN ({sb.ToString().TrimEnd((','))})";
            var result = _client.CreateDocumentQuery<Document>(UriFactory.CreateDocumentCollectionUri(DatabaseId, collectionName), query).ToList();
            return result;
        }
    }
}