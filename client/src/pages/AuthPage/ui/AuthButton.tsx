import Button from '@mui/material/Button';
import { useNavigate } from 'react-router-dom';
import { authUser } from '../api/authUser';

export const AuthButton = () => {
    const navigate = useNavigate();

    const handleLogin = async () => {
        const success = await authUser();
        if (success) {
            navigate('/vacancies?page=1');
        }
    };

    return (
        <Button
            variant="contained"
            onClick={handleLogin}
            sx={{
                bgcolor: 'white',
                color: '#0b0948',
                px: 4,
                py: 1,
                borderRadius: 3,
                textTransform: 'none',
                fontSize: '1.1rem',
                fontWeight: 600,
                '&:hover': {
                    bgcolor: '#a6a6a6',
                },
            }}
        >
            Войти
            <br />
            с помощью HeadHunter
        </Button>
    );
};
