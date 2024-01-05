﻿using GUI.Core;
using GUI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUI.MVVM.ViewModel
{
    public class PayWindowViewModel : ViewModelBase
    {
        public PayWindowViewModel(IPayService payservice,ViewModelLocator viewModelLocator, IWindowManager windowManager)     
        {
            var mainVM = viewModelLocator.MainWindowViewModel;

            mainVM.PayEvent += (sender, args) => { TotalPrice = payservice.TotalPrice;  PaidAmount = 0.1; };

            CashInCommand = new DelegateCommand((o) =>
            {
                mainVM.shoppingBasketObject.Clear();
                windowManager.CloseWindow(viewModelLocator.PayWindowViewModel);
            });

            CloseCommand = new DelegateCommand((o) => windowManager.CloseWindow(viewModelLocator.PayWindowViewModel));
        }

        private double _totalPrice;

        public double TotalPrice {
            get {return _totalPrice;}
            set
            {
                if (_totalPrice != value)
                {
                    _totalPrice = value;
                    OnPropertyChanged(nameof(TotalPrice));
                }
            }
        }

        private double _paidAmount;
        public double PaidAmount
        {
            get { return _paidAmount; }
            set
            {
                if (_paidAmount != value)
                {
                    _paidAmount = value;
                    OnPropertyChanged(nameof(PaidAmount)); 
                    CalculateChange(); 
                }
            }
        }

        private void CalculateChange()
        {
            Change = Math.Round(PaidAmount - TotalPrice,2);
            OnPropertyChanged(nameof(Change)); 
        }
        public double Change { get; set; }


        ////////////////////////////////////////COMMANDS////////////////////////////////////////

        public DelegateCommand CashInCommand { get; set; }

        public DelegateCommand CloseCommand { get; set; }
    }
}
