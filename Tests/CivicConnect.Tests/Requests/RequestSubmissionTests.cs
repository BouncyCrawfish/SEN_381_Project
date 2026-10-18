using CivicConnect.Requests.Contracts;
using CivicConnect.Requests.Domain;

namespace CivicConnect.Tests.Requests;

/// <summary>TC-15: equivalence partitioning and boundary value analysis for FR-001, FR-002.</summary>
public class RequestSubmissionTests
{
    private const string GoodLocation = "12 Long Street";
    private const string GoodDescription = "Large pothole outside number twelve.";

    private static RequestHarness SignedInRequester()
    {
        var h = new RequestHarness();
        h.SignInAs(RequestHarness.RequesterId);
        return h;
    }

    // ---- TC-15: partitions of the submission form ----

    [Fact]
    public async Task TC15_ValidSubmission_CreatesSubmittedRequest_WithReferenceNumber()
    {
        var h = SignedInRequester();

        var result = await h.Service.SubmitRequestAsync(new SubmitRequestDto(1, GoodLocation, GoodDescription));

        Assert.True(result.IsSuccess);
        Assert.True(result.Value > 0);                                     // unique reference number
        var saved = h.Repo.Requests[result.Value];
        Assert.Equal(RequestStatus.Submitted, saved.Status);
        Assert.Equal(RequestHarness.RequesterId, saved.RequesterId);
        Assert.Single(saved.History);                                      // initial history row
    }

    [Theory]
    [InlineData(0L, GoodLocation, GoodDescription)]                         // no category chosen
    [InlineData(1L, "", GoodDescription)]                                   // missing location
    [InlineData(1L, "   ", GoodDescription)]                                // blank location
    [InlineData(1L, GoodLocation, "")]                                      // missing description
    [InlineData(1L, GoodLocation, "          ")]                            // blank description
    public async Task TC15_MissingOrBlankFields_AreRejected_AndNothingIsSaved(long category, string location, string description)
    {
        var h = SignedInRequester();

        var result = await h.Service.SubmitRequestAsync(new SubmitRequestDto(category, location, description));

        Assert.True(result.IsFailure);
        Assert.Equal(CivicConnect.Common.ErrorType.Validation, result.Error.Type);
        Assert.Empty(h.Repo.Requests);
    }

    [Theory]
    [InlineData(999L)]   // not in the controlled list
    [InlineData(3L)]     // exists but retired
    [InlineData(-5L)]
    public async Task TC15_CategoryOutsideTheApprovedList_IsRejected(long category)
    {
        var h = SignedInRequester();

        var result = await h.Service.SubmitRequestAsync(new SubmitRequestDto(category, GoodLocation, GoodDescription));

        Assert.True(result.IsFailure);
        Assert.Empty(h.Repo.Requests);
    }

    [Fact]
    public async Task TC15_FieldLevelMessages_NameTheMissingFields()
    {
        var h = SignedInRequester();

        var result = await h.Service.SubmitRequestAsync(new SubmitRequestDto(1, "", ""));

        Assert.Contains("Location is required.", result.Error.Message);
        Assert.Contains("Description is required.", result.Error.Message);
    }

    [Fact]
    public async Task TC15_SurroundingWhitespace_IsTrimmedBeforeSaving()
    {
        var h = SignedInRequester();

        var result = await h.Service.SubmitRequestAsync(new SubmitRequestDto(1, "  " + GoodLocation + "  ", "  " + GoodDescription + "  "));

        Assert.Equal(GoodLocation, h.Repo.Requests[result.Value].Location);
        Assert.Equal(GoodDescription, h.Repo.Requests[result.Value].Description);
    }

    // ---- TC-15: description length at, below and above the limits ----

    [Theory]
    [InlineData(RequestRules.MinDescriptionLength - 1, false)]   // just below the minimum
    [InlineData(RequestRules.MinDescriptionLength, true)]        // at the minimum
    [InlineData(RequestRules.MinDescriptionLength + 1, true)]    // just above the minimum
    [InlineData(RequestRules.MaxDescriptionLength - 1, true)]    // just below the maximum
    [InlineData(RequestRules.MaxDescriptionLength, true)]        // at the maximum
    [InlineData(RequestRules.MaxDescriptionLength + 1, false)]   // just above the maximum
    public async Task TC15_DescriptionLength_Boundaries(int length, bool accepted)
    {
        var h = SignedInRequester();

        var result = await h.Service.SubmitRequestAsync(new SubmitRequestDto(1, GoodLocation, new string('a', length)));

        Assert.Equal(accepted, result.IsSuccess);
    }

    [Theory]
    [InlineData(RequestRules.MaxLocationLength, true)]
    [InlineData(RequestRules.MaxLocationLength + 1, false)]
    public async Task TC15_LocationLength_Boundaries(int length, bool accepted)
    {
        var h = SignedInRequester();

        var result = await h.Service.SubmitRequestAsync(new SubmitRequestDto(1, new string('l', length), GoodDescription));

        Assert.Equal(accepted, result.IsSuccess);
    }
}
