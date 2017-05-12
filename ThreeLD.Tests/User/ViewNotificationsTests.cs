using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Web.Mvc;

using ThreeLD.DB.Models;
using ThreeLD.DB.Repositories;
using ThreeLD.Web.Controllers;
using ThreeLD.Web.Models.ViewModels;

namespace ThreeLD.Tests.User
{
	[TestClass]
	public class ViewNotificationsTests
	{
		private Mock<IPrincipal> mockPrincipal;
		private string username = "user";

		[TestInitialize]
		public void Init()
		{
			this.mockPrincipal = new Mock<IPrincipal>();
			var identity = new GenericIdentity(this.username);
			var nameIdentifierClaim = new Claim(
				ClaimTypes.NameIdentifier, this.username);
			identity.AddClaim(nameIdentifierClaim);

			this.mockPrincipal.Setup(x => x.Identity).Returns(identity);
		}

		[TestMethod]
		public void ViewEmptyNotificationsTest()
		{
			var mockRepository =
				new Mock<IRepository<Notification>>();

			var controllerContext = new Mock<ControllerContext>();
			controllerContext.SetupGet(x => x.HttpContext.User)
				.Returns(this.mockPrincipal.Object);

			var controller =
				new UserController(null, null, mockRepository.Object)
				{
					ControllerContext = controllerContext.Object
				};

			var result = controller.ViewNotifications();

			Assert.IsNotNull(result.Model);

			var notificationsModel = (NotificationsViewModel)result.Model;

			Assert.AreEqual(notificationsModel.ReadNotifications.Count, 0);
			Assert.AreEqual(notificationsModel.UnreadNotifications.Count, 0);

			foreach (var r in notificationsModel.ReadNotifications)
			{
				Assert.AreEqual(r.Message, string.Empty);
				Assert.AreEqual(r.IsRead, false);
				Assert.AreEqual(r.From, string.Empty);
				Assert.AreEqual(r.To, string.Empty);
			}

			foreach (var n in notificationsModel.UnreadNotifications)
			{
				Assert.AreEqual(n.Message, string.Empty);
				Assert.AreEqual(n.IsRead, false);
				Assert.AreEqual(n.From, string.Empty);
				Assert.AreEqual(n.To, string.Empty);
			}
		}

		[TestMethod]
		public void ViewOnlyReadNotificationsTest()
		{
			var mockRepository = new Mock<IRepository<Notification>>();

			var controllerContext = new Mock<ControllerContext>();
			controllerContext.SetupGet(x => x.HttpContext.User)
				.Returns(this.mockPrincipal.Object);

			var n = new Notification
			{
				Message = "myNotification",
				IsRead = true,
				From = "editorId",
				To = "user"
			};

			var notifications = new List<Notification> { n };

			mockRepository.Setup(r => r.GetAll())
				.Returns(notifications.AsQueryable);

			var controller =
				new UserController(null, null, mockRepository.Object)
				{
					ControllerContext = controllerContext.Object
				};

			var result = controller.ViewNotifications();

			Assert.IsNotNull(result.Model);

			var notificationsModel = (NotificationsViewModel)result.Model;

			Assert.AreEqual(notificationsModel.ReadNotifications.Count, 1);

			foreach (var r in notificationsModel.ReadNotifications)
			{
				Assert.AreEqual(r.Message, "myNotification");
				Assert.AreEqual(r.IsRead, true);
				Assert.AreEqual(r.From, "editorId");
				Assert.AreEqual(r.To, "user");
			}

			Assert.AreEqual(notificationsModel.UnreadNotifications.Count, 0);

			foreach (var u in notificationsModel.UnreadNotifications)
			{
				Assert.AreEqual(u.Message, string.Empty);
				Assert.AreEqual(u.IsRead, false);
				Assert.AreEqual(u.From, string.Empty);
				Assert.AreEqual(u.To, string.Empty);
			}
		}

		[TestMethod]
		public void ViewOnlyUnreadNotificationsTest()
		{
			var mockRepository = new Mock<IRepository<Notification>>();

			var controllerContext = new Mock<ControllerContext>();
			controllerContext.SetupGet(x => x.HttpContext.User)
				.Returns(this.mockPrincipal.Object);

			var n = new Notification
			{
				Message = "unreadNotification",
				IsRead = false,
				From = "editorId",
				To = "user"
			};

			var notifications = new List<Notification> { n };

			mockRepository.Setup(r => r.GetAll())
				.Returns(notifications.AsQueryable);

			var controller =
				new UserController(null, null, mockRepository.Object)
				{
					ControllerContext = controllerContext.Object
				};

			var result = controller.ViewNotifications();

			Assert.IsNotNull(result.Model);

			var notificationsModel = (NotificationsViewModel)result.Model;

			Assert.AreEqual(notificationsModel.ReadNotifications.Count, 0);

			foreach (var r in notificationsModel.ReadNotifications)
			{
				Assert.AreEqual(r.Message, string.Empty);
				Assert.AreEqual(r.IsRead, false);
				Assert.AreEqual(r.From, string.Empty);
				Assert.AreEqual(r.To, string.Empty);
			}

			Assert.AreEqual(notificationsModel.UnreadNotifications.Count, 1);

			foreach (var u in notificationsModel.UnreadNotifications)
			{
				Assert.AreEqual(u.Message, "unreadNotification");
				Assert.AreEqual(u.IsRead, false);
				Assert.AreEqual(u.From, "editorId");
				Assert.AreEqual(u.To, "user");
			}
		}

		[TestMethod]
		public void ViewAllNotificationsTest()
		{
			var mockRepository = new Mock<IRepository<Notification>>();

			var controllerContext = new Mock<ControllerContext>();
			controllerContext.SetupGet(x => x.HttpContext.User)
				.Returns(this.mockPrincipal.Object);

			var first = new Notification
			{
				Message = "readNotification",
				IsRead = true,
				From = "editorId",
				To = "user"
			};

			var second = new Notification
			{
				Message = "unreadNotification",
				IsRead = false,
				From = "editorId",
				To = "user"
			};

			var notifications = new List<Notification> { first, second };

			mockRepository.Setup(r => r.GetAll())
				.Returns(notifications.AsQueryable);

			var controller =
				new UserController(null, null, mockRepository.Object)
				{
					ControllerContext = controllerContext.Object
				};

			var result = controller.ViewNotifications();

			Assert.IsNotNull(result.Model);

			var notificationsModel = (NotificationsViewModel)result.Model;

			Assert.AreEqual(notificationsModel.ReadNotifications.Count, 1);

			foreach (var r in notificationsModel.ReadNotifications)
			{
				Assert.AreEqual(r.Message, "readNotification");
				Assert.AreEqual(r.IsRead, true);
				Assert.AreEqual(r.From, "editorId");
				Assert.AreEqual(r.To, "user");
			}

			Assert.AreEqual(notificationsModel.UnreadNotifications.Count, 1);

			foreach (var u in notificationsModel.UnreadNotifications)
			{
				Assert.AreEqual(u.Message, "unreadNotification");
				Assert.AreEqual(u.IsRead, false);
				Assert.AreEqual(u.From, "editorId");
				Assert.AreEqual(u.To, "user");
			}
		}
	}
}
