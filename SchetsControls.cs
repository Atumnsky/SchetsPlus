using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Security.Cryptography;
using System.Windows.Forms;

public class SchetsControl : UserControl
{
    private Schets schets;
    private Color penkleur;

    /////////////////////////////////////////////////////////////////
    private TweepuntTool previewTool;
    private Point previewPunt;
    /////////////////////////////////////////////////////////////////
    public Color PenKleur
    {
        get { return penkleur; }
    }
    public Schets Schets
    {
        get { return schets; }
    }
    public SchetsControl()
    {
        this.BorderStyle = BorderStyle.Fixed3D;
        this.schets = new Schets();

        this.DoubleBuffered = true; // meer smooth
        //this.Paint += this.teken;
        //this.Resize += this.veranderAfmeting;
        //this.veranderAfmeting(null, null);
    }
    /////////////////////////////////////////////////////////////////
    public void ZetPreview(TweepuntTool tool, Point p)
    {
        previewTool = tool;
        previewPunt = p;
        Invalidate(true);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        e.Graphics.Clear(Color.White);
        base.OnPaint(e);
        
        schets.AllesTekenen(e.Graphics);

        if (previewTool != null)
            previewTool.TekenPreview(e.Graphics, previewPunt);
    }

    //protected override void OnPaintBackground(PaintEventArgs e)
    //{
    //    e.Graphics.Clear(Color.White);
    //}
    //private void teken(object o, PaintEventArgs pea)
    //{ 
    //    schets.AllesTekenen(pea.Graphics);

    //    if (previewTool != null)
    //    {
    //        previewTool.TekenPreview(pea.Graphics, previewPunt);
    //    }

    //}    
    /////////////////////////////////////////////////////////////////
    //private void veranderAfmeting(object o, EventArgs ea)
    //{
    //    schets.VeranderAfmeting(this.ClientSize);
    //    this.Invalidate();
    //}
    //public Graphics MaakBitmapGraphics()
    //{
    //    Graphics g = schets.BitmapGraphics;
    //    g.SmoothingMode = SmoothingMode.AntiAlias;
    //    return g;
    //}
    public void Roteer(object o, EventArgs ea)
    {
        schets.Roteer(this.ClientSize);
        Invalidate(true);
    }

    public void Schoon(object o, EventArgs ea)
    {
        schets.Schoon();
        this.Invalidate(true);
    }
    //public void Roteer(object o, EventArgs ea)
    //{
    //    schets.VeranderAfmeting(new Size(this.ClientSize.Height, this.ClientSize.Width));
    //    schets.Roteer();
    //    this.Invalidate();
    //}
    public void VeranderKleur(object obj, EventArgs ea)
    {
        string kleurNaam = ((ComboBox)obj).Text;
        penkleur = Color.FromName(kleurNaam);
    }
    public void VeranderKleurViaMenu(object obj, EventArgs ea)
    {
        string kleurNaam = ((ToolStripMenuItem)obj).Text;
        penkleur = Color.FromName(kleurNaam);
        
    }
}