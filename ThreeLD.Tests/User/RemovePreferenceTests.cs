using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;
using System.Linq;
using ThreeLD.DB.Models;
using ThreeLD.DB.Repositories;

namespace ThreeLD.Tests.User
{
    [TestClass]
    public class RemovePreferenceTests
    {
        private Mock<IRepository<Preference>> mockRepository;
        private Preference[] preferences;

        [TestInitialize]
        public void Init()
        {
            this.preferences = new Preference[]
            {
                new Preference()
                {
                    Id = 1,
                    Category = "test",
                    UserId = "testUserId"
                },
                new Preference()
                {
                    Id = 2,
                    Category = "test",
                    UserId = "testUserId"
                },
                new Preference()
                {
                    Id = 3,
                    Category = "test",
                    UserId = "testUserId"
                }
            };

            this.mockRepository = new Mock<IRepository<Preference>>();
            this.mockRepository.Setup(r => r.GetAll())
                .Returns(this.preferences.AsQueryable);
        }
        
    }
}
