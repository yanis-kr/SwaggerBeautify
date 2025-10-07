using FluentAssertions;
using WebApi.Infrastructure.Mediator;

namespace WebApi.Tests.Infrastructure;

public class UnitTests
{
    [Fact]
    public void Unit_ValueShouldBeDefaultInstance()
    {
        // Arrange & Act
        var unit = Unit.Value;

        // Assert
        unit.Should().Be(default(Unit));
    }

    [Fact]
    public void Unit_TwoInstancesShouldBeEqual()
    {
        // Arrange
        var unit1 = Unit.Value;
        var unit2 = new Unit();

        // Act & Assert
        unit1.Should().Be(unit2);
        (unit1 == unit2).Should().BeTrue();
        (unit1 != unit2).Should().BeFalse();
    }

    [Fact]
    public void Unit_EqualsWithObject_ShouldReturnTrue()
    {
        // Arrange
        var unit = Unit.Value;
        object obj = new Unit();

        // Act & Assert
        unit.Equals(obj).Should().BeTrue();
    }

    [Fact]
    public void Unit_EqualsWithNonUnit_ShouldReturnFalse()
    {
        // Arrange
        var unit = Unit.Value;
        object obj = "not a unit";

        // Act & Assert
        unit.Equals(obj).Should().BeFalse();
    }

    [Fact]
    public void Unit_GetHashCode_ShouldReturnZero()
    {
        // Arrange
        var unit = Unit.Value;

        // Act
        var hashCode = unit.GetHashCode();

        // Assert
        hashCode.Should().Be(0);
    }

    [Fact]
    public void Unit_ToString_ShouldReturnParentheses()
    {
        // Arrange
        var unit = Unit.Value;

        // Act
        var str = unit.ToString();

        // Assert
        str.Should().Be("()");
    }
}

