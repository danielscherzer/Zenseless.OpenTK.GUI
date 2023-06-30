using ImGuiNET;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

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
		var imguiKeys = Enum.GetNames<ImGuiKey>().Select(name => name.ToLowerInvariant()).Zip(Enum.GetValues<ImGuiKey>(), (Name, Key) => (Name, Key)).ToList();
		var opentkKeys = Enum.GetNames<Keys>().Select(name => name.ToLowerInvariant()).Zip(Enum.GetValues<Keys>(), (Name, Key) => (Name, Key)).ToList();
		// set exact case-invariant name matches
		foreach (var imgui in imguiKeys)
		{
			foreach (var (Name, Key) in opentkKeys.Where(d => d.Name == imgui.Name))
			{
				io.KeyMap[(int)imgui.Key] = (int)Key;
			}
		}
		io.KeyMap[(int)ImGuiKey.LeftArrow] = (int)Keys.Left;
		io.KeyMap[(int)ImGuiKey.RightArrow] = (int)Keys.Right;
		io.KeyMap[(int)ImGuiKey.UpArrow] = (int)Keys.Up;
		io.KeyMap[(int)ImGuiKey.DownArrow] = (int)Keys.Down;
		for(int i =  0; i < 10; i++)
		{
			io.KeyMap[(int)ImGuiKey._0 + i] = (int)Keys.D0 + i;
		}
		io.DeltaTime = 1f / 60f;

		window.TextInput += args => PressChar((char)args.Unicode);
		window.UpdateFrame += args => Update(window.MouseState, window.KeyboardState, (float)args.Time);
	}

	/// <summary>
	/// Update the ImGui input state
	/// </summary>
	/// <param name="mouseState">The <see cref="MouseState"/>.</param>
	/// <param name="keyboardState">The <see cref="KeyboardState"/>.</param>
	/// <param name="deltaTime">Delta time in seconds.</param>
	private void Update(MouseState mouseState, KeyboardState keyboardState, float deltaTime)
	{
		ImGuiIOPtr io = ImGui.GetIO();
		io.DeltaTime = deltaTime;

		io.MouseDown[0] = mouseState[MouseButton.Left];
		io.MouseDown[1] = mouseState[MouseButton.Right];
		io.MouseDown[2] = mouseState[MouseButton.Middle];
		io.MouseWheel = mouseState.ScrollDelta.Y;
		io.MouseWheelH = mouseState.ScrollDelta.X;
		io.MousePos = mouseState.Position.ToSystemNumerics();

		foreach (Keys key in Enum.GetValues(typeof(Keys)))
		{
			if (key == Keys.Unknown)
			{
				continue;
			}
			io.KeysDown[(int)key] = keyboardState.IsKeyDown(key);
		}

		foreach (var c in _pressedChars)
		{
			io.AddInputCharacter(c);
		}
		_pressedChars.Clear();

		io.KeyCtrl = keyboardState.IsKeyDown(Keys.LeftControl) || keyboardState.IsKeyDown(Keys.RightControl);
		io.KeyAlt = keyboardState.IsKeyDown(Keys.LeftAlt) || keyboardState.IsKeyDown(Keys.RightAlt);
		io.KeyShift = keyboardState.IsKeyDown(Keys.LeftShift) || keyboardState.IsKeyDown(Keys.RightShift);
		io.KeySuper = keyboardState.IsKeyDown(Keys.LeftSuper) || keyboardState.IsKeyDown(Keys.RightSuper);
	}

	private readonly List<char> _pressedChars = new();

	private void PressChar(char keyChar) => _pressedChars.Add(keyChar);
}
