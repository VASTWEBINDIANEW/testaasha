using System.IO;

namespace Vastwebmulti.Models
{
    public class JSONReadWrite
    {
        public JSONReadWrite() { }

        public string Read(string fileName, string location)
        {
            string CombinePath = Path.Combine(location, fileName);
            string path = System.Web.Hosting.HostingEnvironment.MapPath(CombinePath);

            string jsonResult;


            using (StreamReader streamReader = new StreamReader(path))
            {
                jsonResult = streamReader.ReadToEnd();
            }
            return jsonResult;
        }

        public void Write(string fileName, string location, string jSONString)
        {
            string CombinePath = Path.Combine(location, fileName);
            string path = System.Web.Hosting.HostingEnvironment.MapPath(CombinePath);

            using (var streamWriter = System.IO.File.CreateText(path))
            {
                streamWriter.Write(jSONString);
            }
        }
    }
}