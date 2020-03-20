using System;
using System.Collections.Concurrent;
using CodeCityCrew.Settings.Abstractions;
using CodeCityCrew.Settings.Model;
using Microsoft.AspNetCore.Hosting;
using Moq;
using NUnit.Framework;

namespace CodeCityCrew.Settings.Test
{
    public class Tests
    {
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
        public void As_By_T_Not_Found()
        {
            MockSettingDbContext.Setup(context =>
                    context.Settings.Find("CodeCityCrew.Settings.Test.MySetting", "Development"))
                .Returns(() => null);

            var settingService = new SettingService(MockSettingDbContext.Object, MockIWebHostEnvironment.Object);

            var mySetting = settingService.As<MySetting>();

            MockSettingDbContext.Verify(
                context => context.Settings.Find("CodeCityCrew.Settings.Test.MySetting", "Development"), Times.Once);

            MockSettingDbContext.Verify(context => context.Settings.Add(It.IsAny<Setting>()), Times.Once);

            MockSettingDbContext.Verify(context => context.SaveChanges(), Times.Once);

            Assert.NotNull(mySetting);

            Assert.AreEqual("Application", mySetting.ApplicationName);

            Assert.AreEqual(DateTime.MaxValue, mySetting.CreatedDate);
        }

        public Mock<IWebHostEnvironment> MockIWebHostEnvironment { get; set; }

        public Mock<SettingDbContext> MockSettingDbContext { get; set; }

        [Test]
        public void As_By_T()
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
            var mySetting = settingService.As<MySetting>();

            MockSettingDbContext.Verify(
                context => context.Settings.Find("CodeCityCrew.Settings.Test.MySetting", "Development"), Times.Once);

            MockSettingDbContext.Verify(context => context.Settings.Add(It.IsAny<Setting>()), Times.Never);

            MockSettingDbContext.Verify(context => context.SaveChanges(), Times.Never);

            Assert.NotNull(mySetting);

            Assert.AreEqual("Application", mySetting.ApplicationName);

            Assert.AreEqual(DateTime.MaxValue, mySetting.CreatedDate);
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

            Assert.AreEqual("Application", (mySetting as MySetting).ApplicationName);

            Assert.AreEqual(DateTime.MaxValue, (mySetting as MySetting).CreatedDate);
        }


        [Test]
        public void As_By_T_Always_Get_From_Db()
        {
            MockSettingDbContext.Setup(context =>
                    context.Settings.Find("CodeCityCrew.Settings.Test.MySettingNeverCached", "Development"))
                .Returns(() => new Setting
                {
                    EnvironmentName = "Application",
                    Id = "CodeCityCrew.Settings.Test.MySettingNeverCached",
                    AssemblyName = "CodeCityCrew.Settings.Test",
                    Value = "{\"ApplicationName\":\"Application\",\"CreatedDate\":\"9999-12-31T23:59:59.9999999\"}"
                });

            var settingService = new SettingService(MockSettingDbContext.Object, MockIWebHostEnvironment.Object);

            //action
            var mySetting = settingService.As<MySettingNeverCached>();

            MockSettingDbContext.Verify(
                context => context.Settings.Find("CodeCityCrew.Settings.Test.MySettingNeverCached", "Development"), Times.Once);

            MockSettingDbContext.Verify(context => context.Settings.Add(It.IsAny<Setting>()), Times.Never);

            MockSettingDbContext.Verify(context => context.SaveChanges(), Times.Never);

            Assert.NotNull(mySetting);

            Assert.AreEqual("Application", (mySetting as MySettingNeverCached).ApplicationName);

            Assert.AreEqual(DateTime.MaxValue, (mySetting as MySettingNeverCached).CreatedDate);
        }

        [Test]
        public void Force_Reload()
        {
            var setting1 = new Setting
            {
                EnvironmentName = "Application",
                Id = "CodeCityCrew.Settings.Test.MySettingNeverCached",
                AssemblyName = "CodeCityCrew.Settings.Test",
                Value = "{\"ApplicationName\":\"\",\"CreatedDate\":\"9999-12-31T23:59:59.9999999\"}"
            };

            var setting2 = new Setting
            {
                EnvironmentName = "Application",
                Id = "CodeCityCrew.Settings.Test.MySettingNeverCached",
                AssemblyName = "CodeCityCrew.Settings.Test",
                Value = "{\"ApplicationName\":\"Application\",\"CreatedDate\":\"9999-12-31T23:59:59.9999999\"}"
            };

            MockSettingDbContext.SetupSequence(context =>
                    context.Settings.Find("CodeCityCrew.Settings.Test.MySettingNeverCached", "Development"))
                .Returns(() => setting1).Returns(setting2);

            var settingService = new SettingService(MockSettingDbContext.Object, MockIWebHostEnvironment.Object);

            //action
            var mySetting1 = settingService.As<MySettingNeverCached>();

            Assert.AreEqual(string.Empty, mySetting1.ApplicationName);

            Assert.AreEqual(DateTime.MaxValue, mySetting1.CreatedDate);

            Assert.NotNull(mySetting1);

            var mySetting2 = settingService.As<MySettingNeverCached>(true);

            Assert.AreEqual("Application", mySetting2.ApplicationName);

            Assert.AreEqual(DateTime.MaxValue, mySetting2.CreatedDate);

            Assert.NotNull(mySetting2);

            MockSettingDbContext.Verify(
                context => context.Settings.Find("CodeCityCrew.Settings.Test.MySettingNeverCached", "Development"), Times.Exactly(2));

            MockSettingDbContext.Verify(context => context.Settings.Add(It.IsAny<Setting>()), Times.Never);

            MockSettingDbContext.Verify(context => context.SaveChanges(), Times.Never);

            

            
        }

        [Test]
        public void Dictionary()
        {
            var dictionary = new ConcurrentDictionary<string, string>();

            dictionary.TryAdd("1", "1_value");

            dictionary.AddOrUpdate("2", "2_value", (s, s1) => "2_value");

            dictionary.AddOrUpdate("1", "2_value", (s, s1) => "2_value");
        }
    }
}