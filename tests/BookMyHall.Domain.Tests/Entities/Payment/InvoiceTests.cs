using FluentAssertions;
using BookMyHall.Domain.Payments;

namespace BookMyHall.Domain.Tests.Paymnents;

public sealed class InvoiceTests
{
    [Fact]
    public void Invoice_Should_Assign_InvoiceId()
    {
        var invoice = new Invoice();
        var id = Guid.NewGuid();
        invoice.InvoiceId = id;
        invoice.InvoiceId.Should().Be(id);
    }

    [Fact]
    public void Invoice_Should_Assign_BookingId()
    {
        var invoice = new Invoice();
        var bookingId = Guid.NewGuid();
        invoice.BookingId = bookingId;
        invoice.BookingId.Should().Be(bookingId);
    }

    [Fact]
    public void Invoice_Should_Assign_InvoiceNumber()
    {
        var invoice = new Invoice();
        invoice.InvoiceNumber = "INV-2026001";
        invoice.InvoiceNumber.Should().Be("INV-2026001");
    }

    [Fact]
    public void Invoice_Should_Assign_PaymentId()
    {
        var invoice = new Invoice();
        var paymentId = Guid.NewGuid();
        invoice.PaymentId = paymentId;
        invoice.PaymentId.Should().Be(paymentId);
    }

    [Fact]
    public void Invoice_Should_Assign_InvoiceDate()
    {
        var invoice = new Invoice();
        var invoiceDate = DateTimeOffset.UtcNow;
        invoice.InvoiceDate = invoiceDate;
        invoice.InvoiceDate.Should().Be(invoiceDate);
    }

    [Fact]
    public void Invoice_Should_Assign_SubTotal()
    {
        var invoice = new Invoice();
        invoice.SubTotal = 10000m;
        invoice.SubTotal.Should().Be(10000m);
    }

    [Fact]
    public void Invoice_Should_Assign_TaxAmount()
    {
        var invoice = new Invoice();
        invoice.TaxAmount = 1800m;
        invoice.TaxAmount.Should().Be(1800m);
    }

    [Fact]
    public void Invoice_Should_Assign_DiscountAmount()
    {
        var invoice = new Invoice();
        invoice.DiscountAmount = 500m;
        invoice.DiscountAmount.Should().Be(500m);
    }

    [Fact]
    public void Invoice_Should_Assign_TotalAmount()
    {
        var invoice = new Invoice();
        invoice.TotalAmount = 11300m;
        invoice.TotalAmount.Should().Be(11300m);
    }

    [Fact]
    public void Invoice_Should_Assign_InvoiceStatus()
    {
        var invoice = new Invoice();
        invoice.InvoiceStatus = "Paid";
        invoice.InvoiceStatus.Should().Be("Paid");
    }

    [Fact]
    public void Invoice_Should_Assign_PdfUrl()
    {
        var invoice = new Invoice();
        invoice.PdfUrl = "/invoices/INV-2026001.pdf";
        invoice.PdfUrl.Should().Be("/invoices/INV-2026001.pdf");
    }

    [Fact]
    public void Invoice_Should_Assign_Remarks()
    {
        var invoice = new Invoice();
        invoice.Remarks = "Invoice generated successfully.";
        invoice.Remarks.Should().Be("Invoice generated successfully.");
    }

    [Fact]
    public void Invoice_Should_Assign_All_Properties()
    {
        var invoiceId = Guid.NewGuid();
        var bookingId = Guid.NewGuid();
        var paymentId = Guid.NewGuid();
        var invoiceDate = DateTimeOffset.UtcNow;

        var invoice = new Invoice
        {
            InvoiceId = invoiceId,
            BookingId = bookingId,
            InvoiceNumber = "INV-2026001",
            PaymentId = paymentId,
            InvoiceDate = invoiceDate,
            SubTotal = 10000m,
            TaxAmount = 1800m,
            DiscountAmount = 500m,
            TotalAmount = 11300m,
            InvoiceStatus = "Paid",
            PdfUrl = "/invoices/INV-2026001.pdf",
            Remarks = "Invoice generated successfully."
        };

        invoice.InvoiceId.Should().Be(invoiceId);
        invoice.BookingId.Should().Be(bookingId);
        invoice.InvoiceNumber.Should().Be("INV-2026001");
        invoice.PaymentId.Should().Be(paymentId);
        invoice.InvoiceDate.Should().Be(invoiceDate);
        invoice.SubTotal.Should().Be(10000m);
        invoice.TaxAmount.Should().Be(1800m);
        invoice.DiscountAmount.Should().Be(500m);
        invoice.TotalAmount.Should().Be(11300m);
        invoice.InvoiceStatus.Should().Be("Paid");
        invoice.PdfUrl.Should().Be("/invoices/INV-2026001.pdf");
        invoice.Remarks.Should().Be("Invoice generated successfully.");
    }

    [Fact]
    public void Invoice_Should_Have_Default_Values()
    {
        var invoice = new Invoice();
        invoice.InvoiceId.Should().Be(Guid.Empty);
        invoice.BookingId.Should().Be(Guid.Empty);
        invoice.InvoiceNumber.Should().BeEmpty();
        invoice.PaymentId.Should().Be(Guid.Empty);
        invoice.InvoiceDate.Should().Be(default);
        invoice.SubTotal.Should().Be(0);
        invoice.TaxAmount.Should().Be(0);
        invoice.DiscountAmount.Should().Be(0);
        invoice.TotalAmount.Should().Be(0);
        invoice.InvoiceStatus.Should().BeEmpty();
        invoice.PdfUrl.Should().BeEmpty();
        invoice.Remarks.Should().BeEmpty();
    }
}