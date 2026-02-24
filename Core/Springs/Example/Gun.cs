using Snowdrama.Timer;
using UnityEngine;


/// <summary>
/// Triggers a particle system every 0.5 seconds
/// </summary>
public class TimedExplosion : MonoBehaviour
{
    [SerializeField] private ParticleSystem ParticleSystem;
    private Timer myTimer;
    private void Start()
    {
        myTimer = new Timer(0.5f, true, true);
        myTimer.OnComplete += Fire;
    }
    private void Update()
    {
        myTimer.UpdateTime(Time.deltaTime);
    }

    public void Fire()
    {
        ParticleSystem.Play();
    }
}