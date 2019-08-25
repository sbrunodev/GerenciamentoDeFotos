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
    /// Lógica interna para Teste.xaml
    /// </summary>
    public partial class Teste : Window
    {
        public Teste()
        {
            InitializeComponent();
        }

        private double angle = 90;

        private void ZoomInButton_Click(object sender, RoutedEventArgs e)
        {
            Scale.ScaleX += 0.25;
            Scale.ScaleY += 0.25;
        }

        private void ZoomOutButton_Click(object sender, RoutedEventArgs e)
        {
            if (Scale.ScaleX > 0.25)
            {
                Scale.ScaleX -= 0.25;
                Scale.ScaleY -= 0.25;
            }
        }

        private void RotateButton_Click(object sender, RoutedEventArgs e)
        {
            Rotator.Angle += angle;
            if (Rotator.Angle == 360)
                Rotator.Angle = 0;
        }
    }
}
