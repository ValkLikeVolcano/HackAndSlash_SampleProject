using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(PlayerMovement))]
public class PlayerDash : MonoBehaviour
{
	PlayerMovement moveScript;

	public float dashSpeed;
	public float dashTime;
	public float cooldownTime = 1f;
	bool isOnCooldown;

	private void Start()
	{
		moveScript = GetComponent<PlayerMovement>();
	}
	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.LeftShift) && moveScript.grounded && !isOnCooldown)
		{
			StartCoroutine(Cooldown());
			StartCoroutine(Dash());
		}

		IEnumerator Dash()
		{
			float startTime = Time.time;

			while(Time.time < startTime + dashTime)
			{
				moveScript.controller.Move(moveScript.moveDir * dashSpeed * Time.deltaTime);

				yield return null;
			}
		}

		IEnumerator Cooldown()
		{
			isOnCooldown = true;

			yield return new WaitForSeconds(cooldownTime);

			isOnCooldown = false;
		}
	}
}
