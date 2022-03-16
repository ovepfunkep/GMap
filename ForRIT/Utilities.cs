using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ForRIT
{
    internal class Utilities
    {
        public static readonly string SQLString = @"Data Source=DESKTOP-M7EVVNL\SQLEXPRESS;Initial Catalog=MapMarkers;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

        public class MapMarker
        {
            public int Id { get; set; }
            public string TName { get; set; }
            public double Latitude { get; set; }
            public double Longitude { get; set; }
        }

        public class Methods
        {
            public static MapMarker GetMapMarkerFromDB(string TName)
            {
                MapMarker mapMarker = new MapMarker();
                using (SqlConnection connection = new SqlConnection(SQLString))
                {
                    string query = "Select * from Technique where TName = @TName";
                    connection.Open();
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("TName", TName);
                    SqlDataReader reader = command.ExecuteReader();
                    reader.Read();
                    mapMarker.Id = (int)reader["Id"];
                    mapMarker.TName = TName;
                    mapMarker.Latitude = (double)(decimal)reader["Latitude"];
                    mapMarker.Longitude = (double)(decimal)reader["Longitude"];
                }
                return mapMarker;
            }

            public static void UpdateMarker (GMapMarker gMapMarker)
            {
                using (SqlConnection connection = new SqlConnection(SQLString))
                {
                    string query = "Update Technique set Latitude = @Latitude, Longitude = @Longitude where TName = @TName";
                    connection.Open();
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("Latitude", gMapMarker.Position.Lat);
                    command.Parameters.AddWithValue("Longitude", gMapMarker.Position.Lng);
                    command.Parameters.AddWithValue("TName", gMapMarker.ToolTipText);
                    command.ExecuteNonQuery();
                }
            }

            public static GMarkerGoogle AddMarker (MapMarker mapMarker, GMarkerGoogleType gMarkerGoogleType = GMarkerGoogleType.red_dot)
            {
                GMarkerGoogle gMarker = new GMarkerGoogle(new GMap.NET.PointLatLng(mapMarker.Latitude, mapMarker.Longitude), gMarkerGoogleType);
                gMarker.ToolTip = new GMap.NET.WindowsForms.ToolTips.GMapRoundedToolTip(gMarker);
                gMarker.ToolTipText = mapMarker.TName;
                gMarker.ToolTipMode = MarkerTooltipMode.OnMouseOver;
                return gMarker;
            }

            public static GMapOverlay CreateOverlay (string Name, GMarkerGoogleType gMarkerGoogleType = GMarkerGoogleType.red_dot)
            {
                GMapOverlay gMapMarkers = new GMapOverlay(Name);
                using (SqlConnection connection = new SqlConnection(SQLString))
                {
                    string query = "Select * from Technique";
                    connection.Open();
                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        if (reader["TName"].ToString() != "Center") gMapMarkers.Markers.Add(
                            AddMarker(
                                GetMapMarkerFromDB(reader["TName"].ToString())));
                    }
                }
                return gMapMarkers;
            }

        }
    }
}
