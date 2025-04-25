using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingController : PersistantSingleton<LoadingController>
{
    public Fader Loading;

    /// <summary>
    /// Triggers the animation to start
    /// </summary>
    /// <b>Authors</b>
    /// <br>Arad Bozorgmehr (Vrglab)</br>
    public void Begin()
    {
        Loading.FadeIn();
    }

    /// <summary>
    /// Load's all the given scene's in the order of their position in the function param
    /// </summary>
    /// <param name="loadScene">The given scene's</param>
    /// <b>Authors</b>
    /// <br>Arad Bozorgmehr (Vrglab)</br>
    public void LoadTheScene(params string[] loadScene)
    {
        StartCoroutine(loadTheScene(loadScene));
    }

    /// <summary>
    /// This function does basically the exact same thing as <see cref="LoadTheScene(string[])"/> says
    /// </summary>
    /// <remarks>Helper function</remarks>
    /// <b>Authors</b>
    /// <br>Arad Bozorgmehr (Vrglab)</br>
    private IEnumerator loadTheScene(params string[] loadScene)
    {
        Begin();
        foreach (var item in loadScene)
        {
            yield return new WaitForSeconds(Loading.FadeSpeed() + 0.2f);
            AsyncOperation ao = SceneManager.LoadSceneAsync(item);
            while (!ao.isDone)
            {
                yield return null;
            }
        }
        Stop();
    }

    /// <summary>
    /// Triggers the animation to stop
    /// </summary>
    /// <b>Authors</b>
    /// <br>Arad Bozorgmehr (Vrglab)</br>
    public void Stop()
    {
        Loading.FadeOut();
    }
}
