import IconButton from '@mui/material/IconButton';
import SatelliteAltIcon from '@mui/icons-material/SatelliteAlt';

interface HomeButtonProps {
    isActive: boolean;
    onClick: () => void;
}

export const HomeButton = ({ isActive, onClick }: HomeButtonProps) => {
    return (
        <IconButton
            edge="start"
            color="inherit"
            onClick={onClick}
            sx={{
                borderRadius: 0,
                width: 56,
                height: 1,
                ml: 0,
                color: isActive ? '#0f0c5f' : 'white',
                bgcolor: isActive ? '#e7d8f8' : '#0f0c5f',
                '&:hover': {
                    bgcolor: isActive ? '#e7d8f8' : '#0b0948',
                },
            }}
        >
            <SatelliteAltIcon />
        </IconButton>
    );
};
