using System;
using System.Collections.Generic;
using System.Security.Principal;
using Fathcore;
using Fathcore.Data.Abstractions;
using Fathcore.Extensions;
using Fathcore.Tests.Fakes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Moq;

namespace Fathcore.Tests
{
    public abstract class TestsBase
    {
        #region Fields
        #endregion

        #region Property
        public IPrincipal Principal { get; set; } = EngineContext.Current.Principal;
        public DefaultHttpContext HttpContext { get; set; } = new DefaultHttpContext();
        public RouteData RouteData { get; set; } = new RouteData();
        public ActionDescriptor ActionDescriptor { get; set; } = new ActionDescriptor();
        public List<IFilterMetadata> IFiltersMetadata { get; set; } = new List<IFilterMetadata>();
        #endregion

        #region Context Methods
        public ActionContext GetActionContext() => new ActionContext(HttpContext, RouteData, ActionDescriptor);

        public ActionExecutingContext GetActionExcecutingContext() => new ActionExecutingContext(GetActionContext(), IFiltersMetadata, null, GetMockController().Object);

        public ExceptionContext GetExceptionContext() => new ExceptionContext(GetActionContext(), IFiltersMetadata);

        public ExceptionContext GetExceptionContext(Exception exception)
        {
            ExceptionContext exceptionContext = GetExceptionContext();
            exceptionContext.Exception = exception;
            return exceptionContext;
        }
        #endregion
    
        #region Mocking Methods
        public Mock<ControllerBase> GetMockController() => new Mock<ControllerBase>();

        public Mock<IStringLocalizer<T>> GetMockStringLocalizer<T>(string key, string value)
        {
            Mock<IStringLocalizer<T>> mockStringLocalizer = new Mock<IStringLocalizer<T>>();
            var localizedString = new LocalizedString(key, value);
            mockStringLocalizer.Setup(_ => _[key]).Returns(localizedString);
            return mockStringLocalizer;
        }
        #endregion
    
        #region Services Methods
        public static ServiceProvider ConfigureProvider(Action<IServiceCollection> configure)
        {
            var services = new ServiceCollection();

            configure(services);

            return services.BuildServiceProvider();
        }
        #endregion

        #region Database Methods
        public DbContextOptions<T> DatabaseOptions<T>() where T : DbContext, IDbContext
        {
            string databaseName = Guid.NewGuid().ToString();
            return DatabaseOptions<T>(databaseName);
        }

        public DbContextOptions<T> DatabaseOptions<T>(string databaseName) where T : DbContext, IDbContext
        {
            DbContextOptions<T> options = new DbContextOptionsBuilder<T>().UseInMemoryDatabase(databaseName).Options;

            using (T context = Activator.CreateInstance(typeof(T), new object[] { options }) as T) context.Database.EnsureDeleted();
            using (T context = Activator.CreateInstance(typeof(T), new object[] { options }) as T) context.Database.EnsureCreated();

            return options;
        }

        public DbContextOptions<T> DatabaseOptionsWithData<T>(Type contextType = null) where T : DbContext, IDbContext
        {
            string databaseName = Guid.NewGuid().ToString();
            return DatabaseOptionsWithData<T>(databaseName, contextType ?? typeof(T));
        }

        public DbContextOptions<T> DatabaseOptionsWithData<T>(string databaseName, Type contextType) where T : DbContext, IDbContext
        {
            DbContextOptions<T> options = DatabaseOptions<T>(databaseName);

            List<FakeEntity> fakeEntities = new FakeEntities();

            using (IDbContext context = Activator.CreateInstance(contextType, new object[] { options }) as IDbContext)
            {
                context.Set<FakeEntity>().AddRange(fakeEntities);
                context.Audit();
                context.SaveChanges();
            }

            return options;
        }
        #endregion
    }
}
