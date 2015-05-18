using UnityEngine;
using System.Collections;

public class CharController : MonoBehaviour {

	public Transform currentPost, startPost;
	public bool draggingLine;
	
	// Update is called once per frame
	void FixedUpdate () {
		GetComponent<Rigidbody2D>().velocity = new Vector3 (Input.GetAxis ("Horizontal"), Input.GetAxis ("Vertical"), 0) * 2.5f;

	}

	void Update () {
		if (Input.GetButtonDown ("Set")) {
			if (!draggingLine && currentPost) {
				LinesManager.Instance.StartLine(currentPost, transform);
				startPost = currentPost;
				draggingLine = true;
			} else if (draggingLine && currentPost && currentPost != startPost) { // if the final post is different finishes the line
				draggingLine = !LinesManager.Instance.FinishLine(currentPost);
			} else if (draggingLine && currentPost && currentPost == startPost) { // if it is equal deletes the current line
				LinesManager.Instance.CancelLine();
				draggingLine = false;
			}
		}
		if (Input.GetKeyDown (KeyCode.R)) {
			Application.LoadLevel(Application.loadedLevel);
		}
	}

	void OnTriggerEnter2D (Collider2D col) {
		if (col.tag == "Post") {
			currentPost = col.transform;
			currentPost.GetComponent<PostScript>().state = PostScript.PostState.Hover;
			if (draggingLine) {
				LinesManager.Instance.SnapLine(currentPost);
			}
		} 
	}

	void OnCollisionEnter2D (Collision2D col) {
		if (col.gameObject.tag == "Slime") {
			GameMaster.GM.FinishGame();
		}
	}

	void OnTriggerExit2D (Collider2D col) {
		if (col.tag == "Post") {
			PostScript post = currentPost.GetComponent<PostScript>();
			if (post.closed) {
				post.state = PostScript.PostState.Closed;
			} else {
				post.state = PostScript.PostState.Open;
			}
			currentPost = null;

			if (draggingLine) {
				LinesManager.Instance.UndoSnapLine(transform);
			}
		}
	}
	

}
