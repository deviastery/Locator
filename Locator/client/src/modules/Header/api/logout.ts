import type { ErrorResponse } from "../../../contracts/responses/ErrorResponse.ts";

export const logout = async (): Promise<{ success: boolean; error?: string }> => {
    try {
        const response = await fetch('http://localhost:5003/api/users/auth/logout', {
            method: 'POST',
            credentials: 'include',
            headers: {
                'Content-Type': 'application/json',
            },
        });

        if (!response.ok) {
            const errorData: ErrorResponse[] = await response.json().catch(() => []);
            console.error('Failed to logout', response.status, errorData);

            const firstError = errorData[0];

            return {
                success: false,
                error: firstError?.message || 'Не удалось выйти из системы'
            };
        }

        return { success: true };
    } catch (e) {
        console.error('Error during logout', e);
        return {
            success: false,
            error: 'Произошла ошибка при выходе'
        };
    }
};