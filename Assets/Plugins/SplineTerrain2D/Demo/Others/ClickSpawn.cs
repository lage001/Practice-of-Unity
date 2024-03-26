using UnityEngine;
using System.Collections;

public class ClickSpawn : MonoBehaviour
{

	public GameObject Brick;
	Vector3 offset;

	void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			offset = new Vector3(0,0, -transform.position.z);
			GameObject brick = Instantiate(Brick, Camera.main.ScreenToWorldPoint(Input.mousePosition + offset), Brick.transform.rotation) as GameObject;
			brick.transform.Rotate(0, 0, UnityEngine.Random.Range(0, 0));
			brick.transform.localScale = new Vector3(
				UnityEngine.Random.Range(1f, 3f),
				UnityEngine.Random.Range(1f, 5f), 
				1) * 0.4f;
			brick.GetComponent<Rigidbody2D>().mass = brick.transform.localScale.x * brick.transform.localScale.y * 5f;
			Destroy(brick, 5.0f);
		}
	}
}