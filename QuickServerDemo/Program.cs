
using AppResources;
using QuickServerDemo;
using RMSoftware.Http;
using System.Net;

class Program
{
    private static readonly string Host = "localhost";
    private static readonly int Port = 8080;

    private static PhotoWall pw = new PhotoWall();
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
            qs.SendResponse(context.Response,"image/png", appres.rmfbsft0);
            Console.WriteLine("splash route");
        });

        qs.DefineRoute("/upload", (context) =>
        {
            qs.SendResponse(context.Response, appres.submit);
            Console.WriteLine("show upload form");
        });

        qs.DefineRoute("/wall", (context) =>
        {
            qs.SendResponse(context.Response, appres.wall.Replace("<!--PHOTOWALL-->", pw.ToString()));
            Console.WriteLine("show wall");
        });
        qs.DefineRoute("/postimg", (context) =>
        {
            if (context.Request.HttpMethod != "POST")
            {
                qs.SendResponse(context.Response, "Method Not Allowed", HttpStatusCode.MethodNotAllowed);
                return;
            }

            var formData = qs.ParseFormData(context.Request);

            FileField? iamge = formData.Files.FirstOrDefault(x => x.Name == "image");

            FormField? author = formData.Fields.FirstOrDefault(x => x.Name == "author");
            FormField? description = formData.Fields.FirstOrDefault(x => x.Name == "description");

            if (iamge == null || author == null || description == null)
            {
                qs.SendResponse(context.Response, "Missing parameters", HttpStatusCode.BadRequest);

                return;
            }
            
            if (ImageValidation.IsImageValid(iamge.FileName,iamge.Data,out byte[] imageBytes, out string err))
            {
                string filename = $"uploads/{DateTime.Now:yyyyMMddHHmmssffff}.jpg";
                using (FileStream fs = File.Create(filename))
                {
                    fs.Write(imageBytes);
                    fs.Flush();
                }
                pw.AddPost(new PhotoWallPost() { Author = author.Value, Description = description.Value ,ImageUrl = filename});
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
        qs.DefineStaticFileRoute("/asset", "asset");
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
