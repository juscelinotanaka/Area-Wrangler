using UnityEngine;
using System.Collections;
using System;

public class GridManager : MonoBehaviour {

	public static GridManager Instance;
	public int rows = 5, cols = 5;

	public Transform postsParent;
	public GameObject post;
	
	public GameObject[,] posts;

	float scrWidth, scrHeight;
	float sqrHeight, sqrWidth;

	public float startX, startY, distX, distY;

	public static float Unit;

	public Transform[] startPosts;

	// w: 860, H: 740

	void Awake () {

		if (Instance == null) {
			Instance = this;
		}

		scrWidth = Screen.width;
		scrHeight = Screen.height;
		posts = new GameObject[rows,cols];
		
		// take the square are to place the posts relative to the actual resolution of the game
		sqrHeight = 730f * scrHeight / 1012f;
		sqrWidth = 850f * sqrHeight / 730f;
		
		// divides the space between the posts
		distX = sqrWidth / (float) (cols - 1);
		distY = sqrHeight / (float) (rows - 1);


		
		if (distX < distY) {
			distY = distX;

			startX = (scrWidth - sqrWidth) / 2;
			sqrHeight = distY * (rows-1);
			startY = (scrHeight - sqrHeight) / 2;
		} else {
			distX = distY;
			// take the starting X and Y positions of the grid
			sqrWidth = distX * (cols -1);
			startX = (scrWidth - sqrWidth) / 2;
			startY = (scrHeight - sqrHeight) / 2;
		}

		float outlineOffset = 40f * scrHeight / 1012f;
		startPosts [0].position = Camera.main.ScreenToWorldPoint( new Vector3 (startX - outlineOffset, startY - outlineOffset, 10) );
		startPosts [1].position = Camera.main.ScreenToWorldPoint( new Vector3 (startX + sqrWidth + outlineOffset, startY - outlineOffset, 10) );
		startPosts [2].position = Camera.main.ScreenToWorldPoint( new Vector3 (startX + sqrWidth + outlineOffset, startY + sqrHeight + outlineOffset, 10) );
		startPosts [3].position = Camera.main.ScreenToWorldPoint( new Vector3 (startX - outlineOffset, startY + sqrHeight + outlineOffset, 10) );

		// instantiate all points on grid
		for (int i = 0; i < rows; i++) {
			for (int j = 0; j < cols; j++) {
				posts[i,j] = Instantiate (post, Camera.main.ScreenToWorldPoint( new Vector3 (startX + j*distX, startY + i * distY, 10)), transform.rotation) as GameObject;
				posts[i,j].transform.SetParent(postsParent);
				posts[i,j].name = "Post"+j+"_"+i;
				posts[i,j].GetComponent<PostScript>().state = PostScript.PostState.Open;
				posts[i,j].GetComponent<PostScript>().point.x = j;
				posts[i,j].GetComponent<PostScript>().point.y = i;
			}
		}

		Unit = Vector3.Distance (posts [0, 0].transform.position, posts [0, 1].transform.position);
	}

	IEnumerator CreateStartLines () {
		yield return new WaitForEndOfFrame ();
		// it creates the initial lines / out borders.
		LinesManager.Instance.CreateOutline (startPosts [0], startPosts [1]);
		LinesManager.Instance.CreateOutline (startPosts [1], startPosts [2]);
		LinesManager.Instance.CreateOutline (startPosts [2], startPosts [3]);
		LinesManager.Instance.CreateOutline (startPosts [3], startPosts [0]);
	}

	// Use this for initialization
	void Start () {

		StartCoroutine (CreateStartLines ());


	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
