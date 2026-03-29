using System.Collections.Generic;

public enum Element
{
    Fire,
    Water,
    Electric,
    Physic,
    Psy,
    force,
    Wind,
    normal,
}

public enum EnemyStatus
{
    None,
    Burn,
    poisoned,
    stun,
    wet,
    electrify,
}

public static class ElementSensitivity
{
    //const float weakness = 2f;
    //const float resistance = 0.5f;
    // Structure des relations
    private static readonly Dictionary<Element, ElementRelations> chart
        = new Dictionary<Element, ElementRelations>()
        {
            //fire
            {
                Element.Fire,
                new ElementRelations(
                    weaknesses: new List<Element>{Element.Water},
                    resistances: new List<Element>(){Element.Wind},
                    immunities: null
                    )
            },
            //Water,
            {
                Element.Water,
                new ElementRelations(
                    weaknesses: new List<Element>(){Element.Electric},
                    resistances: new List<Element>(){Element.Fire },
                    immunities: null
                    )
            },
            //Electric,
            {
                Element.Electric,
                new ElementRelations(
                    weaknesses:  new List < Element >(),
                    resistances: new List < Element >() { Element.Water },
                    immunities: null
                    )
            },
            //Physic,
            {
                Element.Physic,
                new ElementRelations(
                    weaknesses: new List<Element>(){Element.Psy},
                    resistances: new List<Element>(){Element.force},
                    immunities : null
                    )
            },
            //Psy,
            {
                Element.Psy,
                new ElementRelations(
                    weaknesses: new List<Element>(){Element.Physic},
                    resistances: new List<Element>(){Element.force},
                    immunities : null)
            },
            //force,
            {
                Element.force,
                new ElementRelations(
                    weaknesses: new List<Element>(){Element.Wind},
                    resistances: new List<Element>(){},
                    immunities: null)
            },
            //Wind,
            {
                Element.Wind,
                new ElementRelations(
                    weaknesses : new List<Element>{Element.force},
                    resistances: new List<Element>{Element.Fire},
                    immunities : null)
            },
            //normal
            {
                Element.normal,
                new ElementRelations(
                    weaknesses: new List < Element >(),
                    resistances: new List < Element >(),
                    immunities : null)
            },

        };

    public static float GetMultiplierAttack(Element attack, Element defense)
    {
        var r = chart[defense];

        if (r.immunities != null && r.immunities.Contains(attack))
            return 0f;

        if (r.weaknesses.Contains(attack))
            return 1.15f;

        if (r.resistances.Contains(attack))
            return 0.5f;

        return 1f;
    }

    public static float GetMultiplierCrit(Element attack, Element defense)
    {
        var r = chart[defense];

        if (r.immunities != null && r.immunities.Contains(attack))
            return 0f;

        if (r.weaknesses.Contains(attack))
            return 1.05f;

        if (r.resistances.Contains(attack))
            return 0.5f;

        return 1f;
    }


    // Classe relation
    private class ElementRelations
    {
        public List<Element> weaknesses;
        public List<Element> resistances;
        public List<Element> immunities;

        public ElementRelations(
            List<Element> weaknesses,
            List<Element> resistances,
            List<Element> immunities = null)
        {
            this.weaknesses = weaknesses;
            this.resistances = resistances;
            this.immunities = immunities;
        }
    }
}
