using System;
using System.Collections.Generic;
using System.IO;

namespace Algoritmo_Gen
{
    class Program
    {
        static List<Individuo> salvar = new List<Individuo>();
        static int numerox;
        static string ruta_output = @"C:\Users\USER\Desktop\Outputs.csv";
        static void Main(string[] args)
        {
            List<Individuo> individuos = new List<Individuo>();
            if (File.Exists(ruta_output))
            {
                File.Delete(ruta_output);
            }


            Console.WriteLine("Escribe un numero entre 1 y 15 para elevar al cuadrado");
            numerox = Convert.ToInt32(Console.ReadLine());
            Console.Clear();

            int maxGen = 50; //Cuantas generaciones
            int maxInd = 30; //Individuos por generación
            int maxSave = 2; //Cuantos salvas (no puede ser mayor a maxpadres)
            int maxCrias = 16; //cuantos hijos
            int mutacion = 10; //probabilidad de mutación
            int maxPadres = 20; //Cuantos padres

            if (maxPadres % 2 != 0)
            {
                Console.WriteLine("Número de Padres no valido se disminuira en uno");
                maxPadres--;
                Console.WriteLine("");
            }
            if((maxPadres/2)*maxCrias > maxInd)
            {
                double nuevo = maxPadres/2;
                maxCrias = Convert.ToInt32(Math.Floor((maxInd - 2)/nuevo));
                Console.WriteLine("Número de crias no valido se cambiara a: " + maxCrias);
                Console.WriteLine("Número total de crias: " + (maxCrias*(maxPadres/2)));
            }

            Console.WriteLine("/////////////////////////////////Generación: 1 /////////////////////////////////////");
            for (int i  = 0; i < maxInd; i++)
            {
                individuos.Add(Generar_Individuo());
                Console.WriteLine("Individuo: " + individuos.Count + " Valor: " + individuos[individuos.Count - 1].x);
                File.AppendAllText(ruta_output, individuos[individuos.Count - 1].x.ToString() + ",");
            }
            File.AppendAllText(ruta_output, "\n");

            for (int Gen = 2; Gen <= maxGen; Gen++)
            {
                List<Individuo> mejores = new List<Individuo>();
                Console.WriteLine("////////////////////// Mejores Individuos ////////////////////////");              
                foreach (Individuo x in mejores_individuos(individuos, maxSave, maxPadres))
                {
                    if(mejores.Count < maxPadres)
                    {
                        mejores.Add(x);
                        Console.WriteLine("Individuo: " + (individuos.IndexOf(x) + 1) + " Puntaje: " + x.puntaje + " Valor: " + x.x + " Binario: " + BinFormat(x.x));
                    }
                }

                individuos.Clear();
                individuos = Crias(mejores,mutacion, maxCrias);
                foreach(Individuo i in individuos)
                {
                    Console.Write(" " + i.x);
                }
                Console.WriteLine("");

                foreach(Individuo i in salvar)
                {
                    individuos.Add(i);
                    Console.WriteLine("Salvado: " + i.x);
                }
                salvar.Clear();

                if (individuos.Count < maxInd)
               {
                    int indexer = individuos.Count;
                    for (int i = 0; i < maxInd - indexer; i++)
                    {
                        individuos.Add(Generar_Individuo());
                    }
               }

                Console.WriteLine("");
                Console.WriteLine("/////////////////////////////////Generación: " + Gen + " /////////////////////////////////////");
                foreach (Individuo i in individuos)
                {
                    Console.WriteLine("Individuo: " + (individuos.IndexOf(i)+1) + " Valor: " + i.x);
                    File.AppendAllText(ruta_output, i.x.ToString() + ",");
                }
                File.AppendAllText(ruta_output, "\n");
            }

            Console.WriteLine("");
            Console.WriteLine("Respuesta: " + mejores_individuos(individuos, maxSave, maxPadres)[0].x);

        }
        static List<Individuo> mejores_individuos(List<Individuo> individuos, int save = 2, int padres = 4)
        {
            List<double> puntajes = new List<double>();
            List<Individuo> mejores = new List<Individuo>();
            foreach(Individuo i in individuos)
            {
                puntajes.Add(i.puntaje);
            };
            puntajes.Sort();

            for (int y = individuos.Count; y > individuos.Count - padres; y--)
            {
                foreach(Individuo i in individuos)
                {
                    if(i.puntaje == puntajes[y - 1] && !mejores.Contains(i))
                    {
                        mejores.Add(i);
                        break;
                    }
                }
            }

            for(int y = 0; y < save; y++)
            {
                salvar.Add(mejores[y]);
            }

            return mejores;
        }
        static Individuo Generar_Individuo()
        {
            Random r = new Random();
            string binstring = "";
            for(int i = 0; i < 8; i++)
            {
                binstring = binstring + r.Next(0,2).ToString();
            }
            int aux = DecFormat(binstring);

            Individuo nuevo = new Individuo()
            {
                x = aux,
            };
            nuevo.puntaje = Funcion(numerox, nuevo.x);
            return nuevo;
        }
        static List<Individuo> Crias(List<Individuo> padres,int MutRatio = 5, int CriasPer_Ind = 2)
        {

            List<Individuo> Out = new List<Individuo>();
            string bin1;
            string bin2;
            string Ind ="";
            for (int i = 0; i < padres.Count; i +=2)
            {
                bin1 = BinFormat(padres[i].x);
                bin2 = BinFormat(padres[i+1].x);
                Console.WriteLine("Padres: " + i + " (" + bin1 + ") y " + (i+1) +" (" + bin2 + ")");
                Random r = new Random();
                Random rand = new Random(250);
                for (int cri = 0; cri < CriasPer_Ind; cri++)
                {
                    int r2d2 = r.Next(1, bin1.Length);
                    Console.WriteLine("cromosomas a tomar: " + r2d2);
                    Console.Write("Cria: ");
                    if ((cri+1) % 2 > 0)
                    {
                        Ind = bin1.Substring(0, r2d2) + bin2.Substring(r2d2);
                    }
                    else
                    {
                        Ind = bin2.Substring(0, r2d2) + bin1.Substring(r2d2);
                    }

                    if (rand.Next(1, 101) <= MutRatio)
                    {
                        Ind = Mutar(Ind);
                        Console.WriteLine("mutado ///// Bin: " + Ind);
                    }
                    Out.Add(new Individuo()
                    {
                        x = DecFormat(Ind)
                    });
                    Out[Out.Count - 1].puntaje = Funcion(numerox, Out[Out.Count - 1].x);
                    Console.Write(DecFormat(Ind) +" (" + Ind + ") \n");
                }
                Console.WriteLine("");
            }
            return Out;
        }
        static string Mutar(string ind)
        {
            string individuo = "";
            Random r = new Random();
            int index = r.Next(1, ind.Length);
            individuo = ind.Substring(0,index - 1);
            individuo = ind.Substring(index, 1) == "0" ? individuo + "1" : individuo + "0";
            individuo = individuo + ind.Substring(index);
            return individuo;
        }
        static string BinFormat(int x)
        {
            string Out = "";
            for(int y = 7; y > -1; y--)
            {
                if(x >= Math.Pow(2, y))
                {
                    Out = Out + "1";
                    x = Convert.ToInt32(x - Math.Pow(2, y));
                }
                else
                {
                    Out = Out + "0";
                }
            }

            return Out;

        }
        static int DecFormat(string x)
        {
            int aux = 0;
            for(int i = 0; i < x.Length; i++)
            {
                aux += x.Substring(i, 1) == "0" ? 0 : Convert.ToInt32(Math.Pow(2,(x.Length-1)-i));
            }
            return aux;
        }
        static double Funcion(int x, int indx)
        { 
            double aux = Math.Pow(x, 2) - indx;
            return 1 - Math.Abs(aux)/100;
        }
    }
    public class Individuo
    {
        public int x { get; set; }
        public double puntaje { get; set; }
        public Individuo()
        {

        }
    }
}
