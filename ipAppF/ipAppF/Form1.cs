using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Net;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;

namespace ipAppF
{
    public partial class Form1 : Form
    {
        private WebClient wc;
        public Form1()
        {
            InitializeComponent();
            wc = new WebClient();
            InitializeGMapControl();
        }
        private void InitializeGMapControl() //metoda pro nastaveni ovladani google map 
        {
            gMapControl1.MapProvider = GMapProviders.GoogleMap;
            gMapControl1.MinZoom = 5;
            gMapControl1.MaxZoom = 50;
            gMapControl1.Zoom = 15;
            gMapControl1.DragButton = MouseButtons.Left;
        }
        private string ExtractValue(string line, string pattern) 
        {
            Match match = Regex.Match(line, pattern);
            return match.Groups[1].Value;
        }
        private double ParseDouble(string value)
        {
            double result;
            double.TryParse(value, out result);
            return result;
        }
        private void UpdateLocation(string line) //metoda ve ktere nacitame data z textboxu a pak je pomoci regetu prevadime na vhodny format pro gMap.
        {
            line = wc.DownloadString($"http://ipwho.is/{textBox1.Text}?output=xml");

            string country = ExtractValue(line, "<country>(.*?)</country>");
            string ip = ExtractValue(line, "<ip>(.*?)</ip>");
            string latitude = ExtractValue(line, "<latitude>((.*?))</latitude>");
            string longitude = ExtractValue(line, "<longitude>((.*?))</longitude>");

            double la = ParseDouble(latitude);
            double lo = ParseDouble(longitude);

            label1.Text = country;
            label2.Text = ip;

            SetMapPosition(la, lo);
        }

        private void SetMapPosition(double latitude, double longitude) //metoda pro zobrazeni gMap s nastavenou polohou
        {
            gMapControl1.Position = new GMap.NET.PointLatLng(latitude, longitude);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string line = "";
            UpdateLocation(line);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (Regex.IsMatch(textBox1.Text, "[^0-9-.]"))
            {
                textBox1.Text = textBox1.Text.Remove(textBox1.TextLength - 1);
                textBox1.SelectionStart = textBox1.Text.Length;
            }
        }

        private void textBox1_Click(object sender, EventArgs e)
        {
            textBox1.Text = String.Empty;

        }

        private void label2_TextChanged(object sender, EventArgs e)
        {
            if (label1.Text == String.Empty)
            {
                label2.Text = "You wrote incorrect IP";
            }
        }
    }
}
