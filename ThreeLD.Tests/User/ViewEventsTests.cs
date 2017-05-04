using Microsoft.AspNet.Identity;
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

            var userStore = new Mock<IUserStore<DB.Models.User>>();
            userStore.Setup(x => x.CreateAsync(user))
             .Returns(Task.FromResult(IdentityResult.Success));
            userStore.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(user);

            Task<IdentityResult> tt =
                (Task<IdentityResult>)userStore.Object.CreateAsync(user);

            var userManager = new Mock<AppUserManager>(userStore.Object);

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
                BookmarkedBy = new List<DB.Models.User>() { user }
            };

            user.BookmarkedEvents = new List<Event>() { bookmarkedEvent };

            Event[] events = new Event[] { bookmarkedEvent };

            var mockRepository = new Mock<IRepository<Event>>();
            mockRepository.Setup(r => r.GetAll()).Returns(events.AsQueryable);

            var userStore = new Mock<IUserStore<DB.Models.User>>();
            userStore.Setup(x => x.CreateAsync(user))
             .Returns(Task.FromResult(IdentityResult.Success));
            userStore.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(user);

            Task<IdentityResult> tt =
                (Task<IdentityResult>)userStore.Object.CreateAsync(user);

            var userManager = new Mock<AppUserManager>(userStore.Object);

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
        }
    }
}
