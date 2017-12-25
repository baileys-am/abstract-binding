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

            // Sync bindings
            IReadOnlyDictionary<string, IExampleObject> bindings = new Dictionary<string, IExampleObject>();
            StepExample("Press any key to sync bindings.", () =>
            {
                sender.SynchronizeBindings();
                bindings = sender.GetBindingsByType<IExampleObject>();
                Console.WriteLine($"Found {bindings.Count} {nameof(IExampleObject)}");
            });
            
            // Event subscribe
            StepExample("Press any key to subscribe to event.", () =>
            {
                
                Console.WriteLine($"Subscribing to {nameof(IExampleObject.NotifyRequested)}...");
                bindings.Values.First().NotifyRequested += _obj1_NotifyRequested;
            });

            // Event unsubscribe
            StepExample("Press any key to unsubscribe from event.", () =>
            {
                Console.WriteLine($"Unsubscribing from {nameof(IExampleObject.NotifyRequested)}...");
                bindings.Values.First().NotifyRequested -= _obj1_NotifyRequested;
            });

            // Property get
            StepExample("Press any key to get property value.", () =>
            {
                var value = bindings.Values.First().StrProperty;
                Console.WriteLine(value == null ? $"Looks like the value is 'null'. Let's set it!" : $"Value: {value}");
            });

            // Property set
            StepExample("Press any key to set property value.", () =>
            {
                string value = "You set a string!";
                bindings.Values.First().StrProperty = value;
                Console.WriteLine($"Value set to: {value}");
            });

            // Property get (again)
            StepExample("Press any key to get property value.", () =>
            {
                var value = bindings.Values.First().StrProperty;
                Console.WriteLine($"Look it's the same value you just set: {value}");
            });

            // Method invoke
            StepExample("Press any key to invoke a void return method.", () =>
            {
                bindings.Values.First().MethodVoidStr("Method invoked!");
                Console.WriteLine("You invoked the method!");
            });

            Console.WriteLine("Press any key to exit example.");
            Console.ReadKey();
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
