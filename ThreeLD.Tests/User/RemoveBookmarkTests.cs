using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web.Mvc;

using ThreeLD.DB.Infrastructure;
using ThreeLD.DB.Models;
using ThreeLD.DB.Repositories;
using ThreeLD.Web.Controllers;

namespace ThreeLD.Tests.User
{
    [TestClass]
    public class RemoveBookmarkTests
    {
        private Mock<IPrincipal> mockPrincipal;
        private string username = "test@example.com";

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
        public void RemoveNonExistentBookmarkTest()
        {
            const int eventId = 1;
            Event[] events = new Event[] { new Event() { Id = eventId } };

            DB.Models.User user = new DB.Models.User()
            {
                Id = this.username
            };

            var mockRepository = new Mock<IRepository<Event>>();
            mockRepository.Setup(r => r.GetById(It.IsAny<int>()))
                .Returns(events[0]);
            mockRepository.Setup(r => r.Save()).Returns(0);

            var userManager = new Mock<AppUserManager>(
                new UserStore<DB.Models.User>());
            userManager.Setup(m => m.FindByIdAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(user));

            var controllerContext = new Mock<ControllerContext>();
            controllerContext.SetupGet(x => x.HttpContext.User)
                .Returns(this.mockPrincipal.Object);

            var controller =
                new UserController(null, mockRepository.Object, null)
                {
                    ControllerContext = controllerContext.Object,
                    UserManager = userManager.Object
                };

            Assert.IsFalse(events[0].BookmarkedBy.Contains(user));
            Assert.IsFalse(user.BookmarkedEvents.Contains(events[0]));

            controller.RemoveBookmark(eventId, null);

            Assert.IsFalse(events[0].BookmarkedBy.Contains(user));
            Assert.IsFalse(user.BookmarkedEvents.Contains(events[0]));

            Assert.IsNull(controller.TempData["message"]);
            Assert.IsNotNull(controller.TempData["error"]);

            mockRepository.Verify(r => r.GetById(It.IsAny<int>()), Times.Once);
            mockRepository.Verify(r => r.Save(), Times.Once);
        }

        [TestMethod]
        public void RemoveExistentBookmarkTest()
        {
            const int eventId = 1;
            var eventToRemove = new Event()
            {
                Id = eventId,
                BookmarkedBy = new List<DB.Models.User>()
            };
            DB.Models.User user = new DB.Models.User()
            {
                Id = this.username,
                BookmarkedEvents = new List<Event>() { eventToRemove }
            };
            eventToRemove.BookmarkedBy.Add(user);

            Event[] events = new Event[] { eventToRemove };

            var mockRepository = new Mock<IRepository<Event>>();
            mockRepository.Setup(r => r.GetById(It.IsAny<int>()))
                .Returns(events[0]);
            mockRepository.Setup(r => r.Save()).Returns(1);

            var userManager = new Mock<AppUserManager>(
                new UserStore<DB.Models.User>());
            userManager.Setup(m => m.FindByIdAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(user));

            var controllerContext = new Mock<ControllerContext>();
            controllerContext.SetupGet(x => x.HttpContext.User)
                .Returns(this.mockPrincipal.Object);

            var controller =
                new UserController(null, mockRepository.Object, null)
                {
                    ControllerContext = controllerContext.Object,
                    UserManager = userManager.Object
                };

            Assert.IsTrue(events[0].BookmarkedBy.Contains(user));
            Assert.IsTrue(user.BookmarkedEvents.Contains(events[0]));

            controller.RemoveBookmark(eventId, null);

            Assert.IsFalse(events[0].BookmarkedBy.Contains(user));
            Assert.IsFalse(user.BookmarkedEvents.Contains(events[0]));

            Assert.IsNotNull(controller.TempData["message"]);
            Assert.IsNull(controller.TempData["error"]);

            mockRepository.Verify(r => r.GetById(It.IsAny<int>()), Times.Once);
            mockRepository.Verify(r => r.Save(), Times.Once);
        }
    }
}
