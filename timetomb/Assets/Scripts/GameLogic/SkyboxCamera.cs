using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyboxCamera : MonoBehaviour {

    private Skybox m_Skybox = null;
    private float m_SkyboxRot = 0;

	// Use this for initialization
	void Start () {
		
        m_Skybox = GetComponent<Skybox>();
	}
	
	// Update is called once per frame
	void Update () {
		m_SkyboxRot += 2f * Time.deltaTime;
        m_SkyboxRot %= 360f;
        m_Skybox.material.SetFloat("_Rotation", m_SkyboxRot);
	}
}
