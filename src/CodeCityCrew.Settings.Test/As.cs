using System;
using CodeCityCrew.Settings.Model;
using Microsoft.AspNetCore.Hosting;
using Moq;
using NUnit.Framework;

namespace CodeCityCrew.Settings.Test
{
    public class As
    {
        public Mock<IWebHostEnvironment> MockIWebHostEnvironment { get; set; }

        public Mock<SettingDbContext> MockSettingDbContext { get; set; }

        [SetUp]
        public void Setup()
        {
            MockSettingDbContext = new Mock<SettingDbContext>();

            MockSettingDbContext.Setup(context => context.SaveChanges());

            MockSettingDbContext.Setup(context => context.Settings.Add(It.IsAny<Setting>()));

            MockIWebHostEnvironment = new Mock<IWebHostEnvironment>();

            MockIWebHostEnvironment.Setup(environment => environment.EnvironmentName).Returns("Development");
        }

        [Test]
        public void As_By_Namespace()
        {
            MockSettingDbContext.Setup(context =>
                    context.Settings.Find("CodeCityCrew.Settings.Test.MySetting", "Development"))
                .Returns(() => new Setting
                {
                    EnvironmentName = "Application",
                    Id = "CodeCityCrew.Settings.Test.MySetting",
                    AssemblyName = "CodeCityCrew.Settings.Test",
                    Value = "{\"ApplicationName\":\"Application\",\"CreatedDate\":\"9999-12-31T23:59:59.9999999\"}"
                });

            var settingService = new SettingService(MockSettingDbContext.Object, MockIWebHostEnvironment.Object);

            //action
            var mySetting = settingService.As("CodeCityCrew.Settings.Test.MySetting");

            MockSettingDbContext.Verify(
                context => context.Settings.Find("CodeCityCrew.Settings.Test.MySetting", "Development"), Times.Once);

            MockSettingDbContext.Verify(context => context.Settings.Add(It.IsAny<Setting>()), Times.Never);

            MockSettingDbContext.Verify(context => context.SaveChanges(), Times.Never);

            Assert.NotNull(mySetting);

            Assert.AreEqual("Application", (mySetting as MySetting)?.ApplicationName);

            Assert.AreEqual(DateTime.MaxValue, ((MySetting)mySetting).CreatedDate);
        }

        [Test]
        public void As_By_Namespace_Not_Found_In_Database()
        {
            MockSettingDbContext.Setup(context =>
                    context.Settings.Find("CodeCityCrew.Settings.Test.MySetting", "Development"))
                .Returns(() => null);

            var settingService = new SettingService(MockSettingDbContext.Object, MockIWebHostEnvironment.Object);

            var setting = settingService.As("CodeCityCrew.Settings.Test.MySetting") as MySetting;

            Assert.NotNull(setting);

            Assert.AreEqual("Application", setting.ApplicationName);

            Assert.AreEqual(DateTime.MaxValue, setting.CreatedDate);
        }
    }
}
