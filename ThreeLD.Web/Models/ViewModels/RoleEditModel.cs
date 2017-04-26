using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using ThreeLD.DB.Models;

namespace ThreeLD.Web.Models.ViewModels
{
    [ExcludeFromCodeCoverage]
    public class RoleEditModel
    {
        public AppRole Role { get; set; }
        public IEnumerable<User> Members { get; set; }
        public IEnumerable<User> NonMembers { get; set; }
    }
}