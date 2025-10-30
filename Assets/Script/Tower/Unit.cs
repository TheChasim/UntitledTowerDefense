using System.Collections;
using UnityEngine;
using static UnityEngine.UI.Image;

public class Unit : MonoBehaviour
{

    internal GameObject target;
    internal Tower campRef;

    [Header("Stats")]
    [SerializeField] internal float damage = 1f;
    [SerializeField] internal float range = 0.75f;
    [SerializeField] internal float cooldown = 1f;
    [SerializeField] internal float health = 5f;
    [SerializeField] internal float speed = 5f;
    bool canAtack = true;

    [Header("Séparation simple")]
    [SerializeField] float radiusForEnemy = 0.25f;
    [SerializeField] float separationRadius = 0.5f;
    [SerializeField] float separationForce = 1f;
    [SerializeField] LayerMask obstaclesMask;


    //internal Vector3 velocity; // direction courante lissée


    // Start is called before the first frame update
    void Start()
    {
        Tower.allUnitList.Add(this.gameObject);
        speed = UnityEngine.Random.Range(speed - 0.75f, speed + 0.75f);
        //velocity = transform.forward * speed;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (target == null)
        { return; }

        if (Vector3.Distance(transform.position, target.transform.position) < range)
        {
            if (canAtack)
            {
                StartCoroutine(OnAttack());
            }
        }

        //diretion vers un enemi
        Vector3 dir = (target.transform.position - transform.position);
        dir.y = 0f;

        //direction opposer si trop proche unit
        Vector3 separation = Vector3.zero;
        foreach (var unit in Tower.allUnitList)
        {
            if (unit == null || unit == gameObject)
            { continue; }

            float dist = Vector3.Distance(transform.position, unit.transform.position);
            if (dist < separationRadius && dist > 0.001f)
            {
                // Force de répulsion inversement proportionnelle à la distance
                float force = 1f - (dist / separationRadius); // plus proche = plus fort
                Vector3 push = (transform.position - unit.transform.position).normalized * force;

                separation += push;
            }
        }

        // 3. Combine le mouvement vers la cible + séparation
        Vector3 moveDir = (dir.normalized + separation * separationForce).normalized;

        bool hasLineOfSight = !Physics.Linecast(transform.position, target.transform.position, obstaclesMask);
        // Debug visuel
        Debug.DrawLine(transform.position, target.transform.position, hasLineOfSight ? Color.green : Color.red);

        // 5. Déplace l'unité
        if (hasLineOfSight && Vector3.Distance(transform.position, target.transform.position) > radiusForEnemy)
        {
            transform.position += moveDir * speed * Time.fixedDeltaTime;
        }

        transform.position = new Vector3 (transform.position.x, 0.25f, transform.position.z);
    }

    private IEnumerator OnAttack()
    {
        //if(target == null)
        //{
        //    yield break;
        //}

        canAtack = false;

        target.GetComponent<EnemyAI>().OnTakeDamage(damage);

        health -= damage;
        if (health <= 0f)
        { Destroy(gameObject); }

        yield return new WaitForSeconds(cooldown);

        canAtack = true;
    }

    private void OnDestroy()
    {
        Tower.allUnitList.Remove(this.gameObject);
        campRef.unitlist.Remove(this.gameObject);
    }
}
