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
            if (cbxCategorie.SelectedItem == cbxItemAndere)
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
            var selectedCategorie = ((ComboBoxItem)cbxCategorie.SelectedValue).Content.ToString();
            string categorieNaam;

            if (selectedCategorie.Equals("Andere"))
            {
                if (string.IsNullOrEmpty(txtboxAndere.Text))
                {
                    MessageBox.Show("Gelieve een categorienaam in te geven.");
                    return;
                }
                else
                {
                    categorieNaam = txtboxAndere.Text;
                }
            }
            else
            {
                categorieNaam = ((ComboBoxItem)cbxCategorie.SelectedItem).Content.ToString();
            }

            _uitgaves.Add(new UitgaveItem
            {
                Uitgegeven = Convert.ToDouble(txtBoxUitgegeven.Text),
                Datum = dtpDatum.SelectedDate.GetValueOrDefault().ToString("dd MMMM yyyy"),
                Categorie = categorieNaam
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
            new TotaalWindow(GetMaandJaarTotalen(), GetCategorieTotalen()).Show();
        }

        private List<MaandJaarTotaal> GetMaandJaarTotalen()
        {
            var maandJaarTotalen = new List<MaandJaarTotaal>();
            var uitgaveItemList = ConverteerItemsNaarUitgaveItems(dataGrid.Items);
            var maandJaarUitgegevenList = ConverteerUitgaveItemsNaarMaandJaarUitgegeven(uitgaveItemList);
            for (int maand = 1; maand <= 12; maand = maand + 1)
            {
                var maandNaam = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(maand);
                var jaarUitgegeven = maandJaarUitgegevenList.GroupBy(l => l.Jaar).ToList();
                var maandUitgegevenListHuidigeMaand = maandJaarUitgegevenList.Where(uitgave => uitgave.Maand.Equals(maandNaam));

                foreach(var jaar in jaarUitgegeven)
                {
                    var maandJaarUitgegevenHuidigJaar = maandUitgegevenListHuidigeMaand.Where(mju => mju.Jaar == jaar.Key);
                    var somUitgegevenHuidigeMaand = maandJaarUitgegevenHuidigJaar.Sum(uitgave => uitgave.Uitgegeven);
                    var maandTotaal = new MaandJaarTotaal
                    {
                        Maand = maandNaam,
                        Jaar = jaar.Key,
                        Totaal = somUitgegevenHuidigeMaand
                    };
                    maandJaarTotalen.Add(maandTotaal);
                }
            }
            return maandJaarTotalen;
        }

        private List<CategorieTotaal> GetCategorieTotalen()
        {
            var categorieTotalen = new List<CategorieTotaal>();
            var uitgaveItemList = ConverteerItemsNaarUitgaveItems(dataGrid.Items);
            var categorieUitgegevenList = ConverteerUitgaveItemsNaarCategorieUitgegeven(uitgaveItemList);

            var categorieen = dataGrid.Items.OfType<UitgaveItem>().Select(uitgaveItem => uitgaveItem.Categorie).Distinct().ToList();

            for (int categorie = 1; categorie <= categorieen.Count; categorie = categorie + 1)
            {
                var categorieNaam = categorieen[categorie - 1];
                var categorieUitgegevenListHuidigeCategorie = categorieUitgegevenList.Where(cu => cu.Categorie.Equals(categorieNaam));
                var somUitgegevenHuidigeCategorie = categorieUitgegevenListHuidigeCategorie.Sum(cu => cu.Uitgegeven);

                var categorieTotaal = new CategorieTotaal
                {
                    Categorie = categorieNaam,
                    Totaal = somUitgegevenHuidigeCategorie
                };
                categorieTotalen.Add(categorieTotaal);
            }
            return categorieTotalen;
        }

        private List<CategorieUitgegeven> ConverteerUitgaveItemsNaarCategorieUitgegeven(List<UitgaveItem> uitgaveItems)
        {
            var categorieUitgegevenList = new List<CategorieUitgegeven>();
            foreach (var uitgaveItem in uitgaveItems)
            {
                var categorie = uitgaveItem.Categorie;
                var categorieUitgegeven = new CategorieUitgegeven
                {
                    Categorie = categorie,
                    Uitgegeven = uitgaveItem.Uitgegeven
                };
                categorieUitgegevenList.Add(categorieUitgegeven);
            }
            return categorieUitgegevenList;
        }

        private List<UitgaveItem> ConverteerItemsNaarUitgaveItems(ItemCollection datagridItems)
        {
            return datagridItems.OfType<UitgaveItem>().ToList();
        }

        private List<MaandJaarUitgegeven> ConverteerUitgaveItemsNaarMaandJaarUitgegeven(List<UitgaveItem> uitgaveItems)
        {
            var maandUitgegevenList = new List<MaandJaarUitgegeven>();
            foreach (var uitgaveItem in uitgaveItems)
            {
                var datetime = DateTime.ParseExact(uitgaveItem.Datum, "dd MMMM yyyy", null);
                var maandNaam = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(datetime.Month);
                var maandJaarUitgegeven = new MaandJaarUitgegeven
                {
                    Maand = maandNaam,
                    Jaar = datetime.Year,
                    Uitgegeven = uitgaveItem.Uitgegeven
                };
                maandUitgegevenList.Add(maandJaarUitgegeven);
            }
            return maandUitgegevenList;
        }
    }
}
