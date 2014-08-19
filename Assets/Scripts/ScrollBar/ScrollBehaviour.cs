using UnityEngine;
using System.Collections;

public enum WindowScrollBarDirection{
	Vertical,
	Horizontal
}

public class ScrollBehaviour : MonoBehaviour {

	public WindowScrollBarDirection m_direction = WindowScrollBarDirection.Vertical;
	public float m_startOffset, m_endOffset;
	public string m_camera;

	public float m_value = 0;

	public int m_steps = 0;

	private WindowInputsManagement m_inputManagement;
	private GameObject m_scrollBarContent;
	private GameObject m_scrollBarButton;

	private bool m_isDragging;
	private Vector3 m_startPosition;
	private Vector3 m_offset;
	private Plane m_plane;

	// Use this for initialization
	void Start () {

		GameObject camera = GameObject.Find(m_camera);
		this.m_inputManagement = camera.GetComponent<WindowInputsManagement>();
		
		this.m_scrollBarContent = this.transform.FindChild("ScrollContent").gameObject;
		this.m_scrollBarButton = this.transform.FindChild("ScrollButton").gameObject;

		this.m_plane = new Plane(camera.transform.forward,this.m_scrollBarContent.transform.position);

		this.SetButtonPosition();
	}

	#region DRAGGING
	// Update is called once per frame
	void Update () {
		/*if(this.m_isDragging & this.m_inputManagement.IsMouseHoverWindow()){
			Vector3 move = this.m_inputManagement.ScreenPointToRay(this.m_plane);
			this.SetButtonPosition(move + m_offset);
		}
		
		// On vérifie si l'on clic sur le bouton de la scrollbar
		RaycastHit hit;
		if(Input.GetMouseButtonDown(0)){
			if(this.m_inputManagement.WindowPhysicsRaycast(this.m_inputManagement.MouseRelativePosition,out hit)){
				if(hit.transform.tag == "WindowScrollBarButton" && hit.transform.gameObject.GetInstanceID() == this.m_scrollBarButton.GetInstanceID()){
					this.m_isDragging = true; // on clique sur la fenetre, on commence le déplacement
					this.m_startPosition = this.m_scrollBarButton.transform.localPosition;
					this.m_offset = this.m_scrollBarButton.transform.position - this.m_inputManagement.ScreenPointToRay(this.m_plane);// on enregistre la position de la souris pour établir par la suite une valeur de déplacement
				}
			}
		}

		if(Input.GetMouseButtonUp(0)){ // on relache la fenetre, fin du déplacement
			this.m_isDragging = false;
			if(this.m_steps > 0){ // si la barre fonctionne par cran, on repositionne la barre correctement
				this.SetButtonPosition();
			}
		}*/
	}

	// positionne le bouton en prenant en compte les limites
	private void SetButtonPosition(Vector3 position){
		this.m_scrollBarButton.transform.position = position;

		Vector3 local = this.m_scrollBarButton.transform.localPosition;
		switch((int)this.m_direction){
		case (int)WindowScrollBarDirection.Horizontal:
			local.z = this.m_startPosition.z;
			local.x = ConfineBorder(local.x,this.m_scrollBarContent.transform.localScale.x);
			this.m_value = CalculateValue(local.x,this.m_scrollBarContent.transform.localScale.x);
			break;
		case (int)WindowScrollBarDirection.Vertical:
			local.x = this.m_startPosition.x;
			local.z = ConfineBorder(local.z,this.m_scrollBarContent.transform.localScale.z);
			this.m_value = CalculateValue(local.z,this.m_scrollBarContent.transform.localScale.z);
			break;
		}
		local.y = this.m_startPosition.y;
		this.m_scrollBarButton.transform.localPosition = local;
	}

	// positionne la barre selon l'attribut value
	private void SetButtonPosition(){
		Vector3 local = this.m_scrollBarButton.transform.localPosition;
		switch((int)this.m_direction){
		case (int)WindowScrollBarDirection.Horizontal:
			local.x = ((10 * this.m_scrollBarContent.transform.localScale.x - this.m_endOffset - this.m_startOffset) * this.m_value - 5 * this.m_scrollBarContent.transform.localScale.x + this.m_endOffset);
			break;
		case (int)WindowScrollBarDirection.Vertical:
			local.z = ((10 * this.m_scrollBarContent.transform.localScale.z - this.m_endOffset - this.m_startOffset) * this.m_value - 5 * this.m_scrollBarContent.transform.localScale.z + this.m_endOffset);
			break;
		}

		this.m_scrollBarButton.transform.localPosition = local;
	}

	// retourne la valeur par rapport à la position de la barre
	private float CalculateValue(float posVal, float refVal){
		refVal *= 5f;
		float value = (posVal + refVal - this.m_endOffset) / ((2 * refVal - this.m_endOffset - this.m_startOffset));
		// si la scrollbar se déplace par étape, on adapte la valeur
		return ManageSteps(value);
	}

	// Limit une valeur d'entrée selon les offsets fournis pour le bouton de la scrollbar
	private float ConfineBorder(float val, float scale){
		if(val > 5f*scale - this.m_startOffset){
			return 5f*scale - this.m_startOffset;
		}
		if(val < -5f*scale + this.m_endOffset){
			return -5f*scale + this.m_endOffset;
		}
		return val;
	}
	#endregion

	#region STEPS
	// gère le cas où la scrollbar à des étapes / des crans
	private float ManageSteps(float value){
		if(this.m_steps > 0){
			float step = 1/ (float)this.m_steps;
			return Mathf.Round(value / step) * step; 
		}
		return value;
	}
	#endregion

	#region GETTER-SETTER
	public float Value{
		get{
			return this.m_value;
		}
		set{
			this.m_value = value;
			if(this.m_value > 1){
				this.m_value = 1;
			}
			if(this.m_value < 0){
				this.m_value = 0;
			}
			// si la scrollbar se déplace par étape, on adapte la valeur
			this.m_value = ManageSteps(this.m_value);
			// positionner la barre de force
			this.SetButtonPosition();
		}
	}

	public bool IsDragging{
		get{return this.m_isDragging;}
	}

	public WindowScrollBarDirection Direction{
		get{return this.m_direction;}
	}
	public int Steps{
		get{return this.m_steps;}
		set{
			this.m_steps = value;
			this.Value = this.m_value; // on force le recalcul de la valeur et le repositionnement de la barre
		}
	}
	#endregion
}
