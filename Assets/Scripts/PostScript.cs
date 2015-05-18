using UnityEngine;
using System.Collections;

public class PostScript : MonoBehaviour {

	public PostState state;
	public PostState lastState;

	public enum PostState {
		Open,
		Selected,
		Hover,
		Closed
	}

	public Point point;

	public bool closed;

	// Update is called once per frame
	void Update () {
		if (state == PostState.Open) {
			GetComponent<SpriteRenderer> ().color = new Color (1, 1, 1, 0.25f);
		} else if (state == PostState.Hover) {
			GetComponent<SpriteRenderer> ().color = new Color (0f, 1f, 0.95f);
		} else if (state == PostState.Selected) {
			GetComponent<SpriteRenderer> ().color = Color.green;
		}	else if (state == PostState.Closed) {
			GetComponent<SpriteRenderer> ().color = new Color (0.65f, 0.49f, 0.1f);
		}
	}

	public void SetClosed () {
		state = PostState.Closed;
		closed = true;
	}

//	void OnTriggerStay2D (Collider2D col) {
//		if (col.tag == "Player") {
//			state = PostState.Selected;
//		} else if (col.tag == "Line") {
//			if (col.GetComponent<LineScript>().GetState() == LineScript.LineState.Dragging) {
//				state = PostState.Hover;
//			} else if (col.GetComponent<LineScript>().GetState() == LineScript.LineState.Done) {
//				state = PostState.Closed;
//				closed = true;
//			}
//		}
//	}

//	void OnTriggerEnter2D (Collider2D col) {
//		if (col.tag == "Player") {
//			state = PostState.Selected;
//		} else if (col.tag == "Line") {
//			if (col.GetComponent<LineScript>().GetState() == LineScript.LineState.Done) {
//				state = PostState.Closed;
//				closed = true;
//			} else if (col.GetComponent<LineScript>().GetState() == LineScript.LineState.Dragging) {
//				state = PostState.Hover;
//			}
//		} 
//	}



//	void OnTriggerExit2D (Collider2D col) {
//		if (closed) {
//			state = PostState.Closed;
//		} else {
//			state = PostState.Open;
//		}
//	}
}
