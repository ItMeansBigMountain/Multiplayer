using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;

public class GorillaAI : MonoBehaviourPunCallbacks
{
    [Header("Targeting")]
    public float detectRadius = 15f;
    public float dangerZone = 3f;
    public float attackCooldown = 2f;
    private NavMeshAgent agent;
    private Transform target;

    [Header("Attack Settings")]
    public int damage = 30;
    public float knockbackForce = 750f;
    public float punchRadius = 2f;
    public Transform punchOrigin;

    [Header("Health Settings")]
    public int maxHealth = 99999;
    private int currentHealth;
    private GameObject HealthBar;

    [Header("Animation Settings")]
    private Animator anim;
    private float nextAttackTime;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        HealthBar = transform.Find("Health_Bar").gameObject;
        currentHealth = maxHealth;
    }

    private void Update()
    {
        UpdateTarget();

        if (target)
        {
            float dist = Vector3.Distance(transform.position, target.position);

            if (dist > dangerZone)
            {
                anim.SetBool("isWalking", true);
                anim.SetBool("isAttacking", false);
                agent.SetDestination(target.position);
            }
            else
            {
                anim.SetBool("isWalking", false);
                agent.ResetPath();

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
            anim.SetBool("isAttacking", false);
        }
    }

    void UpdateTarget()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        float closest = detectRadius;
        Transform closestTarget = null;

        foreach (GameObject player in players)
        {
            float dist = Vector3.Distance(transform.position, player.transform.position);
            if (dist < closest)
            {
                closest = dist;
                closestTarget = player.transform;
            }
        }

        // Only lock if player is alive
        if (closestTarget != null)
            target = closestTarget;
    }

    // 🧠 Animation event calls this
    public void TryPunch()
    {
        Vector3 origin = punchOrigin != null ? punchOrigin.position : transform.position + transform.forward * 1.5f;
        Collider[] hits = Physics.OverlapSphere(origin, punchRadius);
        foreach (var hit in hits)
        {
            // Ignore self and other gorillas
            if (hit.transform.root == transform.root) continue;

            // Knockback Target
            Rigidbody rb = hit.GetComponent<Rigidbody>();

            // Apply Player Damage
            if (hit.CompareTag("Player"))
            {
                var player = hit.GetComponent<ArenaShooter_PlayerControls>();
                if (player != null)
                {
                    // Player Only Knockback (BORING)
                    //if (rb != null)
                    //    rb.AddForce((hit.transform.position - transform.position).normalized * knockbackForce, ForceMode.Impulse);

                    // Player Damage
                    Debug.Log($"[GORILLA] Hit player: {hit.name}");
                    player.ApplyDamage(damage);
                }
            }
            // OPTIONALLY KNOCK BACK EVERYTHING IN RADIUS WHEN GORILLA PUNCHES 
            if (rb != null)
            {
                rb.AddForce((hit.transform.position - transform.position).normalized * knockbackForce, ForceMode.Impulse);
            }
        }
    }

    public void ApplyDamage(int damage)
    {
        if (photonView == null || !photonView.IsMine)
        {
            Debug.LogWarning("PhotonView not available or not owned by this client.");
            return;
        }
        photonView.RPC("TakeDamage", RpcTarget.All, damage);
    }


    [PunRPC]
    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Max(0, currentHealth);
        HealthBar.GetComponent<HealthBar>().UpdateHealthBar(currentHealth, maxHealth);
        Debug.Log($"[GORILLA] Took {amount} damage. Current HP: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("[GORILLA] Died.");
        PhotonNetwork.Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        if (punchOrigin != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(punchOrigin.position, punchRadius);
        }

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectRadius);
    }

}
