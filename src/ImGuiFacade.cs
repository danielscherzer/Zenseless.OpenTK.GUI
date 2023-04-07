using ImGuiNET;
using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;
using System;
using Zenseless.Patterns;
using Zenseless.Resources;

namespace Zenseless.OpenTK.GUI;

/// <summary>
/// Creates a faced for using <seealso cref="ImGuiInput"/> and <seealso cref="ImGuiRenderer"/>
/// </summary>
public class ImGuiFacade : Disposable
{
	/// <summary>
	/// Create a new instance.
	/// </summary>
	public ImGuiFacade(GameWindow window, float fontScale = 1f)
	{
		_input = new ImGuiInput(window);
		_renderer = new ImGuiRenderer();
		ImGui.GetIO().FontGlobalScale = fontScale;
	}

	/// <summary>
	/// Render the user interface
	/// </summary>
	/// <param name="windowResolution">Window resolution in pixels.</param>
	public void Render(Vector2i windowResolution) => _renderer.Render(windowResolution);

	/// <summary>
	/// Load a TTF font and use it for rendering the GUI
	/// </summary>
	/// <param name="fontData">TTF file read into a byte array</param>
	/// <param name="sizePixels">Intented size in pixels. Bigger means bigger texture is created.</param>
	public void LoadFontTTF(byte[] fontData, float sizePixels) => _renderer.LoadFontTTF(fontData, sizePixels);

	/// <summary>
	/// Load the embedded DroidSans font and use it for rendering the GUI
	/// </summary>
	/// <param name="sizePixels">Intented size in pixels. Bigger means bigger texture is created.</param>
	public void LoadFontDroidSans(float sizePixels)
	{
		var resDir = new ShortestMatchResourceDirectory(new EmbeddedResourceDirectory());
		LoadFontTTF(resDir.Resource("DroidSans.ttf").AsByteArray(), sizePixels);
	}

	/// <summary>
	/// Will be called from the default Dispose method.
	/// Implementers should dispose all their resources her.
	/// </summary>
	protected override void DisposeResources() => DisposeAllFields(this);

#pragma warning disable IDE0052
	private readonly ImGuiInput _input;
#pragma warning restore IDE0052
	private readonly ImGuiRenderer _renderer;
}
