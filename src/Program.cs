using System;
using System.IO;
using System.Net;
using CommandLine;

namespace opengl_beef {
    class Program {
        static void Main(string[] args) {
            Parser.Default.ParseArguments<CmdOptions>(args).WithParsed(Run);
        }

        static void Run(CmdOptions options) {
            GlParser.Parse(GetGlXmlStream());

            GlVersion glVersion = GlParser.GetGlVersion(options);
            if (glVersion == null) {
                Console.WriteLine("Can't find specified gl version or profile.");
                return;
            }

            new GlFullVersion(glVersion, options.Profile).Generate();
        }

        static Stream GetGlXmlStream() {
            WebRequest req = WebRequest.Create("http://raw.githubusercontent.com/KhronosGroup/OpenGL-Registry/master/xml/gl.xml");
            return req.GetResponse().GetResponseStream();
        }
    }
}
