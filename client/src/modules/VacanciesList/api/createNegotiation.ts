import type { ErrorResponse } from "../../../contracts/responses/ErrorResponse.ts";

export const createNegotiation = 
    async (vacancyId: number): Promise<{ success: boolean; error?: string }> => {
    try {
        const response = await fetch(`http://localhost:5003/api/vacancies/${vacancyId}/negotiations`, {
            method: 'POST',
            credentials: 'include',
            headers: {
                'Content-Type': 'application/json',
            },
        });

        if (!response.ok) {
            const errorData: ErrorResponse[] = await response.json().catch(() => []);
            console.error('Failed to create negotiation', response.status, errorData);

            const firstError = errorData[0];

            return {
                success: false,
                error: firstError?.message || 'Не удалось отправить отклик'
            };
        }
        
        return { success: true };
    } catch (e) {
        console.error('Error creating negotiation', e);
        return {
            success: false,
            error: 'Произошла ошибка при отправке отклика'
        };
    }
};