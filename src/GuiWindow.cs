using ImGuiNET;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using System;

namespace Zenseless.OpenTK.GUI;

/// <summary>
/// Child of <see cref="GameWindow"/> that also handles gui rendering with <see cref="ImGuiNET"/>
/// </summary>
/// <para>
/// Use GameWindowSettings.Default and NativeWindowSettings.Default to get some sensible default attributes.
/// </para>
/// </remarks>
public class GuiWindow : GameWindow
{
	/// <summary>
	/// Initializes a new instance of the <see cref="GuiWindow"/> class with sensible default attributes.
	/// </summary>
	/// <param name="gameWindowSettings">The <see cref="GameWindow"/> related settings.</param>
	/// <param name="nativeWindowSettings">The <see cref="NativeWindow"/> related settings.</param>
	public GuiWindow(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings, nativeWindowSettings)
	{
		gui = new(this);
	}

	/// <summary>
	/// Access to the <see cref="ImGuiFacade"/>
	/// </summary>
	public ImGuiFacade Gui => gui;

	/// <summary>
	/// Returns <c>true</c> if keyboard or mouse focus is on any ImGui element
	/// </summary>
	/// <returns></returns>
	public static bool HasFocus()
	{
		var io = ImGui.GetIO();
		return io.WantCaptureKeyboard || io.WantCaptureMouse;
	}

	/// <summary>
	/// Run when the window is ready to update. This is called before <see cref="OnRenderFrame(FrameEventArgs)"/>.
	/// </summary>
	/// <param name="args">The event arguments for this frame.</param>
	protected override void OnUpdateFrame(FrameEventArgs args)
	{
		ImGui.NewFrame();
		base.OnUpdateFrame(args);
	}

	/// <summary>
	/// Run when the window is ready to render. This is called after <see cref="OnUpdateFrame(FrameEventArgs)"/>.
	/// </summary>
	/// <param name="args">The event arguments for this frame.</param>
	protected override void OnRenderFrame(FrameEventArgs args)
	{
		base.OnRenderFrame(args);
		ImGui.End();
		gui.Render(ClientSize);
		SwapBuffers();
	}

	private ImGuiFacade gui;
}