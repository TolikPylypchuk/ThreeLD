﻿using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using ThreeLD.DB.Models;
using ThreeLD.Web.Controllers;
 
namespace ThreeLD.Tests.User
{
	[TestClass]
	public class ProposeEventTests
	{
		[TestMethod]
		public void ProposeEventGetTest()
		{
			var controller = new UserController(null, null, null);
			var result = controller.ProposeEvent();

			Assert.IsNotNull(result.Model);

			var eventModel = (Event)result.Model;

			Assert.AreEqual(eventModel.Name, null);
			Assert.AreEqual(eventModel.DateTime, new DateTime());
			Assert.AreEqual(eventModel.Duration, new TimeSpan());
			Assert.AreEqual(eventModel.Address, null);
			Assert.AreEqual(eventModel.Url, null);
			Assert.AreEqual(eventModel.Description, null);
			Assert.AreEqual(eventModel.Category, null);
		}

		[TestMethod]
		public void ProposeEventPostTest()
		{
		}
	}
}