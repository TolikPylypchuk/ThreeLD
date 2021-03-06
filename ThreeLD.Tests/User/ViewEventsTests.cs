﻿using Microsoft.AspNet.Identity.EntityFramework;
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
			var events = new Event[] { };

			var user = new DB.Models.User()
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

			mockRepository.Verify(r => r.GetAll(), Times.Exactly(2));
		}

		[TestMethod]
		public void ViewEventsBookmarkedEventTest()
		{
			var user = new DB.Models.User
			{
				Id = this.username
			};

			Event bookmarkedEvent = new Event()
			{
				IsApproved = true,
				BookmarkedBy = new List<DB.Models.User>{ user },
                Category = "test"
			};

			user.BookmarkedEvents = new List<Event>{ bookmarkedEvent };

			var events = new[] { bookmarkedEvent };

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

			mockRepository.Verify(r => r.GetAll(), Times.Exactly(2));

		}

		[TestMethod]
		public void ViewEventsNonBookmarkedEventTest()
		{
			var user = new DB.Models.User
			{
				Id = this.username
			};

			Event bookmarkedEvent = new Event
			{
				IsApproved = true,
				BookmarkedBy = new List<DB.Models.User>(),
                Category = "test"
			};

			user.BookmarkedEvents = new List<Event>();

			var events = new[] { bookmarkedEvent };

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

            mockRepository.Verify(r => r.GetAll(), Times.Exactly(2));
        }
    }
}
