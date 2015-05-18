using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class LinesManager : MonoBehaviour {

	public static LinesManager Instance;
	public GameObject linePrefab;
	public Transform linesParent;

	public LineScript currentLine;
	
	public Point[] sqr = new Point[4];

	public bool checkContinuity;

	// Use this for initialization
	void Start () {
		if (Instance == null) {
			Instance = this;
		}
	}

	public void CreateOutline (Transform from, Transform to) {
		GameObject lineObj = Instantiate (linePrefab) as GameObject;
		lineObj.transform.SetParent (linesParent);
		LineScript newLine = lineObj.GetComponent<LineScript>();
		newLine.state = LineScript.LineState.Outline;
		newLine.startObject = from;
		newLine.endObject = to;
		newLine.SetOutline();
	}

	public void StartLine (Transform post, Transform playerTransform) {
		GameObject lineObj = Instantiate (linePrefab) as GameObject;
		lineObj.transform.SetParent (linesParent);
		LineScript newLine = lineObj.GetComponent<LineScript>();
		newLine.startObject = post;
		if (post.GetComponent<PostScript> ().closed) {
			checkContinuity = true;
		} else {
			checkContinuity = false;
		}
		newLine.endObject = playerTransform;
		newLine.SetDragging ();
		currentLine = newLine;
	}


	public void SnapLine (Transform post) {
		currentLine.endObject = post;
	}

	public void UndoSnapLine (Transform player) {
		currentLine.endObject = player;
	}


	// source: http://stackoverflow.com/questions/2432428/is-there-any-algorithm-for-calculating-area-of-a-shape-given-co-ordinates-that-d
	double PolygonArea(Point[] polygon)
	{
		int i,j;
		double area = 0; 
		
		for (i=0; i < polygon.Length; i++) {
			j = (i + 1) % polygon.Length;
			
			area += polygon[i].x * polygon[j].y;
			area -= polygon[i].y * polygon[j].x;
		}
		
		area /= 2;
		return (area < 0 ? -area : area);
	}

	void FindShape() {

	}

	void CheckContinuity() {

	}

	public bool FinishLine(Transform endPost) {
		if (currentLine.state == LineScript.LineState.Dragging) { // if slime is over line doesnt finish the fence

			currentLine.endObject = endPost;
			if (checkContinuity && endPost.GetComponent<PostScript> ().closed) {
				CheckContinuity ();
			}
			if (endPost.GetComponent<PostScript> ().closed) {


				//print ("Area: " + PolygonArea (sqr));
			}
			currentLine.UpdateSettings ();
			currentLine.SetDone ();
			currentLine = null;
			GameMaster.GM.fencesUsed++;
			return true;
		} else {
			return false;
		}
	}

	public void CancelLine () {
		Destroy(currentLine.gameObject);
		currentLine = null;
	}
}
