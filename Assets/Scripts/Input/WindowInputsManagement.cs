using UnityEngine;
using System.Collections;

public class WindowInputsManagement : MonoBehaviour{

	private bool m_focus = true;
	private Transform m_display;
	private Camera m_camera;
	private RenderTextureFree m_render;

	public void Start(){
		this.m_display = this.transform.FindChild("Interface").FindChild("Display").transform;
		this.m_camera = this.GetComponentInChildren<Camera>();
		this.m_render = this.GetComponent<RenderTextureFree>();

		CameraRaycastManager.Instance.Listen(this.m_camera,1 << LayerMask.NameToLayer("Windowed"),true); // on écoute la nouvelle caméra
	}

	public void Update(){

	}

	#region INPUTS
	public bool IsMouseHoverWindow(){
		if(CameraRaycastManager.Instance.CollisionExists(Camera.main)){
			RaycastHit hit = CameraRaycastManager.Instance.GetHit(Camera.main);
			return hit.transform.gameObject.GetInstanceID() == m_display.gameObject.GetInstanceID();
		}
		return false;
	}

	// retourne la position relative du curseur de la souris dur le plan de la fenetre
	public Vector2 MouseRelativePosition{
		get{
			if(CameraRaycastManager.Instance.CollisionExists(Camera.main)){
				RaycastHit hit = CameraRaycastManager.Instance.GetHit(Camera.main);
				if(hit.transform.gameObject.GetInstanceID() == this.m_display.gameObject.GetInstanceID()){
					Vector3 relativePosition = this.m_display.InverseTransformPoint(hit.point); // position relative par au rapport repère du plan
					relativePosition.x = (float)(relativePosition.x + 5.0f) / 10.0f;
					relativePosition.z = (float)(relativePosition.z + 5.0f) / 10.0f; // position relative au plan normalisé

					// on cherche la position dans le rectangle mis à jour de la fenetre (le viewport)
					Rect textureRect = m_render.NormalizedTextureRect;
					relativePosition.x -= textureRect.x;
					relativePosition.z -= textureRect.y;

					if(relativePosition.x >= 0 && relativePosition.z >= 0 && relativePosition.x < textureRect.width && relativePosition.z < textureRect.height){
						// position dans la fenetre
						return new Vector2(
							relativePosition.x / textureRect.width * this.m_camera.rect.width * Screen.width,
							relativePosition.z / textureRect.height * this.m_camera.rect.height  * Screen.height);
					}
				}
			}
			return new Vector2(-1,-1);
		}
	}

	// indique la position de la souris dans un plan
	public Vector3 ScreenPointToRay(Plane plane)
	{
		Ray ray = this.m_camera.ScreenPointToRay(this.MouseRelativePosition);
		float distance;
		if (plane.Raycast(ray, out distance))
		{
			return ray.GetPoint(distance);
		}
		return Vector3.zero;
	}
	#endregion

	#region GETTER-SETTER
	public bool Focus{
		get{return this.m_focus;}
		set{this.m_focus = value;}
	}
	#endregion

	#region STATIC_CAMERA_FUNCTIONS
	private static Rect GetCameraViewport(Camera camera){
		return new Rect(
			camera.rect.x * Screen.width,
			camera.rect.y * Screen.height,
			camera.rect.width * Screen.width,
			camera.rect.height * Screen.height
			);
	}
	#endregion
}
