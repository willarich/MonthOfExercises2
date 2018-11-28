using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;

namespace day1
{
    class Program
    {
        static void Main(string[] args)
        {
            //console.writeline("hello world");

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(
            "https://www.google.com");
            request.Method = "GET";

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                string result = reader.ReadToEnd();
                Console.WriteLine(result);
            }

            var name = Console.ReadLine();

        }
    }
}
