using UnityEngine;
using UnityEngine.AI;

public class GorillaAI : MonoBehaviour
{
    public float detectRadius = 15f;
    public float attackRange = 2f;
    public float attackCooldown = 2f;

    private Transform target;
    private NavMeshAgent agent;
    private Animator anim;
    private float nextAttackTime;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        FindPlayer();

        if (target)
        {
            float dist = Vector3.Distance(transform.position, target.position);

            if (dist > attackRange)
            {
                agent.SetDestination(target.position);
                anim.SetBool("isWalking", true);
                anim.SetBool("isAttacking", false);
            }
            else
            {
                agent.ResetPath();
                anim.SetBool("isWalking", false);

                if (Time.time > nextAttackTime)
                {
                    anim.SetBool("isAttacking", true);
                    nextAttackTime = Time.time + attackCooldown;
                }
            }
        }
        else
        {
            anim.SetBool("isWalking", false);
        }
    }

    void FindPlayer()
    {
        if (target) return;

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        float shortest = Mathf.Infinity;

        foreach (GameObject player in players)
        {
            float dist = Vector3.Distance(transform.position, player.transform.position);
            if (dist < shortest && dist <= detectRadius)
            {
                shortest = dist;
                target = player.transform;
            }
        }
    }
}
