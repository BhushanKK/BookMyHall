using BookMyHall.Domain.Entities.Identity;
using BookMyHall.Domain.Identity;
using Microsoft.EntityFrameworkCore;

namespace BookMyHall.Persistence.Context;

public partial class BookMyHallDbContext
{
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<User> Users => Set<User>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<OTP> OTPs => Set<OTP>();
    public DbSet<Device> Devices => Set<Device>();
    public DbSet<LoginHistory> LoginHistories => Set<LoginHistory>();
    public DbSet<UserSession> UserSessions => Set<UserSession>();
    public DbSet<PasswordResetToken> PasswordResetTokens => Set<PasswordResetToken>();
    public DbSet<EmailVerificationToken> EmailVerificationTokens => Set<EmailVerificationToken>();
    public DbSet<UserPreference> UserPreferences => Set<UserPreference>();
    public DbSet<Menu> Menus => Set<Menu>();
    public DbSet<MenuRolePermission> MenuRolePermissions => Set<MenuRolePermission>();
}