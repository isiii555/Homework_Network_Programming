using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
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
using System.Windows.Threading;

namespace Screenshot_App
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public BitmapImage PhotoSource
        {
            get { return (BitmapImage)GetValue(PhotoSourceProperty); }
            set { SetValue(PhotoSourceProperty, value); }
        }

        public Dispatcher Dispatcher { get; set; } = Dispatcher.CurrentDispatcher;

        // Using a DependencyProperty as the backing store for PhotoSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PhotoSourceProperty =
            DependencyProperty.Register("PhotoSource", typeof(BitmapImage), typeof(MainWindow));
        public IPAddress IpAdress { get; set; }

        public IPEndPoint IpAdressEndPoint { get; set; }

        public TcpClient Client { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            PhotoSource = new();
            DataContext = this;
            IpAdress = IPAddress.Parse("192.168.100.225");
            IpAdressEndPoint = new(IpAdress, 27001);
            Client = new TcpClient();
        }

        public RelayCommand StartCommand
        {
            get => new RelayCommand(() =>
            {
                Task.Run(() =>
                {
                    while (true)
                    {
                        try
                        {
                            Client = new();
                            Client.Connect(IpAdressEndPoint);
                            if (Client.Connected)
                            {
                                try
                                {
                                    using (NetworkStream networkStream = Client.GetStream())
                                    {
                                        byte[] imageData = new byte[4096];
                                        int bytesRead;
                                        using (MemoryStream memoryStream = new MemoryStream())
                                        {

                                            while ((bytesRead = networkStream.Read(imageData, 0, imageData.Length)) > 0)
                                            {
                                                memoryStream.Write(imageData, 0, bytesRead);
                                            }


                                            memoryStream.Seek(0, SeekOrigin.Begin);
                                            BitmapImage bitmapImage = new BitmapImage();
                                            bitmapImage.BeginInit();
                                            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                                            bitmapImage.StreamSource = memoryStream;
                                            bitmapImage.EndInit();
                                            bitmapImage.Freeze();

                                            Dispatcher.Invoke(() => PhotoSource = bitmapImage);
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    // Handle exceptions if any
                                    MessageBox.Show("Error loading the image: " + ex.Message);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                        Task.Delay(5000).Wait();
                    }
                }
                );
            })
            {
            };
        }
    }
}
