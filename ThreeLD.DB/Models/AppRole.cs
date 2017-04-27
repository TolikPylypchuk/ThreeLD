using Microsoft.AspNet.Identity.EntityFramework;

namespace ThreeLD.DB.Models
{
    public class AppRole : IdentityRole
    {
        public AppRole() { }
        public AppRole(string name) : base(name) { }
    }
}
