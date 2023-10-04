using Example;
using ImGuiNET;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using Zenseless.OpenTK;
using Zenseless.OpenTK.GUI;
using Buffer = Zenseless.OpenTK.Buffer;

using GameWindow window = new(GameWindowSettings.Default, new NativeWindowSettings() 
{ APIVersion = new Version(4, 5), Flags = ContextFlags.Debug });
DebugOutputGL debugOutput = new();
debugOutput.DebugEvent += (_, args) => Console.WriteLine(args.Message);
using ImGuiFacade gui = new(window);
gui.LoadFontDroidSans(24);

window.KeyDown += args => { if (Keys.Escape == args.Key) window.Close(); };

string input = "hallo";
Vector3 color3 = new(1f, 1f, 1f);
Vector4 color4 = new(1f, 1f, 1f, 1f);
var triangles = Helper.CreateRandomTriangles(100);
using Buffer buffer = new();
buffer.Set(triangles);
using VertexArray vertexArray = new();
vertexArray.BindAttribute(0, buffer, 2, Vector2.SizeInBytes, VertexAttribType.Float);

window.RenderFrame += args =>
{
	GL.ClearColor(new Color4(0, 32, 48, 255));
	GL.Clear(ClearBufferMask.ColorBufferBit);

	vertexArray.Bind();
	GL.DrawArrays(PrimitiveType.Triangles, 0, triangles.Length); // draw with vertex array data
};

window.RenderFrame += args =>
{
	ImGui.NewFrame(); // call each frame before any ImGui.* calls

	ImGui.ShowDemoWindow();

	ImGui.Begin("Style");
	ImGui.ShowStyleEditor();
	ImGui.End();

	ImGui.Begin("user");
	ImGuiIOPtr io = ImGui.GetIO();

	ImGui.SliderFloat("Font scale", ref io.FontGlobalScale, 0.5f, 4f, "%.1f");
	ImGui.InputText("text", ref input, 255);
	ImGuiHelper.ColorEdit(nameof(color3), ref color3);
	ImGuiHelper.SliderFloat(nameof(color3), ref color3);
	ImGuiHelper.ColorEdit(nameof(color4), ref color4);
	ImGuiHelper.SliderFloat(nameof(color4), ref color4);
	for (int i = 0; i < 10; ++i)
	{
		ImGui.Button("testbutton" + i);
	}
	ImGui.End();
	gui.Render(window.ClientSize);
};
window.RenderFrame += _ => window.SwapBuffers();
window.Resize += (window) => GL.Viewport(0, 0, window.Width, window.Height);

window.Run();