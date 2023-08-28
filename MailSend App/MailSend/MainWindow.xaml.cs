using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
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

namespace MailSend
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MailAddress FromAdress { get; set; }

        public MailAddress ToAddress { get; set; }

        public SmtpClient SmtpClient { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            FromAdress = new MailAddress("islam.salamzade.191@gmail.com", "Islam Salamzade");
            SmtpClient = new SmtpClient("smtp.gmail.com",587);
            SmtpClient.Credentials = new NetworkCredential("islam.salamzade.191@gmail.com", "iqtlzvilirnndxlj");
            SmtpClient.EnableSsl = true;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            ToAddress = new MailAddress(Send_txt.Text);
            var subject = Subject_txt.Text;
            var body = Body_txt.Text;
            Task.Run(() =>
            {
                MailMessage mailMessage = new MailMessage(FromAdress, ToAddress);
                mailMessage.Subject = subject;
                mailMessage.Body = body;
                SmtpClient.Send(mailMessage);
                MessageBox.Show($"Message sent to {ToAddress}"!);
                Dispatcher.BeginInvoke(() =>
                {
                    Subject_txt.Clear();
                    Body_txt.Clear();
                    Send_txt.Clear();
                });
            });
        }
    }
}
