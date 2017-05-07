using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web.Mvc;

using ThreeLD.DB.Infrastructure;
using ThreeLD.DB.Models;
using ThreeLD.DB.Repositories;
using ThreeLD.Web.Controllers;
using ThreeLD.Web.Models.ViewModels;

namespace ThreeLD.Tests.User
{
    [TestClass]
    public class ViewEventsTests
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
        public void ViewEventsNoApprovedEventsTest()
        {
            Event[] events = new Event[] { };

            DB.Models.User user = new DB.Models.User()
            {
                Id = this.username
            };

            var mockRepository = new Mock<IRepository<Event>>();
            mockRepository.Setup(r => r.GetAll()).Returns(events.AsQueryable);
            
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
            
            var viewResult = controller.ViewEvents();
            
            Assert.AreEqual(
                0, ((ViewEventsUserModel)viewResult.Model).Events.Count);

            mockRepository.Verify(r => r.GetAll(), Times.Once);
        }

        [TestMethod]
        public void ViewEventsBookmarkedEventTest()
        {
            DB.Models.User user = new DB.Models.User()
            {
                Id = this.username
            };

            Event bookmarkedEvent = new Event()
            {
                IsApproved = true,
                BookmarkedBy = new List<DB.Models.User>() { user }
            };

            user.BookmarkedEvents = new List<Event>() { bookmarkedEvent };

            Event[] events = new Event[] { bookmarkedEvent };

            var mockRepository = new Mock<IRepository<Event>>();
            mockRepository.Setup(r => r.GetAll()).Returns(events.AsQueryable);
            
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


            var viewResult = controller.ViewEvents();

            Assert.AreEqual(
                1, ((ViewEventsUserModel)viewResult.Model).Events.Count);
            Assert.IsTrue(((ViewEventsUserModel)viewResult.Model)
                .Events.ToArray()[0].Value);

            mockRepository.Verify(r => r.GetAll(), Times.Once);

        }

        [TestMethod]
        public void ViewEventsNonBookmarkedEventTest()
        {
            DB.Models.User user = new DB.Models.User()
            {
                Id = this.username
            };

            Event bookmarkedEvent = new Event()
            {
                IsApproved = true,
                BookmarkedBy = new List<DB.Models.User>() { }
            };

            user.BookmarkedEvents = new List<Event>() { };

            Event[] events = new Event[] { bookmarkedEvent };

            var mockRepository = new Mock<IRepository<Event>>();
            mockRepository.Setup(r => r.GetAll()).Returns(events.AsQueryable);

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


            var viewResult = controller.ViewEvents();

            Assert.AreEqual(
                1, ((ViewEventsUserModel)viewResult.Model).Events.Count);
            Assert.IsFalse(((ViewEventsUserModel)viewResult.Model)
                .Events.ToArray()[0].Value);

            mockRepository.Verify(r => r.GetAll(), Times.Once);

        }
    }
}
