﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

namespace PZ1
{
    /// <summary>
    /// Interaction logic for Poligon.xaml
    /// </summary>
    public partial class Poligon : Window
    {
        public Poligon()
        {
            InitializeComponent();

            cmbColor.ItemsSource = typeof(Colors).GetProperties();
            cmbColor2.ItemsSource = typeof(Colors).GetProperties();
        }

       
        private void NacrtajPoligon_Click(object sender, RoutedEventArgs e)
        {
            double debljinaKonturneLinijeee = 1, parsiranjeKonturneLinije, providnostt = 0, parsiranjeProvidnosti;
            if (validate())
            {
                if (!double.TryParse(debljinaKonturneLinije.Text, out parsiranjeKonturneLinije)) { }
                debljinaKonturneLinijeee = parsiranjeKonturneLinije;

                if (!double.TryParse(providnost.Text, out parsiranjeProvidnosti)) { }
                providnostt = parsiranjeProvidnosti;

                Polygon polygon = new Polygon();
                polygon.StrokeThickness = debljinaKonturneLinijeee;
                Canvas.SetZIndex(polygon, 3);

                string boja = cmbColor.SelectedItem.ToString();
               
                boja = boja.Substring(27, boja.Length - 27);
               
                polygon.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(boja));
              
                polygon.Opacity = (100 - providnostt) / 100;
                polygon.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString(boja));
               

                double koordinataX = 0, koordinataY = 0;
                double poXmin = 0, poXmax = 0, poYmin = 0, poYmax = 0, prolaz = 0;
                int brojZaUslov = ((MainWindow)Application.Current.MainWindow).koordinatePoX.Count;
                for (int i = 0; i < brojZaUslov; i++)
                {
                    koordinataX = ((MainWindow)Application.Current.MainWindow).koordinatePoX[i];
                    koordinataY = ((MainWindow)Application.Current.MainWindow).koordinatePoY[i];

                    System.Windows.Point tacka = new System.Windows.Point(koordinataX, koordinataY);

                    if (prolaz == 0)
                    {
                        poXmax = koordinataX;
                        poXmin = koordinataX;
                        poYmax = koordinataY;
                        poYmin = koordinataY;
                        prolaz++;
                    }
                    else
                    {
                       
                        if (koordinataX > poXmax)
                        {
                            poXmax = koordinataX;
                        }

                        if (koordinataX < poXmin)
                        {
                            poXmin = koordinataX;
                        }

                     
                        if (koordinataY > poYmax)
                        {
                            poYmax = koordinataY;
                        }

                        if (koordinataY < poYmin)
                        {
                            poYmin = koordinataY;
                        }
                    }

                    polygon.Points.Add(tacka);
                }


               
                var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
                var stringChars = new char[8];
                var random = new Random();

                for (int i = 0; i < stringChars.Length; i++)
                {
                    stringChars[i] = chars[random.Next(chars.Length)];
                }

                var finalString = new String(stringChars);
                polygon.Name = finalString;
               
                TextBlock prosledjujemTekst = new TextBlock();
                prosledjujemTekst.Margin = new Thickness(koordinataX - (poXmax - poXmin) / 2, koordinataY - (poYmax - poYmin) / 2, 0, 0);
                prosledjujemTekst.FontSize = 10;
                
                Canvas.SetZIndex(prosledjujemTekst, 4);

                string tekstZaPoligon = "";
                if (tekstUnutarPoligona.Text.Length != 0)
                {
                    tekstZaPoligon = tekstUnutarPoligona.ToString();
                    tekstZaPoligon = tekstZaPoligon.Substring(33, tekstZaPoligon.Length - 33);
                }

                if (cmbColor2.SelectedItem != null)
                {
                    string boja2 = cmbColor2.SelectedItem.ToString();
                    boja2 = boja2.Substring(27, boja2.Length - 27);
                    prosledjujemTekst.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(boja2));
                }
               
                prosledjujemTekst.Text = tekstZaPoligon;
                
                prosledjujemTekst.Name = finalString + "pgtb";

                ((MainWindow)Application.Current.MainWindow).canvas.Children.Add(prosledjujemTekst);
                ((MainWindow)Application.Current.MainWindow).canvas.Children.Add(polygon);

                ((MainWindow)Application.Current.MainWindow).koordinatePoX.Clear();
                ((MainWindow)Application.Current.MainWindow).koordinatePoY.Clear();
                this.Close();
            }
            else
            {
                MessageBox.Show("Popunite sva obavezna polja", "Greska!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool validate()
        {
            bool result = true;

            
            double parsiranjeProvidnosti;
            if (providnost.Text.Trim().Equals(""))
            {
                result = false;
                providnost.BorderBrush = Brushes.Red;
                providnost.BorderThickness = new Thickness(1);
                lblProvidnost.Content = "Ne moze biti prazno";
            }
            else if (providnost.Text.Length > 10)
            {
                result = false;
                providnost.BorderBrush = Brushes.Red;
                providnost.BorderThickness = new Thickness(1);
                lblProvidnost.Content = "Morate uneti manji broj";
            }
            else if (!double.TryParse(providnost.Text, out parsiranjeProvidnosti))
            {
                result = false;
                providnost.BorderBrush = Brushes.Red;
                providnost.BorderThickness = new Thickness(1);
                lblProvidnost.Content = "Unesite broj bez karaktera";
            }
            else if (parsiranjeProvidnosti < 0)
            {
                result = false;
                providnost.BorderBrush = Brushes.Red;
                providnost.BorderThickness = new Thickness(1);
                lblProvidnost.Content = "Mora biti >0";
            }
            else if (parsiranjeProvidnosti >= 96)
            {
                result = false;
                providnost.BorderBrush = Brushes.Red;
                providnost.BorderThickness = new Thickness(1);
                lblProvidnost.Content = "Mora biti <=95";
            }
            else
            {
                providnost.BorderBrush = Brushes.Green;
                lblProvidnost.Content = string.Empty;
            }

          
            double parsiranjeKonturneLinije;

            if (debljinaKonturneLinije.Text.Trim().Equals(""))
            {
                result = false;
                debljinaKonturneLinije.BorderBrush = Brushes.Red;
                debljinaKonturneLinije.BorderThickness = new Thickness(1);
                lblKonturnaLinijaGreska.Content = "Ne moze biti prazno";
            }
            else if (debljinaKonturneLinije.Text.Length > 10)
            {
                result = false;
                debljinaKonturneLinije.BorderBrush = Brushes.Red;
                debljinaKonturneLinije.BorderThickness = new Thickness(1);
                lblKonturnaLinijaGreska.Content = "Morate uneti manji broj";
            }
            else if (!double.TryParse(debljinaKonturneLinije.Text, out parsiranjeKonturneLinije))
            {
                result = false;
                debljinaKonturneLinije.BorderBrush = Brushes.Red;
                debljinaKonturneLinije.BorderThickness = new Thickness(1);
                lblKonturnaLinijaGreska.Content = "Unesite broj bez karaktera";
            }
            else if (parsiranjeKonturneLinije <= 0)
            {
                result = false;
                debljinaKonturneLinije.BorderBrush = Brushes.Red;
                debljinaKonturneLinije.BorderThickness = new Thickness(1);
                lblKonturnaLinijaGreska.Content = "Mora biti veci od 0";
            }
            else
            {
                debljinaKonturneLinije.BorderBrush = Brushes.Green;
                lblKonturnaLinijaGreska.Content = string.Empty;
            }

            
            if (cmbColor.SelectedItem == null)
            {
                result = false;
                cmbColor.BorderBrush = Brushes.Red;
                cmbColor.BorderThickness = new Thickness(1);
                lblcmdGreska.Content = "Morate izabrati boju";
            }
            else
            {
                cmbColor.BorderBrush = Brushes.Green;
                lblcmdGreska.Content = string.Empty;
            }

            return result;
        }

        private void cmbColor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbColor.SelectedItem != null)
            {
                try
                {
                    var izabrana = (PropertyInfo)cmbColor.SelectedItem;
                    var boja = (Color)izabrana.GetValue(null, null);
                }
                catch (Exception)
                {
                    MessageBox.Show("Boja nije izabrana", "Greska!", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                }
            }
        }
    }
}
