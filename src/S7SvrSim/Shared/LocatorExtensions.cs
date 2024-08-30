using Microsoft.Extensions.DependencyInjection;
using Splat;
using System;

namespace S7SvrSim.Shared.Utils
{
    public static class LocatorExtensions
    {

        #region location
        public static TService GetRequiredService<TService>(this IReadonlyDependencyResolver resolver)
        {
            return resolver.GetService<TService>() ?? throw new System.Exception($"Locator未注册{typeof(TService).Name}");
        }
        #endregion


        #region registion
        public static void RegisterTransientEx<TService>(this IMutableDependencyResolver resolver, IServiceProvider sp, params object[] args)
        {
            resolver.Register(() => ActivatorUtilities.CreateInstance<TService>(sp, args));
        }
        public static void RegisterTransientEx<TService, TImpl>(this IMutableDependencyResolver resolver, IServiceProvider sp, params object[] args)
            where TImpl : TService
        {
            resolver.Register<TService>(() => ActivatorUtilities.CreateInstance<TImpl>(sp, args));
        }



        public static void RegisterLazySingletonEx<TService>(this IMutableDependencyResolver resolver, IServiceProvider sp, params object[] args)
        {
            resolver.RegisterLazySingleton(() => ActivatorUtilities.CreateInstance<TService>(sp, args));
        }


        public static void RegisterLazySingletonEx<TService,TImpl>(this IMutableDependencyResolver resolver, IServiceProvider sp, params object[] args)
            where TImpl: TService
        {
            resolver.RegisterLazySingleton<TService>(() => ActivatorUtilities.CreateInstance<TImpl>(sp, args));
        }
        #endregion
    }
}
