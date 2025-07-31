using ImGuiNET;
using OpenTK.Mathematics;
using System;
using System.Diagnostics;
using System.Linq.Expressions;

namespace Zenseless.OpenTK.GUI.PropertyGUI;
/// <summary>
/// The static class ImGuiProperty provides methods to create ImGui controls for properties.
/// </summary>
public static class ImGuiProperty
{
	/// <summary>
	/// Renders a checkbox UI element and updates the associated property value if the checkbox state changes.
	/// </summary>
	/// <param name="propertyExpression">A property expression.</param>
	/// <returns><see langword="true"/> if the checkbox state was changed by the user; otherwise, <see langword="false"/>.</returns>
	/// <exception cref="ArgumentException">Thrown if the property value is not of type <see langword="bool"/>.</exception>
	public static bool CheckBox<PropertyType>(Expression<Func<PropertyType>> propertyExpression)
		=> CheckBox(Property.FromExpression(propertyExpression));

	/// <summary>
	/// Renders a checkbox UI element and updates the associated property value if the checkbox state changes.
	/// </summary>
	/// <param name="property">The property representing the checkbox state. The property's value must be of type <see langword="bool"/>.</param>
	/// <returns><see langword="true"/> if the checkbox state was changed by the user; otherwise, <see langword="false"/>.</returns>
	/// <exception cref="ArgumentException">Thrown if the <paramref name="property"/> value is not of type <see langword="bool"/>.</exception>
	public static bool CheckBox(Property property) => CheckBox(property.PropertyInfo.Name, property);

	/// <summary>
	/// Renders a checkbox UI element and updates the associated property value if the checkbox state changes.
	/// </summary>
	/// <param name="label">The label to display next to the checkbox.</param>
	/// <param name="property">The property representing the checkbox state. The property's value must be of type <see langword="bool"/>.</param>
	/// <returns><see langword="true"/> if the checkbox state was changed by the user; otherwise, <see langword="false"/>.</returns>
	/// <exception cref="ArgumentException">Thrown if the <paramref name="property"/> value is not of type <see langword="bool"/>.</exception>
	public static bool CheckBox(string label, Property property)
	{
		switch (property.Value)
		{
			case bool value:
				if (ImGui.Checkbox(label, ref value))
				{
					property.SetValue(value);
					return true;
				}
				break;
			default:
				throw new ArgumentException($"Property {property.PropertyInfo.Name} has value of type {property.Value?.GetType().Name ?? "null"} but expected type is {typeof(bool).Name}");
		}
		return false;
	}

	/// <summary>
	/// Represents a property with a label and a strongly-typed value.
	/// </summary>
	/// <typeparam name="PropertyType">The type of the value associated with the property.</typeparam>
	/// <param name="Label">The label/name of the property.</param>
	/// <param name="Value">The value of the property.</param>
	public record class TypedProperty<PropertyType>(in string Label, PropertyType Value)
	{
		/// <summary>
		/// Gets or sets the value of the property.
		/// </summary>
		public PropertyType Value = Value;
	}

	/// <summary>
	/// Handles property unpacking for a control function that operates on a strongly-typed property.
	/// </summary>
	/// <param name="propertyExpression">A property expression.</param>
	/// <param name="control">Callback that can be used to display a control element for the property.</param>
	/// <returns>The return value of the callback <paramref name="control"/>.</returns>
	/// <exception cref="ArgumentException">Thrown if there is a mismatch between the type of the property and <typeparamref name="PropertyType"/></exception>
	public static bool Control<PropertyType>(Expression<Func<PropertyType>> propertyExpression, Func<TypedProperty<PropertyType>, bool> control)
		=> Control(Property.FromExpression(propertyExpression), control);

	/// <summary>
	/// Handles property unpacking for a control function that operates on a strongly-typed property.
	/// </summary>
	/// <param name="property">The property representing the control state.</param>
	/// <param name="control">Callback that can be used to display a control element for the property.</param>
	/// <returns>The return value of the callback <paramref name="control"/>.</returns>
	/// <exception cref="ArgumentException">Thrown if there is a mismatch between the type of the property and <typeparamref name="PropertyType"/></exception>
	public static bool Control<PropertyType>(this Property property, Func<TypedProperty<PropertyType>, bool> control)
	{
		if(property.Value is PropertyType value)
		{
			var info = new TypedProperty<PropertyType>(property.PropertyInfo.Name, value);
			if (control(info))
			{
				property.SetValue(info.Value);
				return true;
			}
		}
		else
		{
			throw new ArgumentException($"Property {property.PropertyInfo.Name} has value of type {property.Value?.GetType().Name ?? "null"} but expected type is {typeof(PropertyType).Name}");
		}
		return false;
	}

	/// <summary>
	/// Renders a slider UI element and updates the associated property value if the slider state changes.
	/// </summary>
	/// <param name="propertyExpression">A property expression.</param>
	/// <param name="min">Slider minimal value</param>
	/// <param name="max">Slider maximal value</param>
	/// <returns><see langword="true"/> if the slider state was changed by the user; otherwise, <see langword="false"/>.</returns>
	/// <exception cref="ArgumentException">Thrown if the property value is not of a type for which a slider is available.</exception>
	public static bool Slider<PropertyType>(Expression<Func<PropertyType>> propertyExpression, float min, float max) 
		=> Slider(Property.FromExpression(propertyExpression), min, max);

	/// <summary>
	/// Renders a slider UI element and updates the associated property value if the slider state changes.
	/// </summary>
	/// <param name="property">The property representing the slider state.</param>
	/// <param name="min">Slider minimal value</param>
	/// <param name="max">Slider maximal value</param>
	/// <returns><see langword="true"/> if the slider state was changed by the user; otherwise, <see langword="false"/>.</returns>
	/// <exception cref="ArgumentException">Thrown if the <paramref name="property"/> value is not of a type for which a slider is available.</exception>
	public static bool Slider(Property property, float min, float max) => Slider(property.PropertyInfo.Name, property, min, max);

	/// <summary>
	/// Renders a slider UI element and updates the associated property value if the slider state changes.
	/// </summary>
	/// <param name="label">The label to display next to the checkbox.</param>
	/// <param name="property">The property representing the slider state.</param>
	/// <param name="min">Slider minimal value</param>
	/// <param name="max">Slider maximal value</param>
	/// <returns><see langword="true"/> if the slider state was changed by the user; otherwise, <see langword="false"/>.</returns>
	/// <exception cref="ArgumentException">Thrown if the <paramref name="property"/> value is not of a type for which a slider is available.</exception>
	public static bool Slider(string label, Property property, float min, float max)
	{
		switch (property.Value)
		{
			case int value:
				if (ImGui.SliderInt(label, ref value, (int)min, (int)max))
				{
					property.SetValue(value);
					return true;
				}
				break;
			case float value:
				if (ImGui.SliderFloat(label, ref value, min, max))
				{
					property.SetValue(value);
					return true;
				}
				break;
			case Vector2 value:
				{
					System.Numerics.Vector2 v = value.ToSystemNumerics();
					if (ImGui.SliderFloat2(label, ref v, min, max))
					{
						property.SetValue(v.ToOpenTK());
						return true;
					}
				}
				break;
			case Vector3 value:
				{
					System.Numerics.Vector3 v = value.ToSystemNumerics();
					if (ImGui.SliderFloat3(label, ref v, min, max))
					{
						property.SetValue(v.ToOpenTK());
						return true;
					}
				}
				break;
			case Vector4 value:
				{
					System.Numerics.Vector4 v = value.ToSystemNumerics();
					if (ImGui.SliderFloat4(label, ref v, min, max))
					{
						property.SetValue(v.ToOpenTK());
						return true;
					}
				}
				break;
			default:
				throw new ArgumentException($"Property {property.PropertyInfo.Name} has value of type {property.Value?.GetType().Name ?? "null"}. No slider available for this type.");
		}
		return false;
	}
}
