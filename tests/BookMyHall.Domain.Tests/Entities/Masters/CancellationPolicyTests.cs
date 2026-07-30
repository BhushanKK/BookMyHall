using FluentAssertions;
using BookMyHall.Domain.Masters;

namespace BookMyHall.Domain.Tests.Entities.Master;

public sealed class CancellationPolicyTests
{
    [Fact]
    public void CancellationPolicy_Should_Be_Inactive_By_Default()
    {
        var cancellationPolicy = new CancellationPolicy();
        cancellationPolicy.IsActive.Should().BeFalse();
    }

    [Fact]
    public void CancellationPolicy_Should_Assign_CancellationPolicyId()
    {
        var cancellationPolicy = new CancellationPolicy();
        var id = Guid.NewGuid();
        cancellationPolicy.CancellationPolicyId = id;
        cancellationPolicy.CancellationPolicyId.Should().Be(id);
    }

    [Fact]
    public void CancellationPolicy_Should_Assign_PolicyName()
    {
        var cancellationPolicy = new CancellationPolicy();
        cancellationPolicy.PolicyName = "Standard Policy";
        cancellationPolicy.PolicyName.Should().Be("Standard Policy");
    }

    [Fact]
    public void CancellationPolicy_Should_Assign_Description()
    {
        var cancellationPolicy = new CancellationPolicy();
        cancellationPolicy.Description = "50% refund before 48 hours.";
        cancellationPolicy.Description.Should().Be("50% refund before 48 hours.");
    }

    [Fact]
    public void CancellationPolicy_Should_Assign_RefundPercentage()
    {
        var cancellationPolicy = new CancellationPolicy();
        cancellationPolicy.RefundPercentage = 50m;
        cancellationPolicy.RefundPercentage.Should().Be(50m);
    }

    [Fact]
    public void CancellationPolicy_Should_Assign_CancellationBeforeHours()
    {
        var cancellationPolicy = new CancellationPolicy();
        cancellationPolicy.CancellationBeforeHours = 48;
        cancellationPolicy.CancellationBeforeHours.Should().Be(48);
    }

    [Fact]
    public void CancellationPolicy_Should_Assign_IsActive()
    {
        var cancellationPolicy = new CancellationPolicy();
        cancellationPolicy.IsActive = true;
        cancellationPolicy.IsActive.Should().BeTrue();
    }

    [Fact]
    public void CancellationPolicy_Should_Assign_All_Properties()
    {
        var cancellationPolicyId = Guid.NewGuid();
        var cancellationPolicy = new CancellationPolicy
        {
            CancellationPolicyId = cancellationPolicyId,
            PolicyName = "Standard Policy",
            Description = "50% refund before 48 hours.",
            RefundPercentage = 50m,
            CancellationBeforeHours = 48,
            IsActive = true
        };

        cancellationPolicy.CancellationPolicyId.Should().Be(cancellationPolicyId);
        cancellationPolicy.PolicyName.Should().Be("Standard Policy");
        cancellationPolicy.Description.Should().Be("50% refund before 48 hours.");
        cancellationPolicy.RefundPercentage.Should().Be(50m);
        cancellationPolicy.CancellationBeforeHours.Should().Be(48);
        cancellationPolicy.IsActive.Should().BeTrue();
    }

    [Fact]
    public void CancellationPolicy_Should_Have_Default_Values()
    {
        var cancellationPolicy = new CancellationPolicy();
        cancellationPolicy.CancellationPolicyId.Should().Be(Guid.Empty);
        cancellationPolicy.PolicyName.Should().BeEmpty();
        cancellationPolicy.Description.Should().BeEmpty();
        cancellationPolicy.RefundPercentage.Should().Be(0m);
        cancellationPolicy.CancellationBeforeHours.Should().Be(0);
        cancellationPolicy.IsActive.Should().BeFalse();
    }
}