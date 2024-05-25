using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
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


namespace ClientApp
{
    /// <summary>
    /// Логика взаимодействия для LoginPage.xaml
    /// </summary>
    public partial class LoginPage : Page
    {
        MainWindow mW;
        public LoginPage(MainWindow _mainWindow)
        {

            InitializeComponent();
            mW = _mainWindow;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (lgnTB.Text != "" && pwdTB.Text != "")
            {
                try
                {
                    //текст запроса авторизации к серверу
                    string requestForServer = "autorization|" + lgnTB.Text + "|" + pwdTB.Text + "\n";

                    mW.servCon = new ServerConnection();
                    //запрос к серверу
                    string res = mW.servCon.Request(requestForServer);
                   
                    //расшифровка ответа от сервера
                    switch (res.Substring(0, res.IndexOf('|'))){
                        case "accepted":
                            mW.mainFrame.Content = new DashboardPage(mW);
                            break;
                        case "denied":
                            errorLb.Content = "Логин или пароль не верны";
                            break;
                        default:
                            errorLb.Content = "Ошибка авторизации";
                            break;
                    }
                }
                catch (Exception er)
                {
                    errorLb.Content = "Error";
                }
            }
        }
    }
}
