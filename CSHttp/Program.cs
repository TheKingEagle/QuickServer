using CSHttp;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;

class Program
{
    private static readonly string Host = "localhost";
    private static readonly int Port = 8080;
    
    static void Main(string[] args)
    {
        QuickServer qs = new QuickServer(Host, Port);

        qs.DefineRoute("/", (context) =>
        {
            qs.SendResponse(context.Response, "Hello World!!");
            Console.WriteLine("Default route");
        });

        qs.DefineRoute("/about", (context) =>
        {
            qs.SendResponse(context.Response, "QuickServer demo v1.0 something");
            Console.WriteLine("about route");
        });

        qs.DefineRoute("/splash", (context) =>
        {
            qs.SendResponse(context.Response,"image/png",AppResources.appres.rmfbsft0);
            Console.WriteLine("splash route");
        });

        qs.DefineRoute("/err", (context) =>
        {
            qs.SendResponse(context.Response,"Fatal Error",HttpStatusCode.InternalServerError);
            Console.WriteLine("errrrrrr");
        });
        Task.Run(()=> qs.Start());
        Console.WriteLine("Entering console loop. 'exit' to stop.");
        while (true)
        {
            string inl = Console.ReadLine()!;

            if(inl.ToLower() == "exit")
            {
                break;
            }
        }
        qs.Stop();
        Console.WriteLine("Stopped, press a key to continue.");
        Console.ReadKey();
        
    }

    
}
