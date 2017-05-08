using System;

namespace Payload
{
    class Program
    {
        private static object _locker = new object();

        private static void Log(string str)
        {
            lock (_locker)
            {
                Console.CursorLeft = 0;
                Console.Write(new string(' ', Console.WindowWidth - 1));
                Console.CursorLeft = 0;

                Console.Write(str);
            }
        }

        static void Main(string[] args)
        {
            var requestResponsePayload = new RequestResponsePayload();

            Console.WriteLine("Tcp request-response payload:");

            requestResponsePayload.Run(Transport.Tcp, 4, 100000, Log);

            Console.WriteLine();
            Console.WriteLine("Intradomain request-response payload:");

            requestResponsePayload.Run(Transport.Intradomain, 4, 100000, Log);

            Console.WriteLine();
            Console.WriteLine("Tcp publisher-subscriber payload:");

            var pubSubPayload = new PubSubPayload();

            pubSubPayload.Run(Transport.Tcp, 100, 10000, Log);

            Console.WriteLine();
            Console.WriteLine("Intradomain publisher-subscriber payload:");
            pubSubPayload.Run(Transport.Intradomain, 100, 10000, Log);

            Console.WriteLine();
            Console.WriteLine("Done");
            Console.ReadKey();
        }
    }
}
