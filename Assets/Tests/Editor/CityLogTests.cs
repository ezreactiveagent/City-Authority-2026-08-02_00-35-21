using CityAuthority.Accountability;
using CityAuthority.Data;
using NUnit.Framework;
using UnityEditor;

namespace CityAuthority.Tests.Editor
{
    public class CityLogTests
    {
        private static District LoadDistrict(string path) => AssetDatabase.LoadAssetAtPath<District>(path);

        [Test]
        public void Record_PreservesInsertionOrder()
        {
            var log = new CityLog();
            log.Record(new AccountabilityEvent("First", "first"));
            log.Record(new AccountabilityEvent("Second", "second"));

            Assert.AreEqual(2, log.Events.Count);
            Assert.AreEqual("First", log.Events[0].EventType);
            Assert.AreEqual("Second", log.Events[1].EventType);
        }

        [Test]
        public void ByCategory_ReturnsOnlyMatchingEntries()
        {
            var log = new CityLog();
            log.Record(new AccountabilityEvent("A", "a", category: AccountabilityCategory.IgnoredWarning));
            log.Record(new AccountabilityEvent("B", "b", category: AccountabilityCategory.ActionCompleted));
            log.Record(new AccountabilityEvent("C", "c", category: AccountabilityCategory.IgnoredWarning));

            Assert.AreEqual(2, log.CountByCategory(AccountabilityCategory.IgnoredWarning));
            Assert.AreEqual(1, log.CountByCategory(AccountabilityCategory.ActionCompleted));
            Assert.AreEqual(0, log.CountByCategory(AccountabilityCategory.RecommendationRejected));
        }

        [Test]
        public void ByDistrict_ReturnsOnlyEntriesForThatDistrict()
        {
            var downtown = LoadDistrict("Assets/Data/Slice/District_Downtown.asset");
            var riverside = LoadDistrict("Assets/Data/Slice/District_Riverside.asset");

            var log = new CityLog();
            log.Record(new AccountabilityEvent("A", "a", downtown));
            log.Record(new AccountabilityEvent("B", "b", riverside));
            log.Record(new AccountabilityEvent("C", "c", downtown));

            var downtownEvents = new System.Collections.Generic.List<AccountabilityEvent>(log.ByDistrict(downtown));
            Assert.AreEqual(2, downtownEvents.Count);

            var riversideEvents = new System.Collections.Generic.List<AccountabilityEvent>(log.ByDistrict(riverside));
            Assert.AreEqual(1, riversideEvents.Count);
        }

        [Test]
        public void ByLevel_ReturnsOnlyEntriesAtThatNotificationLevel()
        {
            var log = new CityLog();
            log.Record(new AccountabilityEvent("A", "a", relatedLevel: NotificationLevel.Warning));
            log.Record(new AccountabilityEvent("B", "b", relatedLevel: NotificationLevel.Critical));
            log.Record(new AccountabilityEvent("C", "c", relatedLevel: NotificationLevel.Warning));

            var warningEvents = new System.Collections.Generic.List<AccountabilityEvent>(log.ByLevel(NotificationLevel.Warning));
            Assert.AreEqual(2, warningEvents.Count);
        }
    }
}
