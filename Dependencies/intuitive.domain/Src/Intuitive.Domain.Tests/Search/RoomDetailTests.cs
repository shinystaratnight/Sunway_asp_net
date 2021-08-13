namespace Intuitive.Domain.Tests.Search
{
    using System.Collections.Generic;
    using Intuitive.Domain.Search;
    using NUnit.Framework;
    
    /// <summary>
    /// Unit tests for the room detail data class
    /// </summary>
    [TestFixture]
    public class RoomDetailTests
    {
        #region Child12OrOver

        /// <summary>
        /// Child 12 Or Over test
        /// </summary>
        /// <param name="children">The number of children</param>
        /// <param name="childAgesCSV">The child ages</param>
        /// <param name="expected">The expected total</param>
        [TestCase(3, "2,3,7", 0)]
        [TestCase(3, "2,13,7", 1)]
        [TestCase(3, "2,3,12", 1)]
        [TestCase(3, "2,13,12", 2)]
        [TestCase(3, "12,13,12", 3)]
        public void Child12OrOver_Should_ReturnChildrenOver12_When_Called(int children, string childAgesCSV, int expected)
        {
            // Arrange
            RoomDetail roomDetail = new RoomDetail(0, children, 0, childAgesCSV);

            // Act
            int childrenOver12 = roomDetail.Child12OrOver;

            // Assert
            Assert.AreEqual(expected, childrenOver12);
        }

        #endregion

        #region ChildAndInfantAges

        /// <summary>
        /// Child and infant ages test
        /// </summary>
        [Test]
        public void ChildAndInfantAges_Should_IncludeChildAges_When_Called()
        {
            // Arrange
            RoomDetail roomDetail = new RoomDetail(0, 3, 0, "3,6,14");

            // Act
            var childAndInfantAges = roomDetail.ChildAndInfantAges();

            // Assert
            Assert.AreEqual(3, childAndInfantAges.Count);
            Assert.AreEqual(3, childAndInfantAges[0]);
            Assert.AreEqual(6, childAndInfantAges[1]);
            Assert.AreEqual(14, childAndInfantAges[2]);
        }

        /// <summary>
        /// Child and infant ages test
        /// </summary>
        [Test]
        public void ChildAndInfantAges_Should_IncludeInfants_When_Called()
        {
            // Arrange
            RoomDetail roomDetail = new RoomDetail(0, 3, 2, "3,6,14");

            // Act
            var childAndInfantAges = roomDetail.ChildAndInfantAges();

            // Assert
            Assert.AreEqual(5, childAndInfantAges.Count);
            Assert.AreEqual(3, childAndInfantAges[0]);
            Assert.AreEqual(6, childAndInfantAges[1]);
            Assert.AreEqual(14, childAndInfantAges[2]);
            Assert.AreEqual(0, childAndInfantAges[3]);
            Assert.AreEqual(0, childAndInfantAges[4]);
        }

        /// <summary>
        /// Child and infant ages test
        /// </summary>
        [Test]
        public void ChildAndInfantAges_Should_IncludeInfantsWithOverrideAges_When_OverrideAgeSpecified()
        {
            // Arrange
            int infantAge = 1;
            RoomDetail roomDetail = new RoomDetail(0, 3, 2, "3,6,14");

            // Act
            var childAndInfantAges = roomDetail.ChildAndInfantAges(infantAge);

            // Assert
            Assert.AreEqual(5, childAndInfantAges.Count);
            Assert.AreEqual(3, childAndInfantAges[0]);
            Assert.AreEqual(6, childAndInfantAges[1]);
            Assert.AreEqual(14, childAndInfantAges[2]);
            Assert.AreEqual(infantAge, childAndInfantAges[3]);
            Assert.AreEqual(infantAge, childAndInfantAges[4]);
        }

        #endregion
    }
}