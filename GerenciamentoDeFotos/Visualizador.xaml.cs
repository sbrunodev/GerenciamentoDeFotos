using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
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
    /// Lógica interna para Visualizador.xaml
    /// </summary>
    public partial class Visualizador : Window
    {
        byte[] ByteImg;
        DataTable DtImagens;
        int Posicao;
        public Visualizador(string CaminhoImagem, DataTable pDtImagens, int pPosicao)
        {            
            InitializeComponent();            
            DtImagens = pDtImagens;
            Posicao = pPosicao;
            CarregaImagem(CaminhoImagem);
            AtualizaTituloImagem();
            Horario();

        
        }

        private void CarregaImagem(string CaminhoImagem)
        {
            ByteImg = null;
            ByteImg = ReadImageFile(CaminhoImagem);
            Imagem.Source = ToImage();
        }

        public BitmapImage ToImage() // byte[] array
        {
            try
            {
                using (var ms = new System.IO.MemoryStream(ByteImg)) // array
                {
                    var image = new BitmapImage();
                    image.BeginInit();
                    image.CacheOption = BitmapCacheOption.OnLoad; // here
                    image.StreamSource = ms;
                    image.EndInit();
                    return image;
                }
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public byte[] ReadImageFile(string imageLocation)
        {
            try
            {
                byte[] imageData = null;
                FileInfo fileInfo = new FileInfo(imageLocation);
                long imageFileLength = fileInfo.Length;
                FileStream fs = new FileStream(imageLocation, FileMode.Open, FileAccess.Read);
                BinaryReader br = new BinaryReader(fs);
                imageData = br.ReadBytes((int)imageFileLength);
                return imageData;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        private void btnFechar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnProx_Click(object sender, RoutedEventArgs e)
        {
            if( (Posicao+1) <= DtImagens.Rows.Count-1 )            
                Posicao++;
            AtualizaTituloImagem();            
        }

        private void btnAnt_Click(object sender, RoutedEventArgs e)
        {
            if ((Posicao - 1) >= 0)            
                Posicao--;
            AtualizaTituloImagem();         
        }

        private void AtualizaTituloImagem()
        {
            CarregaImagem(DtImagens.Rows[Posicao]["Caminho"].ToString());
            lblNome.Content = DtImagens.Rows[Posicao]["Nome"].ToString();
            UltimoMovimento = 0;
            if (Posicao == DtImagens.Rows.Count - 1)
                btnProx.Visibility = Visibility.Collapsed;
            else
                btnProx.Visibility = Visibility.Visible;

            if (Posicao == 0)
                btnAnt.Visibility = Visibility.Collapsed;
            else
                btnAnt.Visibility = Visibility.Visible;
        }

        private void Window_MouseUp(object sender, MouseButtonEventArgs e)
        {
           
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                this.Close();

            if (e.Key == Key.Left)
                btnAnt_Click(null, null);
            if (e.Key == Key.Right)
                btnProx_Click(null, null);

            if (e.Key == Key.Enter)
                ttbMensagem.Focus();
        }

   
        int UltimoMovimento = 0;
        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            UltimoMovimento = 0;

            if(!ttbMensagem.IsVisible)
            {
                btnProx.Visibility = Visibility.Visible;
                btnAnt.Visibility = Visibility.Visible;
                ttbMensagem.Visibility = Visibility.Visible;
                btnFechar.Visibility = Visibility.Visible;
                GridButtons.Visibility = Visibility.Visible;
            }
        }


        // Tempo
        System.Windows.Threading.DispatcherTimer Timer = new System.Windows.Threading.DispatcherTimer();
        private void Horario()
        {
            Timer.Tick += new EventHandler(Timer_Tick);
            Timer.Interval = new TimeSpan(0, 0, 1);
            Timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            UltimoMovimento = UltimoMovimento + 1;

            if(UltimoMovimento==5)
            {
                btnProx.Visibility = Visibility.Collapsed;
                btnAnt.Visibility = Visibility.Collapsed;
                ttbMensagem.Visibility = Visibility.Collapsed;
                btnFechar.Visibility = Visibility.Collapsed;
                GridButtons.Visibility = Visibility.Collapsed;

               
               
            }

        }

        private void btnZoomMax_Click(object sender, RoutedEventArgs e)
        {
            Scale.ScaleX += 0.25;
            Scale.ScaleY += 0.25;
        }

        private void btnZoomMenos_Click(object sender, RoutedEventArgs e)
        {
            if (Scale.ScaleX > 0.25)
            {
                Scale.ScaleX -= 0.25;
                Scale.ScaleY -= 0.25;
            }
        }


        private double Angle = 90;

        private void btnRotate_Click(object sender, RoutedEventArgs e)
        {
            Rotator.Angle += Angle;
            if (Rotator.Angle == 360)
                Rotator.Angle = 0;
        }
    }
}
