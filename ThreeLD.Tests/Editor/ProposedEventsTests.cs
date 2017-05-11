using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

using ThreeLD.DB.Models;
using ThreeLD.DB.Repositories;
using ThreeLD.Web.Controllers;

namespace ThreeLD.Tests.Editor
{
	[TestClass]
	[SuppressMessage("ReSharper", "UnusedVariable")]
	[SuppressMessage("ReSharper", "InconsistentNaming")]
	public class ProposedEventsTests
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

			this.mockPrincipal.Setup(p => p.Identity)
				.Returns(identity);
		}
		
		[TestMethod]
		public void ViewProposedEventsTest()
		{
			var events = new List<Event>
			{
				new Event
				{
					Id = 1,
					Name = "Test 1",
					Address = "Test",
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
					Address = "Test",
					DateTime = DateTime.Now,
					Duration = TimeSpan.FromHours(2),
					Url = "https://www.event2.test.com",
					Description = "A test event",
					Category = "Test",
					IsApproved = false
				}
			};

			var mockEvents = new Mock<IRepository<Event>>();
			mockEvents.Setup(repo => repo.GetAll())
				.Returns(events.AsQueryable());

			var controller = new EditorController(mockEvents.Object, null);

			var result = controller.ViewProposedEvents();

			Assert.IsInstanceOfType(result.Model, typeof(IEnumerable<Event>));

			var model = ((IEnumerable<Event>)result.Model).ToList();

			Assert.AreEqual(1, model.Count);
			Assert.AreEqual(2, model[0].Id);

			mockEvents.Verify(repo => repo.GetAll(), Times.Once());
		}

		[TestMethod]
		public void ApproveEventTest()
		{
			const int id = 1;

			var eventToApprove = new Event
			{
				Id = id,
				Name = "Test 1",
				Address = "Test",
				DateTime = DateTime.Now,
				Duration = TimeSpan.FromHours(2),
				Url = "https://www.event1.test.com",
				Description = "A test event",
				Category = "Test",
				IsApproved = false
			};

			var mockEvents = new Mock<IRepository<Event>>();
			mockEvents.Setup(repo => repo.GetById(id)).Returns(eventToApprove);
			mockEvents.Setup(repo => repo.Save()).Returns(1);

			var mockNotifications = new Mock<IRepository<Notification>>();

			var mockContext = new Mock<ControllerContext>();
			mockContext.SetupGet(c => c.HttpContext.User)
				.Returns(this.mockPrincipal.Object);

			var controller = new EditorController(
				mockEvents.Object, mockNotifications.Object)
			{
				ControllerContext = mockContext.Object
			};

			var result = controller.ApproveEvent(id);

			Assert.IsTrue(eventToApprove.IsApproved);

			Assert.IsNotNull(controller.TempData["message"]);
			Assert.IsNull(controller.TempData["error"]);

			mockEvents.Verify(repo => repo.GetById(id), Times.Once());
			mockEvents.Verify(repo => repo.Update(eventToApprove), Times.Once());
			mockEvents.Verify(repo => repo.Save(), Times.Once());
		}

		[TestMethod]
		public void ApproveNonExistentTest()
		{
			const int id = 1;

			var mockEvents = new Mock<IRepository<Event>>();
			mockEvents.Setup(repo => repo.GetById(id)).Returns((Event)null);

			var controller = new EditorController(mockEvents.Object, null);

			var result = controller.ApproveEvent(id);

			Assert.IsNull(controller.TempData["message"]);
			Assert.IsNotNull(controller.TempData["error"]);

			mockEvents.Verify(repo => repo.GetById(id), Times.Once());
			mockEvents.Verify(repo => repo.Save(), Times.Never());
		}
	}
}
