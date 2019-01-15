using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FullScreenSprite : MonoBehaviour {

	void Start ()
	{
		ScaleToMatchScreen ();
	}

	void ScaleToMatchScreen ()
	{
		/*
		 * jika tidak ada sprite, maka tidak ada yang bisa di scale
		 */
		SpriteRenderer renderer = GetComponent<SpriteRenderer> ();
		if (renderer == null)
			return;


		Vector2 spriteSize = renderer.sprite.bounds.size;
		float cameraHeight = Camera.main.orthographicSize * 2;
		Vector2 cameraSize = new Vector2 (Camera.main.aspect * cameraHeight, cameraHeight);

		/*
		 * reset sprite scale 
		 */
		transform.localScale = new Vector3 (1f, 1f, 1f);
		Vector2 scale = new Vector2 (cameraSize.x / spriteSize.x, cameraSize.y / spriteSize.y);

		/*
		 * gunakana scale factor yang paling besar
		 */
		if (scale.x > scale.y) {
			transform.localScale *= scale.x;
		} else {
			transform.localScale *= scale.y;
		}

	}
}
