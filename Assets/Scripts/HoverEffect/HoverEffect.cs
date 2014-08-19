using UnityEngine;
using System.Collections;

public class HoverEffect : MonoBehaviour {

	public Material m_hoverMaterial;

	private Material m_outMaterial;
	private MeshRenderer m_renderer;

	// Use this for initialization
	void Start () {
		this.m_renderer = this.GetComponent<MeshRenderer>();

		this.m_outMaterial = this.m_renderer.material;
	}
	
	// Update is called once per frame
	void Update () {
		RaycastHit hit;
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		if (Physics.Raycast(ray, out hit,Camera.main.farClipPlane)){
			if(hit.transform.gameObject.GetInstanceID() == this.gameObject.GetInstanceID()){ // la souris est au dessus du bouton
				this.m_renderer.material = this.m_hoverMaterial;
				return;
			}
		}

		this.m_renderer.material = this.m_outMaterial;
	}
}
