using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Linq;
using System.Security.Principal;
using System.Web.Mvc;
using Moq;

using ThreeLD.DB.Models;
using ThreeLD.DB.Repositories;
using ThreeLD.Web.Controllers;
using System.Web;

namespace ThreeLD.Tests.User
{
    [TestClass]
    public class ViewPreferencesTests
    {
        [TestMethod]
        public void TestMethod1()
        {            
            var preferences = new[]
            {
                new Preference
                {
                    Id = 1,
                    UserId = "testUserId"
                },
                new Preference
                {
                    Id = 2,
                    UserId = "testUserId"
                },
                new Preference
                {
                    Id = 3,
                    UserId = "anotherTestUserId"
                }
            };

            var mockRepo = new Mock<IRepository<Preference>>();
            mockRepo.Setup(r => r.GetAll()).Returns(preferences.AsQueryable<Preference>);

            var controllerContext = new Mock<ControllerContext>();
            var principal = new Mock<IPrincipal>();
            principal.Setup(x => x.Identity.Name).Returns("testUserId");
            controllerContext.SetupGet(x => x.HttpContext.User)
                .Returns(principal.Object);

            var controller = new UserController(mockRepo.Object, null)
            {
                ControllerContext = controllerContext.Object
            };
            var viewResult = controller.ViewPreferences();
            
            Assert.AreEqual(2, ((Preference[])viewResult.Model).Length);

            mockRepo.Verify(r => r.GetAll(), Times.Once());
        }
    }
}