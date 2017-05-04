using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Web.Mvc;

using ThreeLD.DB.Models;
using ThreeLD.DB.Repositories;
using ThreeLD.Web.Controllers;
using ThreeLD.Web.Models.ViewModels;

namespace ThreeLD.Tests.User
{
    [TestClass]
    public class ViewNotificationsTests
    {
        private Mock<IPrincipal> mockPrincipal;
        private string username = "user";

        [TestInitialize]
        public void Init()
        {
            this.mockPrincipal = new Mock<IPrincipal>();
            var identity = new GenericIdentity(this.username);
            var nameIdentifierClaim = new Claim(
                ClaimTypes.NameIdentifier, this.username);
            identity.AddClaim(nameIdentifierClaim);

            this.mockPrincipal.Setup(x => x.Identity).Returns(identity);
        }

        [TestMethod]
        public void ViewEmptyNotificationsTest()
        {
            Mock<IRepository<Notification>> mockRepository =
                new Mock<IRepository<Notification>>();

            var controllerContext = new Mock<ControllerContext>();
            controllerContext.SetupGet(x => x.HttpContext.User)
                .Returns(this.mockPrincipal.Object);

            var controller =
                new UserController(null, null, mockRepository.Object)
                {
                    ControllerContext = controllerContext.Object
                };

            ViewResult result = controller.ViewNotifications();

            Assert.IsNotNull(result.Model);

            var notificationsModel = (NotificationsViewModel)result.Model;

            Assert.AreEqual(notificationsModel.ReadNotifications.Count, 0);
            Assert.AreEqual(notificationsModel.UnreadNotifications.Count, 0);

            foreach (var r in notificationsModel.ReadNotifications)
            {
                Assert.AreEqual(r.Message, string.Empty);
                Assert.AreEqual(r.IsRead, false);
                Assert.AreEqual(r.From, string.Empty);
                Assert.AreEqual(r.To, string.Empty);
            }

            foreach (var n in notificationsModel.UnreadNotifications)
            {
                Assert.AreEqual(n.Message, string.Empty);
                Assert.AreEqual(n.IsRead, false);
                Assert.AreEqual(n.From, string.Empty);
                Assert.AreEqual(n.To, string.Empty);
            }
        }

        [TestMethod]
        public void ViewOnlyReadNotificationsTest()
        {
            Mock<IRepository<Notification>> mockRepository =
                new Mock<IRepository<Notification>>();

            var controllerContext = new Mock<ControllerContext>();
            controllerContext.SetupGet(x => x.HttpContext.User)
                .Returns(this.mockPrincipal.Object);

            Notification n = new Notification();
            n.Message = "myNotification";
            n.IsRead = true;
            n.From = "editorId";
            n.To = "user";

            List<Notification> notifications = new List<Notification>();
            notifications.Add(n);

            mockRepository.Setup(r => r.GetAll())
                .Returns(notifications.AsQueryable);

            var controller =
                new UserController(null, null, mockRepository.Object)
                {
                    ControllerContext = controllerContext.Object
                };

            ViewResult result = controller.ViewNotifications();

            Assert.IsNotNull(result.Model);

            var notificationsModel = (NotificationsViewModel)result.Model;

            Assert.AreEqual(notificationsModel.ReadNotifications.Count, 1);

            foreach (var r in notificationsModel.ReadNotifications)
            {
                Assert.AreEqual(r.Message, "myNotification");
                Assert.AreEqual(r.IsRead, true);
                Assert.AreEqual(r.From, "editorId");
                Assert.AreEqual(r.To, "user");
            }

            Assert.AreEqual(notificationsModel.UnreadNotifications.Count, 0);

            foreach (var u in notificationsModel.UnreadNotifications)
            {
                Assert.AreEqual(u.Message, string.Empty);
                Assert.AreEqual(u.IsRead, false);
                Assert.AreEqual(u.From, string.Empty);
                Assert.AreEqual(u.To, string.Empty);
            }
        }

        [TestMethod]
        public void ViewOnlyUnreadNotificationsTest()
        {
            Mock<IRepository<Notification>> mockRepository =
                new Mock<IRepository<Notification>>();

            var controllerContext = new Mock<ControllerContext>();
            controllerContext.SetupGet(x => x.HttpContext.User)
                .Returns(this.mockPrincipal.Object);

            Notification n = new Notification();
            n.Message = "unreadNotification";
            n.IsRead = false;
            n.From = "editorId";
            n.To = "user";

            List<Notification> notifications = new List<Notification>();
            notifications.Add(n);

            mockRepository.Setup(r => r.GetAll())
                .Returns(notifications.AsQueryable);

            var controller =
                new UserController(null, null, mockRepository.Object)
                {
                    ControllerContext = controllerContext.Object
                };

            ViewResult result = controller.ViewNotifications();

            Assert.IsNotNull(result.Model);

            var notificationsModel = (NotificationsViewModel)result.Model;

            Assert.AreEqual(notificationsModel.ReadNotifications.Count, 0);

            foreach (var r in notificationsModel.ReadNotifications)
            {
                Assert.AreEqual(r.Message, string.Empty);
                Assert.AreEqual(r.IsRead, false);
                Assert.AreEqual(r.From, string.Empty);
                Assert.AreEqual(r.To, string.Empty);
            }

            Assert.AreEqual(notificationsModel.UnreadNotifications.Count, 1);

            foreach (var u in notificationsModel.UnreadNotifications)
            {
                Assert.AreEqual(u.Message, "unreadNotification");
                Assert.AreEqual(u.IsRead, false);
                Assert.AreEqual(u.From, "editorId");
                Assert.AreEqual(u.To, "user");
            }
        }

        [TestMethod]
        public void ViewAllNotificationsTest()
        {
            Mock<IRepository<Notification>> mockRepository =
                new Mock<IRepository<Notification>>();

            var controllerContext = new Mock<ControllerContext>();
            controllerContext.SetupGet(x => x.HttpContext.User)
                .Returns(this.mockPrincipal.Object);

            Notification first = new Notification();
            first.Message = "readNotification";
            first.IsRead = true;
            first.From = "editorId";
            first.To = "user";

            Notification second = new Notification();
            second.Message = "unreadNotification";
            second.IsRead = false;
            second.From = "editorId";
            second.To = "user";

            List<Notification> notifications = new List<Notification>();
            notifications.Add(first);
            notifications.Add(second);

            mockRepository.Setup(r => r.GetAll())
                .Returns(notifications.AsQueryable);

            var controller =
                new UserController(null, null, mockRepository.Object)
                {
                    ControllerContext = controllerContext.Object
                };

            ViewResult result = controller.ViewNotifications();

            Assert.IsNotNull(result.Model);

            var notificationsModel = (NotificationsViewModel)result.Model;

            Assert.AreEqual(notificationsModel.ReadNotifications.Count, 1);

            foreach (var r in notificationsModel.ReadNotifications)
            {
                Assert.AreEqual(r.Message, "readNotification");
                Assert.AreEqual(r.IsRead, true);
                Assert.AreEqual(r.From, "editorId");
                Assert.AreEqual(r.To, "user");
            }

            Assert.AreEqual(notificationsModel.UnreadNotifications.Count, 1);

            foreach (var u in notificationsModel.UnreadNotifications)
            {
                Assert.AreEqual(u.Message, "unreadNotification");
                Assert.AreEqual(u.IsRead, false);
                Assert.AreEqual(u.From, "editorId");
                Assert.AreEqual(u.To, "user");
            }
        }
    }
}
