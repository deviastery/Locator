import IconButton from '@mui/material/IconButton';
import LogoutIcon from '@mui/icons-material/Logout';

interface LogoutButtonProps {
    isLoading: boolean;
    onClick: () => void;
}

export const LogoutButton = ({ isLoading, onClick }: LogoutButtonProps) => {
    return (
        <IconButton
            color="inherit"
            onClick={onClick}
            disabled={isLoading}
            sx={{
                borderRadius: 0,
                width: 56,
                height: 1,
                bgcolor: isLoading ? '#757575' : '#0f0c5f',
                mr: 0,
                transition: 'background-color 0.2s ease',
                '&:hover': {
                    bgcolor: '#0b0948',
                },
                '&.Mui-disabled': {
                    bgcolor: '#757575',
                    color: 'white',
                },
            }}
        >
            <LogoutIcon />
        </IconButton>
    );
};
