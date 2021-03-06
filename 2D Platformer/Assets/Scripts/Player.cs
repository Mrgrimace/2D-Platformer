﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    [System.Serializable] //permet de voir la classe en public
    public class PlayerStats
    {
        public int Health = 100;

    }
    public PlayerStats playerStats =new PlayerStats();

    public int fallBoundary = -20;

    public void DamagePlayer(int damage)
    {
        playerStats.Health -= damage;
        if(playerStats.Health<=0)
        {
            Debug.Log("dead");
            GameMaster.KillPlayer(this);
        }
    }

    private void Update()
    {
        if(transform.position.y<=fallBoundary)
        {
            DamagePlayer(100000);    
        }
    }

}
