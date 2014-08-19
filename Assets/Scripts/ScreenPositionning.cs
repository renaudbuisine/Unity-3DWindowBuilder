using UnityEngine;
using System.Collections;

public class ScreenPositionning : MonoBehaviour {
	public bool m_initPosition = true;
	public float m_screenToWorldDepth; // profondeur de la fenetre
	public Vector2 m_screenToWorld; // position relative sur l'écran 
	public bool m_relative = true;
	public bool m_parentPriority; // si c'est un parent, on exécute en premier

	void Awake() {
		if(this.m_parentPriority && this.m_initPosition){
			this.PlaceOnScreen();
		}         
	}

	// Use this for initialization
	void Start () {
		if(!this.m_parentPriority && this.m_initPosition){
			this.PlaceOnScreen();
		}
	}

	#region SCREEN_POSITION
	public void PlaceOnScreen(){
		Vector3 screenPos = this.m_relative 
			? new Vector3(this.m_screenToWorld.x * Screen.width,
			              this.m_screenToWorld.y * Screen.height,
			              this.m_screenToWorldDepth)
				: new Vector3(this.m_screenToWorld.x,
				              this.m_screenToWorld.y,
				              this.m_screenToWorldDepth);

		this.transform.position = (Camera.main.ScreenToWorldPoint(screenPos));
	}
	#endregion

}
