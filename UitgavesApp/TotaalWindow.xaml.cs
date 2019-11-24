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

namespace UitgavesApp
{
    /// <summary>
    /// Interaction logic for TotaalWindow.xaml
    /// </summary>
    public partial class TotaalWindow : Window
    {
        private readonly List<MaandTotaal> _maandTotalen;
        public TotaalWindow(List<MaandTotaal> maandTotalen)
        {
            InitializeComponent();
            _maandTotalen = maandTotalen;
        }

        private void BtnMaand_Click(object sender, RoutedEventArgs e)
        {
            new MaandTotaalWindow(_maandTotalen).Show();
        }


        private void BtnCategorie_Click(object sender, RoutedEventArgs e)
        {
            new CategorieTotaalWindow().Show();
        }
    }
}
