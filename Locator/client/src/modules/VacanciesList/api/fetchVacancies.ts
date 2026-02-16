import type { GetVacanciesDto } from "../../../contracts/dtos/GetVacanciesDto.ts";
import type { VacanciesFetchResult } from "../../../contracts/responses/VacanciesResponse.ts";
import type { ErrorResponse } from "../../../contracts/responses/ErrorResponse.ts";
import { handleUnauthorized } from "../../../utils/handleUnauthorized.ts";

export const fetchVacancies = 
    async (params: GetVacanciesDto = {}): Promise<VacanciesFetchResult> => {
    try {
        const queryParams = new URLSearchParams();

        if (params.searchField) queryParams.append('searchField', params.searchField);
        if (params.experience) queryParams.append('experience', params.experience);
        if (params.employment) queryParams.append('employment', params.employment);
        if (params.schedule) queryParams.append('schedule', params.schedule);
        if (params.area) queryParams.append('area', params.area.toString());
        if (params.perPage) queryParams.append('perPage', params.perPage.toString());
        if (params.pages) queryParams.append('pages', params.pages.toString());
        if (params.page) queryParams.append('page', params.page.toString());
        if (params.salary?.from) queryParams.append('salary.from', params.salary.from.toString());
        if (params.salary?.to) queryParams.append('salary.to', params.salary.to.toString());
        if (params.salary?.currency) queryParams.append('salary.currency', params.salary.currency);

        const url = `https://localhost:5003/api/vacancies?${queryParams.toString()}`;

        const response = await fetch(url, {
            method: 'GET',
            credentials: 'include',
            headers: {
                'Content-Type': 'application/json',
            },
        });

        if (response.status === 401) {
            handleUnauthorized();
            return { unauthorized: true };
        }

        if (!response.ok) {
            const errorData: ErrorResponse[] = await response.json().catch(() => []);
            console.error('Failed to fetch vacancies', response.status, errorData);

            const firstError = errorData[0];
            if (firstError?.message) {
                console.error('Error message:', firstError.message);
            }

            return null;
        }

        const data = await response.json();
        return data;
    } catch (e) {
        console.error('Error fetching vacancies', e);
        return null;
    }
};
