using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

public class Schets
{
    //private Bitmap bitmap;
    /////////////////////////////////////////////////////////////////
    private List<ISchetsElement> elementen = new List<ISchetsElement>();
    /////////////////////////////////////////////////////////////////
    //public Schets()
    //{
    //    bitmap = new Bitmap(1, 1);
    //}

    //public Graphics BitmapGraphics
    //{
    //    get { return Graphics.FromImage(bitmap); }
    //}
    //public void VeranderAfmeting(Size sz)
    //{
    //    if (sz.Width > bitmap.Size.Width || sz.Height > bitmap.Size.Height)
    //    {
    //        Bitmap nieuw = new Bitmap(Math.Max(sz.Width, bitmap.Size.Width)
    //                                 , Math.Max(sz.Height, bitmap.Size.Height)
    //                                 );
    //        Graphics gr = Graphics.FromImage(nieuw);
    //        gr.FillRectangle(Brushes.White, 0, 0, sz.Width, sz.Height);
    //        gr.DrawImage(bitmap, 0, 0);
    //        bitmap = nieuw;
    //    }
    //}
    //public void Teken(Graphics gr)
    //{
    //    gr.DrawImage(bitmap, 0, 0);
    //}
    //public void Schoon()
    //{
    //    Graphics gr = Graphics.FromImage(bitmap);
    //    gr.FillRectangle(Brushes.White, 0, 0, bitmap.Width, bitmap.Height);
    //}
    //public void Roteer()
    //{
    //    bitmap.RotateFlip(RotateFlipType.Rotate90FlipNone);
    //}

    /////////////////////////////////////////////////////////////////
    public void Roteer(Size canvas)
    {
        foreach (var e in elementen)
            e.Roteer(canvas);
    }
    
    public void VoegToe(ISchetsElement e)
    {
        elementen.Add(e);
    }

    public void AllesTekenen(Graphics g)
    {
        foreach(ISchetsElement e in elementen)
            e.Teken(g);

    }

    public void VerwijderElementOp(Point p)
    {
        // van achter naar voren tellen omdat de nieuwe element achterin worden toegevoegd
        for (int i = elementen.Count - 1; i >= 0; i--)
        {
            if (elementen[i].Raak(p))
            {
                elementen.RemoveAt(i);
                break;
            }
        }
    }
    
    public void Schoon()
    {
        elementen.Clear();
    }

    public Bitmap NaarBitmap(Size size)
    {
        Bitmap bmp = new Bitmap(size.Width, size.Height);
        using (Graphics g = Graphics.FromImage(bmp))
        {
            g.Clear(Color.White);
            AllesTekenen(g);
        }

        return bmp;
    }

    public void Opslaan(string Schets)
    {
        using (StreamWriter sw = new StreamWriter(Schets))
        {
            foreach (var e in elementen)
                sw.WriteLine(e.ElementInformatie());
        }
    }

    public void Openen(string Schets)
    {
        elementen.Clear();

        StreamReader sr = new StreamReader(Schets); 
        string regel;

        while((regel = sr.ReadLine()) != null)
        {
            string[] s = regel.Split(';');
            string tool = s[0];

            if (tool == "Rechthoek")
            {
                VoegToe(new RechthoekElement(
                    new Point(int.Parse(s[1]), int.Parse(s[2])),
                    new Point(int.Parse(s[3]), int.Parse(s[4])),
                    new SolidBrush(Color.FromName(s[5]))
                ));
            }
            else if (tool == "VolRechthoek")
            {
                VoegToe(new VolRechthoekElement(
                    new Point(int.Parse(s[1]), int.Parse(s[2])),
                    new Point(int.Parse(s[3]), int.Parse(s[4])),
                    new SolidBrush(Color.FromName(s[5]))
                ));
            }
            else if (tool == "Cirkel")
            {
                VoegToe(new CirkelElement(
                    new Point(int.Parse(s[1]), int.Parse(s[2])),
                    new Point(int.Parse(s[3]), int.Parse(s[4])),
                    new SolidBrush(Color.FromName(s[5])) 
                ));
            }
            else if (tool == "VolCirkel")
            {
                VoegToe(new VolCirkelElement(
                    new Point(int.Parse(s[1]), int.Parse(s[2])),
                    new Point(int.Parse(s[3]), int.Parse(s[4])),
                    new SolidBrush(Color.FromName(s[5]))
                ));
            }
            else if (tool == "Lijn")
            {
                VoegToe(new LijnElement(
                    new Point(int.Parse(s[1]), int.Parse(s[2])),
                    new Point(int.Parse(s[3]), int.Parse(s[4])),
                    new SolidBrush(Color.FromName(s[5]))
                ));
            }

            else if (tool == "Tekst")
            {
                TekstElement t = new TekstElement(
                    new Point(int.Parse(s[1]), int.Parse(s[2])),
                    new SolidBrush(Color.FromName(s[5]))
                );

                foreach (char c in s[4])
                    t.VoegLetterToe(c);

                float hoek = float.Parse(s[3]);
                while (hoek > 0)
                {
                    t.Roteer(new Size(0, 0));
                    hoek -= 90;
                }

                VoegToe(t);
            }

            else if (tool == "Pen")
            {
                Brush kwast = new SolidBrush(Color.FromName(s[s.Length - 1]));
                PenElement pen = new PenElement(kwast);
                                                    //De laatste lege string weghalen
                string[] punten = s[1].Split('|', StringSplitOptions.RemoveEmptyEntries);
                foreach (string pt in punten)
                {
                    string[] xy = pt.Split(',');
                    pen.VoegPuntToe(new Point(int.Parse(xy[0]), int.Parse(xy[1])));
                }

                VoegToe(pen);
            }
        }
    }
    
    /////////////////////////////////////////////////////////////////
}