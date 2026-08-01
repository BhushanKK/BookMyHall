using AutoMapper;
using BookMyHall.Domain.Masters;
namespace BookMyHall.Application.Features.Master;

public sealed class PaymentModeProfile : Profile
{
    public PaymentModeProfile()
    {
        CreateMap<CreatePaymentModeCommand, PaymentMode>();

        CreateMap<UpdatePaymentModeCommand, PaymentMode>();

        CreateMap<PaymentMode, PaymentModeDto>();
    }
}