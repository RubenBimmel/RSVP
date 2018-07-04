using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCounterCondition", menuName = "Events/Counter condition")]
public class CounterCondition : Condition {
	public int startingValue;
	public int targetValue;
	private int value;

	public override bool IsTrue ()
	{
		return (value == targetValue);
	}

	public void AddValue (int deltaValue) {
		value += deltaValue;
	}

	public override void Reset ()
	{
		value = startingValue;
		base.Reset ();
	}
}
