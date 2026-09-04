namespace BookMyHall.Contracts.Messaging;

public sealed record HallImageUploadedMessage(
    Guid HallImageId,
    Guid HallId,
    string ObjectKey);