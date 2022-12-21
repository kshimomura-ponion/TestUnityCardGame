using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MonoEventTrigger : MonoBehaviour
{
	public UnityEvent onAwake = new UnityEvent ();
	public UnityEvent onDestroy = new UnityEvent ();

	void Awake()
	{
		onAwake.Invoke ();
	}

	void OnDestroy()
	{
		onDestroy.Invoke ();
	}
}