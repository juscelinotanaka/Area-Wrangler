using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class LineScript : MonoBehaviour {

	public Transform startObject, endObject;

	LineRenderer line;

	BoxCollider2D box;
	
	public float lineWidth; // use the same as you set in the line renderer.

	Text text;

	public Point startPoint, endPoint;

	bool hasText = true;

	public LineState state;
	public enum LineState {
		Dragging,
		Blocked,
		Done,
		Outline
	}

	public float length;
	public Vector3 direction;
	
	void Start()
	{
		line = GetComponent<LineRenderer> ();
		box = GetComponent<BoxCollider2D> ();

		line.SetWidth (lineWidth, lineWidth);
		box.offset = Vector3.zero;

		text = GetComponentInChildren<Text> ();
		text.text = "2.665";

		UpdateSettings ();
	}


	IEnumerator UpdateNextFrame () {
		yield return new WaitForEndOfFrame();
		UpdateSettings ();
	}

	public IEnumerator UpdateText () {
		yield return new WaitForEndOfFrame();
		direction = startObject.position - endObject.position;
		
		direction = direction.normalized;
		
		if (direction.y < 0) {
			text.transform.eulerAngles = new Vector3 (0, 0, 0);
		} else if (direction.x > 0 || direction.x < 0) {
			text.transform.up = Vector3.up;
			text.transform.eulerAngles = new Vector3 (0, 0, 0);
		}
	}

	public int CoordinateToID (Point p) {
		return (int) (p.x + 1 + p.y * GridManager.Instance.cols);
	}

	public void CreateConnection () {
		RaycastHit2D[] hits = Physics2D.RaycastAll (startObject.position, direction, length);
		PostScript currentPost = null, lastPost = null;
		foreach (RaycastHit2D hit in hits) {
			if (hit.transform.gameObject.tag == "Post") {
				lastPost = currentPost;
				currentPost = hit.transform.GetComponent<PostScript>();
				currentPost.SetClosed();
				if (lastPost != null) {
					ShapesManager.Instance.AddEdgeToGraph(CoordinateToID(lastPost.point), CoordinateToID(currentPost.point));
				}
				//print (hit.transform.gameObject.name);
			}
		}

		List<List<Point>> shapes = ShapesManager.Instance.LookForShapes();
		ShapesManager.Instance.PrintShapeAreas();
	}
	
	public void SetDone () {
		state = LineState.Done;
		StartCoroutine (UpdateNextFrame ());
		StartCoroutine (UpdateText ());
		StartCoroutine (SetState (LineState.Done));

		CreateConnection ();
	}

	public IEnumerator RemoveText () {
		yield return new WaitForEndOfFrame();
		Destroy(text.transform.parent.gameObject);
	}

	public IEnumerator SetState (LineState s) {
		yield return new WaitForEndOfFrame();
		
		state = s;
	}

	public void SetOutline () {
		gameObject.layer = LayerMask.GetMask ("Default");
		hasText = false;
		StartCoroutine (UpdateNextFrame ());
		StartCoroutine (RemoveText ());
		StartCoroutine (SetState (LineState.Outline));
	}

	public void SetDragging () {
		state = LineState.Dragging;
	}

	public LineState GetState() {
		return state;
	}

//	void OnTriggerEnter2D(Collider2D col) {
//		print ("trig: " + col.tag);
//	}
//
//	void OnCollisionEnter2D (Collision2D col) {
//		print ("col: " + col.gameObject.tag);
//	}

	public void UpdateSettings () {
		line.SetPosition(0, startObject.position);
		line.SetPosition(1, endObject.position);

		box.transform.position = startObject.position + (endObject.position - startObject.position) / 2;
		box.size = new Vector2(0.06f, length);
		box.transform.up = direction;
		
		length = Vector3.Distance(endObject.position, startObject.position);
		direction = endObject.position - startObject.position;
		direction = direction.normalized;


		if (hasText) {
			text.text = (length / GridManager.Unit).ToString ("F2");
			if (direction.y < 0) {
				text.transform.eulerAngles = new Vector3 (0, 0, 0);
			} else if (direction.x > 0 || direction.x < 0) {
				text.transform.up = Vector3.up;
				text.transform.eulerAngles = new Vector3 (0, 0, 0);
			}
		}


	}

	bool blocked = false;
	void FixedUpdate () {
		if (state == LineState.Dragging || state == LineState.Blocked) {
			blocked = false;

			RaycastHit2D[] hits = Physics2D.RaycastAll (startObject.position + direction * 0.25f, direction, length - 0.5f);
			foreach (RaycastHit2D hit in hits) {
				Debug.DrawLine(startObject.position + direction * 0.25f, hit.point);

				if (hit.transform.gameObject.tag == "Slime") {
					state = LineState.Blocked;
					blocked = true;
				} else if (hit.transform.gameObject.tag == "Line") {
					if (!hit.transform.gameObject.Equals (this.gameObject)) {
						state = LineState.Blocked;
						blocked = true;
						break;
					}
				}
			}
			if (!blocked) {
				state = LineState.Dragging;
			}
		}
	}
	
	void Update()
	{
		if (state == LineState.Dragging || state == LineState.Blocked) {
			UpdateSettings ();
			box.isTrigger = true;
		} else if (state == LineState.Done || state == LineState.Outline) {
			box.isTrigger = false;
		}

		if (state == LineState.Blocked) {
			line.SetColors (Color.red, Color.red);
		} else if (state == LineState.Outline) {
			line.SetColors (new Color(0.035f, 0.46f, 0.17f), new Color(0.035f, 0.46f, 0.17f));
		} else {
			line.SetColors (new Color(1f, 0.78f, 0.25f), new Color(1f, 0.78f, 0.25f));
		}

	}
}
