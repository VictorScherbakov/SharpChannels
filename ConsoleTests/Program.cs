using System;
using System.Linq;

namespace ConsoleTests
{
    class Program
    {
        static void Main(string[] args)
        {
            var examples = new ExampleBase[]
            {
                new SimpleSendReceiveExample(Transport.Tcp),
                new RequestResponseExample(Transport.Tcp),
                new SimpleSendReceiveExample(Transport.Intradomain),
                new RequestResponseExample(Transport.Intradomain)
            };

            int i = 0;
            foreach (var example in examples)
            {
                Console.WriteLine("{0} - {1}", example.Transport + " " + example.GetType().Name, ++i);
            }

            ConsoleKeyInfo ch;
            var chars = new[] {'1', '2', '3', '4' };
            do
            {
                ch = Console.ReadKey();

            } while (!chars.Contains(ch.KeyChar));

            var index = int.Parse(ch.KeyChar.ToString()) - 1;
            
            Console.WriteLine();

            examples[index].Do();
        }
    }
}
