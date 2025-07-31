using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Zenseless.OpenTK.GUI.PropertyGUI;

/// <summary>
/// A record struct representing a property of an object instance.
/// </summary>
/// <param name="Instance">An object instance that has the property refered to in <paramref name="PropertyInfo"/></param>
/// <param name="PropertyInfo">The <see cref="PropertyInfo"/>.</param>
public readonly record struct Property(object Instance, PropertyInfo PropertyInfo)
{
	/// <summary>
	/// Create a <see cref="Property"/> from a property expression of the form <code>() => instance.Property</code>
	/// </summary>
	/// <typeparam name="PropertyType">The datatype of the property</typeparam>
	/// <param name="propertyExpression">A property expression of the form <code>() => instance.Property</code></param>
	/// <returns></returns>
	/// <exception cref="ArgumentException"></exception>
	/// <exception cref="InvalidOperationException"></exception>
	public static Property FromExpression<PropertyType>(Expression<Func<PropertyType>> propertyExpression)
	{
		if (propertyExpression.Body is MemberExpression memberExpression)
		{
			if (memberExpression?.Member is PropertyInfo propertyInfo)
			{
				if (memberExpression.Expression is null) throw new ArgumentException("Invalid expression given");
				if (Evaluate(memberExpression.Expression) is object instance)
				{
					return new Property(instance, propertyInfo);
				}
			}
		}
		throw new InvalidOperationException("Please provide a valid property expression, like '() => instance.PropertyName'.");
	}

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

	/// <summary>
	/// Gets the value of the property represented by this instance.
	/// </summary>
	public object? Value => PropertyInfo.GetValue(Instance);

	private static object? Evaluate(Expression e)
	{
		switch (e.NodeType)
		{
			case ExpressionType.Constant:
				return (e as ConstantExpression)?.Value;
			case ExpressionType.MemberAccess:
				{
					if (e is not MemberExpression propertyExpression) return null;
					var field = propertyExpression.Member as FieldInfo;
					var property = propertyExpression.Member as PropertyInfo;
					var container = propertyExpression.Expression == null ? null : Evaluate(propertyExpression.Expression);
					if (field != null)
						return field.GetValue(container);
					else if (property != null)
						return property.GetValue(container, null);
					else
						return null;
				}
			default:
				return null;
		}
	}
}
