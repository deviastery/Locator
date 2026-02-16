import Button from '@mui/material/Button';
import Tooltip from '@mui/material/Tooltip';

interface CreateReviewButtonProps {
    canLeaveReview: boolean;
    onClick: () => void;
}

export const CreateReviewButton = ({ canLeaveReview, onClick }: CreateReviewButtonProps) => {
    return (
        <Tooltip
            title={canLeaveReview ? '' : 'С момента отклика прошло менее 5 дней'}
            arrow
        >
            <span>
                <Button
                    variant="contained"
                    onClick={onClick}
                    disabled={!canLeaveReview}
                    sx={{
                        bgcolor: canLeaveReview ? '#0f0c5f' : '#9e9e9e',
                        color: 'white',
                        borderRadius: 3,
                        px: 3,
                        py: 1,
                        textTransform: 'none',
                        fontWeight: 600,
                        ml: 2,
                        whiteSpace: 'nowrap',
                        '&:hover': {
                            bgcolor: canLeaveReview ? '#0b0948' : '#9e9e9e',
                        },
                        '&.Mui-disabled': {
                            bgcolor: '#9e9e9e',
                            color: 'white',
                        },
                    }}
                >
                    Оставить отзыв
                </Button>
            </span>
        </Tooltip>
    );
};
