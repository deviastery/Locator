import Button from '@mui/material/Button';

interface CancelButtonProps {
    onClick: () => void;
    disabled?: boolean;
}

export const CancelButton = ({ onClick, disabled = false }: CancelButtonProps) => {
    return (
        <Button
            variant="contained"
            onClick={onClick}
            disabled={disabled}
            sx={{
                bgcolor: '#ff293e',
                color: 'white',
                px: 4,
                py: 1.5,
                borderRadius: 3,
                textTransform: 'none',
                fontSize: '1rem',
                fontWeight: 600,
                '&:hover': {
                    bgcolor: '#f00017',
                },
            }}
        >
            Отмена
        </Button>
    );
};
