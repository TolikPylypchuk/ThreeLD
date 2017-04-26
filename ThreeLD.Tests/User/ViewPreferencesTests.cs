using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;
using System.Linq;
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
        [TestMethod]
        public void ViewPreferencesGetUserIdReturnsNullTest()
        {            
            var preferences = new Preference[]
            {
            };

            var mockRepo = new Mock<IRepository<Preference>>();
            mockRepo.Setup(r => r.GetAll()).Returns(preferences.AsQueryable<Preference>);

            var controllerContext = new Mock<ControllerContext>();
            var principal = new Mock<IPrincipal>();
            controllerContext.SetupGet(x => x.HttpContext.User)
                .Returns(principal.Object);

            var controller = new UserController(mockRepo.Object, null)
            {
                ControllerContext = controllerContext.Object
            };
            var viewResult = controller.ViewPreferences();
            
            Assert.AreEqual(0, ((Preference[])viewResult.Model).Length);

            mockRepo.Verify(r => r.GetAll(), Times.Once());
        }
    }
}