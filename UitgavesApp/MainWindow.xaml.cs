using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace UitgavesApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ObservableCollection<UitgaveItem> _uitgaves;

        public MainWindow()
        {
            InitializeComponent();
            _uitgaves = new ObservableCollection<UitgaveItem>();
            dataGrid.ItemsSource = _uitgaves;

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
            _uitgaves.Add(new UitgaveItem
            {
                Uitgegeven = Convert.ToDouble(txtBoxUitgegeven.Text),
                Datum = dtpDatum.SelectedDate.GetValueOrDefault().ToString("dd MMMM yyyy"),
                Categorie = ((ComboBoxItem)cbxCategorie.SelectedItem).Content.ToString()
            });
        }

        class UitgaveItem
        {
            public double Uitgegeven { get; set; }
            public string Datum { get; set; }
            public string Categorie { get; set; }
        }

        private void BtnTotaal_Click(object sender, RoutedEventArgs e)
        {
            new TotaalWindow(GetMaandTotalen()).Show();
        }

        private List<MaandTotaal> GetMaandTotalen()
        {
            var maandTotalen = new List<MaandTotaal>();
            var uitgaveItemList = ConverteerItemsNaarUitgaveItems(dataGrid.Items);
            var uitgaveList = ConverteerUitgaveItemsNaarUitgaves(uitgaveItemList);
            for (int maand = 1; maand <= 12; maand = maand + 1)
            {
                var maandNaam = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(maand);
                var maandTotaal = new MaandTotaal
                {
                    Maand = maandNaam,
                    Totaal = uitgaveList.Where(uitgave => uitgave.Maand.Equals(maandNaam)).Sum(uitgave => uitgave.Uitgegeven).ToString("C", CultureInfo.CreateSpecificCulture("nl"))
                };
                maandTotalen.Add(maandTotaal);
            }
            return maandTotalen;
        }

        private List<CategorieTotaal> GetCategorieTotalen()
        {
            var categorieTotalen = new List<CategorieTotaal>();
            return null;
        }

        private List<UitgaveItem> ConverteerItemsNaarUitgaveItems(ItemCollection datagridItems)
        {
            return datagridItems.OfType<UitgaveItem>().ToList();
        }

        private List<Uitgave> ConverteerUitgaveItemsNaarUitgaves(List<UitgaveItem> uitgaveItems)
        {
            var uitgaveList = new List<Uitgave>();
            foreach (var uitgaveItem in uitgaveItems)
            {
                var maand = DateTime.ParseExact(uitgaveItem.Datum, "dd MMMM yyyy", null).Month;
                var maandNaam = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(maand);
                var uitgave = new Uitgave
                {
                    Maand = maandNaam,
                    Uitgegeven = uitgaveItem.Uitgegeven
                };
                uitgaveList.Add(uitgave);
            }
            return uitgaveList;
        }
    }
}
