using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace PSI_V2
{
    class MyImage
    {
        byte[] bfType = new byte[2]; //Type de fichier
        byte[] bfSize = new byte[4]; // Taille totale du ficher
        byte[] bfReserved1 = new byte[2]; // Champ réservé
        byte[] bfReserved2 = new byte[2];// champ réservé
        byte[] bfOffBits = new byte[4]; //Adresse de la zone de définition de l'image
        byte[] biSize = new byte[4]; //Taille en octet du header
        byte[] biWidth = new byte[4]; //Largeur de l'image en pixels
        byte[] biHeight = new byte[4]; //Hauteur de l'image en pixels
        byte[] biPlanes = new byte[2]; //Nombre de plans
        byte[] biBitCount = new byte[2]; //Nombre de bits par pixels
        byte[] biCompression = new byte[4]; //Type de compression : 0=pas de compression ; 1 = commpressé à 8 bits par pixel ; 2 = 4bits par pixels
        byte[] biSizeImage = new byte[4]; // Taille en octet des données de l'image
        byte[] biXpelsPerMeter = new byte[4]; // Résolution horizontale en pixels par mètre
        byte[] biYpelsPerMeter = new byte[4]; //Résolution vertical en pixels par mètre
        byte[] biClrUsed = new byte[4]; //Nombre e couleurs dans l'imahe : 0 = maximum possible. Si une palette estutilisée, ce nombre indique le nombre de couleurs de la palette
        byte[] biClrImportant = new byte[4]; // Nombre de couleurs importantes. 0 = Toutes importantes

        int hauteur;
        int largeur;
        int tailleOffset;

        byte[] input;
        byte[] header;
        byte[] image;

        Pixel[,] matriceRVB;
        public byte [] GetHeader
        {
            get { return header; }
        }
        public Pixel [,] GetMatriceRvb
        {
            get { return matriceRVB; }
        }

        public MyImage(Pixel[,] matriceRVB)
        {
            bfType = new byte[] { 66, 77 };
            UpdateVariables();
        }
        public MyImage(string file)
        {
            input = File.ReadAllBytes(file);

            Array.Copy(input, 0, bfType, 0, 2);
            Array.Copy(input, 2, bfSize, 0, 4);
            Array.Copy(input, 6, bfReserved1, 0, 2);
            Array.Copy(input, 8, bfReserved2, 0, 2);
            Array.Copy(input, 10, bfOffBits, 0, 4);
            Array.Copy(input, 14, biSize, 0, 4);
            Array.Copy(input, 18, biWidth, 0, 4);
            Array.Copy(input, 22, biHeight, 0, 4);
            Array.Copy(input, 26, biPlanes, 0, 2);
            Array.Copy(input, 28, biBitCount, 0, 2);
            Array.Copy(input, 30, biCompression, 0, 4);
            Array.Copy(input, 34, biSizeImage, 0, 4);
            Array.Copy(input, 38, biXpelsPerMeter, 0, 4);
            Array.Copy(input, 42, biYpelsPerMeter, 0, 4);
            Array.Copy(input, 46, biClrUsed, 0, 4);
            Array.Copy(input, 50, biClrImportant, 0, 4);

            hauteur = Convertir_Endian_To_Int(biHeight);
            largeur = Convertir_Endian_To_Int(biWidth); 
            tailleOffset = Convertir_Endian_To_Int(bfOffBits);

            
            FormerHeader();

            image = new byte[hauteur * largeur * 3];
            Array.Copy(input, 54, image, 0, hauteur * largeur * 3);

            this.matriceRVB = new Pixel[this.hauteur, this.largeur];

            int pointeur = 0;
            int bourrage = 0;
            if ((largeur * 3) % 4 != 0)
            {
                int c = largeur * 3;
                while (c % 4 != 0)
                {
                    c++;
                }
                bourrage = largeur * 3 - c;
            }
            for (int ligne = 0; ligne < matriceRVB.GetLength(0); ligne++)
            {
                for (int colonne = 0; colonne < matriceRVB.GetLength(1); colonne++)
                {

                    byte[] b = new byte[3];
                    Array.Copy(image, pointeur, b, 0, 3);
                    matriceRVB[ligne, colonne] = new Pixel(b);
                    pointeur += 3;
                }
                pointeur += bourrage;
            }
    }

        public int Convertir_Endian_To_Int(byte[] tab)
        {
            double retourner = 0;
            int cpt = 0;
            for (int i = 0; i < tab.Length; i++)
            {
                retourner += tab[i] * Math.Pow(256, cpt);
                cpt++;
            }
            return Convert.ToInt32(retourner);
        }

        public byte[] Convertir_Int_To_Endian(int value, int tailleTab)
        {
            byte[] tab = new byte[tailleTab];
            int quotient;
            for (int index = 0; index < tailleTab; index++)
            {
                tab[index] = (byte)(value % 256);
                quotient = (value / 256);
                value = quotient;
            }
            return tab;
        }

        public void FormerHeader()
        {
            header = new byte[0];
            header = Concat(header, bfType);
            header = Concat(header, bfSize);
            header = Concat(header, bfReserved1);
            header = Concat(header, bfReserved2);
            header = Concat(header, bfOffBits);
            header = Concat(header, biSize);
            header = Concat(header, biWidth);
            header = Concat(header, biHeight);
            header = Concat(header, biPlanes);
            header = Concat(header, biBitCount);
            header = Concat(header, biCompression);
            header = Concat(header, biSizeImage);
            header = Concat(header, biXpelsPerMeter);
            header = Concat(header, biYpelsPerMeter);
            header = Concat(header, biClrUsed);
            header = Concat(header, biClrImportant);
        }

        public byte[] Concat(byte[] first, byte[] second)
        {

            byte[] resul = new byte[first.Length + second.Length];
            first.CopyTo(resul, 0);
            second.CopyTo(resul, first.Length);
            return resul;
        }

        public void UpdateVariables()
        {
            hauteur = matriceRVB.GetLength(0);
            largeur = matriceRVB.GetLength(1);
            bfSize = Convertir_Int_To_Endian((hauteur * largeur * 3) + tailleOffset, 4);
            biSizeImage = Convertir_Int_To_Endian((hauteur * largeur * 3), 4);
            biWidth = Convertir_Int_To_Endian(largeur, 4);
            biHeight = Convertir_Int_To_Endian(hauteur, 4);

            FormerHeader();

            int bourrage = 0;
            if ((largeur * 3) % 4 != 0)
            {
                int c = largeur * 3;
                while (c % 4 != 0)
                {
                    c++;
                }
                bourrage = c - largeur * 3;
            }

            int index = 0;
            image = new byte[hauteur * largeur * 3];
            for (int i = 0; i < hauteur; i++)
            {
                for (int j = 0; j < largeur; j++)
                {
                    image[index] = matriceRVB[i, j].R;
                    image[index + 1] = matriceRVB[i, j].G;
                    image[index+ 2] = matriceRVB[i, j].B;
                    index += 3;
                }
                for (int m = 0; m < bourrage; m++)
                {
                    image[index] = 0;
                    image[index + 1] = 0;
                    image[index + 2] = 0;
                    index += 3;
                }
            }            
        }

        public void From_Image_To_File(string file)
        {
            UpdateVariables();
            
            byte[] données = Concat(header, image);
            File.WriteAllBytes(file, données);

        }

        //Traitement d'images:
        public void EffetMiroir()
        {
            Pixel[,] output = new Pixel[hauteur, largeur];
            for (int ligne = 0; ligne < hauteur; ligne++)
            {
                for (int colonne = 0; colonne < largeur; colonne++)
                {
                    output[ligne, colonne] = matriceRVB[ligne, largeur - 1 - colonne];
                    output[ligne, largeur - 1 - colonne] = matriceRVB[ligne, colonne];
                }
            }
            matriceRVB = output;
            UpdateVariables();
        } //Check
        public void Nuances_De_Gris()
        {
            for (int i = 0; i < matriceRVB.GetLength(0); i++)
            {
                for (int j = 0; j < matriceRVB.GetLength(1); j++)
                {
                    byte moyen = Convert.ToByte((matriceRVB[i, j].R + matriceRVB[i, j].B + matriceRVB[i, j].G) / 3);
                    matriceRVB[i, j].R = moyen;
                    matriceRVB[i, j].G = moyen;
                    matriceRVB[i, j].B = moyen;
                }
            }
            UpdateVariables();
        }
        public void Image_En_Noir_Et_Blanc()
        {
            for (int i = 0; i < matriceRVB.GetLength(0); i++)
            {
                for (int j = 0; j < matriceRVB.GetLength(1); j++)
                {
                    byte moyen = Convert.ToByte((matriceRVB[i, j].R + matriceRVB[i, j].B + matriceRVB[i, j].G) / 3);
                    if (moyen <= 127)
                    {
                        matriceRVB[i, j].R = 0;
                        matriceRVB[i, j].G = 0;
                        matriceRVB[i, j].B = 0;
                    }
                    else
                    {
                        matriceRVB[i, j].R = 255;
                        matriceRVB[i, j].G = 255;
                        matriceRVB[i, j].B = 255;
                    }
                }
            }
            UpdateVariables();
        }
        public void AgrandissementImage(int coef)
        {
            Pixel[,] matriceAgrandie = new Pixel[hauteur * coef, largeur * coef];
            for (int i = 0; i < hauteur; i++)
            {
                for (int j = 0; j < largeur; j++)
                {
                    for (int m = i * coef; m < coef * (i + 1); m++)
                    {
                        for (int n = j * coef; n < coef * (j + 1); n++)
                        {
                            matriceAgrandie[m, n] = matriceRVB[i, j];
                        }
                    }
                }
            }
            matriceRVB = matriceAgrandie;
            hauteur = hauteur * coef;
            largeur = largeur * coef;
            UpdateVariables();
        }

        /*public void RetrecissementImage(int coef)
        {
            Pixel[,] matriceRetrecie = new Pixel[hauteur / coef, largeur / coef];
            int m = 0;

            for (int i = 0; i < hauteur; i = i + coef)
            {
                int n = 0;
                for (int j = 0; j < largeur; j = j + coef)
                {
                    matriceRetrecie[m, n] = matriceRVB[i, j];
                    n++;
                }
                m++;
            }
            matriceRVB = matriceRetrecie;
            hauteur = hauteur / coef;
            largeur = largeur / coef;
            UpdateVariables();*/
        

        public void Convolution(int a)
        {

            Pixel[,] matrice = new Pixel[hauteur, largeur];
            int[,] kernel = new int[3, 3];
            int n = 1;
            if (a == 1) // Contour
            {
                n = 1;
                kernel = new int[,] { { -1, -1, -1 }, { -1, 8, -1 }, { -1, -1, -1 } };

            }
            else if (a == 2) //flou gaussien 3x3
            {
                kernel = new int[,] { { 1, 2, 1 }, { 2, 4, 2 }, { 1, 2, 1 } };
                n = 16;
            }
            else if (a == 3) //renforcement bord
            {
                kernel = new int[,] { { 0, 0, 0 }, { -1, 1, 0 }, { 0, 0, 0 } };
                n = 1;
            }
            else if (a == 4) //repoussage
            {
                kernel = new int[,] { { -2, -1, 0 }, { -1, 1, 1 }, { 0, 1, 2 } };
                n = 1;
            }

            for (int i = 0; i < hauteur; i++)
            {
                for (int j = 0; j < largeur; j++)
                {
                    int sommeR = ((matriceRVB[(i - 1 + hauteur) % hauteur, (j - 1 + largeur) % largeur].R) * kernel[0, 0] +
                        (matriceRVB[(i - 1 + hauteur) % hauteur, (j + largeur) % largeur].R) * kernel[0, 1] +
                        (matriceRVB[(i - 1 + hauteur) % hauteur, (j + 1 + largeur) % largeur].R) * kernel[0, 2] +
                        (matriceRVB[(i + hauteur) % hauteur, (j - 1 + largeur) % largeur].R) * kernel[1, 0] +
                        (matriceRVB[(i + hauteur) % hauteur, (j + largeur) % largeur].R) * kernel[1, 1] +
                        (matriceRVB[(i + hauteur) % hauteur, (j + 1 + largeur) % largeur].R) * kernel[1, 2] +
                        (matriceRVB[(i + 1 + hauteur) % hauteur, (j - 1 + largeur) % largeur].R) * kernel[2, 0] +
                        (matriceRVB[(i + 1 + hauteur) % hauteur, (j + largeur) % largeur].R) * kernel[2, 1] +
                        (matriceRVB[(i + 1 + hauteur) % hauteur, (j + 1 + largeur) % largeur].R) * kernel[2, 2]) / n;

                    int sommeG = ((matriceRVB[(i - 1 + hauteur) % hauteur, (j - 1 + largeur) % largeur].G) * kernel[0, 0] +
                        (matriceRVB[(i - 1 + hauteur) % hauteur, (j + largeur) % largeur].G) * kernel[0, 1] +
                        (matriceRVB[(i - 1 + hauteur) % hauteur, (j + 1 + largeur) % largeur].G) * kernel[0, 2] +
                        (matriceRVB[(i + hauteur) % hauteur, (j - 1 + largeur) % largeur].G) * kernel[1, 0] +
                        (matriceRVB[(i + hauteur) % hauteur, (j + largeur) % largeur].G) * kernel[1, 1] +
                        (matriceRVB[(i + hauteur) % hauteur, (j + 1 + largeur) % largeur].G) * kernel[1, 2] +
                        (matriceRVB[(i + 1 + hauteur) % hauteur, (j - 1 + largeur) % largeur].G) * kernel[2, 0] +
                        (matriceRVB[(i + 1 + hauteur) % hauteur, (j + largeur) % largeur].G) * kernel[2, 1] +
                        (matriceRVB[(i + 1 + hauteur) % hauteur, (j + 1 + largeur) % largeur].G) * kernel[2, 2]) / n;

                    int sommeB = ((matriceRVB[(i - 1 + hauteur) % hauteur, (j - 1 + largeur) % largeur].B) * kernel[0, 0] +
                        (matriceRVB[(i - 1 + hauteur) % hauteur, (j + largeur) % largeur].B) * kernel[0, 1] +
                        (matriceRVB[(i - 1 + hauteur) % hauteur, (j + 1 + largeur) % largeur].B) * kernel[0, 2] +
                        (matriceRVB[(i + hauteur) % hauteur, (j - 1 + largeur) % largeur].B) * kernel[1, 0] +
                        (matriceRVB[(i + hauteur) % hauteur, (j + largeur) % largeur].B) * kernel[1, 1] +
                        (matriceRVB[(i + hauteur) % hauteur, (j + 1 + largeur) % largeur].B) * kernel[1, 2] +
                        (matriceRVB[(i + 1 + hauteur) % hauteur, (j - 1 + largeur) % largeur].B) * kernel[2, 0] +
                        (matriceRVB[(i + 1 + hauteur) % hauteur, (j + largeur) % largeur].B) * kernel[2, 1] +
                        (matriceRVB[(i + 1 + hauteur) % hauteur, (j + 1 + largeur) % largeur].B) * kernel[2, 2]) / n;

                    byte r;
                    if (sommeR < 0) { sommeR = 0; } else if (sommeR > 255) { sommeR = 255; }

                    r = Convert.ToByte(sommeR);

                    byte g;
                    if (sommeG < 0) { sommeG = 0; } else if (sommeG > 255) { sommeG = 255; }
                    g = Convert.ToByte(sommeG);

                    byte b;
                    if (sommeB < 0) { sommeB = 0; } else if (sommeB > 255) { sommeB = 255; }
                    b = Convert.ToByte(sommeB);

                    matrice[i, j] = new Pixel(new byte[] { r, g, b });

                }
            }
            for (int i = 0; i < hauteur; i++)
            {
                for (int j = 0; j < largeur; j++)
                {
                    matriceRVB[i, j] = matrice[i, j];
                }
            }
        }


    }

    
}
