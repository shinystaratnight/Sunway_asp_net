namespace Intuitive.Domain.Tests.Financial
{
    using Intuitive.Domain.Financial;
    using NUnit.Framework;

    /// <summary>
    /// Rounding rule tests
    /// </summary>
    [TestFixture]
    public class RoundingRuleTests
    {
        /// <summary>
        /// Get rounded value test
        /// </summary>
        [Test]
        public void GetRoundedValue_Should_RoundToWholeNumberBelow_When_RoundingRuleIsRoundAndDecimalPortionBelowHalf()
        {
            // Arrange
            RoundingRule roundingRule = new RoundingRule();
            decimal testDecimal = 1.49M;

            // Act
            decimal roundedValue = roundingRule.GetRoundedValue(testDecimal, "Round");

            // Assert
            Assert.AreEqual(1.0M, roundedValue);
        }

        /// <summary>
        /// Get rounded value test
        /// </summary>
        [Test]
        public void GetRoundedValue_Should_RoundToWholeNumberAbove_When_RoundingRuleIsRoundAndDecimalPortionAboveHalf()
        {
            // Arrange
            RoundingRule roundingRule = new RoundingRule();
            decimal testDecimal = 1.51M;

            // Act
            decimal roundedValue = roundingRule.GetRoundedValue(testDecimal, "Round");

            // Assert
            Assert.AreEqual(2.0M, roundedValue);
        }

        /// <summary>
        /// Get rounded value test
        /// </summary>
        [Test]
        public void GetRoundedValue_Should_RoundToWholeNumberBelow_When_RoundingRuleIsRoundTotalAndDecimalPortionBelowHalf()
        {
            // Arrange
            RoundingRule roundingRule = new RoundingRule();
            decimal testDecimal = 1.49M;

            // Act
            decimal roundedValue = roundingRule.GetRoundedValue(testDecimal, "Round (Total)");

            // Assert
            Assert.AreEqual(1.0M, roundedValue);
        }

        /// <summary>
        /// Get rounded value test
        /// </summary>
        [Test]
        public void GetRoundedValue_Should_RoundToWholeNumberAbove_When_RoundingRuleIsRoundTotalAndDecimalPortionAboveHalf()
        {
            // Arrange
            RoundingRule roundingRule = new RoundingRule();
            decimal testDecimal = 1.51M;

            // Act
            decimal roundedValue = roundingRule.GetRoundedValue(testDecimal, "Round (Total)");

            // Assert
            Assert.AreEqual(2.0M, roundedValue);
        }

        /// <summary>
        /// Get rounded value test
        /// </summary>
        [Test]
        public void GetRoundedValue_Should_RoundToWholeNumberAbove_When_RoundingRuleIsRoundUpAndDecimalPortionBelowHalf()
        {
            // Arrange
            RoundingRule roundingRule = new RoundingRule();
            decimal testDecimal = 1.49M;

            // Act
            decimal roundedValue = roundingRule.GetRoundedValue(testDecimal, "Round Up");

            // Assert
            Assert.AreEqual(2.0M, roundedValue);
        }

        /// <summary>
        /// Get rounded value test
        /// </summary>
        [Test]
        public void GetRoundedValue_Should_RoundToWholeNumberAbove_When_RoundingRuleIsRoundUpAndDecimalPortionAboveHalf()
        {
            // Arrange
            RoundingRule roundingRule = new RoundingRule();
            decimal testDecimal = 1.51M;

            // Act
            decimal roundedValue = roundingRule.GetRoundedValue(testDecimal, "Round Up");

            // Assert
            Assert.AreEqual(2.0M, roundedValue);
        }

        /// <summary>
        /// Get rounded value test
        /// </summary>
        [Test]
        public void GetRoundedValue_Should_KeepSameValue_When_RoundingRuleIsRoundUpAndDecimalPortionIsZero()
        {
            // Arrange
            RoundingRule roundingRule = new RoundingRule();
            decimal testDecimal = 2.0M;

            // Act
            decimal roundedValue = roundingRule.GetRoundedValue(testDecimal, "Round Up");

            // Assert
            Assert.AreEqual(2.0M, roundedValue);
        }

        /// <summary>
        /// Get rounded value test
        /// </summary>
        [Test]
        public void GetRoundedValue_Should_RoundToWholeNumberAbove_When_RoundingRuleIsRoundUpTotalAndDecimalPortionBelowHalf()
        {
            // Arrange
            RoundingRule roundingRule = new RoundingRule();
            decimal testDecimal = 1.49M;

            // Act
            decimal roundedValue = roundingRule.GetRoundedValue(testDecimal, "Round Up (Total)");

            // Assert
            Assert.AreEqual(2.0M, roundedValue);
        }

        /// <summary>
        /// Get rounded value test
        /// </summary>
        [Test]
        public void GetRoundedValue_Should_RoundToWholeNumberAbove_When_RoundingRuleIsRoundUpTotalAndDecimalPortionAboveHalf()
        {
            // Arrange
            RoundingRule roundingRule = new RoundingRule();
            decimal testDecimal = 1.51M;

            // Act
            decimal roundedValue = roundingRule.GetRoundedValue(testDecimal, "Round Up (Total)");

            // Assert
            Assert.AreEqual(2.0M, roundedValue);
        }

        /// <summary>
        /// Get rounded value test
        /// </summary>
        [Test]
        public void GetRoundedValue_Should_KeepSameValue_When_RoundingRuleIsRoundUpTotalAndDecimalPortionIsZero()
        {
            // Arrange
            RoundingRule roundingRule = new RoundingRule();
            decimal testDecimal = 2.0M;

            // Act
            decimal roundedValue = roundingRule.GetRoundedValue(testDecimal, "Round Up (Total)");

            // Assert
            Assert.AreEqual(2.0M, roundedValue);
        }

        /// <summary>
        /// Get rounded value test
        /// </summary>
        [Test]
        public void GetRoundedValue_Should_RoundToClosest9_When_ANumberEndingWith0IsPassed()
        {
            // Arrange
            RoundingRule roundingRule = new RoundingRule();
            decimal testDecimal = 10M;

            // Act
            decimal roundedValue = roundingRule.GetRoundedValue(testDecimal, "5's and 9's");

            // Assert
            Assert.AreEqual(9m, roundedValue);
        }

        /// <summary>
        /// Get rounded value test
        /// </summary>
        [Test]
        public void GetRoundedValue_Should_RoundToClosest9_When_ANumberEndingWith1IsPassed()
        {
            // Arrange
            RoundingRule roundingRule = new RoundingRule();
            decimal testDecimal = 11M;

            // Act
            decimal roundedValue = roundingRule.GetRoundedValue(testDecimal, "5's and 9's");

            // Assert
            Assert.AreEqual(9m, roundedValue);
        }

        /// <summary>
        /// Get rounded value test
        /// </summary>
        [Test]
        public void GetRoundedValue_Should_RoundToClosest5_When_ANumberEndingWith2IsPassed()
        {
            // Arrange
            RoundingRule roundingRule = new RoundingRule();
            decimal testDecimal = 122M;

            // Act
            decimal roundedValue = roundingRule.GetRoundedValue(testDecimal, "5's and 9's");

            // Assert
            Assert.AreEqual(125m, roundedValue);
        }

        /// <summary>
        /// Get rounded value test
        /// </summary>
        [Test]
        public void GetRoundedValue_Should_RoundToClosest5_When_ANumberEndingWith3IsPassed()
        {
            // Arrange
            RoundingRule roundingRule = new RoundingRule();
            decimal testDecimal = 123M;

            // Act
            decimal roundedValue = roundingRule.GetRoundedValue(testDecimal, "5's and 9's");

            // Assert
            Assert.AreEqual(125m, roundedValue);
        }

        /// <summary>
        /// Get rounded value test
        /// </summary>
        [Test]
        public void GetRoundedValue_Should_RoundToClosest5_When_ANumberEndingWith4IsPassed()
        {
            // Arrange
            RoundingRule roundingRule = new RoundingRule();
            decimal testDecimal = 124M;

            // Act
            decimal roundedValue = roundingRule.GetRoundedValue(testDecimal, "5's and 9's");

            // Assert
            Assert.AreEqual(125m, roundedValue);
        }

        /// <summary>
        /// Get rounded value test
        /// </summary>
        [Test]
        public void GetRoundedValue_Should_RoundToClosest5_When_ANumberEndingWith5IsPassed()
        {
            // Arrange
            RoundingRule roundingRule = new RoundingRule();
            decimal testDecimal = 125M;

            // Act
            decimal roundedValue = roundingRule.GetRoundedValue(testDecimal, "5's and 9's");

            // Assert
            Assert.AreEqual(125m, roundedValue);
        }

        /// <summary>
        /// Get rounded value test
        /// </summary>
        [Test]
        public void GetRoundedValue_Should_RoundToClosest5_When_ANumberEndingWith6IsPassed()
        {
            // Arrange
            RoundingRule roundingRule = new RoundingRule();
            decimal testDecimal = 126M;

            // Act
            decimal roundedValue = roundingRule.GetRoundedValue(testDecimal, "5's and 9's");

            // Assert
            Assert.AreEqual(125m, roundedValue);
        }

        /// <summary>
        /// Get rounded value test
        /// </summary>
        [Test]
        public void GetRoundedValue_Should_RoundToClosest9_When_ANumberEndingWith7IsPassed()
        {
            // Arrange
            RoundingRule roundingRule = new RoundingRule();
            decimal testDecimal = 127M;

            // Act
            decimal roundedValue = roundingRule.GetRoundedValue(testDecimal, "5's and 9's");

            // Assert
            Assert.AreEqual(129m, roundedValue);
        }

        /// <summary>
        /// Get rounded value test
        /// </summary>
        [Test]
        public void GetRoundedValue_Should_RoundToClosest9_When_ANumberEndingWith8IsPassed()
        {
            // Arrange
            RoundingRule roundingRule = new RoundingRule();
            decimal testDecimal = 128M;

            // Act
            decimal roundedValue = roundingRule.GetRoundedValue(testDecimal, "5's and 9's");

            // Assert
            Assert.AreEqual(129m, roundedValue);
        }

        /// <summary>
        /// Get rounded value test
        /// </summary>
        [Test]
        public void GetRoundedValue_Should_RoundToClosest9_When_ANumberEndingWith9IsPassed()
        {
            // Arrange
            RoundingRule roundingRule = new RoundingRule();
            decimal testDecimal = 129M;

            // Act
            decimal roundedValue = roundingRule.GetRoundedValue(testDecimal, "5's and 9's");

            // Assert
            Assert.AreEqual(129m, roundedValue);
        }

        /// <summary>
        /// Get rounded value test
        /// </summary>
        [Test]
        public void GetRoundedValue_Should_RoundToClosest5_When_Number1IsPassed()
        {
            // Arrange
            RoundingRule roundingRule = new RoundingRule();
            decimal testDecimal = 1m;

            // Act
            decimal roundedValue = roundingRule.GetRoundedValue(testDecimal, "5's and 9's");

            // Assert
            Assert.AreEqual(5m, roundedValue);
        }

        /// <summary>
        /// Get rounded value test
        /// </summary>
        [Test]
        public void GetRoundedValue_Should_StayAt0_When_Number0IsPassed()
        {
            // Arrange
            RoundingRule roundingRule = new RoundingRule();
            decimal testDecimal = 0m;

            // Act
            decimal roundedValue = roundingRule.GetRoundedValue(testDecimal, "5's and 9's");

            // Assert
            Assert.AreEqual(0m, roundedValue);
        }

        /// <summary>
        /// Get rounded value test
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="output">The output.</param>
        [TestCase(1.01, 5.0)]
        [TestCase(5.01, 5.0)]
        [TestCase(6.9, 5.0)]
        [TestCase(7.1, 9.0)]
        [TestCase(11.51, 9.0)]
        [TestCase(12.51, 15.0)]
        [TestCase(12.09, 15.0)]
        public void GetRoundedValue_Should_RoundCorrectly_When_NumberIsDecimal(decimal input, decimal output)
        {
            // Arrange
            RoundingRule roundingRule = new RoundingRule();

            // Act
            decimal roundedValue = roundingRule.GetRoundedValue(input, "5's and 9's");

            // Assert
            Assert.AreEqual(output, roundedValue);
        }

        /// <summary>
        /// Get rounded value test
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="output">The output.</param>
        [TestCase(1.0, 9.0)]
        [TestCase(5.0, 9.0)]
        [TestCase(6.0, 9.0)]
        [TestCase(7.0, 9.0)]
        [TestCase(11.0, 19.0)]
        [TestCase(12.0, 19.0)]
        [TestCase(26.0, 29.0)]
        public void GetRoundedValue_Should_RoundUpTo9_When_UpTo9RuleIsUsed(decimal input, decimal output)
        {
            // Arrange
            RoundingRule roundingRule = new RoundingRule();

            // Act
            decimal roundedValue = roundingRule.GetRoundedValue(input, "Up To 9");

            // Assert
            Assert.AreEqual(output, roundedValue);
        }

        /// <summary>
        /// Get rounded value test
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="output">The output.</param>
        [TestCase(0.0, 0.0)]
        [TestCase(1.0, 9.0)]
        [TestCase(5.0, 9.0)]
        [TestCase(6.0, 9.0)]
        [TestCase(9.0, 9.0)]
        [TestCase(10.0, 9.0)]
        [TestCase(11.0, 9.0)]
        [TestCase(12.0, 9.0)]
        [TestCase(20.0, 19.0)]
        [TestCase(26.0, 19.0)]
        public void GetRoundedValue_Should_RoundDownTo9_When_DownTo9RuleIsUsed(decimal input, decimal output)
        {
            // Arrange
            RoundingRule roundingRule = new RoundingRule();

            // Act
            decimal roundedValue = roundingRule.GetRoundedValue(input, "Down To 9");

            // Assert
            Assert.AreEqual(output, roundedValue);
        }

        /// <summary>
        /// Get rounded value test
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="output">The output.</param>
        [TestCase(0.0, 0.0)]
        [TestCase(1.0, 9.0)]
        [TestCase(5.0, 9.0)]
        [TestCase(7.0, 9.0)]
        [TestCase(10.0, 9.0)]
        [TestCase(12.0, 9.0)]
        [TestCase(20.0, 19.0)]
        [TestCase(24.0, 19.0)]
        [TestCase(26.0, 29.0)]
        public void GetRoundedValue_Should_RoundToNearest9_When_To9RuleIsUsed(decimal input, decimal output)
        {
            // Arrange
            RoundingRule roundingRule = new RoundingRule();

            // Act
            decimal roundedValue = roundingRule.GetRoundedValue(input, "To 9");

            // Assert
            Assert.AreEqual(output, roundedValue);
        }

        /// <summary>
        /// Get rounded value test
        /// </summary>
        [Test]
        public void GetRoundedValue_Should_ReturnSameDecimal_When_UnRoundingRuleedRoundingRuleUsed()
        {
            // Arrange
            RoundingRule roundingRule = new RoundingRule();
            decimal testDecimal = 1.01m;

            // Act
            decimal roundedValue = roundingRule.GetRoundedValue(testDecimal, "Denis");

            // Assert
            Assert.AreEqual(testDecimal, roundedValue);
        }
    }
}
