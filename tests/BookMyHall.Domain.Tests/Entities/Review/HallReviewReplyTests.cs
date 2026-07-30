using FluentAssertions;
using BookMyHall.Domain.Review;

namespace BookMyHall.Domain.Tests.Review;

public sealed class HallReviewReplyTests
{
    [Fact]
    public void HallReviewReply_Should_Assign_HallReviewReplyId()
    {
        var reply = new HallReviewReply();
        var id = Guid.NewGuid();
        reply.HallReviewReplyId = id;
        reply.HallReviewReplyId.Should().Be(id);
    }

    [Fact]
    public void HallReviewReply_Should_Assign_HallReviewId()
    {
        var reply = new HallReviewReply();
        var reviewId = Guid.NewGuid();
        reply.HallReviewId = reviewId;
        reply.HallReviewId.Should().Be(reviewId);
    }

    [Fact]
    public void HallReviewReply_Should_Assign_Reply()
    {
        var reply = new HallReviewReply();
        reply.Reply = "Thank you for your valuable feedback.";
        reply.Reply.Should().Be("Thank you for your valuable feedback.");
    }

    [Fact]
    public void HallReviewReply_Should_Assign_RepliedBy()
    {
        var reply = new HallReviewReply();
        var repliedBy = Guid.NewGuid();
        reply.RepliedBy = repliedBy;
        reply.RepliedBy.Should().Be(repliedBy);
    }

    [Fact]
    public void HallReviewReply_Should_Assign_All_Properties()
    {
        var replyId = Guid.NewGuid();
        var reviewId = Guid.NewGuid();
        var repliedBy = Guid.NewGuid();
        var reply = new HallReviewReply
        {
            HallReviewReplyId = replyId,
            HallReviewId = reviewId,
            Reply = "Thank you for your valuable feedback.",
            RepliedBy = repliedBy
        };

        reply.HallReviewReplyId.Should().Be(replyId);
        reply.HallReviewId.Should().Be(reviewId);
        reply.Reply.Should().Be("Thank you for your valuable feedback.");
        reply.RepliedBy.Should().Be(repliedBy);
    }

    [Fact]
    public void HallReviewReply_Should_Have_Default_Values()
    {
        var reply = new HallReviewReply();
        reply.HallReviewReplyId.Should().Be(Guid.Empty);
        reply.HallReviewId.Should().Be(Guid.Empty);
        reply.Reply.Should().BeEmpty();
        reply.RepliedBy.Should().Be(Guid.Empty);
    }
}