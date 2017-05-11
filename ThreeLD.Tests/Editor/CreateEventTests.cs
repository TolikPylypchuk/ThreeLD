using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
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
	public class CreateEventTests
	{
		[TestMethod]
		public void CreateEventPostValidTest()
		{
			var events = new List<Event>();

			var eventToAdd = new Event
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
			mockEvents.Setup(repo => repo.Add(eventToAdd))
				.Callback(() => events.Add(eventToAdd));
			mockEvents.Setup(repo => repo.Save()).Returns(1);
			
			var controller = new EditorController(mockEvents.Object, null);

			controller.Validate(eventToAdd);

			var result = controller.CreateEvent(eventToAdd);

			Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));

			var addedEvent = events.FirstOrDefault();

			mockEvents.Verify(repo => repo.Add(eventToAdd), Times.Once());
			mockEvents.Verify(repo => repo.Save(), Times.Once());

			Assert.AreSame(eventToAdd, addedEvent);
		}

		[TestMethod]
		public void CreateEventPostMessageTest()
		{
			var eventToAdd = new Event
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
			mockEvents.Setup(repo => repo.Update(eventToAdd));
			mockEvents.Setup(repo => repo.Save()).Returns(1);

			var controller = new EditorController(mockEvents.Object, null);

			controller.Validate(eventToAdd);

			var result = controller.CreateEvent(eventToAdd);

			Assert.IsNotNull(controller.TempData["message"]);
		}

		[TestMethod]
		public void CreateEventPostInvalidTest()
		{
			var eventToAdd = new Event
			{
				Id = 1,
				Address = "Updated Test",
				DateTime = DateTime.Now,
				Duration = TimeSpan.FromHours(2),
				Url = "https://www.event1.test.com"
			};
			
			var mockEvents = new Mock<IRepository<Event>>();
			mockEvents.Setup(repo => repo.Save()).Returns(1);

			var controller = new EditorController(mockEvents.Object, null);

			controller.Validate(eventToAdd);

			var result = controller.CreateEvent(eventToAdd);

			Assert.IsInstanceOfType(result, typeof(ViewResult));

			var viewResult = (ViewResult)result;

			Assert.AreSame(eventToAdd, viewResult.Model);

			mockEvents.Verify(repo => repo.Add(eventToAdd), Times.Never());
			mockEvents.Verify(repo => repo.Save(), Times.Never());
		}
	}
}
