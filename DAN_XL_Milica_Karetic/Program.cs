﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace DAN_XL_Milica_Karetic
{
    class Program
    {
        /// <summary>
        /// Format array
        /// </summary>
        public static string[] formats = new string[2] { "A3", "A4" };
        /// <summary>
        /// List of PCs
        /// </summary>
        public static List<Thread> threads = new List<Thread>();
        /// <summary>
        /// Random object
        /// </summary>
        public static Random rnd = new Random();
        /// <summary>
        /// File name for colors file
        /// </summary>
        static string fileName = @"..\..\Palete.txt";

        /// <summary>
        /// Event that signals that all PCs are done printing
        /// </summary>
        static CountdownEvent countdown = new CountdownEvent(10);

        /// <summary>
        /// A3 format printer
        /// </summary>
        static SemaphoreSlim semaphoreA3 = new SemaphoreSlim(1);
        /// <summary>
        /// A4 format printer
        /// </summary>
        static SemaphoreSlim semaphoreA4 = new SemaphoreSlim(1);

        /// <summary>
        /// locker for A3 printer
        /// </summary>
        static readonly object lockerA3 = new object();
        /// <summary>
        /// locker for A4 printer
        /// </summary>
        static readonly object lockerA4 = new object();


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
            lock (lockerA3)
            {
                semaphoreA3.Wait();

                Console.WriteLine("\nPrinting A3 document for " + Thread.CurrentThread.Name);
                //printing for 1000ms
                Thread.Sleep(1000);
                
                Console.WriteLine("\n\n=====> Printing is complete. User of " + Thread.CurrentThread.Name + " can come for document in A3 format\n\n");
                
                semaphoreA3.Release();
                //singal that one PC is done printing
                countdown.Signal();
            }
        }

        /// <summary>
        /// A4 format printer
        /// </summary>
        public static void PrintA4()
        {

            lock (lockerA4)
            {
                semaphoreA4.Wait();

                Console.WriteLine("\nPrinting A4 document for " + Thread.CurrentThread.Name);
                Thread.Sleep(1000);
                
                Console.WriteLine("\n\n=====> Printing is complete. User of " + Thread.CurrentThread.Name + " can come for document in A4 format\n\n");
                
                semaphoreA4.Release();
                countdown.Signal();
            }
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

        /// <summary>
        /// Printing method
        /// </summary>
        public static void Print()
        {
            do
            {
                Thread.Sleep(100);

                string currentName = Thread.CurrentThread.Name;
                //get colors from file
                List<string> colors = getColors();
                //random color
                string color = GetRandomColor(colors);

                string format = "";
                string orientation = "";

                //random format for printing
                int randomFormat = rnd.Next(0, 2);
                if (randomFormat == 0)
                    format = "A3";
                else
                    format = "A4";

                //random orientation
                int randomOrientation = rnd.Next(0, 2);
                if (randomOrientation == 0)
                    orientation = "portrait";
                else
                    orientation = "landscape";

                //send request
                Console.WriteLine(currentName + " sent request for printing document in " + format + " format. Color: " + color + ". Orientation: " + orientation);

                //if format is A3 send request to A3 printer
                if (format == "A3")
                {
                    //if printer is free
                    if (semaphoreA3.CurrentCount > 0)
                        PrintA3();
                    //if printer is busy try again
                    else
                    {
                        Thread t = new Thread(Print);
                        t.Name = Thread.CurrentThread.Name;
                        t.Start();
                    }                       
                }
                //if format is A4 send request to A4 printer
                else if (format == "A4")
                {
                    //if printer is free
                    if (semaphoreA4.CurrentCount > 0)
                        PrintA4();
                    //if printer is busy try again
                    else
                    {
                        Thread t = new Thread(Print);
                        t.Name = Thread.CurrentThread.Name;
                        t.Start();
                    }
                        
                }               
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
