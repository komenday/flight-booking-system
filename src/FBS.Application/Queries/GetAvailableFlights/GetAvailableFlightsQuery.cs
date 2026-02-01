using FBS.Application.Common.Result;
using MediatR;

namespace FBS.Application.Queries.GetAvailableFlights;

public record GetAvailableFlightsQuery(string DepartureAirport, string ArrivalAirport, DateTime Date)
    : IRequest<Result<List<FlightSummaryDto>>>;
