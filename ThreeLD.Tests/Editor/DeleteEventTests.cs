using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

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
	public class DeleteEventTests
	{
		[TestMethod]
		public void DeleteTest()
		{
			const int id = 1;

			var ev = new Event
			{
				Id = id,
				Name = "Test",
				Address = "Test",
				DateTime = DateTime.Now,
				Duration = TimeSpan.FromHours(2),
				Url = "https://www.event1.test.com",
				Description = "A test event",
				Category = "Test",
				IsApproved = true
			};

			var events = new List<Event> { ev };
			
			var mockEvents = new Mock<IRepository<Event>>();
			mockEvents.Setup(repo => repo.GetById(id)).Returns(events[0]);
			mockEvents.Setup(repo => repo.Delete(ev))
				.Callback(() => events.RemoveAll(e => e.Id == ev.Id));
			mockEvents.Setup(repo => repo.Save()).Returns(1);

			var controller = new EditorController(
				mockEvents.Object, null);
			
			var result = controller.DeleteEvent(id);

			Assert.AreEqual(0, events.Count);
			Assert.IsNotNull(controller.TempData["message"]);
			Assert.IsNull(controller.TempData["error"]);

			mockEvents.Verify(repo => repo.GetById(id), Times.Once());
			mockEvents.Verify(repo => repo.Delete(ev), Times.Once());
			mockEvents.Verify(repo => repo.Save(), Times.Once());
		}

		[TestMethod]
		public void DeleteNonExistentTest()
		{
			const int id = 1;

			var eventNotToDelete = new Event
			{
				Id = 2,
				Name = "Test",
				Address = "Test",
				DateTime = DateTime.Now,
				Duration = TimeSpan.FromHours(2),
				Url = "https://www.event1.test.com",
				Description = "A test event",
				Category = "Test",
				IsApproved = true
			};

			var events = new List<Event> { eventNotToDelete };

			var mock = new Mock<IRepository<Event>>();
			mock.Setup(repo => repo.GetById(id)).Returns((Event)null);

			var controller = new EditorController(mock.Object, null);
			
			var result = controller.DeleteEvent(id);

			Assert.AreEqual(1, events.Count);
			Assert.IsNull(controller.TempData["message"]);
			Assert.IsNotNull(controller.TempData["error"]);

			mock.Verify(repo => repo.GetById(id), Times.Once());
			mock.Verify(repo => repo.Delete(eventNotToDelete), Times.Never());
			mock.Verify(repo => repo.Save(), Times.Never());
		}

		[TestMethod]
		public void DeleteProposedTest()
		{
			const int id = 1;

			var ev = new Event
			{
				Id = id,
				Name = "Test",
				Address = "Test",
				DateTime = DateTime.Now,
				Duration = TimeSpan.FromHours(2),
				Url = "https://www.event1.test.com",
				Description = "A test event",
				Category = "Test",
				IsApproved = false
			};

			var events = new List<Event> { ev };

			var mock = new Mock<IRepository<Event>>();
			mock.Setup(repo => repo.GetById(id)).Returns(events[0]);
			mock.Setup(repo => repo.Delete(ev))
				.Callback(() => events.RemoveAll(e => e.Id == ev.Id));
			mock.Setup(repo => repo.Save()).Returns(1);

			var controller = new EditorController(mock.Object, null);

			var result = controller.DeleteEvent(id);

			Assert.AreEqual(0, events.Count);
			Assert.IsNotNull(controller.TempData["message"]);
			Assert.IsNull(controller.TempData["error"]);

			mock.Verify(repo => repo.GetById(id), Times.Once());
			mock.Verify(repo => repo.Delete(ev), Times.Once());
			mock.Verify(repo => repo.Save(), Times.Once());
		}
	}
}
