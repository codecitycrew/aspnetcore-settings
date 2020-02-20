using CodeCityCrew.Settings.Abstractions;
using CodeCityCrew.Settings.Model;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;

namespace CodeCityCrew.Settings
{
    public class SettingService : ISettingService
    {
        /// <summary>
        /// The application database context.
        /// </summary>
        private readonly SettingDbContext _settingsDbContext;

        /// <summary>
        /// The environment name
        /// </summary>
        private readonly string _environmentName;

        /// <summary>
        /// The dictionary
        /// </summary>
        private readonly ConcurrentDictionary<string, string> _settingsDictionary;

        public SettingService(SettingDbContext applicationDbContext, IWebHostEnvironment webHostEnvironment)
        {
            this._settingsDbContext = applicationDbContext;
            _environmentName = webHostEnvironment.EnvironmentName;
            _settingsDictionary = new ConcurrentDictionary<string, string>();
        }

        public T As<T>() where T : new()
        {
            var id = typeof(T).FullName;

            if (id == null)
            {
                throw new InvalidOperationException();
            }

            if (_settingsDictionary.TryGetValue(id, out var value))
            {
                return JsonConvert.DeserializeObject<T>(value);
            }

            var setting = _settingsDbContext.Settings.Find(id, _environmentName);

            if (setting == null)
            {
                setting = new Setting()
                {
                    Id = id,
                    Value = JsonConvert.SerializeObject(new T()),
                    EnvironmentName = _environmentName
                };

                _settingsDictionary.TryAdd(setting.Id, setting.Value);

                _settingsDbContext.Settings.Add(setting);

                _settingsDbContext.SaveChanges();
            }
            else
            {
                _settingsDictionary.TryAdd(id, setting.Value);
            }

            return JsonConvert.DeserializeObject<T>(setting.Value);
        }

        public void Save<T>(T value)
        {
            var id = typeof(T).FullName;

            //get from db.
            var setting = _settingsDbContext.Settings.Find(id, _environmentName);

            //not found.
            if (setting == null)
            {
                setting = new Setting
                {
                    Id = id,
                    Value = JsonConvert.SerializeObject(value),
                    EnvironmentName = _environmentName
                };

                _settingsDictionary.TryAdd(setting.Id, setting.Value);

                _settingsDbContext.Settings.Add(setting);
            }
            else
            {
                var newValue = JsonConvert.SerializeObject(value);

                _settingsDictionary.AddOrUpdate(id, arg => setting.Value, (arg1, arg2) => newValue);

                setting.Value = newValue;
            }

            _settingsDbContext.SaveChanges();
        }
    }
}
