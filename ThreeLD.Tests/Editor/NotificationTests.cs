using System;
using System.Collections.Generic;
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
	[SuppressMessage("ReSharper", "InconsistentNaming")]
	public class NotificationTests
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
		public void ApproveEventNotificationTest()
		{
			var eventToApprove = new Event
			{
				Id = 1,
				Name = "Test 1",
				Address = "Test",
				DateTime = DateTime.Now,
				Duration = TimeSpan.FromHours(2),
				Url = "https://www.event1.test.com",
				Description = "A test event",
				Category = "Test",
				IsApproved = true,
				ProposedBy = "b"
			};
			
			var notifications = new List<Notification>();

			var mockEvents = new Mock<IRepository<Event>>();
			mockEvents.Setup(repo => repo.GetById(eventToApprove.Id))
				.Returns(eventToApprove);
			mockEvents.Setup(repo => repo.Save()).Returns(1);

			var mockNotifications = new Mock<IRepository<Notification>>();
			mockNotifications.Setup(repo => repo.Add(It.IsAny<Notification>()))
				.Callback<Notification>(n => notifications.Add(n));
			mockNotifications.Setup(repo => repo.Save()).Returns(1);

			var mockContext = new Mock<ControllerContext>();
			mockContext.SetupGet(c => c.HttpContext.User)
				.Returns(this.mockPrincipal.Object);

			var controller = new EditorController(
				mockEvents.Object, mockNotifications.Object)
			{
				ControllerContext = mockContext.Object
			};

			var result = controller.ApproveEvent(eventToApprove.Id);

			Assert.AreEqual(1, notifications.Count);

			var notification = notifications[0];

			Assert.AreEqual(eventToApprove.CreatedBy, notification.From);
			Assert.AreEqual(eventToApprove.ProposedBy, notification.To);

			mockNotifications.Verify(
				repo => repo.Add(It.IsAny<Notification>()), Times.Once());
			mockNotifications.Verify(repo => repo.Save(), Times.Once());
		}

		[TestMethod]
		public void EditEventNotificationTest()
		{
			var eventToUpdate = new Event
			{
				Id = 1,
				Name = "Test 1",
				Address = "Test",
				DateTime = DateTime.Now,
				Duration = TimeSpan.FromHours(2),
				Url = "https://www.event1.test.com",
				Description = "A test event",
				Category = "Test",
				IsApproved = true,
				ProposedBy = "b"
			};

			var notifications = new List<Notification>();

			var mockEvents = new Mock<IRepository<Event>>();
			mockEvents.Setup(repo => repo.GetById(eventToUpdate.Id))
				.Returns(eventToUpdate);
			mockEvents.Setup(repo => repo.Save()).Returns(1);

			var mockNotifications = new Mock<IRepository<Notification>>();
			mockNotifications.Setup(repo => repo.Add(It.IsAny<Notification>()))
				.Callback<Notification>(n => notifications.Add(n));
			mockNotifications.Setup(repo => repo.Save()).Returns(1);

			var mockContext = new Mock<ControllerContext>();
			mockContext.SetupGet(c => c.HttpContext.User)
				.Returns(this.mockPrincipal.Object);

			var controller = new EditorController(
				mockEvents.Object, mockNotifications.Object)
			{
				ControllerContext = mockContext.Object
			};

			controller.Validate(eventToUpdate);

			var result = controller.EditEvent(eventToUpdate);

			Assert.AreEqual(1, notifications.Count);

			var notification = notifications[0];

			Assert.AreEqual(eventToUpdate.CreatedBy, notification.From);
			Assert.AreEqual(eventToUpdate.ProposedBy, notification.To);

			mockNotifications.Verify(
				repo => repo.Add(It.IsAny<Notification>()), Times.Once());
			mockNotifications.Verify(repo => repo.Save(), Times.Once());
		}

		[TestMethod]
		public void DeleteEventNotificationTest()
		{
			var eventToDelete = new Event
			{
				Id = 1,
				Name = "Test 1",
				Address = "Test",
				DateTime = DateTime.Now,
				Duration = TimeSpan.FromHours(2),
				Url = "https://www.event1.test.com",
				Description = "A test event",
				Category = "Test",
				IsApproved = true,
				ProposedBy = "b",
				CreatedBy = userId
			};

			var notifications = new List<Notification>();

			var mockEvents = new Mock<IRepository<Event>>();
			mockEvents.Setup(repo => repo.GetById(eventToDelete.Id))
				.Returns(eventToDelete);
			mockEvents.Setup(repo => repo.Save()).Returns(1);

			var mockNotifications = new Mock<IRepository<Notification>>();
			mockNotifications.Setup(repo => repo.Add(It.IsAny<Notification>()))
				.Callback<Notification>(n => notifications.Add(n));
			mockNotifications.Setup(repo => repo.Save()).Returns(1);

			var mockContext = new Mock<ControllerContext>();
			mockContext.SetupGet(c => c.HttpContext.User)
				.Returns(this.mockPrincipal.Object);

			var controller = new EditorController(
				mockEvents.Object, mockNotifications.Object)
			{
				ControllerContext = mockContext.Object
			};

			controller.Validate(eventToDelete);

			var result = controller.DeleteEvent(eventToDelete.Id);

			Assert.AreEqual(1, notifications.Count);

			var notification = notifications[0];

			Assert.AreEqual(eventToDelete.CreatedBy, notification.From);
			Assert.AreEqual(eventToDelete.ProposedBy, notification.To);

			mockNotifications.Verify(
				repo => repo.Add(It.IsAny<Notification>()), Times.Once());
			mockNotifications.Verify(repo => repo.Save(), Times.Once());
		}
	}
}
