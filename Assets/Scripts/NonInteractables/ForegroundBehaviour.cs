using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForegroundBehaviour : MonoBehaviour {

    private SpriteRenderer sprRenderer;

	// Use this for initialization
	void Start () {
        sprRenderer = GetComponent<SpriteRenderer>();
        InvokeRepeating("SpriteFlip", 3, 3);
    }
	
	private void SpriteFlip()
    {
        sprRenderer.flipX = !sprRenderer.flipX;
    }
}
