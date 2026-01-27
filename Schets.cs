using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

public class Schets
{
     
    private List<ISchetsElement> elementen = new List<ISchetsElement>();

    public bool IsGewijzigd = false;
    public void Roteer(Size canvas)
    {
        foreach (var e in elementen)
            e.Roteer(canvas);

        IsGewijzigd = true;
    }
    
    public void VoegToe(ISchetsElement e)
    {
        elementen.Add(e);
        IsGewijzigd = true;
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
                IsGewijzigd = true;
                break;
            }
        }
    }
    
    public void Schoon()
    {
        elementen.Clear();
        IsGewijzigd = true;
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

        IsGewijzigd = false;
    }
    //using om de stream te sluiten
    public void Openen(string Schets)
    {
        elementen.Clear();

        StreamReader sr = new StreamReader(Schets); 
        string regel;

        using (sr)
        {
            while ((regel = sr.ReadLine()) != null)
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
                    int x = int.Parse(s[1]);
                    int y = int.Parse(s[2]);
                    float hoek = float.Parse(s[3]);
                    string tekst = s[4];
                    Color kleur = Color.FromName(s[5]);


                    VoegToe(new TekstElement(new Point(x, y), new SolidBrush(kleur), tekst, hoek));
                }

                else if (tool == "Pen")
                {
                    string puntenstring = s[1];
                    string kleurnaam =s[2];

                    Brush kwast = new SolidBrush(Color.FromName(kleurnaam));
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
        IsGewijzigd = false;
    }


}