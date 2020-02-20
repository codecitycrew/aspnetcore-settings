using System;
using System.Collections.Generic;
using System.Text;

namespace CodeCityCrew.Settings.Test
{
    public class MySetting
    {
        public string ApplicationName { get; set; } = "Application";

        public DateTime CreatedDate { get; set; } = DateTime.MaxValue;
    }
}
