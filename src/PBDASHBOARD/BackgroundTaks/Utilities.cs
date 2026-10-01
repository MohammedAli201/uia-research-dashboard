using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PBDASHBOARD.BackgroundTaks
{
    
   public static class Utilities
    {
        /// <summary>
        /// logs the result of confirm method that checks if amount of pages are equal
        /// to the computed pages.
        /// </summary>
        /// <param name="confirm"></param>
        /// <param name="url"></param>
        /// <returns></returns>
       public static async Task LogToLogCofirm( bool confirm, string url,string relativeFilePath,
        int pages, int expectedPages )
        {
            await using var file = new StreamWriter(relativeFilePath,append:true);
            await file.WriteLineAsync(url + ":" + confirm + $"pages:{pages}, calculate:{expectedPages}");
        } 
        
        /// <summary>
        /// Gets the current year UTC.
        /// </summary>
        /// <returns>the current year</returns>
        public static string getDateTimeNow()
        {
           return DateTime.UtcNow.Year.ToString();
           
        }

        /// <summary>
        /// Logs into debug file
        /// </summary>
        /// <param name="count"></param>
        /// <param name="url"></param>
        /// <param name="RelativeFIlePath"></param>
        /// <returns></returns>
        public static async Task LogXtotalCount(int count, string url, string RelativeFIlePath)
        {
            await using var file = new StreamWriter(RelativeFIlePath,append:true);
            await file.WriteLineAsync( "Date: " + DateTime.Now +"," + url + ": total count =" + count);
        }
        /// <summary>
        /// Get the total amount of publication.
        /// </summary>
        /// <param name="headers"></param>
        /// <returns></returns>
        public static int getXCount(HttpHeaders headers)
        {
            IEnumerable<string> values;
            var xTotalCount = string.Empty;
            if (headers.TryGetValues("X-Total-Count", out values))
            {
                xTotalCount = values.FirstOrDefault(); // returns the total amount of the request
            }

            if (xTotalCount != null)
            {
                try
                {
                    var result = double.Parse(xTotalCount);
                    result = (int) Math.Ceiling(
                        result / 1000); // this is the number of pages the request maximum can have
                    Console.WriteLine(result);
                    if (result == 0)
                    {
                        // This is because 
                        result = 1;
                    }

                    return (int) result;
                }
                catch (FormatException)
                {
                    Console.WriteLine($"Unable to parse '{xTotalCount}'");
                }
            }


            Console.WriteLine("X-Total-Count");
            return 0;
        }
        /// <summary>
        /// delegate
        /// </summary>
        public delegate string DelUpdateDatePublished();

      
    }

}