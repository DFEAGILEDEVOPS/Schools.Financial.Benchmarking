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
using SFB.Web.Common.DataObjects;

namespace SFB.Web.DAL.Repositories
{
    public class CosmosDbEdubaseRepository : IEdubaseRepository
    {
        private static readonly string DatabaseId = ConfigurationManager.AppSettings["database"];
        private static DocumentClient _client;
        private readonly string _collectionName;

        public CosmosDbEdubaseRepository(IDataCollectionManager dataCollectionManager)
        {
            _client = new DocumentClient(new Uri(ConfigurationManager.AppSettings["endpoint"]),
                ConfigurationManager.AppSettings["authKey"],
                new ConnectionPolicy
                {
                    ConnectionMode = ConnectionMode.Direct,
                    ConnectionProtocol = Protocol.Tcp
                });

            _collectionName = dataCollectionManager.GetLatestActiveCollectionByDataGroup(DataGroups.Edubase);

            CreateUDFs();
        }

        private void CreateUDFs()
        {
            UserDefinedFunction parseFtUdf = new UserDefinedFunction()
            {
                Id = "PARSE_FINANCIAL_TYPE_CODE",
                Body = @"function(code) {
                switch (code) {
                   case 'A':
                       return 'Academies';
                   case 'M':
                       return 'Maintained';
                   default:
                       return 'Maintained';
                    }
                }"
            };

            _client.CreateUserDefinedFunctionAsync(UriFactory.CreateDocumentCollectionUri(DatabaseId, _collectionName), parseFtUdf);
        }

        public EdubaseDataObject GetSchoolDataObjectByUrn(int urn)
        {
            return GetSchoolDataObjectById(new Dictionary<string, int> { { DBFieldNames.URN, urn } });
        }

        public List<EdubaseDataObject> GetMultipleSchoolDataObjectsByUrns(List<int> urns)
        {
            return GetMultipleSchoolDataObjetsByIds(DBFieldNames.URN, urns);
        }

        public EdubaseDataObject GetSchoolByLaEstab(string laEstab)
        {
            return GetSchoolDataObjectById(new Dictionary<string, int>
            {
                {DBFieldNames.LA_CODE, Int32.Parse(laEstab.Substring(0, 3))},
                {DBFieldNames.ESTAB_NO, Int32.Parse(laEstab.Substring(3))}
            });
        }

        #region Private methods
       
        private EdubaseDataObject GetSchoolDataObjectById(Dictionary<string, int> fields)
        {

            var sb = new StringBuilder();
            foreach (var field in fields)
            {
                sb.Append($"c.{field.Key}=@{field.Key} AND ");
            }

            var where = sb.ToString().Substring(0, sb.ToString().Length - 5);

            var query =
                "SELECT c['URN'], c['EstablishmentName'], c['OverallPhase'], c['PhaseOfEducation'], c['TypeOfEstablishment'], c['Street'], c['Town'], c['Location'], c['Postcode'], c['Trusts'], " +
                " c['LAName'], c['LACode'], c['EstablishmentNumber'], c['TelephoneNum'], c['NumberOfPupils'], c['StatutoryLowAge'], c['StatutoryHighAge'], c['HeadFirstName'], " +
                $"c['HeadLastName'], c['OfficialSixthForm'], c['SchoolWebsite'], c['OfstedRating'], c['OfstedLastInsp'], udf.PARSE_FINANCIAL_TYPE_CODE(c['FinanceType']) AS FinanceType, c['OpenDate'], c['CloseDate'] FROM c WHERE {where}";

            SqlQuerySpec querySpec = new SqlQuerySpec(query);
            querySpec.Parameters = new SqlParameterCollection();
            foreach (var field in fields)
            {
                querySpec.Parameters.Add(new SqlParameter($"@{field.Key}", field.Value));
            }

            var result = _client.CreateDocumentQuery<EdubaseDataObject>(UriFactory.CreateDocumentCollectionUri(DatabaseId, _collectionName), querySpec, new FeedOptions() { MaxItemCount = 1 }).ToList().FirstOrDefault();
            return result;
        }

        private List<EdubaseDataObject> GetMultipleSchoolDataObjetsByIds(string fieldName, List<int> ids)
        {
            var sb = new StringBuilder();
            ids.ForEach(u => sb.Append(u + ","));

            var query = "SELECT c['URN'], c['EstablishmentName'], c['OverallPhase'], c['TypeOfEstablishment'], c['Street'], c['Town'], c['Postcode'], udf.PARSE_FINANCIAL_TYPE_CODE(c['FinanceType']) AS FinanceType" +
                        $" FROM c WHERE c.{fieldName} IN ({sb.ToString().TrimEnd((','))})";
            var result = _client.CreateDocumentQuery<EdubaseDataObject>(UriFactory.CreateDocumentCollectionUri(DatabaseId, _collectionName), query).ToList();
            return result;
        }

        public List<int> GetAllSchoolUrns()
        {
            var query = $"SELECT VALUE c.URN FROM c";

            var result = _client.CreateDocumentQuery<int>(UriFactory.CreateDocumentCollectionUri(DatabaseId, _collectionName), query).ToList();

            return result;
        }


        #endregion
    }
}