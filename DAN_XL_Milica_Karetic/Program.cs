using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace DAN_XL_Milica_Karetic
{
    class Program
    {
        public static string[] formats = new string[2] { "A3", "A4" };
        public static List<Thread> threads = new List<Thread>();
        public static Random rnd = new Random();
        static string fileName = @"..\..\Palete.txt";

        static CountdownEvent countdown = new CountdownEvent(10);

        static SemaphoreSlim semaphoreA3 = new SemaphoreSlim(1);
        static SemaphoreSlim semaphoreA4 = new SemaphoreSlim(1);

        /// <summary>
        /// Write colors to file
        /// </summary>
        public static void WriteColor()
        {
            string[] colors = new string[] { "red", "black", "green", "white", "yellow" };
            
            StreamWriter sw = new StreamWriter(fileName);
            using (sw)
            {
                for (int i = 0; i < colors.Length; i++)
                {
                    sw.WriteLine(colors[i]);
                }
            }
        }

        /// <summary>
        /// A3 format printer
        /// </summary>
        public static void PrintA3()
        {
            semaphoreA3.Wait();

            Console.WriteLine("\nPrinting A3 for " + Thread.CurrentThread.Name);
            Thread.Sleep(1000);

            Console.WriteLine("\nPrinting is complete. User of " + Thread.CurrentThread.Name + " can come for document in A3 format");

            semaphoreA3.Release();
            countdown.Signal();     
        }

        /// <summary>
        /// A4 format printer
        /// </summary>
        public static void PrintA4()
        {
            semaphoreA4.Wait();

            Console.WriteLine("\nPrinting A4 for " + Thread.CurrentThread.Name);
            Thread.Sleep(1000);

            Console.WriteLine("\nPrinting is complete. User of " + Thread.CurrentThread.Name + " can come for document in A4 format");

            semaphoreA4.Release();
            countdown.Signal();
        }

        /// <summary>
        /// Get colors from file
        /// </summary>
        /// <returns>Color list</returns>
        public static List<string> getColors()
        {
            StreamReader sr = new StreamReader(fileName);
            List<string> colors = new List<string>();
            using (sr)
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    colors.Add(line);
                }
            }

            return colors;
        }

        /// <summary>
        /// Get random color from color list
        /// </summary>
        /// <param name="colors">Color list</param>
        /// <returns>Random color from list</returns>
        public static string GetRandomColor(List<string> colors)
        {
            int num = rnd.Next(0, colors.Count);
            return colors[num];
        }


        public static void Print()
        {
            do
            {
                string currentName = Thread.CurrentThread.Name;
                List<string> colors = getColors();
                string color = GetRandomColor(colors);

                string format = "";
                string orientation = "";

                int randomFormat = rnd.Next(0, 2);
                if (randomFormat == 0)
                    format = "A3";
                else
                    format = "A4";

                int randomOrientation = rnd.Next(0, 2);
                if (randomOrientation == 0)
                    orientation = "portrait";
                else
                    orientation = "landscape";

                Console.WriteLine(currentName + " sent request for printing document in " + format + " format. Color: " + color + ". Orientation: " + orientation);

                if (format == "A3")
                {
                    PrintA3();
                }
                else if (format == "A4")
                {
                    PrintA4();
                }
                Thread.Sleep(100);

                countdown.Wait();

            } while (countdown.CurrentCount != 0);
         
            
        }
        static void Main(string[] args)
        {
            //thread for writing colors into file
            Thread t1 = new Thread(new ThreadStart(WriteColor))
            {
                Name = "WriteColor"
            };
            t1.Start();
            t1.Join();

            //10 PCs
            for (int i = 0; i < 10; i++)
            {
                Thread t = new Thread(new ThreadStart(Print))
                {
                    Name = string.Format("PC_{0}", i+1)
                };
                threads.Add(t);
            }
            //start PCs
            for (int i = 0; i < threads.Count; i++)
            {
                threads[i].Start();
            }
            //join them
            for (int i = 0; i < threads.Count; i++)
            {
                threads[i].Join();
            }
            Console.WriteLine("\nThey're all done printing");
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
