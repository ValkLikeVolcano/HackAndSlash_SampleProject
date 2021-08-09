using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
	PlayerHandler ph;
	[HideInInspector]
	public CharacterController controller;
	public Transform cam;
	public Transform groundCheck;
	public float groundDistance = 0.4f;
	public LayerMask groundMask;

	public float speed = 5f;
	public float jumpHeight = 4f;
	public float turnSmoothTime = 0.1f;
	float turnSmoothVelocity;
	public float gravity = -9.81f;
	[HideInInspector]
	public Vector3 velocity;
	[HideInInspector]
	public Vector3 moveDir;
	[HideInInspector]
	public bool grounded;

	public void Start()
	{
		controller = GetComponent<CharacterController>();
		ph = GetComponent<PlayerHandler>();
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = true;
	}

	public void Update()
	{
		grounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

		if(grounded && velocity.y < 0)
		{
			velocity.y = -2f;
		}

		float horizontal = Input.GetAxisRaw("Horizontal");
		float vertical = Input.GetAxisRaw("Vertical");

		Vector3 dir = new Vector3(horizontal, 0f, vertical).normalized;

		if(dir.magnitude >= 0.1f)
		{
			float targetAngle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
			float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);

			if (ph.canTurn)
			{
				transform.rotation = Quaternion.Euler(0f, angle, 0f);
			}	

			moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

			if (ph.canMove)
			{
				controller.Move(moveDir.normalized * speed * Time.deltaTime);
			}
		}

		if (Input.GetButtonDown("Jump") && grounded && ph.canJump)
		{
			velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
		}

		velocity.y += gravity * Time.deltaTime;

		if (ph.hasGravity)
		{
			controller.Move(velocity * Time.deltaTime);
		}
	}
}
