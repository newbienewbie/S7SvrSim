﻿using MediatR;
using Microsoft.Extensions.DependencyInjection;
using S7Server.Simulator.ViewModels;
using S7Svr.Simulator.MessageHandlers;
using S7Svr.Simulator.ViewModels;
using S7Svr.Simulator;
using S7SvrSim.Services;
using S7SvrSim.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Splat;
using S7SvrSim.ViewModels.Rw;
using S7SvrSim.UserControls.Rws;

namespace S7SvrSim
{
    internal static class Exts
    {
        public static IServiceCollection AddS7CoreServices(this IServiceCollection services)
        {
            services.AddMediatR(typeof(MessageNotificationHandler).Assembly);
            services.AddSingleton<PyScriptRunner>();
            services.AddSingleton<IS7ServerService, S7ServerService>();
            services.AddSingleton<IS7DataBlockService, S7DataBlockService>();
            services.AddSingleton<IS7MBService, S7MBService>();

            return services;
        }


        public static void RegisterViews(this IServiceProvider sp)
        {
            Locator.CurrentMutable.RegisterLazySingletonEx<MsgLoggerVM>(sp);

            Locator.CurrentMutable.RegisterLazySingletonEx<ConfigPyEngineVM>(sp);
            Locator.CurrentMutable.RegisterLazySingletonEx<ConfigSnap7ServerVM>(sp);

            Locator.CurrentMutable.RegisterLazySingletonEx<RunningSnap7ServerVM>(sp);
            Locator.CurrentMutable.RegisterLazySingletonEx<RwTargetVM>(sp);
            Locator.CurrentMutable.RegisterLazySingletonEx<OperationVM>(sp);
            Locator.CurrentMutable.RegisterLazySingletonEx<MainVM>(sp);

            Locator.CurrentMutable.RegisterLazySingletonEx<RwBitVM>(sp);
            Locator.CurrentMutable.RegisterLazySingletonEx<RwByteVM>(sp);
            Locator.CurrentMutable.RegisterLazySingletonEx<RwShortVM>(sp);
            Locator.CurrentMutable.RegisterLazySingletonEx<RwUInt32VM>(sp);
            Locator.CurrentMutable.RegisterLazySingletonEx<RwUInt64VM>(sp);
            Locator.CurrentMutable.RegisterLazySingletonEx<RwRealVM>(sp);
            Locator.CurrentMutable.RegisterLazySingletonEx<RwStringVM>(sp);

            Locator.CurrentMutable.Register<IViewFor<RwBitVM>, BitOpsView>();
            Locator.CurrentMutable.Register<IViewFor<RwByteVM>, ByteOpsView>();
            Locator.CurrentMutable.Register<IViewFor<RwShortVM>, ShortOpsView>();
            Locator.CurrentMutable.Register<IViewFor<RwUInt32VM>, UInt32OpsView>();
            Locator.CurrentMutable.Register<IViewFor<RwUInt64VM>, UInt64OpsView>();
            Locator.CurrentMutable.Register<IViewFor<RwRealVM>, RealOpsView>();
            Locator.CurrentMutable.Register<IViewFor<RwStringVM>, StringOpsView>();

        }
    }
}