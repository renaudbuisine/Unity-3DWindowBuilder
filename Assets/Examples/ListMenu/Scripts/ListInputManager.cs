using UnityEngine;

public class ListInputManager : MonoBehaviour, IWindowListener{

	public string m_cameraName;
	public string m_subListName;
	public string m_subWindowName;
	public int m_elementsByLine = 1;
	public Vector3 m_stepTranslation;
	
	private WindowInputsManagement m_inputManagement;
	private Transform m_list; // liste d'éléments
	private Transform m_subList;
	private Vector3 m_basePosition;

	private WindowBehaviour m_behaviour;
	private Transform m_subWindowInterface;

	private int m_selectedInd = 0;
	private int m_previousSelectedInd = 0;

	// Use this for initialization
	void Start () {
		GameObject camera = GameObject.Find(m_cameraName);
		this.m_inputManagement = camera.GetComponent<WindowInputsManagement>();

		this.m_list = this.transform.FindChild("ListContentElts");

		this.m_subList = GameObject.Find(m_subListName).transform;

		this.m_behaviour = GameObject.Find(m_subWindowName).GetComponent<WindowBehaviour>();
		this.m_behaviour.ForceReduce(); // on ferme par défaut la fenetre

		this.m_subWindowInterface = this.m_behaviour.transform.FindChild("Interface");

		this.m_basePosition = this.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		
		// On vérifie si l'on clic sur un élément de la liste
		/*RaycastHit hit;
		if(Input.GetMouseButtonDown(0)){
			if(this.m_inputManagement.WindowPhysicsRaycast(this.m_inputManagement.MouseRelativePosition,out hit)){
				if(hit.transform.name == "TabPlane" && hit.transform.parent.parent.parent.GetInstanceID() == this.m_list.GetInstanceID()){
					string subName = hit.transform.parent.name; // on récupère l'indice de l'élément cliqué afin de déterminer quelle liste ouvrir
					string lineName = hit.transform.parent.parent.name;
					int subInd = int.Parse(subName.Substring(subName.IndexOf("_") + 1));
					int lineInd = int.Parse(lineName.Substring(lineName.IndexOf("_") + 1));
					int finalInd = lineInd * this.m_elementsByLine + subInd;

					this.m_previousSelectedInd = this.m_selectedInd;

					// si l'on reclique sur le meme element on le referme
					if(this.m_selectedInd == finalInd){
						if(this.m_behaviour.Reduced){
							this.m_behaviour.RestoreWindow(); // on ouvre la fenetre
						}
						else{
							this.m_behaviour.ReduceWindow(); // on ferme la fenetre
						}
					}
					else{
						this.m_selectedInd = finalInd;
						if(this.m_behaviour.Reduced){
							FindSubListElement(this.m_previousSelectedInd).SetActive(false); // on éteint l'ancienne fenetre
							FindSubListElement(this.m_selectedInd).SetActive(true); // on allume la nouvelle
							this.m_behaviour.RestoreWindow(); // on ouvre la fenetre
						}
						else{
							this.m_behaviour.ReduceWindow(); // on ferme la fenetre
						}
					}
				}
			}
		}*/
	}

	// trouve la sous liste correspondante à un indice
	private GameObject FindSubListElement(int ind){
		return this.m_subList.FindChild(this.m_subListName + "Content_" + ind).gameObject;
	}

	private void RestoreSubList(){
		Debug.Log(FindSubListElement(this.m_previousSelectedInd).name);
		Debug.Log(FindSubListElement(this.m_selectedInd).name);
		FindSubListElement(this.m_previousSelectedInd).SetActive(false); // on éteint l'ancienne fenetre
		FindSubListElement(this.m_selectedInd).SetActive(true); // on allume la nouvelle
		this.m_subWindowInterface.position = this.m_basePosition + this.m_stepTranslation * (this.m_selectedInd % this.m_elementsByLine);
		this.m_behaviour.RestoreWindow(); // on ouvre la fenetre
	}

	#region WINDOWLISTENER
	
	public void OnClosing(Object window){}
	public void OnRestoring(Object window){}
	public void OnReducing(Object window){
		if(((WindowBehaviour)window).gameObject.GetInstanceID() == this.m_inputManagement.gameObject.GetInstanceID()){
			this.m_behaviour.ForceReduce();
		}
	}

	public void OnPostRestoring(Object window){
		Debug.Log("dkjflkdjflkdsjflkdsjf");
	}

	// fonction utilisé pour autoriser l'ouverture de la fenetre à nouveau
	public void OnPostReducing(Object window){
		if(((WindowBehaviour)window).gameObject.GetInstanceID() == this.m_behaviour.gameObject.GetInstanceID()){
			this.RestoreSubList();
		}
	}

	#endregion
}
