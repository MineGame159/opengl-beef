using System;
using CommandLine;

namespace opengl_beef {
    class CmdOptions {
        [Option("glVersion", Required = true, HelpText = "OpenGL version to generate. (3.0, 3.1, etc)")]
        public String Version { get; set; }

        [Option("profile", Required = true, HelpText = "OpenGL profile to generate. (Core or Compatibility)")]
        public GlProfile Profile { get; set; }
    }
}