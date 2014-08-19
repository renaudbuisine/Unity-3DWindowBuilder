using UnityEngine;
using System.Collections;

public interface IWindowedInputListener{
	// fonction appelé avant chaque rafraissement de l'affichage d'une fenetre
	void ManageInputs(GameObject management);
}
