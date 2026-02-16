import type { NegotiationsFetchResult } from '../types/responses/NegotiationsResponse.ts';
import type { GetNegotiationsDto } from "../types/dtos/GetNegotiationsDto";
import type { ErrorResponse } from "../../../contracts/responses/ErrorResponse.ts";
import { handleUnauthorized } from "../../../utils/handleUnauthorized.ts";

export const fetchNegotiations = 
    async (params: GetNegotiationsDto = {}): Promise<NegotiationsFetchResult> => {
    try {
        const queryParams = new URLSearchParams();

        if (params.page) queryParams.append('page', params.page.toString());
        if (params.pages) queryParams.append('pages', params.pages.toString());
        if (params.perPage) queryParams.append('perPage', params.perPage.toString());

        const url = `https://localhost:5003/api/vacancies/negotiations?${queryParams.toString()}`;

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
            console.error('Failed to fetch negotiations', response.status, errorData);

            const firstError = errorData[0];
            if (firstError?.message) {
                console.error('Error message:', firstError.message);
            }

            return null;
        }

        const data = await response.json();
        return data;
    } catch (e) {
        console.error('Error fetching negotiations', e);
        return null;
    }
};