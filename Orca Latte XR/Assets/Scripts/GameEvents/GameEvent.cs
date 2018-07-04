using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameEvent : MonoBehaviour {
	public Condition[] conditions;
	public UnityEvent OnTrigger;
	public bool triggerOnce;
	private bool triggered;

	public void Evaluate () {
		if (!triggered) {
			foreach (Condition c in conditions) {
				if (!c.IsTrue ()) {
					return;
				}
				OnTrigger.Invoke ();
				triggered = true;
			}
		} 
		else if (!triggerOnce) {
			foreach (Condition c in conditions) {
				if (!c.IsTrue ()) {
					triggered = false;
					return;
				}
			}
		}
	}

	public void Reset () {
		triggered = false;
		foreach (Condition c in conditions) {
			c.Reset ();
		}
	}
}
