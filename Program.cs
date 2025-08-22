using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Humanizer;

namespace Nezamikaj.Obrazovku
{
    class Program
    {
        static void Main(string[] args)
        {
            bool stop = false;
            Timer timer = null;
            Console.WriteLine("To Stop Press Escape or Q");

#if DEBUG
            //args = new string[] { "120" };
#endif

            int? secondsLeft = null;
            if (args != null && args.Count() > 0)
            {
                var s = args.FirstOrDefault();
                if (int.TryParse(s, out int sec))
                {
                    Console.WriteLine($"Timer set to {sec} seconds.");
                    var ts = new TimeSpan(0, 0, 0, sec);
                    secondsLeft = sec;
                    timer = new Timer((a) => 
                    {
                        timer.Dispose();
                        Environment.Exit(0);
                    }, null, ts, ts);
                }
            }

            UInt32 postX = 20;
            UInt32 postY = 20;
            UInt32 posMax = 500;
            //Console.WriteLine("To Stop Press Escape or Q");
            while (!stop)
            {
                for(UInt32 x = 0; x <= posMax; x++ )
                {

                    postX = x;
                    Random rand = new Random();
                    var delaySec = rand.Next(2, 20);
                    postY = (UInt32)delaySec;
                    Thread.Sleep(delaySec * 1000);
                    if (secondsLeft.HasValue)                    
                        secondsLeft = secondsLeft - delaySec;                    

                    Win32.mouse_event((int)(Win32.MouseEventFlags.MOVE), postX, postY, 0, 0);
                    Win32.mouse_event((int)(Win32.MouseEventFlags.LEFTDOWN), postX, postY, 0, 0);

                    if (timer != null)
                    {
                        //Console.WriteLine($"Timer will close app in {secondsLeft} seconds.");
                        Console.SetCursorPosition(0, Console.CursorTop);
                        string txt = TimeSpan.FromSeconds(secondsLeft.Value).Humanize(3);                        
                        Console.Write($"Timer will close app in {txt} seconds.");
                    }

                    if (Console.KeyAvailable)
                    {
                        ConsoleKeyInfo key = Console.ReadKey(true);
                        switch (key.Key)
                        {
                            case ConsoleKey.Q:
                            case ConsoleKey.Escape:
                                {
                                    Console.WriteLine("pressed key to stop");
                                    timer?.Dispose();
                                    stop = true;
                                    break;
                                }
                            default:
                                break;
                        }
                    }
                    if (stop)
                        break;
                }

            }


        }
    }
}
