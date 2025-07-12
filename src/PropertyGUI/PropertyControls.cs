using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Zenseless.OpenTK.GUI.PropertyGUI;

/// <summary>
/// Factory class for generating Controls based on properties of an object instance.
/// </summary>
public static partial class PropertyControls
{
	/// <summary>
	/// Creates UI controls based on the given object instances for all properties marked with <see cref="ControlAttribute"/>.
	/// </summary>
	/// <param name="instances">Object instances to search for <see cref="ControlAttribute"/></param>
	/// <returns>A <see cref="List{Control}"/></returns>
	/// <exception cref="ApplicationException"></exception>
	public static List<Control> CreateControls(params object[] instances)
	{
		static T GetAttribut<T>(PropertyInfo propertyInfo) where T : Attribute => propertyInfo.GetCustomAttribute<T>() ?? throw new ApplicationException($"Property {propertyInfo.Name} is marked with {typeof(T).Name} but no attribute found.");

		List<Control> controls = [];
		foreach (var instance in instances)
		{
			var guiControlProperties = instance.GetType().GetMembers().Where(type => Attribute.IsDefined(type, typeof(ControlAttribute), true)).Cast<PropertyInfo>()
				.OrderBy(property => GetAttribut<ControlAttribute>(property).Order);
			foreach(var property in guiControlProperties)
			{
				if (Attribute.IsDefined(property, typeof(SliderAttribute)))
				{
					var sliderAttribute = GetAttribut<SliderAttribute>(property);
					controls.Add(new Slider(instance, property, sliderAttribute));
				}
				else if (Attribute.IsDefined(property, typeof(BoolAttribute)))
				{
					var boolAttribute = GetAttribut<BoolAttribute>(property);
					controls.Add(new Checkbox(instance, property));
				}
			}
		}
		return controls;
	}
}
