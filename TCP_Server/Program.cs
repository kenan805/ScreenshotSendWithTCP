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

        listener.Start(100);

        while (true)
        {
            Console.WriteLine("Listener...");
            var client = listener.AcceptTcpClient();
            Console.WriteLine("New client connected...(Write \"send\")");

            using (var stream = client.GetStream())
            {
                var bw = new BinaryWriter(stream);
                var br = new BinaryReader(stream);

                if (Console.ReadLine()?.ToLower() == "send")
                    bw.Write(await Task.Run(() => ScreenConsole()));
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
