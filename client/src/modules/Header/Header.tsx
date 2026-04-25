import AppBar from '@mui/material/AppBar';
import Toolbar from '@mui/material/Toolbar';
import Box from '@mui/material/Box';
import { useNavigate, useLocation } from 'react-router-dom';
import { useState } from 'react';
import { logout } from './api/logout';
import { HomeButton } from "./ui/HomeButton.tsx";
import { NegotiationsButton } from './ui/NegotiationsButton.tsx';
import { LogoutButton } from './ui/LogoutButton.tsx';

export const Header = () => {
    const navigate = useNavigate();
    const location = useLocation();
    const [isLoggingOut, setIsLoggingOut] = useState(false);

    const isHome = location.pathname === '/vacancies';
    const isNegotiations = location.pathname === '/negotiations';

    const handleHomeClick = () => {
        localStorage.setItem('vacanciesPage', '1');
        navigate('/vacancies?page=1');
    };

    const handleLogout = async () => {
        const confirmed = window.confirm('Вы уверены, что хотите выйти?');
        if (confirmed) {
            setIsLoggingOut(true);
            const result = await logout();
            setIsLoggingOut(false);

            if (result.success) {
                navigate('/auth');
            } else {
                alert(result.error || 'Не удалось выйти из системы');
            }
        }
    };

    return (
        <AppBar
            position="static"
            sx={{
                bgcolor: '#0f0c5f',
                boxShadow: 'none',
            }}
        >
            <Toolbar disableGutters sx={{ height: 48, minHeight: 48, px: 0, py: 0 }}>
                <HomeButton
                    isActive={isHome}
                    onClick={handleHomeClick}
                />

                <Box sx={{ flexGrow: 1 }} />

                <NegotiationsButton
                    isActive={isNegotiations}
                    onClick={() => navigate('/negotiations')}
                />

                <LogoutButton
                    isLoading={isLoggingOut}
                    onClick={handleLogout}
                />
            </Toolbar>
        </AppBar>
    );
};