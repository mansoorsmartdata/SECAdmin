﻿using Autofac;
using Autofac.Integration.WebApi;
using SECAdmin.Data;
using SECAdmin.Data.Infrastructure;
using SECAdmin.Data.Repositories;
using SECAdmin.Services;
using SECAdmin.Services.Abstract;
using System.Data.Entity;
using System.Reflection;
using System.Web.Http;

namespace SECAdmin.Web
{
    public class AutofacWebapiConfig
    {
        public static IContainer Container;
        public static void Initialize(HttpConfiguration config)
        {
            Initialize(config, RegisterServices(new ContainerBuilder()));
        }

        public static void Initialize(HttpConfiguration config, IContainer container)
        {
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
        }

        private static IContainer RegisterServices(ContainerBuilder builder)
        {
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());

            // EF HomeCinemaContext
            builder.RegisterType<SECAdminContext>()
                   .As<DbContext>()
                   .InstancePerRequest();

            builder.RegisterType<DbFactory>()
                .As<IDbFactory>()
                .InstancePerRequest();

            builder.RegisterType<UnitOfWork>()
                .As<IUnitOfWork>()
                .InstancePerRequest();

            builder.RegisterGeneric(typeof(EntityBaseRepository<>))
                   .As(typeof(IEntityBaseRepository<>))
                   .InstancePerRequest();

            //Services
            builder.RegisterAssemblyTypes(typeof(ClientDataService).Assembly).Where(t => t.Name.EndsWith("Service")).AsImplementedInterfaces().InstancePerRequest();

            Container = builder.Build();

            return Container;
        }
    }
}