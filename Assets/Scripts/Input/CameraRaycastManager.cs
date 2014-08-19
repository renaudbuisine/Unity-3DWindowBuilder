using UnityEngine;
using System.Collections;

public class CameraRaycastManager : MonoBehaviour {

	private class HitState{
		public RaycastHit m_hit;
		public bool m_collision;
		public LayerMask m_layerMask;
		public bool m_waitInput; // indique si l'on met à jour la collision automatiquement ou non

		public HitState(int layer, bool wait){
			this.m_collision = false;
			this.m_layerMask = layer;
			this.m_waitInput = wait;
		}
	}
	
	private static CameraRaycastManager g_inst = null;

	private Hashtable m_cameras; // dictionnaire des cameras à écouter pour le raycast


	void Start() {
		this.m_cameras = new Hashtable();
		CameraRaycastManager.g_inst = this; // on crée l'instance unique

		CameraRaycastManager.g_inst.Listen(Camera.main); // on ajoute par défaut la camera principale
	}

	#region UPDATE
	// Update is called once per frame
	void Update () {
		foreach (DictionaryEntry cam in this.m_cameras)
		{
			HitState state = (HitState)cam.Value;
			Camera camera = (Camera)cam.Key;
			if(!state.m_waitInput){ // si l'on attend pas de demande de mise à jour, on met à jour
				this.UpdateHitState(camera,state,Input.mousePosition);
			}
		}
	}

	// force la mise à jour d'une camera
	public void Update(Camera camera ,Vector3 position){
		this.UpdateHitState(camera,GetHitState(camera),position);
		//if(GetHitState(camera).m_collision)Debug.Log(GetHitState(camera).m_hit.transform.name);
	}
	#endregion

	#region COLLISION
	// met à jour la collision dans la structure liée à la caméra
	private void UpdateHitState(Camera cam, HitState state,Vector3 position){
		Ray ray = cam.ScreenPointToRay (position);
		state.m_collision = Physics.Raycast(ray, out state.m_hit,cam.farClipPlane, state.m_layerMask.value);
	}
	#endregion

	#region ADD_REMOVE_CAMERA
	// ajoute l'écoute d'une caméra
	public void Listen(Camera camera, int layer,bool wait){
		this.m_cameras.Add(camera,new HitState(layer,wait));
	}
	public void Listen(Camera camera){
		this.Listen(camera,-1,false);
	}
	// on arrete l'écoute
	public void Stop(Camera camera){
		this.m_cameras.Remove(camera);
	}
	#endregion

	#region GETTER-SETTER	
	public static CameraRaycastManager Instance{
		get{return CameraRaycastManager.g_inst;}
	}
	private HitState GetHitState(Camera camera){
		return (HitState)(this.m_cameras[camera]);
	}
	public bool CollisionExists(Camera camera){
		return GetHitState(camera).m_collision;
	}
	public bool IsWaiting(Camera camera){
		return GetHitState(camera).m_waitInput;
	}
	public RaycastHit GetHit(Camera camera){
		return GetHitState(camera).m_hit;
	}
	#endregion
}
