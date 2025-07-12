using System;

namespace Zenseless.OpenTK.GUI.PropertyGUI;

/// <summary>
/// Specifies that a property should be rendered as a slider control in a user interface.
/// </summary>
/// <remarks>Use this attribute to define a slider control for a property, specifying the minimum and maximum
/// values for the slider range. The slider can be used to adjust the property's value within the defined
/// range.</remarks>
[AttributeUsage(AttributeTargets.Property)]
public class SliderAttribute : ControlAttribute
{
	/// <summary>
	/// Specifies a range of values for a slider control in a user interface.
	/// </summary>
	/// <remarks>Use this attribute to define the minimum and maximum values for a slider control. The slider will
	/// allow selection of values within the specified range.</remarks>
	/// <param name="min">The minimum value of the slider range. Must be less than <paramref name="max"/>.</param>
	/// <param name="max">The maximum value of the slider range. Must be greater than <paramref name="min"/>.</param>
	/// <param name="order">The order in which the slider appears relative to other controls. Defaults to 0.</param>
	/// <exception cref="ArgumentException">Thrown if <paramref name="min"/> is greater than or equal to <paramref name="max"/>.</exception>
	public SliderAttribute(float min, float max, int order = 0): base(order)
	{
		if (min >= max) throw new ArgumentException("Min must be less than Max");
		Min = min;
		Max = max;
	}

	/// <summary>
	/// The minimum value of the slider range.
	/// </summary>
	public float Min { get; }
	/// <summary>
	///	The maximum value of the slider range.
	/// </summary>
	public float Max { get; }
}
