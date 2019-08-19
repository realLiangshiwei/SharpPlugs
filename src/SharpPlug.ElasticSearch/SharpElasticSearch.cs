using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Elasticsearch.Net;
using Microsoft.Extensions.Options;
using Nest;
using SharpPlug.ElasticSearch.Configuration;

namespace SharpPlug.ElasticSearch
{
    /// <summary>
    /// .NetCore esPlug
    /// </summary>
    public class SharpElasticSearch : ISharpElasticsearch
    {
        public IElasticClient EsClient { get; set; }

        private readonly IOptions<ElasticSearchOptions> _options;

        public SharpElasticSearch(IOptions<ElasticSearchOptions> options)
        {
            _options = options;
            EsClient = GetClient();
        }

        /// <summary>
        /// GetesClient
        /// </summary>
        /// <returns></returns>
        private ElasticClient GetClient()
        {
            var str = _options.Value.ConnectionString;
            var strs = str.Split('|');
            var nodes = strs.Select(s => new Uri(s)).ToList();
            var connectionPool = new SniffingConnectionPool(nodes);
            var connectionString = new ConnectionSettings(connectionPool);
           
            connectionString.BasicAuthentication(_options.Value.AuthUserName, _options.Value.AuthPassWord);

            return new ElasticClient(connectionString);
        }

        /// <summary>
        /// CreateEsIndex Not Mapping
        /// Auto Set Alias alias is Input IndexName
        /// </summary>
        /// <param name="indexName"></param>
        /// <param name="shard"></param>
        /// <param name="numberOfReplicas"></param>
        /// <returns></returns>
        public virtual async Task CrateIndexAsync(string indexName, int shard = 1, int numberOfReplicas = 1)
        {
            var exits = await EsClient.Indices.AliasExistsAsync(indexName);

            if (exits.Exists)
                return;
            var newName = indexName + DateTime.Now.Ticks;
            var result = await EsClient
                .Indices.CreateAsync(newName,
                    ss =>
                        ss.Index(newName)
                            .Settings(
                                o => o.NumberOfShards(shard).NumberOfReplicas(numberOfReplicas).Setting("max_result_window", int.MaxValue)));
            if (result.Acknowledged)
            {
                await EsClient.Indices.PutAliasAsync(newName, indexName);
                return;
            }
            throw new ElasticSearchException($"Create Index {indexName} failed :" + result.ServerError.Error.Reason);
        }

        /// <summary>
        /// CreateEsIndex auto Mapping T Property
        /// Auto Set Alias alias is Input IndexName
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="indexName"></param>
        /// <param name="shard"></param>
        /// <param name="eplica"></param>
        /// <returns></returns>
        public virtual async Task CreateIndexAsync<T>(string indexName, int shard = 1, int eplica = 1) where T : class
        {
            var exits = await EsClient.Indices.AliasExistsAsync(indexName);

            if (exits.Exists)
                return;
            var newName = indexName + DateTime.Now.Ticks;
            var result = await EsClient
                .Indices.CreateAsync(newName,
                    ss =>
                        ss.Index(newName)
                            .Settings(
                                o => o.NumberOfShards(shard).NumberOfReplicas(eplica).Setting("max_result_window", int.MaxValue))
                            .Map(m => m.AutoMap<T>()));
            if (result.Acknowledged)
            {
                await EsClient.Indices.PutAliasAsync(newName, indexName);
                return;
            }
            throw new ElasticSearchException($"Create Index {indexName} failed : :" + result.ServerError.Error.Reason);
        }

        /// <summary>
        /// AddOrUpdate Document
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="indexName"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public virtual async Task AddOrUpdateAsync<T>(string indexName, T model) where T : class
        {
            var exis = EsClient.DocumentExists(DocumentPath<T>.Id(new Id(model)), dd => dd.Index(indexName));

            if (exis.Exists)
            {
                var result = await EsClient.UpdateAsync(DocumentPath<T>.Id(new Id(model)),
                    ss => ss.Index(indexName).Doc(model).RetryOnConflict(3));

                if (result.ServerError == null) return;
                throw new ElasticSearchException($"Update Document failed at index{indexName} :" + result.ServerError.Error.Reason);
            }
            else
            {
                var result = await EsClient.IndexAsync<T>(model, ss => ss.Index(indexName));
                if (result.ServerError == null) return;
                throw new ElasticSearchException($"Insert Docuemnt failed at index {indexName} :" + result.ServerError.Error.Reason);
            }
        }

        /// <summary>
        /// Bulk AddOrUpdate Docuemnt,Default bulkNum is 1000
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="indexName"></param>
        /// <param name="list"></param>
        /// <param name="bulkNum">bulkNum</param>
        /// <returns></returns>
        public virtual async Task BulkAddorUpdateAsync<T>(string indexName, List<T> list, int bulkNum = 1000) where T : class
        {
            if (list.Count <= bulkNum)
                await BulkAddOrUpdate<T>(indexName, list);
            else
            {

                var total = (int)Math.Ceiling(list.Count * 1.0f / bulkNum);
                var tasks = new List<Task>();
                for (var i = 0; i < total; i++)
                {
                    var i1 = i;
                    tasks.Add(Task.Factory.StartNew(() => BulkAddOrUpdate(indexName, list.Skip(i1 * bulkNum).Take(bulkNum).ToList()))); ;
                }
                await Task.WhenAll(tasks.ToArray());

            }
        }

        private async Task BulkAddOrUpdate<T>(string indexName, List<T> list) where T : class
        {
            var bulk = new BulkRequest(indexName)
            {
                Operations = new List<IBulkOperation>()
            };
            foreach (var item in list)
            {
                bulk.Operations.Add(new BulkIndexOperation<T>(item));
            }
            var response = await EsClient.BulkAsync(bulk);
            if (response.Errors)
                throw new Exception($"Bulk InsertOrUpdate Docuemnt failed at index {indexName} :{response.ServerError.Error.Reason}");

        }

        private async Task BulkDelete<T>(string indexName, List<T> list) where T : class
        {

            var bulk = new BulkRequest(indexName)
            {
                Operations = new List<IBulkOperation>()
            };
            foreach (var item in list)
            {
                bulk.Operations.Add(new BulkDeleteOperation<T>(new Id(item)));
            }
            var response = await EsClient.BulkAsync(bulk);
            if (response.Errors)
                throw new Exception($"Bulk Delete Docuemnt at index {indexName} :{response.ServerError.Error.Reason}");
        }

        /// <summary>
        ///  Bulk Delete Docuemnt,Default bulkNum is 1000
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="indexName"></param>
        /// <param name="list"></param>
        /// <param name="bulkNum">bulkNum</param>
        /// <returns></returns>
        public virtual async Task BulkDeleteAsync<T>(string indexName, List<T> list, int bulkNum = 1000) where T : class
        {
            if (list.Count <= bulkNum)
                await BulkDelete<T>(indexName, list);
            else
            {
                var total = (int)Math.Ceiling(list.Count * 1.0f / bulkNum);
                var tasks = new List<Task>();
                for (var i = 0; i < total; i++)
                {
                    var i1 = i;
                    tasks.Add(Task.Factory.StartNew(() => BulkDelete<T>(indexName, list.Skip(i1 * bulkNum).Take(bulkNum).ToList())));
                }
                await Task.WhenAll(tasks);

            }
        }

        /// <summary>
        /// Delete Docuemnt
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="indexName"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public virtual async Task DeleteAsync<T>(string indexName, T model) where T : class
        {
            var response = await EsClient.DeleteAsync(new DeleteRequest(indexName, new Id(model)));
            if (response.ServerError == null) return;
            throw new Exception($"Delete Docuemnt at index {indexName} :{response.ServerError.Error.Reason}");

        }

        /// <summary>
        /// Delete Index
        /// </summary>
        /// <param name="indexName"></param>
        /// <returns></returns>
        public virtual async Task DeleteIndexAsync(string indexName)
        {
            var response = await EsClient.Indices.DeleteAsync(indexName);
            if (response.Acknowledged) return;
            throw new Exception($"Delete index {indexName} failed :{response.ServerError.Error.Reason}");
        }

        public virtual async Task ReIndex<T>(string indexName) where T : class
        {
            await DeleteIndexAsync(indexName);
            await CreateIndexAsync<T>(indexName);
        }
        /// <summary>
        /// Non-stop Update Doucments
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="indexName"></param>
        /// <returns></returns>
        public virtual async Task ReBuild<T>(string indexName) where T : class
        {
            var result = await EsClient.Indices.GetAliasAsync(indexName);
            var oldName = result.Indices.Keys.First();
            //创建新的索引
            var newIndex = indexName + DateTime.Now.Ticks;
            var createResult = await EsClient.Indices.CreateAsync(newIndex,
                c =>
                    c.Index(newIndex)
                        .Map(m => m.AutoMap<T>()));
            if (!createResult.Acknowledged)
            {
                throw new Exception($"reBuild create newIndex {indexName} failed :{result.ServerError.Error.Reason}");
            }
            //重建索引数据
            var reResult = await EsClient.ReindexOnServerAsync(descriptor => descriptor.Source(source => source.Index(indexName))
                .Destination(dest => dest.Index(newIndex)));

            if (reResult.ServerError != null)
            {
                throw new Exception($"reBuild {indexName} datas failed :{reResult.ServerError.Error.Reason}");
            }

            //删除旧索引
            var deleteResult = await EsClient.Indices.DeleteAsync(oldName);
            var reAliasResult = await EsClient.Indices.PutAliasAsync(newIndex, indexName);

            if (!deleteResult.Acknowledged)
            {
                throw new Exception($"reBuild delete old Index {oldName.Name}   failed :{deleteResult.ServerError.Error.Reason}");
            }
            if (!reAliasResult.IsValid)
            {
                throw new Exception($"reBuild set Alias {indexName}  failed :{reAliasResult.ServerError.Error.Reason}");
            }
        }

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
        public virtual async Task<ISearchResponse<T>> SearchAsync<T>(string indexName, SearchDescriptor<T> query, int skip, int size, string[] includeFields = null,
            string preTags = "<strong style=\"color: red;\">", string postTags = "</strong>", bool disableHigh = false, params string[] highField) where T : class
        {
            query.Index(indexName);
            var highdes = new HighlightDescriptor<T>();
            if (disableHigh)
            {
                preTags = "";
                postTags = "";
            }
            highdes.PreTags(preTags).PostTags(postTags);

            var ishigh = highField != null && highField.Length > 0;

            var hfs = new List<Func<HighlightFieldDescriptor<T>, IHighlightField>>();

            //分页
            query.Skip(skip).Take(size);
            //关键词高亮
            if (ishigh)
            {
                foreach (var s in highField)
                {
                    hfs.Add(f => f.Field(s));
                }
            }

            highdes.Fields(hfs.ToArray());
            query.Highlight(h => highdes);
            if (includeFields != null)
                query.Source(ss => ss.Includes(ff => ff.Fields(includeFields.ToArray())));
            var response = await EsClient.SearchAsync<T>(query);
            return response;
        }
    }
}
