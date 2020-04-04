using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Runtime.Loader;
using CodeCityCrew.Settings.Abstractions;
using CodeCityCrew.Settings.Model;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;

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
            _settingsDbContext = applicationDbContext;
            _environmentName = webHostEnvironment.EnvironmentName;
            _settingsDictionary = new ConcurrentDictionary<string, string>();
        }

        public T As<T>(bool forceReload = false) where T : new()
        {
            var key = typeof(T).FullName;

            if (key == null)
            {
                throw new InvalidOperationException();
            }

            // if force reload is false, try get setting from dictionary otherwise try to find it in the database.
            if (!forceReload && _settingsDictionary.TryGetValue(key, out var value))
            {
                return JsonConvert.DeserializeObject<T>(value);
            }

            var setting = _settingsDbContext.Settings.Find(key, _environmentName);

            if (setting == null)
            {
                setting = Create<T>(JsonConvert.SerializeObject(new T()));

                _settingsDbContext.Settings.Add(setting);

                _settingsDbContext.SaveChanges();
            }

            _settingsDictionary.AddOrUpdate(key, setting.Value, (s, s1) => setting.Value);

            return JsonConvert.DeserializeObject<T>(setting.Value);
        }

        public object As(string id)
        {
            var assembly = AssemblyLoadContext.Default.Assemblies.FirstOrDefault(assembly1 =>
                assembly1.DefinedTypes.Any(info => info.FullName == id));

            if (assembly == null)
            {
                return null;
            }

            if (_settingsDictionary.TryGetValue(id, out var value))
            {
                return JsonConvert.DeserializeObject(value);
            }

            var setting = _settingsDbContext.Settings.Find(id, _environmentName);

            if (setting == null)
            {
                var instance = Activator.CreateInstanceFrom(assembly.Location, id);

                var unwrap = instance.Unwrap();

                var serializeObject = JsonConvert.SerializeObject(unwrap);

                setting = new Setting { Id = id, AssemblyName = assembly .GetName().Name, EnvironmentName = _environmentName, Value = serializeObject };

                _settingsDbContext.Settings.Add(setting);

                _settingsDbContext.SaveChanges();
            }

            _settingsDictionary.AddOrUpdate(id, setting.Value, (s, s1) => setting.Value);

            var type = assembly.DefinedTypes.FirstOrDefault(info => info.FullName == id);

            return JsonConvert.DeserializeObject(setting.Value, type);
        }

        public void Save<T>(T value)
        {
            var id = typeof(T).FullName;

            //get from db.
            var setting = _settingsDbContext.Settings.Find(id, _environmentName);

            //not found.
            if (setting == null)
            {
                setting = Create<T>(JsonConvert.SerializeObject(value));

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

        private Setting Create<T>(string value)
        {
            var type = typeof(T);

            return new Setting
            {
                Id = type.FullName,
                Value = value,
                EnvironmentName = _environmentName,
                AssemblyName = type.Assembly.GetName().Name
            };
        }
    }
}
