using FluentAssertions;
using BookMyHall.Domain.Booking;

namespace BookMyHall.Domain.Tests.Entities.Bookings;

public sealed class BookingPaymentScheduleTests
{
    [Fact]
    public void BookingPaymentSchedule_Should_Assign_BookingPaymentScheduleId()
    {
        var paymentSchedule = new BookingPaymentSchedule();
        var id = Guid.NewGuid();
        paymentSchedule.BookingPaymentScheduleId = id;
        paymentSchedule.BookingPaymentScheduleId.Should().Be(id);
    }

    [Fact]
    public void BookingPaymentSchedule_Should_Assign_BookingId()
    {
        var paymentSchedule = new BookingPaymentSchedule();
        var bookingId = Guid.NewGuid();
        paymentSchedule.BookingId = bookingId;
        paymentSchedule.BookingId.Should().Be(bookingId);
    }

    [Fact]
    public void BookingPaymentSchedule_Should_Assign_InstallmentNo()
    {
        var paymentSchedule = new BookingPaymentSchedule();
        paymentSchedule.InstallmentNo = 1;
        paymentSchedule.InstallmentNo.Should().Be(1);
    }

    [Fact]
    public void BookingPaymentSchedule_Should_Assign_DueDate()
    {
        var paymentSchedule = new BookingPaymentSchedule();
        var dueDate = DateTime.Today.AddDays(30);
        paymentSchedule.DueDate = dueDate;
        paymentSchedule.DueDate.Should().Be(dueDate);
    }

    [Fact]
    public void BookingPaymentSchedule_Should_Assign_DueAmount()
    {
        var paymentSchedule = new BookingPaymentSchedule();
        paymentSchedule.DueAmount = 10000m;
        paymentSchedule.DueAmount.Should().Be(10000m);
    }

    [Fact]
    public void BookingPaymentSchedule_Should_Assign_PaidAmount()
    {
        var paymentSchedule = new BookingPaymentSchedule();
        paymentSchedule.PaidAmount = 5000m;
        paymentSchedule.PaidAmount.Should().Be(5000m);
    }

    [Fact]
    public void BookingPaymentSchedule_Should_Assign_BalanceAmount()
    {
        var paymentSchedule = new BookingPaymentSchedule();
        paymentSchedule.BalanceAmount = 5000m;
        paymentSchedule.BalanceAmount.Should().Be(5000m);
    }

    [Fact]
    public void BookingPaymentSchedule_Should_Assign_PaymentStatus()
    {
        var paymentSchedule = new BookingPaymentSchedule();
        paymentSchedule.PaymentStatus = true;
        paymentSchedule.PaymentStatus.Should().BeTrue();
    }

    [Fact]
    public void BookingPaymentSchedule_Should_Assign_Remarks()
    {
        var paymentSchedule = new BookingPaymentSchedule();
        paymentSchedule.Remarks = "First installment paid.";
        paymentSchedule.Remarks.Should().Be("First installment paid.");
    }

    [Fact]
    public void BookingPaymentSchedule_Should_Assign_All_Properties()
    {
        var paymentScheduleId = Guid.NewGuid();
        var bookingId = Guid.NewGuid();
        var dueDate = DateTime.Today.AddDays(30);
        var paymentSchedule = new BookingPaymentSchedule
        {
            BookingPaymentScheduleId = paymentScheduleId,
            BookingId = bookingId,
            InstallmentNo = 1,
            DueDate = dueDate,
            DueAmount = 10000m,
            PaidAmount = 5000m,
            BalanceAmount = 5000m,
            PaymentStatus = true,
            Remarks = "First installment paid."
        };

        paymentSchedule.BookingPaymentScheduleId.Should().Be(paymentScheduleId);
        paymentSchedule.BookingId.Should().Be(bookingId);
        paymentSchedule.InstallmentNo.Should().Be(1);
        paymentSchedule.DueDate.Should().Be(dueDate);
        paymentSchedule.DueAmount.Should().Be(10000m);
        paymentSchedule.PaidAmount.Should().Be(5000m);
        paymentSchedule.BalanceAmount.Should().Be(5000m);
        paymentSchedule.PaymentStatus.Should().BeTrue();
        paymentSchedule.Remarks.Should().Be("First installment paid.");
    }

    [Fact]
    public void BookingPaymentSchedule_Should_Have_Default_Values()
    {
        var paymentSchedule = new BookingPaymentSchedule();
        paymentSchedule.BookingPaymentScheduleId.Should().Be(Guid.Empty);
        paymentSchedule.BookingId.Should().Be(Guid.Empty);
        paymentSchedule.InstallmentNo.Should().Be(0);
        paymentSchedule.DueDate.Should().Be(default);
        paymentSchedule.DueAmount.Should().Be(0m);
        paymentSchedule.PaidAmount.Should().Be(0m);
        paymentSchedule.BalanceAmount.Should().Be(0m);
        paymentSchedule.PaymentStatus.Should().BeFalse();
        paymentSchedule.Remarks.Should().BeEmpty();
    }
}