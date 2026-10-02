namespace BookMyHall.Contracts.Messaging;

public sealed record VendorImageUploadedMessage(
    Guid VendorImageId,
    Guid VendorId,
    string ObjectKey);