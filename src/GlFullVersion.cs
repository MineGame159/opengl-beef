using System;
using System.Collections.Generic;
using System.IO;

namespace opengl_beef {
    class GlFullVersion {
        public String Version { get; }
        public double VersionDouble { get; }
        public GlProfile Profile { get; }

        public bool HasProfiles { get { return VersionDouble >= 3.2; } }

        public List<GlEnum> Enums { get; }
        public List<GlFunction> Functions { get; }

        public GlFullVersion(GlVersion glTargetVersion, GlProfile profile) {
            Version = glTargetVersion.Version;
            VersionDouble = glTargetVersion.VersionDouble;
            Profile = profile;

            Enums = new List<GlEnum>();
            Functions = new List<GlFunction>();

            foreach (GlVersion glVersion in GlParser.Versions) {
                if (glVersion.VersionDouble > glTargetVersion.VersionDouble) break;

                ApplyGlVersion(glVersion);
            }
        }

        private void ApplyGlVersion(GlVersion glVersion) {
            foreach (GlEnum glEnum in glVersion.AddedEnums) {
                if (!Enums.Contains(glEnum)) Enums.Add(glEnum);
            }
            foreach (GlFunction glFunction in glVersion.AddedFunctions) {
                if (!Functions.Contains(glFunction)) Functions.Add(glFunction);
            }

            if (HasProfiles) {
                if (Profile == GlProfile.Core) ApplyCoreProfile(glVersion);
                else if (Profile == GlProfile.Compatibility) ApplyCompatibilityProfile(glVersion);
            }
        }

        private void ApplyCoreProfile(GlVersion glVersion) {
            foreach (GlEnum glEnum in glVersion.AddedCoreEnums) {
                if (!Enums.Contains(glEnum)) Enums.Add(glEnum);
            }

            Enums.RemoveAll(glEnum => glVersion.RemovedCoreEnums.Contains(glEnum));
            Functions.RemoveAll(glFunction => glVersion.RemovedCoreFunctions.Contains(glFunction));
        }

        private void ApplyCompatibilityProfile(GlVersion glVersion) {
            foreach (GlEnum glEnum in glVersion.AddedCompatibilityEnums) {
                if (!Enums.Contains(glEnum)) Enums.Add(glEnum);
            }
            foreach (GlFunction glFunction in glVersion.AddedCompatibilityFunctions) {
                if (!Functions.Contains(glFunction)) Functions.Add(glFunction);
            }
        }

        public void Generate() {
            Console.WriteLine("Generating Beef file.");

            using (StreamWriter writer = new StreamWriter("GL.bf", false)) {
                writer.WriteLine("using System;");
                writer.WriteLine();
                writer.WriteLine("namespace opengl_beef {");
                writer.WriteLine("    class GL {");

                WriteFirstThings(writer);
                writer.WriteLine();
                WriteEnums(writer);
                writer.WriteLine();
                WriteFunctions(writer);
                writer.WriteLine();
                WriteLastThings(writer);

                writer.WriteLine("    }");
                writer.WriteLine("}");
            }
        }

        private void WriteFirstThings(StreamWriter writer) {
            writer.WriteLine("        public function void* GetProcAddressFunc(StringView procname);");
        }

        private void WriteEnums(StreamWriter writer) {
            foreach (GlEnum glEnum in Enums) {
                writer.WriteLine("        public const uint " + glEnum.Name + " = " + glEnum.Value + ";");
            }
        }

        private void WriteFunctions(StreamWriter writer) {
            int i = 0;
            foreach (GlFunction glFunction in Functions) {
                if (i > 0) writer.WriteLine();

                writer.Write("        public function " + ConvertType(glFunction.ReturnType) + " " + GetFunctionPointerName(glFunction) + "(");
                int ii = 0;
                foreach (GlParameter glParameter in glFunction.Parameters) {
                    if (ii > 0) writer.Write(", ");
                    writer.Write(ConvertType(glParameter.Type) + glParameter.Pointer + " " + ConvertParamName(glParameter.Name));
                    ii++;
                }
                writer.WriteLine(");");

                writer.WriteLine("        public static " + GetFunctionPointerName(glFunction) + " " + glFunction.Name + ";");

                i++;
            }
        }

        private void WriteLastThings(StreamWriter writer) {
            writer.WriteLine("        public static void Init(GetProcAddressFunc getProcAddress) {");
            foreach (GlFunction glFunction in Functions) {
                writer.WriteLine("            " + glFunction.Name + " = (" + GetFunctionPointerName(glFunction) + ") getProcAddress(\"" + glFunction.Name + "\");");
            }
            writer.WriteLine("        }");
        }

        private String GetFunctionPointerName(GlFunction function) {
            return Capitalize(function.Name);
        }

        private String Capitalize(String str) {
            return str.Substring(0, 1).ToUpper() + str.Substring(1);
        }

        private String ConvertParamName(String name) {
            switch (name) {
                case "params": return "paramss";
                case "ref":    return "reff";
                default:       return name;
            }
        }

        private String ConvertType(String type) {
            switch (type) {
                case "GLenum":               return "uint";
                case "GLboolean":            return "uint8";
                case "GLbitfield":           return "uint";
                case "GLvoid":               return "void";
                case "GLbyte":               return "int8";
                case "GLubyte":              return "uint8";
                case "GLshort":              return "int16";
                case "GLushort":             return "uint16";
                case "GLint":                return "int";
                case "GLuint":               return "uint";
                case "GLclampx":             return "int32";
                case "GLsizei":              return "int";
                case "GLfloat":              return "float";
                case "GLclampf":             return "float";
                case "GLdouble":             return "double";
                case "GLclampd":             return "double";
                case "GLeglClientBufferEXT": return "void*";
                case "GLeglImageOES":        return "void*";
                case "GLchar":               return "char8";
                case "GLcharARB":            return "char8";
                case "GLhandleARB":          return "void*";
                case "GLhalf":               return "uint16";
                case "GLhalfARB":            return "uint16";
                case "GLfixed":              return "int32";
                case "GLintptr":             return "int";
                case "GLintptrARB":          return "int";
                case "GLsizeiptr":           return "int";
                case "GLsizeiptrARB":        return "int";
                case "GLint64":              return "int64";
                case "GLint64EXT":           return "int64";
                case "GLuint64":             return "uint64";
                case "GLuint64EXT":          return "uint64";
                case "GLsync":               return "void*";
                default:                     return type.Replace("const", "").Trim();
            }
        }
    }
}