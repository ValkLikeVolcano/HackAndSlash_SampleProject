using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerTargeting : MonoBehaviour
{
    PlayerMovement moveScript;
    public Transform selectedTarget;
    public List<Transform> enemys;
    private Transform myTransform;

    public float enemyPinRadius = 5f;

    void Start()
    {
        selectedTarget = null;
        myTransform = transform;
    }

    void Update()
    {
        myTransform = transform;

        if (Input.GetButtonDown("Fire1"))
		{
            LookForEnemy();
		}
    }

    public void LookForEnemy()
	{
        Collider[] hitColliders = Physics.OverlapSphere(myTransform.position, enemyPinRadius);
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

        var closest = enemys.OrderBy(t => (t.position - myTransform.position).sqrMagnitude).First().transform;

        PinToEnemy(closest);

        closest = null;
        hitColliders = null;
    }

    public void PinToEnemy(Transform closest)
	{
        Vector3 moveVector = closest.position - transform.position;
        moveScript.controller.Move(moveVector);

        enemys.Clear();
    }
}
