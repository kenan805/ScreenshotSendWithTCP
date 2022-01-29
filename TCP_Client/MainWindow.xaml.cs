using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TCP_Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

        }

        public byte[] ReadAllBytes(BinaryReader reader)
        {
            const int bufferSize = 1024;
            using (var ms = new MemoryStream())
            {
                byte[] buffer = new byte[bufferSize];
                int count;
                while ((count = reader.Read(buffer, 0, buffer.Length)) != 0)
                    ms.Write(buffer, 0, count);
                return ms.ToArray();
            }

        }
        public static BitmapImage LoadImage(byte[] imageData)
        {
            var image = new BitmapImage();
            using (var mem = new MemoryStream(imageData))
            {
                mem.Position = 0;
                image.BeginInit();
                image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.StreamSource = mem;
                image.EndInit();

            }
            image.Freeze();
            return image;
        }
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var client = new TcpClient();
                client.Connect("127.0.0.1", 45678);
                using (var stream = client.GetStream())
                {
                    var bw = new BinaryWriter(stream);
                    var br = new BinaryReader(stream);

                    while (true)
                    {
                        var imageData = await Task.Run(() => ReadAllBytes(br));

                        imgFromServer.Source = LoadImage(imageData);

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Client error: " + ex.Message);
            }
        }

    }
}
