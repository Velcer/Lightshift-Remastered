using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectFadeOut : MonoBehaviour {

	float lifetime;
	public float maxLifetime;
	[Header("change scales (leave at 0 for no scaling)")]
	public Vector3 maxSize;

	void Start ()
	{
		lifetime = maxLifetime;
	}

	void Update () 
	{
		if (maxSize != Vector3.zero)
		{
			float tempX = transform.localScale.x;
			float tempY = transform.localScale.y;
			float tempZ = transform.localScale.z;

			if (maxSize.x != 0)
				tempX = lifetime / maxLifetime * maxSize.x;
			if (maxSize.y != 0)
				tempY = lifetime / maxLifetime * maxSize.y;
			if (maxSize.z != 0)
				tempZ = lifetime / maxLifetime * maxSize.z;
			transform.localScale = new Vector3 (tempX, tempY, tempZ);
				
		}
//timer for death
		lifetime -= Time.deltaTime;
		if (lifetime < 0)
			Destroy (gameObject);
	}
}
