using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Affected : MonoBehaviour
{
    readonly static Dictionary<Rigidbody, Affected> allAffected = new();

    readonly List<PersistentEffectHolder> persistentEffects = new();

    public bool hurtByCollisions = false;
    public bool DontDestroyOnDeath = false;
    public bool isPlayer;

    public event Action<float, string> OnTakeDamage;
    public event Action OnDie;

    [SerializeField]
    List<Transform> auraPositions = new();

    public GameObject instantiateOnDestroy;

    class PersistentEffectHolder
    {
        public PersistentEffect persistentEffect;
        public float duration;
        public List<MiniObject> auraObjects;

        public void Destroy()
        {
            foreach (MiniObject go in auraObjects)
            {
                go.DestroyThis();
            }
            auraObjects.Clear();
        }

        
    }

    Rigidbody thisRb;
    public Rigidbody Rigidbody => thisRb;

    [SerializeField]
    float health = 10;
    float maxHealth;


    private void Awake()
    {
        thisRb = GetComponent<Rigidbody>();
        allAffected[thisRb] = this;
        maxHealth = health;
    }

    private void OnDestroy()
    {
        allAffected.Remove(thisRb);
    }

    public static Affected GetAffected(Rigidbody rb)
    {
        if (allAffected.ContainsKey(rb))
            return allAffected[rb];
        return null;
    }


    public void AddPersistentEffect(PersistentEffect persistentEffect,
        Func<Transform, MiniObject> createAuras)
    {
        List<MiniObject> auras =
            auraPositions.Select(transform => createAuras(transform))
            .Where(obj => obj != null)
            .ToList();

        persistentEffects.Add(
            new()
            {
                persistentEffect = persistentEffect,
                duration = persistentEffect.Duration,
                auraObjects = auras
            });
        persistentEffect.OnApply?.Invoke(this);
    }

    void RemovePersistentEffect(PersistentEffectHolder holder)
    {
        persistentEffects.Remove(holder);
        holder.Destroy();
        holder.persistentEffect.OnRemove?.Invoke(this);
    }

    private void Update()
    {
        foreach (var holder in persistentEffects.ToArray())
        {
            var effect = holder.persistentEffect;
            var duration = holder.duration;
            duration -= Time.deltaTime;
            if (duration < 0 || 
                (effect.ShouldRemoveEarly != null && effect.ShouldRemoveEarly(this)))
            {
                RemovePersistentEffect(holder);
                continue;
            }
            effect.OnUpdate(this);
            holder.duration = duration;
        }


    }

    private void FixedUpdate()
    {
        if (transform.position.y < Floor.FloorHeight)
        {
            if (Rigidbody.linearVelocity.y < 0)
                Rigidbody.linearVelocity = new Vector3(Rigidbody.linearVelocity.x, 0f, Rigidbody.linearVelocity.z);
            transform.position = new Vector3(transform.position.x, Floor.FloorHeight, transform.position.z);
        }
    }

    const float DAMAGE_MAGNITUDE = 4;
    private void OnCollisionEnter(Collision collision)
    {
        float magnitude = collision.impulse.magnitude;
        if (magnitude > DAMAGE_MAGNITUDE)
        {
            if (hurtByCollisions)
            {
                TakeDamage(magnitude - DAMAGE_MAGNITUDE, "Physics");
            }
            if (Time.timeSinceLevelLoad > 1f)
            {
                AudioManager.Instance.PlaySound(
                    Mathf.Max(.5f ,(magnitude - DAMAGE_MAGNITUDE) / 20f),
    transform.position, AudioManager.Sound.Thud);
            }

        }
    }

    public void TakeDamage(float amount, string id)
    {
        OnTakeDamage?.Invoke(amount, id);
        health -= amount;
        if (isPlayer && amount > 1)
        {
            AudioManager.Instance.PlaySound(.5f, transform.position, AudioManager.Sound.Hurt);
        }

        if (health <= 0)
        {
            OnDie?.Invoke();
            if (!DontDestroyOnDeath && instantiateOnDestroy == null)
            {
                instantiateOnDestroy = Resources.Load("Destroy") as GameObject;
            }
            if (instantiateOnDestroy != null)
            {
                foreach (var pos in auraPositions)
                {
                    var obj = Instantiate(instantiateOnDestroy, pos.position, pos.rotation);
                    obj.transform.localScale = pos.localScale;

                }
            }

            if (!DontDestroyOnDeath)
            {
                Destroy(gameObject);
            }
        }
    }

    public void Heal(float amount)
    {
        health += amount;
        if (health > maxHealth)
        {
            health = maxHealth;
        }
    }

}
