import Box from '@mui/material/Box';
import Typography from '@mui/material/Typography';
import type { Review } from "../types/Review.ts";

interface ReviewsListProps {
    reviews: Review[];
}

export const ReviewsList = ({ reviews }: ReviewsListProps) => {
    if (reviews.length === 0) {
        return (
            <Typography variant="body2" color="text.secondary">
                Отзывов пока нет
            </Typography>
        );
    }

    return (
        <Box
            sx={{
                maxHeight: '200px',
                overflowY: 'auto',
                pr: 1,
                '&::-webkit-scrollbar': {
                    width: '8px',
                },
                '&::-webkit-scrollbar-track': {
                    bgcolor: '#f0f0f0',
                    borderRadius: '4px',
                },
                '&::-webkit-scrollbar-thumb': {
                    bgcolor: '#c0c0c0',
                    borderRadius: '4px',
                    '&:hover': {
                        bgcolor: '#a0a0a0',
                    },
                },
            }}
        >
            {reviews.map((review, index) => (
                <Box key={review.id} sx={{ mb: index < reviews.length - 1 ? 2 : 0 }}>
                    <Box sx={{ display: 'flex', gap: 1, mb: 0.5 }}>
                        <Typography variant="body2" sx={{ fontWeight: 600 }}>
                            {review.userName}
                        </Typography>
                        <Typography variant="body2" color="text.secondary">
                            Рейтинг вакансии {review.mark.toFixed(1)}
                        </Typography>
                    </Box>
                    {review.comment && (
                        <Typography variant="body2">
                            {review.comment}
                        </Typography>
                    )}
                </Box>
            ))}
        </Box>
    );
};
