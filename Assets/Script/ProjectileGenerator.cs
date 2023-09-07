using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileGenerator : MonoBehaviour
{
    [SerializeField] GameObject m_projectilePrefab;
    [SerializeField] float m_projectileSpeed = 15f;
    [SerializeField] float m_spawnDelay = 5f;
    [SerializeField] Transform m_spawnLimitTopLeft;
    [SerializeField] Transform m_spawnLimitTopRight;
    private float m_spawnTimer = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        m_spawnTimer += Time.deltaTime;

        if (m_spawnTimer >= m_spawnDelay)
        {
            m_spawnTimer = 0.0f;

            GameObject lProjectile = Instantiate(m_projectilePrefab, transform.position, Quaternion.identity);
            lProjectile.transform.position = new Vector3(Random.Range(m_spawnLimitTopLeft.position.x, m_spawnLimitTopRight.position.x), m_spawnLimitTopLeft.position.y, m_spawnLimitTopLeft.position.z);
            lProjectile.GetComponent<Projectile>().SetSpeed(m_projectileSpeed);
        }
    }
}
