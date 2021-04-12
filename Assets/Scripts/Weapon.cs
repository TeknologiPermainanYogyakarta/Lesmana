using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public int damage;

    // Start is called before the first frame update
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Musuh"))
        {
            giveDamage(collision.GetComponent<Health>());
        }
    }

    public void giveDamage(Health target)
    {
        if (target != null)
        {
            target.Decrement(damage);
        }
    }
}