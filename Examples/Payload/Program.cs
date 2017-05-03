using System;

namespace Payload
{
    class Program
    {
        private static void Log(string str)
        {
            Console.CursorLeft = 0;
            Console.Write(str);
        }

        static void Main(string[] args)
        {
            var payload = new RequestResponsePayload();

            Console.WriteLine("Tcp request-response payload:");

            payload.Run(Transport.Tcp, 4, 100000, Log);

            Console.WriteLine();
            Console.WriteLine("Intradomain request-response payload:");

            payload.Run(Transport.Intradomain, 4, 100000, Log);

            Console.WriteLine();
            Console.WriteLine("Done");
            Console.ReadKey();
        }
    }
}
