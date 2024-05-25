using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
    /// Логика взаимодействия для DashboardPage.xaml
    /// </summary>
    public partial class DashboardPage : Page
    {

        MainWindow mW;

        public DashboardPage(MainWindow _mainWindow)
        {
            mW = _mainWindow;
            InitializeComponent();
        }

        private void OnLoad() {

            try
            {
                //заканчивающиеся ЭЦП
                string requestForServer = "endingEDS|\n";

                string[,] res = mW.servCon.RequestRows(requestForServer);
                if (res!=null)
                { 
                    for (int i=0; i<res.Length; i++)
                    {

                        // заполнение datagridview
                        //areEndingDG. ({

                       // }
                       // );
                        //.Rows[i].Cells[0].Value = res[0,0];
                    }
                        

                }
                else {
                    areEndingDG.Visibility = Visibility.Hidden;
                    areEndingL.Visibility = Visibility.Visible;
                } 
                
                
                //запрос ЭЦП в работе

                //запрос к серверу ЭЦП которые необходимо анулировать
            }catch (Exception er){
                
            }


        }


        private void newTgPush_Click(object sender, RoutedEventArgs e)
        {

        }

        private void searchBut_Click(object sender, RoutedEventArgs e)
        {
            mW.mainFrame.Content = new SearchPage(mW);
        }
    }
}
