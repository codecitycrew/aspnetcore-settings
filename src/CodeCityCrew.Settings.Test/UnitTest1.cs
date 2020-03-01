using System;
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
        }

        [Test]
        public void Test1()
        {
            var mockSettingDbContext = new Mock<SettingDbContext>();

            mockSettingDbContext.Setup(context => context.SaveChanges());

            mockSettingDbContext.Setup(context => context.Settings.Add(It.IsAny<Setting>()));

            mockSettingDbContext.Setup(context =>
                    context.Settings.Find("CodeCityCrew.Settings.Test.MySetting", "Development"))
                .Returns(() => null);

            var mockIWebHostEnvironment = new Mock<IWebHostEnvironment>();

            mockIWebHostEnvironment.Setup(environment => environment.EnvironmentName).Returns("Development");

            var settingService = new SettingService(mockSettingDbContext.Object, mockIWebHostEnvironment.Object);

            var mySetting = settingService.As<MySetting>();

            mockSettingDbContext.Verify(
                context => context.Settings.Find("CodeCityCrew.Settings.Test.MySetting", "Development"), Times.Once);

            mockSettingDbContext.Verify(context => context.Settings.Add(It.IsAny<Setting>()), Times.Once);

            mockSettingDbContext.Verify(context => context.SaveChanges(), Times.Once);

            Assert.NotNull(mySetting);

            Assert.AreEqual("Application", mySetting.ApplicationName);

            Assert.AreEqual(DateTime.MaxValue, mySetting.CreatedDate);
        }

        [Test]
        public void Test2()
        {
            var mockSettingDbContext = new Mock<SettingDbContext>();

            mockSettingDbContext.Setup(context => context.SaveChanges());

            mockSettingDbContext.Setup(context => context.Settings.Add(It.IsAny<Setting>()));

            mockSettingDbContext.Setup(context =>
                    context.Settings.Find("CodeCityCrew.Settings.Test.MySetting", "Development"))
                .Returns(() => new Setting
                {
                    EnvironmentName = "Application",
                    Id = "CodeCityCrew.Settings.Test.MySetting",
                    AssemblyName = "CodeCityCrew.Settings.Test",
                    Value = "{\"ApplicationName\":\"Application\",\"CreatedDate\":\"9999-12-31T23:59:59.9999999\"}"
                });

            var mockIWebHostEnvironment = new Mock<IWebHostEnvironment>();

            mockIWebHostEnvironment.Setup(environment => environment.EnvironmentName).Returns("Development");

            var settingService = new SettingService(mockSettingDbContext.Object, mockIWebHostEnvironment.Object);

            var mySetting = settingService.As<MySetting>();

            mockSettingDbContext.Verify(
                context => context.Settings.Find("CodeCityCrew.Settings.Test.MySetting", "Development"), Times.Once);

            mockSettingDbContext.Verify(context => context.Settings.Add(It.IsAny<Setting>()), Times.Never);

            mockSettingDbContext.Verify(context => context.SaveChanges(), Times.Never);

            Assert.NotNull(mySetting);

            Assert.AreEqual("Application", mySetting.ApplicationName);

            Assert.AreEqual(DateTime.MaxValue, mySetting.CreatedDate);
        }

        [Test]
        public void Test3()
        {
            var mockSettingDbContext = new Mock<SettingDbContext>();

            mockSettingDbContext.Setup(context => context.SaveChanges());

            mockSettingDbContext.Setup(context => context.Settings.Add(It.IsAny<Setting>()));

            mockSettingDbContext.Setup(context =>
                    context.Settings.Find("CodeCityCrew.Settings.Test.MySetting", "Development"))
                .Returns(() => new Setting
                {
                    EnvironmentName = "Application", 
                    Id = "CodeCityCrew.Settings.Test.MySetting",
                    AssemblyName = "CodeCityCrew.Settings.Test",
                    Value = "{\"ApplicationName\":\"Application\",\"CreatedDate\":\"9999-12-31T23:59:59.9999999\"}"
                });

            var mockIWebHostEnvironment = new Mock<IWebHostEnvironment>();

            mockIWebHostEnvironment.Setup(environment => environment.EnvironmentName).Returns("Development");

            var settingService = new SettingService(mockSettingDbContext.Object, mockIWebHostEnvironment.Object);

            var mySetting = settingService.As("CodeCityCrew.Settings.Test.MySetting");

            mockSettingDbContext.Verify(
                context => context.Settings.Find("CodeCityCrew.Settings.Test.MySetting", "Development"), Times.Once);

            mockSettingDbContext.Verify(context => context.Settings.Add(It.IsAny<Setting>()), Times.Never);

            mockSettingDbContext.Verify(context => context.SaveChanges(), Times.Never);

            Assert.NotNull(mySetting);

            Assert.AreEqual("Application", (mySetting as MySetting).ApplicationName);

            Assert.AreEqual(DateTime.MaxValue, (mySetting as MySetting).CreatedDate);
        }

        [Test]
        public void Test4()
        {
            var x = new MySetting();

            var assemblyFullName = x.GetType().Assembly.GetName().Name;

            var assemblyName = x.GetType().Assembly.ExportedTypes;
        }
    }
}