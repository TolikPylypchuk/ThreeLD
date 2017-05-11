using System;
using System.Diagnostics.CodeAnalysis;
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
			
			var mock = new Mock<IRepository<Event>>();
			mock.Setup(repo => repo.GetById(eventToUpdate.Id))
				.Returns(eventToUpdate);

			var controller = new EditorController(mock.Object, null);

			var result = controller.EditEvent(eventToUpdate.Id);

			Assert.AreSame(eventToUpdate, (Event)result.Model);

			mock.Verify(repo => repo.GetById(eventToUpdate.Id), Times.Once());
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

			var mock = new Mock<IRepository<Event>>();
			mock.Setup(repo => repo.Update(eventToUpdate))
				.Callback(() => events[0] = eventToUpdate);
			mock.Setup(repo => repo.Save()).Returns(1);

			var controller = new EditorController(mock.Object, null);

			controller.Validate(eventToUpdate);

			var result = controller.EditEvent(eventToUpdate);

			Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));

			Assert.AreSame(eventToUpdate, events[0]);

			mock.Verify(repo => repo.Update(eventToUpdate), Times.Once());
			mock.Verify(repo => repo.Save(), Times.Once());
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

			var mock = new Mock<IRepository<Event>>();
			mock.Setup(repo => repo.Update(eventToUpdate));
			mock.Setup(repo => repo.Save()).Returns(1);

			var controller = new EditorController(mock.Object, null);

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

			var mock = new Mock<IRepository<Event>>();
			mock.Setup(repo => repo.Update(eventToUpdate))
				.Callback(() => events[0] = eventToUpdate);
			mock.Setup(repo => repo.Save()).Returns(1);

			var controller = new EditorController(mock.Object, null);

			controller.Validate(eventToUpdate);

			var result = controller.EditEvent(eventToUpdate);

			Assert.IsInstanceOfType(result, typeof(ViewResult));

			var viewResult = (ViewResult)result;

			Assert.AreSame(originalEvent, events[0]);
			Assert.AreSame(eventToUpdate, viewResult.Model);

			mock.Verify(repo => repo.Update(eventToUpdate), Times.Never());
			mock.Verify(repo => repo.Save(), Times.Never());
		}
	}
}
