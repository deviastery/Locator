import Button from '@mui/material/Button';

interface SubmitButtonProps {
    onClick: () => void;
    loading: boolean;
    success: boolean;
}

export const SubmitButton = ({ onClick, loading, success }: SubmitButtonProps) => {
    return (
        <Button
            variant="contained"
            onClick={onClick}
            disabled={loading || success}
            sx={{
                bgcolor: loading || success ? '#9e9e9e' : '#0f0c5f',
                color: 'white',
                px: 4,
                py: 1.5,
                borderRadius: 3,
                textTransform: 'none',
                fontSize: '1rem',
                fontWeight: 600,
                '&:hover': {
                    bgcolor: loading || success ? '#757575' : '#0b0948',
                },
                '&.Mui-disabled': {
                    bgcolor: '#9e9e9e',
                    color: 'white',
                },
            }}
        >
            {loading ? 'Отправка...' : 'Отправить'}
        </Button>
    );
};
