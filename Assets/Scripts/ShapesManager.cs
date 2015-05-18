using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class ShapesManager : MonoBehaviour {

	public static ShapesManager Instance;

	static List<Point> Graph = new List<Point> ();
	static List<List<Point>> Shapes = new List<List<Point>>();
	static int[,] graph;
	static List<int[]> cycles = new List<int[]>();

	public GameObject AreaText;

	void Awake () {
		if (Instance == null) {
			Instance = this;
		}
	}


	void LookForCycles () {
		graph = new int[GridManager.Instance.rows * GridManager.Instance.cols, 2];
		for (int i = 0; i < Graph.Count; i++) {
			graph [i, 0] = (int) Graph [i].x;
			graph [i, 1] = (int) Graph [i].y;
		}
		
		for (int i = 0; i < graph.GetLength (0); i++) {
			for (int j = 0; j < graph.GetLength (1); j++) {
				findNewCycles (new int[] { graph [i, j] });
			}
		}
	}
	
	public static void SetShapes () {
		int x, y;
		foreach (int[] cy in cycles) {
			List<Point> Shape = new List<Point> ();
			for (int i = 0; i < cy.Length; i++) {
				x = cy [i] % GridManager.Instance.cols - 1;
				if (x == -1) {
					x = GridManager.Instance.cols - 1;
				}
				y = Mathf.CeilToInt( cy [i] / (float)GridManager.Instance.rows ) - 1;
				Shape.Add (new Point (x,  y ));
			}
			Shapes.Add (Shape);
		}
	}

	public void AddEdgeToGraph( Point p) {
		print ("Point added From: " + p.x + " to: " + p.y);
		Graph.Add (new Point(p));
	}

	public void AddEdgeToGraph( int from, int to) {
		print ("Point added From: " + from + " to: " + to);
		Graph.Add (new Point(from, to));
	}

	public List<List<Point>> LookForShapes() {
		LookForCycles ();
		SetShapes ();
		return Shapes;
	}

	public int lastShape = 0;

	//Debug Function
	public void PrintShapeAreas () {
		print ("in: " + lastShape + " " + Shapes.Count);
		for (int i = lastShape; i < Shapes.Count; i++) {
			List<Point> Shape = Shapes[i];

			Point center = GetCentroid(Shape);
			float pX, pY;
			pX = GridManager.Instance.startX + GridManager.Instance.distX * center.x;
			pY = GridManager.Instance.startY + GridManager.Instance.distY * center.y;
			Vector3 finalPos = Camera.main.ScreenToWorldPoint(new Vector3(pX, pY, 10));

			GameObject areaText = Instantiate(AreaText, finalPos, transform.rotation) as GameObject;
			areaText.GetComponentInChildren<Text>().text = PolygonArea (Shape).ToString("F1");
			lastShape = Shapes.Count;
		}

	}


	/// <summary>
	/// Method to compute the centroid of a polygon. This does NOT work for a complex polygon.
	/// </summary>
	/// <param name="poly">points that define the polygon</param>
	/// <returns>centroid point, or Point.Empty if something wrong</returns>
	public static Point GetCentroid(List<Point> poly)
	{
		float accumulatedArea = 0.0f;
		float centerx = 0.0f;
		float centery = 0.0f;
		
		for (int i = 0, j = poly.Count - 1; i < poly.Count; j = i++)
		{
			float temp = poly[i].x * poly[j].y - poly[j].x * poly[i].y;
			accumulatedArea += temp;
			centerx += (poly[i].x + poly[j].x) * temp;
			centery += (poly[i].y + poly[j].y) * temp;
		}

		accumulatedArea = Mathf.Abs (accumulatedArea);
		
		if (accumulatedArea < 1E-7f) {
			print ("null");
			return null;  // Avoid division by zero
		}
		
		accumulatedArea *= 3f;
		return new Point(Mathf.Abs(centerx / accumulatedArea), Mathf.Abs(centery / accumulatedArea));
	}
	
	// Use this for initialization
	void Start () {
		//DebugTest ();
		//PrintShapeAreas ();

	}
	
	// Update is called once per frame
	void Update () {
	
	}


	public static double PolygonArea(List<Point> polygon)
	{
		int i,j;
		double area = 0; 
		
		for (i=0; i < polygon.Count; i++) {
			j = (i + 1) % polygon.Count;
			
			area += polygon[i].x * polygon[j].y;
			area -= polygon[i].y * polygon[j].x;
		}
		
		area /= 2;
		return (area < 0 ? -area : area);
	}

	// Functions to find the cycles
	static void findNewCycles(int[] path)
	{
		int n = path[0];
		int x;
		int[] sub = new int[path.Length + 1];
		
		for (int i = 0; i < graph.GetLength(0); i++)
			for (int y = 0; y <= 1; y++)
				if (graph[i, y] == n)
					//  edge referes to our current node
			{
				x = graph[i, (y + 1) % 2];
				if (!visited(x, path))
					//  neighbor node not on path yet
				{
					sub[0] = x;
					System.Array.Copy(path, 0, sub, 1, path.Length);
					//  explore extended path
					findNewCycles(sub);
				}
				else if ((path.Length > 2) && (x == path[path.Length - 1]))
					//  cycle found
				{
					int[] p = normalize(path);
					int[] inv = invert(p);
					if (isNew(p) && isNew(inv))
						cycles.Add(p);
				}
			}
	}
	
	static bool equals(int[] a, int[] b)
	{
		bool ret = (a[0] == b[0]) && (a.Length == b.Length);
		
		for (int i = 1; ret && (i < a.Length); i++)
			if (a[i] != b[i])
		{
			ret = false;
		}
		
		return ret;
	}
	
	static int[] invert(int[] path)
	{
		int[] p = new int[path.Length];
		
		for (int i = 0; i < path.Length; i++)
			p[i] = path[path.Length - 1 - i];
		
		return normalize(p);
	}
	
	//  rotate cycle path such that it begins with the smallest node
	static int[] normalize(int[] path)
	{
		int[] p = new int[path.Length];
		int x = smallest(path);
		int n;
		
		System.Array.Copy(path, 0, p, 0, path.Length);
		
		while (p[0] != x)
		{
			n = p[0];
			System.Array.Copy(p, 1, p, 0, p.Length - 1);
			p[p.Length - 1] = n;
		}
		
		return p;
	}
	
	static bool isNew(int[] path)
	{
		bool ret = true;
		
		foreach(int[] p in cycles)
			if (equals(p, path))
		{
			ret = false;
			break;
		}
		
		return ret;
	}
	
	static int smallest(int[] path)
	{
		int min = path[0];
		
		foreach (int p in path)
			if (p < min)
				min = p;
		
		return min;
	}
	
	static bool visited(int n, int[] path)
	{
		bool ret = false;
		
		foreach (int p in path)
			if (p == n)
		{
			ret = true;
			break;
		}
		
		return ret;
	}
}
