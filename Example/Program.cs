using ImGuiNET;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Zenseless.OpenTK.GUI;
using Zenseless.Resources;

GameWindow window = new(GameWindowSettings.Default, NativeWindowSettings.Default);
window.WindowState = WindowState.Maximized;

ImGuiInput imGuiInput = new(window);
using ImGuiRenderer gui = new();

gui.SetFont(new ShortestMatchResourceDirectory(new EmbeddedResourceDirectory()).Resource("DroidSans.ttf").AsByteArray());

window.KeyDown += args => { if (Keys.Escape == args.Key) window.Close(); };
window.UpdateFrame += args => imGuiInput.Update(window.MouseState, window.KeyboardState, (float)args.Time);

string input = "hallo";
Vector3 color3 = new(1f, 1f, 1f);
Vector4 color4 = new(1f, 1f, 1f, 1f);
window.RenderFrame += args =>
{
	ImGui.NewFrame(); // call each frame before any ImGui.* calls

	GL.ClearColor(new Color4(0, 32, 48, 255));
	GL.Clear(ClearBufferMask.ColorBufferBit);

	ImGuiIOPtr io = ImGui.GetIO();
	ImGui.SliderFloat("Font scale", ref io.FontGlobalScale, 0.5f, 4f, "%.1f");
	ImGui.InputText("text", ref input, 255);
	ImGuiHelper.ColorEdit(nameof(color3), ref color3);
	ImGuiHelper.SliderFloat(nameof(color3), ref color3);
	ImGuiHelper.ColorEdit(nameof(color4), ref color4);
	ImGuiHelper.SliderFloat(nameof(color4), ref color4);
	for (int i = 0; i < 10; ++i) ImGui.Button("testbutton" + i);

	ImGui.ShowDemoWindow();

	gui.Render();
	window.SwapBuffers();
};

window.Resize += (window) => GL.Viewport(0, 0, window.Width, window.Height);
window.Resize += (window) => ImGuiRenderer.WindowResized(window.Width, window.Height);

window.Run();