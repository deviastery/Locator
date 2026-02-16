import Button from '@mui/material/Button';
import Typography from '@mui/material/Typography';

interface NegotiationsButtonProps {
    isActive: boolean;
    onClick: () => void;
}

export const NegotiationsButton = ({ isActive, onClick }: NegotiationsButtonProps) => {
    return (
        <Button
            color="inherit"
            onClick={onClick}
            sx={{
                borderRadius: 0,
                height: 1,
                minHeight: 0,
                px: 4,
                display: 'flex',
                alignItems: 'center',
                color: isActive ? '#0f0c5f' : 'white',
                bgcolor: isActive ? '#e7d8f8' : '#0f0c5f',
                '&:hover': {
                    bgcolor: isActive ? '#e7d8f8' : '#0b0948',
                },
            }}
        >
            <Typography variant="button">
                Мои отклики
            </Typography>
        </Button>
    );
};
