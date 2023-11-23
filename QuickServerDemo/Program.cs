
using AppResources;
using Microsoft.VisualBasic;
using QuickServerDemo;
using RMSoftware.Http;
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
            qs.SendResponse(context.Response, appres.index);
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

        qs.DefineRoute("/upload", (context) =>
        {
            qs.SendResponse(context.Response, appres.submit);
            Console.WriteLine("show upload form");
        });

        qs.DefineRoute("/postimg", (context) =>
        {
            if (context.Request.HttpMethod != "POST")
            {
                qs.SendResponse(context.Response, "Method Not Allowed", HttpStatusCode.MethodNotAllowed);
                return;
            }

            var formData = qs.ParseFormData(context.Request);

            FileField? f = formData.Files.FirstOrDefault(x => x.Name == "image");

            if (f == null)
            {
                qs.SendResponse(context.Response, "Missing image.", HttpStatusCode.BadRequest);

                return;
            }
            
            if (ImageValidation.IsImageValid(f.FileName,f.Data,out byte[] imageBytes, out string err))
            {
                string ts = DateTime.Now.ToString("yyyyMMddHHmmssffff");
                //copy
                using (FileStream fs = File.Create($"uploads/{ts}.jpg"))
                {
                    fs.Write(imageBytes);
                    fs.Flush();
                }
                qs.SendResponse(context.Response, "Uploaded!!", HttpStatusCode.OK);
            }
            else
            {
                qs.SendResponse(context.Response, "<h3>Invalid image.</h3>\n" + err, HttpStatusCode.BadRequest);

                return;
            }


        });

        qs.DefineRoute("/err", (context) =>
        {
            qs.SendResponse(context.Response,"Fatal Error",HttpStatusCode.InternalServerError);
            Console.WriteLine("errrrrrr");
        });

        qs.DefineStaticFileRoute("/uploads", "uploads");
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
