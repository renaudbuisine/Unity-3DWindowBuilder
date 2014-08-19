using UnityEngine;
using System.Collections;

public enum WindowButtonType{
	Restore,
	Reduce,
	Close,
	Count
}

public class WindowButtonsManagement : MonoBehaviour {

	// Use this for initialization
	void Start () {
		this.HideButton(WindowButtonType.Restore);
	}
	
	// Update is called once per frame
	void Update () {
		this.MouseManagement();
	}

	#region MOUSE_MANAGEMENT
	private void MouseManagement(){
		if ( Input.GetMouseButtonDown(0)){
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			if (Physics.Raycast(ray, out hit,Camera.main.farClipPlane,1 << LayerMask.NameToLayer("Window"))){
				if(hit.transform.tag == "WindowTrigger" && hit.transform.parent.GetInstanceID() == this.transform.GetInstanceID()){
					switch(hit.transform.gameObject.name){
					case "RestoreObject":
						this.Window.GetComponent<WindowBehaviour>().RestoreWindow();
						break;
					case "ReduceObject":
						this.Window.GetComponent<WindowBehaviour>().ReduceWindow();
						break;
					case "CloseObject":
						this.Window.GetComponent<WindowBehaviour>().CloseWindow();
						break;
					}
				}
			}
		}
	}
	#endregion

	#region SHOW-HIDE
	public void ShowButton(WindowButtonType button){
		this.SetButtonActive(button,true);
	}
	public void HideButton(WindowButtonType button){
		this.SetButtonActive(button,false);
	}
	private void SetButtonActive(WindowButtonType button, bool state){
		string buttonName = "";
		switch(button){
		case WindowButtonType.Restore :
			buttonName = "RestoreObject";
			break;
		case WindowButtonType.Reduce :
			buttonName = "ReduceObject";
			break;
		case WindowButtonType.Close :
			buttonName = "CloseObject";
			break;
		}
		Transform restore = this.transform.FindChild(buttonName);
		if(restore != null){// si le bouton existe, on le cache
			restore.gameObject.SetActive(state);
		}
	}
	#endregion

	#region WINDOW
	private GameObject Window{
		get{return this.transform.parent.parent.gameObject;}
	}
	#endregion
}
