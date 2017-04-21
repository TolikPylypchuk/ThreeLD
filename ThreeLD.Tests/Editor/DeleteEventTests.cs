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

			var events = new List<Event>
			{
				new Event
				{
					Id = id,
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
			mock.Setup(repo => repo.Delete(id))
				.Callback(() => events.RemoveAll(e => e.Id == id));
			mock.Setup(repo => repo.Save()).Returns(1);

			var controller = new EditorController(mock.Object);
			
			var result = controller.DeleteEvent(id);

			Assert.AreEqual(0, events.Count);
			Assert.IsNotNull(controller.TempData["message"]);
		}

		[TestMethod]
		public void DeleteNonExistentTest()
		{
			const int id = 2;

			var events = new List<Event>
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
			mock.Setup(repo => repo.Delete(id))
				.Callback(() => events.RemoveAll(e => e.Id == id));
			mock.Setup(repo => repo.Save()).Returns(0);

			var controller = new EditorController(mock.Object);
			
			var result = controller.DeleteEvent(id);

			Assert.AreEqual(1, events.Count);
			Assert.IsNull(controller.TempData["message"]);
		}
	}
}
