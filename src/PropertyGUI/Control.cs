using ImGuiNET;
using System;
using System.Reflection;

namespace Zenseless.OpenTK.GUI.PropertyGUI;

/// <summary>
/// Represents a control for interacting with a property of an object instance.
/// </summary>
/// <remarks>The <see cref="Control"/> class provides functionality to display and modify the value of a property
/// on a given object instance. It supports drawing the property name and value, retrieving the current value, and
/// setting a new value with type validation.</remarks>
/// <param name="instance">An object instance that has the property refered to in <paramref name="propertyInfo"/></param>
/// <param name="propertyInfo">The <see cref="PropertyInfo"/>.</param>
public class Control(object instance, PropertyInfo propertyInfo)
{
	/// <summary>
	/// Renders the property name and its value as text in the current ImGui context.
	/// </summary>
	/// <remarks>This method uses ImGui to display the property name and value in the user interface. Ensure that an
	/// ImGui rendering context is active before calling this method.</remarks>
	public virtual void Draw()
	{
		ImGui.Text($"{PropertyInfo.Name}:{Value}");
	}

	/// <summary>
	/// Gets the value of the property represented by this instance.
	/// </summary>
	public object? Value => PropertyInfo.GetValue(Instance);

	/// <summary>
	/// An object instance that has the property refered to in <see cref="PropertyInfo"/>."/>
	/// </summary>
	public object Instance { get; } = instance;
	/// <summary>
	/// The <see cref="PropertyInfo"/>
	/// </summary>
	public PropertyInfo PropertyInfo { get; } = propertyInfo;

	/// <summary>
	/// Sets the value of the property represented by this instance.
	/// </summary>
	/// <remarks>This method validates that the provided <paramref name="value"/> is compatible with the property's
	/// type before attempting to set it. If the validation fails, an <see cref="InvalidCastException"/> is thrown.</remarks>
	/// <param name="value">The value to assign to the property. The type of the value must be compatible with the property's type.</param>
	/// <exception cref="InvalidCastException">Thrown if the type of <paramref name="value"/> is not assignable to the property's type.</exception>
	public void SetValue(object? value)
	{
		if (PropertyInfo.PropertyType.IsAssignableFrom(value?.GetType() ?? typeof(object)))
		{
			PropertyInfo.SetValue(Instance, value);
		}
		else
		{
			throw new InvalidCastException($"Cannot set property {PropertyInfo.Name} to value of type {value?.GetType().Name ?? "null"}");
		}
	}
}
