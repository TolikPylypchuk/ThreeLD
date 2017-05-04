using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Web.Mvc;

using Ninject;
using Ninject.Web.Common;
using ThreeLD.DB;
using ThreeLD.DB.Models;
using ThreeLD.DB.Repositories;

namespace ThreeLD.Web.Infrastructure
{
	[ExcludeFromCodeCoverage]
	public class NinjectDependencyResolver : IDependencyResolver
	{
		private IKernel kernel;

		public NinjectDependencyResolver(IKernel kernelParam)
		{
			this.kernel = kernelParam;
			this.AddBindings();
		}

		public object GetService(Type serviceType)
		{
			return this.kernel.TryGet(serviceType);
		}

		public IEnumerable<object> GetServices(Type serviceType)
		{
			return this.kernel.GetAll(serviceType);
		}

		private void AddBindings()
		{
			this.kernel.Bind<IRepository<Event>>()
					   .To<DbRepository<Event>>()
					   .InRequestScope();

			this.kernel.Bind<IRepository<Preference>>()
					   .To<DbRepository<Preference>>()
					   .InRequestScope();

            this.kernel.Bind<IRepository<Notification>>()
                       .To<DbRepository<Notification>>()
                       .InRequestScope();

            this.kernel.Bind<AppDbContext>()
					   .ToSelf()
					   .InRequestScope();
		}
	}
}
