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


            builder.Register((context, parameters) => new MongoDBClientConfigurationManager(
                context.Resolve<MongoClient>(),
                configuration["MongoDB:database"],
                configuration["MongoDB:UsersCollection"]))
            .As<IClientConfigurationManager>().SingleInstance();       

        }
    }
}
