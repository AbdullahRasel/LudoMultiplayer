using UnityEngine;

public static class Utils
{
	struct Box {
		public Vector2 p1, p2, p3, p4;
	};

	public static void DrawBoxCast2D (Vector2 origin, Vector2 size, float angle, Vector2 dir, float dist, Color color)
	{
		Box b1 = new Box ();
		Box b2 = new Box ();

		float w = size.x * 0.5f;
		float h = size.y * 0.5f;

		b1.p1 = new Vector2 (-w, h);
		b1.p2 = new Vector2 (w, h);
		b1.p3 = new Vector2 (-w, -h);
		b1.p4 = new Vector2 (w, -h);

		Quaternion rot = Quaternion.AngleAxis (angle, new Vector3 (0f, 0f, 1f));
		b1.p1 = rot * b1.p1;
		b1.p2 = rot * b1.p2;
		b1.p3 = rot * b1.p3;
		b1.p4 = rot * b1.p4;

		b1.p1 += origin;
		b1.p2 += origin;
		b1.p3 += origin;
		b1.p4 += origin;

		Vector2 rDist = dir.normalized * dist;
		b2.p1 = b1.p1 + rDist;
		b2.p2 = b1.p2 + rDist;
		b2.p3 = b1.p3 + rDist;
		b2.p4 = b1.p4 + rDist;

		DrawBox (b1, color);
		DrawBox (b2, color);

		Debug.DrawLine (b1.p1, b2.p1, Color.gray, 1f);
		Debug.DrawLine (b1.p2, b2.p2, Color.gray, 1f);
		Debug.DrawLine (b1.p3, b2.p3, Color.gray, 1f);
		Debug.DrawLine (b1.p4, b2.p4, Color.gray, 1f);
	}

	private static void DrawBox (Box box, Color color)
	{
		Debug.DrawLine (box.p1, box.p2, color, 1f);
		Debug.DrawLine (box.p1, box.p3, color, 1f);
		Debug.DrawLine (box.p3, box.p4, color, 1f);
		Debug.DrawLine (box.p2, box.p4, color, 1f);
	}
}
