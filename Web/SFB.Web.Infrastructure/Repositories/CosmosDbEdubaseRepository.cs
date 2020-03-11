using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using SFB.Web.ApplicationCore.Entities;
using System.Threading.Tasks;
using SFB.Web.ApplicationCore.DataAccess;
using SFB.Web.ApplicationCore.Helpers.Constants;

namespace SFB.Web.Infrastructure.Repositories
{
    public class CosmosDbEdubaseRepository : AppInsightsLoggable, IEdubaseRepository
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
            return GetSchoolDataObjectById(new Dictionary<string, object> { { SchoolTrustFinanceDataFieldNames.URN, urn } }).FirstOrDefault();
        }

        public List<EdubaseDataObject> GetMultipleSchoolDataObjectsByUrns(List<int> urns)
        {
            return GetMultipleSchoolDataObjetsByIds(SchoolTrustFinanceDataFieldNames.URN, urns);
        }

        public List<EdubaseDataObject> GetSchoolsByLaEstab(string laEstab, bool openOnly)
        {
            var parameters = new Dictionary<string, object>
            {
                {EdubaseDataFieldNames.LA_CODE, Int32.Parse(laEstab.Substring(0, 3))},
                {EdubaseDataFieldNames.ESTAB_NO, Int32.Parse(laEstab.Substring(3))}
            };

            if(openOnly)
            {
                parameters.Add(EdubaseDataFieldNames.ESTAB_STATUS, "Open");
            }

            return GetSchoolDataObjectById(parameters);
        }

        public List<int> GetAllSchoolUrns()
        {
            var query = $"SELECT VALUE c.URN FROM c";

            var result = _client.CreateDocumentQuery<int>(UriFactory.CreateDocumentCollectionUri(DatabaseId, _collectionName), query).ToList();

            return result;
        }

        public async Task<IEnumerable<EdubaseDataObject>> GetAcademiesByCompanyNoAsync(int companyNo)
        {
            var query = $"SELECT c['{EdubaseDataFieldNames.URN}'], " +
                $"c['{EdubaseDataFieldNames.ESTAB_NAME}'], " +
                $"c['{EdubaseDataFieldNames.OVERALL_PHASE}'], " +
                $"c['{EdubaseDataFieldNames.COMPANY_NUMBER}'] " +
                $"FROM c WHERE c.{EdubaseDataFieldNames.COMPANY_NUMBER}=@CompanyNo " +
                $"AND c.{EdubaseDataFieldNames.FINANCE_TYPE} = 'A' " +
                $"AND c.{EdubaseDataFieldNames.ESTAB_STATUS_IN_YEAR} = 'Open'";
            SqlQuerySpec querySpec = new SqlQuerySpec(query);
            querySpec.Parameters = new SqlParameterCollection();
            querySpec.Parameters.Add(new SqlParameter($"@CompanyNo", companyNo));

            try
            {
                var documentQuery = _client.CreateDocumentQuery<EdubaseDataObject>(UriFactory.CreateDocumentCollectionUri(DatabaseId, _collectionName), querySpec);
                return await documentQuery.QueryAsync();
            }
            catch (Exception ex)
            {
                var errorMessage = $"{_collectionName} could not be loaded! : {ex.Message} : {querySpec.Parameters[0].Name} = {querySpec.Parameters[0].Value}";
                base.LogException(ex, errorMessage);
                return null;
            }
        }

        public async Task<int> GetAcademiesCountByCompanyNoAsync(int companyNo)
        {
            var query = $"SELECT VALUE COUNT(c) " +
                $"FROM c WHERE c.{EdubaseDataFieldNames.COMPANY_NUMBER}=@CompanyNo " +
                $"AND c.{EdubaseDataFieldNames.FINANCE_TYPE} = 'A' " +
                $"AND c.{EdubaseDataFieldNames.ESTAB_STATUS_IN_YEAR} = 'Open'";
            SqlQuerySpec querySpec = new SqlQuerySpec(query);
            querySpec.Parameters = new SqlParameterCollection();
            querySpec.Parameters.Add(new SqlParameter($"@CompanyNo", companyNo));

            try
            {
                var documentQuery = _client.CreateDocumentQuery<int>(UriFactory.CreateDocumentCollectionUri(DatabaseId, _collectionName), querySpec);
                return (await documentQuery.QueryAsync()).First();
            }
            catch (Exception ex)
            {
                var errorMessage = $"{_collectionName} could not be loaded! : {ex.Message} : {querySpec.Parameters[0].Name} = {querySpec.Parameters[0].Value}";
                base.LogException(ex, errorMessage);
                return 0;
            }
        }

        #region Private methods

        private List<EdubaseDataObject> GetSchoolDataObjectById(Dictionary<string, object> fields)
        {

            var sb = new StringBuilder();
            foreach (var field in fields)
            {
                sb.Append($"c.{field.Key}=@{field.Key} AND ");
            }

            var where = sb.ToString().Substring(0, sb.ToString().Length - 5);

            var query =
                $"SELECT c['{EdubaseDataFieldNames.URN}'], c['{EdubaseDataFieldNames.ESTAB_NAME}'], c['{EdubaseDataFieldNames.OVERALL_PHASE}'], c['{EdubaseDataFieldNames.PHASE_OF_EDUCATION}'], c['{EdubaseDataFieldNames.TYPE_OF_ESTAB}'], c['{EdubaseDataFieldNames.STREET}'], c['{EdubaseDataFieldNames.TOWN}'], c['{EdubaseDataFieldNames.LOCATION}'], c['{EdubaseDataFieldNames.POSTCODE}'], c['{EdubaseDataFieldNames.TRUSTS}'], c['{EdubaseDataFieldNames.MAT_NUMBER}'], c['{EdubaseDataFieldNames.COMPANY_NUMBER}'], " +
                $"c['{EdubaseDataFieldNames.LA_CODE}'], c['{EdubaseDataFieldNames.ESTAB_NO}'], c['{EdubaseDataFieldNames.TEL_NO}'], c['{EdubaseDataFieldNames.NO_PUPIL}'], c['{EdubaseDataFieldNames.STAT_LOW}'], c['{EdubaseDataFieldNames.STAT_HIGH}'], c['{EdubaseDataFieldNames.HEAD_FIRST_NAME}'], " +
                $"c['{EdubaseDataFieldNames.HEAD_LAST_NAME}'], c['{EdubaseDataFieldNames.HAS_NURSERY}'], c['{EdubaseDataFieldNames.OFFICIAL_6_FORM}'], c['{EdubaseDataFieldNames.SCHOOL_WEB_SITE}'], c['{EdubaseDataFieldNames.OFSTED_RATING}'], c['{EdubaseDataFieldNames.OFSTE_LAST_INSP}'], udf.PARSE_FINANCIAL_TYPE_CODE(c['{EdubaseDataFieldNames.FINANCE_TYPE}']) AS {EdubaseDataFieldNames.FINANCE_TYPE}, c['{EdubaseDataFieldNames.OPEN_DATE}'], c['{EdubaseDataFieldNames.CLOSE_DATE}'] FROM c WHERE {where}";

            SqlQuerySpec querySpec = new SqlQuerySpec(query);
            querySpec.Parameters = new SqlParameterCollection();
            foreach (var field in fields)
            {
                querySpec.Parameters.Add(new SqlParameter($"@{field.Key}", field.Value));
            }

            List<EdubaseDataObject> result;
            try
            {
                result = _client.CreateDocumentQuery<EdubaseDataObject>(UriFactory.CreateDocumentCollectionUri(DatabaseId, _collectionName), querySpec, new FeedOptions() { MaxItemCount = 1 }).ToList();
            }catch(Exception ex)
            {
                var errorMessage = $"{_collectionName} could not be loaded! : {ex.Message} : {querySpec.Parameters[0].Name} = {querySpec.Parameters[0].Value}";
                base.LogException(ex, errorMessage);
                return null;
            }
            if (result == null)
            {
                throw new ApplicationException("School document not found in Edubase collection!");
            }
            return result;
        }

        private List<EdubaseDataObject> GetMultipleSchoolDataObjetsByIds(string fieldName, List<int> ids)
        {
            var sb = new StringBuilder();
            ids.ForEach(u => sb.Append(u + ","));

            var query = $"SELECT c['{EdubaseDataFieldNames.URN}'], " +
                $"c['{EdubaseDataFieldNames.ESTAB_NAME}'], " +
                $"c['{EdubaseDataFieldNames.OVERALL_PHASE}'], " +
                $"c['{EdubaseDataFieldNames.TYPE_OF_ESTAB}'], " +
                $"c['{EdubaseDataFieldNames.STREET}'], " +
                $"c['{EdubaseDataFieldNames.TOWN}'], " +
                $"c['{EdubaseDataFieldNames.POSTCODE}'], " +
                $"c['{EdubaseDataFieldNames.LA_CODE}'], " +
                $"c['{EdubaseDataFieldNames.NO_PUPIL}'], " +
                $"udf.PARSE_FINANCIAL_TYPE_CODE(c['{EdubaseDataFieldNames.FINANCE_TYPE}']) AS {EdubaseDataFieldNames.FINANCE_TYPE} " +
                $"FROM c WHERE c.{fieldName} IN ({sb.ToString().TrimEnd(',')})";

            List<EdubaseDataObject> result = null;
            try
            {
                result = _client.CreateDocumentQuery<EdubaseDataObject>(UriFactory.CreateDocumentCollectionUri(DatabaseId, _collectionName), query).ToList();
                if(result.Count < ids.Count)
                {
                    throw new Newtonsoft.Json.JsonSerializationException();
                }
            }catch(Exception ex)
            {
                var errorMessage = $"{_collectionName} could not be loaded! : {ex.Message} : URNs = {sb.ToString().TrimEnd(',')}";
                base.LogException(ex, errorMessage);
                throw new ApplicationException($"One or more documents could not be loaded from {_collectionName} : URNs = {sb.ToString().TrimEnd(',')}");
            }
            return result;
        }

        #endregion
    }
}