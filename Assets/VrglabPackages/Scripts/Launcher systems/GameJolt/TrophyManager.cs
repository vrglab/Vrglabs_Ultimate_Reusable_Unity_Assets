using GameJolt.API.Objects;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TrophyManager : Instancable<TrophyManager>
{

    /// <summary>
    /// Unlocks a GameJolt Trophy
    /// </summary>
    /// <param name="TrophyId"></param>
    /// <b>Authors</b>
    /// <br>Arad Bozorgmehr (Vrglab)</br>
    public void Unlock(Achievment Trophy)
    {
        GameJolt.API.Trophies.Unlock(Trophy.gameJolt_id);
    }

    /// <summary>
    /// Get's all of the Trophies within a game
    /// </summary>
    /// <returns>A array of all existing trophies</returns>
    /// <b>Authors</b>
    /// <br>Arad Bozorgmehr (Vrglab)</br>
    public Trophy[] GetAllTrophies()
    {
        Trophy[] tr = null;
        GameJolt.API.Trophies.Get((GameJolt.API.Objects.Trophy[] trophies) => {
            if (trophies != null)
            {
                tr = trophies;
            }
        });

        return tr;
    }
}
