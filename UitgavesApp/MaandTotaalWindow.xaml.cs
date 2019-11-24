using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for MaandTotaalWindow.xaml
    /// </summary>
    public partial class MaandTotaalWindow : Window
    {
        private readonly ObservableCollection<MaandJaarTotaal> _maandJaarTotalen;

        public MaandTotaalWindow(List<MaandJaarTotaal> maandJaarTotalen)
        {
            InitializeComponent();
            _maandJaarTotalen = new ObservableCollection<MaandJaarTotaal>(maandJaarTotalen);
            cbxJaren.ItemsSource = _maandJaarTotalen.Select(mjt => mjt.Jaar).Distinct().OrderBy(mjt => mjt);
            cbxJaren.SelectedIndex = 0;
            dataGrid.ItemsSource = _maandJaarTotalen.Where(mjt => mjt.Jaar == (int) cbxJaren.SelectedItem);
        }

        private void CbxJaren_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            dataGrid.ItemsSource = _maandJaarTotalen.Where(mjt => mjt.Jaar == (int)cbxJaren.SelectedItem);
        }
    }
}
