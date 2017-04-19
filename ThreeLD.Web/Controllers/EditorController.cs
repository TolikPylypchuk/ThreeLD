using System.Web.Mvc;

using ThreeLD.DB.Models;

namespace ThreeLD.Web.Controllers
{
	public class EditorController : Controller
	{
		[HttpGet]
		[Authorize(Roles = "Editor")]
		public ViewResult CreateEvent()
		{
			return this.View(new Event());
		}
	}
}
