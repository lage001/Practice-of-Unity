using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movement : MonoBehaviour {

	public float speed = 3f;
	
	// Update is called once per frame
	void Update ()
	{

		gameObject.transform.position = gameObject.transform.position + new Vector3(speed, 0, 0) * Time.deltaTime;
	}
}
