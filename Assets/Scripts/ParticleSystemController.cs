using UnityEngine;

public class ParticleSystemController : MonoBehaviour
{
    [SerializeField] private new ParticleSystem particleSystem;
    [SerializeField] private new Transform transform;


    private void Awake()
    {
        if (!particleSystem)
            particleSystem = GetComponentInChildren<ParticleSystem>();

        if (!transform)
            transform = GetComponent<Transform>();
    }

    public void SetTrailMaterial(Material mat)
    {
        particleSystem.GetComponent<ParticleSystemRenderer>().trailMaterial = mat;
    }

    public void PlayAt(Vector2 position, Vector2 direction)
    {
        SetTransform(position, direction);
        Play();
    }

    public void Stop()
    {
        if (particleSystem.isStopped || particleSystem.isPaused) return;
        particleSystem.Stop();
    }

    private void SetTransform(Vector2 position, Vector2 direction)
    {
        transform.Translate(position);
        var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.eulerAngles = new Vector3(0, 0, angle);
    }

    private void Play()
    {
        if (particleSystem.isPlaying && particleSystem.isEmitting) return;
        particleSystem.Play();
    }
}