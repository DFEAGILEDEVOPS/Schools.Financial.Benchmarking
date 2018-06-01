using System;
using System.Collections.Generic;
using System.Configuration;
using System.Dynamic;
using System.Linq;
using System.Text;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using SFB.Web.Common;
using SFB.Web.DAL.Helpers;

namespace SFB.Web.DAL.Repositories
{
    public class DocumentDbEdubaseRepository : IEdubaseRepository
    {
        private static readonly string DatabaseId = ConfigurationManager.AppSettings["database"];
        private static DocumentClient _client;
        private readonly string _collectionName;

        public DocumentDbEdubaseRepository(IDataCollectionManager dataCollectionManager)
        {
            _client = new DocumentClient(new Uri(ConfigurationManager.AppSettings["endpoint"]),
                ConfigurationManager.AppSettings["authKey"],
                new ConnectionPolicy
                {
                    ConnectionMode = ConnectionMode.Direct,
                    ConnectionProtocol = Protocol.Tcp
                });

            _collectionName = dataCollectionManager.GetActiveCollectionByDataGroup(DataGroups.Edubase);
        }

        public dynamic GetSchoolByUrn(string urn)
        {
            return GetSchoolById(new Dictionary<string, string> { { DBFieldNames.URN, urn } });
        }


        public bool QuerySchoolByUrn(string urn)
        {
            var query = $"SELECT c.id FROM c WHERE c.URN={urn}";

            var result = _client.CreateDocumentQuery<Document>(UriFactory.CreateDocumentCollectionUri(DatabaseId, _collectionName), query, new FeedOptions() { MaxItemCount = 1 }).ToList().FirstOrDefault();
            return result != null ;
        }

        public dynamic GetMultipleSchoolsByUrns(List<string> urns)
        {
            return GetMultipleSchoolsByIds(DBFieldNames.URN, urns);
        }

        public dynamic GetSchoolByLaEstab(string laEstab)
        {
            return GetSchoolById(new Dictionary<string, string>
            {
                {DBFieldNames.LA_CODE, laEstab.Substring(0, 3)},
                {DBFieldNames.ESTAB_NO, laEstab.Substring(3)}
            });
        }

        public dynamic GetSponsorByName(string name)
        {
            return GetSponsorById(DBFieldNames.TRUSTS, name);
        }

        #region Private methods
       
        private dynamic GetSchoolById(Dictionary<string, string> fields)
        {
            var sb = new StringBuilder();
            foreach (var field in fields)
            {
                sb.Append($"c.{field.Key}={field.Value} AND ");
            }

            var where = sb.ToString().Substring(0, sb.ToString().Length - 5);

            var query =
                "SELECT c['URN'], c['EstablishmentName'], c['OverallPhase'], c['PhaseOfEducation'], c['TypeOfEstablishment'], c['Street'], c['Town'], c['Location'], c['Postcode'], c['Trusts'], " +
                " c['LAName'], c['LACode'], c['EstablishmentNumber'], c['TelephoneNum'], c['NumberOfPupils'], c['StatutoryLowAge'], c['StatutoryHighAge'], c['HeadFirstName'], " +
                $"c['HeadLastName'], c['OfficialSixthForm'], c['SchoolWebsite'], c['OfstedRating'], c['OfstedLastInsp'], c['FinanceType'], c['OpenDate'], c['CloseDate'] FROM c WHERE {where}";

            var result = _client.CreateDocumentQuery<Document>(UriFactory.CreateDocumentCollectionUri(DatabaseId, _collectionName), query, new FeedOptions() { MaxItemCount = 1 }).ToList().FirstOrDefault();
            return result;
        }

        private dynamic GetMultipleSchoolsByIds(string fieldName, List<string> ids)
        {
            var sb = new StringBuilder();
            ids.ForEach(u => sb.Append(u + ","));

            var query = "SELECT c['URN'], c['EstablishmentName'], c['OverallPhase'], c['TypeOfEstablishment'], c['Street'], c['Town'], c['Postcode'], c['FinanceType']" +
                        $" FROM c WHERE c.{fieldName} IN ({sb.ToString().TrimEnd((','))})";
            var result = _client.CreateDocumentQuery<Document>(UriFactory.CreateDocumentCollectionUri(DatabaseId, _collectionName), query).ToList();
            return result;
        }

        private dynamic GetSponsorById(string fieldName, string id)
        {
            var query = $"SELECT c.URN, c.EstablishmentName FROM c WHERE c['SchoolSponsorFlag']='Linked to a sponsor' AND c['{fieldName}']='{id}'";
            var matches = _client.CreateDocumentQuery<Document>(UriFactory.CreateDocumentCollectionUri(DatabaseId, _collectionName), query).ToList();

            dynamic result = new ExpandoObject();
            result.Results = matches;

            return result;
        }

        public List<int> GetAllSchoolUrns()
        {
            var query = $"SELECT c.URN FROM c";

            var result = _client.CreateDocumentQuery<Document>(UriFactory.CreateDocumentCollectionUri(DatabaseId, _collectionName), query).ToList();

            return result.Select(r => r.GetPropertyValue<int>("URN")).ToList();
        }


        #endregion
    }
}