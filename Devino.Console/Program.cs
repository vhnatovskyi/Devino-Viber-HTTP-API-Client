using Devino.API;
using System;

namespace Devino.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            SmsClient client = new SmsClient("sano_ua", "123456");

            System.Console.WriteLine(client.IsAuthorize);
            System.Console.WriteLine(client.GetBalance());
            System.Console.ReadKey();
        }
    }
}
