using KonyhaSegedWPF.ReceptModels;
using KonyhaSegedWPF.Services;
using System;
using System.Collections.Generic;
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

namespace KonyhaSegedWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ReceptService receptService = new ReceptService();
        private readonly HozzavaloService hozzavaloService = new HozzavaloService();
        private readonly AlapanyagService alapanyagService = new AlapanyagService();
        public List<Recept> receptek;
        public List<Hozzavalo> hozzavalok;
        public List<Alapanyag> alapanyagok;

        public MainWindow()
        {
            InitializeComponent();
            receptek = receptService.ReadReceptek();
            hozzavalok = hozzavaloService.ReadHozzavalok();
            alapanyagok = alapanyagService.ReadAlapanyagok();

            foreach (var recept in receptek)
            {
                tbkReceptek.Text += recept.ToString();
            }

            foreach (var hozzavalo in hozzavalok)
            {
                tbkHozzavalok.Text += hozzavalo.ToString();
            }

            foreach (var alapanyag in alapanyagok)
            {
                tbkAlapanyagok.Text += alapanyag.ToString();
            }
        }
    }
}
