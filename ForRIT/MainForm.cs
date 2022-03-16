using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static ForRIT.Utilities;
using GMap.NET.Projections;

namespace ForRIT
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            GMap.NET.GMaps.Instance.Mode = GMap.NET.AccessMode.ServerAndCache; 
            gMapControl1.MapProvider = GMap.NET.MapProviders.GoogleMapProvider.Instance;
            MapMarker CenterMarker = Methods.GetMapMarkerFromDB("Center");
            gMapControl1.Position = new GMap.NET.PointLatLng(CenterMarker.Latitude, CenterMarker.Longitude);
            gMapControl1.DragButton = MouseButtons.Left; 
            gMapControl1.ShowCenter = false;
            gMapControl1.Overlays.Add(Methods.CreateOverlay("Markers"));
            foreach (GMapMarker m in gMapControl1.Overlays.SelectMany(m => m.Markers))
                gMapControl1.UpdateMarkerLocalPosition(m);
        }

        private GMapMarker selectedMarker = null;
        private void gMapControl1_MouseDown(object sender, MouseEventArgs e)
        {
            selectedMarker = gMapControl1.Overlays
                .SelectMany(o => o.Markers)
                .FirstOrDefault(m => m.IsMouseOver == true);
        }

        private void gMapControl1_MouseUp(object sender, MouseEventArgs e)
        {
            if (selectedMarker == null) return;
            var latlng = gMapControl1.FromLocalToLatLng(e.X, e.Y);
            selectedMarker.Position = latlng;
            Methods.UpdateMarker(selectedMarker);
            selectedMarker = null;
        }
    }
}
