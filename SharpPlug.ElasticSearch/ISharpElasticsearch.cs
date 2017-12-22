using System.Collections.Generic;
using System.Threading.Tasks;
using Nest;

namespace SharpPlug.ElasticSearch
{
    public interface ISharpElasticsearch
    {
        IElasticClient EsClient { get; set; }

        /// <summary>
        /// CreateEsIndex Not Mapping
        /// Auto Set Alias alias is Input IndexName
        /// </summary>
        /// <param name="indexName"></param>
        /// <returns></returns>
        Task CrateIndexAsync(string indexName);

        /// <summary>
        /// CreateEsIndex auto Mapping T Property
        /// Auto Set Alias alias is Input IndexName
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="indexName"></param>
        /// <param name="shard"></param>
        /// <param name="eplica"></param>
        /// <returns></returns>
        Task CreateIndexAsync<T>(string indexName, int shard = 1, int eplica = 1) where T : class;

        /// <summary>
        /// ReIndex
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="indexName"></param>
        /// <returns></returns>
        Task ReIndex<T>(string indexName) where T : class;


        /// <summary>
        /// AddOrUpdate Document
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="indexName"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        Task AddOrUpdateAsync<T>(string indexName, T model) where T : class;


        /// <summary>
        /// Bulk AddOrUpdate Docuemnt,Default bulkNum is 1000
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="indexName"></param>
        /// <param name="list"></param>
        /// <param name="bulkNum">bulkNum</param>
        /// <returns></returns>
        Task BulkAddorUpdateAsync<T>(string indexName, List<T> list, int bulkNum = 1000) where T : class;

        /// <summary>
        ///  Bulk Delete Docuemnt,Default bulkNum is 1000
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="indexName"></param>
        /// <param name="list"></param>
        /// <param name="bulkNum">bulkNum</param>
        /// <returns></returns>
        Task BulkDeleteAsync<T>(string indexName, List<T> list, int bulkNum = 1000) where T : class;

        /// <summary>
        /// Delete Docuemnt
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="indexName"></param>
        /// <param name="typeName"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        Task DeleteAsync<T>(string indexName, string typeName, T model) where T : class;

        /// <summary>
        /// Delete Index
        /// </summary>
        /// <param name="indexName"></param>
        /// <returns></returns>
        Task DeleteIndexAsync(string indexName);


        /// <summary>
        /// Non-stop Update Doucments
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="indexName"></param>
        /// <returns></returns>
        Task ReBuild<T>(string indexName) where T : class;

        /// <summary>
        /// search
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="indexName"></param>
        /// <param name="query"></param>
        /// <param name="skip">skip num</param>
        /// <param name="size">return document size</param>
        /// <param name="includeFields">return fields</param>
        /// <param name="preTags">Highlight tags</param>
        /// <param name="postTags">Highlight tags</param>
        /// <param name="disableHigh"></param>
        /// <param name="highField">Highlight fields</param>
        /// <returns></returns>
        Task<ISearchResponse<T>> SearchAsync<T>(string indexName, SearchDescriptor<T> query,
            int skip, int size, string[] includeFields = null, string preTags = "<strong style=\"color: red;\">",
            string postTags = "</strong>", bool disableHigh = false, params string[] highField) where T : class;
    }
}
