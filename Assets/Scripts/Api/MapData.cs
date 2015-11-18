using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using UnityEngine;

namespace Api
{
    public class MapData
    {
        public SystemDataPart SystemData { get; private set; }
        public JumpDataPart JumpData { get; private set; }

        public MapData()
        {
            SystemData = new SystemDataPart();
            JumpData = new JumpDataPart();
        }
    }

    public abstract class DataPart<T> : IEnumerable<T>
    {
        XmlReader reader;

        public DataPart(string fileName)
        {
            Stream stream = new FileStream(Path.Combine(Application.dataPath, fileName), FileMode.Open);
            reader = XmlReader.Create(stream);
        }

        public IEnumerator<T> GetEnumerator()
        {
            reader.MoveToContent();
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element && reader.IsStartElement() && reader.Name == "row")
                {
                    XElement element = XNode.ReadFrom(reader) as XElement;
                    if (element != null)
                        yield return ParseElement(element);
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        protected abstract T ParseElement(XElement element);
    }

    public class SystemDataPart : DataPart<SolarSystem>
    {
        public SystemDataPart() : base("mapdata.xml") { }

        protected override SolarSystem ParseElement(XElement element)
        {
            float x, y, z;
            decimal sec;
            int id;

            if (float.TryParse(element.Element("x").Value, out x) && float.TryParse(element.Element("y").Value, out y)
                && float.TryParse(element.Element("z").Value, out z) && decimal.TryParse(element.Element("security").Value, out sec)
                && int.TryParse(element.Element("solarSystemID").Value, out id))
            {
                return new SolarSystem(new Vector3(x, y, z), sec, id);
            }
            return null;
        }
    }

    public class JumpDataPart : DataPart<JumpConnection>
    {
        public JumpDataPart() : base("jumps.xml") { }

        protected override JumpConnection ParseElement(XElement element)
        {
            int startID, endID;

            if (int.TryParse(element.Element("fromSolarSystemID").Value, out startID) && int.TryParse(element.Element("toSolarSystemID").Value, out endID))
            {
                return new JumpConnection(startID, endID);
            }
            return null;
        }
    }

    public class SolarSystem
    {
        public Vector3 Position { get; private set; }
        public decimal Security { get; private set; }
        public int ID { get; private set; }

        public SolarSystem(Vector3 pos, decimal sec, int id)
        {
            Position = pos;
            Security = sec;
            ID = id;
        }
    }

    public class JumpConnection
    {
        public int StartID { get; private set; }
        public int EndID { get; private set; }

        public JumpConnection(int start, int end)
        {
            StartID = start;
            EndID = end;
        }
    }
}