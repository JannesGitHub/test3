﻿using GUI.Services;
using GUI.MVVM.ViewModel;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace GUI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly IServiceCollection services = new ServiceCollection();

        private readonly IServiceProvider _serviceProvider;

        public App()
        {
            services.AddSingleton<MainVM>();
            services.AddSingleton<AddManuallyVM>();
            services.AddSingleton<EditProductInLineOfGoodsVM>();
            services.AddSingleton<AddProductToLineOfGoodsVM>();
            services.AddSingleton<SearchProductInLineOfGoodsVM>();
            services.AddSingleton<PayVM>();

            services.AddSingleton<ViewModelLocator>();
            services.AddSingleton<WindowMapper>();

            services.AddSingleton<IWindowManager, WindowManager>();
            services.AddSingleton<IAddManuallyService, AddManuallyService>();
            services.AddSingleton<IEditLineOfGoods, EditLineOfGoodsService>();
            services.AddSingleton<IPayService, PayService>();

            _serviceProvider = services.BuildServiceProvider();

        }

        protected override void OnStartup(StartupEventArgs e)
        {
            var windowManager = _serviceProvider.GetRequiredService<IWindowManager>();
            windowManager.ShowWindow(viewModel: _serviceProvider.GetRequiredService<MainVM>());
            base.OnStartup(e);
        }

    }
}
