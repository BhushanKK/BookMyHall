using FluentAssertions;
using BookMyHall.Domain.Notifications;

namespace BookMyHall.Domain.Tests.Notifications;

public sealed class NotificationTemplateTests
{
    [Fact]
    public void NotificationTemplate_Should_Be_Inactive_By_Default()
    {
        var template = new NotificationTemplate();
        template.IsActive.Should().BeFalse();
    }

    [Fact]
    public void NotificationTemplate_Should_Assign_NotificationTemplateId()
    {
        var template = new NotificationTemplate();
        var id = Guid.NewGuid();
        template.NotificationTemplateId = id;
        template.NotificationTemplateId.Should().Be(id);
    }

    [Fact]
    public void NotificationTemplate_Should_Assign_TemplateCode()
    {
        var template = new NotificationTemplate();
        template.TemplateCode = "BOOKING_CONFIRM";
        template.TemplateCode.Should().Be("BOOKING_CONFIRM");
    }

    [Fact]
    public void NotificationTemplate_Should_Assign_TemplateName()
    {
        var template = new NotificationTemplate();
        template.TemplateName = "Booking Confirmation";
        template.TemplateName.Should().Be("Booking Confirmation");
    }

    [Fact]
    public void NotificationTemplate_Should_Assign_NotificationType()
    {
        var template = new NotificationTemplate();
        template.NotificationType = "Email";
        template.NotificationType.Should().Be("Email");
    }

    [Fact]
    public void NotificationTemplate_Should_Assign_ModuleName()
    {
        var template = new NotificationTemplate();
        template.ModuleName = "Booking";
        template.ModuleName.Should().Be("Booking");
    }

    [Fact]
    public void NotificationTemplate_Should_Assign_Subject()
    {
        var template = new NotificationTemplate();
        template.Subject = "Booking Confirmed";
        template.Subject.Should().Be("Booking Confirmed");
    }

    [Fact]
    public void NotificationTemplate_Should_Assign_Body()
    {
        var template = new NotificationTemplate();
        template.Body = "Your booking has been confirmed.";
        template.Body.Should().Be("Your booking has been confirmed.");
    }

    [Fact]
    public void NotificationTemplate_Should_Assign_IsActive()
    {
        var template = new NotificationTemplate();
        template.IsActive = true;
        template.IsActive.Should().BeTrue();
    }

    [Fact]
    public void NotificationTemplate_Should_Assign_All_Properties()
    {
        var templateId = Guid.NewGuid();
        var template = new NotificationTemplate
        {
            NotificationTemplateId = templateId,
            TemplateCode = "BOOKING_CONFIRM",
            TemplateName = "Booking Confirmation",
            NotificationType = "Email",
            ModuleName = "Booking",
            Subject = "Booking Confirmed",
            Body = "Your booking has been confirmed.",
            IsActive = true
        };

        template.NotificationTemplateId.Should().Be(templateId);
        template.TemplateCode.Should().Be("BOOKING_CONFIRM");
        template.TemplateName.Should().Be("Booking Confirmation");
        template.NotificationType.Should().Be("Email");
        template.ModuleName.Should().Be("Booking");
        template.Subject.Should().Be("Booking Confirmed");
        template.Body.Should().Be("Your booking has been confirmed.");
        template.IsActive.Should().BeTrue();
    }

    [Fact]
    public void NotificationTemplate_Should_Have_Default_Values()
    {
        var template = new NotificationTemplate();
        template.NotificationTemplateId.Should().Be(Guid.Empty);
        template.TemplateCode.Should().BeEmpty();
        template.TemplateName.Should().BeEmpty();
        template.NotificationType.Should().BeEmpty();
        template.ModuleName.Should().BeEmpty();
        template.Subject.Should().BeEmpty();
        template.Body.Should().BeEmpty();
        template.IsActive.Should().BeFalse();
    }
}