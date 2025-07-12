using ImGuiNET;
using System.Diagnostics;
using System.Reflection;

namespace Zenseless.OpenTK.GUI.PropertyGUI;

/// <summary>
/// Represents a checkbox control.
/// </summary>
/// <param name="instance">An object instance that has the property refered to in <paramref name="propertyInfo"/></param>
/// <param name="propertyInfo">The <see cref="PropertyInfo"/>.</param>
public class Checkbox(object instance, PropertyInfo propertyInfo) : Control(instance, propertyInfo)
{
	/// <summary>
	/// Renders a checkbox UI control for editing the value of the associated property.
	/// </summary>
	public override void Draw()
	{
		switch (Value)
		{
			case bool value:
				if(ImGui.Checkbox(PropertyInfo.Name, ref value))
				{
					SetValue(value);
				}
				break;
			default:
				Trace.TraceError($"No checkbox for {PropertyInfo.Name} with value of type {Value?.GetType().Name ?? "null"}");
				break;
		}
	}
}
