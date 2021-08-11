using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HammerController : MonoBehaviour
{
	PlayerSimpleGroundAttack psgt;
	Animator anim;

	private void Start()
	{
		psgt = FindObjectOfType<PlayerSimpleGroundAttack>();
		anim = GetComponent<Animator>();
	}
	private void Update()
	{
		if (psgt.chasing)
		{
			anim.SetTrigger("simpleTargetAttack");
			psgt.chasing = false;
		}
		if (psgt.onDesination)
		{
			anim.SetTrigger("simpleTargetFollowAttack");
			psgt.onDesination = false;
		}
		if (psgt.attack)
		{
			anim.SetTrigger("simpleAttack");
			psgt.attack = false;
		}
	}

	//public void OnCollisionEnter(Collision collision)
	//{
		//if(collision.rigidbody != null)
		//{
		//	collision.collider.GetComponent<Rigidbody>().AddForceAtPosition(20 * transform.forward, collision.GetContact(0).point, ForceMode.Impulse);
		//}
	//}
}
