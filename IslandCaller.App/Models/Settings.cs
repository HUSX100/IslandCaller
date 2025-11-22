using Microsoft.Win32;
using System.Text.Json;
using System.Windows;

namespace IslandCaller.App.Models
{
    public class Settings
    {
        public static SettingsModel Instance { get; } = new SettingsModel();

        public void Load()
        {
            RegistryKey IsC_RootKey = Registry.CurrentUser.OpenSubKey(@"Software\IslandCaller", writable: true);
            RegistryKey IsC_GeneralKey;
            RegistryKey IsC_ProfileKey;
            RegistryKey IsC_HoverKey;
            RegistryKey IsC_SecurityKey;
            RegistryKey IsC_HoverKey_Position;
            RegistryKey IsC_SecurityKey_SecretKey;

            if (IsC_RootKey == null)
            {
                IsC_RootKey = Registry.CurrentUser.CreateSubKey(@"Software\IslandCaller", writable: true);
                IsC_GeneralKey = IsC_RootKey?.CreateSubKey("General", writable: true);
                IsC_ProfileKey = IsC_RootKey?.CreateSubKey("Profile", writable: true);
                IsC_HoverKey = IsC_RootKey?.CreateSubKey("Hover", writable: true);
                IsC_SecurityKey = IsC_RootKey?.CreateSubKey("Security", writable: true);
                IsC_HoverKey_Position = IsC_HoverKey?.CreateSubKey("Position", writable: true);
                IsC_SecurityKey_SecretKey = IsC_SecurityKey?.CreateSubKey("SecretKey", writable: true);

                IsC_GeneralKey?.SetValue("BreakDisable", Instance.General.BreakDisable);
                IsC_ProfileKey?.SetValue("ProfileNum", Instance.Profile.ProfileNum);
                IsC_ProfileKey?.SetValue("DefaultProfileName", Instance.Profile.DefaultProfile.ToString());
                IsC_ProfileKey?.SetValue("IsPreferProfile", Instance.Profile.IsPreferProfile);
                IsC_ProfileKey?.SetValue("ProfileList", JsonSerializer.Serialize(Instance.Profile.ProfileList));
                IsC_ProfileKey?.SetValue("PreferProfile", JsonSerializer.Serialize(Instance.Profile.ProfilePrefer));
                IsC_HoverKey?.SetValue("IsEnable", Instance.Hover.IsEnable);
                IsC_HoverKey_Position?.SetValue("X", Instance.Hover.Position.X);
                IsC_HoverKey_Position?.SetValue("Y", Instance.Hover.Position.Y);
                IsC_SecurityKey?.SetValue("EncryptionMode", Instance.Security.EncryptionMode);
                IsC_SecurityKey?.SetValue("IsLockOn", Instance.Security.IsLockOn);
                IsC_SecurityKey_SecretKey?.SetValue("AESKey", Instance.Security.SecretKey.AESKey);
                IsC_SecurityKey_SecretKey?.SetValue("TOTPKey", Instance.Security.SecretKey.TOTPKey);
                IsC_SecurityKey_SecretKey?.SetValue("Passkey", Instance.Security.SecretKey.Passkey);
                IsC_SecurityKey_SecretKey?.SetValue("Password", Instance.Security.SecretKey.Password);
                IsC_SecurityKey_SecretKey?.SetValue("USBKey", Instance.Security.SecretKey.USBKey);
                ProfileProcess.CreateDemoCsv();
                MessageBox.Show("欢迎使用 IslandCaller！");
            }
            else
            {
                IsC_GeneralKey = IsC_RootKey?.OpenSubKey("General", writable: true);
                IsC_ProfileKey = IsC_RootKey?.OpenSubKey("Profile", writable: true);
                IsC_HoverKey = IsC_RootKey?.OpenSubKey("Hover", writable: true);
                IsC_SecurityKey = IsC_RootKey?.OpenSubKey("Security", writable: true);
                IsC_HoverKey_Position = IsC_HoverKey?.OpenSubKey("Position", writable: true);
                IsC_SecurityKey_SecretKey = IsC_SecurityKey?.OpenSubKey("SecretKey", writable: true);

                Instance.General.BreakDisable = Convert.ToBoolean(IsC_GeneralKey?.GetValue("BreakDisable") ?? false);
                Instance.Profile.ProfileNum = Convert.ToInt32(IsC_ProfileKey?.GetValue("ProfileNum") ?? 1);
                Instance.Profile.DefaultProfile = Guid.Parse(IsC_ProfileKey?.GetValue("DefaultProfileName") as string ?? Guid.Empty.ToString());
                Instance.Profile.IsPreferProfile = Convert.ToBoolean(IsC_ProfileKey?.GetValue("IsPreferProfile") ?? false);
                Instance.Profile.ProfileList = JsonSerializer.Deserialize<Dictionary<Guid, string>>(IsC_ProfileKey?.GetValue("ProfileList") as string ?? string.Empty);
                Instance.Profile.ProfilePrefer = JsonSerializer.Deserialize<Dictionary<Guid, string>>(IsC_ProfileKey?.GetValue("PreferProfile") as string ?? string.Empty);
                Instance.Hover.IsEnable = Convert.ToBoolean(IsC_HoverKey?.GetValue("IsEnable") ?? true);
                Instance.Hover.Position.X = Convert.ToDouble(IsC_HoverKey_Position?.GetValue("X") ?? 200.0);
                Instance.Hover.Position.Y = Convert.ToDouble(IsC_HoverKey_Position?.GetValue("Y") ?? 200.0);
                Instance.Security.EncryptionMode = Convert.ToInt32(IsC_SecurityKey?.GetValue("EncryptionMode") ?? 0);
                Instance.Security.IsLockOn = Convert.ToBoolean(IsC_SecurityKey?.GetValue("IsLockOn") ?? false);
                Instance.Security.SecretKey.AESKey = IsC_SecurityKey_SecretKey?.GetValue("AESKey") as string ?? string.Empty;
                Instance.Security.SecretKey.TOTPKey = IsC_SecurityKey_SecretKey?.GetValue("TOTPKey") as string ?? string.Empty;
                Instance.Security.SecretKey.Passkey = IsC_SecurityKey_SecretKey?.GetValue("Passkey") as string ?? string.Empty;
                Instance.Security.SecretKey.Password = IsC_SecurityKey_SecretKey?.GetValue("Password") as string ?? string.Empty;
                Instance.Security.SecretKey.USBKey = IsC_SecurityKey_SecretKey?.GetValue("USBKey") as string ?? string.Empty;
            }

            SettingsBinder.Bind(Instance, Save);
        }

        public void Save()
        {
            RegistryKey IsC_RootKey = Registry.CurrentUser.OpenSubKey(@"Software\IslandCaller", writable: true);
            RegistryKey IsC_GeneralKey = IsC_RootKey?.OpenSubKey("General", writable: true);
            RegistryKey IsC_ProfileKey = IsC_RootKey?.OpenSubKey("Profile", writable: true);
            RegistryKey IsC_HoverKey = IsC_RootKey?.OpenSubKey("Hover", writable: true);
            RegistryKey IsC_HoverKey_Position = IsC_HoverKey?.OpenSubKey("Position", writable: true);
            RegistryKey IsC_SecurityKey = IsC_RootKey?.OpenSubKey("Security", writable: true);
            RegistryKey IsC_SecurityKey_SecretKey = IsC_SecurityKey?.OpenSubKey("SecretKey", writable: true);

            IsC_GeneralKey?.SetValue("BreakDisable", Instance.General.BreakDisable);
            IsC_ProfileKey?.SetValue("ProfileNum", Instance.Profile.ProfileNum);
            IsC_ProfileKey?.SetValue("DefaultProfileName", Instance.Profile.DefaultProfile.ToString());
            IsC_ProfileKey?.SetValue("IsPreferProfile", Instance.Profile.IsPreferProfile);
            IsC_ProfileKey?.SetValue("ProfileList", JsonSerializer.Serialize(Instance.Profile.ProfileList));
            IsC_ProfileKey?.SetValue("PreferProfile", JsonSerializer.Serialize(Instance.Profile.ProfilePrefer));
            IsC_HoverKey?.SetValue("IsEnable", Instance.Hover.IsEnable);
            IsC_HoverKey_Position?.SetValue("X", Instance.Hover.Position.X);
            IsC_HoverKey_Position?.SetValue("Y", Instance.Hover.Position.Y);
            IsC_SecurityKey?.SetValue("EncryptionMode", Instance.Security.EncryptionMode);
            IsC_SecurityKey?.SetValue("IsLockOn", Instance.Security.IsLockOn);
        }
    }
    public static class SettingsBinder
    {
        public static void Bind(SettingsModel model, Action onChange)
        {
            // General
            model.General.PropertyChanged += (_, _) => onChange();

            // Hover
            model.Hover.PropertyChanged += (_, _) => onChange();
            model.Hover.Position.PropertyChanged += (_, _) => onChange();

            // Security
            model.Security.PropertyChanged += (_, _) => onChange();
            model.Security.SecretKey.PropertyChanged += (_, _) => onChange();
        }
    }

}