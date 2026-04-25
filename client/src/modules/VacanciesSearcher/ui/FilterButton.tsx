import IconButton from '@mui/material/IconButton';
import MenuIcon from '@mui/icons-material/Menu';

interface FilterButtonProps {
    onClick: () => void;
}

export const FilterButton = ({ onClick }: FilterButtonProps) => {
    return (
        <IconButton
            onClick={onClick}
            sx={{
                bgcolor: '#0f0c5f',
                color: 'white',
                borderRadius: '12px 0 0 12px',
                width: 56,
                height: 56,
                '&:hover': {
                    bgcolor: '#0b0948',
                },
            }}
        >
            <MenuIcon />
        </IconButton>
    );
};
