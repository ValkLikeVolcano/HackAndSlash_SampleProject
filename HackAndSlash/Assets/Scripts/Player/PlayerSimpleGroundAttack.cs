using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerSimpleGroundAttack : MonoBehaviour
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
    public bool chasing;
    [HideInInspector]
    public bool onDesination;
    [HideInInspector]
    public bool attack;

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
		{
            attack = true;
            return;
		}

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.tag == "Enemy")
            {
                enemys.Add(hitCollider.transform);
            }
        }

        if (enemys.Count == 0)
		{
            attack = true;
            return;
        }  

        closest = enemys.OrderBy(t => (t.position - myTransform.position).sqrMagnitude).First().transform;

        StartCoroutine(PinToEnemy(closest));
    }

    private Vector3 _direction;
    private Quaternion _lookRotation;

    IEnumerator PinToEnemy(Transform enemy)
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
        {
            attack = true;
            chasing = false;
            yield return null;
        }
        else
            chasing = true;
        checkDistance = true;


        while (distance > 2f)
        {
            if (distance > enemyPinRadius)
			{
                yield return null;
            }     

            Vector3 movDiff = enemy.position - transform.position;

            transform.rotation = Quaternion.Slerp(transform.rotation, _lookRotation, Time.deltaTime * 25f);
            moveScript.controller.Move(movDiff.normalized * Time.deltaTime * 25);

            yield return null;
        }

        yield return new WaitUntil(() => distance <= 2f);

        transform.rotation = _lookRotation;
        chasing = false;
        onDesination = true;

        yield return new WaitForSeconds(0.1f);

        closest = null;
        hitColliders = null;
        enemys.Clear();
        checkDistance = false;
        canLookForEnemy = true;
        onDesination = false;
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
