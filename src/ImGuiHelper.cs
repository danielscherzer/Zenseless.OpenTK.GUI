using ImGuiNET;
using OpenTK.Mathematics;
using System;

namespace Zenseless.OpenTK.GUI;

/// <summary>
/// Helper functions for using ImGui with OpenTK
/// </summary>
public class ImGuiHelper
{
	internal static void AssureContextCreated()
	{
		IntPtr context = ImGui.GetCurrentContext();
		if (context == IntPtr.Zero)
		{
			context = ImGui.CreateContext();
			ImGui.SetCurrentContext(context);
		}
	}

	/// <summary>
	/// Create a SliderFloat2 with <see cref="Vector2"/> input.
	/// </summary>
	/// <param name="label">The label of the slider</param>
	/// <param name="value">The <see cref="Vector2"/> value to edit.</param>
	public static void SliderFloat(string label, ref Vector2 value)
	{
		System.Numerics.Vector2 sysV = value.ToSystemNumerics();
		if (ImGui.SliderFloat2(label, ref sysV, -1f, 1f))
		{
			value = sysV.ToOpenTK();
		}
	}

	/// <summary>
	/// Create a SliderFloat3 with <see cref="Vector3"/> input.
	/// </summary>
	/// <param name="label">The label of the slider</param>
	/// <param name="value">The <see cref="Vector3"/> value to edit.</param>
	public static void SliderFloat(string label, ref Vector3 value)
	{
		System.Numerics.Vector3 sysV = value.ToSystemNumerics();
		if (ImGui.SliderFloat3(label, ref sysV, -1f, 1f))
		{
			value = sysV.ToOpenTK();
		}
	}

	/// <summary>
	/// Create a SliderFloat4 with <see cref="Vector4"/> input.
	/// </summary>
	/// <param name="label">The label of the slider</param>
	/// <param name="value">The <see cref="Vector4"/> value to edit.</param>
	public static void SliderFloat(string label, ref Vector4 value)
	{
		System.Numerics.Vector4 sysV = value.ToSystemNumerics();
		if (ImGui.SliderFloat4(label, ref sysV, -1f, 1f))
		{
			value = sysV.ToOpenTK();
		}
	}

	/// <summary>
	/// Create a ColorEdit3 with <see cref="Vector3"/> input.
	/// </summary>
	/// <param name="label">The label of the slider</param>
	/// <param name="value">The <see cref="Vector3"/> value to edit.</param>
	public static void ColorEdit(string label, ref Vector3 value)
	{
		System.Numerics.Vector3 sysColor = value.ToSystemNumerics();
		if (ImGui.ColorEdit3(label, ref sysColor))
		{
			value = sysColor.ToOpenTK();
		}
	}

	/// <summary>
	/// Create a ColorEdit4 with <see cref="Vector4"/> input.
	/// </summary>
	/// <param name="label">The label of the slider</param>
	/// <param name="value">The <see cref="Vector4"/> value to edit.</param>
	public static void ColorEdit(string label, ref Vector4 value)
	{
		System.Numerics.Vector4 sysColor = value.ToSystemNumerics();
		if (ImGui.ColorEdit4(label, ref sysColor))
		{
			value = sysColor.ToOpenTK();
		}
	}
}
