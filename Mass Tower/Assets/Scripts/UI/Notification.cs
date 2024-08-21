using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Analytics;
using Unity.Services.Core;
using Unity.Services.PushNotifications;
using System.Threading.Tasks;

using System;
using TMPro;
using UnityEngine.Playables;

public class Notification : MonoBehaviour
{
    public TextMeshProUGUI deviceTokenText;
    public TextMeshProUGUI debugText;

    private bool userGiveCinsent = false;

    public async Task Start()
    {
        if (userGiveCinsent)
        {
            AnalyticsService.Instance.StartDataCollection();
        }

        try
        {
            string pushToken = await PushNotificationsService.Instance.RegisterForPushNotificationsAsync();
            PushNotificationsService.Instance.OnRemoteNotificationReceived += notifiactionData =>
            {
                deviceTokenText.text = pushToken;
                Debug.Log("Received a notification");
                debugText.text = "Received a notification";
            };
        }
        catch (Exception e)
        {
            Debug.Log("Failed to retrieve a push notification token");
        }
    }

    public async Task initializedNotificationAsync()
    {
        await UnityServices.InitializeAsync();
    }

    async void Awake()
    {
        await initializedNotificationAsync();
    }
}
