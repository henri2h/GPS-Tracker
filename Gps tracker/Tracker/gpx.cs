using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows;
using System.Xml;
using System.AppCore;
using System.Xml.Linq;
using Console = System.AppCore.Console;

namespace Gps_tracker
{
    public static class gpx
    {
        public static string generateGPXOutput(List<point> track)
        {
            Console.WriteLine("Generating gpx output");

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

            List<point> trackCopy = new List<point>(track);

            foreach (point pointLocal in trackCopy)
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

        public static String GenerateCompleteGPX(List<point> points, String trackName)
        {
            DateTime dt = DateTime.Now;

            XDocument xDoc = new XDocument(new XDeclaration("1.0", "utf-8", "yes"));
            
            XElement xGpx = new XElement("gpx");
            xGpx.SetAttributeValue("version", "1.0");
            XElement xTrk = new XElement("trk");
            XElement xTrksgt = new XElement("trkseg");



            xTrk.Add(new XElement("name", trackName));
            xTrk.Add(new XElement("number", 1));

            foreach (point pt in points)
            {
                XElement xPoint = new XElement("trkpt");

                xPoint.SetAttributeValue("lon", pt.longitude.ToString());
                xPoint.SetAttributeValue("lat", pt.latitude.ToString());

                XElement xEle = new XElement("ele", pt.altitude.ToString());
                xPoint.Add(xEle);

                string time = pt.date.ToUniversalTime().ToString();
                XElement xTime = new XElement("time", time);
                xPoint.Add(xTime);

                xTrksgt.Add(xPoint);
            }

            xTrk.Add(xTrksgt);

            xGpx.Add(xTrk);
            xDoc.Add(xGpx);

            return xDoc.ToString();
        }


    }
}
