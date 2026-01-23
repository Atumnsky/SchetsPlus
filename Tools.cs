using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;
using System.Windows.Forms;

public interface ISchetsTool
{
    void MuisVast(SchetsControl s, Point p);
    void MuisDrag(SchetsControl s, Point p);
    void MuisLos(SchetsControl s, Point p);
    void Letter(SchetsControl s, char c);
}
/////////////////////////////////////////////////////////////////
public interface ISchetsElement
{
    void Teken(Graphics g);
    bool Raak(Point p);
    void Roteer(Size canvas);
}
/////////////////////////////////////////////////////////////////
public abstract class StartpuntTool : ISchetsTool
{
    protected Point startpunt;
    protected Point eindpunt;
    protected Brush kwast;

    public virtual void MuisVast(SchetsControl s, Point p)
    {
        startpunt = p;
    }
    public virtual void MuisLos(SchetsControl s, Point p)
    {
        kwast = new SolidBrush(s.PenKleur);
        eindpunt = p;
    }
    public abstract void MuisDrag(SchetsControl s, Point p);
    public abstract void Letter(SchetsControl s, char c);
}

public class TekstTool : StartpuntTool
{
    public override string ToString() { return "tekst"; }

    public override void MuisDrag(SchetsControl s, Point p) { }

    public override void Letter(SchetsControl s, char c)
    {
        if (c >= 32)
        {/////////////////////////////////////////////////////////////////
            kwast = new SolidBrush(s.PenKleur);
            s.Schets.VoegToe(new TekstElement(c.ToString(), startpunt, kwast));
            Font font = new Font("Tahoma", 40);
            Size sz = TextRenderer.MeasureText(c.ToString(), font);
            startpunt.X += sz.Width;
            s.Invalidate();
         /////////////////////////////////////////////////////////////////
        }
    }
}

public abstract class TweepuntTool : StartpuntTool
{
    public static Rectangle Punten2Rechthoek(Point p1, Point p2)
    {
        return new Rectangle(new Point(Math.Min(p1.X, p2.X), Math.Min(p1.Y, p2.Y))
                            , new Size(Math.Abs(p1.X - p2.X), Math.Abs(p1.Y - p2.Y))
                            );
    }


    public static Pen MaakPen(Brush b, int dikte)
    {
        Pen pen = new Pen(b, dikte);
        pen.StartCap = LineCap.Round;
        pen.EndCap = LineCap.Round;
        return pen;
    }
    public override void MuisVast(SchetsControl s, Point p)
    {
        base.MuisVast(s, p);
        kwast = Brushes.Gray;
    }
    public override void MuisDrag(SchetsControl s, Point p)
    {
        s.ZetPreview(this, p);
    }
    public override void MuisLos(SchetsControl s, Point p)
    {
        base.MuisLos(s, p);
        s.ZetPreview(null, Point.Empty);
        s.Invalidate(true);
    }
    public override void Letter(SchetsControl s, char c)
    {
    }
    public abstract void Bezig(Graphics g, Point p1, Point p2);

    //public virtual void Compleet(Graphics g, Point p1, Point p2)
    //{
    //    this.Bezig(g, p1, p2);
    //}

    /////////////////////////////////////////////////////////////////
    public void TekenPreview(Graphics g, Point huidigPunt)
    {
        Bezig(g, startpunt, huidigPunt);
    }
    /////////////////////////////////////////////////////////////////
}

/////////////////////////////////////////////////////////////////
public class RechthoekTool : TweepuntTool
{
    public override string ToString() { return "kader"; }

    public override void Bezig(Graphics g, Point p1, Point p2)
    {
        g.DrawRectangle(MaakPen(kwast, 3), TweepuntTool.Punten2Rechthoek(p1, p2));
    }

    public override void MuisLos(SchetsControl s, Point p)
    {
        kwast = new SolidBrush(s.PenKleur);
        eindpunt = p;
        s.ZetPreview(null, Point.Empty);
        s.Schets.VoegToe(new RechthoekElement(startpunt, eindpunt, kwast));
        s.Invalidate(true);
    }
}

public class VolRechthoekTool : RechthoekTool
{
    public override string ToString() { return "vlak"; }

    //public override void Compleet(Graphics g, Point p1, Point p2)
    //{
    //    g.FillRectangle(kwast, TweepuntTool.Punten2Rechthoek(p1, p2));
    //}

    public override void MuisLos(SchetsControl s, Point p)
    {
        kwast = new SolidBrush(s.PenKleur);
        eindpunt = p;
        s.ZetPreview(null, Point.Empty);
        s.Schets.VoegToe(new VolRechthoekElement(startpunt, eindpunt, kwast));
        s.Invalidate(true);
    }
}

public class CirkelTool : TweepuntTool
{
    public override string ToString() { return "cirkel"; }

    public override void Bezig(Graphics g, Point p1, Point p2)
    {
        g.DrawEllipse(MaakPen(kwast, 3), TweepuntTool.Punten2Rechthoek(p1, p2));
    }

    public override void MuisLos(SchetsControl s, Point p)
    {
        kwast = new SolidBrush(s.PenKleur);
        eindpunt = p;
        s.ZetPreview(null, Point.Empty);
        s.Schets.VoegToe(new CirkelElement(startpunt, eindpunt, kwast));
        s.Invalidate(true);
    }
}

public class VolCirkelTool : CirkelTool
{
    public override string ToString() { return "schijf"; }

    //public override void Compleet(Graphics g, Point p1, Point p2)
    //{
    //    g.FillEllipse(kwast, TweepuntTool.Punten2Rechthoek(p1, p2));
    //}

    public override void MuisLos(SchetsControl s, Point p)
    {
        kwast = new SolidBrush(s.PenKleur);
        eindpunt = p;
        s.ZetPreview(null, Point.Empty);
        s.Schets.VoegToe(new VolCirkelElement(startpunt, eindpunt, kwast));
        s.Invalidate(true);
    }
}
public class LijnTool : TweepuntTool
{
    public override string ToString() { return "lijn"; }

    public override void Bezig(Graphics g, Point p1, Point p2)
    {
    g.DrawLine(MaakPen(this.kwast, 3), p1, p2);
    }

    public override void MuisLos(SchetsControl s, Point p)
    {
        kwast = new SolidBrush(s.PenKleur);
        eindpunt = p;
        s.ZetPreview(null, Point.Empty);
        s.Schets.VoegToe(new LijnElement(startpunt, eindpunt, kwast));
        s.Invalidate(true);
    }

}

public class PenTool : LijnTool
{
    private PenElement huidig;
    public override string ToString() { return "pen"; }

    public override void MuisVast(SchetsControl s, Point p)
    {
        kwast = new SolidBrush(s.PenKleur);
        huidig = new PenElement(kwast);

        huidig.VoegPuntToe(p);
        s.Schets.VoegToe(huidig);
    }

    public override void MuisDrag(SchetsControl s, Point p)
    {
        huidig.VoegPuntToe(p);
        s.Invalidate(true);
    }

    public override void MuisLos(SchetsControl s, Point p)
    {
    }

    public override void Letter(SchetsControl s, char c)
    {
    }
}
/////////////////////////////////////////////////////////////////

/////////////////////////////////////////////////////////////////
public class GumTool : ISchetsTool
{
    public override string ToString() { return "gum"; }

    public void MuisVast(SchetsControl s, Point p)
    {
        s.Schets.VerwijderElementOp(p);
        s.Invalidate(true);
    }

    public void MuisDrag(SchetsControl s, Point p)
    {
        s.Schets.VerwijderElementOp(p);
        s.Invalidate(true);
    }

    public void MuisLos(SchetsControl s, Point p) { }
    public void Letter(SchetsControl s, Char c) { }
    
}
/////////////////////////////////////////////////////////////////

/////////////////////////////////////////////////////////////////
//// De getekende obect als elementen opslaan
public class RechthoekElement : ISchetsElement
{
    private Point p1, p2;
    private Brush kwast;

    public RechthoekElement(Point p1, Point p2, Brush kwast)
    {
        this.p1 = p1;
        this.p2 = p2;
        this.kwast = kwast;
    }

    private Point RoteerPunt(Point p, Size s)
    {
        return new Point(s.Height - p.Y, p.X);
    }
    public void Roteer(Size canvas)
    {
        p1 = RoteerPunt(p1, canvas);
        p2 = RoteerPunt(p2, canvas);
    }

    public void Teken(Graphics g)
    {
        g.DrawRectangle(TweepuntTool.MaakPen(kwast, 3), TweepuntTool.Punten2Rechthoek(p1, p2));
    }

    public bool Raak(Point p)
    {
        Rectangle r = TweepuntTool.Punten2Rechthoek(p1, p2);
        int hitBox = 5;

        bool boven = Math.Abs(p.Y - r.Top) <= hitBox && p.X >= r.Left - hitBox && p.X <= r.Right + hitBox;
        bool onder = Math.Abs(p.Y - r.Bottom) <= hitBox && p.X >= r.Left - hitBox && p.X <= r.Right + hitBox;
        bool links = Math.Abs(p.X - r.Left) <= hitBox && p.Y >= r.Top - hitBox && p.Y <= r.Bottom + hitBox;
        bool rechts = Math.Abs(p.X - r.Right) <= hitBox && p.Y >= r.Top - hitBox && p.Y <= r.Bottom + hitBox;

        return boven || onder || links || rechts;
    }
}
public class VolRechthoekElement : ISchetsElement
{
    private Rectangle rect;
    private Brush kwast;

    public VolRechthoekElement(Point p1, Point p2, Brush kwast)
    {
        rect = TweepuntTool.Punten2Rechthoek(p1, p2);
        this.kwast = kwast;
    }
    private Point RoteerPunt(Point p, Size s)
    {
        return new Point(s.Height - p.Y, p.X);
    }
    public void Roteer(Size canvas)
    {
        Point p1 = new Point(rect.Left, rect.Top);
        Point p2 = new Point(rect.Right, rect.Bottom);

        p1 = RoteerPunt(p1, canvas);
        p2 = RoteerPunt(p2, canvas);

        rect = Rectangle.FromLTRB(Math.Min(p1.X, p2.X), Math.Min(p1.Y, p2.Y), Math.Max(p1.X, p2.X), Math.Max(p1.Y, p2.Y));
    }

    public void Teken(Graphics g)
    {
        g.FillRectangle(kwast, rect);
    }

    public bool Raak(Point p)
    {
        return rect.Contains(p);
    }
}

public class CirkelElement : ISchetsElement
{
    private Rectangle rect;
    private Brush kwast;

    public CirkelElement(Point p1, Point p2, Brush kwast)
    {
        rect = TweepuntTool.Punten2Rechthoek(p1, p2);
        this.kwast = kwast;
    }
    private Point RoteerPunt(Point p, Size s)
    {
        return new Point(s.Height - p.Y, p.X);
    }
    public void Roteer(Size canvas)
    {
        Point p1 = new Point(rect.Left, rect.Top);
        Point p2 = new Point(rect.Right, rect.Bottom);

        p1 = RoteerPunt(p1, canvas);
        p2 = RoteerPunt(p2, canvas);

        rect = Rectangle.FromLTRB(Math.Min(p1.X, p2.X), Math.Min(p1.Y, p2.Y), Math.Max(p1.X, p2.X), Math.Max(p1.Y, p2.Y));
    }

    public void Teken(Graphics g)
    {
        g.DrawEllipse(TweepuntTool.MaakPen(kwast,3), rect);
    }

    public bool Raak(Point p)
    {
        //https://stackoverflow.com/questions/59971407/how-can-i-test-if-a-point-is-in-an-ellipse

        //middelpunt ellipse
        float centerX = rect.Left + rect.Width / 2f;
        float centerY = rect.Top + rect.Height / 2f;

        //1/2 hoogte
        float a = rect.Width / 2f;
        float b = rect.Height / 2f;
        
        float dx = p.X - centerX;
        float dy = p.Y - centerY;

        float ellipsRaak = (dx*dx) / (a*a) + (dy*dy) / (b*b);

        float hitBox = 0.15f;

        return Math.Abs(ellipsRaak - 1) <= hitBox;

    }
}

public class VolCirkelElement : ISchetsElement
{
    private Rectangle rect;
    private Brush kwast;

    public VolCirkelElement(Point p1, Point p2, Brush kwast)
    {
        rect = TweepuntTool.Punten2Rechthoek(p1, p2);
        this.kwast = kwast;
    }
    private Point RoteerPunt(Point p, Size s)
    {
        return new Point(s.Height - p.Y, p.X);
    }
    public void Roteer(Size canvas)
    {
        Point p1 = new Point(rect.Left, rect.Top);
        Point p2 = new Point(rect.Right, rect.Bottom);

        p1 = RoteerPunt(p1, canvas);
        p2 = RoteerPunt(p2, canvas);

        rect = Rectangle.FromLTRB(Math.Min(p1.X, p2.X), Math.Min(p1.Y, p2.Y), Math.Max(p1.X, p2.X), Math.Max(p1.Y, p2.Y));
    }
    public void Teken(Graphics g)
    {
        g.FillEllipse(kwast, rect);
    }

    public bool Raak(Point p)
    {
        //https://stackoverflow.com/questions/59971407/how-can-i-test-if-a-point-is-in-an-ellipse

        //middelpunt ellipse
        float centerX = rect.Left + rect.Width / 2f;
        float centerY = rect.Top + rect.Height / 2f;

        //1/2 hoogte
        float a = rect.Width / 2f;
        float b = rect.Height / 2f;

        float dx = p.X - centerX;
        float dy = p.Y - centerY;

        float ellipsRaak = (dx * dx) / (a * a) + (dy * dy) / (b * b);

        return ellipsRaak <= 1.0f;

    }
}      
       
public class LijnElement : ISchetsElement
{
    private Point p1, p2;
    private Brush kwast;

    public LijnElement(Point p1, Point p2, Brush kwast)
    {
        this.p1 = p1;
        this.p2 = p2;
        this.kwast = kwast;
    }
    private Point RoteerPunt(Point p, Size s)
    {
        return new Point(s.Height - p.Y, p.X);
    }
    public void Roteer(Size canvas)
    {
        p1 = RoteerPunt(p1, canvas);
        p2 = RoteerPunt(p2, canvas);
    }
    public void Teken(Graphics g)
    {
        g.DrawLine(TweepuntTool.MaakPen(kwast, 3), p1, p2);
    }

    public bool Raak(Point p)
    {
        //Formule: d = |ax0 + by0 - c| / sqrt(a^2 + b^2)

        float a = p2.Y - p1.Y;
        float b = p1.X - p2.X;
        float c = a * p1.X + b * p1.Y;

        float x0 = p.X;
        float y0 = p.Y;

        float d = Math.Abs(a*x0 + b*y0 - c) / (float)Math.Sqrt(a*a + b*b);

        //De wiskundig formule is voor een oneindige lijn dus moeten wij hier kijken of het binnen de werkelijke lijn is
        int hitBox = 5;

        // check of projectie binnen de lijn valt
        float dx = p2.X - p1.X;
        float dy = p2.Y - p1.Y;

        float dot =
            (p.X - p1.X) * dx +
            (p.Y - p1.Y) * dy;

        float lenSq = dx * dx + dy * dy;

        bool binnenSegment = dot >= 0 && dot <= lenSq;

        return d <= hitBox && binnenSegment;
    }
}      
       
public class PenElement : ISchetsElement
{      
    private List<Point> punten = new List<Point>();
    private Brush kwast;

    public PenElement(Brush kwast)
    {
        this.kwast = kwast;
    }
    private Point RoteerPunt(Point p, Size s)
    {
        return new Point(s.Height - p.Y, p.X);
    }
    public void Roteer(Size canvas)
    {
        for (int i = 0; i < punten.Count; i++)
            punten[i] = RoteerPunt(punten[i], canvas);
    }
    public void VoegPuntToe(Point p)
    {
        punten.Add(p);
    }

    public void Teken(Graphics g)
    {
        for (int i = 1; i < punten.Count; i++)
            g.DrawLine(TweepuntTool.MaakPen(kwast,3), punten[i-1], punten[i]);
    }

    public bool Raak(Point p)
    {
        int hitBox = 5;
        //Voor pen is elk kleine stuk lijn checken
        for (int i = 1; i < punten.Count; i++)
        {
            Point p1 = punten[i - 1];
            Point p2 = punten[i];

            float a = p2.Y - p1.Y;
            float b = p1.X - p2.X;
            float c = a * p1.X + b * p1.Y;

            float d = Math.Abs(a * p.X + b * p.Y - c) / (float)Math.Sqrt(a * a + b * b);

            bool binnenX = p.X >= Math.Min(p1.X, p2.X) - hitBox && p.X <= Math.Max(p1.X, p2.X) + hitBox;
            bool binnenY = p.Y >= Math.Min(p1.Y, p2.Y) - hitBox && p.Y <= Math.Max(p1.Y, p2.Y) + hitBox;

            if (d <= hitBox && binnenX && binnenY)
                return true;
        }

            return false;
    }
}

public class TekstElement : ISchetsElement
{
    private string tekst;
    private Point positie;
    private Brush kwast;
    private Font font;

    public TekstElement(string tekst, Point positie, Brush kwast)
    {
        this.tekst = tekst;
        this.positie = positie;
        this.kwast = kwast;
        this.font = new Font("Tahoma", 40);
    }
    private Point RoteerPunt(Point p, Size s)
    {
        return new Point(s.Height - p.Y, p.X);
    }
    public void Roteer(Size canvas)
    {
        positie = RoteerPunt(positie, canvas);
    }
    public void Teken(Graphics g)
    {
        g.DrawString(tekst, font, kwast, positie);
    }

    public bool Raak(Point p)
    {
        using (Bitmap bitmap = new Bitmap(1, 1))
        using (Graphics g = Graphics.FromImage(bitmap))
        {
            SizeF sz = g.MeasureString(tekst, font);
            RectangleF r = new RectangleF(positie, sz);
            return r.Contains(p);
        }
    }
    /////////////////////////////////////////////////////////////////
}


