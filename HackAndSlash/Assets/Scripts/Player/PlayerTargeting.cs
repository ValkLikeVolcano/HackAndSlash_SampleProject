using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerTargeting : MonoBehaviour
{
    PlayerHandler ph;
    PlayerMovement moveScript;
    public Transform selectedTarget;
    public List<Transform> enemys;
    [HideInInspector]
    public Collider[] hitColliders;
    private Transform myTransform;

    public float enemyPinRadius = 5f;
    public float attackCooldown = 1f;
    private float distance;

    bool checkDistance;
    bool canLookForEnemy;
    bool isOnCooldown;
    [HideInInspector]
    public bool simpleAtk;

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
                enemys.Add(hitCollider.transform);
			}
		}

        if (enemys.Count == 0)
            return;

        closest = enemys.OrderBy(t => (t.position - myTransform.position).sqrMagnitude).First().transform;

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
        simpleAtk = true;

        while (distance > 2f)
		{
            Vector3 movDiff = enemy.position - transform.position;

            transform.rotation = Quaternion.Slerp(transform.rotation, _lookRotation, Time.deltaTime * 20f);
            moveScript.controller.Move(movDiff.normalized * Time.deltaTime * 30);

            yield return null;
        }

        yield return new WaitUntil(()=> distance <= 2f);

        transform.rotation = _lookRotation;

        yield return new WaitForSeconds(0.1f);

        closest = null;
        hitColliders = null;
        enemys.Clear();
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
