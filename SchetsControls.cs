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

     
    private TweepuntTool previewTool;
    private Point previewPunt;
     
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

        this.DoubleBuffered = true;
    }
     
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
    public void VeranderKleur(object obj, EventArgs ea)
    {
        string kleurNaam = ((ComboBox)obj).Text;
        penkleur = Color.FromName(kleurNaam);
    }
    public void VeranderKleurViaMenu(object obj, EventArgs ea)
    {
        string kleurNaam = ((ToolStripMenuItem)obj).Text;
        penkleur = Color.FromName(kleurNaam);

        //Keuze van de combobox synchroniseren
        if (this.Parent is SchetsWin win && win.cbb != null)
            win.cbb.SelectedItem = kleurNaam;
        
    }
}