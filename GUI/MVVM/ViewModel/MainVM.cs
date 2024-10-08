﻿using Camera;
using DetectionLibrary;
using GUI.MVVM.ViewModel;
using KassenmanagementLibrary;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Timers;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Printing;
using GUI.Core;
using GUI.Services;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;
using System.Media;

namespace GUI.MVVM.ViewModel
{
    public class MainVM : ViewModelBase
    {
        public MainVM(IAddManuallyService addManuallyService, IPayService payService, IChangeWeightService changeWeightService, IWindowManager windowManager, ViewModelLocator viewModelLocator)
        {
            addManuallyService.ShoppingBasket = ShoppingBasket;

            EditLineOfGoodsService.LineOfGoods = LineOfGoods.GetFromXML();

            Camera.NewFrame += OnNewFrame;

            ScanCommand = new DelegateCommand(execute: async (o) =>
            {
            List<Bitmap> bitmapsToScan = new List<Bitmap>();

            ScanStatus = "Scanning process is running.";

            for (int i = 0; i < 15; i++)
            {
                await Task.Delay(50);

                if (CurrentBitmap != null)
                    bitmapsToScan.Add(CurrentBitmap);
            }
            IDetection detectionObj = new Detection();

            (SortedDictionary<double, Product>, Product ?) input = await Task.Run(() => detectionObj.getDetectionOutput(EditLineOfGoodsService.LineOfGoods, bitmapsToScan)); //asynchron 

				addManuallyService.ScanData = input.Item1;

                if (input.Item2 != null)
                {
                    _detectedSound.Play(); //sound for detected
                    ShoppingBasket.AddArticle(input.Item2);
                }
                else
                {
                    _notDetectedSound.Play(); //sound for not detected
                }

				ScanStatus = "Press Space to scan your product!";
                
            }, canExecute: (o) => CurrentBitmap != null);    




            DownQuantityCommand = new DelegateCommand(execute: (o) => 
            {
                if (SelectedArticle.Source.Quantityarticle)
                {
                    ShoppingBasket.DownQuantity(SelectedArticle);

                    if (ShoppingBasket._ShoppingBasket[ShoppingBasket._ShoppingBasket.IndexOf(SelectedArticle)].Quantity == 0)
                        ShoppingBasket._ShoppingBasket.Remove(SelectedArticle);
                }
                else
                {
                    changeWeightService.NewWeight = SelectedArticle.Quantity;

                    windowManager.ShowWindow(viewModelLocator.ChangeWeightVM);

                    ChangeWeightEvent?.Invoke(this, EventArgs.Empty);
                }
            }, canExecute: (o) => SelectedArticle != null);

            UpQuantityCommand = new DelegateCommand(execute: (o) =>
            {
                if (SelectedArticle.Source.Quantityarticle)
                {
                    ShoppingBasket.UpQuantity(SelectedArticle);
                }
                else
                {
                    changeWeightService.NewWeight = SelectedArticle.Quantity;

                    windowManager.ShowWindow(viewModelLocator.ChangeWeightVM);

                    ChangeWeightEvent?.Invoke(this, EventArgs.Empty);
                }
            },  canExecute: (o) => SelectedArticle != null);




            ClearCommand = new DelegateCommand(execute: (o) => { ShoppingBasket.Clear(); }, canExecute: (o) => ShoppingBasket._ShoppingBasket.Count >= 0);




            PayWindowCommand = new DelegateCommand((o) =>
            {
                payService.TotalPrice = ShoppingBasket.SumPrice;
               
                windowManager.ShowWindow(viewModelLocator.PayVM);

                PayEvent?.Invoke(this, EventArgs.Empty);
            });




            AddManuallyWindowCommand = new DelegateCommand((o) =>
            {
                if (addManuallyService.ScanData != null)
                {
                    windowManager.ShowWindow(viewModelLocator.AddManuallyVM);

                    AddManuallyEvent?.Invoke(this, EventArgs.Empty);
                }
                else
                {
                    MessageBox.Show("Please try to scan first!");
                }
            });




            EditLineOfGoodsWindowCommand = new DelegateCommand((o) => { windowManager.ShowWindow(viewModelLocator.SearchProductInLineOfGoodsVM); });




            CloseCommand = new DelegateCommand((o) => {EditLineOfGoodsService.LineOfGoods.Safe(); Environment.Exit(0);});

            /////////////////////////////////////////////////////SOUND INITIALIZING//////////////////////////////////////////////////
            string fileNameDetected = "Detected.wav";
            string filePathDetected = Path.Combine(AppDomain.CurrentDomain.BaseDirectory);
            filePathDetected = filePathDetected.Substring(0, filePathDetected.IndexOf("image-recognition-POS-System"));
            filePathDetected += "image-recognition-POS-System\\Sounds\\" + fileNameDetected;

            _detectedSound = new SoundPlayer(filePathDetected);

            string fileNameNotDetected = "NotDetected.wav";
            string filePathNotDetected = Path.Combine(AppDomain.CurrentDomain.BaseDirectory);
            filePathNotDetected = filePathNotDetected.Substring(0, filePathNotDetected.IndexOf("image-recognition-POS-System"));
            filePathNotDetected += "image-recognition-POS-System\\Sounds\\" + fileNameNotDetected;

            _notDetectedSound = new SoundPlayer(filePathNotDetected);
        }

        ////////////////////////////////////////////////////////ATTRIBUTES FOR SHOOPING BASKET//////////////////////////////////////////////////////////////
        
        public ShoppingBasket ShoppingBasket { get; set; } = new ShoppingBasket();

        public Article? SelectedArticle { get; set; }

        
        private string scanStatus = "Press Space to scan your product!";

        public string ScanStatus {
            get { return scanStatus; }
            set
            {
                if (scanStatus != value)
                {
                    scanStatus = value;
                    OnPropertyChanged(nameof(ScanStatus));
                }
            }
        }

        ////////////////////////////////////////////////////////////ATTRIBUTES FOR CAMERA//////////////////////////////////////////////////////////////

        public Cam Camera { get; set; } = new Cam();

        public Bitmap CurrentBitmap { get; set; }

        private BitmapSource _currentSource;

        public BitmapSource CurrentSource
        {
            get { return _currentSource; }
            set
            {
                if (_currentSource != value)
                {
                    _currentSource = value;
                    OnPropertyChanged(nameof(CurrentSource));
                }
            }
        }

        //Sounds

        private readonly SoundPlayer _detectedSound;

        private readonly SoundPlayer _notDetectedSound;

        //////////////////////////////////////////////////////////CAMERA METHODS////////////////////////////////////////////////////////////////////////

        public virtual void OnNewFrame(object? sender, EventArgs e)
        {
            if (Camera != null)
            {
                Bitmap bitmap = Camera.GetCurrentBitmap();

                CurrentBitmap = bitmap; //speichern in lokaler Variable

                EditLineOfGoodsService.CurrentBitmap = CurrentBitmap; //weitergeben an Service für add und edit LineOfGoods

                if (bitmap != null)
                {
                    // Überprüfen Sie, ob der Dispatcher verfügbar ist und auf dem UI-Thread läuft
                    if (System.Windows.Application.Current.Dispatcher.CheckAccess())
                    {
                        ShowBitmap(bitmap);
                    }
                    else
                    {
                        // Falls nicht, rufen Sie den Dispatcher des UI-Threads auf
                        System.Windows.Application.Current.Dispatcher.Invoke(() => ShowBitmap(bitmap));
                    }
                }
            }
        }

        private void ShowBitmap(Bitmap bitmap)
        {
            // Konvertiere das Bitmap in ein BitmapImage und zeige es in der GUI an
            BitmapImage bitmapImage = BmpImageFromBmp(bitmap);
            CurrentSource = bitmapImage;
        }

        private BitmapImage BmpImageFromBmp(Bitmap bmp) //Aus Forum: Transformiert Bitmap in GUI nutzbares BitmapSource
        {
            using (var memory = new System.IO.MemoryStream())
            {
                bmp.Save(memory, System.Drawing.Imaging.ImageFormat.Png);
                memory.Position = 0;

                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                bitmapImage.Freeze();

                return bitmapImage;
            }
        }


        /////////////////////////////////////////////////////////////////COMMANDS////////////////////////////////////////////////////////////////////

        public DelegateCommand UpQuantityCommand { get; set; }

        public DelegateCommand DownQuantityCommand { get; set; }

        public DelegateCommand PayWindowCommand { get; set; }

        public DelegateCommand EditLineOfGoodsWindowCommand { get; set;}

        public DelegateCommand AddManuallyWindowCommand { get; set; }

        public DelegateCommand ScanCommand { get; set; }

        public DelegateCommand CloseCommand { get; set; }

        public DelegateCommand ClearCommand { get; set; }

        ////////////////////////////////////////////////////////////////EVENTS/////////////////////////////////////////////////////////////////////

        public event EventHandler PayEvent = delegate { };

        public event EventHandler ChangeWeightEvent = delegate { };

        public event EventHandler AddManuallyEvent = delegate { };
    }
}
