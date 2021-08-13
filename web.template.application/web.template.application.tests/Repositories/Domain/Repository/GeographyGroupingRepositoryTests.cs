////namespace Web.Template.Application.Tests.Repositories.Domain.Repository
////{
////    using System.Collections.Generic;

////    using NUnit.Framework;

////    using Web.Template.Domain.Entities.Geography;

////    /// <summary>
////    ///     Class of tests used to test the Geography Grouping Repository.
////    /// </summary>
////    [TestFixture]
////    internal class GeographyGroupingRepositoryTests
////    {
////        /////// <summary>
////        ///////     Geography Group Repository should return a geography group for the correspondingID when supplied an ID
////        /////// </summary>
////        ////[Test]
////        ////public void GeoGrpRepo_Should_ReturnGeoGrpWithTheCorrespondingID_When_SuppliedCorrectID()
////        ////{
////        ////    ////Arrange
////        ////    var geographyGroupFactoryMock = new Mock<GeographyGroupFactory>();
////        ////    geographyGroupFactoryMock.Setup(factory => factory.Create()).Returns(FakeGeographyGroup());
////        ////    IGeographyGroupingRepository geoGroupRepo = new GeographyGroupingRepository(geographyGroupFactoryMock.Object);

////        ////    ////Act
////        ////    GeographyGrouping geogGroup = geoGroupRepo.GetGeographyGroupById(1);

////        ////    ////Assert
////        ////    ////Assert.AreEqual(geogGroup.Name, "TestGroupA");
////        ////    ////Assert.IsTrue(geogGroup.ShowInSearch);
////        ////    ////Assert.AreEqual(geogGroup.Geographies.Count, 2);
////        ////    ////Assert.AreEqual(geogGroup.Geographies[0].Level1Name, "level1NameA");
////        ////    ////Assert.AreEqual(geogGroup.Geographies[0].Level1Id, 1);
////        ////    ////Assert.AreEqual(geogGroup.Geographies[0].Level2Name, "level2NameA");
////        ////    ////Assert.AreEqual(geogGroup.Geographies[0].Level2Id, 2);
////        ////    ////Assert.AreEqual(geogGroup.Geographies[0].Level3Name, "level3NameA");
////        ////    ////Assert.AreEqual(geogGroup.Geographies[0].Level3Id, 3);
////        ////}

////        /////// <summary>
////        ///////     Geography Group Repository should return null when Given a fake ID.
////        /////// </summary>
////        ////[Test]
////        ////public void GeoGrpRepo_Should_ReturnNull_When_NonExistantIDSupplied()
////        ////{
////        ////    ////Arrange
////        ////    var geographyGroupFactoryMock = new Mock<GeographyGroupFactory>();
////        ////    geographyGroupFactoryMock.Setup(factory => factory.Create()).Returns(FakeGeographyGroup());
////        ////    IGeographyGroupingRepository geoGroupRepo = new GeographyGroupingRepository(geographyGroupFactoryMock.Object);

////        ////    ////Act
////        ////    GeographyGrouping geogGroup = geoGroupRepo.GetGeographyGroupById(3);

////        ////    ////Assert
////        ////    Assert.IsNull(geogGroup);
////        ////}

////        /// <summary>
////        ///     Function used to mock geography groups
////        /// </summary>
////        /// <returns>A fake list of geographies</returns>
////        private static List<GeographyGrouping> FakeGeographyGroup()
////        {
////            return new List<GeographyGrouping> { ////new GeographyGrouping
////                                                   ////    {
////                                                   ////        Geographies =
////                                                   ////            new List<Geography>
////                                                   ////                {
////                                                   ////                    new Geography
////                                                   ////                        {
////                                                   ////                            Level1Name = "level1NameA", 
////                                                   ////                            Level1Id = 1, 
////                                                   ////                            Level2Name = "level2NameA", 
////                                                   ////                            Level2Id = 2, 
////                                                   ////                            Level3Name = "level3NameA", 
////                                                   ////                            Level3Id = 3
////                                                   ////                        }, 
////                                                   ////                    new Geography
////                                                   ////                        {
////                                                   ////                            Level1Name = "level1NameB", 
////                                                   ////                            Level1Id = 11, 
////                                                   ////                            Level2Name = "level2NameB", 
////                                                   ////                            Level2Id = 22, 
////                                                   ////                            Level3Name = "level3NameB", 
////                                                   ////                            Level3Id = 33
////                                                   ////                        }
////                                                   ////                }, 
////                                                   ////        Id = 1, 
////                                                   ////        Name = "TestGroupA", 
////                                                   ////        ShowInSearch = true
////                                                   ////    }, 
////                                                   ////new GeographyGrouping
////                                                   ////    {
////                                                   ////        Geographies =
////                                                   ////            new List<Geography>
////                                                   ////                {
////                                                   ////                    new Geography
////                                                   ////                        {
////                                                   ////                            Level1Name = "level1NameC", 
////                                                   ////                            Level1Id = 4, 
////                                                   ////                            Level2Name = "level2NameC", 
////                                                   ////                            Level2Id = 5, 
////                                                   ////                            Level3Name = "level3NameC", 
////                                                   ////                            Level3Id = 6
////                                                   ////                        }, 
////                                                   ////                    new Geography
////                                                   ////                        {
////                                                   ////                            Level1Name = "level1NameD", 
////                                                   ////                            Level1Id = 44, 
////                                                   ////                            Level2Name = "level2NameD", 
////                                                   ////                            Level2Id = 55, 
////                                                   ////                            Level3Name = "level3NameD", 
////                                                   ////                            Level3Id = 66
////                                                   ////                        }
////                                                   ////                }, 
////                                                   ////        Id = 2, 
////                                                   ////        Name = "TestGroupB", 
////                                                   ////        ShowInSearch = true
////                                                   ////    }
////                                               };
////        }
////    }
////}
