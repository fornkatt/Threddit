using Threddit.Application.DTOs.Responses.Validation;

namespace Threddit.Application.Interfaces.Driving;

public interface IGetValidationLimitsUseCase
{
    GetValidationLimitsResponse Execute();
}