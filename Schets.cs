using System;
using System.Collections.Generic;
using System.Drawing;

public class Schets
{
    private Bitmap bitmap;

    public Schets()
    {
        bitmap = new Bitmap(1, 1);
    }
    public Graphics BitmapGraphics
    {
        get { return Graphics.FromImage(bitmap); }
    }
    public void VeranderAfmeting(Size sz)
    {
        if (sz.Width > bitmap.Size.Width || sz.Height > bitmap.Size.Height)
        {
            Bitmap nieuw = new Bitmap(Math.Max(sz.Width, bitmap.Size.Width)
                                     , Math.Max(sz.Height, bitmap.Size.Height)
                                     );
            Graphics gr = Graphics.FromImage(nieuw);
            gr.FillRectangle(Brushes.White, 0, 0, sz.Width, sz.Height);
            gr.DrawImage(bitmap, 0, 0);
            bitmap = nieuw;
        }
    }
    public void Teken(Graphics gr)
    {
        gr.DrawImage(bitmap, 0, 0);
    }
    public void Schoon()
    {
        Graphics gr = Graphics.FromImage(bitmap);
        gr.FillRectangle(Brushes.White, 0, 0, bitmap.Width, bitmap.Height);
    }
    public void Roteer()
    {
        bitmap.RotateFlip(RotateFlipType.Rotate90FlipNone);
    }

    /////////////////////////////////////////////////////////////////
    public void TekenActie(Graphics gr, SchetsControl s)
    {
        
    }
    
    public void AllesTekenen(Graphics gr, SchetsControl s)
    {
        Graphics g = Graphics.FromImage(bitmap);
        g.Clear(Color.White);

        Point startpunt;
        Point eindpunt;
        Brush kwast;

        //elk elementen apart tekenen, en niet als geheel 1 bitmap
        foreach (var actie in s.acties)
        {
            int n = 0;
            n++;
            startpunt = s.acties[n].;
            TekenActie(gr, s);
        }
    }

    /////////////////////////////////////////////////////////////////
}