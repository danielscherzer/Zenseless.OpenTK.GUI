using OpenTK.Mathematics;
using Zenseless.OpenTK.GUI.PropertyGUI;

class SomeData
{
	[Slider(0f, 1f)] public float Value { get; set; } = 0.5f;
	[Bool] public bool CollisionResponse { get; set; } = true;
	[Slider(0.01f, 0.5f, -1)] public float ParticleSize { get; set; } = 0.1f;
	[Slider(0f, 1f)] public Vector2 Value2 { get; set; } = Vector2.UnitX;
	[Slider(0f, 1f)] public Vector3 Value3 { get; set; } = Vector3.One;
	public Vector4 Value4 { get; set; } = Vector4.UnitZ;
	public float Value5 { get; set; } = 0.5f;
	public float Value6 { get; set; } = 0.5f;
	public float Value7 { get; set; } = 0.5f;
};
