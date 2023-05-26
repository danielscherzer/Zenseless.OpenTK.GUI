using Example;
using ImGuiNET;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Zenseless.OpenTK;
using Zenseless.OpenTK.GUI;

GameWindow window = new(GameWindowSettings.Default, ImmediateMode.NativeWindowSettings);

using ImGuiFacade gui = new(window);
gui.LoadFontDroidSans(24);

window.KeyDown += args => { if (Keys.Escape == args.Key) window.Close(); };

string input = "hallo";
Vector3 color3 = new(1f, 1f, 1f);
Vector4 color4 = new(1f, 1f, 1f, 1f);
var triangles = Helper.CreateRandomTria(100);
GL.EnableVertexAttribArray(0);

window.RenderFrame += args =>
{
	GL.ClearColor(new Color4(0, 32, 48, 255));
	GL.Clear(ClearBufferMask.ColorBufferBit);

	GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 0, triangles);
	GL.DrawArrays(PrimitiveType.Triangles, 0, triangles.Length); // draw with vertex array data
};

window.RenderFrame += args =>
{
	ImGui.NewFrame(); // call each frame before any ImGui.* calls

	ImGuiIOPtr io = ImGui.GetIO();
	ImGui.SliderFloat("Font scale", ref io.FontGlobalScale, 0.5f, 4f, "%.1f");
	ImGui.InputText("text", ref input, 255);
	ImGuiHelper.ColorEdit(nameof(color3), ref color3);
	ImGuiHelper.SliderFloat(nameof(color3), ref color3);
	ImGuiHelper.ColorEdit(nameof(color4), ref color4);
	ImGuiHelper.SliderFloat(nameof(color4), ref color4);
	for (int i = 0; i < 10; ++i) ImGui.Button("testbutton" + i);
	ImGui.ShowDemoWindow();

	ImGui.Begin("Style");
	ImGui.ShowStyleEditor();
	ImGui.End();

	gui.Render(window.Size);
};
window.RenderFrame += _ => window.SwapBuffers();
window.Resize += (window) => GL.Viewport(0, 0, window.Width, window.Height);

window.Run();