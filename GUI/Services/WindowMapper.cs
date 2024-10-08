﻿using GUI.MVVM.ViewModel;
using GUI.MVVM.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GUI.Services
{
    /// <summary>
    /// Service um ViewModel und View zu verknüpfen 
    /// -> Wird später verwendet um WindowType über ViewModel anzusteuern
    /// </summary>
    public class WindowMapper
    {
        private readonly Dictionary<Type, Type> _mappings = new();

        public WindowMapper()
        {
            RegisterMapping<MainVM, MainWindow>();
            RegisterMapping<AddManuallyVM, addManuallyWindow>();
            RegisterMapping<EditProductInLineOfGoodsVM, editProductInLineOfGoodsWindow>();
            RegisterMapping<AddProductToLineOfGoodsVM, addProductToLineOfGoodsWindow>();
            RegisterMapping<SearchProductInLineOfGoodsVM, searchProductInLineOfGoodsWindow>();
            RegisterMapping<PayVM, PayWindow>();
            RegisterMapping<ChangeWeightVM, ChangeWeightWindow>();
        }

        public void RegisterMapping<TViewModel, TWindow>() where TViewModel : ViewModelBase where TWindow : Window
        {
            _mappings[typeof(TViewModel)] = typeof(TWindow);
        }

        public Type GetWindowTypeForViewModel(Type viewModelType)
        {
            _mappings.TryGetValue(viewModelType, out var windowType);
            return windowType;
        }
    }
}
