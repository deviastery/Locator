using System.Net;
using System.Text;
using System.Text.Json;
using CSharpFunctionalExtensions;
using Shared;
using Shared.Abstractions;
using Vacancies.Application.Fails;

namespace Vacancies.Application.CreateRequestVacancyRatingCommand;

public class CreateRequestVacancyRatingCommandHandler: ICommandHandler<Guid, CreateRequestVacancyRatingCommand>
{
    private readonly HttpClient _httpClient;
    
    public CreateRequestVacancyRatingCommandHandler(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<Result<Guid, Failure>> Handle(
        CreateRequestVacancyRatingCommand command, 
        CancellationToken cancellationToken)
    {
        // Create vacancy Rating
        var requestBody = new
        {
            averageMark = command.Dto.AverageMark,
        };

        string jsonRequest = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
        
        var request = new HttpRequestMessage(
            HttpMethod.Post, 
            $"https://localhost:5001/api/ratings/vacancies/{command.Dto.VacancyId}")
        {
            Content = content,
        };
        request.Headers.Add("User-Agent", "Locator/1.0");
        request.Headers.Add("Api-Gateway", "Signed");

        var response = await _httpClient.SendAsync(request, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return Errors.CreateVacancyRatingFail().ToFailure();
        }

        string jsonResponse = await response.Content.ReadAsStringAsync(cancellationToken);
        var ratingId = JsonSerializer.Deserialize<Guid>(jsonResponse);

        return ratingId;
    }
}