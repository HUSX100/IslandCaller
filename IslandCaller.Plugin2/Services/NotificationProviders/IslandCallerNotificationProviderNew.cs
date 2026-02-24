using ClassIsland.Core.Abstractions.Services;
using ClassIsland.Core.Abstractions.Services.NotificationProviders;
using ClassIsland.Core.Attributes;
using ClassIsland.Core.Models.Notification;
using ClassIsland.Shared.Enums;
using ClassIsland.Shared.Interfaces;
using Microsoft.Extensions.Hosting;
using System.Runtime.InteropServices;

using IslandCaller.Models;

namespace IslandCaller.Services.NotificationProvidersNew;

[NotificationProviderInfo(
    "9B570BF1-9A32-40C0-9D5D-4FFA69E03A37",
    "IslandCallerServices",
    PackIconKind.AccountCheck,
    "用于为IslandCaller提供通知接口")]
public class IslandCallerNotificationProviderNew : NotificationProviderBase
{
    public async void RandomCall(int stunum)
    {
        if (Settings.Instance.General.BreakDisable & Status.Instance.lessonstatu == TimeState.Breaking) return;
        string output = "Marshal.PtrToStringBSTR(ptr1)";
        int maskduration = stunum * 2 + 1; // 计算持续时间
        ShowNotification(new NotificationRequest()
        {
            MaskContent = new NotificationContent()
            {
                Content = output,
                Duration = new TimeSpan(0, 0, maskduration),
                IsSpeechEnabled = true,
                SpeechContent = output,
            }
        });
    }
}
