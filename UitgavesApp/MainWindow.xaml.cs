﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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
            SeedGrid();
        }

        private void SeedGrid()
        {
            using (SqlConnection conn = new SqlConnection(App.ConnectionString))
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.Text,
                    CommandText = @"SELECT Datum, Bedrag, Naam 
                                    FROM Uitgaves
                                    JOIN Categorieen
                                        ON Uitgaves.CategorieId = Categorieen.Id",
                    Connection = conn
                };

                try
                {
                    conn.Open();
                    var reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        var datum = ((DateTime) reader["Datum"]).ToString("dd MMMM yyyy");
                        var bedrag = (double) reader["Bedrag"];
                        var categorie = reader["Naam"].ToString();

                        _uitgaves.Add(new UitgaveItem
                        {
                            Datum = datum,
                            Uitgegeven = bedrag,
                            Categorie = categorie
                        });
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    cmd.Dispose();
                }
            }
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
            using (SqlConnection conn = new SqlConnection(App.ConnectionString))
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.Text,
                    CommandText = @"BEGIN
                                       IF NOT EXISTS (SELECT * FROM Categorieen WHERE Naam = @Naam)
                                       BEGIN
                                           INSERT INTO Categorieen (Naam) VALUES (@Naam);
                                       END
                                    END",
                    Connection = conn
                };

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

                var bedrag = Convert.ToDouble(txtBoxUitgegeven.Text);
                var datum = dtpDatum.SelectedDate.GetValueOrDefault();

                _uitgaves.Add(new UitgaveItem
                {
                    Uitgegeven = bedrag,
                    Datum = datum.ToString("dd MMMM yyyy"),
                    Categorie = categorieNaam
                });

                cmd.Parameters.AddWithValue("@Naam", categorieNaam);

                try
                {
                    conn.Open();
                    var modified = cmd.ExecuteScalar();

                    int categorieId;

                    if (modified != null)
                    {
                        cmd = new SqlCommand
                        {
                            CommandType = CommandType.Text,
                            CommandText = "SELECT @@IDENTITY;",
                            Connection = conn
                        };

                        categorieId = (int) cmd.ExecuteScalar();
                    }
                    else
                    {
                        cmd = new SqlCommand
                        {
                            CommandType = CommandType.Text,
                            CommandText = "SELECT Id FROM Categorieen WHERE Naam = @Naam",
                            Connection = conn
                        };
                        cmd.Parameters.AddWithValue("@Naam", categorieNaam);
                        categorieId = (int) cmd.ExecuteScalar();
                    }

                    cmd = new SqlCommand
                    {
                        CommandType = CommandType.Text,
                        CommandText = @"INSERT INTO Uitgaves (Datum, Bedrag, CategorieId) VALUES (@Datum, @Bedrag, @CategorieId);",
                        Connection = conn
                    };
                    cmd.Parameters.AddWithValue("@Datum", datum);
                    cmd.Parameters.AddWithValue("@Bedrag", bedrag);
                    cmd.Parameters.AddWithValue("@CategorieId", categorieId);

                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    cmd.Dispose();
                }
            }           
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

        private void TxtBoxUitgegeven_KeyDown(object sender, KeyEventArgs e)
        {
            var numpadNummers = e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9;
            var toetsenBordNummers = e.Key >= Key.D0 && e.Key <= Key.D9;
            var komma = e.Key == Key.OemComma;

            if (!numpadNummers && !toetsenBordNummers && !komma)
            {
                e.Handled = true;
                MessageBox.Show($"{e.Key.ToString()} is forbidden");
            }
        }
    }
}
