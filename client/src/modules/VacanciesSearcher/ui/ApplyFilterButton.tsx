import Button from '@mui/material/Button';

interface ApplyFilterButtonProps {
    onClick: () => void;
}

export const ApplyFilterButton = ({ onClick }: ApplyFilterButtonProps) => {
    return (
        <Button
            variant="contained"
            fullWidth
            onClick={onClick}
            sx={{
                bgcolor: 'white',
                color: '#0f0c5f',
                fontWeight: 700,
                fontSize: '1.1rem',
                py: 1.5,
                borderRadius: 2,
                textTransform: 'none',
                mt: 3,
                '&:hover': {
                    bgcolor: '#f0f0f0',
                },
            }}
        >
            Найти
        </Button>
    );
};
