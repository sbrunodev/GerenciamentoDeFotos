using Microsoft.Win32;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GerenciamentoDeFotos
{
    /// <summary>
    /// Interação lógica para MainWindow.xam
    /// </summary>
    public partial class MainWindow : Window
    {
        double TamanhoMiniaturas = 300; // Altere esse valor para mudar o tamanho das miniaturas.
        string NomePasta = "ImagensPacientes"; // Altere esse valor para alterar o nome da pasta para salvar as fotos
        byte[] ByteImg;
        double Largura, Altura;
        string CaminhoProjeto;
        string CaminhoPasta;
        int Codigo;
        DataTable DtImagens = new DataTable();

        public MainWindow()
        {
            ByteImg = null;
            Largura = 0; Altura = 0;
            CaminhoPasta = "";
            InitializeComponent();
            CaminhoProjeto = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + @"\" + NomePasta + @"\";
            ExistePasta();
            Codigo = 0;
            btnCancelar.Visibility = Visibility.Collapsed;


            DtImagens.Columns.Add("Caminho");
            DtImagens.Columns.Add("Nome");
        }

        private void ExistePasta()
        {
            if (Directory.Exists(CaminhoProjeto))
                Directory.CreateDirectory(CaminhoProjeto);
        }


        private void GerenciaTamanho()
        {
            if (Largura >= TamanhoMiniaturas)
            {
                Largura = Largura / 2;
                Altura = Altura / 2;
                GerenciaTamanho();
            }

        }

        private bool ExisteDT(string Descricao, string Campo)
        {
            for(int i=0; i < DtImagens.Rows.Count; i++)
                if (DtImagens.Rows[i][Campo].ToString().Equals(Descricao))
                    return false;
            
            return true;
        }

        private void btnAddFoto_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog FileImagem = new OpenFileDialog();
                FileImagem.Multiselect = true;
                FileImagem.InitialDirectory = Environment.SpecialFolder.MyPictures.ToString();
                FileImagem.Filter = "Image File (*.jpg;*.bmp;*.gif;*.jpeg;*.png;)|*.jpg;*.bmp;*.gif;*.jpeg;*.png;";
                FileImagem.ShowDialog();
                {
                    List <string> ListCaminhos = new List<string>();
                    List<string> ListNomes = new List<string>();
                    
                    foreach (string CaminhoImagem in FileImagem.FileNames)                    
                        ListCaminhos.Add(CaminhoImagem);

                    foreach (string NomeImagem in FileImagem.SafeFileNames)
                        ListNomes.Add(NomeImagem);

                    for(int Pos=0;Pos<ListCaminhos.Count;Pos++)
                    {
                        bool v1, v2;
                        v1 = ExisteDT(ListCaminhos[Pos],"Caminho");
                        v2 = ExisteDT(ListNomes[Pos], "Nome");

                        if(v1 == true && v2 == true)                        
                            DtImagens.Rows.Add(ListCaminhos[Pos], ListNomes[Pos]);                        
                    }

                    for(int i=0;i<DtImagens.Rows.Count;i++)
                    {
                        string NomeImagem = DtImagens.Rows[i]["Nome"].ToString();
                        string CaminhoImagem = DtImagens.Rows[i]["Caminho"].ToString();
                        
                        // Caso a imagem não exista
                        if(!File.Exists(CaminhoPasta + NomeImagem)) { 
                            File.Copy(CaminhoImagem, CaminhoPasta+NomeImagem);
                            ByteImg = ReadImageFile(CaminhoImagem);
                            
                            BitmapImage Img = ToImage();
                            Altura = Img.Height; Largura = Img.Width;
                            GerenciaTamanho();
                            AdicionaFoto(Img, CaminhoImagem);
                        }
                        
                    }



                }
                FileImagem = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }


        private void AdicionaFoto(BitmapImage Img, string Nome)
        {
            // Corpo da imagem
            StackPanel CorpoImagem = new StackPanel();
            CorpoImagem.Width = Largura;
            CorpoImagem.Height = Altura + 60; // Esse 60 é o tamanho do TextBox
            CorpoImagem.Margin = new Thickness(5);

            // Grid Imagem
            Grid GridImagem = new Grid();
            GridImagem.Width = Largura;
            GridImagem.Height = Altura;
            GridImagem.Background = new SolidColorBrush(Colors.Blue);

            int ID = ContarStackPanel(); 

            // Imagem
            Image Imagem = new Image();
            Imagem.Source = Img;
            Imagem.Height = Altura;
            Imagem.Width = Largura;
            Imagem.MouseUp += Imagem_MouseUp;
            Imagem.Name = "Img" + ID;

            // Descrição da Imagem
            TextBox DescricaoImagem = new TextBox();
            DescricaoImagem.Width = Largura;
            DescricaoImagem.Height = 60;
            DescricaoImagem.TextWrapping = TextWrapping.Wrap;


            // Botão Fechar
            Button BtnFecharImagem = new Button();
            BtnFecharImagem.Content = "X";
            BtnFecharImagem.Width = 25;
            BtnFecharImagem.Height = 25;
            BtnFecharImagem.FontSize = 20;
            BtnFecharImagem.VerticalAlignment = VerticalAlignment.Top;
            BtnFecharImagem.HorizontalAlignment = HorizontalAlignment.Right;
            BtnFecharImagem.Visibility = Visibility.Collapsed;
            BtnFecharImagem.Click += new RoutedEventHandler(ExcluirImagem);
            BtnFecharImagem.Name = "Btn" + ID;

            // Adicionar o botão X em cima da imagem
            GridImagem.Children.Add(BtnFecharImagem);
            Panel.SetZIndex(BtnFecharImagem, 10); // Deixar o botão fechar em cima da imagem
            GridImagem.Children.Add(Imagem);

            // Adicionar Imagem e Descrição ao corpo da imagem
            CorpoImagem.Children.Add(GridImagem);
            CorpoImagem.Children.Add(DescricaoImagem);


            // Adicionar corpo da imagem ao painel
            //CorpoImagem.Background = new SolidColorBrush(Colors.White);
            CorpoImagem.VerticalAlignment = VerticalAlignment.Top;
            CorpoImagem.MouseEnter += new MouseEventHandler(MouseEntrandoNaFoto);
            CorpoImagem.MouseLeave += new MouseEventHandler(MouseSaiuDaFoto);

            // Adicionar corpo da imagem ao painel
            WrapFundo.Children.Add(CorpoImagem);
        }

        private void Imagem_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Image Img = (Image)sender;
            
            int PosicaoImg = Convert.ToInt32(Img.Name.Replace("Img","").ToString());

            Visualizador Visualize = new Visualizador(DtImagens.Rows[PosicaoImg]["Caminho"].ToString(),DtImagens, PosicaoImg);
            Visualize.Show();
        }

        private void ExcluirImagem(object sender, RoutedEventArgs e)
        {
            Mensagem Pergunta = new Mensagem("Deseja realmente excluir essa mensagem?");
            Pergunta.ShowDialog();

            if (Pergunta._Confirmacao)
            {
                Button BtnFechar = (Button)sender;

                StackPanel p = GetParentWindow(BtnFechar);
                if (p == null)
                    MessageBox.Show("Não foi possivel excluir essa imagem");
                else
                    WrapFundo.Children.Remove(p);
            }
        }

        public static StackPanel GetParentWindow(DependencyObject child)
        {
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);

            if (parentObject == null)
                return null;

            StackPanel parent = parentObject as StackPanel;
            if (parent != null)
                return parent;
            else
                return GetParentWindow(parentObject);
        }


        private void MouseSaiuDaFoto(object sender, MouseEventArgs e)
        {
            StackPanel Painel = (StackPanel)sender;
            Grid _Grid = Painel.Children.OfType<Grid>().FirstOrDefault();
            Button BtnFecharImagem = _Grid.Children.OfType<Button>().FirstOrDefault();

            BtnFecharImagem.Visibility = Visibility.Collapsed;
        }

        private void MouseEntrandoNaFoto(object sender, MouseEventArgs e)
        {
            StackPanel Painel = (StackPanel)sender;
            Grid _Grid = Painel.Children.OfType<Grid>().FirstOrDefault();
            Button BtnFecharImagem = _Grid.Children.OfType<Button>().FirstOrDefault();

            BtnFecharImagem.Visibility = Visibility.Visible;
        }

        private int ContarStackPanel()
        {
            int Count = 0;
            foreach (Object obj in WrapFundo.Children)
                if (obj.GetType() == typeof(StackPanel))
                    Count++;

            return Count;
        }

        // Realiza o salvar as fotos e criar as pastas correspondentes

        private string Valida()
        {
            string e = "";
            if (ttbCodigo.Text.Equals(""))
                e += "Informe o código\n";
            else
            {
                string CaminhoPasta = CaminhoProjeto + ttbCodigo.Text + @"\";
                if (Directory.Exists(CaminhoPasta))
                    e += "Esse código já está sendo usado\n";
            }
            return e;
        }

        private void btnSalvar_Click(object sender, RoutedEventArgs e)
        {
            string strValida = Valida();
            if (strValida.Equals(""))
            {
                Directory.CreateDirectory(CaminhoProjeto + "\\" + ttbCodigo.Text);
                ttbCodigo.Text = ""; ttbNome.Text = "";
                Codigo = 0;
            }
            else
                MessageBox.Show(strValida);
        }

        private void CarregaImagens(string CaminhoPasta)
        {
            DirectoryInfo D = new DirectoryInfo(CaminhoPasta);
           
            FileInfo[] Fotos = D.GetFiles();
            
            foreach (FileInfo F in Fotos)
            {
                DtImagens.Rows.Add(F.FullName, F.Name);
               
                ByteImg = ReadImageFile(F.FullName);

                BitmapImage Img = ToImage();
                Altura = Img.Height; Largura = Img.Width;
                GerenciaTamanho();
                AdicionaFoto(Img, F.FullName);
            }
        }

        private void btnPesquisar_Click(object sender, RoutedEventArgs e)
        {
            if (!ttbCodigo.Text.Equals(""))
            {
                CaminhoPasta = CaminhoProjeto + ttbCodigo.Text + "\\";

                if (Directory.Exists(CaminhoPasta))
                {
                    btnCancelar.Visibility = Visibility.Visible;
                    btnSalvar.IsEnabled = false;
                    btnPesquisar.IsEnabled = false;
                    DtImagens.Rows.Clear();
                    CarregaImagens(CaminhoPasta);
                }
                else
                {
                    MessageBox.Show("Essa Pasta não foi Encontrada");
                    CaminhoPasta = "";
                }

            }
            else
                MessageBox.Show("Informe um código para ser pesquisado");
        }


        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            btnSalvar.IsEnabled = true;
            btnPesquisar.IsEnabled = true;
            btnCancelar.Visibility = Visibility.Collapsed;
            ttbCodigo.Text = "";
            CaminhoPasta = "";
            WrapFundo.Children.Clear();
        }

        // Gerenciando as imagens
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

        private void tabFotos_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (CaminhoPasta.Equals(""))
                btnAddFoto.IsEnabled = false;
            else
                btnAddFoto.IsEnabled = true;
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



    }
}
