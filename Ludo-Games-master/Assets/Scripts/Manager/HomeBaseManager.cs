using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeBaseManager : MonoBehaviour {

	[SerializeField]
	private GameObject homebases;

	private Transform[] blueHomeBase = new Transform[4];
	private Transform[] redHomeBase = new Transform[4];
	private Transform[] greenHomeBase = new Transform[4];
	private Transform[] yellowHomeBase = new Transform[4];

	public Transform[] BlueHomeBase {
		get { return blueHomeBase; }
	}

	public Transform[] RedHomeBase {
		get { return redHomeBase; }
	}

	public Transform[] GreenHomeBase {
		get { return greenHomeBase; }
	}

	public Transform[] YellowHomeBase {
		get { return yellowHomeBase; }
	}

	void Start ()
	{
		GetHomeBasePoints (blueHomeBase, "BlueHomeBasePoints");
		GetHomeBasePoints (redHomeBase, "RedHomeBasePoints");
		GetHomeBasePoints (greenHomeBase, "GreenHomeBasePoints");
		GetHomeBasePoints (yellowHomeBase, "YellowHomeBasePoints");
	}

	private void GetHomeBasePoints(Transform[] outHomeBasePoints, string homeBasePointsName)
	{
		Transform t = homebases.transform.Find (homeBasePointsName);
		if (t != null) {
			int count = Mathf.Min (t.childCount, outHomeBasePoints.Length);
			for (int i = 0; i < count; i++) {
				outHomeBasePoints [i] = t.GetChild (i);
			}
		}
	}
}
