using OpenTK.Mathematics;
using System;
using Zenseless.Patterns;

namespace Example;

internal static class Helper
{
	public static Vector2[] CreateRandomTriangles(int count)
	{
		Vector2[] points = new Vector2[count * 3];
		const float size = 0.02f;
		var rnd = new Random();
		float rnd1() => rnd.NextFloat(-1f, 1f); // net6 NextSingle has same speed
		for (int i = 0; i < count * 3; i += 3)
		{
			var v = new Vector2(rnd1(), rnd1());
			points[i] = v;
			points[i + 1] = v + new Vector2(size, 0f);
			points[i + 2] = v + new Vector2(size);
		}
		return points;
	}
}