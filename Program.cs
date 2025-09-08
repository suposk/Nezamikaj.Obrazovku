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
        // Change 'async void Main' to 'static async Task Main' and make MoveMouseLoopAsync static

        static async Task Main(string[] args)
        {
            bool stop = false;
            Timer timer = null;
            Console.WriteLine("To Stop Press Escape or Q");

#if DEBUG
            //args = new string[] { "5" };
#endif

            int? secondsLeft = null;
            if (args != null && args.Count() > 0)
            {
                var s = args.FirstOrDefault();
                if (int.TryParse(s, out int min))
                {
                    Console.WriteLine($"Timer set to {min} minutes.");
                    //var ts = new TimeSpan(0, 0, 0, sec);
                    var ts = new TimeSpan(0, 0, min, 0);
                    secondsLeft = min * 60;
                    timer = new Timer((a) =>
                    {
                        timer.Dispose();
                        Environment.Exit(0);
                    }, null, ts, ts);
                }
            }

            var cts = new CancellationTokenSource();            
            UInt32 postX = 20;
            Task moveTask = MoveMouseLoopAsync(postX, secondsLeft, timer, cts.Token);
            //Console.WriteLine("To Stop Press Escape or Q");
            while (!stop)
            {
                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo key = Console.ReadKey(true);
                    switch (key.Key)
                    {
                        case ConsoleKey.Q:
                        case ConsoleKey.Escape:
                            {
                                //Console.WriteLine("--------------");
                                Console.WriteLine();
                                Console.WriteLine($"Pressed key {key.Key} to stop");
                                timer?.Dispose();
                                stop = true;
                                break;
                            }
                        default:
                            break;
                    }
                }
            }
        }

        static async Task MoveMouseLoopAsync(UInt32 max, int? secondsLeft, Timer timer, CancellationToken token)
        {
            UInt32 postX = max;
            UInt32 postY = max;
            while (!token.IsCancellationRequested)
            {
                for (UInt32 x = 0; x <= max; x++)
                {
                    postX = x;
                    Random rand = new Random();
                    var delaySec = rand.Next(2, 20);
                    postY = (UInt32)delaySec;
                    await Task.Delay(delaySec * 1000, token);
                    if (secondsLeft.HasValue)
                        secondsLeft = secondsLeft - delaySec;

                    Win32.mouse_event((int)(Win32.MouseEventFlags.MOVE), postX, postY, 0, 0);
                    Win32.mouse_event((int)(Win32.MouseEventFlags.LEFTDOWN), postX, postY, 0, 0);

                    if (timer != null && secondsLeft.HasValue)
                    {
                        Console.SetCursorPosition(0, Console.CursorTop);
                        string txt = TimeSpan.FromSeconds(secondsLeft.Value).Humanize(3);
                        Console.Write($"Timer will close app in {txt}.");
                    }
                }
            }
        }
    }
}
