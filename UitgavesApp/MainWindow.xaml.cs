using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace UitgavesApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<Uitgave> _uitgaves;

        public MainWindow()
        {
            InitializeComponent();
            _uitgaves = new List<Uitgave>();
            //dataGrid.ItemsSource = _uitgaves;
        }

        private void CbxCategorie_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if(cbxCategorie.SelectedItem == cbxItemAndere)
            {
                txtboxAndere.Visibility = Visibility.Visible;
                lblAndere.Visibility = Visibility.Visible;
            }
            else
            {
                txtboxAndere.Visibility = Visibility.Hidden;
                lblAndere.Visibility = Visibility.Hidden;
            }
        }

        private void BtnToevoegen_Click(object sender, RoutedEventArgs e)
        {
            dataGrid.ItemsSource = new List<Uitgave>
            {
                new Uitgave
                {
                    Uitgegeven = Convert.ToDouble(txtBoxUitgegeven.Text),
                    Datum = dtpDatum.SelectedDate.GetValueOrDefault().ToString("dd MMMM yyyy"),
                    Categorie = ((ComboBoxItem) cbxCategorie.SelectedItem).Content.ToString()
                }
            };
        }

        class Uitgave
        {
            public double Uitgegeven { get; set; }
            public string Datum { get; set; }
            public string Categorie { get; set; }
        }
    }
}
