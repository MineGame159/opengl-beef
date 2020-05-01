# OpenGL-Beef
**opengl-beef** is a Beef opengl loader generator. Works with any windowing library that provides GetProcAddress function. For now it only generates gl api and no extensions.

# Quick Start
1. Install dotnet core sdk.
2. Copy this repository.
3. Run "dotnet run --glVersion 3.3 --profile Core" in your copied opengl-beef folder with modified version and profile arguments.
4. Copy generated GL.bf file to your project and change its namespace.
5. Happy coding! Example using my [glfw-beef](https://github.com/MineGame159/glfw-beef) library:
```
using System;
using glfw_beef;

namespace test {
	class Program {
		public static void Main() {
			Glfw.Init();

			GlfwWindow* window = Glfw.CreateWindow(640, 480, "OpenGL Test", null, null);

			Glfw.MakeContextCurrent(window);
			GL.Init(=> Glfw.GetProcAddress);

			while (!Glfw.WindowShouldClose(window)) {
				GL.glClearColor(1, 0, 1, 1);
				GL.glClear(GL.GL_COLOR_BUFFER_BIT);

				Glfw.PollEvents();
				Glfw.SwapBuffers(window);
			}

			Glfw.DestroyWindow(window);
		}
	}
}
```

Why I am not using Beef for the generator? When there is xml library I will consider rewriting it.