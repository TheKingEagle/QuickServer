using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CSHttp
{
    public class QuickServer
    {
        public string Host { get;private set; }
        public int Port { get; private set; }

        private readonly Dictionary<string, Action<HttpListenerContext>> Routes = new Dictionary<string, Action<HttpListenerContext>>();

        private readonly HttpListener Listener = new HttpListener();

        public QuickServer(string host, int port) 
        { 
            Host = host;
            Port = port;

        }

        public void Start() 
        {
            string baseUrl = $"http://{Host}:{Port}/";
            Console.WriteLine($"Listening for http requests on {baseUrl}");
            Listener.Prefixes.Add(baseUrl);
            Listener.Start();
            while (true)
            {
                try
                {
                    HttpListenerContext context = Listener.GetContext();
                    ThreadPool.QueueUserWorkItem((o) =>
                    {
                        var ctx = o as HttpListenerContext;
                        RouteRequest(ctx);
                    }, context);
                }
                catch (HttpListenerException ex)
                {
                    if (ex.ErrorCode != 995)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
                catch (ObjectDisposedException) { }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

        }

        public void Stop() => Listener.Abort();

        public void DefineRoute(string path, Action<HttpListenerContext> handler)
        {
            Routes[path] = handler;
        }

        private void RouteRequest(HttpListenerContext context)
        {
            string urlPath = context.Request.Url.LocalPath;

            if (Routes.TryGetValue(urlPath, out var handler))
            {
                handler(context);
            }
            else
            {
                SendResponse(context.Response, "404 - Not Found", HttpStatusCode.NotFound);
            }
        }

        public void SendResponse(HttpListenerResponse response,string contentType, byte[] content, HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            response.ContentType = contentType;
            response.StatusCode = (int)statusCode;
            response.OutputStream.Write(content, 0, content.Length);
            response.Close();
        }

        public void SendResponse(HttpListenerResponse response, string content, HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            response.ContentType = "text/html";
            response.StatusCode = (int)statusCode;
            byte[] c = Encoding.UTF8.GetBytes(content);
            response.OutputStream.Write(c, 0, c.Length);
            response.Close();
        }
    }
}
