using Zenseless.OpenTK.GUI.PropertyGUI;

class MyPropertyControls
{
	[Slider(0f, 1f)] public float Value { get; set; } = 0.5f;
	[Bool] public bool CollisionResponse { get; set; } = true;
	[Slider(0.01f, 0.5f, -1)] public float ParticleSize { get; set; } = 0.1f;
	[Slider(0f, 1f)] public float Value2 { get; set; } = 0.9f;
};
