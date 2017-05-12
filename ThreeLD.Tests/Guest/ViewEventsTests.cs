using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

using ThreeLD.DB.Models;
using ThreeLD.DB.Repositories;
using ThreeLD.Web.Controllers;


namespace ThreeLD.Tests.Guest
{
    [TestClass]
    public class ViewEventsTests
    {
        [TestMethod]
        public void ViewEventsEmptyTest()
        {
            var mockRepository =
                new Mock<IRepository<Event>>();

            var newEvent = new Event
            {
                Name = "event",
                DateTime = DateTime.Now,
                Duration = TimeSpan.MinValue,
                Address = "address1",
                Url = "event.com",
                Description = "event description",
                Category = "event category",
                IsApproved = false,
                CreatedBy = null,
                ProposedBy = "user"
            };

            var events = new List<Event> { newEvent };

            mockRepository.Setup(r => r.GetAll())
                .Returns(events.AsQueryable);

            var controller = new GuestController(mockRepository.Object);

            var result = controller.ViewEvents();

            Assert.IsNotNull(result.Model);

            var eventModel = (List<Event>)result.Model;

            Assert.AreEqual(eventModel.Count, 0);

            foreach (var e in eventModel)
            {
                Assert.AreEqual(e.Name, string.Empty);
                Assert.AreEqual(e.DateTime, default(DateTime));
                Assert.AreEqual(e.Duration, default(TimeSpan));
                Assert.AreEqual(e.Address, string.Empty);
                Assert.AreEqual(e.Url, string.Empty);
                Assert.AreEqual(e.Description, string.Empty);
                Assert.AreEqual(e.Category, string.Empty);
                Assert.AreEqual(e.IsApproved, false);
                Assert.AreEqual(e.CreatedBy, string.Empty);
                Assert.AreEqual(e.ProposedBy, string.Empty);
            }
        }
    }
}
