using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHandler : MonoBehaviour
{
	public bool canDash;
	public bool canMove;
	public bool canTurn;
	public bool canJump;
	public bool hasGravity;

	private void Start()
	{
		canDash = true;
		canMove = true;
		canTurn = true;
		canJump = true;
		hasGravity = true;
	}
}
