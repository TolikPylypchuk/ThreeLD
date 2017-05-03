using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;
using System.Collections.Generic;
using System.Web.Mvc;

using ThreeLD.DB.Models;
using ThreeLD.DB.Repositories;
using ThreeLD.Web.Controllers;

namespace ThreeLD.Tests.User
{
    [TestClass]
    public class RemovePreferenceTests
    {
        [TestMethod]
        public void RemoveExistentPreferenceTest()
        {
            const int preferenceToRemoveId = 1;

            List<Preference> preferences = new List<Preference>
            {
                new Preference()
                {
                    Id = 1,
                    Category = "test",
                    UserId = "testUserId"
                }
            };

            Mock<IRepository<Preference>> mockRepository = 
                new Mock<IRepository<Preference>>();

            mockRepository.Setup(r => r.GetById(preferenceToRemoveId))
                .Returns(preferences[0]);
            mockRepository.Setup(r => r.Delete(preferenceToRemoveId))
                .Callback(() => 
                    preferences.RemoveAll(p => p.Id == preferenceToRemoveId));
            mockRepository.Setup(r => r.Save()).Returns(1);

            var controllerContext = new Mock<ControllerContext>();

            var controller =
                new UserController(mockRepository.Object, null)
                {
                    ControllerContext = controllerContext.Object
                };

            Assert.AreEqual(1, preferences.Count);

            var viewResult = controller.RemovePreference(preferenceToRemoveId);

            Assert.AreEqual(0, preferences.Count);
            Assert.IsNotNull(controller.TempData["message"]);
            Assert.IsNull(controller.TempData["error"]);
            
            mockRepository.Verify(
                r => r.Delete(preferenceToRemoveId), Times.Once());
            mockRepository.Verify(r => r.Save(), Times.Once());
        }

        [TestMethod]
        public void RemoveNonExistentPreferenceTest()
        {
            const int preferenceToRemoveId = 2;

            List<Preference> preferences = new List<Preference>
            {
                new Preference()
                {
                    Id = 1,
                    Category = "test",
                    UserId = "testUserId"
                }
            };

            Mock<IRepository<Preference>> mockRepository =
                new Mock<IRepository<Preference>>();

            mockRepository.Setup(r => r.GetById(preferenceToRemoveId))
                .Returns((Preference)null);
            mockRepository.Setup(r => r.Delete(preferenceToRemoveId))
                .Callback(() =>
                    preferences.RemoveAll(p => p.Id == preferenceToRemoveId));
            mockRepository.Setup(r => r.Save()).Returns(0);

            var controllerContext = new Mock<ControllerContext>();

            var controller =
                new UserController(mockRepository.Object, null)
                {
                    ControllerContext = controllerContext.Object
                };

            Assert.AreEqual(1, preferences.Count);

            var viewResult = controller.RemovePreference(preferenceToRemoveId);

            Assert.AreEqual(1, preferences.Count);
            Assert.IsNull(controller.TempData["message"]);
            Assert.IsNotNull(controller.TempData["error"]);
            
            mockRepository.Verify(
                r => r.Delete(preferenceToRemoveId), Times.Once());
            mockRepository.Verify(r => r.Save(), Times.Once());
        }
    }
}
