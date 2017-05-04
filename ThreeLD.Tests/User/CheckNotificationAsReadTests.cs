using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;
using System.Web.Mvc;

using ThreeLD.DB.Models;
using ThreeLD.DB.Repositories;
using ThreeLD.Web.Controllers;

namespace ThreeLD.Tests.User
{
    [TestClass]
    public class CheckNotificationAsReadTests
    {
        private const int notificationId = 1;

        [TestMethod]
        public void NonExistentNotificationTest()
        {
            var mockRepository = new Mock<IRepository<Notification>>();
            mockRepository.Setup(r => r.GetById(notificationId))
                .Returns((Notification)null);

            var controllerContext = new Mock<ControllerContext>();

            var controller =
                new UserController(null, null, mockRepository.Object)
                {
                    ControllerContext = controllerContext.Object
                };

            controller.CheckNotificationAsRead(notificationId);

            Assert.IsNull(controller.TempData["message"]);
            Assert.IsNotNull(controller.TempData["error"]);

            mockRepository.Verify(
                r => r.GetById(notificationId), Times.Once());
            mockRepository.Verify(r => r.Save(), Times.Never());
        }

        [TestMethod]
        public void AlreadyReadNotificationTest()
        {
            var notification = new Notification()
            {
                Id = notificationId,
                IsRead = true
            };

            var mockRepository = new Mock<IRepository<Notification>>();
            mockRepository.Setup(r => r.GetById(notificationId))
                .Returns(notification);

            var controllerContext = new Mock<ControllerContext>();

            var controller =
                new UserController(null, null, mockRepository.Object)
                {
                    ControllerContext = controllerContext.Object
                };

            controller.CheckNotificationAsRead(notificationId);

            Assert.IsNull(controller.TempData["message"]);
            Assert.IsNotNull(controller.TempData["error"]);

            mockRepository.Verify(
                r => r.GetById(notificationId), Times.Once());
            mockRepository.Verify(r => r.Save(), Times.Never());
        }

        [TestMethod]
        public void UnreadNotificationTest()
        {
            var notification = new Notification()
            {
                Id = notificationId,
                IsRead = false
            };

            var mockRepository = new Mock<IRepository<Notification>>();
            mockRepository.Setup(r => r.GetById(notificationId))
                .Returns(notification);

            var controllerContext = new Mock<ControllerContext>();

            var controller =
                new UserController(null, null, mockRepository.Object)
                {
                    ControllerContext = controllerContext.Object
                };

            controller.CheckNotificationAsRead(notificationId);

            Assert.IsTrue(notification.IsRead);
            Assert.IsNotNull(controller.TempData["message"]);
            Assert.IsNull(controller.TempData["error"]);

            mockRepository.Verify(
                r => r.GetById(notificationId), Times.Once());
            mockRepository.Verify(r => r.Save(), Times.Once());
        }
    }
}
