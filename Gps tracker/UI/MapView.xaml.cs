using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Gps_tracker.UI
{
    public sealed partial class MapView : UserControl
    {
        public MapView()
        {
            this.InitializeComponent();
            // place the mapServiceToken here, you can get one at https://www.bingmapsportal.com/

            MapControl.MapServiceToken = AppCore.Core.settings.MapServiceToken;
            MapControl.MapServiceToken = "dIQYRjm1oGFEfWPNnTmx~GRofurcHYDuU4uJtNG1C6Q~AhcpDsCLAmjtPskvs3dCm3TMl2Hhawxmy66H6cGFAmkUcOFou7gYl0xbTzzit0Id";

        }
        public void centerMap(Geopoint point)
        {
            MapControl.Center = point;
            MapControl.ZoomLevel = 16;
        }

        public void setUIMapSegement(BasicGeoposition start, BasicGeoposition end)
        {
            Windows.UI.Xaml.Controls.Maps.MapPolyline mapPolyline = new Windows.UI.Xaml.Controls.Maps.MapPolyline();
            mapPolyline.Path = new Geopath(new List<BasicGeoposition>() { start, end });

            mapPolyline.StrokeColor = Colors.Black;
            mapPolyline.StrokeThickness = 3;
            mapPolyline.StrokeDashed = true;
            MapControl.MapElements.Add(mapPolyline);
        }

        public void updateMap(point[] points)
        {
            try
            {
                if (points.Length > 0)
                {
                    point oldPoint = points[points.Length - 2];
                    point current = points[points.Length - 1];

                    if (oldPoint != null)
                    {
                        setUIMapSegement(
                            new BasicGeoposition() { Latitude = oldPoint.latitude, Longitude = oldPoint.longitude },
                            new BasicGeoposition() { Latitude = current.latitude, Longitude = current.longitude }
                        );


                    }
                }

            }
            catch (Exception ex)
            {
                ex.Source = "MainPage.updateUIMap";
                ErrorMessage.printOut(ex);
            }
        }

        /// <summary>
        /// Refresh all the map
        /// </summary>
        public void updateUIAllMap(point[] points)
        {
            MapControl.MapElements.Clear();
            try
            {
                point oldPoint = null;
                foreach (point pointElement in points)
                {
                    if (oldPoint != null)
                    {
                        Windows.UI.Xaml.Controls.Maps.MapPolyline mapPolyline = new Windows.UI.Xaml.Controls.Maps.MapPolyline();
                        mapPolyline.Path = new Geopath(new List<BasicGeoposition>() {
                            new BasicGeoposition() {Latitude=oldPoint.latitude, Longitude=oldPoint.longitude},
                            new BasicGeoposition() {Latitude=pointElement.latitude, Longitude=pointElement.longitude}
                             });
                        mapPolyline.StrokeColor = Colors.Black;
                        mapPolyline.StrokeThickness = 3;
                        mapPolyline.StrokeDashed = true;
                        MapControl.MapElements.Add(mapPolyline);
                    }

                    oldPoint = pointElement;
                }

            }
            catch (Exception ex)
            {
                ex.Source = "MainPage.updateUIMap";
                ErrorMessage.printOut(ex);
            }
        }

        private void MapControl1_Loaded(object sender, RoutedEventArgs e)
        { //UIMapView.updateMap(GPSLocator.track.ToArray());
        }
    }

}
