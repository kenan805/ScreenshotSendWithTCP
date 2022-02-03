// See https://aka.ms/new-console-template for more information

using System.Drawing;
using System.Drawing.Imaging;
using System.Net;
using System.Net.Sockets;

MainAsync(args).GetAwaiter().GetResult();

async Task MainAsync(string[] args)
{
    try
    {
        var ipAddress = IPAddress.Parse("127.0.0.1");
        var listener = new TcpListener(ipAddress, 45678);

        Console.WriteLine("Listener...");
        listener.Start(100);
        TcpClient client = null;
        List<TcpClient> clients = new List<TcpClient>();
        Task.Run(() =>
        {
            while (true)
            {
                var client = listener.AcceptTcpClient();
                clients.Add(client);
                Console.WriteLine("New client connected...(Write \"send\")");
            }
        });

        while (true)
        {
            if (Console.ReadLine()?.ToLower() == "send")
            {
                for (int i = 0; i < clients.Count; i++)
                {
                    if (clients[i].Connected)
                    {
                        using (var stream = clients[i].GetStream())
                        {
                            BinaryWriter binaryWriter = new BinaryWriter(stream);
                            binaryWriter.Write(await Task.Run(() => ScreenConsole()));
                        }
                    }
                }
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.Message);
    }
}



static byte[] ScreenConsole()
{

    Bitmap memoryImage = new Bitmap(1920, 1080);
    Size s = new Size(memoryImage.Width, memoryImage.Height);
    Graphics memoryGraphics = Graphics.FromImage(memoryImage);
    memoryGraphics.CopyFromScreen(0, 0, 0, 0, s);


    using (var ms = new MemoryStream())
    {
        memoryImage.Save(ms, ImageFormat.Png);
        Console.WriteLine("Screen oldu");
        return ms.ToArray();
    }

}
