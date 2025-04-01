using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Snowdrama.Timer;
using UnityEngine.UI;

public class ShootingExample : MonoBehaviour
{
    [Header("Shoot Itterator")]
    public float delayBetweenShots;
    Iterator shootIterator;
    public ParticleSystem shootParticle;
    public GameObject bulletPrefab;
    public bool shooting = false;
    public bool canShoot = true;

    [Header("Cooldown")]
    public float cooldownTime;
    Timer cooldownTimer;
    public Image cooldownImage;
    // Start is called before the first frame update
    void Start()
    {
        shootIterator = new Iterator(delayBetweenShots, -1, false); // infinite duration, don't start automatically
        shootIterator.OnStart += ShootIterator_OnStart;
        shootIterator.OnIterate += ShootIterator_OnIterate;

        cooldownTimer = new Timer(cooldownTime);
        cooldownTimer.OnComplete += CooldownTimer_OnComplete;
    }


    private void ShootIterator_OnStart()
    {
        Shoot();
    }

    private void ShootIterator_OnIterate()
    {
        Shoot();
    }

    public void Shoot()
    {
        Instantiate(bulletPrefab, this.transform);
        shootParticle.Play();
    }

    // Update is called once per frame
    void Update()
    {
        shootIterator.UpdateTime(Time.deltaTime);
        cooldownTimer.UpdateTime(Time.deltaTime);
        cooldownImage.fillAmount = 1 - cooldownTimer.GetPercentageComplete();
        if (shooting && canShoot && !shootIterator.Active)
        {
            shootIterator.Start();
        }

        if(!shooting && shootIterator.Active)
        {
            shootIterator.Stop();
        }
    }

    public void StartShoot()
    {
        shooting = true;
    }

    public void StopShoot()
    {
        shooting = false;
        canShoot = false;
        cooldownTimer.Start();
    }

    private void CooldownTimer_OnComplete()
    {
        canShoot = true;
    }

}
