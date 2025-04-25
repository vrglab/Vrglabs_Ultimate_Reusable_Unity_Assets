using UnityEngine;

public class PhoneUiController : Instancable<PhoneUiController>
{

    [SerializeField]
    private GameObject _Canvas;


    public void OnUiStateChanged()
    {
#if UNITY_ANDROID
        if(LevelManager.Instance?.CurrentLevel.Level.PhoneUIEnabled == true)
        {
            _Canvas.SetActive(true);
        }else
        {
            _Canvas.SetActive(false);
        }
#else
        _Canvas.SetActive(false);
#endif
    }
}
