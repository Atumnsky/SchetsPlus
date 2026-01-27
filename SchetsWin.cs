using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

public class SchetsWin : Form
{
    MenuStrip menuStrip;
    SchetsControl schetscontrol;
    ISchetsTool huidigeTool;
    Panel paneel;
    bool vast;
    private string huidigBestand = null;

    private void veranderAfmeting(object o, EventArgs ea)
    {
        schetscontrol.Size = new Size(this.ClientSize.Width - 70
                                      , this.ClientSize.Height - 50);
        paneel.Location = new Point(64, this.ClientSize.Height - 30);
    }

    private void klikToolMenu(object obj, EventArgs ea)
    {
        this.huidigeTool = (ISchetsTool)((ToolStripMenuItem)obj).Tag;
    }

    private void klikToolButton(object obj, EventArgs ea)
    {
        this.huidigeTool = (ISchetsTool)((RadioButton)obj).Tag;
    }

    private void afsluiten(object obj, EventArgs ea)
    {
        this.Close();
    }

    public SchetsWin()
    {
        ISchetsTool[] deTools = { new PenTool()
                                , new LijnTool()
                                , new RechthoekTool()
                                , new VolRechthoekTool()
                                , new CirkelTool()
                                , new VolCirkelTool()
                                , new TekstTool()
                                , new GumTool()
                                };
        String[] deKleuren = { "Black", "Red", "Green", "Blue", "Yellow", "Magenta", "Cyan" };

        this.ClientSize = new Size(700, 510);
        huidigeTool = deTools[0];

        schetscontrol = new SchetsControl();
        schetscontrol.Location = new Point(64, 10);
        schetscontrol.MouseDown += (object o, MouseEventArgs mea) =>
        {
            vast = true;
            huidigeTool.MuisVast(schetscontrol, mea.Location);
        };
        schetscontrol.MouseMove += (object o, MouseEventArgs mea) =>
        {
            if (vast)
                huidigeTool.MuisDrag(schetscontrol, mea.Location);
        };
        schetscontrol.MouseUp += (object o, MouseEventArgs mea) =>
        {
            if (vast)
                huidigeTool.MuisLos(schetscontrol, mea.Location);
            vast = false;
        };
        schetscontrol.KeyPress += (object o, KeyPressEventArgs kpea) =>
        {
            huidigeTool.Letter(schetscontrol, kpea.KeyChar);
        };
        this.Controls.Add(schetscontrol);

        menuStrip = new MenuStrip();
        menuStrip.Visible = false;
        huidigBestand = null;
        this.Text = "Schets - Nieuw";
        this.Controls.Add(menuStrip);
        this.maakFileMenu();
        this.maakToolMenu(deTools);
        this.maakActieMenu(deKleuren);
        this.maakToolButtons(deTools);
        this.maakActieButtons(deKleuren);
        this.Resize += this.veranderAfmeting;
        this.veranderAfmeting(null, null);
        this.FormClosing += SchetsWinGesloten;
    }

    private void SchetsWinGesloten(object sender, FormClosingEventArgs e)
    {
        if (!schetscontrol.Schets.IsGewijzigd)
            return;

        DialogResult waarschuwing = MessageBox.Show(
            "De schets is gewijzigd. Wilt u deze opslaan?",
            "Opslaan?",
            MessageBoxButtons.YesNoCancel,
            MessageBoxIcon.Warning);

        if (waarschuwing == DialogResult.Yes)
        {
            opslaan(null, EventArgs.Empty);

            if (schetscontrol.Schets.IsGewijzigd)
                e.Cancel = true;
        }

        else if (waarschuwing == DialogResult.Cancel)
        {
            e.Cancel = true;
        }
    }

    private void maakFileMenu()
    {
        ToolStripMenuItem menu = new ToolStripMenuItem("File");
        menu.MergeAction = MergeAction.MatchOnly;
        menu.DropDownItems.Add("Sluiten", null, this.afsluiten);
        menuStrip.Items.Add(menu);

         
        menu.DropDownItems.Add("Opslaan", null, this.opslaan);
        menu.DropDownItems.Add("Openen", null, this.openen);
        menu.DropDownItems.Add("Exporteer afbeelding", null, this.exporteer);
         
    }

     
    private void opslaan(object sender, EventArgs e)
    {
        if (huidigBestand == null)
        {
            SaveFileDialog file = new SaveFileDialog();
            file.Filter = "Schets(*.schets)|*.schets";

            if (file.ShowDialog() != DialogResult.OK)
                return;

            huidigBestand = file.FileName;
        }

        schetscontrol.Schets.Opslaan(huidigBestand);
        this.Text = $"Schets - {Path.GetFileName(huidigBestand)}";
    }

    private void openen(object sender, EventArgs e)
    {
        OpenFileDialog file = new OpenFileDialog();
        file.Filter = "Schets(*.schets)|*.schets";

        if (file.ShowDialog() != DialogResult.OK)
            return;

        schetscontrol.Schets.Openen(file.FileName);
        schetscontrol.Invalidate();

        huidigBestand = file.FileName;
        this.Text = $"Schets - {Path.GetFileName(huidigBestand)}";
    }
    
    ///https://learn.microsoft.com/en-us/dotnet/api/system.windows.forms.savefiledialog?view=windowsdesktop-10.0
    ///https://stackoverflow.com/questions/11055258/how-to-use-savefiledialog-for-saving-images-in-c
    private void exporteer(object sender, EventArgs e)
    {
        SaveFileDialog file = new SaveFileDialog();
        file.Filter = "PNG (*.png)|*.png|" + "JPEG (*.jpg)|*.jpg|" + "BITMAP (*.bmp)|*.bmp";

        if(file.ShowDialog() != DialogResult.OK)
            return;

        using (Bitmap bmp = schetscontrol.Schets.NaarBitmap(schetscontrol.ClientSize))
        {
            if (file.FileName.EndsWith(".png"))
                bmp.Save(file.FileName, System.Drawing.Imaging.ImageFormat.Png);
            else if (file.FileName.EndsWith(".jpg"))
                bmp.Save(file.FileName, System.Drawing.Imaging.ImageFormat.Jpeg);
            else
                bmp.Save(file.FileName, System.Drawing.Imaging.ImageFormat.Bmp);
        }

    }
     

    private void maakToolMenu(ICollection<ISchetsTool> tools)
    {
        ToolStripMenuItem menu = new ToolStripMenuItem("Tool");
        foreach (ISchetsTool tool in tools)
        {
            ToolStripItem item = new ToolStripMenuItem();
            item.Tag = tool;
            item.Text = tool.ToString();
            item.Image = new Bitmap($"../../../Icons/{tool.ToString()}.png");
            item.Click += this.klikToolMenu;
            menu.DropDownItems.Add(item);
        }
        menuStrip.Items.Add(menu);
    }

    private void maakActieMenu(String[] kleuren)
    {
        ToolStripMenuItem menu = new ToolStripMenuItem("Actie");
        menu.DropDownItems.Add("Clear", null, schetscontrol.Schoon);
        menu.DropDownItems.Add("Roteer", null, schetscontrol.Roteer);
        ToolStripMenuItem submenu = new ToolStripMenuItem("Kies kleur");
        foreach (string k in kleuren)
            submenu.DropDownItems.Add(k, null, schetscontrol.VeranderKleurViaMenu);
        menu.DropDownItems.Add(submenu);
        menuStrip.Items.Add(menu);
    }

    private void maakToolButtons(ICollection<ISchetsTool> tools)
    {
        int t = 0;
        foreach (ISchetsTool tool in tools)
        {
            RadioButton b = new RadioButton();
            b.Appearance = Appearance.Button;
            b.Size = new Size(48, 62);
            b.Location = new Point(10, 10 + t * 62);
            b.Tag = tool;
            b.Text = tool.ToString();
            b.Image = new Bitmap($"../../../Icons/{tool.ToString()}.png");
            b.TextAlign = ContentAlignment.TopCenter;
            b.ImageAlign = ContentAlignment.BottomCenter;
            b.Click += this.klikToolButton;
            this.Controls.Add(b);
            if (t == 0) b.Select();
            t++;
        }
    }

    private void maakActieButtons(String[] kleuren)
    {
        paneel = new Panel(); this.Controls.Add(paneel);
        paneel.Size = new Size(600, 24);

        Button clear = new Button(); paneel.Controls.Add(clear);
        clear.Text = "Clear";
        clear.Location = new Point(0, 0);
        clear.Click += schetscontrol.Schoon;

        Button rotate = new Button(); paneel.Controls.Add(rotate);
        rotate.Text = "Rotate";
        rotate.Location = new Point(80, 0);
        rotate.Click += schetscontrol.Roteer;

        Label penkleur = new Label(); paneel.Controls.Add(penkleur);
        penkleur.Text = "Penkleur:";
        penkleur.Location = new Point(180, 3);
        penkleur.AutoSize = true;

        ComboBox cbb = new ComboBox(); paneel.Controls.Add(cbb);
        cbb.Location = new Point(240, 0);
        cbb.DropDownStyle = ComboBoxStyle.DropDownList;
        cbb.SelectedValueChanged += schetscontrol.VeranderKleur;
        foreach (string k in kleuren)
            cbb.Items.Add(k);
        cbb.SelectedIndex = 0;
    }
}