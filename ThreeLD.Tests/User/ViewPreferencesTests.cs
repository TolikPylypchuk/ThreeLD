using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Web.Mvc;

using ThreeLD.DB.Models;
using ThreeLD.DB.Repositories;
using ThreeLD.Web.Controllers;

namespace ThreeLD.Tests.User
{
    [TestClass]
    public class ViewPreferencesTests
    {
        private Mock<IPrincipal> mockPrincipal;
        private Mock<IRepository<Preference>> mockRepository;

        private string username = "test@example.com";
        private Preference[] preferences;

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
            
            this.mockPrincipal = new Mock<IPrincipal>();
            var identity = new GenericIdentity(this.username);
            var nameIdentifierClaim = new Claim(
                ClaimTypes.NameIdentifier, this.username);
            identity.AddClaim(nameIdentifierClaim);
            this.mockPrincipal.Setup(x => x.Identity).Returns(identity);

            this.mockRepository = new Mock<IRepository<Preference>>();
            this.mockRepository.Setup(r => r.GetAll())
                .Returns(this.preferences.AsQueryable);
        }

        [TestMethod]
        public void ViewPreferencesGetUserIdReturnsUsernameTest()
        {
            var controllerContext = new Mock<ControllerContext>();
            var principal = this.mockPrincipal;
            controllerContext.SetupGet(x => x.HttpContext.User)
                .Returns(principal.Object);

            var controller =
                new UserController(this.mockRepository.Object, null)
                {
                    ControllerContext = controllerContext.Object
                };
            var viewResult = controller.ViewPreferences();
            
            Assert.AreEqual(2, ((Preference[])viewResult.Model).Length);

            this.mockRepository.Verify(r => r.GetAll(), Times.Once());
        }
    }
}