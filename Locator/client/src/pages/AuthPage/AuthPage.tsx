import Box from '@mui/material/Box';
import { AuthButton } from './ui/AuthButton';

export const AuthPage = () => {
    return (
        <Box
            sx={{
                display: 'flex',
                alignItems: 'center',
                justifyContent: 'center',
                minHeight: '100vh',
                bgcolor: '#0f0c5f',
            }}
        >
            <AuthButton/>
        </Box>
    );
}
