using ImGuiNET;
using System.Reflection;

namespace Zenseless.OpenTK.GUI.PropertyGUI;

/// <summary>
/// Represents a slider control for adjusting numeric values within a specified range.
/// </summary>
/// <remarks>The <see cref="Slider"/> class provides functionality to render a slider control using ImGui,
/// allowing users to interactively adjust integer or floating-point values. The control is bound to a property of an
/// object instance and respects the constraints defined by the associated <see cref="SliderAttribute"/>.</remarks>
/// <param name="instance">An object instance that has the property refered to in <paramref name="propertyInfo"/></param>
/// <param name="propertyInfo">The <see cref="PropertyInfo"/>.</param>
/// <param name="attribute">The <see cref="SliderAttribute"/></param>
public class Slider(object instance, PropertyInfo propertyInfo, SliderAttribute attribute): Control(instance, propertyInfo)
{
	/// <summary>
	/// Renders a UI control for editing the value of the associated property.
	/// </summary>
	/// <remarks>The method dynamically determines the type of the property value and renders an appropriate slider
	/// control. For integer values, a slider with integer bounds is displayed. For floating-point values, a slider with 
	/// floating-point bounds is displayed. The control allows the user to modify the property's value within the specified range.</remarks>
	public override void Draw()
	{
		switch (Value)
		{
			case int value:
				ImGui.SliderInt(PropertyInfo.Name, ref value, (int)attribute.Min, (int)attribute.Max);
				SetValue(value);
				break;
			case float value:
				ImGui.SliderFloat(PropertyInfo.Name, ref value, attribute.Min, attribute.Max);
				SetValue(value);
				break;
			default: break;
		}
	}
}
