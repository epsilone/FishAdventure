#region Usings

using UnityEngine;
using NUnit.Framework;

#endregion

[TestFixture]
public class CSharpTestDriverTests
{
    [Test]
    private void SomeSimpleTest()
    {
        Assert.That(true,Is.EqualTo(true));
    }
}
