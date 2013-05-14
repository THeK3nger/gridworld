using UnityEngine;
using System.Collections;

public class CameraScript : MonoBehaviour {

    public float edgeDelta = 10.0f;
    public float edgeSpeed = 5.0f;
    public int maxZoomOut = 10;

    private Vector3 rightDirection;
    private Vector3 leftDirection;
    private Vector3 upDirection;
    private Vector3 downDirection;

	// Use this for initialization
	void Start () {
        rightDirection = transform.right;
        leftDirection = transform.right * -1;
        upDirection = transform.up;
        downDirection = transform.up * -1;
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.mousePosition.x >= Screen.width - edgeDelta)
        {
            // Move the camera
            transform.position += rightDirection * Time.deltaTime * edgeSpeed;
        }
        if (Input.mousePosition.x < edgeDelta)
        {
            // Move the camera
            transform.position += leftDirection * Time.deltaTime * edgeSpeed;
        }
        if (Input.mousePosition.y >= Screen.height - edgeDelta)
        {
            // Move the camera
            transform.position += upDirection * Time.deltaTime * edgeSpeed;
        }
        if (Input.mousePosition.y < edgeDelta)
        {
            // Move the camera
            transform.position += downDirection * Time.deltaTime * edgeSpeed;
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0) // back
        {
            Camera.main.orthographicSize = Mathf.Min(Camera.main.orthographicSize + 1, maxZoomOut);
        }
        if (Input.GetAxis("Mouse ScrollWheel") > 0) // forward
        {
            Camera.main.orthographicSize = Mathf.Max(Camera.main.orthographicSize - 1, 1);
        }
	}
}
