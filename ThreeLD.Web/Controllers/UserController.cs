using System.Web.Mvc;

using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Linq;

using ThreeLD.DB.Models;
using ThreeLD.DB.Repositories;

namespace ThreeLD.Web.Controllers
{
    [Authorize(Roles = "User")]
    public class UserController : Controller
    {
        private IRepository<Preference> preferences;
        private IRepository<Event> events;

        public UserController(
            IRepository<Preference> preferences,
            IRepository<Event> events)
        {
            this.preferences = preferences;
            this.events = events;
        }

        [HttpPost]
        public ActionResult AddBookmark(int eventId)
        {
            var manager = this.UserManager;

            User currentUser = manager.FindById(User.Identity.GetUserId());
            Event chosenEvent = this.events.GetById(eventId);

            chosenEvent.BookmarkedBy.Add(currentUser);
            this.events.Save();

            currentUser.BookmarkedEvents.Add(chosenEvent);

            this.TempData["message"] =
                $"Event {chosenEvent.Name} has been bookmarked.";

            //Redirect to ViewAllEvents
            return this.RedirectToAction("");
        }

        [HttpPost]
        public ActionResult RemoveBookmark(int eventId)
        {
            var manager = this.UserManager;

            User currentUser = manager.FindById(User.Identity.GetUserId());
            Event chosenEvent = this.events.GetById(eventId);

            chosenEvent.BookmarkedBy.Remove(currentUser);
            int res = this.events.Save();

            currentUser.BookmarkedEvents.Add(chosenEvent);

            if (res != 0)
            {
                this.TempData["message"] =
                    $"Bookmark on event {chosenEvent.Name} " +
                    $"has been removed.";
            }

            //Redirect to ViewAllEvents
            return this.RedirectToAction("");
        }

        [HttpGet]
        public ViewResult ViewPreferences()
        {
            return this.View(this.preferences.GetAll()
                .Where(p => p.UserId == User.Identity.GetUserId()));
        }

        [HttpGet]
        public ViewResult AddPreference()
        {
            return this.View();
        }

        [HttpPost]
        public ActionResult AddPreference(string preferenceCategory)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View();
            }

            Preference newPreference = new Preference();
            newPreference.UserId = User.Identity.GetUserId();
            newPreference.Category = preferenceCategory;

            this.preferences.Add(newPreference);
            this.preferences.Save();

            this.TempData["message"] =
                $"Preference with category {newPreference.Category} " +
                $"has been created.";

            return this.RedirectToAction("ViewPreferences");
        }

        [HttpPost]
        public ActionResult RemovePreference(int id)
        {
            this.preferences.Delete(id);
            int res = this.preferences.Save();

            if (res != 0)
            {
                this.TempData["message"] =
                    $"Preference with category " +
                    $"{this.preferences.GetById(id).Category} " +
                    $"has been removed.";
            }

            return this.RedirectToAction("ViewPreferences");
        }

        private UserManager<User> UserManager
        {
            get
            {
                return new UserManager<User>(
                    new UserStore<User>(new DB.AppDbContext()));
            }
        }
    }
}
