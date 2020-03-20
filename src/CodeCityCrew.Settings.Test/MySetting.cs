using System;

namespace CodeCityCrew.Settings.Test
{
    public class MySetting
    {
        public string ApplicationName { get; set; } = "Application";

        public DateTime CreatedDate { get; set; } = DateTime.MaxValue;
    }

    public class MySettingNeverCached
    {
        public string ApplicationName { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
