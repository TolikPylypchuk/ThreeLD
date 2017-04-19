using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using ThreeLD.DB.Models;
using ThreeLD.Web.Controllers;

namespace ThreeLD.Tests.Editor
{
	[TestClass]
	public class CreateEventTests
	{
		[TestMethod]
		public void CreateEventGetTest()
		{
			var controller = new EditorController();

			var result = controller.CreateEvent();

			var model = result.Model as Event;

			Assert.IsNotNull(model);

			Assert.AreEqual(null, model.Name);
			Assert.AreEqual(default(DateTime), model.DateTime);
			Assert.AreEqual(default(TimeSpan), model.Duration);
			Assert.AreEqual(null, model.Address);
			Assert.AreEqual(null, model.Url);
			Assert.AreEqual(null, model.Description);
			Assert.AreEqual(null, model.Category);
		}
	}
}
