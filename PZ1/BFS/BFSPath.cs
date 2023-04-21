using PZ1.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PZ1.BFS
{

    public class BFSPath
    {
        //u red se smesta polje koje je pocetak i pocinje pretraga
        public static PozicijaPolja[,] BFSPronadji(LineEntity line, char[,] BFSlinije)
        {
            BFSlinije[(int)line.PocetakX, (int)line.PocetakY] = 'P';
            BFSlinije[(int)line.KrajX, (int)line.KrajY] = 'K';

            PozicijaPolja pocetak = new PozicijaPolja((int)line.PocetakX, (int)line.PocetakY);
            Queue<PozicijaPolja> queue = new Queue<PozicijaPolja>();
            queue.Enqueue(pocetak);

            bool[,] poseceno = new bool[301, 301]; 
            for (int i = 0; i <= 300; i++)
            {
                for (int j = 0; j <= 300; j++)
                {
                    poseceno[i, j] = false;
                }
            }
            poseceno[pocetak.PozX, pocetak.PozY] = true;

            PozicijaPolja[,] prev = new PozicijaPolja[301, 301];
            for (int i = 0; i <= 300; i++)
            {
                for (int j = 0; j <= 300; j++)
                {
                    prev[i, j] = null;
                }
            }

            while (queue.Count != 0)
            {
                PozicijaPolja polje = queue.Dequeue();

               //za svako polje koje se nadje u redu se gleda tacka koja je iznad,ispod,levo,desno
                if (BFSlinije[polje.PozX, polje.PozY] == 'K')
                {
                    BFSlinije[(int)line.PocetakX, (int)line.PocetakY] = ' ';
                    BFSlinije[(int)line.KrajX, (int)line.KrajY] = ' ';
                    return prev;
                }

                if (isValid(polje.PozX - 1, polje.PozY, poseceno, BFSlinije))
                {
                    queue.Enqueue(new PozicijaPolja(polje.PozX - 1, polje.PozY));
                    poseceno[polje.PozX - 1, polje.PozY] = true;
                    prev[polje.PozX - 1, polje.PozY] = polje;
                }

                if (isValid(polje.PozX + 1, polje.PozY, poseceno, BFSlinije))
                {
                    queue.Enqueue(new PozicijaPolja(polje.PozX + 1, polje.PozY));
                    poseceno[polje.PozX + 1, polje.PozY] = true;
                    prev[polje.PozX + 1, polje.PozY] = polje;
                }

                if (isValid(polje.PozX, polje.PozY - 1, poseceno, BFSlinije))
                {
                    queue.Enqueue(new PozicijaPolja(polje.PozX, polje.PozY - 1));
                    poseceno[polje.PozX, polje.PozY - 1] = true;
                    prev[polje.PozX, polje.PozY - 1] = polje;
                }

                if (isValid(polje.PozX, polje.PozY + 1, poseceno, BFSlinije))
                {
                    queue.Enqueue(new PozicijaPolja(polje.PozX, polje.PozY + 1));
                    poseceno[polje.PozX, polje.PozY + 1] = true;
                    prev[polje.PozX, polje.PozY + 1] = polje;
                }
            }
            BFSlinije[(int)line.PocetakX, (int)line.PocetakY] = ' ';
            BFSlinije[(int)line.KrajX, (int)line.KrajY] = ' ';
            return prev;
        }
        //provera je l polje izvan dimenzija 300x300 ili je vec poseceno
        //pretraga traje dok se ne dodje do polja koje je kraj voda
        private static bool isValid(int pozX, int pozY, bool[,] poseceno, char[,] bFSlinije)
        {
            if (pozX >= 0 && pozY >= 0 && pozX < 300 && pozY < 300 && bFSlinije[pozX, pozY] != 'X' && poseceno[pozX, pozY] == false)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        //sledi rekonstruisanje putanje koje predstavlja vod
        //Krece od poslednje tacke i vraca korak unazad dok ne dodje do pocetne tacke
        public static List<PozicijaPolja> RekonstruisanjePutanje(PozicijaPolja pocetak, PozicijaPolja kraj, PozicijaPolja[,] prev)
        {
            List<PozicijaPolja> putanja = new List<PozicijaPolja>();
            PozicijaPolja polje;

            for (polje = kraj; polje != null; polje = prev[polje.PozX, polje.PozY])
            {
                putanja.Add(polje);
            }
            putanja.Reverse();

            if (putanja[0].PozX == pocetak.PozX && putanja[0].PozY == pocetak.PozY)
            {
                return putanja;
            }
            else
            {
                return null;
            }
        }
    }
}
