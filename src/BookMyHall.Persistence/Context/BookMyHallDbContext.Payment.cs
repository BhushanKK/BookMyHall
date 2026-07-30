using Microsoft.EntityFrameworkCore;
using BookMyHall.Domain.Payments;

namespace BookMyHall.Persistence.Context;

public partial class BookMyHallDbContext
{
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<PaymentTransaction> PaymentTransactions => Set<PaymentTransaction>();
    public DbSet<Refund> Refunds => Set<Refund>();
    public DbSet<Invoice> Invoices => Set<Invoice>();
    public DbSet<PaymentWebhook> PaymentWebhooks => Set<PaymentWebhook>();
}