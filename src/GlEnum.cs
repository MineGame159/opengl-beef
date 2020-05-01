using System;
using System.Xml;
using System.Collections.Generic;

namespace opengl_beef {
    class GlEnum {
        public String Name { get; }
        public String Value { get; }

        public GlEnum(XmlNode node) {
            Name = node.Attributes["name"].Value.Trim();
            Value = node.Attributes["value"].Value.Trim();
        }
    }
}