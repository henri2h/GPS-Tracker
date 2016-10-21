using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows;
using System.Xml;

namespace Gps_tracker
{
    public static class gpx
    {
        public static string generateGPXOutput(List<point> track)
        {
            System.Diagnostics.Debug.WriteLine("generating gpx output");

            XmlDocument doc = new XmlDocument();
            
            XmlDeclaration xmlDeclaration = doc.CreateXmlDeclaration("1.0", "UTF-8", "no");
            XmlElement root = doc.DocumentElement;
            doc.InsertBefore(xmlDeclaration, root);


            XmlElement gpx = doc.CreateElement(string.Empty, "gpx", string.Empty);
            doc.AppendChild(gpx);

            XmlElement trk = doc.CreateElement(string.Empty, "trk", string.Empty);
            gpx.AppendChild(trk);

            XmlElement name = doc.CreateElement(string.Empty, "name", string.Empty);
            XmlText nameValue = doc.CreateTextNode("track : " + DateTime.Now.ToString());
            name.AppendChild(nameValue);
            trk.AppendChild(name);

            XmlElement trkseg = doc.CreateElement(string.Empty, "trkseg", string.Empty);
            trk.AppendChild(trkseg);

            foreach (point pointLocal in track)
            {
                //create point
                XmlElement trkpt = doc.CreateElement(string.Empty, "trkpt", string.Empty);
                //set attribute latitude and longitude
                // add attributes to the xml node

                XmlAttribute lat = doc.CreateAttribute("lat");
                lat.Value = pointLocal.latitude.ToString();
                trkpt.Attributes.Append(lat);

                XmlAttribute lon = doc.CreateAttribute("lon");
                lon.Value = pointLocal.longitude.ToString();
                trkpt.Attributes.Append(lon);

                // add the xml node to the xml document
                trkseg.AppendChild(trkpt);

                // add elevetion to the xml node
                XmlElement ele = doc.CreateElement(string.Empty, "ele", string.Empty);
                XmlText value = doc.CreateTextNode(pointLocal.altitude.ToString());
                ele.AppendChild(value);
                trkpt.AppendChild(ele);
            }

            //returning the xml document to a string
            StringWriter output = new StringWriter();
            doc.Save(output);
            return output.ToString();
        }
        

    }
}
