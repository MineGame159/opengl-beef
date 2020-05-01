using System;
using System.Collections.Generic;
using System.Xml;
using System.Globalization;

namespace opengl_beef {
    class GlVersion : IComparable<GlVersion> {
        public String Version { get; }
        public double VersionDouble { get; }

        public List<GlEnum> AddedEnums { get; }
        public List<GlFunction> AddedFunctions { get; }

        public bool HasProfiles { get { return VersionDouble >= 3.2; } }

        public List<GlEnum> AddedCoreEnums { get; }

        public List<GlEnum> RemovedCoreEnums { get; }
        public List<GlFunction> RemovedCoreFunctions { get; }

        public List<GlEnum> AddedCompatibilityEnums { get; }
        public List<GlFunction> AddedCompatibilityFunctions { get; }

        public GlVersion(XmlNode node) {
            Version = node.Attributes["number"].Value;
            VersionDouble = double.Parse(Version, CultureInfo.InvariantCulture);

            AddedEnums = new List<GlEnum>();
            AddedFunctions = new List<GlFunction>();
            AddedCoreEnums = new List<GlEnum>();
            RemovedCoreEnums = new List<GlEnum>();
            RemovedCoreFunctions = new List<GlFunction>();
            AddedCompatibilityEnums = new List<GlEnum>();
            AddedCompatibilityFunctions = new List<GlFunction>();

            foreach (XmlNode childNode in node) {
                if (childNode.Name == "require") ParseRequireNode(childNode);
                else if (childNode.Name == "remove") ParseRemoveNode(childNode);
            }
        }

        private void ParseRequireNode(XmlNode node) {
            XmlNode profileAttr = node.Attributes["profile"];

            List<GlEnum> enums = profileAttr == null ? AddedEnums : (profileAttr.Value == "compatibility" ? AddedCompatibilityEnums : AddedCoreEnums);
            List<GlFunction> functions = profileAttr == null ? AddedFunctions : (profileAttr.Value == "compatibility" ? AddedCompatibilityFunctions : null);

            foreach (XmlNode childNode in node) {
                if (childNode.Name == "enum") {
                    enums.Add(GlParser.Enums[childNode.Attributes["name"].Value]);
                } else if (childNode.Name == "command") {
                    functions.Add(GlParser.Functions[childNode.Attributes["name"].Value]);
                }
            }
        }

        private void ParseRemoveNode(XmlNode node) {
            XmlNode profileAttr = node.Attributes["profile"];

            List<GlEnum> enums = profileAttr.Value == "core" ? RemovedCoreEnums : null;
            List<GlFunction> functions = profileAttr.Value == "core" ? RemovedCoreFunctions : null;

            foreach (XmlNode childNode in node) {
                if (childNode.Name == "enum") {
                    enums.Add(GlParser.Enums[childNode.Attributes["name"].Value]);
                } else if (childNode.Name == "command") {
                    functions.Add(GlParser.Functions[childNode.Attributes["name"].Value]);
                }
            }
        }

        public int CompareTo(GlVersion other) {
            return VersionDouble.CompareTo(other.VersionDouble);
        }
    }
}