using FBS.Application.Common.Result;
using MediatR;

namespace FBS.Application.Queries.GetFlightByNumber;

public record GetFlightByNumberQuery(string FlightNumber) : IRequest<Result<FlightDetailsDto>>;
