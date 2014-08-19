using UnityEngine;
using System.Collections;

public enum WindowAnimation{
	Top,
	Bottom,
	Right,
	Left
}


/// <summary>
/// W////////////////////////////////////:
/// @todo penser à insérer le champs active de rendertexturefree !!!!
/// </summary>


public class WindowBehaviour : MonoBehaviour {

	private class WindowAnimationState{
		public Vector3 m_initPositionB;
		public Vector3 m_initPositionBu;
		public Vector3 m_translation;
		public float m_timeLeft;
		public WindowAnimation m_animation;

		public WindowAnimationState(WindowAnimation animation,float time, Transform border, Transform buttons, Vector3 translation){
			this.m_animation = animation;
			this.m_timeLeft = time;
			this.m_initPositionB = border.localPosition;
			this.m_initPositionBu = buttons != null ? buttons.localPosition : Vector3.zero;
			this.m_translation = translation;
		}
	}

	private class WindowMaterialsParams{
		public Vector2 m_displayScale,m_displayOffset,m_maksScale,m_maskOffset,m_borderScale,m_borderOffset;

		public WindowMaterialsParams(){}
	}

	public WindowAnimation m_animation = WindowAnimation.Left;
	public bool m_animationAllowed = true;
	public float m_animationDuration = 0.5f;
	public float m_animationDistance = 10; // distance parcourue lors de l'animation
	public bool m_buttonsFollowed = true;

	private bool m_reduced = false;

	private WindowAnimationState m_state = null;

	private GameObject m_display;
	private Transform m_buttons;
	
	private Vector3 m_displayScale;

	private WindowMaterialsParams m_initMaterials;

	private RenderTextureFree m_renderer;

	// Use this for initialization
	void Start () {
		if(m_animation <= 0){
			this.m_animationAllowed = false;
		}

		Transform inter = this.transform.FindChild("Interface");
		this.m_display = inter.transform.FindChild("Display").gameObject;
		this.m_buttons = inter.FindChild("Buttons");
		
		this.m_displayScale = this.m_display.transform.localScale;

		this.m_initMaterials = new WindowMaterialsParams();
		MeshRenderer renderer = this.m_display.GetComponent<MeshRenderer>();
		this.m_initMaterials.m_borderScale = renderer.material.GetTextureScale("_Border");
		this.m_initMaterials.m_borderOffset = renderer.material.GetTextureOffset("_Border");
		this.m_initMaterials.m_displayScale = renderer.material.mainTextureScale;
		this.m_initMaterials.m_displayOffset = renderer.material.mainTextureOffset;
		this.m_initMaterials.m_maksScale = renderer.material.GetTextureScale("_Mask");
		this.m_initMaterials.m_maskOffset = renderer.material.GetTextureOffset("_Mask");

		this.m_renderer = this.GetComponent<RenderTextureFree>();
	}
	
	// Update is called once per frame
	void Update () {
		if(this.m_state != null){ // il y a une animation à gérer
			this.m_state.m_timeLeft -= Time.deltaTime;
			if(this.m_state.m_timeLeft < 0){
				this.m_state.m_timeLeft = 0;
			}

			float progress = (this.m_animationDuration - this.m_state.m_timeLeft) / this.m_animationDuration;
			
			this.UpdateAnimation(progress);

			if(this.m_state.m_timeLeft == 0){ // fin de l'animation
				if(this.m_reduced){ // on envoit les messages de fin d'action
					this.BroadcastMessage("OnPostReducing",this,SendMessageOptions.DontRequireReceiver);
					this.m_renderer.Active = false;
				}
				else{	
					this.BroadcastMessage("OnPostRestoring",this,SendMessageOptions.DontRequireReceiver);
					this.m_renderer.Active = true;
				}
				this.m_state = null;
			}
		}
	}

	#region ANIMATION

	// lance une animation
	private bool LaunchAnimation(bool reduced){
		if(this.m_state != null || this.m_reduced == reduced){ // si l'animation n'est pas autorisée ou si une animation est en cours ou si la fenetre est deja dans l'etat final attendu
			return false;
		}

		if(!this.m_animationAllowed){
			if(!reduced){
				this.ForceRestore();
			}
			else{
				this.ForceReduce();
			}
			return false;
		}

		this.m_reduced = reduced;

		this.SetButtons(this.m_reduced);// affichage des boutons

		this.m_state = new WindowAnimationState(this.m_animation,this.m_animationDuration,this.m_display.transform,this.m_buttons,this.CalculateFullTranslation(this.m_animation,reduced)); // on génère le structure de l'animation
		return true;
	}

	private void SetButtons(bool reduced){
		if(m_buttons != null){
			// on change l'affichage des boutons
			Transform button = this.m_buttons.FindChild("RestoreObject");
			if(button != null){ // on vérifie que le bouton existe
				button.gameObject.SetActive(reduced);
			}
			button = this.m_buttons.FindChild("ReduceObject");
			if(button != null){
				button.gameObject.SetActive(!reduced);
			}
		}
	}

	// indique la translation des éléments suite à l'animation
	private Vector3 CalculateFullTranslation(WindowAnimation animation, bool reduced){
		Vector3 finalPosition = Vector3.zero;
		switch((int)animation){
		case (int)WindowAnimation.Top :
			finalPosition.z = this.m_animationDistance;
			break;
		case (int)WindowAnimation.Bottom:
			finalPosition.z = -this.m_animationDistance;
			break;
		case (int)WindowAnimation.Right:
			finalPosition.x = this.m_animationDistance;
			break;
		case (int)WindowAnimation.Left:
			finalPosition.x = -this.m_animationDistance;
			break;
		}
		return reduced ? finalPosition : -finalPosition;
	}

	// met à jour la positions et les proportions des éléments pour l'animation
	private void UpdateAnimation(float progress){
		Vector3 scaleEffect = Vector3.one; // on calcul le scale de réduction
		float value = this.m_reduced ? 1 - progress : progress;
		switch((int)this.m_state.m_animation){
		case (int)WindowAnimation.Top :
			scaleEffect.z = -value;
			break;
		case (int)WindowAnimation.Bottom:
			scaleEffect.z = value;
			break;
		case (int)WindowAnimation.Right:
			scaleEffect.x = -value;
			break;
		case (int)WindowAnimation.Left:
			scaleEffect.x = value;
			break;
		}

		this.m_display.transform.localPosition = this.m_state.m_initPositionB + (new Vector3(this.m_displayScale.x * this.m_state.m_translation.x, this.m_displayScale.y * this.m_state.m_translation.y,this.m_displayScale.z * this.m_state.m_translation.z) * progress) / 2; // on met à jour la position des éléments (effet de décalage)
		if(m_buttonsFollowed && this.m_buttons != null){
			this.m_buttons.localPosition = this.m_state.m_initPositionBu + 2 * (this.m_display.transform.localPosition - this.m_state.m_initPositionB);
		}
		
		// on applique le nouveau scale
		this.m_display.transform.localScale = new Vector3(this.m_displayScale.x * scaleEffect.x,
		                                                  this.m_displayScale.y * scaleEffect.y,
		                                                  this.m_displayScale.z * scaleEffect.z);

		// on déplace les textures pour avoir une impression de réduction globale vers une direction
		MeshRenderer renderer = this.m_display.GetComponent<MeshRenderer>(); // on gère les proportions des textures
		renderer = this.m_display.GetComponent<MeshRenderer>(); // on gère les proportions des textures
		if(renderer != null){
			renderer.material.mainTextureScale = new Vector2(scaleEffect.x * this.m_initMaterials.m_displayScale.x,scaleEffect.z * this.m_initMaterials.m_displayScale.y);
			renderer.material.mainTextureOffset = new Vector2(scaleEffect.x * this.m_initMaterials.m_displayOffset.x,scaleEffect.z * this.m_initMaterials.m_displayOffset.y);
			renderer.material.SetTextureScale("_Mask",new Vector2(scaleEffect.x * this.m_initMaterials.m_maksScale.x,scaleEffect.z * this.m_initMaterials.m_maksScale.y));
			renderer.material.SetTextureOffset("_Mask",new Vector2(scaleEffect.x * this.m_initMaterials.m_maskOffset.x,scaleEffect.z * this.m_initMaterials.m_maskOffset.y));
			renderer.material.SetTextureScale("_Border",new Vector2(scaleEffect.x * this.m_initMaterials.m_borderScale.x,scaleEffect.z * this.m_initMaterials.m_borderScale.y));
			renderer.material.SetTextureOffset("_Border",new Vector2(scaleEffect.x * this.m_initMaterials.m_borderOffset.x,scaleEffect.z * this.m_initMaterials.m_borderOffset.y));
		}
	}

	// force la fin de l'animation de restauration de fenetre
	public void ForceRestore(){
		/*if(this.m_reduced){// on restore la fenetre on lève un event
			this.BroadcastMessage("OnRestoring",this,SendMessageOptions.DontRequireReceiver);
		}*/

		this.m_reduced = false;
		
		this.SetButtons(this.m_reduced);// affichage des boutons
		
		this.m_state = new WindowAnimationState(this.m_animation,this.m_animationDuration,this.m_display.transform,this.m_buttons,this.CalculateFullTranslation(this.m_animation,this.m_reduced)); // on génère le structure de l'animation

		this.UpdateAnimation(1);

		this.m_state = null;
	}
	// force la fin de l'animation de réduction de fenetre
	public void ForceReduce(){
		/*if(!this.m_reduced){// on restore la fenetre on lève un event
			this.BroadcastMessage("OnReducing",this,SendMessageOptions.DontRequireReceiver);
		}*/
		this.m_reduced = true;
		
		this.SetButtons(this.m_reduced);// affichage des boutons
		
		this.m_state = new WindowAnimationState(this.m_animation,this.m_animationDuration,this.m_display.transform,this.m_buttons,this.CalculateFullTranslation(this.m_animation,this.m_reduced)); // on génère le structure de l'animation
		
		this.UpdateAnimation(1);

		this.m_state = null;
	}
	#endregion

	#region WINDOW_EVENTS
	public void CloseWindow(){
		this.m_reduced = false;
		this.ForceRestore();
		this.transform.gameObject.SetActive(false); // on ferme la fenetre
		this.BroadcastEvent("OnClosing");
	}

	public void RestoreWindow(){
		if(LaunchAnimation(false)){
			this.BroadcastEvent("OnRestoring");
			this.m_renderer.Active = true;
		}
	}

	public void ReduceWindow(){
		if(LaunchAnimation(true))
			this.BroadcastEvent("OnReducing");
	}
	// envoie des messages à tous les objets (event onclose, onreduce,...)
	private void BroadcastEvent(string eventFct){
		GameObject[] gameObjects = (GameObject[])GameObject.FindObjectsOfType(typeof(GameObject));
		foreach (GameObject obj in gameObjects) {
			if (obj.transform.parent == null) {
				obj.gameObject.BroadcastMessage(eventFct, this, SendMessageOptions.DontRequireReceiver);
			}
		}
	}
	#endregion

	#region GETTER-SETTER
	public bool Reduced{
		get{return this.m_reduced;}
	}
	public bool AnimationAllowed{
		get{return this.m_animationAllowed;}
		set{this.m_animationAllowed = value;}
	}
	public WindowAnimation AnimationType{
		get{return this.m_animation;}
		set{this.m_animation = value;}
	}
	public bool IsAnimated{
		get{return this.m_state != null;}
	}
	#endregion
}
