using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DAN_XL_Milica_Karetic
{
    class Program
    {
        public static string[] formats = new string[2] { "A3", "A4" };
        public static List<Thread> threads = new List<Thread>();
        public static Random rnd = new Random();
        static string fileName = @"..\..\Palete.txt";

        static AutoResetEvent eventA3 = new AutoResetEvent(false);
        static AutoResetEvent eventA4 = new AutoResetEvent(false);

        static CountdownEvent countdown = new CountdownEvent(10);
        static readonly object locker = new object();

        static SemaphoreSlim semaphoreA3 = new SemaphoreSlim(1);
        static SemaphoreSlim semaphoreA4 = new SemaphoreSlim(1);


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

        public static void PrintA3()
        {
            //lock(locker)
            //{
                //Console.WriteLine("Printing A3 for " + Thread.CurrentThread.Name);
                //Thread.Sleep(1000);
                //eventA3.Set();
                //Console.WriteLine("Printing is complete. User of " + Thread.CurrentThread.Name + " can come for document in A3 format");

                ////barrier.SignalAndWait();
                //countdown.Signal();

                semaphoreA3.Wait();

                Console.WriteLine("Printing A3 for " + Thread.CurrentThread.Name);
                Thread.Sleep(1000);

                Console.WriteLine("Printing is complete. User of " + Thread.CurrentThread.Name + " can come for document in A3 format");

                semaphoreA3.Release();
                countdown.Signal();
            //}           
        }

        public static void PrintA4()
        {
            //lock(locker)
            // {
            //     Console.WriteLine("Printing A4 for " + Thread.CurrentThread.Name);
            //     Thread.Sleep(1000);
            //     eventA4.Set();
            //     Console.WriteLine("Printing is complete. User of " + Thread.CurrentThread.Name + " can come for document in A4 format");

            //     // barrier.SignalAndWait();
            //     countdown.Signal();
            // }
            semaphoreA4.Wait();

            Console.WriteLine("Printing A4 for " + Thread.CurrentThread.Name);
            Thread.Sleep(1000);

            Console.WriteLine("Printing is complete. User of " + Thread.CurrentThread.Name + " can come for document in A4 format");

            semaphoreA4.Release();
            countdown.Signal();
        }

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
                    //eventA3.WaitOne();
                    // countdown.Wait();

                }
                else if (format == "A4")
                {
                    PrintA4();

                    //eventA4.WaitOne();
                    // countdown.Wait();
                }
                Thread.Sleep(100);
                //eventA3.WaitOne();
                //eventA4.WaitOne();

                countdown.Wait();

            } while (countdown.CurrentCount != 0);

         
            
        }
        static void Main(string[] args)
        {
            Thread t1 = new Thread(new ThreadStart(WriteColor))
            {
                Name = "WriteColor"
            };
            t1.Start();
            t1.Join();

            for (int i = 0; i < 10; i++)
            {
                Thread t = new Thread(new ThreadStart(Print))
                {
                    Name = string.Format("Computer_{0}", i+1)
                };
                threads.Add(t);
            }
            for (int i = 0; i < threads.Count; i++)
            {
                threads[i].Start();
            }
            for (int i = 0; i < threads.Count; i++)
            {
                threads[i].Join();
            }
           
            
            Console.ReadKey();
        }
    }
}
