using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor (typeof (InputLayer))]
public class CSInputEditor : Editor
{
	public override void OnInspectorGUI()
	{
		InputLayer inputLayer = (InputLayer)target;

		if (DrawDefaultInspector())
		{
			inputLayer.UpdateChange();
		}
	}
}
