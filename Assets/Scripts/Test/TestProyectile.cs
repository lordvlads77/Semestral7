using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestProyectile : MonoBehaviour
{
    public Projectile projectile;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Assert(projectile != null, "Need to have assigned a '" + nameof(Projectile) + "' to work");
        var player = GameObject.FindGameObjectWithTag("Player");
        projectile.setDestination(player.transform.position);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
