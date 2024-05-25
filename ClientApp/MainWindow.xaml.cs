using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
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
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        internal ServerConnection servCon;

        public MainWindow()
        {
            InitializeComponent();
            OnLoad();
        }


        private void OnLoad() {

            //сохраненние прошлой авторизации
            if (mainFrame.Content == null) {
                mainFrame.Content = new LoginPage(this);
            }
        }




    }
}
