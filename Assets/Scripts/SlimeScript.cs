using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D))]

public class SlimeScript : MonoBehaviour {

	public Vector2 direction;
	public float value;

	Text text;

	public void CollisionReaction (Collision2D col) {
		if (col.gameObject.tag == "Line") {
			direction = Vector3.Reflect(direction, col.contacts[0].normal);
			
			if (Random.Range(0f, 1f) < 0.25f && (direction == Vector2.up || direction == -Vector2.up)) { // if going up or down
				direction += Random.Range(-1, 2) * Vector2.right; // randomize a possible right or left change
			}
			if (Random.Range(0f, 1f) < 0.25f && (direction == Vector2.right || direction == -Vector2.right)) { // if going left or right
				direction += Random.Range(-1, 2) * Vector2.up; // randomize a possible up or down change
			}
		}
	}

	void OnCollisionEnter2D (Collision2D col) {
		CollisionReaction (col);
	}

	void OnCollisionStay2D (Collision2D col) {
		//CollisionReaction (col);
//		if (col.gameObject.tag == "Line") {
//			direction = Vector3.Reflect(direction, col.contacts[0].normal);
//			
//			if (Random.Range(0f, 1f) < 0.25f && (direction == Vector2.up || direction == -Vector2.up)) { // if going up or down
//				direction += Random.Range(-1, 2) * Vector2.right; // randomize a possible right or left change
//			}
//			if (Random.Range(0f, 1f) < 0.25f && (direction == Vector2.right || direction == -Vector2.right)) { // if going left or right
//				direction += Random.Range(-1, 2) * Vector2.up; // randomize a possible up or down change
//			}
//		}
	}

//	void OnTriggerStay2D(Collider2D col) {
//		if (col.tag == "Line") {
//			LinesManager.Instance.overLine = true;
//		}
//	}

//	void OnTriggerExit2D(Collider2D col) {
//		if (col.tag == "Line") {
//			LinesManager.Instance.overLine = false;
//		}
//	}

	// Use this for initialization
	void Start () {
		transform.localScale = new Vector3 (0.3f, 0.3f, 0.3f);
		do {
			direction = new Vector2 (Random.Range (-1, 2), Random.Range (-1, 2));
		} while (direction == Vector2.zero);
		text = GetComponentInChildren<Text> ();
		text.text = value.ToString ();
	}

	void Update () {
		if (Input.GetKey (KeyCode.RightBracket)) {
			Time.timeScale += 0.5f;
		}
		if (Input.GetKey (KeyCode.LeftBracket)) {
			Time.timeScale -= 0.5f;
		}
		if (Input.GetKey (KeyCode.P)) {
			Time.timeScale = 1f;
		}
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		GetComponent<Rigidbody2D> ().velocity = (direction);
	}
}
