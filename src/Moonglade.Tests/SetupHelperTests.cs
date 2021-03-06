﻿using System.Data;
using System.Diagnostics.CodeAnalysis;
using Dapper;
using Moonglade.Setup;
using Moq;
using Moq.Dapper;
using NUnit.Framework;

namespace Moonglade.Tests
{
    [TestFixture]
    [ExcludeFromCodeCoverage]
    public class SetupHelperTests
    {
        private MockRepository mockRepository;

        private Mock<IDbConnection> mockDbConnection;

        [SetUp]
        public void SetUp()
        {
            mockRepository = new(MockBehavior.Default);
            mockDbConnection = mockRepository.Create<IDbConnection>();
        }

        [Test]
        public void IsFirstRun_Yes()
        {
            mockDbConnection.SetupDapper(c => c.ExecuteScalar<int>(It.IsAny<string>(), null, null, null, null)).Returns(0);
            var setupHelper = new SetupRunner(mockDbConnection.Object);

            var result = setupHelper.IsFirstRun();
            Assert.IsTrue(result);
        }

        [Test]
        public void IsFirstRun_No()
        {
            mockDbConnection.SetupDapper(c => c.ExecuteScalar<int>(It.IsAny<string>(), null, null, null, null)).Returns(1);
            var setupHelper = new SetupRunner(mockDbConnection.Object);

            var result = setupHelper.IsFirstRun();
            Assert.IsFalse(result);
        }

        [Test]
        public void SetupDatabase_OK()
        {
            mockDbConnection.SetupDapper(c => c.Execute(It.IsAny<string>(), null, null, null, null)).Returns(996);
            var setupHelper = new SetupRunner(mockDbConnection.Object);

            Assert.DoesNotThrow(() =>
            {
                setupHelper.SetupDatabase();
            });
        }

        [Test]
        public void ClearData_OK()
        {
            mockDbConnection.SetupDapper(c => c.Execute(It.IsAny<string>(), null, null, null, null)).Returns(251);
            var setupHelper = new SetupRunner(mockDbConnection.Object);

            Assert.DoesNotThrow(() =>
            {
                setupHelper.ClearData();
            });
        }

        [Test]
        public void ResetDefaultConfiguration_OK()
        {
            mockDbConnection.SetupDapper(c => c.Execute(It.IsAny<string>(), null, null, null, null)).Returns(251);
            var setupHelper = new SetupRunner(mockDbConnection.Object);

            Assert.DoesNotThrow(() =>
            {
                setupHelper.ResetDefaultConfiguration();
            });
        }

        [Test]
        public void InitSampleData_OK()
        {
            mockDbConnection.SetupDapper(c => c.Execute(It.IsAny<string>(), null, null, null, null)).Returns(251);
            var setupHelper = new SetupRunner(mockDbConnection.Object);

            Assert.DoesNotThrow(() =>
            {
                setupHelper.InitSampleData();
            });
        }

        [Test]
        public void InitFirstRun_OK()
        {
            mockDbConnection.SetupDapper(c => c.Execute(It.IsAny<string>(), null, null, null, null)).Returns(251);
            var setupHelper = new SetupRunner(mockDbConnection.Object);

            Assert.DoesNotThrow(() =>
            {
                setupHelper.InitFirstRun();
            });
        }
    }
}
