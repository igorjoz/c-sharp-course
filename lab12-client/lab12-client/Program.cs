using System;
using System.Net.Sockets;
using System.Text.Json;
using System.Text;
using System.Threading.Tasks;

namespace TcpClientApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                TcpClient client = new TcpClient("127.0.0.1", 9000);
                NetworkStream stream = client.GetStream();
                byte[] buffer = new byte[1024];
                int bytesRead;
                StringBuilder dataBuilder = new StringBuilder();

                while (true)
                {
                    try
                    {
                        // Send object to server
                        CustomObject obj = new CustomObject { Data = new Random().Next(1, 100) };
                        string json = JsonSerializer.Serialize(obj) + "\n";
                        buffer = Encoding.UTF8.GetBytes(json);
                        await stream.WriteAsync(buffer, 0, buffer.Length);
                        Console.WriteLine($"Sent: {obj.Data}");

                        // Receive modified object from server
                        bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                        dataBuilder.Append(Encoding.UTF8.GetString(buffer, 0, bytesRead));
                        string data = dataBuilder.ToString();

                        if (data.EndsWith("\n"))
                        {
                            json = data.TrimEnd('\n');
                            var receivedObject = JsonSerializer.Deserialize<CustomObject>(json);
                            Console.WriteLine($"Received modified: {receivedObject.Data}");

                            dataBuilder.Clear();

                            await Task.Delay(2000);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error: {ex.Message}");
                        break;
                    }
                }

                client.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Connection error: {ex.Message}");
            }
        }
    }

    public class CustomObject
    {
        public int Data { get; set; }
    }
}
