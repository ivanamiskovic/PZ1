using PZ1.BFS;
using PZ1.Model;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using Point = System.Windows.Point;


namespace PZ1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Sve sto koristim od promenljivih

        List<Point> matrica = new List<Point>();
        public Dictionary<Point, PowerEntity> dictTackaElement = new Dictionary<Point, PowerEntity>();

        
        public List<PowerEntity> listaElemenataIzXML = new List<PowerEntity>();
        public List<LineEntity> listaVodova = new List<LineEntity>();
        public List<LineEntity> vodDuplikat = new List<LineEntity>();

        
        public double noviX, noviY;

        
        public int checkMinMax = 1;
        public double praviXmax, praviXmin, praviYmax, praviYmin;

        
        public double relativnoX, relativnoY;
        public double odstojanjeLon, odstojanjeLat;

        
        public double poX, poY;

        
        public List<double> koordinatePoX = new List<double>();
        public List<double> koordinatePoY = new List<double>();

       
        public int numberChildren = 0;
        List<UIElement> obrisaniListaZaBrojanje = new List<UIElement>();
        List<UIElement> ponovoIscrtaj = new List<UIElement>();
        List<UIElement> saMape = new List<UIElement>();

        
       
        public List<UIElement> zaBrisanje = new List<UIElement>();

        
        public List<SwitchEntity> listaSviceva = new List<SwitchEntity>();
        public List<LineEntity> vodoviZaBrisanje = new List<LineEntity>();
        public List<LineEntity> listaNeobrisanihVodova = new List<LineEntity>();
        public List<PowerEntity> entitetiZaBrisanje = new List<PowerEntity>();
        public List<PowerEntity> listaNeobrisanihEntiteta = new List<PowerEntity>();
        public List<UIElement> elementZaBrisanje = new List<UIElement>();

       
        public int brojac;
        public int brojac2;
        public int brojac3;


        
        #endregion

        public MainWindow()
        {
            InitializeComponent();
            UcitavanjeMatrice();
            UcitavanjeElemenata(); 
        }

        private void UcitavanjeMatrice()
        {
            
            Point rt;

            for (int i = 0; i <= 300; i++)
            {
                for (int j = 0; j <= 300; j++)
                {
                    rt = new Point(i, j);
                    matrica.Add(rt);
                }
            }
        }

        private void Load_Click(object sender, RoutedEventArgs e)
        {
            CrtajEntitete();
            CrtajVodove();

           
            numberChildren = canvas.Children.Count;
        }

        private void CrtajEntitete()
        {
            foreach (var element in listaElemenataIzXML)
            {
                ToLatLon(element.X, element.Y, 34, out noviX, out noviY);
                MestoNaCanvasu(noviX, noviY, out relativnoX, out relativnoY);

                
                Rectangle rect = new Rectangle();
                rect.Fill = element.Boja;
                rect.ToolTip = element.ToolTip; 
                rect.Width = 2;
                rect.Height = 2;




                #region Bez preklapanja
                Point mojaTacka = matrica.Find(pomocnaTacka => pomocnaTacka.X == relativnoX && pomocnaTacka.Y == relativnoY);

                int brojac = 1;
                if (!dictTackaElement.ContainsKey(mojaTacka))
                {
                    dictTackaElement.Add(mojaTacka, element);
                }
                else
                {
                    bool flag = false;
                    while (true)
                    {
                        for (double iksevi = relativnoX - brojac * 3; iksevi <= relativnoX + brojac * 3; iksevi += 3) 
                        {
                            if (iksevi < 0)
                                continue;
                            for (double ipsiloni = relativnoY - brojac * 3; ipsiloni <= relativnoY + brojac * 3; ipsiloni += 3)
                            {
                                if (ipsiloni < 0)
                                    continue;
                                mojaTacka = matrica.Find(t => t.X == iksevi && t.Y == ipsiloni);
                                if (!dictTackaElement.ContainsKey(mojaTacka))
                                {
                                    dictTackaElement.Add(mojaTacka, element);
                                    flag = true;
                                    break;
                                }
                            }
                            if (flag)
                                break;
                        }
                        if (flag)
                            break;

                        brojac++;
                    }
                }
                #endregion
                
                Canvas.SetBottom(rect, mojaTacka.X * 3);
                Canvas.SetLeft(rect, mojaTacka.Y * 3);
                canvas.Children.Add(rect);
            }
        }

        bool iscrtaoPresek = false;
        private void CrtajVodove()
        {
            
            foreach (LineEntity line in listaVodova)
            {
                Point start, end;
                pronadiTacke(line, out start, out end);

                
                if ((end.X == 1000 && end.Y == 1000) || (start.X == 1000 && start.Y == 1000))
                {
                    continue;
                }

                
                line.PocetakX = start.X;
                line.PocetakY = start.Y;
                line.KrajX = end.X;
                line.KrajY = end.Y;
                BFSProlaz(line, start, end);
            }

            foreach (LineEntity line in listaVodova)
            {
                Point start, end;
                pronadiTacke(line, out start, out end);

               
                if ((end.X == 1000 && end.Y == 1000) || (start.X == 1000 && start.Y == 1000))
                {
                    continue;
                }

               
                if (line.Prolaz != "1")
                {
                    BFSProlaz2(line, start, end);
                }

               
                line.Prolaz = "0";
            }
            iscrtaoPresek = true;
        }
        //pocetak algoritma BFS
        private void BFSProlaz(LineEntity line, Point start, Point end)
        {
            PozicijaPolja pocetak = new PozicijaPolja((int)line.PocetakX, (int)line.PocetakY);
            PozicijaPolja kraj = new PozicijaPolja((int)line.KrajX, (int)line.KrajY);
            //skladisti predjena polja u matricu prev,svako polje neposeceno sem pocetnog
            
            PozicijaPolja[,] prev = BFSPath.BFSPronadji(line, BFSprom.BFSlinije);

            List<PozicijaPolja> putanja = BFSPath.RekonstruisanjePutanje(pocetak, kraj, prev);

            
            if (putanja == null)
            {
                
                line.Prolaz = "0";
            }
            else
            {
                
                line.Prolaz = "1";

                Point p1 = new Point(pocetak.PozY * 3 + 1, -pocetak.PozX * 3 + 900 - 1);
                Point p3 = new Point(kraj.PozY * 3 + 1, -kraj.PozX * 3 + 900 - 1);
                putanja.Remove(pocetak);
                putanja.Remove(kraj);

                if (start.X != end.X)
                {

                    Polyline polyline = new Polyline();
                    polyline.Stroke = Brushes.Orchid;
                    polyline.StrokeThickness = 0.5;
                    polyline.ToolTip = "Line\nID: " + line.Id + " Name: " + line.Name;

                    polyline.MouseRightButtonDown += promeniBoju_MouseRightButtonDown;

                    PointCollection points = new PointCollection();
                    points.Add(p1);
                    foreach (PozicijaPolja zauzeto in putanja)
                    {
                        BFSprom.BFSlinije[zauzeto.PozX, zauzeto.PozY] = 'X';
                        points.Add(new Point(zauzeto.PozY * 3 + 1, -zauzeto.PozX * 3 + 900 - 1));
                    }
                    points.Add(p3);
                    polyline.Points = points;




                    canvas.Children.Add(polyline);
                }
            }
        }

        private void BFSProlaz2(LineEntity line, Point start, Point end)
        {
            PozicijaPolja pocetak = new PozicijaPolja((int)line.PocetakX, (int)line.PocetakY);
            PozicijaPolja kraj = new PozicijaPolja((int)line.KrajX, (int)line.KrajY);
            PozicijaPolja[,] prev = BFSPath.BFSPronadji(line, BFSprom.BFSlinije2);
            List<PozicijaPolja> putanja = BFSPath.RekonstruisanjePutanje(pocetak, kraj, prev);

            
            if (putanja == null)
            {
               
                line.Prolaz = "0";
            }
            else
            {
                
                line.Prolaz = "2";

                Point p1 = new Point(pocetak.PozY * 3 + 1, -pocetak.PozX * 3 + 900 - 1);
                Point p3 = new Point(kraj.PozY * 3 + 1, -kraj.PozX * 3 + 900 - 1);
                putanja.Remove(pocetak);
                putanja.Remove(kraj);

                if (start.X != end.X)
                {

                    Polyline polyline = new Polyline();
                    polyline.Stroke = Brushes.Orchid;
                    polyline.StrokeThickness = 0.5;
                    polyline.ToolTip = "Line\nID: " + line.Id + " Name: " + line.Name;

                    polyline.MouseRightButtonDown += promeniBoju_MouseRightButtonDown;

                    PointCollection points = new PointCollection();
                    points.Add(p1);
                    foreach (PozicijaPolja zauzeto in putanja)
                    {
                        if (BFSprom.BFSlinije[zauzeto.PozX, zauzeto.PozY] == 'X')
                        {
                            if (iscrtaoPresek == false)
                            {
                                Ellipse preklapanje = new Ellipse();
                                preklapanje.Height = 0.5;
                                preklapanje.Width = 0.5;
                                preklapanje.Fill = Brushes.Crimson;
                                Canvas.SetLeft(preklapanje, zauzeto.PozY * 3 + 0.5);
                                Canvas.SetTop(preklapanje, -zauzeto.PozX * 3 + 900 - 1.5);
                                canvas.Children.Add(preklapanje);
                            }
                        }
                        points.Add(new Point(zauzeto.PozY * 3 + 1, -zauzeto.PozX * 3 + 900 - 1));
                    }
                    points.Add(p3);
                    polyline.Points = points;






                    canvas.Children.Add(polyline);
                }
            }
        }

       
        private void promeniBoju_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            Polyline mojVod = (Polyline)sender;
            Point p1 = new Point();
            Point p2 = new Point();
            p1 = mojVod.Points.First();
            p2 = mojVod.Points.ElementAt(mojVod.Points.Count - 1);

            Rectangle r = new Rectangle();
            r.Fill = Brushes.Yellow;
            r.Width = 3;
            r.Height = 3;
            Canvas.SetBottom(r, 900 - 1.5 - p1.Y); 
            Canvas.SetLeft(r, -1.5 + p1.X);
            canvas.Children.Add(r);

            Rectangle r2 = new Rectangle();
            r2.Fill = Brushes.Yellow;
            r2.Width = 3;
            r2.Height = 3;
            Canvas.SetBottom(r2, 900 - 1.5 - p2.Y);
            Canvas.SetLeft(r2, -1.5 + p2.X);
            canvas.Children.Add(r2);
        }

        private void pronadiTacke(LineEntity le, out Point start, out Point end)
        {
            PowerEntity elem;

            elem = listaElemenataIzXML.Find(x => x.Id == le.FirstEnd);
           
            try
            {
                start = dictTackaElement.Where(x => x.Value == elem).First().Key;
            }
            catch (Exception ex)
            {
                start = new Point(1000, 1000);
            }

            elem = listaElemenataIzXML.Find(x => x.Id == le.SecondEnd);
            try
            {
                end = dictTackaElement.Where(x => x.Value == elem).First().Key;
            }
            catch (Exception ex)
            {
                end = new Point(1000, 1000);
            }
        }

        private void FindLatLon(double x, double y)
        {
            if (checkMinMax == 1)
            {
                praviXmax = noviX;
                praviYmax = noviY;
                praviXmin = noviX;
                praviYmin = noviY;

                checkMinMax++;
            }
            else
            {
                
                if (noviX > praviXmax)
                {
                    praviXmax = noviX;
                }

                if (noviY > praviYmax)
                {
                    praviYmax = noviY;
                }

              
                if (noviX < praviXmin)
                {
                    praviXmin = noviX;
                }

                if (noviY < praviYmin)
                {
                    praviYmin = noviY;
                }
            }
            odstojanjeLon = (praviXmax - praviXmin) / 300;
            odstojanjeLat = (praviYmax - praviYmin) / 300;
        }
        //koord.entiteta: tacke x i y se oduzmu od min.tacaka x i y koje se javljaju da bi se ustanovila
        //udaljenost od pocetne tacke a onda deli sa odstojanjem vec skalirane ose
        private void MestoNaCanvasu(double noviX, double noviY, out double relativnoX, out double relativnoY)
        {
           
            relativnoX = Math.Round((noviX - praviXmin) / odstojanjeLon);
            relativnoY = Math.Round((noviY - praviYmin) / odstojanjeLat);
        }

        private void Undo_Click(object sender, RoutedEventArgs e)
        {
            if (canvas.Children.Count > 0)
            {
                obrisaniListaZaBrojanje.Add(canvas.Children[canvas.Children.Count - 1]);
                canvas.Children.Remove(canvas.Children[canvas.Children.Count - 1]);
            }

            if (canvas.Children.Count != numberChildren)
            {
                for (int i = 0; i < ponovoIscrtaj.Count; i++)
                {
                    if (ponovoIscrtaj[i] != null)
                        canvas.Children.Add(ponovoIscrtaj[i]);
                }
            }

            for (int i = 0; i < ponovoIscrtaj.Count; i++)
            {
                ponovoIscrtaj[i] = null;
            }
        }

        private void Redo_Click(object sender, RoutedEventArgs e)
        {
            if (obrisaniListaZaBrojanje.Count > 0)
            {
               
                canvas.Children.Add(obrisaniListaZaBrojanje[obrisaniListaZaBrojanje.Count - 1]);
                obrisaniListaZaBrojanje.RemoveAt(obrisaniListaZaBrojanje.Count - 1);
            }
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
           
            if (canvas.Children.Count > 0)
            {
               
                foreach (UIElement jedanOdElemenata in canvas.Children)
                {
                    saMape.Add(jedanOdElemenata);
                }

               
                if (canvas.Children.Count > numberChildren)
                {
                    for (int i = numberChildren; i < canvas.Children.Count; i++)
                    {
                        ponovoIscrtaj.Add(canvas.Children[i]);
                    }
                }

               
                foreach (var item in ponovoIscrtaj)
                {
                    canvas.Children.Remove(item);
                }

               

                numberChildren = canvas.Children.Count;
            }
        }



        private void LeviPromeniNesto_Click(object sender, MouseButtonEventArgs e)
        {
           
            if (e.OriginalSource is Ellipse)
            {
                Ellipse ClickedRectangle = (Ellipse)e.OriginalSource;

                
                canvas.Children.Remove(ClickedRectangle);

                
                string bojaTeksta = "Black", samTekst = "nekiTekst";
                foreach (FrameworkElement item in canvas.Children)
                {
                    if (item.Name == ClickedRectangle.Name + "eltb") 
                    {
                        canvas.Children.Remove(item);
                        bojaTeksta = ((TextBlock)item).Foreground.ToString();
                        samTekst = ((TextBlock)item).Text;
                        break;
                    }
                }

                EditElipsa editElipsa = new EditElipsa(ClickedRectangle.Height, ClickedRectangle.Width, ClickedRectangle.StrokeThickness, ClickedRectangle.Fill, bojaTeksta, samTekst, ClickedRectangle.Opacity);
                editElipsa.Show();

            }
            else if (e.OriginalSource is Polygon)
            {
                Polygon ClickedRectangle = (Polygon)e.OriginalSource;

                canvas.Children.Remove(ClickedRectangle);

                
                string bojaTeksta = "Black", samTekst = "nekiTekst";
                foreach (FrameworkElement item in canvas.Children)
                {
                    if (item.Name == ClickedRectangle.Name + "pgtb") 
                    {
                        canvas.Children.Remove(item);
                        bojaTeksta = ((TextBlock)item).Foreground.ToString();
                        samTekst = ((TextBlock)item).Text;
                        break;
                    }
                }

                EditPoligon editPoligon = new EditPoligon(ClickedRectangle.StrokeThickness, ClickedRectangle.Fill.ToString(), bojaTeksta, samTekst, ClickedRectangle.Points, ClickedRectangle.Opacity);
                editPoligon.Show();
            }
            else if (e.OriginalSource is TextBlock)
            {
                TextBlock ClickedRectangle = (TextBlock)e.OriginalSource;

                string slova = ClickedRectangle.Name;
                slova = slova.Substring(8, slova.Length - 8);
                if (slova != "pgtb" && slova != "eltb")
                {
                    
                    canvas.Children.Remove(ClickedRectangle);
                    EditText editTekst = new EditText(ClickedRectangle.FontSize, ClickedRectangle.Foreground, ClickedRectangle.Text);
                    editTekst.Show();
                }
            }
        }

        private void LeviPoligon_Click(object sender, MouseButtonEventArgs e)
        {
            Poligon poligonCrtez = new Poligon();

           
            int i = 1;

            if (EllipseChecked.IsChecked == true && PolygonChecked.IsChecked == true || EllipseChecked.IsChecked == true && TextChecked.IsChecked == true ||
                EllipseChecked.IsChecked == true && PolygonChecked.IsChecked == true && TextChecked.IsChecked == true ||
                PolygonChecked.IsChecked == true && TextChecked.IsChecked == true)
            {
                i = 2;
                MessageBox.Show("Selektujte iskljucivo jednu opciju", "Greska!", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            if (i == 1 && PolygonChecked.IsChecked == true && koordinatePoX.Count >= 3)
            {
                poligonCrtez.Show();
            }
            else if (PolygonChecked.IsChecked == true)
            {
                MessageBox.Show("Morate izvrsiti desni klik bar 3 puta ako zelite da dodate nov poligon", "Greska!", MessageBoxButton.OK, MessageBoxImage.Information);
                koordinatePoX.Clear();
                koordinatePoY.Clear();
            }
        }

        private void Right_ClickBiloGde(object sender, MouseButtonEventArgs e)
        {
           
            int i = 1;

            if (EllipseChecked.IsChecked == true && PolygonChecked.IsChecked == true || EllipseChecked.IsChecked == true && TextChecked.IsChecked == true ||
                EllipseChecked.IsChecked == true && PolygonChecked.IsChecked == true && TextChecked.IsChecked == true ||
                PolygonChecked.IsChecked == true && TextChecked.IsChecked == true)
            {
                i = 2;
                MessageBox.Show("Selektujte iskljucivo jednu opciju", "Greska!", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            if (i == 1)
            {
                if (EllipseChecked.IsChecked == true)
                {
                    Elipsa elipsaCrtez = new Elipsa();

                   
                    poX = Mouse.GetPosition(canvas).X;
                    poY = Mouse.GetPosition(canvas).Y;

                    elipsaCrtez.Show();
                }
                else if (PolygonChecked.IsChecked == true)
                {
                    poX = Mouse.GetPosition(canvas).X;
                    poY = Mouse.GetPosition(canvas).Y;

                    koordinatePoX.Add(poX);
                    koordinatePoY.Add(poY);
                }
                else if (TextChecked.IsChecked == true)
                {
                    AddText dodajTekstCrtez = new AddText();

                    poX = Mouse.GetPosition(canvas).X;
                    poY = Mouse.GetPosition(canvas).Y;

                    dodajTekstCrtez.Show();
                }
            }
        }

        #region UcitavanjeElemenata
        private void UcitavanjeElemenata()
        {
            
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load("Geographic.xml"); 
            XmlNodeList nodeList;

            
            nodeList = xmlDoc.DocumentElement.SelectNodes("/NetworkModel/Substations/SubstationEntity");
            foreach (XmlNode node in nodeList)
            {
                SubstationEntity subEn = new SubstationEntity();
                subEn.Id = long.Parse(node.SelectSingleNode("Id").InnerText);
                subEn.Name = node.SelectSingleNode("Name").InnerText;
                subEn.X = double.Parse(node.SelectSingleNode("X").InnerText);
                subEn.Y = double.Parse(node.SelectSingleNode("Y").InnerText);
                subEn.ToolTip = "Substation\nID: " + subEn.Id + "  Name: " + subEn.Name;

                ToLatLon(subEn.X, subEn.Y, 34, out noviX, out noviY);
                FindLatLon(noviX, noviY);
                listaElemenataIzXML.Add(subEn);
            }

           
            nodeList = xmlDoc.DocumentElement.SelectNodes("/NetworkModel/Switches/SwitchEntity");
            foreach (XmlNode node in nodeList)
            {
                SwitchEntity sw = new SwitchEntity();
                sw.Id = long.Parse(node.SelectSingleNode("Id").InnerText);
                sw.Name = node.SelectSingleNode("Name").InnerText;
                sw.X = double.Parse(node.SelectSingleNode("X").InnerText);
                sw.Y = double.Parse(node.SelectSingleNode("Y").InnerText);
                sw.Status = node.SelectSingleNode("Status").InnerText;
                sw.ToolTip = "Switch\nID: " + sw.Id + "  Name: " + sw.Name + " Status: " + sw.Status;

                ToLatLon(sw.X, sw.Y, 34, out noviX, out noviY);
                FindLatLon(noviX, noviY);
                listaElemenataIzXML.Add(sw);
                listaSviceva.Add(sw);
            }

            
            nodeList = xmlDoc.DocumentElement.SelectNodes("/NetworkModel/Nodes/NodeEntity");
            foreach (XmlNode node in nodeList)
            {
                NodeEntity nod = new NodeEntity();
                nod.Id = long.Parse(node.SelectSingleNode("Id").InnerText);
                nod.Name = node.SelectSingleNode("Name").InnerText;
                nod.X = double.Parse(node.SelectSingleNode("X").InnerText);
                nod.Y = double.Parse(node.SelectSingleNode("Y").InnerText);
                nod.ToolTip = "Node\nID: " + nod.Id + "  Name: " + nod.Name;

                ToLatLon(nod.X, nod.Y, 34, out noviX, out noviY);
                FindLatLon(noviX, noviY);
                listaElemenataIzXML.Add(nod);
            }

            
            nodeList = xmlDoc.DocumentElement.SelectNodes("/NetworkModel/Lines/LineEntity");
            foreach (XmlNode node in nodeList)
            {
                LineEntity l = new LineEntity();
                l.Id = long.Parse(node.SelectSingleNode("Id").InnerText);
                l.Name = node.SelectSingleNode("Name").InnerText;
                if (node.SelectSingleNode("IsUnderground").InnerText.Equals("true"))
                {
                    l.IsUnderground = true;
                }
                else
                {
                    l.IsUnderground = false;
                }
                l.R = float.Parse(node.SelectSingleNode("R").InnerText);
                l.ConductorMaterial = node.SelectSingleNode("ConductorMaterial").InnerText;
                l.LineType = node.SelectSingleNode("LineType").InnerText;
                l.ThermalConstantHeat = long.Parse(node.SelectSingleNode("ThermalConstantHeat").InnerText);
                l.FirstEnd = long.Parse(node.SelectSingleNode("FirstEnd").InnerText);
                l.SecondEnd = long.Parse(node.SelectSingleNode("SecondEnd").InnerText);

                if (listaElemenataIzXML.Any(x => x.Id == l.FirstEnd))
                {
                    if (listaElemenataIzXML.Any(x => x.Id == l.SecondEnd))
                    {
                        listaVodova.Add(l);
                    }
                }

               
                while (listaVodova.Any(x => x.Id != l.Id && x.FirstEnd == l.FirstEnd && x.SecondEnd == l.SecondEnd))
                {
                    vodDuplikat = listaVodova.FindAll(x => x.Id != l.Id && x.FirstEnd == l.FirstEnd && x.SecondEnd == l.SecondEnd);
                    foreach (LineEntity dupli in vodDuplikat)
                    {
                        listaVodova.Remove(dupli);
                    }
                    vodDuplikat.Clear();
                }
            }
        }
        #endregion

        #region ToLatLon
      
        public static void ToLatLon(double utmX, double utmY, int zoneUTM, out double latitude, out double longitude)
        {
            bool isNorthHemisphere = true;

            var diflat = -0.00066286966871111111111111111111111111;
            var diflon = -0.0003868060578;

            var zone = zoneUTM;
            var c_sa = 6378137.000000;
            var c_sb = 6356752.314245;
            var e2 = Math.Pow((Math.Pow(c_sa, 2) - Math.Pow(c_sb, 2)), 0.5) / c_sb;
            var e2cuadrada = Math.Pow(e2, 2);
            var c = Math.Pow(c_sa, 2) / c_sb;
            var x = utmX - 500000;
            var y = isNorthHemisphere ? utmY : utmY - 10000000;

            var s = ((zone * 6.0) - 183.0);
            var lat = y / (c_sa * 0.9996);
            var v = (c / Math.Pow(1 + (e2cuadrada * Math.Pow(Math.Cos(lat), 2)), 0.5)) * 0.9996;
            var a = x / v;
            var a1 = Math.Sin(2 * lat);
            var a2 = a1 * Math.Pow((Math.Cos(lat)), 2);
            var j2 = lat + (a1 / 2.0);
            var j4 = ((3 * j2) + a2) / 4.0;
            var j6 = ((5 * j4) + Math.Pow(a2 * (Math.Cos(lat)), 2)) / 3.0;
            var alfa = (3.0 / 4.0) * e2cuadrada;
            var beta = (5.0 / 3.0) * Math.Pow(alfa, 2);
            var gama = (35.0 / 27.0) * Math.Pow(alfa, 3);
            var bm = 0.9996 * c * (lat - alfa * j2 + beta * j4 - gama * j6);
            var b = (y - bm) / v;
            var epsi = ((e2cuadrada * Math.Pow(a, 2)) / 2.0) * Math.Pow((Math.Cos(lat)), 2);
            var eps = a * (1 - (epsi / 3.0));
            var nab = (b * (1 - epsi)) + lat;
            var senoheps = (Math.Exp(eps) - Math.Exp(-eps)) / 2.0;
            var delt = Math.Atan(senoheps / (Math.Cos(nab)));
            var tao = Math.Atan(Math.Cos(delt) * Math.Tan(nab));

            longitude = ((delt * (180.0 / Math.PI)) + s) + diflon;
            latitude = ((lat + (1 + e2cuadrada * Math.Pow(Math.Cos(lat), 2) - (3.0 / 2.0) * e2cuadrada * Math.Sin(lat) * Math.Cos(lat) * (tao - lat)) * (tao - lat)) * (180.0 / Math.PI)) + diflat;
        }
        #endregion
    }
}
