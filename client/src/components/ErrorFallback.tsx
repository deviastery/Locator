import Box from '@mui/material/Box';
import Typography from '@mui/material/Typography';
import Button from '@mui/material/Button';
import { useRouteError, useNavigate } from 'react-router-dom';

export const ErrorFallback = () => {
    const error = useRouteError() as Error;
    const navigate = useNavigate();

    const handleGoHome = () => {
        navigate('/vacancies');
    };

    const handleReload = () => {
        window.location.reload();
    };

    return (
        <Box
            sx={{
                display: 'flex',
                flexDirection: 'column',
                alignItems: 'center',
                justifyContent: 'center',
                minHeight: '100vh',
                bgcolor: '#0f0c5f',
                color: 'white',
                px: 3,
                textAlign: 'center',
            }}
        >
            <Typography variant="h3" sx={{ mb: 2, fontWeight: 700 }}>
                Что-то пошло не так
            </Typography>
            <Typography variant="body1" sx={{ mb: 4, maxWidth: 600 }}>
                Произошла непредвиденная ошибка. Попробуйте обновить страницу или вернуться на главную.
            </Typography>
            {error && (
                <Typography
                    variant="body2"
                    sx={{
                        mb: 4,
                        p: 2,
                        bgcolor: 'rgba(255, 255, 255, 0.1)',
                        borderRadius: 1,
                        fontFamily: 'monospace',
                        maxWidth: 800,
                        overflow: 'auto',
                    }}
                >
                    {error.toString()}
                </Typography>
            )}
            <Box sx={{ display: 'flex', gap: 2 }}>
                <Button
                    variant="contained"
                    onClick={handleReload}
                    sx={{
                        bgcolor: 'white',
                        color: '#0f0c5f',
                        '&:hover': { bgcolor: '#f0f0f0' },
                    }}
                >
                    Обновить страницу
                </Button>
                <Button
                    variant="outlined"
                    onClick={handleGoHome}
                    sx={{
                        borderColor: 'white',
                        color: 'white',
                        '&:hover': { borderColor: '#f0f0f0', bgcolor: 'rgba(255, 255, 255, 0.1)' },
                    }}
                >
                    На главную
                </Button>
            </Box>
        </Box>
    );
};
