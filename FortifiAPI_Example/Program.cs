using System;
using System.Threading.Tasks;
using FortifiAPI;

namespace FortifiAPI_Example
{
    class Program
    {
        static void Main(string[] args)
        {
            Task.Run(async () =>
            {
                try
                {
                    var fortifiClient = new Client("ORG:FORT:1739:nowuu", "U0lyZ3pyb2E2-CSA-T0RyUUNMQVZL", "TzRFbFNEYlFBOG5lbFNpb1pxSkp4QTRIR1JFc1dS");
                    
                    // Set client base URL to work with API on local machine
                    //fortifiClient.SetBaseURL("http://fortel.fortifi.me:9090/v1");

                    var response = await fortifiClient.GetBrandsAsync();

                    foreach (var item in response.Data.Brands1)
                    {
                        Console.WriteLine("Got brand " + item.BrandName);
                    }
                }
                catch (SwaggerException e)
                {
                    Console.WriteLine(e.Response);
                    throw e;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

            }).GetAwaiter().GetResult();
        }
    }
}
