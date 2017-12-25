using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractBinding.Examples.App
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new SenderClient("127.0.0.1:6789");
            var serializer = new Serializer();
            client.StartListener();

            var sender = new Sender(client, serializer);
            sender.Register<IExampleObject>();

            StepExample("Press any key to sync bindings.", sender.SynchronizeBindings);
            var bindings = sender.GetBindingsByType<IExampleObject>();
            Console.WriteLine($"Found {bindings.Count} {nameof(IExampleObject)}");
            Console.WriteLine($"Subscribing to {nameof(IExampleObject.NotifyRequested)}...");
            bindings.Values.First().NotifyRequested += _obj1_NotifyRequested;

            StepExample("Press any key to set property value.", () => { bindings.Values.First().StrProperty = "You set a string!"; });
            StepExample("Press any key to invoke method.", () => { bindings.Values.First().MethodVoidStr("Method invoked!"); });

            StepExample("Press any key to exit.", () =>
            {
                Console.WriteLine($"Unsubscribing from {nameof(IExampleObject.NotifyRequested)}...");
                bindings.Values.First().NotifyRequested -= _obj1_NotifyRequested;
            });
        }

        static void StepExample(string instr, Action action)
        {
            Console.WriteLine("-----Start of Example Step-----");
            Console.WriteLine(instr);
            Console.ReadKey();
            try
            {
                Stopwatch stopwatch = Stopwatch.StartNew();
                action();
                stopwatch.Stop();
                Console.WriteLine($"Time elapsed: {stopwatch.ElapsedMilliseconds}");
                Console.WriteLine("-----End of Example Step-----");
                Console.WriteLine();
            }
            catch (AggregateException ex)
            {
                Console.WriteLine($"Exception caught: {ex.InnerException}");
                Console.ReadKey();
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception caught: {ex.Message}");
                Console.ReadKey();
                throw;
            }
        }

        private static void _obj1_NotifyRequested(object sender, EventArgs e)
        {
            Console.WriteLine($"obj1 event: {nameof(IExampleObject.NotifyRequested)}");
        }
    }
}
