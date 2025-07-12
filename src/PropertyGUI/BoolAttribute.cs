using System;

namespace Zenseless.OpenTK.GUI.PropertyGUI;

/// <summary>
/// Specifies that a property is associated with a boolean GUI control.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class BoolAttribute(int Order = 0) : ControlAttribute(Order)
{
}