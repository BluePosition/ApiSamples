using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace BluePosition.Samples.LoginApi
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Please enter the Ocp Api Management Subscription Key (go to developer.cloud.mobileservices.dk to get a subscription key):");
            var ocpApimSubscriptionKey = Console.ReadLine();
            Console.WriteLine("Please enter your Scale user name in the format user@vkxxxxxx.hvoip.dk:");
            var username = Console.ReadLine();
            Console.WriteLine("Password:");
            var password = Console.ReadLine();

            var loginTask = new LoginTask(ocpApimSubscriptionKey);
            var token = loginTask.Execute(username, password).Result;
            Console.WriteLine($"Logged in as {username} with token: {token}");
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }
    }
}
