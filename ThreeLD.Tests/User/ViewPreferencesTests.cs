using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;
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
    public class ViewPreferencesTests
    {
        private Mock<IPrincipal> mockPrincipal;
        private Mock<IRepository<Preference>> preferencesMockRepository;
        private Mock<IRepository<Event>> eventsMockRepository;

        private string username = "test@example.com";
        private Preference[] preferences;
        private Event[] events;

        [TestInitialize]
        public void Init()
        {
            this.preferences = new Preference[]
            {
                new Preference()
                {
                    Id = 1,
                    Category = "test",
                    UserId = this.username
                },
                new Preference()
                {
                    Id = 2,
                    Category = "test",
                    UserId = this.username
                },
                new Preference()
                {
                    Id = 3,
                    Category = "test",
                    UserId = "anotherUserId"
                }
            };

            this.events = new Event[]
            {
                new Event()
                {
                    Id = 1,
                    Category = "test1",
                    IsApproved = true
                },
                new Event()
                {
                    Id = 2,
                    Category = "test2",
                    IsApproved = true
                },
                new Event()
                {
                    Id = 3,
                    Category = "test2",
                    IsApproved = true
                },
                new Event()
                {
                    Id = 4,
                    Category = "test3",
                    IsApproved = false
                }
            };

            this.mockPrincipal = new Mock<IPrincipal>();
            var identity = new GenericIdentity(this.username);
            var nameIdentifierClaim = new Claim(
                ClaimTypes.NameIdentifier, this.username);
            identity.AddClaim(nameIdentifierClaim);
            this.mockPrincipal.Setup(x => x.Identity).Returns(identity);

            this.preferencesMockRepository = new Mock<IRepository<Preference>>();
            this.preferencesMockRepository.Setup(r => r.GetAll())
                .Returns(this.preferences.AsQueryable);

            this.eventsMockRepository = new Mock<IRepository<Event>>();
            this.eventsMockRepository.Setup(r => r.GetAll())
                .Returns(this.events.AsQueryable);
        }

        [TestMethod]
        public void ViewPreferencesGetUserIdReturnsUsernameTest()
        {
            var controllerContext = new Mock<ControllerContext>();
            var principal = this.mockPrincipal;
            controllerContext.SetupGet(x => x.HttpContext.User)
                .Returns(principal.Object);

            var controller =
                new UserController(
                    this.preferencesMockRepository.Object,
                    this.eventsMockRepository.Object)
                {
                    ControllerContext = controllerContext.Object
                };

            var viewResult = controller.ViewPreferences();
            
            Assert.AreEqual(
                2, 
                ((PreferencesViewModel)viewResult.Model).Preferences.Count());
            Assert.AreEqual(
                2,
                ((PreferencesViewModel)viewResult.Model).Categories.Count());

            this.preferencesMockRepository
                .Verify(r => r.GetAll(), Times.Once());
            this.eventsMockRepository.Verify(r => r.GetAll(), Times.Once());
        }
    }
}