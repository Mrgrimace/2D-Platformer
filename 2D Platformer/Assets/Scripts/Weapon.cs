﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour {

    public float fireRate = 0;
    public int damage = 10;
    public LayerMask whatToHit;

    public Transform BulletTrailPrefab;
    public Transform MuzzleFlashPrefab;
    public Transform HitPrefab;
    float timeToSpawEffect = 0;
    public float effectSpawnRate = 10;

    //handle camera shaking
    public float CamShakeAmt = 0.05f;
    public float CamShakeLength = 0.1f;
    CameraShake camShake;

    float timeToFire = 0;
    Transform firePoint;

	// Use this for initialization
	void Awake () {
        firePoint = transform.Find("FirePoint");
        if(firePoint==null)
        {
            Debug.LogError("No FirePoint");
        }
	}
    private void Start()
    {
        camShake = GameMaster.gm.GetComponent<CameraShake>();
        if (camShake == null)
            Debug.LogError("No camera shake script found on GM object");

    }

    // Update is called once per frame
    void Update () {
		if(fireRate == 0)
        {
            if(Input.GetButton("Fire1"))
            {
                Shoot();
            }
        }
        else
        {
            if(Input.GetButton ("Fire1") && Time.time > timeToFire)
            {
                timeToFire = Time.time + 1 / fireRate;
                Shoot();
            }
        }

	}

    void Shoot()
    {
        Vector2 mousePosition = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y);
        Vector2 firePointPosition = new Vector2(firePoint.position.x, firePoint.position.y);

        RaycastHit2D hit = Physics2D.Raycast(firePointPosition, mousePosition- firePointPosition, 100,whatToHit);
        
        Debug.DrawLine(firePointPosition, (mousePosition-firePointPosition)*100,Color.cyan);

        if(hit.collider!=null)
        {
            Debug.DrawLine(firePointPosition, hit.point, Color.red); 
            Enemy enemy = hit.collider.GetComponent<Enemy>();
            if(enemy!= null)
            {
                enemy.DamageEnemy(damage);
                Debug.Log("We hit " + hit.collider.name + "and did" + damage + "damage");
            }
        }

        if (Time.time >= timeToSpawEffect)
        {

            Vector3 hitPosition;
            Vector3 hitNormal;

            if (hit.collider == null)
            {
                hitPosition = (mousePosition - firePointPosition) * 30;
                hitNormal = new Vector3(9999, 9999, 9999);
            }  
            else
            {
                hitPosition = hit.point;
                hitNormal = hit.normal;
            }
                
            Effect(hitPosition, hitNormal);
            timeToSpawEffect = Time.time + 1 / effectSpawnRate;
        }
    }

    void Effect(Vector3 hitPosition,Vector3 hitNormal)
    {
        Transform trail = Instantiate(BulletTrailPrefab,firePoint.position,firePoint.rotation)as Transform;
        LineRenderer lr = trail.GetComponent<LineRenderer>();

        if(lr!=null)
        {
            lr.SetPosition(0,firePoint.position);
            lr.SetPosition(1, hitPosition);
        }
        Destroy(trail.gameObject, 0.04f);

        if(hitNormal!= new Vector3(9999,9999,9999))
        {
            Transform hitParticle = Instantiate(HitPrefab,hitPosition,Quaternion.FromToRotation(Vector3.right,hitNormal)) as Transform;
            Destroy(hitParticle.gameObject, 0.4f);
        }

        Transform clone = Instantiate(MuzzleFlashPrefab, firePoint.position, firePoint.rotation) as Transform;
        clone.parent = firePoint;
        float size = Random.Range(0.6f, 0.9f);
        clone.localScale = new Vector3(size, size, size);
        Destroy(clone.gameObject, 0.02f);

        //Shake the camera
        camShake.Shake(CamShakeAmt, CamShakeLength);
    }
}
