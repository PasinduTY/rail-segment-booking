namespace RailBooking.Domain.Tests;

using RailBooking.Domain;
using Xunit;

public class SegmentTests
{
    [Fact]
    public void Constructor_Throws_WhenDestinationNotAfterOrigin()
    {
        Assert.Throws<ArgumentException>(() => new Segment(3, 3));
        Assert.Throws<ArgumentException>(() => new Segment(5, 2));
    }

    [Theory]
    // Colombo Fort(0) -> Kandy(3), then Kandy(3) -> Badulla(7): adjacent at
    // the shared station, must NOT overlap - this is the case the whole
    // "resell the vacated seat" feature depends on.
    [InlineData(0, 3, 3, 7, false)]
    [InlineData(3, 7, 0, 3, false)]
    // Genuinely disjoint, with a gap between them.
    [InlineData(0, 2, 5, 7, false)]
    // Partial overlap from the left and from the right.
    [InlineData(0, 5, 3, 7, true)]
    [InlineData(3, 7, 0, 5, true)]
    // One leg fully contains the other.
    [InlineData(0, 7, 2, 4, true)]
    [InlineData(2, 4, 0, 7, true)]
    // Two people trying to book the exact same leg.
    [InlineData(0, 3, 0, 3, true)]
    public void OverlapsWith_DetectsOverlapCorrectly(
        int aOrigin, int aDestination, int bOrigin, int bDestination, bool expectedOverlap)
    {
        var a = new Segment(aOrigin, aDestination);
        var b = new Segment(bOrigin, bDestination);

        Assert.Equal(expectedOverlap, a.OverlapsWith(b));
        Assert.Equal(expectedOverlap, b.OverlapsWith(a)); // overlap must be symmetric
    }
}
