using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Snowdrama.Timer;
using UnityEngine.UI;

public class ExplosionExample : MonoBehaviour
{
    [Header("Explosion on a Timer")]
    public float delayBeforeExplosion;
    public ParticleSystem explosion;
    Timer explosionTimer;

    public Image buttonImage;

    // Start is called before the first frame update
    void Start()
    {
        explosionTimer = new Timer(delayBeforeExplosion);
        explosionTimer.OnComplete += ExplosionTimer_OnComplete;
    }

    private void ExplosionTimer_OnComplete()
    {
        explosion.Play();
    }

    // Update is called once per frame
    void Update()
    {
        explosionTimer.UpdateTime(Time.deltaTime);
        buttonImage.fillAmount = 1 - explosionTimer.GetPercentageComplete();
    }

    public void TimedExplosion()
    {
        explosionTimer.Start();
    }
}
