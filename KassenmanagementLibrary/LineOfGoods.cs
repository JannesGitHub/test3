﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;


namespace KassenmanagementLibrary
{
   
    [Serializable]
    public class LineOfGoods : ILineOfGoods
    {


        private static LineOfGoods instance;

        public LineOfGoods GetLineOfGoods()
        {
            return instance;
        }

        public static LineOfGoods Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new LineOfGoods();
                }
                return instance;
            }

        }
        private LineOfGoods()
        {
            // Initialisiere die Liste, wenn noch nicht geschehen
            if (lineOfGoods == null)
            {
                //überprüfe ob filePath leer ist
                // wenn nicht leer-> direkt vorhandenes laden


                // wenn leer -> neues erstellen
                lineOfGoods = new List<Product>();
            }
            
        }
        public List<Product> lineOfGoods { get; set; }

       
        // prüft ob atrikel im sortiment existiert anhand des namens
        public Product FindProduct(string name)
        {
           
            foreach (Product p in lineOfGoods)
            {
                if (p.Name == name)
                {
                    return p;
                }
            }
            return null;
        }
        

        // überladung der containsmethode, sucht anhand der atrikelnummer
        public Product FindProduct(uint articlenumber)//umbenennen
        {
            
            foreach (Product p in lineOfGoods)
            {
                if (articlenumber == p.Articlenumber)
                {
                    return p;
                }
            }
            return null;

        }

        //fügt akritel dem sortiment hinzu
        public void Add(Product p)
        {
            lineOfGoods.Add(p);
        }

        // löscht artikel aus sortiment
        public void Remove(Product p)
        {
            lineOfGoods.Remove(p);
        }


       
       /* public void editProductName(Product product, string name)
        {
            product.Name = name;
        }
        public void editProductArticlenumber(Product product, uint articlenumber) 
        {
            product.Articlenumber = articlenumber;
        }
        public void editProductPrice(Product product, double price)
        {
            product.Price=price;
        }
        public void editProductQuantityarticle(Product product,bool quantityarticle)
        {
            product.Quantityarticle=quantityarticle;
        }*/
      
        XmlSerializer serializer = new XmlSerializer(typeof(LineOfGoods));
       
        //speichert das Sortiment objekt in executeable
      
        public void Safe()
        {
            string fileName = "LineOfGoods.xml";
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory);
			filePath = filePath.Substring(0, filePath.IndexOf("j-kassenscanner"));
			filePath += "j-kassenscanner\\"+fileName;

            //Prüft ob der Dateipfad existiert. Falls nicht wirft er eine Exception


            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentException("Ungültiger Dateipfad.", nameof(filePath));
            }

            string directoryPath = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }


            using (FileStream fs = new FileStream(filePath, FileMode.Create))
            {
                serializer.Serialize(fs, this);
            }
        }


        //frage: geht es auch ohne static und als void??
        public static LineOfGoods GetFromXML()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(LineOfGoods));

            string fileName = "LineOfGoods.xml";
			string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory);
			filePath = filePath.Substring(0, filePath.IndexOf("j-kassenscanner"));
			filePath += "j-kassenscanner\\" + fileName;
			if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentException("Ungültiger Dateipfad.", nameof(filePath));
            }
            else
            {
                using (Stream reader = File.OpenRead(filePath))
                {
                    instance = (LineOfGoods)serializer.Deserialize(reader);
                }
            }
            return instance;
        }

        //erstellt sortimentobjekt
        public static LineOfGoods GetDummi(List<CLIPVector>vectorsForProduct1)
        {

            //vectoren von Can bekommen

            //CLIPVector vector1 = new CLIPVector(new double[] {1,5,2,6});
            //CLIPVector vector2 = new CLIPVector(new double[] {6,3,1,8});
            //CLIPVector vector3 = new CLIPVector(new double[] {12,5,23,6});
            //CLIPVector vector4 = new CLIPVector(new double[] {12,26,4,1});
            //CLIPVector vector5 = new CLIPVector(new double[] {1,26,78,9});

            //List<CLIPVector> vectorsForProduct1 = new List<CLIPVector> { vector1, vector2 };
            Product product1 = new Product("Orange", 1, 19.99, false, vectorsForProduct1);

           /* List<CLIPVector> vectorsForProduct2 = new List<CLIPVector> { vector3, vector4};
            Product product2 = new Product("Apfel", 2, 29.99, false, vectorsForProduct2);

            List<CLIPVector> vectorsForProduct3 = new List<CLIPVector> { vector5, vector1};
            Product product3 = new Product("Eistee", 3, 39.99, true, vectorsForProduct3);

            List<CLIPVector> vectorsForProduct4 = new List<CLIPVector> { vector2, vector3};
            Product product4 = new Product("Schokolade", 4, 49.99, true, vectorsForProduct4);

            List<CLIPVector> vectorsForProduct5 = new List<CLIPVector> { vector4, vector5};
            Product product5 = new Product("Kartoffeln", 5, 59.99, false, vectorsForProduct5);*/

            LineOfGoods line = new LineOfGoods();
            line.Add(product1);
           
            /* line.Add(product2);
            line.Add(product3);
            line.Add(product4);
            line.Add(product5);*/

            return line;
        }
    }
}
