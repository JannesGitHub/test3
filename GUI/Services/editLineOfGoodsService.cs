﻿using KassenmanagementLibrary;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GUI.Services
{

    public interface IEditLineOfGoods
    {
        public static LineOfGoods LineOfGoods { get; set; }

        public static Product SelectedProduct {  get; set; }

        public static Bitmap CurrentBitmap { get; set; }

        public void DeleteProduct(Product product);

        public void EditProduct(Product product);

        public void AddProduct(Product product);

        public bool IsIdUnique(uint id);
    }

    public class EditLineOfGoodsService : IEditLineOfGoods
    {
        public static LineOfGoods LineOfGoods { get; set; }

        public static Product SelectedProduct { get; set; }

        public static Bitmap CurrentBitmap { get; set; }

        public void DeleteProduct(Product product)
        {
            LineOfGoods.Remove(product);
        }

        public void EditProduct(Product product)
        {
            int indexToChange = LineOfGoods.lineOfGoods.FindIndex(p => p.Articlenumber == product.Articlenumber);
            LineOfGoods.lineOfGoods[indexToChange] = product; 
        }

        public void AddProduct(Product product)
        {
            LineOfGoods.lineOfGoods.Add(product);
        }

        public bool IsIdUnique(uint id)
        {
            return !LineOfGoods.lineOfGoods.Any((p) => p.Articlenumber == id);
        }
    }
}
