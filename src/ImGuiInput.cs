using ImGuiNET;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Zenseless.OpenTK.GUI;

/// <summary>
/// Class that handles input for ImGui
/// </summary>
public class ImGuiInput
{
	/// <summary>
	/// Create a new instance
	/// </summary>
	/// <param name="window">A <see cref="GameWindow"/> for connecting input and update event handler.</param>
	public ImGuiInput(GameWindow window)
	{
		ImGuiHelper.AssureContextCreated();

		ImGuiIOPtr io = ImGui.GetIO();
		io.DeltaTime = 1f / 60f;

		window.TextInput += args => io.AddInputCharacter((uint)args.Unicode);
		window.UpdateFrame += args => io.DeltaTime = (float)args.Time;
		window.KeyDown += args => KeyEvent(args.Key, true);
		window.KeyUp += args => KeyEvent(args.Key, false);
		window.MouseDown += args => MouseEvent(args.Button, true);
		window.MouseUp += args => MouseEvent(args.Button, false);
		window.MouseMove += args => io.AddMousePosEvent(args.X, args.Y);
		window.MouseWheel += args => io.AddMouseWheelEvent(args.OffsetX, args.OffsetY);
	}

	private static void MouseEvent(MouseButton button, bool down)
	{
		ImGuiIOPtr io = ImGui.GetIO();
		io.AddMouseButtonEvent((int) button, down);
	}

	private static void KeyEvent(Keys key, bool down)
	{
		void KeyEvent(ImGuiKey imGuiKey)
		{
			ImGuiIOPtr io = ImGui.GetIO();
			io.AddKeyEvent(imGuiKey, down);
		}

		if(keyMapping.TryGetValue(key, out var value))
		{
			KeyEvent(value);
		}
		//TODO: Report unknown key events
	}

	private static readonly Dictionary<Keys, ImGuiKey> keyMapping = CreateKeyMapping();

	private static Dictionary<Keys, ImGuiKey> CreateKeyMapping()
	{
		Dictionary<Keys, ImGuiKey> mapping = [];
		var imguiKeys = Enum.GetNames<ImGuiKey>().Select(name => name.ToLowerInvariant()).Zip(Enum.GetValues<ImGuiKey>(), (Name, Key) => (Name, Key)).ToList();
		var opentkKeys = Enum.GetNames<Keys>().Select(name => name.ToLowerInvariant()).Zip(Enum.GetValues<Keys>(), (Name, Key) => (Name, Key)).ToList();
		// set exact case-invariant name matches
		foreach (var imgui in imguiKeys)
		{
			foreach (var (Name, Key) in opentkKeys.Where(d => d.Name == imgui.Name))
			{
				mapping[Key] = imgui.Key;
			}
		}
		mapping[Keys.Left] = ImGuiKey.LeftArrow;
		mapping[Keys.Right] = ImGuiKey.RightArrow;
		mapping[Keys.Up] = ImGuiKey.UpArrow;
		mapping[Keys.Down] = ImGuiKey.DownArrow;
		for (int i = 0; i < 10; i++)
		{
			mapping[Keys.D0 + i] = ImGuiKey._0 + i;
		}
		return mapping;
	}
}
