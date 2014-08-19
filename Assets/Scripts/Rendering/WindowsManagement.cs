using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WindowsManagement : MonoBehaviour {
	
	public float m_windowsFefresh = 1f / 30;

	private Camera m_camera;

	// Use this for initialization
	void Start () {
		this.m_camera = this.GetComponent<Camera>();
		StartCoroutine("SimulateRenderTexture");
	}

	void Update(){
	}

	// coroutine de capture d'écran pour les fenetres
	private IEnumerator SimulateRenderTexture(){
		while(true){
			
			yield return new WaitForEndOfFrame();// on attend la fin de rendu en cours

			foreach(GameObject window in GameObject.FindGameObjectsWithTag("Window3D")){ // on parcourt les fenetres à mettre à jour
				RenderTextureFree render = window.GetComponent<RenderTextureFree>();
				if(render != null){
					CameraRaycastManager.Instance.Update(window.GetComponentInChildren<Camera>(),window.GetComponent<WindowInputsManagement>().MouseRelativePosition); // on met à jour la collision de la souris dans le viewport de la fenetre
					window.BroadcastMessage("ManageInputs",window,SendMessageOptions.DontRequireReceiver);
					render.Capture();
				}
			}
			this.m_camera.Render();// on affiche le rendu de la caméra principal pour effacer tous les autres rendus pour les captures d'image
			
			yield return new WaitForSeconds(m_windowsFefresh);
		}
	}

	// retourne tous les objects avec un layer spécifique
	public GameObject[] FindGameObjectsWithLayer (int layer) {
		GameObject[] goArray = FindObjectsOfType(typeof(GameObject)) as GameObject[];
		List<GameObject> goList = new List<GameObject>();
		for (int i = 0; i < goArray.Length; i++) {
			if (goArray[i].layer == layer) {
				goList.Add(goArray[i]);
			}
		}
		if (goList.Count == 0) {
			return null;
		}
		return goList.ToArray();
	}

}
