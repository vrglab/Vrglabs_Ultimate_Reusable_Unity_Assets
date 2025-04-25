using System;
using UnityEngine;
#if UWP || MICROSOFT_GAME_CORE
using Unity.XGamingRuntime;
#endif


public class XUser
{
#if UWP || MICROSOFT_GAME_CORE
    private string gamertag;
    private ulong id;
    private XUserState state;
    private XUserHandle handle;
    private Sprite image;

    public XUser(string gamertag, ulong id, XUserState state, XUserHandle handle, Sprite image)
    {
        this.gamertag = gamertag;
        this.id = id;
        this.state = state;
        this.handle = handle;
        this.image = image;
    }

    public void SignOut()
    {
        SDK.XUserCloseHandle(handle);
    }
#endif
}

public class GDK : Singleton<GDK>
{
#if UWP || MICROSOFT_GAME_CORE


    private int xgrnStatus = -1, xblStatus = 0, loginStatus = -1;

    public bool IsInitialized { get { return ((xgrnStatus >= 0) && (xblStatus >= 0)); } }
    public bool IsLoggedIn { get { return loginStatus >= 0; } }

    public XUser ActiveUser { get; private set; }


    Sprite ByteArrayToSprite(byte[] imageData)
    {
        // Create a Texture2D
        Texture2D texture = new Texture2D(2, 2); // Start with a small size; it will resize automatically

        // Load the image data into the texture
        if (texture.LoadImage(imageData))
        {
            // Successfully loaded the texture, now create a Sprite
            return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        }
        else
        {
            Debug.LogError("Failed to load image data into texture.");
            return null;
        }
    }



    public string XGetLocale()
    {
        string outPut = "en";
        try
        {
            if(IsInitialized)
            {
                SDK.XPackageGetUserLocale(out outPut);
            }
        }
        catch (Exception e)
        {
            outPut = "en";
        }
        
        return (outPut == null ? "en" : outPut);
    }

    public void LoginUser()
    {
        if(IsInitialized)
        {
            SDK.XUserAddAsync(XUserAddOptions.None, (int hr, XUserHandle userHandle) =>
            {
                if (hr >= 0 && userHandle.Handle != IntPtr.Zero)
                {
                    SDK.XUserGetGamertag(userHandle, XUserGamertagComponent.Classic, out string tag);
                    SDK.XUserGetId(userHandle, out ulong id);
                    SDK.XUserGetState(userHandle, out XUserState state);
                    SDK.XUserGetGamerPictureAsync(userHandle, XUserGamerPictureSize.Large, (int hresult, byte[] buffer) =>
                    {
                        if (hresult >= 0 && buffer != null && buffer.Length > 0)
                        {
                            Sprite userSprite = ByteArrayToSprite(buffer);

                            ActiveUser = new XUser(tag, id, state, userHandle, userSprite);
                            loginStatus = hr;
                        }

                    });
                }
            });
        }
    }

    public void LogoutUser()
    {
        if(IsLoggedIn)
        {
            ActiveUser.SignOut();
        }
    }


    public void Awake()
    {
        base.Awake();
        xgrnStatus = SDK.XGameRuntimeInitialize();
        //xblStatus = SDK.XBL.XblInitialize(GdkPlatformSettings.gameConfigScid);
    }

    public void OnDestroy()
    {
        LogoutUser();
        SDK.XGameRuntimeUninitialize();
    }
#endif
}
