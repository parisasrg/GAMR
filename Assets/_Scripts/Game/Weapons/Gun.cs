using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Gun : MonoBehaviour
{
    [Header("References")]
    public GunData gunData;
    [SerializeField]
    private Transform muzzle;

    float timeSinceLastShot;

    public Animator gunAnimator;

    public AudioSource audioSource;
    [SerializeField]
    private AudioClip gunShotClip;
    [SerializeField]
    private AudioClip gunEmptyClip;
    [SerializeField]
    private AudioClip gunReloadClip;

    // Detecting index finger
    [SerializeField]
    private WeaponInteractable weaponInteractable;
    bool indexCurled = false;

    // Bullet counter
    public TextMeshPro bulletcounter;

    // Gun Aim
    public GameObject aim;

    private void Start() {
        PlayerController.shootInput += Shoot;
        PlayerController.reloadInput += StartReload;

        gunData.currentAmmo = 10;
        gunData.magStored = 0;
        bulletcounter.text = gunData.currentAmmo.ToString() + "/" + gunData.magStored.ToString();

        aim.SetActive(false);
    }

    public void ChangeText()
    {
        bulletcounter.text = gunData.currentAmmo.ToString() + "/" + gunData.magStored.ToString();
    }

    public void StartReload()
    {
        if(!gunData.reloading)
        {
            // reload
            StartCoroutine(Reload());
        }
    }

    private IEnumerator Reload()
    {
        gunData.reloading = true;

        audioSource.clip = gunReloadClip;
        audioSource.Play();
        // audioSource.PlayOneShot(gunReloadClip);

        yield return new WaitForSeconds(gunData.reloadTime);

        gunData.currentAmmo = gunData.magSize;
        ChangeText();

        gunData.reloading = false;
    }

    private bool CanShoot() => !gunData.reloading && timeSinceLastShot > 1f / (gunData.fireRate / 60f);

    public void Shoot()
    {
        if(gunData.currentAmmo > 0)
        {
            gunAnimator.ResetTrigger("Fire");
            if(CanShoot())
            { 
                gunAnimator.SetTrigger("Fire");
                audioSource.clip = gunShotClip;
                audioSource.Play();
                // audioSource.PlayOneShot(gunShotClip);

                if(Physics.Raycast(muzzle.position, -transform.right, out RaycastHit hitInfo, gunData.maxDistance))
                {
                    if(hitInfo.transform.tag == "Enemy")
                    {
                        hitInfo.transform.gameObject.GetComponent<EnemyStats>().TakeDamage(30);
                        // Destroy(hitInfo.transform.gameObject);
                    }
                }

                gunData.currentAmmo--;
                ChangeText();
                timeSinceLastShot = 0;
            }
        }
        else
        {
            if(gunData.magStored > 0)
            {
                StartReload();
                gunData.magStored -= gunData.magSize;
                // ChangeText();
            }
            else
            {
                audioSource.clip = gunEmptyClip;
                audioSource.Play();
                // audioSource.PlayOneShot(gunEmptyClip);
            }  
        }
    }

    private void FixedUpdate() 
    {
        if(Physics.Raycast(muzzle.position, -transform.right, out RaycastHit hitInfo, gunData.maxDistance))
        {
            aim.transform.position = hitInfo.point;
            aim.SetActive(true);
        }
        else
        {
            aim.SetActive(false);
        }
    }

    private void Update() {
        timeSinceLastShot += Time.deltaTime;

        // if(Physics.Raycast(muzzle.position, -transform.right, out RaycastHit hitInfo, gunData.maxDistance))
        // {
        //     // aim.transform.position = hitInfo.point;
        //     aim.transform.position = Vector3.Lerp (aim.transform.position, hitInfo.point, Time.deltaTime * 1f);
        //     aim.SetActive(true);
        // }
        // else
        // {
        //     aim.SetActive(false);
        // }
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        // Gizmos.DrawRay(transform.position, -transform.right*gunData.maxDistance);
    }
}
