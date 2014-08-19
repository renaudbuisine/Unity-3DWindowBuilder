using UnityEngine;
using System.Collections;

public interface IWindowListener{
	void OnClosing(Object window);
	void OnRestoring(Object window);
	void OnReducing(Object window);
	void OnPostRestoring(Object window);
	void OnPostReducing(Object window);
}
