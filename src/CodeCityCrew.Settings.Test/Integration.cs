using System;
using CodeCityCrew.Settings.Abstractions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;

namespace CodeCityCrew.Settings.Test
{
    public class Integration
    {
        public DbContextOptions<SettingDbContext> Options { get; set; }

        public IWebHostEnvironment WebHostEnvironment { get; set; }

        public ISettingService SettingService { get; set; }

        public SettingDbContext SettingDbContext { get; set; }

        [SetUp]
        public void Setup()
        {
            Options = new DbContextOptionsBuilder<SettingDbContext>().UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;

            var mock = new Mock<IWebHostEnvironment>();

            mock.Setup(environment => environment.EnvironmentName).Returns("Development");

            WebHostEnvironment = mock.Object;

            SettingDbContext = new SettingDbContext(Options);

            SettingService = new SettingService(SettingDbContext, WebHostEnvironment);
        }

        [Test]
        public void Accessing_same_setting_twice()
        {
            // creates new entry
            var setting = SettingService.As<MySetting>();

            setting.ApplicationName = "NewName";

            SettingService.Save(setting);

            var find = SettingDbContext.Settings.Find("CodeCityCrew.Settings.Test.MySetting", "Development");

            Assert.IsNotNull(find);

            // get from dictionary.
            var mySetting1 = SettingService.As<MySetting>();

            Assert.AreEqual("NewName", mySetting1.ApplicationName);
        }

        [Test]
        public void Should_return_object()
        {
            // creates new entry
            var setting = SettingService.As<MySetting>();

            Assert.NotNull(setting);
        }

        [Test]
        public void Should_return_null()
        {
            SettingService.Save((MySetting) null);

            var mySetting = SettingService.As<MySetting>();

            Assert.IsNull(mySetting);
        }
    }
}
