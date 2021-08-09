using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HammerController : MonoBehaviour
{
	PlayerTargeting pt;
	Animator anim;

	private void Start()
	{
		pt = FindObjectOfType<PlayerTargeting>();
		anim = GetComponent<Animator>();
	}
	private void Update()
	{
		if (pt.simpleAtk)
		{
			anim.SetTrigger("simpleTargetAttack");
			pt.simpleAtk = false;
		}
	}

	public void OnCollisionEnter(Collision collision)
	{
		if(collision.rigidbody != null)
		{
			collision.collider.GetComponent<Rigidbody>().AddForceAtPosition(20 * transform.forward, collision.GetContact(0).point, ForceMode.Impulse);
		}
	}
}
