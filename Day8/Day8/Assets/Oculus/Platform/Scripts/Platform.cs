// This file was @generated with LibOVRPlatform/codegen/main. Do not modify it!

using System.Runtime.CompilerServices;
[assembly: InternalsVisibleTo("Assembly-CSharp-Editor")]

namespace Oculus.Platform
{
  using UnityEngine;
  using System;
  using System.Collections;
  using System.Collections.Generic;
  using System.Runtime.InteropServices;

  public sealed class Core {
    private static bool IsPlatformInitialized = false;
    public static bool IsInitialized()
    {
      return IsPlatformInitialized;
    }

    // If LogMessages is true, then the contents of each request response
    // will be printed using Debug.Log. This allocates a lot of heap memory,
    // and so should not be called outside of testing and debugging.
    public static bool LogMessages = false;

    internal static void ForceInitialized()
    {
      IsPlatformInitialized = true;
    }

    private static string getAppID(string appId = null) {
      string configAppID = GetAppIDFromConfig();
      if (String.IsNullOrEmpty(appId))
      {
        if (String.IsNullOrEmpty(configAppID))
        {
          throw new UnityException("Update your app id by selecting 'Oculus Platform' -> 'Edit Settings'");
        }
        appId = configAppID;
      }
      else
      {
        if (!String.IsNullOrEmpty(configAppID))
        {
          Debug.LogWarningFormat("The 'Oculus App Id ({0})' field in 'Oculus Platform/Edit Settings' is clobbering appId ({1}) that you passed in to Platform.Core.Init.  You should only specify this in one place.  We recommend the menu location.", configAppID, appId);
        }
      }
      return appId;
    }

    // Asynchronously Initialize Platform SDK. The result will be put on the message
    // queue with the message type: ovrMessage_PlatformInitializeAndroidAsynchronous
    //
    // While the platform is in an initializing state, it's not fully functional.
    // [Requests]: will queue up and run once platform is initialized.
    //    For example: ovr_User_GetLoggedInUser() can be called immediately after
    //    asynchronous init and once platform is initialized, this request will run
    // [Synchronous Methods]: will return the default value;
    //    For example: ovr_GetLoggedInUserID() will return 0 until platform is
    //    fully initialized
    public static Request<Models.PlatformInitialize> AsyncInitialize(string appId = null) {
      appId = getAppID(appId);

      Request<Models.PlatformInitialize> request;
      if (UnityEngine.Application.isEditor && PlatformSettings.UseStandalonePlatform) {
        var platform = new StandalonePlatform();
        request = platform.InitializeInEditor();
      }
      else if (UnityEngine.Application.platform == RuntimePlatform.WindowsEditor ||
               UnityEngine.Application.platform == RuntimePlatform.WindowsPlayer) {
        var platform = new WindowsPlatform();
        request = platform.AsyncInitialize(appId);
      }
      else if (UnityEngine.Application.platform == RuntimePlatform.Android) {
        var platform = new AndroidPlatform();
        request = platform.AsyncInitialize(appId);
      }
      else {
        throw new NotImplementedException("Oculus platform is not implemented on this platform yet.");
      }

      IsPlatformInitialized = (request != null);

      if (!IsPlatformInitialized)
      {
        throw new UnityException("Oculus Platform failed to initialize.");
      }

      if (LogMessages) {
        Debug.LogWarning("Oculus.Platform.Core.LogMessages is set to true. This will cause extra heap allocations, and should not be used outside of testing and debugging.");
      }

      // Create the GameObject that will run the callbacks
      (new GameObject("Oculus.Platform.CallbackRunner")).AddComponent<CallbackRunner>();
      return request;
    }


    public static void Initialize(string appId = null)
    {
      appId = getAppID(appId);

      if (UnityEngine.Application.isEditor && PlatformSettings.UseStandalonePlatform) {
        var platform = new StandalonePlatform();
        IsPlatformInitialized = platform.InitializeInEditor() != null;
      }
      else if (UnityEngine.Application.platform == RuntimePlatform.WindowsEditor ||
               UnityEngine.Application.platform == RuntimePlatform.WindowsPlayer) {
        var platform = new WindowsPlatform();
        IsPlatformInitialized = platform.Initialize(appId);
      }
      else if (UnityEngine.Application.platform == RuntimePlatform.Android) {
        var platform = new AndroidPlatform();
        IsPlatformInitialized = platform.Initialize(appId);
      }
      else {
        throw new NotImplementedException("Oculus platform is not implemented on this platform yet.");
      }

      if (!IsPlatformInitialized)
      {
        throw new UnityException("Oculus Platform failed to initialize.");
      }

      if (LogMessages) {
        Debug.LogWarning("Oculus.Platform.Core.LogMessages is set to true. This will cause extra heap allocations, and should not be used outside of testing and debugging.");
      }

      // Create the GameObject that will run the callbacks
      (new GameObject("Oculus.Platform.CallbackRunner")).AddComponent<CallbackRunner>();
    }

    private static string GetAppIDFromConfig()
    {
      if (UnityEngine.Application.platform == RuntimePlatform.Android)
      {
        return PlatformSettings.MobileAppID;
      }
      else
      {
        return PlatformSettings.AppID;
      }
    }
  }

  public static class ApplicationLifecycle
  {
    public static Models.LaunchDetails GetLaunchDetails() {
      return new Models.LaunchDetails(CAPI.ovr_ApplicationLifecycle_GetLaunchDetails());
    }
  }

  public static partial class Rooms
  {

    public static Request<Models.Room> UpdateDataStore(UInt64 roomID, Dictionary<string, string> data)
    {
      if (Core.IsInitialized())
      {
        CAPI.ovrKeyValuePair[] kvps = new CAPI.ovrKeyValuePair[data.Count];
        int i=0;
        foreach(var item in data)
        {
          kvps[i++] = new CAPI.ovrKeyValuePair(item.Key, item.Value);
        }

        return new Request<Models.Room>(CAPI.ovr_Room_UpdateDataStore(roomID, kvps));
      }
      return null;
    }

    public static void SetUpdateNotificationCallback(Message<Models.Room>.Callback callback)
    {
        Callback.SetNotificationCallback(
          Message.MessageType.Notification_Room_RoomUpdate,
          callback
        );
    }

    [Obsolete("Deprecated in favor of SetRoomInviteAcceptedNotificationCallback")]
    public static void SetRoomInviteNotificationCallback(Message<string>.Callback callback)
    {
        Callback.SetNotificationCallback(
          Message.MessageType.Notification_Room_InviteAccepted,
          callback
        );
    }

    // Be notified when someone you've invited has accepted your invitation.
    public static void SetRoomInviteAcceptedNotificationCallback(Message<string>.Callback callback)
    {
      Callback.SetNotificationCallback(
        Message.MessageType.Notification_Room_InviteAccepted,
        callback
      );
    }

    // Be notified when you've received an invitation to a room from another player.
    // You can also poll for room invites using Notifications.GetRoomInviteNotifications.
    public static void SetRoomInviteReceivedNotificationCallback(Message<Models.RoomInviteNotification>.Callback callback)
    {
      Callback.SetNotificationCallback(
        Message.MessageType.Notification_Room_InviteReceived,
        callback
      );
    }

  }

  public static partial class Livestreaming
  {
    public static void SetStatusUpdateNotificationCallback(Message<Models.LivestreamingStatus>.Callback callback)
    {
      Callback.SetNotificationCallback(
        Message.MessageType.Notification_Livestreaming_StatusChange,
        callback
      );
    }
  }


  public static partial class Matchmaking
  {
    public class CustomQuery
    {
      public Dictionary<string, object> data;
      public Criterion[] criteria;

      public struct Criterion
      {
        public Criterion(string key_, MatchmakingCriterionImportance importance_)
        {
          key = key_;
          importance = importance_;

          parameters = null;
        }

        public string key;
        public MatchmakingCriterionImportance importance;
        public Dictionary<string, object> parameters;
      }

      public IntPtr ToUnmanaged()
      {
        var customQueryUnmanaged = new CAPI.ovrMatchmakingCustomQueryData();

        if(criteria != null && criteria.Length > 0)
        {
          customQueryUnmanaged.criterionArrayCount = (uint)criteria.Length;
          var temp = new CAPI.ovrMatchmakingCriterion[criteria.Length];

          for(int i=0; i<criteria.Length; i++)
          {
            temp[i].importance_ = criteria[i].importance;
            temp[i].key_ = criteria[i].key;

            if(criteria[i].parameters != null && criteria[i].parameters.Count > 0)
            {
              temp[i].parameterArrayCount = (uint)criteria[i].parameters.Count;
              temp[i].parameterArray = CAPI.ArrayOfStructsToIntPtr(CAPI.DictionaryToOVRKeyValuePairs(criteria[i].parameters));
            }
            else
            {
              temp[i].parameterArrayCount = 0;
              temp[i].parameterArray = IntPtr.Zero;
            }
          }

          customQueryUnmanaged.criterionArray = CAPI.ArrayOfStructsToIntPtr(temp);
        }
        else
        {
          customQueryUnmanaged.criterionArrayCount = 0;
          customQueryUnmanaged.criterionArray = IntPtr.Zero;
        }


        if(data != null && data.Count > 0)
        {
          customQueryUnmanaged.dataArrayCount = (uint)data.Count;
          customQueryUnmanaged.dataArray = CAPI.ArrayOfStructsToIntPtr(CAPI.DictionaryToOVRKeyValuePairs(data));
        }
        else
        {
          customQueryUnmanaged.dataArrayCount = 0;
          customQueryUnmanaged.dataArray = IntPtr.Zero;
        }

        IntPtr res = Marshal.AllocHGlobal(Marshal.SizeOf(customQueryUnmanaged));
        Marshal.StructureToPtr(customQueryUnmanaged, res, true);
        return res;
      }
    }

    public static Request ReportResultsInsecure(UInt64 roomID, Dictionary<string, int> data)
    {
      if(Core.IsInitialized())
      {
        CAPI.ovrKeyValuePair[] kvps = new CAPI.ovrKeyValuePair[data.Count];
        int i=0;
        foreach(var item in data)
        {
          kvps[i++] = new CAPI.ovrKeyValuePair(item.Key, item.Value);
        }

        return new Request(CAPI.ovr_Matchmaking_ReportResultInsecure(roomID, kvps));
      }

      return null;
    }

    public static void SetMatchFoundNotificationCallback(Message<Models.Room>.Callback callback)
    {
      Callback.SetNotificationCallback(
        Message.MessageType.Notification_Matchmaking_MatchFound,
        callback
      );
    }

    public static Request<Models.MatchmakingStats> GetStats(string pool, uint maxLevel, MatchmakingStatApproach approach = MatchmakingStatApproach.Trailing)
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.MatchmakingStats>(CAPI.ovr_Matchmaking_GetStats(pool, maxLevel, approach));
      }

      return null;
    }
  }

  public static class Net
  {
    public static Packet ReadPacket()
    {
      if (!Core.IsInitialized())
      {
        return null;
      }

      var packetHandle = CAPI.ovr_Net_ReadPacket();

      if(packetHandle == IntPtr.Zero)
      {
        return null;
      }

      return new Packet(packetHandle);
    }

    public static bool SendPacket(UInt64 userID, byte[] bytes, SendPolicy policy)
    {
      if(Core.IsInitialized())
      {
        return CAPI.ovr_Net_SendPacket(userID, (UIntPtr)bytes.Length, bytes, policy);
      }

      return false;
    }

    public static void Connect(UInt64 userID)
    {
      if (Core.IsInitialized())
      {
        CAPI.ovr_Net_Connect(userID);
      }
    }

    public static void Accept(UInt64 userID)
    {
      if(Core.IsInitialized())
      {
        CAPI.ovr_Net_Accept(userID);
      }
    }

    public static void Close(UInt64 userID)
    {
      if(Core.IsInitialized())
      {
        CAPI.ovr_Net_Close(userID);
      }
    }

    public static bool IsConnected(UInt64 userID)
    {
      return Core.IsInitialized() && CAPI.ovr_Net_IsConnected(userID);
    }

    public static bool SendPacketToCurrentRoom(byte[] bytes, SendPolicy policy)
    {
      if (Core.IsInitialized())
      {
        return CAPI.ovr_Net_SendPacketToCurrentRoom((UIntPtr)bytes.Length, bytes, policy);
      }

      return false;
    }

    public static bool AcceptForCurrentRoom()
    {
      if (Core.IsInitialized())
      {
        return CAPI.ovr_Net_AcceptForCurrentRoom();
      }

      return false;
    }

    public static void CloseForCurrentRoom()
    {
      if (Core.IsInitialized())
      {
        CAPI.ovr_Net_CloseForCurrentRoom();
      }
    }

    public static Request<Models.PingResult> Ping(UInt64 userID)
    {
      if(Core.IsInitialized())
      {
        return new Request<Models.PingResult>(CAPI.ovr_Net_Ping(userID));
      }

      return null;
    }

    public static void SetPeerConnectRequestCallback(Message<Models.NetworkingPeer>.Callback callback)
    {
      Callback.SetNotificationCallback(
        Message.MessageType.Notification_Networking_PeerConnectRequest,
        callback
      );
    }

    public static void SetConnectionStateChangedCallback(Message<Models.NetworkingPeer>.Callback callback)
    {
      Callback.SetNotificationCallback(
        Message.MessageType.Notification_Networking_ConnectionStateChange,
        callback
      );
    }
  }

  public static partial class Leaderboards
  {
    public static Request<Models.LeaderboardEntryList> GetNextEntries(Models.LeaderboardEntryList list)
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.LeaderboardEntryList>(CAPI.ovr_HTTP_GetWithMessageType(list.NextUrl, (int)Message.MessageType.Leaderboard_GetNextEntries));
      }

      return null;
    }

    public static Request<Models.LeaderboardEntryList> GetPreviousEntries(Models.LeaderboardEntryList list)
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.LeaderboardEntryList>(CAPI.ovr_HTTP_GetWithMessageType(list.PreviousUrl, (int)Message.MessageType.Leaderboard_GetPreviousEntries));
      }

      return null;
    }
  }

  public static partial class Voip
  {
    public static void Start(UInt64 userID)
    {
      if (Core.IsInitialized())
      {
        CAPI.ovr_Voip_Start(userID);
      }
    }

    public static void Accept(UInt64 userID)
    {
      if (Core.IsInitialized())
      {
        CAPI.ovr_Voip_Accept(userID);
      }
    }

    public static void Stop(UInt64 userID)
    {
      if (Core.IsInitialized())
      {
        CAPI.ovr_Voip_Stop(userID);
      }
    }

    public static void SetVoipConnectRequestCallback(Message<Models.NetworkingPeer>.Callback callback)
    {
      if (Core.IsInitialized())
      {
        Callback.SetNotificationCallback(
          Message.MessageType.Notification_Voip_ConnectRequest,
          callback
        );
      }
    }

    public static void SetVoipStateChangeCallback(Message<Models.NetworkingPeer>.Callback callback)
    {
      if (Core.IsInitialized())
      {
        Callback.SetNotificationCallback(
          Message.MessageType.Notification_Voip_StateChange,
          callback
        );
      }
    }

    public static void SetMicrophoneFilterCallback(CAPI.FilterCallback callback)
    {
      if (Core.IsInitialized())
      {
        CAPI.ovr_Voip_SetMicrophoneFilterCallbackWithFixedSizeBuffer(callback, (UIntPtr)CAPI.VoipFilterBufferSize);
      }
    }

    public static void SetMicrophoneMuted(VoipMuteState state)
    {
      if (Core.IsInitialized())
      {
        CAPI.ovr_Voip_SetMicrophoneMuted(state);
      }
    }

    public static VoipMuteState GetSystemVoipMicrophoneMuted()
    {
      if (Core.IsInitialized())
      {
        return CAPI.ovr_Voip_GetSystemVoipMicrophoneMuted();
      }
      return VoipMuteState.Unknown;
    }

    public static SystemVoipStatus GetSystemVoipStatus()
    {
      if (Core.IsInitialized())
      {
        return CAPI.ovr_Voip_GetSystemVoipStatus();
      }
      return SystemVoipStatus.Unknown;
    }

    public static Oculus.Platform.VoipDtxState GetIsConnectionUsingDtx(UInt64 peerID)
    {
      if (Core.IsInitialized())
      {
        return CAPI.ovr_Voip_GetIsConnectionUsingDtx(peerID);
      }
      return Oculus.Platform.VoipDtxState.Unknown;
    }

    public static Oculus.Platform.VoipBitrate GetLocalBitrate(UInt64 peerID)
    {
      if (Core.IsInitialized())
      {
        return CAPI.ovr_Voip_GetLocalBitrate(peerID);
      }
      return Oculus.Platform.VoipBitrate.Unknown;
    }

    public static Oculus.Platform.VoipBitrate GetRemoteBitrate(UInt64 peerID)
    {
      if (Core.IsInitialized())
      {
        return CAPI.ovr_Voip_GetRemoteBitrate(peerID);
      }
      return Oculus.Platform.VoipBitrate.Unknown;
    }

    public static void SetNewConnectionOptions(VoipOptions voipOptions)
    {
      if (Core.IsInitialized())
      {
        CAPI.ovr_Voip_SetNewConnectionOptions((IntPtr)voipOptions);
      }
    }

    public static void SetSystemVoipStateNotificationCallback(Message<Models.SystemVoipState>.Callback callback)
    {
      if (Core.IsInitialized())
      {
        Callback.SetNotificationCallback(
          Message.MessageType.Notification_Voip_SystemVoipState,
          callback
        );
      }
    }
  }

  public static partial class Achievements
  {
    /// Add 'count' to the achievement with the given name. This must be a COUNT
    /// achievement. The largest number that is supported by this method is the max
    /// value of a signed 64-bit integer. If the number is larger than that, it is
    /// clamped to that max value before being passed to the servers.
    ///
    public static Request<Models.AchievementUpdate> AddCount(string name, ulong count)
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.AchievementUpdate>(CAPI.ovr_Achievements_AddCount(name, count));
      }

      return null;
    }

    /// Unlock fields of a BITFIELD achievement.
    /// \param name The name of the achievement to unlock
    /// \param fields A string containing either '0' or '1' characters. Every '1' will unlock the field in the corresponding position.
    ///
    public static Request<Models.AchievementUpdate> AddFields(string name, string fields)
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.AchievementUpdate>(CAPI.ovr_Achievements_AddFields(name, fields));
      }

      return null;
    }

    /// Request all achievement definitions for the app.
    ///
    public static Request<Models.AchievementDefinitionList> GetAllDefinitions()
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.AchievementDefinitionList>(CAPI.ovr_Achievements_GetAllDefinitions());
      }

      return null;
    }

    /// Request the progress for the user on all achievements in the app.
    ///
    public static Request<Models.AchievementProgressList> GetAllProgress()
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.AchievementProgressList>(CAPI.ovr_Achievements_GetAllProgress());
      }

      return null;
    }

    /// Request the achievement definitions that match the specified names.
    ///
    public static Request<Models.AchievementDefinitionList> GetDefinitionsByName(string[] names)
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.AchievementDefinitionList>(CAPI.ovr_Achievements_GetDefinitionsByName(names, (names != null ? names.Length : 0)));
      }

      return null;
    }

    /// Request the user's progress on the specified achievements.
    ///
    public static Request<Models.AchievementProgressList> GetProgressByName(string[] names)
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.AchievementProgressList>(CAPI.ovr_Achievements_GetProgressByName(names, (names != null ? names.Length : 0)));
      }

      return null;
    }

    /// Unlock the achievement with the given name. This can be of any achievement
    /// type.
    ///
    public static Request<Models.AchievementUpdate> Unlock(string name)
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.AchievementUpdate>(CAPI.ovr_Achievements_Unlock(name));
      }

      return null;
    }

  }

  public static partial class Application
  {
    /// Requests version information, including the currently installed and latest
    /// available version name and version code.
    ///
    public static Request<Models.ApplicationVersion> GetVersion()
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.ApplicationVersion>(CAPI.ovr_Application_GetVersion());
      }

      return null;
    }

    /// Launches a different application in the user's library. If the user does
    /// not have that application installed, they will be taken to that app's page
    /// in the Oculus Store
    /// \param appID The ID of the app to launch
    /// \param deeplink_options Additional configuration for this requests. Optional.
    ///
    public static Request<string> LaunchOtherApp(UInt64 appID, ApplicationOptions deeplink_options = null)
    {
      if (Core.IsInitialized())
      {
        return new Request<string>(CAPI.ovr_Application_LaunchOtherApp(appID, (IntPtr)deeplink_options));
      }

      return null;
    }

  }

  public static partial class AssetFile
  {
    /// DEPRECATED. Use AssetFile.DeleteById()
    ///
    public static Request<Models.AssetFileDeleteResult> Delete(UInt64 assetFileID)
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.AssetFileDeleteResult>(CAPI.ovr_AssetFile_Delete(assetFileID));
      }

      return null;
    }

    /// Removes an previously installed asset file from the device by its ID.
    /// Returns an object containing the asset ID and file name, and a success
    /// flag.
    /// \param assetFileID The asset file ID
    ///
    public static Request<Models.AssetFileDeleteResult> DeleteById(UInt64 assetFileID)
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.AssetFileDeleteResult>(CAPI.ovr_AssetFile_DeleteById(assetFileID));
      }

      return null;
    }

    /// Removes an previously installed asset file from the device by its name.
    /// Returns an object containing the asset ID and file name, and a success
    /// flag.
    /// \param assetFileName The asset file name
    ///
    public static Request<Models.AssetFileDeleteResult> DeleteByName(string assetFileName)
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.AssetFileDeleteResult>(CAPI.ovr_AssetFile_DeleteByName(assetFileName));
      }

      return null;
    }

    /// DEPRECATED. Use AssetFile.DownloadById()
    ///
    public static Request<Models.AssetFileDownloadResult> Download(UInt64 assetFileID)
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.AssetFileDownloadResult>(CAPI.ovr_AssetFile_Download(assetFileID));
      }

      return null;
    }

    /// Downloads an asset file by its ID on demand. Returns an object containing
    /// the asset ID and filepath. Sends periodic
    /// MessageType.Notification_AssetFile_DownloadUpdate to track the downloads.
    /// \param assetFileID The asset file ID
    ///
    public static Request<Models.AssetFileDownloadResult> DownloadById(UInt64 assetFileID)
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.AssetFileDownloadResult>(CAPI.ovr_AssetFile_DownloadById(assetFileID));
      }

      return null;
    }

    /// Downloads an asset file by its name on demand. Returns an object containing
    /// the asset ID and filepath. Sends periodic
    /// {notifications.asset_file.download_update}} to track the downloads.
    /// \param assetFileName The asset file name
    ///
    public static Request<Models.AssetFileDownloadResult> DownloadByName(string assetFileName)
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.AssetFileDownloadResult>(CAPI.ovr_AssetFile_DownloadByName(assetFileName));
      }

      return null;
    }

    /// DEPRECATED. Use AssetFile.DownloadCancelById()
    ///
    public static Request<Models.AssetFileDownloadCancelResult> DownloadCancel(UInt64 assetFileID)
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.AssetFileDownloadCancelResult>(CAPI.ovr_AssetFile_DownloadCancel(assetFileID));
      }

      return null;
    }

    /// Cancels a previously spawned download request for an asset file by its ID.
    /// Returns an object containing the asset ID and file path, and a success
    /// flag.
    /// \param assetFileID The asset file ID
    ///
    public static Request<Models.AssetFileDownloadCancelResult> DownloadCancelById(UInt64 assetFileID)
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.AssetFileDownloadCancelResult>(CAPI.ovr_AssetFile_DownloadCancelById(assetFileID));
      }

      return null;
    }

    /// Cancels a previously spawned download request for an asset file by its
    /// name. Returns an object containing the asset ID and file path, and a
    /// success flag.
    /// \param assetFileName The asset file name
    ///
    public static Request<Models.AssetFileDownloadCancelResult> DownloadCancelByName(string assetFileName)
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.AssetFileDownloadCancelResult>(CAPI.ovr_AssetFile_DownloadCancelByName(assetFileName));
      }

      return null;
    }

    /// Returns an array of objects with asset file names and their associated IDs,
    /// and and whether it's currently installed.
    ///
    public static Request<Models.AssetDetailsList> GetList()
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.AssetDetailsList>(CAPI.ovr_AssetFile_GetList());
      }

      return null;
    }

    /// DEPRECATED. Use AssetFile.StatusById()
    ///
    public static Request<Models.AssetDetails> Status(UInt64 assetFileID)
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.AssetDetails>(CAPI.ovr_AssetFile_Status(assetFileID));
      }

      return null;
    }

    /// Returns the details on a single asset: ID, file name, and whether it's
    /// currently installed
    /// \param assetFileID The asset file ID
    ///
    public static Request<Models.AssetDetails> StatusById(UInt64 assetFileID)
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.AssetDetails>(CAPI.ovr_AssetFile_StatusById(assetFileID));
      }

      return null;
    }

    /// Returns the details on a single asset: ID, file name, and whether it's
    /// currently installed
    /// \param assetFileName The asset file name
    ///
    public static Request<Models.AssetDetails> StatusByName(string assetFileName)
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.AssetDetails>(CAPI.ovr_AssetFile_StatusByName(assetFileName));
      }

      return null;
    }

  }

  public static partial class Avatar
  {
  }

  public static partial class CloudStorage
  {
    /// Deletes the specified save data buffer. Conflicts are handled just like
    /// Saves.
    /// \param bucket The name of the storage bucket.
    /// \param key The name for this saved data.
    ///
    public static Request<Models.CloudStorageUpdateResponse> Delete(string bucket, string key)
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.CloudStorageUpdateResponse>(CAPI.ovr_CloudStorage_Delete(bucket, key));
      }

      return null;
    }

    /// Loads the saved entry for the specified bucket and key. If a conflict
    /// exists with the key then an error message is returned.
    /// \param bucket The name of the storage bucket.
    /// \param key The name for this saved data.
    ///
    public static Request<Models.CloudStorageData> Load(string bucket, string key)
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.CloudStorageData>(CAPI.ovr_CloudStorage_Load(bucket, key));
      }

      return null;
    }

    /// Loads all the metadata for the saves in the specified bucket, including
    /// conflicts.
    /// \param bucket The name of the storage bucket.
    ///
    public static Request<Models.CloudStorageMetadataList> LoadBucketMetadata(string bucket)
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.CloudStorageMetadataList>(CAPI.ovr_CloudStorage_LoadBucketMetadata(bucket));
      }

      return null;
    }

    /// Loads the metadata for this bucket-key combination that need to be manually
    /// resolved.
    /// \param bucket The name of the storage bucket
    /// \param key The key for this saved data.
    ///
    public static Request<Models.CloudStorageConflictMetadata> LoadConflictMetadata(string bucket, string key)
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.CloudStorageConflictMetadata>(CAPI.ovr_CloudStorage_LoadConflictMetadata(bucket, key));
      }

      return null;
    }

    /// Loads the data specified by the storage handle.
    ///
    public static Request<Models.CloudStorageData> LoadHandle(string handle)
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.CloudStorageData>(CAPI.ovr_CloudStorage_LoadHandle(handle));
      }

      return null;
    }

    /// load the metadata for the specified key
    /// \param bucket The name of the storage bucket.
    /// \param key The name for this saved data.
    ///
    public static Request<Models.CloudStorageMetadata> LoadMetadata(string bucket, string key)
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.CloudStorageMetadata>(CAPI.ovr_CloudStorage_LoadMetadata(bucket, key));
      }

      return null;
    }

    /// Selects the local save for manual conflict resolution.
    /// \param bucket The name of the storage bucket.
    /// \param key The name for this saved data.
    /// \param remoteHandle The handle of the remote that the local file was resolved against.
    ///
    public static Request<Models.CloudStorageUpdateResponse> ResolveKeepLocal(string bucket, string key, string remoteHandle)
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.CloudStorageUpdateResponse>(CAPI.ovr_CloudStorage_ResolveKeepLocal(bucket, key, remoteHandle));
      }

      return null;
    }

    /// Selects the remote save for manual conflict resolution.
    /// \param bucket The name of the storage bucket.
    /// \param key The name for this saved data.
    /// \param remoteHandle The handle of the remote.
    ///
    public static Request<Models.CloudStorageUpdateResponse> ResolveKeepRemote(string bucket, string key, string remoteHandle)
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.CloudStorageUpdateResponse>(CAPI.ovr_CloudStorage_ResolveKeepRemote(bucket, key, remoteHandle));
      }

      return null;
    }

    /// Note: Cloud Storage is only available for Rift apps.
    ///
    /// Send a save data buffer to the platform. CloudStorage.Save() passes a
    /// pointer to your data in an async call. You need to maintain the save data
    /// until you receive the message indicating that the save was successful.
    ///
    /// If the data is destroyed or modified prior to receiving that message the
    /// data will not be saved.
    /// \param bucket The name of the storage bucket.
    /// \param key The name for this saved data.
    /// \param data Start of the data block.
    /// \param counter Optional. Counter used for user data or auto-deconfliction.
    /// \param extraData Optional. String data that isn't used by the platform.
    ///
    /// <b>Error codes</b>
    /// - \b 100: The stored version has a later timestamp than the data provided. This cloud storage bucket's conflict resolution policy is configured to use the latest timestamp, which is configurable in the developer dashboard.
    ///
    public static Request<Models.CloudStorageUpdateResponse> Save(string bucket, string key, byte[] data, long counter, string extraData)
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.CloudStorageUpdateResponse>(CAPI.ovr_CloudStorage_Save(bucket, key, data, (uint)(data != null ? data.Length : 0), counter, extraData));
      }

      return null;
    }

  }

  public static partial class Entitlements
  {
    /// Returns whether the current user is entitled to the current app.
    ///
    public static Request IsUserEntitledToApplication()
    {
      if (Core.IsInitialized())
      {
        return new Request(CAPI.ovr_Entitlement_GetIsViewerEntitled());
      }

      return null;
    }

  }

  public static partial class GraphAPI
  {
  }

  public static partial class HTTP
  {
  }

  public static partial class IAP
  {
    /// Allow the consumable IAP product to be purchased again. Conceptually, this
    /// indicates that the item was used or consumed.
    ///
    public static Request ConsumePurchase(string sku)
    {
      if (Core.IsInitialized())
      {
        return new Request(CAPI.ovr_IAP_ConsumePurchase(sku));
      }

      return null;
    }

    /// Retrieve a list of IAP products that can be purchased.
    /// \param skus The SKUs of the products to retrieve.
    ///
    public static Request<Models.ProductList> GetProductsBySKU(string[] skus)
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.ProductList>(CAPI.ovr_IAP_GetProductsBySKU(skus, (skus != null ? skus.Length : 0)));
      }

      return null;
    }

    /// Retrieve a list of Purchase that the Logged-In-User has made. This list
    /// will also contain consumable purchases that have not been consumed.
    ///
    public static Request<Models.PurchaseList> GetViewerPurchases()
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.PurchaseList>(CAPI.ovr_IAP_GetViewerPurchases());
      }

      return null;
    }

    /// Launch the checkout flow to purchase the existing product. Oculus Home
    /// tries handle and fix as many errors as possible. Home returns the
    /// appropriate error message and how to resolveit, if possible. Returns a
    /// purchase on success, empty purchase on cancel, and an error on error.
    /// \param sku IAP sku for the item the user wishes to purchase.
    ///
    public static Request<Models.Purchase> LaunchCheckoutFlow(string sku)
    {
      if (Core.IsInitialized())
      {
        if (UnityEngine.Application.isEditor) {
          throw new NotImplementedException("LaunchCheckoutFlow() is not implemented in the editor yet.");
        }

        return new Request<Models.Purchase>(CAPI.ovr_IAP_LaunchCheckoutFlow(sku));
      }

      return null;
    }

  }

  public static partial class LanguagePack
  {
    /// Returns currently installed and selected language pack for an app in the
    /// view of the `asset_details`. Use `language` field to extract neeeded
    /// language info. A particular language can be download and installed by a
    /// user from the Oculus app on the application page.
    ///
    public static Request<Models.AssetDetails> GetCurrent()
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.AssetDetails>(CAPI.ovr_LanguagePack_GetCurrent());
      }

      return null;
    }

  }

  public static partial class Leaderboards
  {
    /// Requests a block of Leaderboard Entries.
    /// \param leaderboardName The name of the leaderboard whose entries to return.
    /// \param limit Defines the maximum number of entries to return.
    /// \param filter Allows you to restrict the returned values by friends.
    /// \param startAt Defines whether to center the query on the user or start at the top of the leaderboard.
    ///
    /// <b>Error codes</b>
    /// - \b 12074: You're not yet ranked on this leaderboard.
    ///
    public static Request<Models.LeaderboardEntryList> GetEntries(string leaderboardName, int limit, LeaderboardFilterType filter, LeaderboardStartAt startAt)
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.LeaderboardEntryList>(CAPI.ovr_Leaderboard_GetEntries(leaderboardName, limit, filter, startAt));
      }

      return null;
    }

    /// Requests a block of leaderboard Entries.
    /// \param leaderboardName The name of the leaderboard.
    /// \param limit The maximum number of entries to return.
    /// \param afterRank The position after which to start.  For example, 10 returns leaderboard results starting with the 11th user.
    ///
    public static Request<Models.LeaderboardEntryList> GetEntriesAfterRank(string leaderboardName, int limit, ulong afterRank)
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.LeaderboardEntryList>(CAPI.ovr_Leaderboard_GetEntriesAfterRank(leaderboardName, limit, afterRank));
      }

      return null;
    }

    /// Writes a single entry to a leaderboard.
    /// \param leaderboardName The leaderboard for which to write the entry.
    /// \param score The score to write.
    /// \param extraData A 2KB custom data field that is associated with the leaderboard entry. This can be a game replay or anything that provides more detail about the entry to the viewer.
    /// \param forceUpdate If true, the score always updates.  This happens even if it is not the user's best score.
    ///
    /// <b>Error codes</b>
    /// - \b 100: Parameter {parameter}: invalid user id: {user_id}
    ///
    public static Request<bool> WriteEntry(string leaderboardName, long score, byte[] extraData = null, bool forceUpdate = false)
    {
      if (Core.IsInitialized())
      {
        return new Request<bool>(CAPI.ovr_Leaderboard_WriteEntry(leaderboardName, score, extraData, (uint)(extraData != null ? extraData.Length : 0), forceUpdate));
      }

      return null;
    }

  }

  public static partial class Livestreaming
  {
    /// Return the status of the current livestreaming session if there is one.
    ///
    public static Request<Models.LivestreamingStatus> GetStatus()
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.LivestreamingStatus>(CAPI.ovr_Livestreaming_GetStatus());
      }

      return null;
    }

    /// Pauses the livestreaming session if there is one. NOTE: this function is
    /// safe to call if no session is active.
    ///
    public static Request<Models.LivestreamingStatus> PauseStream()
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.LivestreamingStatus>(CAPI.ovr_Livestreaming_PauseStream());
      }

      return null;
    }

    /// Resumes the livestreaming session if there is one. NOTE: this function is
    /// safe to call if no session is active.
    ///
    public static Request<Models.LivestreamingStatus> ResumeStream()
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.LivestreamingStatus>(CAPI.ovr_Livestreaming_ResumeStream());
      }

      return null;
    }

  }

  public static partial class Matchmaking
  {
    /// DEPRECATED. Use Browse2.
    /// \param pool A BROWSE type matchmaking pool.
    /// \param customQueryData Optional. Custom query data.
    ///
    /// <b>Error codes</b>
    /// - \b 100: Pool {pool_key} does not contain custom data key {key}. You can configure matchmaking custom data at https://dashboard.oculus.com/application/&lt;app_id&gt;/matchmaking
    /// - \b 12072: Unknown pool: {pool_key}. You can configure matchmaking pools at https://dashboard.oculus.com/application/&lt;app_id&gt;/matchmaking
    ///
    public static Request<Models.MatchmakingBrowseResult> Browse(string pool, CustomQuery customQueryData = null)
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.MatchmakingBrowseResult>(CAPI.ovr_Matchmaking_Browse(pool, customQueryData != null ? customQueryData.ToUnmanaged() : IntPtr.Zero));
      }

      return null;
    }

    /// Modes: BROWSE
    ///
    /// See overview documentation above.
    ///
    /// Return a list of matchmaking rooms in the current pool filtered by skill
    /// and ping (if enabled). This also enqueues the user in the matchmaking
    /// queue. When the user has made a selection, call Room.Join2() on one of the
    /// rooms that was returned. If the user stops browsing, call
    /// Matchmaking.Cancel().
    ///
    /// In addition to the list of rooms, enqueue results are also returned. Call
    /// MatchmakingBrowseResult.GetEnqueueResult() to obtain them. See
    /// OVR_MatchmakingEnqueueResult.h for details.
    /// \param pool A BROWSE type matchmaking pool.
    /// \param matchmakingOptions Additional matchmaking configuration for this request. Optional.
    ///
    /// <b>Error codes</b>
    /// - \b 100: Pool {pool_key} does not contain custom data key {key}. You can configure matchmaking custom data at https://dashboard.oculus.com/application/&lt;app_id&gt;/matchmaking
    /// - \b 12072: Unknown pool: {pool_key}. You can configure matchmaking pools at https://dashboard.oculus.com/application/&lt;app_id&gt;/matchmaking
    ///
    public static Request<Models.MatchmakingBrowseResult> Browse2(string pool, MatchmakingOptions matchmakingOptions = null)
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.MatchmakingBrowseResult>(CAPI.ovr_Matchmaking_Browse2(pool, (IntPtr)matchmakingOptions));
      }

      return null;
    }

    /// DEPRECATED. Use Cancel2.
    /// \param pool The pool in question.
    /// \param requestHash Used to find your entry in a queue.
    ///
    /// <b>Error codes</b>
    /// - \b 10: Room {room_id}: The user does not have permission to {cannot_action} because the user is currently in another room (perhaps on another device), and thus is no longer in this room. Users can only be in one room at a time. If they are active on two different devices at once, there will be undefined behavior.
    /// - \b 10: Room {room_id}: The user does not have permission to {cannot_action} because the user is not in the room (or any room). Perhaps they already left, or they stopped heartbeating. If this is a test environment, make sure you are not using the deprecated initialization methods ovr_PlatformInitializeStandaloneAccessToken (C++)/StandalonePlatform.Initialize(accessToken) (C#).
    /// - \b 10: Room {room_id}: The user does not have permission to {cannot_action} because the user is not the owner of the room.
    /// - \b 100: Invalid room_id: {room_id}. Either the ID is not a valid room or the user does not have permission to see or act on the room.
    ///
    public static Request Cancel(string pool, string requestHash)
    {
      if (Core.IsInitialized())
      {
        return new Request(CAPI.ovr_Matchmaking_Cancel(pool, requestHash));
      }

      return null;
    }

    /// Modes: QUICKMATCH, BROWSE
    ///
    /// Makes a best effort to cancel a previous Enqueue request before a match
    /// occurs. Typically triggered when a user gives up waiting. For BROWSE mode,
    /// call this when a user gives up looking through the room list or when the
    /// host of a room wants to stop receiving new users. If you don't cancel but
    /// the user goes offline, the user/room will be timed out of the queue within
    /// 30 seconds.
    ///
    /// <b>Error codes</b>
    /// - \b 10: Room {room_id}: The user does not have permission to {cannot_action} because the user is currently in another room (perhaps on another device), and thus is no longer in this room. Users can only be in one room at a time. If they are active on two different devices at once, there will be undefined behavior.
    /// - \b 10: Room {room_id}: The user does not have permission to {cannot_action} because the user is not in the room (or any room). Perhaps they already left, or they stopped heartbeating. If this is a test environment, make sure you are not using the deprecated initialization methods ovr_PlatformInitializeStandaloneAccessToken (C++)/StandalonePlatform.Initialize(accessToken) (C#).
    /// - \b 10: Room {room_id}: The user does not have permission to {cannot_action} because the user is not the owner of the room.
    /// - \b 100: Invalid room_id: {room_id}. Either the ID is not a valid room or the user does not have permission to see or act on the room.
    ///
    public static Request Cancel()
    {
      if (Core.IsInitialized())
      {
        return new Request(CAPI.ovr_Matchmaking_Cancel2());
      }

      return null;
    }

    /// DEPRECATED. Use CreateAndEnqueueRoom2.
    /// \param pool The matchmaking pool to use, which is defined for the app.
    /// \param maxUsers Overrides the Max Users value, which is configured in pool settings of the Developer Dashboard.
    /// \param subscribeToUpdates If true, sends a message with type MessageType.Notification_Room_RoomUpdate when the room data changes, such as when users join or leave.
    /// \param customQueryData Optional.  See "Custom criteria" section above.
    ///
    /// <b>Error codes</b>
    /// - \b 100: Pool {pool_key} does not contain custom data key {key}. You can configure matchmaking custom data at https://dashboard.oculus.com/application/&lt;app_id&gt;/matchmaking
    /// - \b 12051: Pool '{pool_key}' is configured for Quickmatch mode. In Quickmatch mode, rooms are created on users' behalf when a match is found. Specify Advanced Quickmatch or Browse mode to use this feature.
    /// - \b 12072: Unknown pool: {pool_key}. You can configure matchmaking pools at https://dashboard.oculus.com/application/&lt;app_id&gt;/matchmaking
    /// - \b 12089: You have asked to enqueue {num_users} users together, but this must be less than the maximum number of users in a room, {max_users}.
    ///
    public static Request<Models.MatchmakingEnqueueResultAndRoom> CreateAndEnqueueRoom(string pool, uint maxUsers, bool subscribeToUpdates = false, CustomQuery customQueryData = null)
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.MatchmakingEnqueueResultAndRoom>(CAPI.ovr_Matchmaking_CreateAndEnqueueRoom(pool, maxUsers, subscribeToUpdates, customQueryData != null ? customQueryData.ToUnmanaged() : IntPtr.Zero));
      }

      return null;
    }

    /// Modes: BROWSE, QUICKMATCH (Advanced; Can Users Create Rooms = true)
    ///
    /// See overview documentation above.
    ///
    /// Create a matchmaking room, join it, and enqueue it. This is the preferred
    /// method. But, if you do not wish to automatically enqueue the room, you can
    /// call CreateRoom2 instead.
    ///
    /// Visit https://dashboard.oculus.com/application/[YOUR_APP_ID]/matchmaking to
    /// set up pools and queries
    /// \param pool The matchmaking pool to use, which is defined for the app.
    /// \param matchmakingOptions Additional matchmaking configuration for this request. Optional.
    ///
    /// <b>Error codes</b>
    /// - \b 100: Pool {pool_key} does not contain custom data key {key}. You can configure matchmaking custom data at https://dashboard.oculus.com/application/&lt;app_id&gt;/matchmaking
    /// - \b 12051: Pool '{pool_key}' is configured for Quickmatch mode. In Quickmatch mode, rooms are created on users' behalf when a match is found. Specify Advanced Quickmatch or Browse mode to use this feature.
    /// - \b 12072: Unknown pool: {pool_key}. You can configure matchmaking pools at https://dashboard.oculus.com/application/&lt;app_id&gt;/matchmaking
    /// - \b 12089: You have asked to enqueue {num_users} users together, but this must be less than the maximum number of users in a room, {max_users}.
    ///
    public static Request<Models.MatchmakingEnqueueResultAndRoom> CreateAndEnqueueRoom2(string pool, MatchmakingOptions matchmakingOptions = null)
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.MatchmakingEnqueueResultAndRoom>(CAPI.ovr_Matchmaking_CreateAndEnqueueRoom2(pool, (IntPtr)matchmakingOptions));
      }

      return null;
    }

    /// DEPRECATED. Use CreateRoom2.
    /// \param pool The matchmaking pool to use, which is defined for the app.
    /// \param maxUsers Overrides the Max Users value, which is configured in pool settings of the Developer Dashboard.
    /// \param subscribeToUpdates If true, sends a message with type MessageType.Notification_Room_RoomUpdate when room data changes, such as when users join or leave.
    ///
    public static Request<Models.Room> CreateRoom(string pool, uint maxUsers, bool subscribeToUpdates = false)
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.Room>(CAPI.ovr_Matchmaking_CreateRoom(pool, maxUsers, subscribeToUpdates));
      }

      return null;
    }

    /// Create a matchmaking room and join it, but do not enqueue the room. After
    /// creation, you can call EnqueueRoom2. However, Oculus recommends using
    /// CreateAndEnqueueRoom2 instead.
    ///
    /// Modes: BROWSE, QUICKMATCH (Advanced; Can Users Create Rooms = true)
    ///
    /// Create a matchmaking room and join it, but do not enqueue the room. After
    /// creation, you can call EnqueueRoom. Consider using CreateAndEnqueueRoom
    /// instead.
    ///
    /// Visit https://dashboard.oculus.com/application/[YOUR_APP_ID]/matchmaking to
    /// set up pools and queries
    /// \param pool The matchmaking pool to use, which is defined for the app.
    /// \param matchmakingOptions Additional matchmaking configuration for this request. Optional.
    ///
    public static Request<Models.Room> CreateRoom2(string pool, MatchmakingOptions matchmakingOptions = null)
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.Room>(CAPI.ovr_Matchmaking_CreateRoom2(pool, (IntPtr)matchmakingOptions));
      }

      return null;
    }

    /// DEPRECATED. Use Enqueue2.
    /// \param pool The pool to enqueue in.
    /// \param customQueryData Optional.  See "Custom criteria" section above.
    ///
    /// <b>Error codes</b>
    /// - \b 100: Pool {pool_key} does not contain custom data key {key}. You can configure matchmaking custom data at https://dashboard.oculus.com/application/&lt;app_id&gt;/matchmaking
    /// - \b 12072: Unknown pool: {pool_key}. You can configure matchmaking pools at https://dashboard.oculus.com/application/&lt;app_id&gt;/matchmaking
    ///
    public static Request<Models.MatchmakingEnqueueResult> Enqueue(string pool, CustomQuery customQueryData = null)
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.MatchmakingEnqueueResult>(CAPI.ovr_Matchmaking_Enqueue(pool, customQueryData != null ? customQueryData.ToUnmanaged() : IntPtr.Zero));
      }

      return null;
    }

    /// Modes: QUICKMATCH
    ///
    /// See overview documentation above.
    ///
    /// Enqueue yourself to await an available matchmaking room. The platform
    /// returns a MessageType.Notification_Matchmaking_MatchFound message when a
    /// match is found. Call Room.Join2() on the returned room. The response
    /// contains useful information to display to the user to set expectations for
    /// how long it will take to get a match.
    ///
    /// If the user stops waiting, call Matchmaking.Cancel().
    /// \param pool The pool to enqueue in.
    /// \param matchmakingOptions Additional matchmaking configuration for this request. Optional.
    ///
    /// <b>Error codes</b>
    /// - \b 100: Pool {pool_key} does not contain custom data key {key}. You can configure matchmaking custom data at https://dashboard.oculus.com/application/&lt;app_id&gt;/matchmaking
    /// - \b 12072: Unknown pool: {pool_key}. You can configure matchmaking pools at https://dashboard.oculus.com/application/&lt;app_id&gt;/matchmaking
    ///
    public static Request<Models.MatchmakingEnqueueResult> Enqueue2(string pool, MatchmakingOptions matchmakingOptions = null)
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.MatchmakingEnqueueResult>(CAPI.ovr_Matchmaking_Enqueue2(pool, (IntPtr)matchmakingOptions));
      }

      return null;
    }

    /// DEPRECATED. Please use Matchmaking.EnqueueRoom2() instead.
    /// \param roomID Returned either from MessageType.Notification_Matchmaking_MatchFound or from Matchmaking.CreateRoom().
    /// \param customQueryData Optional.  See the "Custom criteria" section above.
    ///
    /// <b>Error codes</b>
    /// - \b 10: Room {room_id}: The user does not have permission to {cannot_action} because the user is currently in another room (perhaps on another device), and thus is no longer in this room. Users can only be in one room at a time. If they are active on two different devices at once, there will be undefined behavior.
    /// - \b 10: Room {room_id}: The user does not have permission to {cannot_action} because the user is not in the room (or any room). Perhaps they already left, or they stopped heartbeating. If this is a test environment, make sure you are not using the deprecated initialization methods ovr_PlatformInitializeStandaloneAccessToken (C++)/StandalonePlatform.Initialize(accessToken) (C#).
    /// - \b 10: Room {room_id}: The user does not have permission to {cannot_action} because the user is not the owner of the room.
    /// - \b 100: Invalid room_id: {room_id}. Either the ID is not a valid room or the user does not have permission to see or act on the room.
    /// - \b 12051: Pool '{pool_key}' is configured for Quickmatch mode. In Quickmatch mode, rooms are created on users' behalf when a match is found. Specify Advanced Quickmatch or Browse mode to use this feature.
    ///
    public static Request<Models.MatchmakingEnqueueResult> EnqueueRoom(UInt64 roomID, CustomQuery customQueryData = null)
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.MatchmakingEnqueueResult>(CAPI.ovr_Matchmaking_EnqueueRoom(roomID, customQueryData != null ? customQueryData.ToUnmanaged() : IntPtr.Zero));
      }

      return null;
    }

    /// Modes: BROWSE (for Rooms only), ROOM
    ///
    /// See the overview documentation above. Enqueue yourself to await an
    /// available matchmaking room. MessageType.Notification_Matchmaking_MatchFound
    /// gets enqueued when a match is found.
    ///
    /// The response contains useful information to display to the user to set
    /// expectations for how long it will take to get a match.
    ///
    /// If the user stops waiting, call Matchmaking.Cancel().
    /// \param roomID Returned either from MessageType.Notification_Matchmaking_MatchFound or from Matchmaking.CreateRoom().
    /// \param matchmakingOptions Additional matchmaking configuration for this request. Optional.
    ///
    /// <b>Error codes</b>
    /// - \b 10: Room {room_id}: The user does not have permission to {cannot_action} because the user is currently in another room (perhaps on another device), and thus is no longer in this room. Users can only be in one room at a time. If they are active on two different devices at once, there will be undefined behavior.
    /// - \b 10: Room {room_id}: The user does not have permission to {cannot_action} because the user is not in the room (or any room). Perhaps they already left, or they stopped heartbeating. If this is a test environment, make sure you are not using the deprecated initialization methods ovr_PlatformInitializeStandaloneAccessToken (C++)/StandalonePlatform.Initialize(accessToken) (C#).
    /// - \b 10: Room {room_id}: The user does not have permission to {cannot_action} because the user is not the owner of the room.
    /// - \b 100: Invalid room_id: {room_id}. Either the ID is not a valid room or the user does not have permission to see or act on the room.
    /// - \b 12051: Pool '{pool_key}' is configured for Quickmatch mode. In Quickmatch mode, rooms are created on users' behalf when a match is found. Specify Advanced Quickmatch or Browse mode to use this feature.
    ///
    public static Request<Models.MatchmakingEnqueueResult> EnqueueRoom2(UInt64 roomID, MatchmakingOptions matchmakingOptions = null)
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.MatchmakingEnqueueResult>(CAPI.ovr_Matchmaking_EnqueueRoom2(roomID, (IntPtr)matchmakingOptions));
      }

      return null;
    }

    /// Modes: QUICKMATCH, BROWSE
    ///
    /// Used to debug the state of the current matchmaking pool queue. This is not
    /// intended to be used in production.
    ///
    public static Request<Models.MatchmakingAdminSnapshot> GetAdminSnapshot()
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.MatchmakingAdminSnapshot>(CAPI.ovr_Matchmaking_GetAdminSnapshot());
      }

      return null;
    }

    /// DEPRECATED. Use ovr_Room_Join2.
    /// \param roomID ID of a room previously returned from MessageType.Notification_Matchmaking_MatchFound or Matchmaking.Browse().
    /// \param subscribeToUpdates If true, sends a message with type MessageType.Notification_Room_RoomUpdate when room data changes, such as when users join or leave.
    ///
    public static Request<Models.Room> JoinRoom(UInt64 roomID, bool subscribeToUpdates = false)
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.Room>(CAPI.ovr_Matchmaking_JoinRoom(roomID, subscribeToUpdates));
      }

      return null;
    }

    /// Modes: QUICKMATCH, BROWSE (+ Skill Pool)
    ///
    /// For pools with skill-based matching. See overview documentation above.
    ///
    /// Call after calling Room.Join2() when the players are present to begin a
    /// rated match for which you plan to report the results (using
    /// Matchmaking.ReportResultInsecure()).
    ///
    /// <b>Error codes</b>
    /// - \b 100: There is no active match associated with the room {room_id}.
    /// - \b 100: You can only start matches, report matches, and track skill ratings in matchmaking rooms. {room_id} is a room, but it is not a matchmaking room.
    ///
    public static Request StartMatch(UInt64 roomID)
    {
      if (Core.IsInitialized())
      {
        return new Request(CAPI.ovr_Matchmaking_StartMatch(roomID));
      }

      return null;
    }

  }

  public static partial class Media
  {
    /// Launch the Share to Facebook modal via a deeplink to Home on Gear VR,
    /// allowing users to share local media files to Facebook. Accepts a
    /// postTextSuggestion string for the default text of the Facebook post.
    /// Requires a filePath string as the path to the image to be shared to
    /// Facebook. This image should be located in your app's internal storage
    /// directory. Requires a contentType indicating the type of media to be shared
    /// (only 'photo' is currently supported.)
    /// \param postTextSuggestion this text will prepopulate the facebook status text-input box within the share modal
    /// \param filePath path to the file to be shared to facebook
    /// \param contentType content type of the media to be shared
    ///
    public static Request<Models.ShareMediaResult> ShareToFacebook(string postTextSuggestion, string filePath, MediaContentType contentType)
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.ShareMediaResult>(CAPI.ovr_Media_ShareToFacebook(postTextSuggestion, filePath, contentType));
      }

      return null;
    }

  }

  public static partial class Notifications
  {
    /// Retrieve a list of all pending room invites for your application (for
    /// example, notifications that may have been sent before the user launched
    /// your game). You can also get push notifications with
    /// MessageType.Notification_Room_InviteReceived.
    ///
    public static Request<Models.RoomInviteNotificationList> GetRoomInviteNotifications()
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.RoomInviteNotificationList>(CAPI.ovr_Notification_GetRoomInvites());
      }

      return null;
    }

    /// Mark a notification as read. This causes it to disappear from the Universal
    /// Menu, the Oculus App, Oculus Home, and in-app retrieval.
    ///
    public static Request MarkAsRead(UInt64 notificationID)
    {
      if (Core.IsInitialized())
      {
        return new Request(CAPI.ovr_Notification_MarkAsRead(notificationID));
      }

      return null;
    }

  }

  public static partial class Parties
  {
    /// Load the party the current user is in.
    ///
    public static Request<Models.Party> GetCurrent()
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.Party>(CAPI.ovr_Party_GetCurrent());
      }

      return null;
    }

  }

  public static partial class Rooms
  {
    /// DEPRECATED. Use CreateAndJoinPrivate2.
    /// \param joinPolicy Specifies who can join the room without an invite.
    /// \param maxUsers The maximum number of users allowed in the room, including the creator.
    /// \param subscribeToUpdates If true, sends a message with type MessageType.Notification_Room_RoomUpdate when room data changes, such as when users join or leave.
    ///
    public static Request<Models.Room> CreateAndJoinPrivate(RoomJoinPolicy joinPolicy, uint maxUsers, bool subscribeToUpdates = false)
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.Room>(CAPI.ovr_Room_CreateAndJoinPrivate(joinPolicy, maxUsers, subscribeToUpdates));
      }

      return null;
    }

    /// Creates a new private (client controlled) room and adds the caller to it.
    /// This type of room is good for matches where the user wants to play with
    /// friends, as they're primarially discoverable by examining which rooms your
    /// friends are in.
    /// \param joinPolicy Specifies who can join the room without an invite.
    /// \param maxUsers The maximum number of users allowed in the room, including the creator.
    /// \param roomOptions Additional room configuration for this request. Optional.
    ///
    public static Request<Models.Room> CreateAndJoinPrivate2(RoomJoinPolicy joinPolicy, uint maxUsers, RoomOptions roomOptions)
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.Room>(CAPI.ovr_Room_CreateAndJoinPrivate2(joinPolicy, maxUsers, (IntPtr)roomOptions));
      }

      return null;
    }

    /// Allows arbitrary rooms for the application to be loaded.
    /// \param roomID The room to load.
    ///
    public static Request<Models.Room> Get(UInt64 roomID)
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.Room>(CAPI.ovr_Room_Get(roomID));
      }

      return null;
    }

    /// Easy loading of the room you're currently in. If you don't want live
    /// updates on your current room (by using subscribeToUpdates), you can use
    /// this to refresh the data.
    ///
    public static Request<Models.Room> GetCurrent()
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.Room>(CAPI.ovr_Room_GetCurrent());
      }

      return null;
    }

    /// Allows the current room for a given user to be loaded. Remember that the
    /// user's privacy settings may not allow their room to be loaded. Because of
    /// this, it's often possible to load the users in a room, but not to take
    /// those users and load their room.
    /// \param userID ID of the user for which to load the room.
    ///
    public static Request<Models.Room> GetCurrentForUser(UInt64 userID)
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.Room>(CAPI.ovr_Room_GetCurrentForUser(userID));
      }

      return null;
    }

    /// DEPRECATED. Use GetInvitableUsers2.
    ///
    public static Request<Models.UserList> GetInvitableUsers()
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.UserList>(CAPI.ovr_Room_GetInvitableUsers());
      }

      return null;
    }

    /// Loads a list of users you can invite to a room. These are pulled from your
    /// friends list and recently met lists and filtered for relevance and
    /// interest. If the room cannot be joined, this list will be empty. By
    /// default, the invitable users returned will be for the user's current room.
    ///
    /// If your application grouping was created after September 9 2017, recently
    /// met users will be included by default. If your application grouping was
    /// created before then, you can go to edit the setting in the "Rooms and
    /// Matchmaking" section of Platform Services at dashboard.oculus.com
    ///
    /// Customization can be done via RoomOptions. Create this object with
    /// RoomOptions(). The params that could be used are:
    ///
    /// 1. RoomOptions.SetRoomId()- will return the invitable users for this room
    /// (instead of the current room).
    ///
    /// 2. RoomOptions.SetOrdering() - returns the list of users in the provided
    /// ordering (see UserOrdering enum).
    ///
    /// 3. RoomOptions.SetRecentlyMetTimeWindow() - how long long ago should we
    /// include users you've recently met in the results?
    ///
    /// 4. RoomOptions.SetMaxUserResults() - we will limit the number of results
    /// returned. By default, the number is unlimited, but the server may choose to
    /// limit results for performance reasons.
    ///
    /// 5. RoomOptions.SetExcludeRecentlyMet() - Don't include users recently in
    /// rooms with this user in the result. Also, see the above comment.
    ///
    /// Example custom C++ usage:
    ///
    ///   auto roomOptions = ovr_RoomOptions_Create();
    ///   ovr_RoomOptions_SetOrdering(roomOptions, ovrUserOrdering_PresenceAlphabetical);
    ///   ovr_RoomOptions_SetRoomId(roomOptions, roomID);
    ///   ovr_Room_GetInvitableUsers2(roomOptions);
    ///   ovr_RoomOptions_Destroy(roomOptions);
    /// \param roomOptions Additional configuration for this request. Optional.
    ///
    public static Request<Models.UserList> GetInvitableUsers2(RoomOptions roomOptions = null)
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.UserList>(CAPI.ovr_Room_GetInvitableUsers2((IntPtr)roomOptions));
      }

      return null;
    }

    /// Fetches the list of moderated rooms created for the application.
    ///
    public static Request<Models.RoomList> GetModeratedRooms()
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.RoomList>(CAPI.ovr_Room_GetModeratedRooms());
      }

      return null;
    }

    /// Invites a user to the specified room. They will receive a notification via
    /// MessageType.Notification_Room_InviteReceived if they are in your game,
    /// and/or they can poll for room invites using
    /// Notification.GetRoomInviteNotifications().
    /// \param roomID The ID of your current room.
    /// \param inviteToken A user's invite token, returned by Room.GetInvitableUsers().
    ///
    /// <b>Error codes</b>
    /// - \b 100: The invite token has expired, the user will need to be reinvited to the room.
    /// - \b 100: The target user cannot join you in your current experience
    /// - \b 100: You cannot send an invite to a room you are not in
    ///
    public static Request<Models.Room> InviteUser(UInt64 roomID, string inviteToken)
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.Room>(CAPI.ovr_Room_InviteUser(roomID, inviteToken));
      }

      return null;
    }

    /// Joins the target room (leaving the one you're currently in).
    /// \param roomID The room to join.
    /// \param subscribeToUpdates If true, sends a message with type MessageType.Notification_Room_RoomUpdate when room data changes, such as when users join or leave.
    ///
    /// <b>Error codes</b>
    /// - \b 10: The room you're attempting to join is currently locked. Please try again later.
    /// - \b 10: You don't have permission to enter this room. You may need to be invited first.
    /// - \b 100: Invalid room_id: {room_id}. Either the ID is not a valid room or the user does not have permission to see or act on the room.
    /// - \b 100: The room you're attempting to join is full. Please try again later.
    /// - \b 100: This game isn't available. If it already started or was canceled, you can host a new game at any point.
    ///
    public static Request<Models.Room> Join(UInt64 roomID, bool subscribeToUpdates = false)
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.Room>(CAPI.ovr_Room_Join(roomID, subscribeToUpdates));
      }

      return null;
    }

    /// Joins the target room (leaving the one you're currently in).
    /// \param roomID The room to join.
    /// \param roomOptions Additional room configuration for this request. Optional.
    ///
    /// <b>Error codes</b>
    /// - \b 10: The room you're attempting to join is currently locked. Please try again later.
    /// - \b 10: You don't have permission to enter this room. You may need to be invited first.
    /// - \b 100: Invalid room_id: {room_id}. Either the ID is not a valid room or the user does not have permission to see or act on the room.
    /// - \b 100: The room you're attempting to join is full. Please try again later.
    /// - \b 100: This game isn't available. If it already started or was canceled, you can host a new game at any point.
    ///
    public static Request<Models.Room> Join2(UInt64 roomID, RoomOptions roomOptions)
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.Room>(CAPI.ovr_Room_Join2(roomID, (IntPtr)roomOptions));
      }

      return null;
    }

    /// Allows the room owner to kick a user out of the current room.
    /// \param roomID The room that you currently own (check Room.GetOwner()).
    /// \param userID The user to be kicked (cannot be yourself).
    /// \param kickDurationSeconds Length of the ban, in seconds.
    ///
    /// <b>Error codes</b>
    /// - \b 10: Room {room_id}: The user does not have permission to {cannot_action} because the user is not in the room (or any room). Perhaps they already left, or they stopped heartbeating. If this is a test environment, make sure you are not using the deprecated initialization methods ovr_PlatformInitializeStandaloneAccessToken (C++)/StandalonePlatform.Initialize(accessToken) (C#).
    /// - \b 10: Room {room_id}: The user does not have permission to {cannot_action} because the user is not the owner of the room.
    /// - \b 100: You cannot remove yourself from room {room_id}
    ///
    public static Request<Models.Room> KickUser(UInt64 roomID, UInt64 userID, int kickDurationSeconds)
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.Room>(CAPI.ovr_Room_KickUser(roomID, userID, kickDurationSeconds));
      }

      return null;
    }

    /// Launch the invitable user flow to invite to the logged in user's current
    /// room. This is intended to be a nice shortcut for developers not wanting to
    /// build out their own Invite UI although it has the same rules as if you
    /// build it yourself.
    ///
    public static Request LaunchInvitableUserFlow(UInt64 roomID)
    {
      if (Core.IsInitialized())
      {
        return new Request(CAPI.ovr_Room_LaunchInvitableUserFlow(roomID));
      }

      return null;
    }

    /// Removes you from your current room. Returns the solo room you are now in if
    /// it succeeds
    /// \param roomID The room you're currently in.
    ///
    public static Request<Models.Room> Leave(UInt64 roomID)
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.Room>(CAPI.ovr_Room_Leave(roomID));
      }

      return null;
    }

    /// Allows the room owner to set the description of their room.
    /// \param roomID The room that you currently own (check Room.GetOwner()).
    /// \param description The new name of the room.
    ///
    /// <b>Error codes</b>
    /// - \b 10: Room {room_id}: The user does not have permission to {cannot_action} because the user is currently in another room (perhaps on another device), and thus is no longer in this room. Users can only be in one room at a time. If they are active on two different devices at once, there will be undefined behavior.
    /// - \b 10: Room {room_id}: The user does not have permission to {cannot_action} because the user is not in the room (or any room). Perhaps they already left, or they stopped heartbeating. If this is a test environment, make sure you are not using the deprecated initialization methods ovr_PlatformInitializeStandaloneAccessToken (C++)/StandalonePlatform.Initialize(accessToken) (C#).
    /// - \b 10: Room {room_id}: The user does not have permission to {cannot_action} because the user is not the owner of the room.
    ///
    public static Request<Models.Room> SetDescription(UInt64 roomID, string description)
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.Room>(CAPI.ovr_Room_SetDescription(roomID, description));
      }

      return null;
    }

    /// Disallow new members from being able to join the room. This will prevent
    /// joins from Room.Join(), invites, 'Join From Home', etc. Users that are in
    /// the room at the time of lockdown WILL be able to rejoin.
    /// \param roomID The room whose membership you want to lock or unlock.
    /// \param membershipLockStatus The new LockStatus for the room
    ///
    /// <b>Error codes</b>
    /// - \b 10: Room {room_id}: The user does not have permission to {cannot_action} because the user is currently in another room (perhaps on another device), and thus is no longer in this room. Users can only be in one room at a time. If they are active on two different devices at once, there will be undefined behavior.
    /// - \b 10: Room {room_id}: The user does not have permission to {cannot_action} because the user is not in the room (or any room). Perhaps they already left, or they stopped heartbeating. If this is a test environment, make sure you are not using the deprecated initialization methods ovr_PlatformInitializeStandaloneAccessToken (C++)/StandalonePlatform.Initialize(accessToken) (C#).
    /// - \b 10: Room {room_id}: The user does not have permission to {cannot_action} because the user is not the owner of the room.
    ///
    public static Request<Models.Room> UpdateMembershipLockStatus(UInt64 roomID, RoomMembershipLockStatus membershipLockStatus)
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.Room>(CAPI.ovr_Room_UpdateMembershipLockStatus(roomID, membershipLockStatus));
      }

      return null;
    }

    /// Allows the room owner to transfer ownership to someone else.
    /// \param roomID The room that the user owns (check Room.GetOwner()).
    /// \param userID The new user to make an owner; the user must be in the room.
    ///
    public static Request UpdateOwner(UInt64 roomID, UInt64 userID)
    {
      if (Core.IsInitialized())
      {
        return new Request(CAPI.ovr_Room_UpdateOwner(roomID, userID));
      }

      return null;
    }

    /// Sets the join policy of the user's private room.
    /// \param roomID The room ID that the user owns (check Room.GetOwner()).
    /// \param newJoinPolicy The new join policy for the room.
    ///
    public static Request<Models.Room> UpdatePrivateRoomJoinPolicy(UInt64 roomID, RoomJoinPolicy newJoinPolicy)
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.Room>(CAPI.ovr_Room_UpdatePrivateRoomJoinPolicy(roomID, newJoinPolicy));
      }

      return null;
    }

  }

  public static partial class Users
  {
    /// Retrieve the user with the given ID. This might fail if the ID is invalid
    /// or the user is blocked.
    ///
    /// NOTE: Users will have a unique ID per application.
    /// \param userID User ID retrieved with this application.
    ///
    public static Request<Models.User> Get(UInt64 userID)
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.User>(CAPI.ovr_User_Get(userID));
      }

      return null;
    }

    /// Return an access token for this user, suitable for making REST calls
    /// against graph.oculus.com.
    ///
    public static Request<string> GetAccessToken()
    {
      if (Core.IsInitialized())
      {
        return new Request<string>(CAPI.ovr_User_GetAccessToken());
      }

      return null;
    }

    /// Retrieve the currently signed in user. This call is available offline.
    ///
    /// NOTE: This will not return the user's presence as it should always be
    /// 'online' in your application.
    ///
    /// NOTE: Users will have a unique ID per application.
    ///
    public static Request<Models.User> GetLoggedInUser()
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.User>(CAPI.ovr_User_GetLoggedInUser());
      }

      return null;
    }

    /// Retrieve a list of the logged in user's friends.
    ///
    public static Request<Models.UserList> GetLoggedInUserFriends()
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.UserList>(CAPI.ovr_User_GetLoggedInUserFriends());
      }

      return null;
    }

    /// Retrieve a list of the logged in user's friends and any rooms they might be
    /// in.
    ///
    public static Request<Models.UserAndRoomList> GetLoggedInUserFriendsAndRooms()
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.UserAndRoomList>(CAPI.ovr_User_GetLoggedInUserFriendsAndRooms());
      }

      return null;
    }

    /// Returns a list of users that the logged in user was in a room with
    /// recently, sorted by relevance, along with any rooms they might be in. All
    /// you need to do to use this method is to use our Rooms API, and we will
    /// track the number of times users are together, their most recent encounter,
    /// and the amount of time they spend together.
    ///
    /// Customization can be done via UserOptions. Create this object with
    /// UserOptions(). The params that could be used are:
    ///
    /// 1. UserOptions.SetTimeWindow() - how recently should the users have played?
    /// The default is TimeWindow.ThirtyDays.
    ///
    /// 2. UserOptions.SetMaxUsers() - we will limit the number of results
    /// returned. By default, the number is unlimited, but the server may choose to
    /// limit results for performance reasons.
    /// \param userOptions Additional configuration for this request. Optional.
    ///
    public static Request<Models.UserAndRoomList> GetLoggedInUserRecentlyMetUsersAndRooms(UserOptions userOptions = null)
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.UserAndRoomList>(CAPI.ovr_User_GetLoggedInUserRecentlyMetUsersAndRooms((IntPtr)userOptions));
      }

      return null;
    }

    /// returns an ovrID which is unique per org. allows different apps within the
    /// same org to identify the user.
    /// \param userID to load the org scoped id of
    ///
    public static Request<Models.OrgScopedID> GetOrgScopedID(UInt64 userID)
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.OrgScopedID>(CAPI.ovr_User_GetOrgScopedID(userID));
      }

      return null;
    }

    /// Returns all accounts belonging to this user. Accounts are the Oculus user
    /// and x-users that are linked to this user.
    ///
    public static Request<Models.SdkAccountList> GetSdkAccounts()
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.SdkAccountList>(CAPI.ovr_User_GetSdkAccounts());
      }

      return null;
    }

    /// Part of the scheme to confirm the identity of a particular user in your
    /// backend. You can pass the result of User.GetUserProof() and a user ID from
    /// User.Get() to your your backend. Your server can then use our api to verify
    /// identity. 'https://graph.oculus.com/user_nonce_validate?nonce=USER_PROOF&us
    /// er_id=USER_ID&access_token=ACCESS_TOKEN'
    ///
    /// NOTE: The nonce is only good for one check and then it is invalidated.
    ///
    public static Request<Models.UserProof> GetUserProof()
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.UserProof>(CAPI.ovr_User_GetUserProof());
      }

      return null;
    }

    /// Launch the flow for sending a friend request to a user.
    /// \param userID User ID of user to send a friend request to
    ///
    public static Request<Models.LaunchFriendRequestFlowResult> LaunchFriendRequestFlow(UInt64 userID)
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.LaunchFriendRequestFlowResult>(CAPI.ovr_User_LaunchFriendRequestFlow(userID));
      }

      return null;
    }

    /// Launch the profile of the given user. The profile surfaces information
    /// about the user and supports relevant actions that the viewer may take on
    /// that user, e.g. sending a friend request.
    /// \param userID User ID for profile being viewed
    ///
    public static Request LaunchProfile(UInt64 userID)
    {
      if (Core.IsInitialized())
      {
        return new Request(CAPI.ovr_User_LaunchProfile(userID));
      }

      return null;
    }

  }

  public static partial class Voip
  {
    /// Sets whether SystemVoip should be suppressed so that this app's Voip can
    /// use the mic and play incoming Voip audio.
    ///
    public static Request<Models.SystemVoipState> SetSystemVoipSuppressed(bool suppressed)
    {
      if (Core.IsInitialized())
      {
        return new Request<Models.SystemVoipState>(CAPI.ovr_Voip_SetSystemVoipSuppressed(suppressed));
      }

      return null;
    }

  }


  public static partial class Achievements {
    public static Request<Models.AchievementDefinitionList> GetNextAchievementDefinitionListPage(Models.AchievementDefinitionList list) {
      if (!list.HasNextPage)
      {
        Debug.LogWarning("Oculus.Platform.GetNextAchievementDefinitionListPage: List has no next page");
        return null;
      }

      if (Core.IsInitialized())
      {
        return new Request<Models.AchievementDefinitionList>(
          CAPI.ovr_HTTP_GetWithMessageType(
            list.NextUrl,
            (int)Message.MessageType.Achievements_GetNextAchievementDefinitionArrayPage
          )
        );
      }

      return null;
    }

    public static Request<Models.AchievementProgressList> GetNextAchievementProgressListPage(Models.AchievementProgressList list) {
      if (!list.HasNextPage)
      {
        Debug.LogWarning("Oculus.Platform.GetNextAchievementProgressListPage: List has no next page");
        return null;
      }

      if (Core.IsInitialized())
      {
        return new Request<Models.AchievementProgressList>(
          CAPI.ovr_HTTP_GetWithMessageType(
            list.NextUrl,
            (int)Message.MessageType.Achievements_GetNextAchievementProgressArrayPage
          )
        );
      }

      return null;
    }

  }

  public static partial class CloudStorage {
    public static Request<Models.CloudStorageMetadataList> GetNextCloudStorageMetadataListPage(Models.CloudStorageMetadataList list) {
      if (!list.HasNextPage)
      {
        Debug.LogWarning("Oculus.Platform.GetNextCloudStorageMetadataListPage: List has no next page");
        return null;
      }

      if (Core.IsInitialized())
      {
        return new Request<Models.CloudStorageMetadataList>(
          CAPI.ovr_HTTP_GetWithMessageType(
            list.NextUrl,
            (int)Message.MessageType.CloudStorage_GetNextCloudStorageMetadataArrayPage
          )
        );
      }

      return null;
    }

  }

  public static partial class IAP {
    public static Request<Models.ProductList> GetNextProductListPage(Models.ProductList list) {
      if (!list.HasNextPage)
      {
        Debug.LogWarning("Oculus.Platform.GetNextProductListPage: List has no next page");
        return null;
      }

      if (Core.IsInitialized())
      {
        return new Request<Models.ProductList>(
          CAPI.ovr_HTTP_GetWithMessageType(
            list.NextUrl,
            (int)Message.MessageType.IAP_GetNextProductArrayPage
          )
        );
      }

      return null;
    }

    public static Request<Models.PurchaseList> GetNextPurchaseListPage(Models.PurchaseList list) {
      if (!list.HasNextPage)
      {
        Debug.LogWarning("Oculus.Platform.GetNextPurchaseListPage: List has no next page");
        return null;
      }

      if (Core.IsInitialized())
      {
        return new Request<Models.PurchaseList>(
          CAPI.ovr_HTTP_GetWithMessageType(
            list.NextUrl,
            (int)Message.MessageType.IAP_GetNextPurchaseArrayPage
          )
        );
      }

      return null;
    }

  }

  public static partial class Notifications {
    public static Request<Models.RoomInviteNotificationList> GetNextRoomInviteNotificationListPage(Models.RoomInviteNotificationList list) {
      if (!list.HasNextPage)
      {
        Debug.LogWarning("Oculus.Platform.GetNextRoomInviteNotificationListPage: List has no next page");
        return null;
      }

      if (Core.IsInitialized())
      {
        return new Request<Models.RoomInviteNotificationList>(
          CAPI.ovr_HTTP_GetWithMessageType(
            list.NextUrl,
            (int)Message.MessageType.Notification_GetNextRoomInviteNotificationArrayPage
          )
        );
      }

      return null;
    }

  }

  public static partial class Rooms {
    public static Request<Models.RoomList> GetNextRoomListPage(Models.RoomList list) {
      if (!list.HasNextPage)
      {
        Debug.LogWarning("Oculus.Platform.GetNextRoomListPage: List has no next page");
        return null;
      }

      if (Core.IsInitialized())
      {
        return new Request<Models.RoomList>(
          CAPI.ovr_HTTP_GetWithMessageType(
            list.NextUrl,
            (int)Message.MessageType.Room_GetNextRoomArrayPage
          )
        );
      }

      return null;
    }

  }

  public static partial class Users {
    public static Request<Models.UserAndRoomList> GetNextUserAndRoomListPage(Models.UserAndRoomList list) {
      if (!list.HasNextPage)
      {
        Debug.LogWarning("Oculus.Platform.GetNextUserAndRoomListPage: List has no next page");
        return null;
      }

      if (Core.IsInitialized())
      {
        return new Request<Models.UserAndRoomList>(
          CAPI.ovr_HTTP_GetWithMessageType(
            list.NextUrl,
            (int)Message.MessageType.User_GetNextUserAndRoomArrayPage
          )
        );
      }

      return null;
    }

    public static Request<Models.UserList> GetNextUserListPage(Models.UserList list) {
      if (!list.HasNextPage)
      {
        Debug.LogWarning("Oculus.Platform.GetNextUserListPage: List has no next page");
        return null;
      }

      if (Core.IsInitialized())
      {
        return new Request<Models.UserList>(
          CAPI.ovr_HTTP_GetWithMessageType(
            list.NextUrl,
            (int)Message.MessageType.User_GetNextUserArrayPage
          )
        );
      }

      return null;
    }

  }


}
