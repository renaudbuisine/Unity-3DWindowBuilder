using UnityEngine;
using System.Collections;

public enum WindowPositionningType{
	Fixed,// fenetre immobile
	Vertical, // mouvements verticaux autorisés
	Horizontal, // mouvements horizontaux
	Free // mouvements libres
}

public class WindowPositionning : MonoBehaviour {

	public WindowPositionningType m_positionning = WindowPositionningType.Free;
	public Rect m_windowContent;
	public bool m_linkButtons = true;

	private Transform m_display;
	private bool m_isDragging;
	private Vector3 m_offset;
	private Vector3 m_startPosition;
	private Plane m_plane;
	private Vector3 m_buttonsPosition;
	private Transform m_buttons;

	// Use this for initialization
	void Start () {
		this.m_display = this.transform.FindChild("Display");
		this.m_plane = new Plane(Camera.main.transform.forward,m_display.position);
		if(!this.m_linkButtons){
			this.m_buttons = this.transform.FindChild("Buttons");
			this.m_buttonsPosition = this.m_buttons.position;
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(this.m_positionning == WindowPositionningType.Fixed){ // on a interdit le déplacement, on ne fait rien
			return;
		}

		if(this.m_isDragging){
			Vector3 move = GetMousePosInWorld();
			switch((int)this.m_positionning){
			case (int)WindowPositionningType.Horizontal:
				move.y = this.m_startPosition.y;
				break;
			case (int)WindowPositionningType.Vertical:
				move.x = this.m_startPosition.x;
				move.z = this.m_startPosition.z;
				break;
			}
			transform.position = move + m_offset;

			if(!this.m_linkButtons){
				this.m_buttons.position = this.m_buttonsPosition;
			}
		}

		// on modifie le statu de la fenetre (en déplacement ou non)
		if(Input.GetMouseButtonDown(0)){
			if (CameraRaycastManager.Instance.CollisionExists(Camera.main)){
				RaycastHit hit = CameraRaycastManager.Instance.GetHit(Camera.main);
				if(hit.transform.gameObject.GetInstanceID() == this.m_display.gameObject.GetInstanceID()){
					Vector3 relativePosition = this.m_display.InverseTransformPoint(hit.point); // position relative par au rapport repère du plan
					relativePosition.x = (float)(relativePosition.x + 5.0f) / 10.0f;
					relativePosition.z = (float)(relativePosition.z + 5.0f) / 10.0f; // position relative au plan normalisé
					if(relativePosition.x < this.m_windowContent.x || relativePosition.y < this.m_windowContent.y ||
					   relativePosition.x > this.m_windowContent.x + this.m_windowContent.width || relativePosition.y > this.m_windowContent.y + this.m_windowContent.height){
						this.m_isDragging = true; // on clique sur la fenetre, on commence le déplacement
						this.m_startPosition = GetMousePosInWorld();
						m_offset = this.transform.position - this.m_startPosition;// on enregistre la position de la souris pour établir par la suite une valeur de déplacement
					}
				}
			}
		}
		if(Input.GetMouseButtonUp(0)){ // on relache la fenetre, fin du déplacement
			this.m_isDragging = false;
		}
	}

	private Vector3 GetMousePosInWorld()
	{
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		float distance;
		if (m_plane.Raycast(ray, out distance))
		{
			return ray.GetPoint(distance);
		}
		return Vector3.zero;
	}
	
	#region GETTER-SETTER
	public WindowPositionningType Positionning{
		get{return this.m_positionning;}
		set{this.m_positionning = value;}
	}
	public bool IsDragging{
		get{return this.m_isDragging;}
		set{this.m_isDragging = value;}
	}
	#endregion
}
