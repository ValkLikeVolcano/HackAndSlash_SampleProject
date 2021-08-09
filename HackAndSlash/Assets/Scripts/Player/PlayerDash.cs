using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(PlayerMovement))]
public class PlayerDash : MonoBehaviour
{
	PlayerHandler ph;
	PlayerMovement moveScript;

	public float dashSpeed;
	public float dashTime;
	public float cooldownTime = 1f;
	bool isOnCooldown;

	private void Start()
	{
		moveScript = GetComponent<PlayerMovement>();
		ph = GetComponent<PlayerHandler>();
	}
	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.LeftShift) && !isOnCooldown && ph.canDash)
		{
			ph.canMove = false;
			ph.hasGravity = false;
			ph.canJump = false;
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
			yield return new WaitUntil(() => Time.time >= startTime + dashTime);

			ph.canMove = true;
			ph.hasGravity = true;
			ph.canJump = true;
		}

		IEnumerator Cooldown()
		{
			isOnCooldown = true;

			yield return new WaitForSeconds(cooldownTime);

			isOnCooldown = false;
		}
	}
}
