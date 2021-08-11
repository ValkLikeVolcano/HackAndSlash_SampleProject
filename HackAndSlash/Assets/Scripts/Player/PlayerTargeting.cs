using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerTargeting : MonoBehaviour
{
    PlayerHandler ph;
    PlayerMovement moveScript;
    public Collider[] hitColliders;
    public List<Transform> frontEnemys;
    public List<Transform> backEnemys;
    public Transform selectedTarget;
    private Transform myTransform;

    public float enemyPinRadius = 5f;
    public float attackCooldown = 1f;
    public float moveSpeed = 25f;
    public float rotationSpeed = 30f;
    private float distance;

    bool checkDistance;
    bool canLookForEnemy;
    bool isOnCooldown;


    Transform closest;

    void Start()
    {
        selectedTarget = null;
        myTransform = transform;
        moveScript = GetComponent<PlayerMovement>();
        ph = GetComponent<PlayerHandler>();
        closest = null;
        canLookForEnemy = true;
    }

    public void Update()
    {
        myTransform = transform;

        if (Input.GetButtonDown("Fire1") && canLookForEnemy && !isOnCooldown && moveScript.grounded)
		{
            LookForEnemy();
		}

		if (checkDistance && closest != null)
		{
            distance = Vector3.Distance(transform.position, closest.position);
        }
    }

    public void LookForEnemy()
	{
        hitColliders = Physics.OverlapSphere(myTransform.position, enemyPinRadius);

        if (hitColliders == null)
            return;

        foreach(var hitCollider in hitColliders)
		{
            if(hitCollider.tag == "Enemy")
			{
                Vector3 enemyDir = hitCollider.transform.position - transform.position;

                if(Vector3.Angle(enemyDir, transform.forward) < 80f)
				{
                    frontEnemys.Add(hitCollider.transform);
                }
                else if(Vector3.Angle(enemyDir, transform.forward) > 80f)
				{
                    backEnemys.Add(hitCollider.transform);
				}
			}
		}

        if (frontEnemys.Count == 0)
		{
            if (backEnemys.Count == 0)
                return;

            closest = backEnemys.OrderBy(t => (t.position - myTransform.position).sqrMagnitude).First().transform;
        }
        else
            closest = frontEnemys.OrderBy(t => (t.position - myTransform.position).sqrMagnitude).First().transform;

        StartCoroutine(RotateToEnemy(closest));
    }

    private Vector3 _direction;
    private Quaternion _lookRotation;

    IEnumerator RotateToEnemy(Transform enemy)
	{
        canLookForEnemy = false;
        ph.canDash = false;
        ph.canTurn = false;
        ph.canMove = false;
        ph.canJump = false;

        _direction = (enemy.position - transform.position).normalized;

        _lookRotation = Quaternion.LookRotation(new Vector3(_direction.x, 0f, _direction.z));

        distance = Vector3.Distance(transform.position, closest.position);

        if (distance <= 2.5f)
            yield return null;

        checkDistance = true;

        while (distance > 2f)
		{
            if (distance > enemyPinRadius)
                yield return null;

            Vector3 movDiff = enemy.position - transform.position;

            transform.rotation = Quaternion.Slerp(transform.rotation, _lookRotation, Time.deltaTime * rotationSpeed);
            moveScript.controller.Move(movDiff.normalized * Time.deltaTime * moveSpeed);

            yield return null;
        }

        yield return new WaitUntil(()=> distance <= 2f);

        transform.rotation = _lookRotation;

        yield return new WaitForSeconds(0.1f);

        closest = null;
        hitColliders = null;
        frontEnemys.Clear();
        backEnemys.Clear();
        checkDistance = false;
        canLookForEnemy = true;
        ph.canDash = true;
        ph.canTurn = true;
        ph.canMove = true;
        ph.canJump = true;
        StartCoroutine(SwingCooldown());
    }

    IEnumerator SwingCooldown()
	{
        isOnCooldown = true;

        yield return new WaitForSeconds(attackCooldown);

        isOnCooldown = false;
    }

	private void OnDrawGizmosSelected()
	{
        Gizmos.DrawWireSphere(transform.position, enemyPinRadius);
	}
}
