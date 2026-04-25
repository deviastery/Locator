import Button from '@mui/material/Button';

interface ResetFilterButtonProps {
    onClick: () => void;
}

export const ResetFilterButton = ({ onClick }: ResetFilterButtonProps) => {
    return (
        <Button
            variant="outlined"
            fullWidth
            onClick={onClick}
            sx={{
                color: 'white',
                borderColor: 'white',
                fontWeight: 600,
                fontSize: '1rem',
                py: 1.5,
                borderRadius: 2,
                textTransform: 'none',
                mt: 2,
                '&:hover': {
                    borderColor: 'white',
                    bgcolor: 'rgba(255, 255, 255, 0.1)',
                },
            }}
        >
            Сбросить фильтры
        </Button>
    );
};
