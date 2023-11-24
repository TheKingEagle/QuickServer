using AppResources;
using Newtonsoft.Json;
using System.Diagnostics.Metrics;

namespace QuickServerDemo
{
    internal class PhotoWall
    {
        internal List<PhotoWallPost> posts;

        internal PhotoWall()
        {
            if (!File.Exists("wall.json"))
            {
                posts = new List<PhotoWallPost>();
                using (StreamWriter sw = new StreamWriter("wall.json"))
                {
                    sw.Write(JsonConvert.SerializeObject(posts));
                    sw.Flush();
                }
            }
            else
            {
                using (StreamReader sr = new StreamReader("wall.json"))
                {
                    posts = JsonConvert.DeserializeObject<List<PhotoWallPost>>(sr.ReadToEnd());

                    if(posts == null)
                    {
                        posts = new List<PhotoWallPost>();
                    }
                }
            }
        }

        public override string ToString()
        {
            string mk = "";

            foreach (var item in posts)
            {
                mk += appres.imgmk.Replace("$AUTH", item.Author).Replace("$DESC", item.Description).Replace("$URL", item.ImageUrl)+"\r\n";
            }

            return mk;
        }

        internal void AddPost(PhotoWallPost post)
        {
            posts.Add(post);
            //save
            using (StreamWriter sw = new StreamWriter("wall.json"))
            {
                sw.Write(JsonConvert.SerializeObject(posts));
                sw.Flush();
            }

        }
    }
}
