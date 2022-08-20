using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace Binke.CommonUtility
{
    public static class LogError
    {
        public static void WriteErrorLog(string message, string controller, string action,string path)
        {
            string fileName = string.Format("{0}\\ErrorLog_{1}.txt", path, DateTime.Now.ToString("yyyyMMddHHmmssfff"));

            try
            {
                // Check if file already exists. If yes, delete it.     
                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                }

                // Create a new file     
                using (FileStream fs = File.Create(fileName))
                {
                    string body = string.Format("{0} - {1} - {2}", controller, action, message);
                    // Add some text to file    
                    byte[] author = new UTF8Encoding(true).GetBytes(body);
                    fs.Write(author, 0, author.Length);
                }
            }
            catch (Exception Ex)
            {
                Console.WriteLine(Ex.ToString());
            }
        }
    }
}
