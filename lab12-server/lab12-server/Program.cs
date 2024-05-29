using System;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using System.Text;
using System.Threading.Tasks;

namespace TcpServer
{
    class Program
    {
        static async Task Main(string[] args)
        {
            TcpListener server = new TcpListener(IPAddress.Any, 9000);
            server.Start();

            Console.WriteLine("Server started on port 9000...");

            while (true)
            {
                TcpClient client = await server.AcceptTcpClientAsync();
                Console.WriteLine("Client connected!");

                _ = Task.Run(() => HandleClient(client));
            }
        }

        static async Task HandleClient(TcpClient client)
        {
            try
            {
                NetworkStream stream = client.GetStream();
                byte[] buffer = new byte[1024];
                int bytesRead;
                StringBuilder dataBuilder = new StringBuilder();

                while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) != 0)
                {
                    dataBuilder.Append(Encoding.UTF8.GetString(buffer, 0, bytesRead));
                    string data = dataBuilder.ToString();

                    if (data.EndsWith("\n"))
                    {
                        try
                        {
                            string json = data.TrimEnd('\n');
                            var receivedObject = JsonSerializer.Deserialize<CustomObject>(json);
                            Console.WriteLine($"Received: {receivedObject.Data}");

                            // process object
                            receivedObject.Data += 1;

                            // send modified object back to client
                            json = JsonSerializer.Serialize(receivedObject) + "\n";
                            buffer = Encoding.UTF8.GetBytes(json);
                            await stream.WriteAsync(buffer, 0, buffer.Length);
                            Console.WriteLine($"Sent: {receivedObject.Data}");

                            dataBuilder.Clear();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error processing data: {ex.Message}");
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Connection error: {ex.Message}");
            }
            finally
            {
                client.Close();
                Console.WriteLine("Client disconnected.");
            }
        }
    }

    public class CustomObject
    {
        public int Data { get; set; }
    }
}
