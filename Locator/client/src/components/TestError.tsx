import { useState } from 'react';
import Button from '@mui/material/Button';
import Box from '@mui/material/Box';

export const TestError = () => {
    const [shouldThrow, setShouldThrow] = useState(false);

    if (shouldThrow) {
        throw new Error('Это тестовая ошибка для проверки ErrorBoundary');
    }

    return (
        <Box sx={{ p: 3 }}>
            <Button
                variant="contained"
                color="error"
                onClick={() => setShouldThrow(true)}
            >
                Вызвать ошибку
            </Button>
        </Box>
    );
};
