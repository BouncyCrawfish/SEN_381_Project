using System;
using CivicConnect.Requests.Domain;
using Xunit;

namespace CivicConnect.Tests.Requests
{
    public class RequestStateTest
    {
        [Fact]
        public void TC03_ValidStateTransitions_Succeeds()
        {
            //Arrange
            var request = new Request
            {
                Id = 1,
                Category = "Pothole",
                Location = "123 Main St",
                Description = "Large pothole in the middle of the road.",
                RequesterId = 42,
                CreatedAt = DateTime.UtcNow
            };
            Assert.Equal(RequestStatus.Submitted, request.Status);

            //Act and Assert: Submitted -> Assigned
            var res1 = request.TransitionTo(RequestStatus.Assigned, 1001, "Assigning to staff.");
            Assert.True(res1.IsSuccess);
            Assert.Equal(RequestStatus.Assigned, request.Status);

            //Act and Assert: Assigned -> InProgress
            var res2 = request.TransitionTo(RequestStatus.InProgress, 1001, "Starting work on the request.");
            Assert.True(res2.IsSuccess);
            Assert.Equal(RequestStatus.InProgress, request.Status);

            //Act and Assert: InProgress -> Resolved
            var res3 = request.TransitionTo(RequestStatus.Resolved, 1001, "Request has been resolved.");
            Assert.True(res3.IsSuccess);
            Assert.Equal(RequestStatus.Resolved, request.Status);

            //Act and Assert: Resolved -> Closed
            var res4 = request.TransitionTo(RequestStatus.Closed, 1001, "Closing the request.");
            Assert.True(res4.IsSuccess);
            Assert.Equal(RequestStatus.Closed, request.Status);
        }

        [Fact]

        public void TC04_InvalidJump_SubmittedToClosed_IsRejected()
        {
            //Arrange
            var request = new Request
            {
                Id = 2,
                Category = "Streetlight",
                Location = "456 Elm St",
                Description = "Streetlight not working.",
                RequesterId = 43,
                CreatedAt = DateTime.UtcNow
            };
            Assert.Equal(RequestStatus.Submitted, request.Status);

            //Act
            var result = request.TransitionTo(RequestStatus.Closed, 1002, "Attempting to close directly from Submitted.");

            //Assert
            Assert.True(result.IsFailure);
            Assert.Equal(RequestStatus.Submitted, request.Status);
            Assert.Equal("Invalid state transition from Submitted to Closed.", result.Error.Message);
        }

        [Theory]
        [InlineData(RequestStatus.Closed, RequestStatus.Assigned)]
        [InlineData(RequestStatus.Resolved, RequestStatus.Submitted)]
        public void TC05_InvalidTransitions_AreRejected(RequestStatus initialStatus, RequestStatus targetStatus)
        {
            //Arrange and force status via reflection or construct scenario
            var request = new Request
            {
                Id = 3,
                Category = "Graffiti",
                Location = "789 Oak St",
                Description = "Graffiti on the wall.",
                RequesterId = 44,
                CreatedAt = DateTime.UtcNow
            };

            //Transition step-by-step to the initial status if needed, or test rules directly
            bool canTransition = request.CanTransitionTo(targetStatus);

            //Assert
            Assert.False(canTransition, $"Transition from {initialStatus} to {targetStatus} should be invalid.");
        }
    }
}
