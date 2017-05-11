using System;
using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using System.Security.Principal;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

namespace ThreeLD.Tests.Editor
{
	[TestClass]
	[SuppressMessage("ReSharper", "ImplicitlyCapturedClosure")]
	[SuppressMessage("ReSharper", "UnusedVariable")]
	[SuppressMessage("ReSharper", "InconsistentNaming")]
	public class ViewEventsTests
	{
		private Mock<IPrincipal> mockPrincipal;
		private const string userId = "a";

		[TestInitialize]
		public void Init()
		{
			this.mockPrincipal = new Mock<IPrincipal>();
			var identity = new GenericIdentity(userId);

			var nameIdentifierClaim = new Claim(
				ClaimTypes.NameIdentifier, userId);
			identity.AddClaim(nameIdentifierClaim);

			this.mockPrincipal.Setup(x => x.Identity)
				.Returns(identity);
		}

		[TestMethod]
		public void ViewEventsTest()
		{

		}
	}
}
