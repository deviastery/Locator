import type { ErrorResponse } from "../../../contracts/responses/ErrorResponse.ts";

export const authUser = async (): Promise<boolean> => {
    try {
        const response = await fetch('https://localhost:5003/api/users/auth', {
            method: 'POST',
            credentials: 'include',
            headers: {
                'Content-Type': 'application/json',
            },
        });

        if (!response.ok) {
            const errorData: ErrorResponse[] = await response.json().catch(() => []);
            console.error('Auth failed', response.status, errorData);

            const firstError = errorData[0];
            if (firstError?.message) {
                console.error('Error message:', firstError.message);
            }

            return false;
        }

        return true;
    } catch (e) {
        console.error('Auth error', e);
        return false;
    }
};
