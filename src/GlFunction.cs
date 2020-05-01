using System;
using System.Xml;
using System.Collections.Generic;

namespace opengl_beef {
    class GlParameter {
        public String Type { get; }
        public String Name { get; }
        public bool Pointer { get; }

        public GlParameter(XmlNode node) {
            XmlNode ptypeNode = node["ptype"];
            XmlNode nameNode = node["name"];

            if (ptypeNode != null && nameNode != null) {
                Type = ptypeNode.InnerText.Trim();
                Name = nameNode.InnerText.Trim();
                Pointer = node.InnerText.Contains("*");
            } else {
                int splitI;
                if (node.InnerText.Contains('*')) splitI = node.InnerText.LastIndexOf('*');
                else splitI = node.InnerText.LastIndexOf(' ');

                Type = node.InnerText.Substring(0, splitI).Trim();
                Name = node.InnerText.Substring(splitI + 1).Trim();
            }
        }
    }

    class GlFunction {
        public String ReturnType { get; }
        public String Name { get; }
        public List<GlParameter> Parameters { get; }

        public GlFunction(XmlNode node) {
            Parameters = new List<GlParameter>();
            foreach (XmlNode childNode in node) {
                if (childNode.Name == "proto") {
                    XmlNode ptypeNode = childNode["ptype"];

                    if (ptypeNode != null) {
                        ReturnType = ptypeNode.InnerText.Trim();
                    } else {
                        int splitI = childNode.InnerText.LastIndexOf(' ');
                        ReturnType = childNode.InnerText.Substring(0, splitI).Trim();
                    }

                    Name = childNode["name"].InnerText.Trim();
                } else if (childNode.Name == "param") {
                    Parameters.Add(new GlParameter(childNode));
                }
            }
        }
    }
}