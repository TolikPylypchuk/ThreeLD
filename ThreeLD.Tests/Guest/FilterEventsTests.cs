using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ThreeLD.Tests.Guest
{
    [TestClass]
    public class FilterEventsTests
    {
        /*List<Event> result = new List<Event>();

            var categoriesArray = categories.Split(',');

            foreach (string category in categoriesArray)
            {
                var currentEvents = this.events.GetAll()
                    .Where(e => e.IsApproved == true)
                    .Where(e => e.Category == category)
                    .Where(e =>
                        (start == null ||
                         DateTime.Compare(e.DateTime, start.Value) >= 0) &&
                        (end == null ||
                         DateTime.Compare(e.DateTime, end.Value) <= 0))
                    .ToList();
                
                    foreach (Event currentEvent in currentEvents)
                    {
                        result.Add(currentEvent);
                    }
            }
            
            return this.View(nameof(this.ViewEvents), result);
        */

        [TestMethod]
        public void FilterEventsTest()
        {

        }
    }
}
