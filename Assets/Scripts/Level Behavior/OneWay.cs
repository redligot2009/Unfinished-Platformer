using UnityEngine;
using System.Collections;

public class OneWay : MonoBehaviour {

	public enum OneWayType{ up = 0, down = 1, left = 2, right = 3}

	public OneWayType myType = OneWayType.up;
}
