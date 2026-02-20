using Microsoft.VisualStudio.TestTools.UnitTesting;
using Paketverfolgung;

namespace Paketverfolgung.Test;

[TestClass]
public class ValidationTests
{
    [TestMethod]
    public void IsRequiredFilled_Empty_ReturnsFalse()
    {
        Assert.IsFalse(Validation.IsRequiredFilled(""));
    }

    [TestMethod]
    public void IsRequiredFilled_Text_ReturnsTrue()
    {
        Assert.IsTrue(Validation.IsRequiredFilled("Max"));
    }

    [TestMethod]
    public void IsValidEmail_Valid_ReturnsTrue()
    {
        Assert.IsTrue(Validation.IsValidEmail("max@example.com"));
    }

    [TestMethod]
    public void IsValidEmail_Invalid_ReturnsFalse()
    {
        Assert.IsFalse(Validation.IsValidEmail("not-an-email"));
    }

    [TestMethod]
    public void TryParsePositiveInt_Valid_ReturnsTrueAndValue()
    {
        var ok = Validation.TryParsePositiveInt(" 42 ", out var value);
        Assert.IsTrue(ok);
        Assert.AreEqual(42, value);
    }
}
