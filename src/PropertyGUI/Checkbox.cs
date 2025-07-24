using ImGuiNET;
using System.Diagnostics;
using System.Reflection;

namespace Zenseless.OpenTK.GUI.PropertyGUI;

/// <summary>
/// Represents a checkbox control.
/// </summary>
/// <param name="property">The property the control should display</param>
public class Checkbox(Property property) : Control(property)
{
	/// <summary>
	/// Renders a checkbox UI control for editing the value of the associated property.
	/// </summary>
	public override void Draw()
	{
		switch (Value)
		{
			case bool value:
				if(ImGui.Checkbox(Label, ref value))
				{
					SetValue(value);
				}
				break;
			default:
				Trace.TraceError($"No checkbox for {Property.PropertyInfo.Name} with value of type {Value?.GetType().Name ?? "null"}");
				break;
		}
	}
}
