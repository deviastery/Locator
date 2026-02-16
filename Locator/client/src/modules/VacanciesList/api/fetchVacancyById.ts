import type { VacancyResponse } from "../types/responses/VacancyResponse.ts";
import type { ErrorResponse } from "../../../contracts/responses/ErrorResponse.ts";

export const fetchVacancyById = 
    async (vacancyId: number): Promise<VacancyResponse | null> => {
    try {
        const response = await fetch(`https://localhost:5003/api/vacancies/${vacancyId}`, {
            method: 'GET',
            credentials: 'include',
            headers: {
                'Content-Type': 'application/json',
            },
        });

        if (!response.ok) {
            const errorData: ErrorResponse[] = await response.json().catch(() => []);
            console.error('Failed to fetch vacancy', response.status, errorData);

            const firstError = errorData[0];
            if (firstError?.message) {
                console.error('Error message:', firstError.message);
            }

            return null;
        }

        const data: VacancyResponse = await response.json();
        return data;
    } catch (e) {
        console.error('Error fetching vacancy', e);
        return null;
    }
};
