﻿using System;
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
        public MaandTotaalWindow(List<MaandJaarTotaal> maandJaarTotalen)
        {
            InitializeComponent();
            cbxJaren.ItemsSource = maandJaarTotalen.Select(mjt => mjt.Jaar).Distinct();
            cbxJaren.SelectedIndex = 0;
            dataGrid.ItemsSource = maandJaarTotalen.Where(mjt => mjt.Jaar == (int) cbxJaren.SelectedItem);
        }

    }
}
