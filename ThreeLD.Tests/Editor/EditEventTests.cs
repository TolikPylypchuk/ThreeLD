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

			var events = new[] { eventToUpdate }.AsQueryable();

			var mock = new Mock<IRepository<Event>>();
			mock.Setup(repo => repo.GetById(eventToUpdate.Id))
				.Returns(eventToUpdate);
			mock.Setup(repo => repo.Update(eventToUpdate))
				.Callback(() => events = new[] { eventToUpdate }.AsQueryable());
			mock.Setup(repo => repo.Save()).Returns(1);

			var controller = new EditorController(mock.Object);

			var result = controller.EditEvent(eventToUpdate.Id);

			Assert.AreSame(eventToUpdate, (Event)result.Model);
		}
	}
}
