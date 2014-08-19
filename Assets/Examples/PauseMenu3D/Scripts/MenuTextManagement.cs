using UnityEngine;
using System.Collections;

public class MenuTextManagement : MonoBehaviour, IWindowedInputListener {

	public Color m_selectionColor;
	public int m_selectionFontSize;
	public string m_cameraName;

	private TextMesh m_textMesh;
	private Color m_baseColor;
	private int m_baseFontSize;
	private Camera m_camera;

	void Start(){
		this.m_textMesh = this.GetComponent<TextMesh>();
		this.m_baseColor = this.m_textMesh.color;
		this.m_baseFontSize = this.m_textMesh.fontSize;
		this.m_camera = GameObject.Find(m_cameraName).GetComponentInChildren<Camera>();
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetMouseButtonUp(0)){
			// on vérifie si le curseur de la souris est sur le texte
			if(CameraRaycastManager.Instance.CollisionExists(this.m_camera)){
				RaycastHit hit = CameraRaycastManager.Instance.GetHit(this.m_camera);
				if(hit.transform.gameObject.transform.GetInstanceID() == this.transform.GetInstanceID()){
					switch(hit.transform.tag){
					case "QuitElt":
						// on quitte l'application
						Application.Quit();
						break;
					case "ResumeElt":
						// on ferme le menu
						GameObject.Find(m_cameraName).SetActive(false);
						break;
					}
				}
			}
		}
	}

	// on vérifie si l'on survol le texte
	public void ManageInputs(GameObject window){
		if(this.IsHover()){
			this.m_textMesh.color = this.m_selectionColor;
			this.m_textMesh.fontSize = this.m_selectionFontSize;
		}
		else{
			this.m_textMesh.color = this.m_baseColor;
			this.m_textMesh.fontSize = this.m_baseFontSize;
		}
	}

	private bool IsHover(){
		// on vérifie si le curseur de la souris est sur le texte
		if(CameraRaycastManager.Instance.CollisionExists(this.m_camera)){
			RaycastHit hit = CameraRaycastManager.Instance.GetHit(this.m_camera);
			if(hit.transform.gameObject.transform.GetInstanceID() == this.transform.GetInstanceID()){
				return true;
			}
		}
		return false;
	}
}
