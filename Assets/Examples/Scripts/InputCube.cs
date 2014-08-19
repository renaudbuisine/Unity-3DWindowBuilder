using UnityEngine;
using System.Collections;

public class InputCube : MonoBehaviour, IWindowedInputListener, IWindowListener {
	public void ManageInputs (GameObject management)
	{
	}

	public void OnClosing(Object window){
		Debug.Log("OnClosing");
	}
	public void OnRestoring(Object window){
		Debug.Log("OnRestoring");
	}
	public void OnReducing(Object window){
		Debug.Log("OnReducing");
	}
	public void OnPostReducing(Object window){
		Debug.Log("OnPostReducing");}
	public void OnPostRestoring(Object window){
		Debug.Log("OnPostRestoring");}
	
	public void Update(){
		/*WindowInputsManagement management = GameObject.Find("PauseMenu3D").GetComponent<WindowInputsManagement>();
		if(Input.GetMouseButtonDown(0)){
			RaycastHit hit;
			
			if(management.WindowPhysicsRaycast(management.MouseRelativePosition,out hit)){
				if(hit.transform.gameObject.name == "Cube_w"){
					if(this.GetComponent<MeshRenderer>().material.color == Color.red)
						this.GetComponent<MeshRenderer>().material.color = Color.blue;
					else
						this.GetComponent<MeshRenderer>().material.color = Color.red;
				}
			}
		}*/
	}
}
