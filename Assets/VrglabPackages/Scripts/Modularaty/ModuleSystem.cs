using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class CoreModifier
{
    private Dictionary<string, object> stored_value = new Dictionary<string, object>();

    protected object this[string id] { 
        get { return stored_value[id]; } 
        set { 
            if (stored_value == null)
            {
                stored_value = new Dictionary<string, object>();
            }
            stored_value[id] = value; 
        } 
    }
}


public class Components : CoreModifier
{
    public new object this[string index]
    {
        get { return base[index]; }
        set { base[index] = value; }
    }
}

public class Functions
{
    private Dictionary<string, Delegate> storedFunctions = new Dictionary<string, Delegate>();

    public void Bind<T>(string id, Action<T> function)
    {
        storedFunctions[id] = function;
    }

    public void Bind(string id, Action function)
    {
        storedFunctions[id] = function;
    }

    public void Bind<T, TResult>(string id, Func<T, TResult> function)
    {
        storedFunctions[id] = function;
    }

    public void Bind<TResult>(string id, Func<TResult> function)
    {
        storedFunctions[id] = function;
    }

    public void Call(string id)
    {
        if (storedFunctions.TryGetValue(id, out var function) && function is Action action)
        {
            action();
        }
        else
        {
            throw new InvalidOperationException($"Function with ID '{id}' is either not found or has parameters.");
        }
    }

    public void Call<T>(string id, T param)
    {
        if (storedFunctions.TryGetValue(id, out var function) && function is Action<T> action)
        {
            action(param);
        }
        else
        {
            throw new InvalidOperationException($"Function with ID '{id}' is either not found or has incorrect parameter type.");
        }
    }

    public TResult Call<T, TResult>(string id, T param)
    {
        if (storedFunctions.TryGetValue(id, out var function) && function is Func<T, TResult> func)
        {
            return func(param);
        }
        throw new InvalidOperationException($"Function with ID '{id}' is either not found or has incorrect parameter type.");
    }

    public TResult Call<TResult>(string id)
    {
        if (storedFunctions.TryGetValue(id, out var function) && function is Func<TResult> func)
        {
            return func();
        }
        throw new InvalidOperationException($"Function with ID '{id}' is either not found or has incorrect return type.");
    }
}

public abstract class ModuleSystem
{
    public abstract DataHolder UpdateModule(DataHolder dataHolders, Components components, Functions functions);
    public abstract DataHolder UpdatePhysicsModule(DataHolder dataHolders, Components components, Functions functions);
    public abstract DataHolder InitModule(DataHolder dataHolders, Components components, Functions functions);
}
