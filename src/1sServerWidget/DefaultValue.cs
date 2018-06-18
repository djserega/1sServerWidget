using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _1sServerWidget
{
    internal class DefaultValue
    {
        private readonly string _prefixRegistrySoftware;
        private readonly string _prefixKeyApplication;
        private readonly string _prefixRegistryKey;

        private RegistryKey _registryKeyApplication;
        private RegistryKey _registryKeyApplicationValues;
        private readonly bool _initialize;

        internal string ServerName { get; set; }
        internal int MinUpdateSession { get; set; }

        internal DefaultValue()
        {
            _initialize = true;

            _prefixRegistrySoftware = "Software";
            _prefixKeyApplication = "1sServerWidged";
            _prefixRegistryKey = _prefixRegistrySoftware + "\\" + _prefixKeyApplication;

            ServerName = string.Empty;

            try
            {
                GetDefaultSettings();
            }
            catch (RegistrykeyNotFoundException)
            {
                SetDefaultSettings();
            }

            _initialize = false;
        }

        internal void GetDefaultSettings()
        {
            try
            {
                using (RegistryKey currentUser = Registry.CurrentUser)
                {
                    using (RegistryKey registryKeyApplication = currentUser.OpenSubKey(_prefixRegistryKey))
                    {
                        _registryKeyApplication = registryKeyApplication;

                        ServerName = GetValue("ServerName");
                        int.TryParse(GetValue("MinUpdateSession"), out int minUpdateSession);
                        MinUpdateSession = minUpdateSession;
                    }
                }
            }
            catch (Exception)
            {
                throw new RegistrykeyNotFoundException("Не удалось получить настройки.");
            }
        }

        internal void SetDefaultSettings(string key = "")
        {
            bool keyEmpty = string.IsNullOrWhiteSpace(key);

            using (RegistryKey currentUser = Registry.CurrentUser)
            {
                using (RegistryKey registryKeyApplication = currentUser.OpenSubKey(_prefixRegistrySoftware, true))
                {
                    if (registryKeyApplication == null)
                        _registryKeyApplication = currentUser.CreateSubKey(_prefixRegistrySoftware);
                    else
                        _registryKeyApplication = registryKeyApplication;

                    using (RegistryKey registryKeyApplicationValues = registryKeyApplication.OpenSubKey(_prefixRegistryKey, true))
                    {
                        if (registryKeyApplicationValues == null)
                            _registryKeyApplicationValues = registryKeyApplication.CreateSubKey(_prefixKeyApplication);
                        else
                            _registryKeyApplicationValues = registryKeyApplicationValues;

                        string[] names = _registryKeyApplicationValues.GetValueNames();

                        if (keyEmpty || key == "ServerName")
                            SetValueIfNotFinded(names, "ServerName", ServerName);
                        if (keyEmpty || key == "MinUpdateSession")
                            SetValueIfNotFinded(names, "MinUpdateSession", MinUpdateSession);
                    }
                }
            }
        }

        internal void SetValueByKey(string key)
        {
                switch (key)
            {
                case "ServerName":
                    SetDefaultSettings("ServerName");
                    break;
                case "MinUpdateSession":
                    SetDefaultSettings("MinUpdateSession");
                    break;
                default:
                    throw new ArgumentException("Не удалось найти свойство.", "Имя ключа");
            }
        }

        private string GetValue(string keyName) => _registryKeyApplication.GetValue(keyName).ToString();

        private void SetValue(string key, string value) => _registryKeyApplicationValues.SetValue(key, value);

        private void SetValueIfNotFinded(string[] names, string key, string value = "")
        {
            if (!string.IsNullOrWhiteSpace(names.FirstOrDefault(f => f == key)) || _initialize)
                SetValue(key, value);
        }
        private void SetValueIfNotFinded(string[] names, string key, int value = 0)
        {
            if (!string.IsNullOrWhiteSpace(names.FirstOrDefault(f => f == key)) || _initialize)
                SetValue(key, value.ToString());
        }
    }
}
