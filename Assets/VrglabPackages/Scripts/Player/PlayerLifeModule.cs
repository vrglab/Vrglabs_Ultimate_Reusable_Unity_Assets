using UnityEngine;

public class PlayerLifeModule : ModuleSystem
{

    public class StaticModifiers : CoreModifier
    {
        public int maxHealth { get { return (int)this["maxHealth"]; } private set { this["maxHealth"] = value; } }

        public StaticModifiers(int maxHealth)
        {
            this.maxHealth = maxHealth;
        }
    }

    public class DynamicModifiers : CoreModifier
    {
        public int activeHealth { get { return (int)this["active_health"]; } set { this["active_health"] = value; } }
    }

    private StaticModifiers _staticModifiers;
    private DynamicModifiers _dynamicModifiers;

    public PlayerLifeModule(StaticModifiers staticModifiers)
    {
        _staticModifiers = staticModifiers;
        _dynamicModifiers = new DynamicModifiers();
    }

    public override DataHolder InitModule(DataHolder dataHolders, Components components, Functions functions)
    {
        functions.Bind("TakeDamage", (int amount) =>
        {

        });

        functions.Bind("HealDamage", (int amount) =>
        {

        });

        functions.Bind("ActivateDeath", () =>
        {
            _dynamicModifiers.activeHealth = 0;
        });

        _dynamicModifiers.activeHealth = _staticModifiers.maxHealth;

        return dataHolders;
    }

    public override DataHolder UpdateModule(DataHolder dataHolders, Components components, Functions functions)
    {
        return dataHolders;
    }

    public override DataHolder UpdatePhysicsModule(DataHolder dataHolders, Components components, Functions functions)
    {
        return dataHolders;
    }
}
