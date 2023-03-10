using ImGuiNET;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;

namespace Zenseless.OpenTK.GUI;

/// <summary>
/// Class that handles input for ImGui
/// </summary>
public class ImGuiInput
{
	/// <summary>
	/// Create a new instance
	/// </summary>
	/// <param name="window">A <see cref="NativeWindow"/> for connecting input event handler.</param>
	public ImGuiInput(NativeWindow window)
	{
		ImGuiHelper.AssureContextCreated();

		ImGuiIOPtr io = ImGui.GetIO();
		io.KeyMap[(int)ImGuiKey.Tab] = (int)Keys.Tab;
		io.KeyMap[(int)ImGuiKey.LeftArrow] = (int)Keys.Left;
		io.KeyMap[(int)ImGuiKey.RightArrow] = (int)Keys.Right;
		io.KeyMap[(int)ImGuiKey.UpArrow] = (int)Keys.Up;
		io.KeyMap[(int)ImGuiKey.DownArrow] = (int)Keys.Down;
		io.KeyMap[(int)ImGuiKey.PageUp] = (int)Keys.PageUp;
		io.KeyMap[(int)ImGuiKey.PageDown] = (int)Keys.PageDown;
		io.KeyMap[(int)ImGuiKey.Home] = (int)Keys.Home;
		io.KeyMap[(int)ImGuiKey.End] = (int)Keys.End;
		io.KeyMap[(int)ImGuiKey.Delete] = (int)Keys.Delete;
		io.KeyMap[(int)ImGuiKey.Backspace] = (int)Keys.Backspace;
		io.KeyMap[(int)ImGuiKey.Enter] = (int)Keys.Enter;
		io.KeyMap[(int)ImGuiKey.Escape] = (int)Keys.Escape;
		io.KeyMap[(int)ImGuiKey.A] = (int)Keys.A;
		io.KeyMap[(int)ImGuiKey.C] = (int)Keys.C;
		io.KeyMap[(int)ImGuiKey.V] = (int)Keys.V;
		io.KeyMap[(int)ImGuiKey.X] = (int)Keys.X;
		io.KeyMap[(int)ImGuiKey.Y] = (int)Keys.Y;
		io.KeyMap[(int)ImGuiKey.Z] = (int)Keys.Z;

		io.DeltaTime = 1f / 60f;

		window.TextInput += args => PressChar((char)args.Unicode);
	}

	/// <summary>
	/// Update the ImGui input state
	/// </summary>
	/// <param name="mouseState">The <see cref="MouseState"/>.</param>
	/// <param name="keyboardState">The <see cref="KeyboardState"/>.</param>
	/// <param name="deltaTime">Delta time in seconds.</param>
	public void Update(MouseState mouseState, KeyboardState keyboardState, float deltaTime)
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
