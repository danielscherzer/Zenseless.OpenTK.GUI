using ImGuiNET;
using OpenTK.Mathematics;
using System;

namespace Zenseless.OpenTK.GUI;

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

	public static void VecSlider(string label, ref Vector3 v)
	{
		System.Numerics.Vector3 sysV = v.ToSystemNumerics();
		if (ImGui.SliderFloat3(label, ref sysV, -1f, 1f))
		{
			v = sysV.ToOpenTK();
		}
	}
	
	public static void VecSlider(string label, ref Vector4 v)
	{
		System.Numerics.Vector4 sysV = v.ToSystemNumerics();
		if (ImGui.SliderFloat4(label, ref sysV, -1f, 1f))
		{
			v = sysV.ToOpenTK();
		}
	}

	public static void ColorEdit(string label, ref Vector3 color)
	{
		System.Numerics.Vector3 sysColor = color.ToSystemNumerics();
		if (ImGui.ColorEdit3(label, ref sysColor))
		{
			color = sysColor.ToOpenTK();
		}
	}

	public static void ColorEdit(string label, ref Vector4 color)
	{
		System.Numerics.Vector4 sysColor = color.ToSystemNumerics();
		if (ImGui.ColorEdit4(label, ref sysColor))
		{
			color = sysColor.ToOpenTK();
		}
	}
}
