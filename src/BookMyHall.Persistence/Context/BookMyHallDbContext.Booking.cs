using Microsoft.EntityFrameworkCore;
using BookMyHall.Domain.Booking;

namespace BookMyHall.Persistence.Context;

public partial class BookMyHallDbContext
{
    public DbSet<Bookings> Bookings => Set<Bookings>();
    public DbSet<BookingGuest> BookingGuests => Set<BookingGuest>();
    public DbSet<BookingEvent> BookingEvents => Set<BookingEvent>();
    public DbSet<BookingPaymentSchedule> BookingPaymentSchedules => Set<BookingPaymentSchedule>();
    public DbSet<BookingStatusHistory> BookingStatusHistories => Set<BookingStatusHistory>();
    public DbSet<BookingCancellation> BookingCancellations => Set<BookingCancellation>();
    public DbSet<BookingTimeline> BookingTimelines => Set<BookingTimeline>();
    public DbSet<BookingDocument> BookingDocuments => Set<BookingDocument>();
    public DbSet<BookingChecklist> BookingChecklists => Set<BookingChecklist>();
    public DbSet<BookingReminder> BookingReminders => Set<BookingReminder>();
    public DbSet<BookingNote> BookingNotes => Set<BookingNote>();
}