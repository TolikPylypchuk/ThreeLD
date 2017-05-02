using System;
using System.Diagnostics.CodeAnalysis;
using System.Web;
using System.Web.Mvc;

using Microsoft.Web.Infrastructure.DynamicModuleHelper;

using Ninject;
using Ninject.Web.Common;

using ThreeLD.Web.Infrastructure;

[assembly: WebActivatorEx.PreApplicationStartMethod(
	typeof(ThreeLD.Web.App_Start.NinjectWebCommon), "Start")]
[assembly: WebActivatorEx.ApplicationShutdownMethod(
	typeof(ThreeLD.Web.App_Start.NinjectWebCommon), "Stop")]

namespace ThreeLD.Web.App_Start
{
	[ExcludeFromCodeCoverage]
	public static class NinjectWebCommon 
	{
		private static readonly Bootstrapper bootstrapper = new Bootstrapper();
		
		public static void Start() 
		{
			DynamicModuleUtility.RegisterModule(
				typeof(OnePerRequestHttpModule));
			DynamicModuleUtility.RegisterModule(
				typeof(NinjectHttpModule));
			bootstrapper.Initialize(CreateKernel);
		}
        
		public static void Stop()
		{
			bootstrapper.ShutDown();
		}
        
		private static IKernel CreateKernel()
		{
			var kernel = new StandardKernel();
			try
			{
				kernel.Bind<Func<IKernel>>()
					  .ToMethod(ctx => () => new Bootstrapper().Kernel);
				kernel.Bind<IHttpModule>()
					  .To<HttpApplicationInitializationHttpModule>();

				RegisterServices(kernel);
				return kernel;
			} catch
			{
				kernel.Dispose();
				throw;
			}
		}
		
		private static void RegisterServices(IKernel kernel)
		{
			DependencyResolver.SetResolver(
				new NinjectDependencyResolver(kernel));
		}        
	}
}
