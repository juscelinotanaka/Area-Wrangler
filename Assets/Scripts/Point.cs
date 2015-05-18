using System;

[Serializable]
public class Point { 
	public float x, y;
	public Point (float f, float t) {
		this.x = f;
		this.y = t;
	}
	public Point (Point p) {
		this.x = p.x;
		this.y = p.y;
	}
}