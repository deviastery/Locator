import type { CreateReviewDto } from '../types/dtos/CreateReviewDto';
import type { ErrorResponse } from '../../../contracts/responses/ErrorResponse';

export const createReview = async (
    vacancyId: string,
    reviewData: CreateReviewDto
): Promise<{ success: boolean; error?: string; errorCode?: string }> => {
    try {
        const response = await fetch(`http://localhost:5003/api/vacancies/${vacancyId}/reviews`, {
            method: 'POST',
            credentials: 'include',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(reviewData),
        });

        if (!response.ok) {
            const errorData: ErrorResponse[] = await response.json().catch(() => []);

            const firstError = errorData[0];

            if (firstError?.code === 'review.already.left') {
                return {
                    success: false,
                    error: 'Вы уже оставили отзыв на эту вакансию',
                    errorCode: firstError.code
                };
            }

            if (firstError?.code === 'not.ready.for.review') {
                return {
                    success: false,
                    error: 'С момента отклика прошло менее 5 дней',
                    errorCode: firstError.code
                };
            }

            return {
                success: false,
                error: firstError?.message || 'Не удалось отправить отзыв'
            };
        }

        return { success: true };
    } catch (e) {
        console.error('Error creating review', e);
        return {
            success: false,
            error: 'Произошла ошибка при отправке отзыва'
        };
    }
};