using UnityEngine;
using System.Collections;

#region WINDOWRESIZEMODE_ENUM
public enum WindowResizeMode{
	RespectProportion, // conserve les proportions de la viewport et redimentionne le plan de la fenetre
	KeepViewport, // conserve les proportions de la viewport et celles de la fenetre, la texture contient des parties noires
	KeepPlaneScale, // adapte la forme de la fenetre à celle du plan, redimentionnement de la viewport
	ProvideRect // affiche la fenetre dans le rectangle fournit en paramètre (les valeurs du rectangles sont des %)
}
#endregion

public class RenderTextureFree : MonoBehaviour 
{
	#region PUBLIC_ATTRIBUTES
	public WindowResizeMode m_scaleMode = WindowResizeMode.KeepViewport; // indique si la enetre doit prendre les dimensions de la vue
	public bool m_active = true; // indique si la fenetre est ouverte et nécessite donc une mise à jour
	public Color m_backgroundColor = Color.black;
	public Rect m_windowRectangle = new Rect(0,0,1,1);
	#endregion
	#region PRIVATE_ATTRIBUTES
	private Texture2D m_snapShot;
	private Rect m_textureRect;
	private Camera m_camera;
	#endregion

	#region START
	void Start (){
		this.m_camera = this.GetComponentInChildren<Camera>();
		this.m_camera.enabled = false;

		Transform windowInterface = this.transform.FindChild("Interface");
		Transform plane = windowInterface.FindChild("Display");

		if(plane != null){ // affichage sur un plan

			switch((int)this.m_scaleMode){
			case (int)WindowResizeMode.KeepViewport:
				#region Start_KeepViewport
				Vector3 scale = new Vector3(windowInterface.localScale.x * plane.localScale.x,windowInterface.localScale.y * plane.localScale.y,windowInterface.localScale.z * plane.localScale.z);

				m_textureRect = new Rect(0,0,Mathf.RoundToInt(Screen.width * this.m_camera.rect.width),Mathf.RoundToInt(Screen.height * this.m_camera.rect.height));

				float rel = scale.x / scale.z;

				float viewRel = m_textureRect.width / m_textureRect.height;

				if(rel <= viewRel){
					/// 1/ rel = (height + a) / width
					/// a = width / rel - height
					float a = this.m_textureRect.width / rel - this.m_textureRect.height;
					m_textureRect.y = a / 2;
					m_textureRect.height += a;
					
					this.m_snapShot = new Texture2D((int)m_textureRect.width ,(int)m_textureRect.height,TextureFormat.RGB24, false);
					this.m_textureRect.height -= a;// on adapte la hauteur / largeur pour représenter le rectangle réelle du viewport
				}
				else{
					/// rel = (width + a) / height
					/// a = height * rel - width
					float a = this.m_textureRect.height * rel - this.m_textureRect.width;
					m_textureRect.x = a / 2;
					m_textureRect.width += a;
					
					this.m_snapShot = new Texture2D((int)m_textureRect.width ,(int)m_textureRect.height,TextureFormat.RGB24, false);
					
					this.m_textureRect.width -= a;// on adapte la hauteur / largeur pour représenter le rectangle réelle du viewport
				}

				#endregion
				break;
			case (int)WindowResizeMode.KeepPlaneScale:
				#region Start_KeepPlaneScale
				m_textureRect = new Rect(0,0,Mathf.RoundToInt(Screen.width * this.m_camera.rect.width),Mathf.RoundToInt(Screen.height * this.m_camera.rect.height));
				
				this.m_snapShot = new Texture2D((int)m_textureRect.width ,(int)m_textureRect.height,TextureFormat.RGB24, false);
				#endregion
				break;
			case (int)WindowResizeMode.RespectProportion:
				#region Start_RespectProportion
				m_textureRect = new Rect(0,0,Mathf.RoundToInt(Screen.width * this.m_camera.rect.width),Mathf.RoundToInt(Screen.height * this.m_camera.rect.height));
				
				Vector3 s = new Vector3(m_textureRect.width,0,m_textureRect.height);
				s.Normalize();
				s.y = 1; // on met le scale y à 1 pour des soucis de rendu
				windowInterface.localScale = s;

				this.m_snapShot = new Texture2D((int)m_textureRect.width ,(int)m_textureRect.height,TextureFormat.RGB24, false);
				#endregion
				break;
			case (int)WindowResizeMode.ProvideRect:
				#region Start_ProvideRect
				m_textureRect = new Rect(0,0,Mathf.RoundToInt(Screen.width * this.m_camera.rect.width) / this.m_windowRectangle.width,Mathf.RoundToInt(Screen.height * this.m_camera.rect.height)/  this.m_windowRectangle.height);
				m_textureRect.x = this.m_textureRect.width * this.m_windowRectangle.x;
				m_textureRect.y = this.m_textureRect.height * this.m_windowRectangle.y;

				this.m_snapShot = new Texture2D((int)m_textureRect.width ,(int)m_textureRect.height,TextureFormat.RGB24, false);

				m_textureRect.width *= this.m_windowRectangle.width;
				m_textureRect.height *= this.m_windowRectangle.height;
				#endregion
				break;
			}

			this.FillTexture(this.m_backgroundColor);

			plane.GetComponent<MeshRenderer>().material.mainTexture = this.m_snapShot;
		}
	}

	// rempli la texture de noir !
	private void FillTexture(Color fillColor){
		Color[] fillColorArray =  this.m_snapShot.GetPixels();
		
		for(int i = 0 ; i < fillColorArray.Length ; i++)
		{
			fillColorArray[i] = fillColor;
		}
		
		this.m_snapShot.SetPixels( fillColorArray );
		
		this.m_snapShot.Apply();
	}
	#endregion

	#region CAPTURE
	// capture ce que voit la caméra de la fenetre 3D
	public void Capture(){

		if(!this.m_active){
			return;
		}

		Rect viewport = new Rect(
			this.m_camera.rect.x * Screen.width,
			this.m_camera.rect.y * Screen.height,
			this.m_camera.rect.width * Screen.width,
			this.m_camera.rect.height * Screen.height
			);

		this.m_camera.Render(); // on rend l'image de la caméra
		this.m_snapShot.ReadPixels(viewport,(int) this.m_textureRect.x, (int) this.m_textureRect.y, false);// on fait une capture d'écran
		
		// That's the heavy part, it takes a lot of time.
		this.m_snapShot.Apply();
	}
	#endregion

	#region GETTER - SETTER
	public bool Active{
		set{this.m_active = value;}
		get{return this.m_active;}
	}
	public WindowResizeMode ScaleMode{
		get{return this.m_scaleMode;}
	}
	public Rect TextureRect{
		get{return this.m_textureRect;}
	}
	public Rect NormalizedTextureRect{
		get{return new Rect(this.m_textureRect.x / this.m_snapShot.width,
			                    this.m_textureRect.y / this.m_snapShot.height,
			                    this.m_textureRect.width / this.m_snapShot.width,
			                    this.m_textureRect.height / this.m_snapShot.height);}
	}
	#endregion

}