using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Ruleta2023.Business.Core.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Xsl;
using Ruleta2023.Data.Access.MongoDb.ClientConfiguration.Implementation;
using Ruleta2023.Data.Access.MongoDb.ClientConfiguration.Contract;
using Autofac.Features.AttributeFilters;
using Ruleta2023.Business.Core.Business.User;
using Ruleta2023.Data.Access.Redis.Contract;
using Ruleta2023.Data.Access.Redis.Implementation;
using StackExchange.Redis;
using RedLockNet.SERedis.Configuration;
using RedLockNet.SERedis;
using Ruleta2023.Data.Access.MongoDb.RouletteConfiguration.Implementation;
using Ruleta2023.Business.Core.Business.Roulette;
using Ruleta2023.Data.Access.MongoDb.RouletteConfiguration.Contract;

namespace Ruleta2023.Business.Core.IoCContainer
{
    public static class IoCServiceCollection
    {
        private static string executionEnvironment;

        public static ContainerBuilder BuildContext(IServiceCollection services, IConfiguration configuration)
        {
            var builder = new ContainerBuilder();
            builder.Populate(services);
            return BuildContext(builder, configuration);
        }

        public static ContainerBuilder BuildContext(ContainerBuilder builder, IConfiguration configuration)
        {
            executionEnvironment = EnvironmentFinder.GetEnvironmentName(
                Environment.GetEnvironmentVariable("RULETA2023_ENVIRONMENT"));

            builder.RegisterInstance<string>(configuration["GroupConfig"]).Keyed<string>("Group");

            RegisterMongoDbRepositories(builder, configuration);
            RegisterBusinessImplementations(builder, configuration);
            RegisterStringValues(builder, configuration);
            RegisterDataAccess(builder, configuration);

            return builder;
        }

        private static void RegisterMongoDbRepositories(ContainerBuilder builder, IConfiguration configuration)
        {
            string connectionString = configuration["MongoDB:url"];
            bool usesTls = bool.Parse(configuration["MongoDB:usesTls"]);
            bool bypassCertificate = bool.Parse(configuration["MongoDB:bypassCertificate"]);

            MongoClient mongoDbClient = null;
            if (!usesTls)
            {
                mongoDbClient = new MongoClient(connectionString);
            }
            else
            {
                var mongoUrl = new MongoUrl(connectionString);
                var settings = MongoClientSettings.FromUrl(mongoUrl);
                settings.SslSettings = new SslSettings();
                settings.SslSettings.CheckCertificateRevocation = false;
                settings.UseTls = true;

                if (bypassCertificate)
                {
                    settings.AllowInsecureTls = true;
                }

                mongoDbClient = new MongoClient(settings);
            }
            builder.RegisterInstance(mongoDbClient).As<MongoClient>();


            builder.Register((context, parameters) => new MongoDBUserConfigurationManager(
                context.Resolve<MongoClient>(),
                configuration["MongoDB:database"],
                configuration["MongoDB:UsersCollection"]))
            .As<IUserConfigurationManager>().SingleInstance();

            builder.Register((context, parameters) => new MongoDBRouletteConfigurationManager(
                context.Resolve<MongoClient>(),
                configuration["MongoDB:database"],
                configuration["MongoDB:RouletteCollection"]))
            .As<IRouletteConfigurationManager>().SingleInstance();

        }

        private static void RegisterBusinessImplementations(ContainerBuilder builder, IConfiguration configuration)
        {
            builder.RegisterType<UserConfigurationBusiness>().WithAttributeFiltering();
            builder.RegisterType<RouletteConfigurationBusiness>().WithAttributeFiltering();
        }

        private static void RegisterStringValues(ContainerBuilder builder, IConfiguration configuration)
        {
            builder.RegisterInstance<string>(configuration["Redis:Url"]).Keyed<string>("RedisUrl");
            builder.RegisterInstance<string>(configuration["Redis:TtlSeconds"]);            
        }

        private static void RegisterDataAccess(ContainerBuilder builder, IConfiguration configuration)
        {
            builder.RegisterType<RedisCacheHelper>().As<ICacheHelper>().WithAttributeFiltering().SingleInstance();

            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(configuration["Redis:Url"]);
            builder.RegisterInstance(redis).As<IConnectionMultiplexer>().SingleInstance();
            List<RedLockMultiplexer> multiplexers = new List<RedLockMultiplexer>
            {
                redis
            };
            RedLockFactory redLockFactory = RedLockFactory.Create(multiplexers);
            builder.RegisterInstance(redLockFactory).As<RedLockFactory>();
        }
    }
}
