using System;
using System.Diagnostics.CodeAnalysis;
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
	[SuppressMessage("ReSharper", "ImplicitlyCapturedClosure")]
	[SuppressMessage("ReSharper", "UnusedVariable")]
	public class EditEventTests
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
		public void EditEventGetTest()
		{
			var eventToUpdate = new Event
			{
				Id = 1,
				Name = "Test",
				Address = "Test",
				DateTime = DateTime.Now,
				Duration = TimeSpan.FromHours(2),
				Url = "https://www.event1.test.com",
				Description = "A test event",
				Category = "Test"
			};
			
			var mockEvents = new Mock<IRepository<Event>>();
			mockEvents.Setup(repo => repo.GetById(eventToUpdate.Id))
				.Returns(eventToUpdate);

			var controller = new EditorController(mockEvents.Object, null);

			var result = controller.EditEvent(eventToUpdate.Id);

			Assert.AreSame(eventToUpdate, (Event)result.Model);

			mockEvents.Verify(repo => repo.GetById(eventToUpdate.Id), Times.Once());
		}

		[TestMethod]
		public void EditEventPostValidTest()
		{
			var eventToUpdate = new Event
			{
				Id = 1,
				Name = "Test",
				Address = "Test",
				DateTime = DateTime.Now,
				Duration = TimeSpan.FromHours(2),
				Url = "https://www.event1.test.com",
				Description = "An updated test event",
				Category = "Test"
			};

			var events = new[]
			{
				new Event
				{
					Id = 1,
					Name = "Test",
					Address = "Test",
					DateTime = DateTime.Now,
					Duration = TimeSpan.FromHours(2),
					Url = "https://www.event1.test.com",
					Description = "A test event",
					Category = "Test"
				}
			};

			var mockEvents = new Mock<IRepository<Event>>();
			mockEvents.Setup(repo => repo.Update(eventToUpdate))
				.Callback(() => events[0] = eventToUpdate);
			mockEvents.Setup(repo => repo.Save()).Returns(1);

			var mockContext = new Mock<ControllerContext>();
			mockContext.SetupGet(c => c.HttpContext.User)
				.Returns(this.mockPrincipal.Object);

			var controller = new EditorController(mockEvents.Object, null)
			{
				ControllerContext = mockContext.Object
			};

			controller.Validate(eventToUpdate);

			var result = controller.EditEvent(eventToUpdate);

			Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));

			Assert.AreSame(eventToUpdate, events[0]);

			mockEvents.Verify(repo => repo.Update(eventToUpdate), Times.Once());
			mockEvents.Verify(repo => repo.Save(), Times.Once());
		}

		[TestMethod]
		public void EditEventPostMessageTest()
		{
			var eventToUpdate = new Event
			{
				Name = "Test",
				Address = "Test",
				DateTime = DateTime.Now,
				Duration = TimeSpan.FromHours(2),
				Url = "https://www.event1.test.com",
				Description = "A test event",
				Category = "Test"
			};

			var mockEvents = new Mock<IRepository<Event>>();
			mockEvents.Setup(repo => repo.Update(eventToUpdate));
			mockEvents.Setup(repo => repo.Save()).Returns(1);

			var mockContext = new Mock<ControllerContext>();
			mockContext.SetupGet(c => c.HttpContext.User)
				.Returns(this.mockPrincipal.Object);

			var controller = new EditorController(mockEvents.Object, null)
			{
				ControllerContext = mockContext.Object
			};

			controller.Validate(eventToUpdate);

			var result = controller.EditEvent(eventToUpdate);

			Assert.IsNotNull(controller.TempData["message"]);
		}
		
		[TestMethod]
		public void EditEventPostInvalidTest()
		{
			var eventToUpdate = new Event
			{
				Id = 1,
				Name = "Test",
				Address = "Updated Test",
				DateTime = DateTime.Now,
				Duration = TimeSpan.FromHours(2),
				Url = "https://www.event1.test.com"
			};

			var originalEvent = new Event
			{
				Id = 1,
				Name = "Test",
				Address = "Test",
				DateTime = DateTime.Now,
				Duration = TimeSpan.FromHours(2),
				Url = "https://www.event1.test.com",
				Description = "A test event",
				Category = "Test"
			};

			var events = new[] { originalEvent };

			var mockEvents = new Mock<IRepository<Event>>();
			mockEvents.Setup(repo => repo.Update(eventToUpdate))
				.Callback(() => events[0] = eventToUpdate);
			mockEvents.Setup(repo => repo.Save()).Returns(1);

			var controller = new EditorController(mockEvents.Object, null);

			controller.Validate(eventToUpdate);

			var result = controller.EditEvent(eventToUpdate);

			Assert.IsInstanceOfType(result, typeof(ViewResult));

			var viewResult = (ViewResult)result;

			Assert.AreSame(originalEvent, events[0]);
			Assert.AreSame(eventToUpdate, viewResult.Model);

			mockEvents.Verify(repo => repo.Update(eventToUpdate), Times.Never());
			mockEvents.Verify(repo => repo.Save(), Times.Never());
		}
	}
}
