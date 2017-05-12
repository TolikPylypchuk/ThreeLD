using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web.Mvc;

using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

using ThreeLD.DB.Infrastructure;
using ThreeLD.DB.Models;
using ThreeLD.DB.Repositories;
using ThreeLD.Web.Controllers;
using ThreeLD.Web.Models.ViewModels;

namespace ThreeLD.Tests.User
{
	[TestClass]
	public class ProfileTests
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
		public void ProfileNoEventsTest()
		{
			var events = new Event[] { };

			var mockRepository = new Mock<IRepository<Event>>();
			mockRepository.Setup(r => r.GetAll())
				.Returns(events.AsQueryable());

			var user = new DB.Models.User
			{
				Id = this.username,
				Preferences = new List<Preference>()
			};

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

			var viewResult = controller.Profile();

			Assert.IsNotNull(controller.ViewBag.ReturnURL);

			var modelResult = (ProfileViewModel)viewResult.Model;

			Assert.AreEqual(user, modelResult.User);
			Assert.AreEqual(0, modelResult.Categories.Count());
			Assert.AreEqual(null, modelResult.SelectedCategory);

			mockRepository.Verify(r => r.GetAll(), Times.Once);
		}

		[TestMethod]
		public void ProfileNoApprovedEventsTest()
		{
			var events = new[]
			{
				new Event
				{
					IsApproved = false
				}
			};

			var mockRepository = new Mock<IRepository<Event>>();
			mockRepository.Setup(r => r.GetAll())
				.Returns(events.AsQueryable());

			var user = new DB.Models.User
			{
				Id = this.username,
				Preferences = new List<Preference>()
			};

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

			var viewResult = controller.Profile();

			Assert.IsNotNull(controller.ViewBag.ReturnURL);

			var modelResult = (ProfileViewModel)viewResult.Model;

			Assert.AreEqual(user, modelResult.User);
			Assert.AreEqual(0, modelResult.Categories.Count());
			Assert.AreEqual(null, modelResult.SelectedCategory);

			mockRepository.Verify(r => r.GetAll(), Times.Once);
		}

		[TestMethod]
		public void ProfileApprovedEventTest()
		{
			var events = new[]
			{
				new Event
				{
					Category = "test",
					IsApproved = true
				}
			};

			var mockRepository = new Mock<IRepository<Event>>();
			mockRepository.Setup(r => r.GetAll())
				.Returns(events.AsQueryable());

			var user = new DB.Models.User
			{
				Id = this.username,
				Preferences = new List<Preference>()
			};

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

			var viewResult = controller.Profile();

			Assert.IsNotNull(controller.ViewBag.ReturnURL);

			var modelResult = (ProfileViewModel)viewResult.Model;

			Assert.AreEqual(user, modelResult.User);
			Assert.AreEqual(1, modelResult.Categories.Count());
			Assert.AreEqual(null, modelResult.SelectedCategory);

			mockRepository.Verify(r => r.GetAll(), Times.Once);
		}

		[TestMethod]
		public void ProfileApprovedEventUserPreferenceTest()
		{
			const string category = "test";
			var events = new[]
			{
				new Event
				{
					Category = category,
					IsApproved = true
				}
			};

			var mockRepository = new Mock<IRepository<Event>>();
			mockRepository.Setup(r => r.GetAll())
				.Returns(events.AsQueryable());

			var user = new DB.Models.User
			{
				Id = this.username,
				Preferences = new List<Preference>
				{
					new Preference
					{
						Category = category
					}
				}
			};

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

			var viewResult = controller.Profile();

			Assert.IsNotNull(controller.ViewBag.ReturnURL);

			var modelResult = (ProfileViewModel)viewResult.Model;

			Assert.AreEqual(user, modelResult.User);
			Assert.AreEqual(0, modelResult.Categories.Count());
			Assert.AreEqual(null, modelResult.SelectedCategory);

			mockRepository.Verify(r => r.GetAll(), Times.Once);
		}
	}
}
