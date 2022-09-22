using DaiMaWuDong.AgileFramework.Common.IOCOptions;
using DaiMaWuDong.MSACommerce.Search.Interface;
using Elasticsearch.Net;
using Microsoft.Extensions.Options;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaiMaWuDong.MSACommerce.Search.Service
{
    public class ElasticSearchService : IElasticSearchService
    {
        private readonly ElasticSearchOptions _elasticSearchOptions;

        public ElasticSearchService(IOptionsMonitor<ElasticSearchOptions> optionsMonitor)
        {
            _elasticSearchOptions = optionsMonitor.CurrentValue;
            var uris = _elasticSearchOptions.Urls
                .Select(url => new Uri(url));
            var connectionPool = new StaticConnectionPool(uris);
            var connectionSettings = new ConnectionSettings(connectionPool)
                .DefaultIndex(_elasticSearchOptions.IndexName);
            Client = new ElasticClient(connectionSettings);
        }
        private ElasticClient Client;
        public ElasticClient GetElasticClient()
        {
            return Client;
        }
        public void Send<T>(List<T> model) where T : class
        {
            Client.IndexMany(model);
        }

        public void InsertOrUpdata<T>(T model) where T : class
        {
            Client.IndexDocument(model);
        }

        public bool Delete<T>(string id) where T : class
        {

            var response = Client.Delete<T>(id);
            return response.IsValid;
        }
        public bool DropIndex(string indexName)
        {
            return Client.Indices.Delete(Indices.Parse(indexName)).IsValid;
        }

        public void CreateIndex(string indexName)
        {
            Client.Indices.Create(indexName);
        }

    }
}
