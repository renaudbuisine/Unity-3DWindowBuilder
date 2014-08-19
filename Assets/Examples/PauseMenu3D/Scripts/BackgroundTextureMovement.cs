using UnityEngine;
using System.Collections;

public class BackgroundTextureMovement : MonoBehaviour {
	
	public float m_xSpeed = 1;
	public float m_ySpeed = 2;

	private MeshRenderer m_renderer;

	// Use this for initialization
	void Start () {
		this.m_renderer = this.GetComponent<MeshRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
		Vector2 offset = this.m_renderer.material.mainTextureOffset;
		
		offset.x += this.m_xSpeed * Time.deltaTime;
		offset.y += this.m_ySpeed * Time.deltaTime;

		this.m_renderer.material.mainTextureOffset = offset;
	}
}
