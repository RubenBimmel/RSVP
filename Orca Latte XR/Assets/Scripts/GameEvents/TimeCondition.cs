using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewTimeCondition", menuName = "Events/Time condition")]
public class TimeCondition : Condition {
	public int time;
	public override bool IsTrue ()
	{
		return (GameManager.time > time);
	}
}
