using UnityEngine;
using System.Collections;

public class CameraMenuRotation : MonoBehaviour {
	
	public float m_xAmplitude;
	public float m_yAmplitude;

	// on vérifie si l'on survol le texte
	public void Update(){
		Vector3 diff = Input.mousePosition - GetScreenCenter();
		this.transform.localEulerAngles  = new Vector3(diff.y / Screen.height * m_yAmplitude,diff.x / Screen.width * m_xAmplitude,0);
	}

	private Vector3 GetScreenCenter(){
		return new Vector3(Screen.width / 2,Screen.height / 2,0);
	}
}
