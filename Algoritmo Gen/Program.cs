using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace Algoritmo_Gen
{
    class Program
    {
        static List<Individuo> salvar = new List<Individuo>();
        static void Main(string[] args)
        {
            List<Individuo> individuos = new List<Individuo>();
            if (File.Exists(@"C: \Users\USER\Desktop\Outputs.csv"))
            {
                File.Delete(@"C: \Users\USER\Desktop\Outputs.csv");
            }

            int maxGen = 15;
            int maxInd = 25;
            int maxSave = 2;
            int maxCrias = 8;
            int mutacion = 5;
            int maxPadres = 12;

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
                File.AppendAllText(@"C:\Users\USER\Desktop\Outputs.csv", individuos[individuos.Count - 1].x.ToString() + ",");
            }
            File.AppendAllText(@"C:\Users\USER\Desktop\Outputs.csv", "\n");

            for (int Gen = 2; Gen <= maxGen; Gen++)
            {
                List<Individuo> mejores = new List<Individuo>();
                Console.WriteLine("////////////////////// Mejores Individuos ////////////////////////");              
                foreach (Individuo x in mejores_individuos(individuos, maxSave, maxPadres))
                {
                    if(mejores.Count < maxPadres)
                    {
                        mejores.Add(x);
                        Console.WriteLine("Individuo: " + (individuos.IndexOf(x) + 1) + " Valor: " + x.x + " Binario: " + BinFormat(x.x));
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
                    File.AppendAllText(@"C:\Users\USER\Desktop\Outputs.csv", i.x.ToString() + ",");
                }
                File.AppendAllText(@"C:\Users\USER\Desktop\Outputs.csv", "\n");
            }
            
        }


        static List<Individuo> mejores_individuos(List<Individuo> individuos, int save = 2,int devolver = 4)
        {
            List<Individuo> aux = new List<Individuo>();
            List<int> xs = new List<int>();
            int[] besties = new int[devolver];
            foreach(Individuo p in individuos)
            {
                xs.Add(p.x);
            }

            xs.Sort();

            for(int i = xs.Count; i > xs.Count - devolver; i--)
            {
               besties[xs.Count - i] = xs[i-1];

                if (salvar.Count < save)
                {
                    salvar.Add(new Individuo()
                    {
                        x = xs[i - 1]
                    });
                }

            }

            foreach (int e in besties)
            {
                foreach(Individuo i in individuos)
                {
                    if (i.x == e && !aux.Contains(i) && aux.Count < devolver)
                    {
                        aux.Add(i);
                    }
                }
            }
            return aux;

        }

        static Individuo Generar_Individuo()
        {
            Random r = new Random();
            string binstring = "";
            for(int i = 0; i < 5; i++)
            {
                binstring = binstring + r.Next(0,2).ToString();
            }
            int aux = DecFormat(binstring);

            Individuo nuevo = new Individuo()
            {
                x =  aux 
            };
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
                        Console.ReadKey();
                    }
                    Out.Add(new Individuo()
                    {
                        x = DecFormat(Ind)
                    });
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

        static string BinFormat(int x, int length = 4)
        {
            string Out = "";
            for(int pot = 8; pot >= 0; pot--)
            {
                if (x >= Math.Pow(2, pot))
                {
                    x -= Convert.ToInt32(Math.Pow(2, pot));
                    Out = Out + "1";
                }
                else
                {
                    Out = Out + "0";
                }
            }

            int i = 0;
            foreach (char c in Out)
            {
                if(c == '0')
                {
                    i++;
                }
                else if(c == '1')
                {
                    break;
                }

                if(i == length)
                {
                    break;
                }
            }
            Out = Out.Substring(i);
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
    }



    public class Individuo
    {
        public int x { get; set; }
        public Individuo()
        {

        }

        public double funcion()
        {
            return Math.Pow(x, 2);
        }
    }
}
