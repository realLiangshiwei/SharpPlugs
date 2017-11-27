using System;
using Microsoft.Extensions.DependencyInjection;
using SharpPlug.Core;
using SharpPlug.ElasticSearch.Configuration;

namespace SharpPlug.ElasticSearch
{
    /// <summary>
    ///  Extension methods for setting up Es services in an Microsoft.Extensions.DependencyInjection.IServiceCollection.
    /// </summary>
    public static class EsServiceCollectionExtensions
    {
        public static readonly ElasticSearchOptions DefaultElasticSearchOptions = new ElasticSearchOptions();
        public static ISharpPlugBuilder AddElasticSearchPlug(this ISharpPlugBuilder builder, Action<ElasticSearchOptions> setupAction)
        {
            setupAction(DefaultElasticSearchOptions);
            builder.Services.AddTransient<ISharpElasticsearch, SharpElasticSearch>();
            return builder;
        }
    }
}
