using System;
using System.IO;
using System.Net.Sockets;
using System.Text.Json;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("JSON TCP Client:");

        using (TcpClient client = new TcpClient("127.0.0.1", 5001)) // Samme port som serveren
        {
            NetworkStream ns = client.GetStream();
            StreamReader reader = new StreamReader(ns);
            StreamWriter writer = new StreamWriter(ns) { AutoFlush = true };

            Console.WriteLine("Enter method (random, add, subtract):");
            string method = Console.ReadLine() ?? "";

            Console.WriteLine("Enter the first number:");
            int number1 = int.Parse(Console.ReadLine() ?? "0");

            Console.WriteLine("Enter the second number:");
            int number2 = int.Parse(Console.ReadLine() ?? "0");

            var request = new Request
            {
                Method = method,
                Number1 = number1,
                Number2 = number2
            };

            string jsonRequest = JsonSerializer.Serialize(request);
            await writer.WriteLineAsync(jsonRequest);

            string? response = await reader.ReadLineAsync();
            if (response != null)
            {
                var jsonResponse = JsonSerializer.Deserialize<Response>(response);
                if (jsonResponse != null)
                {
                    Console.WriteLine("Server response:");
                    if (jsonResponse.Status == "success")
                    {
                        Console.WriteLine($"Result: {jsonResponse.Result}");
                    }
                    else
                    {
                        Console.WriteLine($"Error: {jsonResponse.Message}");
                    }
                }
            }
        }
    }

    class Request
    {
        public string Method { get; set; } = "";
        public int Number1 { get; set; }
        public int Number2 { get; set; }
    }

    class Response
    {
        public string Status { get; set; } = "";
        public object? Result { get; set; }
        public string? Message { get; set; }
    }
}
