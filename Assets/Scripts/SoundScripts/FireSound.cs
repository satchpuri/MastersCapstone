﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireSound : MonoBehaviour {

	// Use this for initialization
	void Start () {
        AkSoundEngine.PostEvent("Fire_Stationary_Start", gameObject);
        StartCoroutine(KillSound());
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnDestroy()
    {
        AkSoundEngine.PostEvent("Fire_Stationary_Stop", gameObject);
    }

    IEnumerator KillSound()
    {
        yield return new WaitForSeconds(14f);
        Destroy(gameObject);
    }
}
