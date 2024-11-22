using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSShootingControls : MonoBehaviour
{



    private Camera mainCam;

    private float fireRate = 15f;
    private float nextTimeToFire = 0f;



    public float damage = 7f;
    public GameObject impact;

    [SerializeField]
    private WeaponManager handsWeapon_Manager;

    void Start()
    {
        mainCam = Camera.main;
    }

    
    void Update()
    {
        Shoot();
    }


    public void Shoot()
    {

       if(Input.GetMouseButton(0) && Time.time > nextTimeToFire)
        {
            nextTimeToFire = Time.time + 1f / fireRate;

            RaycastHit hit;

            if (Physics.Raycast(mainCam.transform.position, mainCam.transform.forward, out hit))
            {
                if(hit.transform.tag == "Enemy")
                {
                    if (!handsWeapon_Manager.weapons[0].activeInHierarchy) damage = 10f;
                    else damage = 7f;

                    hit.transform.GetComponent<HealthScript>().ApplyDamage(damage);
                    Instantiate(impact, hit.point, Quaternion.LookRotation(hit.normal));
                }
            }
        }
    }


    // set damage for different weapons











} // class
