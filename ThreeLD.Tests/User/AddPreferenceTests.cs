﻿using System.Collections.Generic;
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
using ThreeLD.Web.Models.ViewModels;

namespace ThreeLD.Tests.User
{
	[TestClass]
	[SuppressMessage("ReSharper", "UnusedVariable")]
	[SuppressMessage("ReSharper", "ImplicitlyCapturedClosure")]
	public class AddPreferenceTests
	{
		private Mock<IPrincipal> mockPrincipal;
		private string username = "test@example.com";

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
		public void AddNullPreferenceCategoryTest()
		{
			var preferenceToAdd = new Preference
			{
				Category = null
			};

			var preferences = new Preference[] { };

			var mockRepository = new Mock<IRepository<Preference>>();

			mockRepository.Setup(r => r.GetAll())
				.Returns(preferences.AsQueryable);

			var controllerContext = new Mock<ControllerContext>();
			controllerContext.SetupGet(x => x.HttpContext.User)
				.Returns(this.mockPrincipal.Object);

			var controller =
				new UserController(mockRepository.Object, null, null)
				{
					ControllerContext = controllerContext.Object
				};

			Assert.AreEqual(0, preferences.Length);

			var viewResult = controller.AddPreference(
				new ProfileViewModel
				{
					SelectedCategory = preferenceToAdd.Category
				});

			Assert.AreEqual(0, preferences.Length);
			Assert.IsNull(controller.TempData["message"]);
			Assert.IsNotNull(controller.TempData["error"]);

			mockRepository.Verify(r => r.GetAll(), Times.Never());
			mockRepository.Verify(
					r => r.Add(It.Is<Preference>(p =>
						p.Category == preferenceToAdd.Category)), Times.Never());
			mockRepository.Verify(r => r.Save(), Times.Never());
		}

		[TestMethod]
		public void AddExistentPreferenceTest()
		{
			var preferenceToAdd = new Preference
			{
				Category = "test"
			};

			var preferences = new []
			{
				new Preference
				{
					Category = "test",
					UserId = this.username
				}
			};

			var mockRepository = new Mock<IRepository<Preference>>();

			mockRepository.Setup(r => r.GetAll())
				.Returns(preferences.AsQueryable);
            
			var controllerContext = new Mock<ControllerContext>();
			controllerContext.SetupGet(x => x.HttpContext.User)
				.Returns(this.mockPrincipal.Object);

			var controller =
				new UserController(mockRepository.Object, null, null)
				{
					ControllerContext = controllerContext.Object
				};

			Assert.AreEqual(1, preferences.Length);

			var viewResult = controller.AddPreference(
				new ProfileViewModel
				{
					SelectedCategory = preferenceToAdd.Category
				});

			Assert.AreEqual(1, preferences.Length);
			Assert.IsNull(controller.TempData["message"]);
			Assert.IsNotNull(controller.TempData["error"]);

			mockRepository.Verify(r => r.GetAll(), Times.Once());
			mockRepository.Verify(
					r => r.Add(It.Is<Preference>(p =>
						p.Category == preferenceToAdd.Category)), Times.Never());
			mockRepository.Verify(r => r.Save(), Times.Never());
		}

		[TestMethod]
		public void AddNonExistentPreferenceTest()
		{
			var preferenceToAdd = new Preference
			{
				Category = "test1"
			};

			var preferences = new List<Preference>
			{
				new Preference
				{
					Category = "test",
					UserId = this.username
				}
			};

			var mockRepository = new Mock<IRepository<Preference>>();

			mockRepository.Setup(r => r.GetAll())
				.Returns(preferences.AsQueryable);
			mockRepository.Setup(r => r.Add(It.Is<Preference>(
				p => p.Category == preferenceToAdd.Category)))
				.Callback(() => preferences.Add(preferenceToAdd));
			mockRepository.Setup(r => r.Save()).Returns(1);

			var controllerContext = new Mock<ControllerContext>();
			controllerContext.SetupGet(x => x.HttpContext.User)
				.Returns(this.mockPrincipal.Object);

			var controller =
				new UserController(mockRepository.Object, null, null)
				{
					ControllerContext = controllerContext.Object
				};

			Assert.AreEqual(1, preferences.Count);

			var viewResult = controller.AddPreference(
				new ProfileViewModel
				{
					SelectedCategory = preferenceToAdd.Category
				});

			Assert.AreEqual(2, preferences.Count);
			Assert.IsNotNull(controller.TempData["message"]);
			Assert.IsNull(controller.TempData["error"]);

			mockRepository.Verify(r => r.GetAll(), Times.Once());
			mockRepository.Verify(
				r => r.Add(It.Is<Preference>(p =>
					p.Category == preferenceToAdd.Category)), Times.Once());
			mockRepository.Verify(r => r.Save(), Times.Once());
		}
	}
}
