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
using System.Windows.Shapes;

namespace GerenciamentoDeFotos
{
    /// <summary>
    /// Lógica interna para Mensagem.xaml
    /// </summary>
    public partial class Mensagem : Window
    {
        public bool _Confirmacao { get; set; }

        public Mensagem(string msg = "", string descricao = "", string btnsim = "Sim", string btnnao = "Não", int Height = 140, int Width = 700)
        {
            InitializeComponent();
            lblmsg.Content = msg;
            lbldescricao.Text = descricao;
            btnSim.Content = btnsim;
            btnNao.Content = btnnao;
            ViewTela.Height = Height;
            ViewTela.Width = Width;
        }

        private void btnSim_Click(object sender, RoutedEventArgs e)
        {
            _Confirmacao = true;
            this.Close();
        }

        private void btnNao_Click(object sender, RoutedEventArgs e)
        {
            _Confirmacao = false;
            this.Close();
        }


        private void ViewTela_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}