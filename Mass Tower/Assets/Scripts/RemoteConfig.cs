using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.Services.RemoteConfig;
using Unity.Services.Core;
using Unity.Services.Authentication;

using System.Threading.Tasks;
    

public class RemoteConfig : MonoBehaviour
{
    public static RemoteConfig Instance { get; private set; }
    public string environmentId;


    public struct userAttributes
    {
        public float enemyMoveSpeed;

    }
    public struct appAttributes { }

    public async Task InitializeRemoteConfigAsync()
    {
        await UnityServices.InitializeAsync();
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
        RemoteConfigService.Instance.SetEnvironmentID(environmentId);
    }

    async void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        if (Utilities.CheckForInternetConnection())
        {
            await InitializeRemoteConfigAsync();
        }
    }

    // Start is called before the first frame update
   IEnumerator Start()
    {
        while (true)
        {
            yield return new WaitForSeconds(3);
            RemoteConfigService.Instance.FetchConfigs<userAttributes, appAttributes>(new userAttributes(),new appAttributes());

            
        }
    }
   public float GetEnemyMoveSpeed()
   {
       return RemoteConfigService.Instance.appConfig.GetFloat("enemy_move_speed");
   }
   
}
