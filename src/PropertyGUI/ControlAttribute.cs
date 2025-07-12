using System;

namespace Zenseless.OpenTK.GUI.PropertyGUI;

/// <summary>
/// Specifies metadata for a property to control its order in a user interface or processing context.
/// </summary>
/// <remarks>This attribute is intended to be applied to properties to define their relative order. The order
/// value can be used by frameworks or tools to arrange properties in a specific sequence.</remarks>
/// <param name="Order">The order value that determines the relative position of the property. Lower values indicate higher priority or earlier placement.</param>
[AttributeUsage(AttributeTargets.Property)]
public class ControlAttribute(int Order) : Attribute
{
	/// <summary>
	/// The order value that determines the relative position of the property.
	/// </summary>
	public int Order { get; } = Order;
}
