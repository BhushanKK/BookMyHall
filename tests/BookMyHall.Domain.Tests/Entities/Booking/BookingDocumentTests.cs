using FluentAssertions;
using BookMyHall.Domain.Booking;

namespace BookMyHall.Domain.Tests.Entities.Bookings;

public sealed class BookingDocumentTests
{
    [Fact]
    public void BookingDocument_Should_Assign_BookingDocumentId()
    {
        var bookingDocument = new BookingDocument();
        var id = Guid.NewGuid();
        bookingDocument.BookingDocumentId = id;
        bookingDocument.BookingDocumentId.Should().Be(id);
    }

    [Fact]
    public void BookingDocument_Should_Assign_BookingId()
    {
        var bookingDocument = new BookingDocument();
        var bookingId = Guid.NewGuid();
        bookingDocument.BookingId = bookingId;
        bookingDocument.BookingId.Should().Be(bookingId);
    }

    [Fact]
    public void BookingDocument_Should_Assign_DocumentName()
    {
        var bookingDocument = new BookingDocument();
        bookingDocument.DocumentName = "Customer ID";
        bookingDocument.DocumentName.Should().Be("Customer ID");
    }

    [Fact]
    public void BookingDocument_Should_Assign_DocumentType()
    {
        var bookingDocument = new BookingDocument();
        bookingDocument.DocumentType = "Identity Proof";
        bookingDocument.DocumentType.Should().Be("Identity Proof");
    }

    [Fact]
    public void BookingDocument_Should_Assign_FileName()
    {
        var bookingDocument = new BookingDocument();
        bookingDocument.FileName = "aadhaar.pdf";
        bookingDocument.FileName.Should().Be("aadhaar.pdf");
    }

    [Fact]
    public void BookingDocument_Should_Assign_FileUrl()
    {
        var bookingDocument = new BookingDocument();
        bookingDocument.FileUrl = "https://example.com/documents/aadhaar.pdf";
        bookingDocument.FileUrl.Should().Be("https://example.com/documents/aadhaar.pdf");
    }

    [Fact]
    public void BookingDocument_Should_Assign_FileSize()
    {
        var bookingDocument = new BookingDocument();
        bookingDocument.FileSize = 204800;
        bookingDocument.FileSize.Should().Be(204800);
    }

    [Fact]
    public void BookingDocument_Should_Assign_ContentType()
    {
        var bookingDocument = new BookingDocument();
        bookingDocument.ContentType = "application/pdf";
        bookingDocument.ContentType.Should().Be("application/pdf");
    }

    [Fact]
    public void BookingDocument_Should_Assign_IsVerified()
    {
        var bookingDocument = new BookingDocument();
        bookingDocument.IsVerified = true;
        bookingDocument.IsVerified.Should().BeTrue();
    }

    [Fact]
    public void BookingDocument_Should_Assign_VerifiedBy()
    {
        var bookingDocument = new BookingDocument();
        var verifiedBy = Guid.NewGuid();
        bookingDocument.VerifiedBy = verifiedBy;
        bookingDocument.VerifiedBy.Should().Be(verifiedBy);
    }

    [Fact]
    public void BookingDocument_Should_Assign_VerifiedDate()
    {
        var bookingDocument = new BookingDocument();
        var verifiedDate = DateTimeOffset.UtcNow;
        bookingDocument.VerifiedDate = verifiedDate;
        bookingDocument.VerifiedDate.Should().Be(verifiedDate);
    }

    [Fact]
    public void BookingDocument_Should_Assign_All_Properties()
    {
        var bookingDocumentId = Guid.NewGuid();
        var bookingId = Guid.NewGuid();
        var verifiedBy = Guid.NewGuid();
        var verifiedDate = DateTimeOffset.UtcNow;
        var bookingDocument = new BookingDocument
        {
            BookingDocumentId = bookingDocumentId,
            BookingId = bookingId,
            DocumentName = "Customer ID",
            DocumentType = "Identity Proof",
            FileName = "aadhaar.pdf",
            FileUrl = "https://example.com/documents/aadhaar.pdf",
            FileSize = 204800,
            ContentType = "application/pdf",
            IsVerified = true,
            VerifiedBy = verifiedBy,
            VerifiedDate = verifiedDate
        };

        bookingDocument.BookingDocumentId.Should().Be(bookingDocumentId);
        bookingDocument.BookingId.Should().Be(bookingId);
        bookingDocument.DocumentName.Should().Be("Customer ID");
        bookingDocument.DocumentType.Should().Be("Identity Proof");
        bookingDocument.FileName.Should().Be("aadhaar.pdf");
        bookingDocument.FileUrl.Should().Be("https://example.com/documents/aadhaar.pdf");
        bookingDocument.FileSize.Should().Be(204800);
        bookingDocument.ContentType.Should().Be("application/pdf");
        bookingDocument.IsVerified.Should().BeTrue();
        bookingDocument.VerifiedBy.Should().Be(verifiedBy);
        bookingDocument.VerifiedDate.Should().Be(verifiedDate);
    }

    [Fact]
    public void BookingDocument_Should_Have_Default_Values()
    {
        var bookingDocument = new BookingDocument();
        bookingDocument.BookingDocumentId.Should().Be(Guid.Empty);
        bookingDocument.BookingId.Should().Be(Guid.Empty);
        bookingDocument.DocumentName.Should().BeEmpty();
        bookingDocument.DocumentType.Should().BeEmpty();
        bookingDocument.FileName.Should().BeEmpty();
        bookingDocument.FileUrl.Should().BeEmpty();
        bookingDocument.FileSize.Should().Be(0);
        bookingDocument.ContentType.Should().BeEmpty();
        bookingDocument.IsVerified.Should().BeFalse();
        bookingDocument.VerifiedBy.Should().Be(Guid.Empty);
        bookingDocument.VerifiedDate.Should().Be(default);
    }
}