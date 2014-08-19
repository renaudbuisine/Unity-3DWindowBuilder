using UnityEngine;
using System.Collections;

public class ListScrolling : MonoBehaviour {

	public int m_elementsByLine = 1;
	public float m_translationValue;
	public float m_speed = 1f;
	public WindowScrollBarDirection m_direction = WindowScrollBarDirection.Horizontal;

	private ScrollBehaviour m_scroll; // scrollbar de la liste
	private Transform m_list; // liste d'éléments
	private int m_position = 0; // déclage des éléments à cause de la bare de scrolling
	private Vector3 m_offsetTarget; // valeur a atteindre sur la droite ou gauche par la liste

	private float m_animationTime;

	private const float c_minTranslation = 20;

	// Use this for initialization
	void Start () {
		this.m_scroll = this.transform.FindChild("ScrollBar").GetComponent<ScrollBehaviour>();
		this.m_list = this.transform.FindChild("ListContentElts");
		this.m_offsetTarget = this.m_list.position;

		int steps = this.m_list.childCount / m_elementsByLine - 1;
		if(steps == 0){
			this.m_scroll.gameObject.SetActive(false); // on cache la barre de scrolling, elle ne sert à rien
		}
		else{
			this.m_scroll.Steps = steps;
		}
		this.m_animationTime = this.m_speed;
	}
	
	// Update is called once per frame
	void Update () {
		int nPos = (int)(this.m_scroll.Value * this.m_scroll.Steps);

		if(nPos != this.m_position){
			float offsetTarget = (this.m_position - nPos) * this.m_translationValue;
			this.m_offsetTarget += (this.m_direction == WindowScrollBarDirection.Horizontal ? new Vector3(offsetTarget,0,0) : new Vector3(0,offsetTarget,0));
			this.m_position = nPos;

			if(this.m_animationTime >= this.m_speed){
				this.m_animationTime = 0;
				StartCoroutine("ManageAnimation");
			}
		}
	}

	// coroutine pour l'animation de la transition d'ue page à l'autre
	private IEnumerator ManageAnimation(){
		while(this.m_animationTime < this.m_speed){
			yield return new WaitForSeconds(1f/30f);

			Vector3 diff = this.m_offsetTarget - this.m_list.position;
			Vector3 dist = diff / (1 - (this.m_animationTime / this.m_speed));
			Vector3 origin = this.m_offsetTarget - dist;
			
			this.m_animationTime += 1f/30f;
			
			if(this.m_animationTime > this.m_speed){
				this.m_animationTime = this.m_speed;
			}
			
			this.m_list.position = origin + (dist * this.m_animationTime / this.m_speed);
		}
	}
}
