using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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

namespace ThreeLD.Tests.Editor
{
	[TestClass]
	[SuppressMessage("ReSharper", "ImplicitlyCapturedClosure")]
	[SuppressMessage("ReSharper", "UnusedVariable")]
	[SuppressMessage("ReSharper", "InconsistentNaming")]
	public class ViewEventsTests
	{
		private Mock<IPrincipal> mockPrincipal;
		private const string userId = "a";

		[TestInitialize]
		public void Init()
		{
			this.mockPrincipal = new Mock<IPrincipal>();
			var identity = new GenericIdentity(userId);

			var nameIdentifierClaim = new Claim(
				ClaimTypes.NameIdentifier, userId);
			identity.AddClaim(nameIdentifierClaim);

			this.mockPrincipal.Setup(x => x.Identity)
				.Returns(identity);
		}

		[TestMethod]
		public void ViewEventsTest()
		{
			var events = new List<Event>
			{
				new Event
				{
					Id = 1,
					Name = "Test 1",
					Address = "Test 1",
					DateTime = DateTime.Now,
					Duration = TimeSpan.FromHours(2),
					Url = "https://www.event1.test.com",
					Description = "A test event",
					Category = "Test",
					IsApproved = true
				},
				new Event
				{
					Id = 2,
					Name = "Test 2",
					Address = "Test 2",
					DateTime = DateTime.Now,
					Duration = TimeSpan.FromHours(2),
					Url = "https://www.event2.test.com",
					Description = "A test event",
					Category = "Test",
					IsApproved = true
				}
			};

			var user = new DB.Models.User
			{
				Id = userId,
				BookmarkedEvents = new List<Event> { events[0] }
			};

			var mockEvents = new Mock<IRepository<Event>>();
			mockEvents.Setup(repo => repo.GetAll())
				.Returns(events.AsQueryable());

			var mockManager = new Mock<AppUserManager>(
				new UserStore<DB.Models.User>());
			mockManager.Setup(m => m.FindByIdAsync(userId))
				.Returns(Task.FromResult(user));

			var mockContext = new Mock<ControllerContext>();
			mockContext.SetupGet(x => x.HttpContext.User)
				.Returns(this.mockPrincipal.Object);

			var controller = new EditorController(mockEvents.Object, null)
			{
				ControllerContext = mockContext.Object,
				UserManager = mockManager.Object
			};

			var result = controller.ViewEvents().Result;
			var model = result.Model as ViewEventsUserModel;

			Assert.IsNotNull(model);
			Assert.AreEqual(events.Count, model.Events.Count);
			Assert.AreEqual(true, model.Events[events[0]]);
			Assert.AreEqual(false, model.Events[events[1]]);
		}
	}
}
