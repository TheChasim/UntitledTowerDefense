using UnityEngine;


[CreateAssetMenu(fileName = "element Effect", menuName = "Element Effect")]
public class ElementEffect : ScriptableObject
{
    public ParticleSystem weatEffect;
    public ParticleSystem burn;
    [SerializeField]public float burnDamage = 0.5f;
    public ParticleSystem poison;
    [SerializeField] public float poisonDamage = 0.5f;
    public ParticleSystem electrocute;
    [SerializeField] public float slow = 0.5f;

    public float minDuration = 1f;
    public float maxDuration = 5f;

    public ParticleSystem GetEffect(Element element)
    {
        switch (element)
        {
            case Element.Fire:
                return burn;
            case Element.Electric: 
                return electrocute;
            case Element.Water:
                return weatEffect;
            default: 
                return null;           
        }
    }

    public float GetDuration() 
    { 
        return Random.Range(minDuration, maxDuration);
    }
}
