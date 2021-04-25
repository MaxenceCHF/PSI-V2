using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSI_V2
{
    class Program
    {
        static void Main(string[] args)
        {
            MyImage a = new MyImage("Images/coco.bmp");
            /*AfficherTableau(a.GetHeader);
            Console.WriteLine();
            Console.WriteLine();*/

            a.Image_En_Noir_Et_Blanc();
            a.From_Image_To_File("Images/test2.bmp");            
            Console.ReadKey();
        }

        static void AfficherTableau(byte[] tableau)
        {
            if (tableau == null || tableau.Length == 0)
            {
                Console.WriteLine("Tableau non valide");
            }
            else for (int index = 0; index <= tableau.Length - 1; index++)
                {
                    Console.Write(tableau[index] + (" "));

                }
        }
        static void AfficherMatrice(Pixel[,] matrice)
        {
            if (matrice == null)
            {
                Console.WriteLine("Matrice nulle");
            }
            else
                if (matrice.Length == 0)
            {
                Console.WriteLine("Matrice vide");
            }
            else
            {
                for (int ligne = 0; ligne < matrice.GetLength(0); ligne++)
                {
                    for (int colonne = 0; colonne < matrice.GetLength(1); colonne++)
                    {
                        AfficherTableau(matrice[ligne, colonne].GetTab);
                    }
                    Console.WriteLine();
                }
            }
        }
    }
}
